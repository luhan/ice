// **********************************************************************
//
// Copyright (c) 2003-2006 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

#include <IceUtil/DisableWarnings.h>
#include <IceSSL/Plugin.h>
#include <IceSSL/Util.h>
#include <IceUtil/StaticMutex.h>

#include <openssl/x509v3.h>
#include <openssl/pem.h>

using namespace std;
using namespace Ice;
using namespace IceSSL;

const char* IceSSL::CertificateReadException::_name = "IceSSL::CertificateReadException";

CertificateReadException::CertificateReadException(const char* file, int line, const string& r) :
    Exception(file, line),
    reason(r)
{
}

const string
CertificateReadException::ice_name() const
{
    return _name;
}

Exception* 
CertificateReadException::ice_clone() const
{
    throw new CertificateReadException(*this);
    return 0; // For compiler warnings.
}

void
CertificateReadException::ice_throw() const
{
    throw *this;
}

const char* IceSSL::CertificateEncodingException::_name = "IceSSL::CertificateEncodingException";

CertificateEncodingException::CertificateEncodingException(const char* file, int line, const string& r) :
    Exception(file, line),
    reason(r)
{
}

const string
CertificateEncodingException::ice_name() const
{
    return _name;
}

Exception* 
CertificateEncodingException::ice_clone() const
{
    throw new CertificateEncodingException(*this);
    return 0; // For compiler warnings.
}

void
CertificateEncodingException::ice_throw() const
{
    throw *this;
}

static IceUtil::Time
ASMUtcTimeToIceUtilTime(const ASN1_UTCTIME* s)
{
    struct tm tm;
    int offset;
    
    memset(&tm, '\0', sizeof tm);
    
#define g2(p) (((p)[0]-'0')*10+(p)[1]-'0')
    tm.tm_year = g2(s->data);
    if(tm.tm_year < 50)
	tm.tm_year += 100;
    tm.tm_mon = g2(s->data + 2) - 1;
    tm.tm_mday = g2(s->data + 4);
    tm.tm_hour = g2(s->data + 6);
    tm.tm_min = g2(s->data + 8);
    tm.tm_sec = g2(s->data + 10);
    if(s->data[12] == 'Z')
    {
	offset = 0;
    }
    else
    {
	offset = g2(s->data + 13) * 60 + g2(s->data + 15);
	if(s->data[12] == '-')
	{
	    offset = -offset;
	}
    }
#undef g2

    //
    // If timegm was on all systems this code could be
    // return IceUtil::Time::seconds(timegm(&tm) - offset*60);
    //
    // Windows doesn't support the re-entrant _r versions.
    //
    time_t tzone;
    {
	static IceUtil::StaticMutex mutex = ICE_STATIC_MUTEX_INITIALIZER;
	IceUtil::StaticMutex::Lock sync(mutex);
	time_t now = time(0);
	tzone = mktime(localtime(&now)) - mktime(gmtime(&now));
    }
    return IceUtil::Time::seconds(mktime(&tm) - offset*60 + tzone);
}

static string
convertX509NameToString(X509_NAME* name)
{
    BIO* out = BIO_new(BIO_s_mem());
    X509_NAME_print_ex(out, name, 0, XN_FLAG_RFC2253);
    BUF_MEM* p;
    BIO_get_mem_ptr(out, &p);
    string result = string(p->data, p->length);
    BIO_free(out);
    return result;
}

static vector<pair<int, string> >
convertGeneralNames(GENERAL_NAMES* gens)
{
    vector<pair<int, string> > alt;
    if(gens == 0)
    {
	return alt;
    }
    for(int i = 0; i < sk_GENERAL_NAME_num(gens); ++i)
    {
	GENERAL_NAME* gen = sk_GENERAL_NAME_value(gens, i);
	pair<int, string> p;
	p.first = gen->type;
	switch(gen->type)
	{
	case GEN_EMAIL:
	{
	    ASN1_IA5STRING* str = gen->d.rfc822Name;
	    if(str && str->type == V_ASN1_IA5STRING && str->data && str->length > 0)
	    {
		p.second = reinterpret_cast<const char*>(str->data);
	    }
	    break;
	}
	case GEN_DNS:
	{
	    ASN1_IA5STRING* str = gen->d.dNSName;
	    if(str && str->type == V_ASN1_IA5STRING && str->data && str->length > 0)
	    {
		p.second = reinterpret_cast<const char*>(str->data);
	    }
	    break;
	}
	case GEN_DIRNAME:
	{
	    p.second = convertX509NameToString(gen->d.directoryName);
	    break;
	}
	case GEN_URI:
	{
	    ASN1_IA5STRING* str = gen->d.uniformResourceIdentifier;
	    if(str && str->type == V_ASN1_IA5STRING && str->data && str->length > 0)
	    {
		p.second = reinterpret_cast<const char*>(str->data);
	    }
	    break;
	}
	case GEN_IPADD:
	{
	    ASN1_OCTET_STRING* addr = gen->d.iPAddress;
	    // TODO: Support IPv6 someday.
	    if(addr && addr->type == V_ASN1_OCTET_STRING && addr->data && addr->length == 4)
	    {
		ostringstream ostr;
		for(int j = 0; j < 4; ++j)
		{
		    if(j > 0)
		    {
			ostr << '.';
		    }
		    ostr << static_cast<int>(addr->data[j]);
		}
		p.second = ostr.str();
	    }
	    break;
	}
	case GEN_OTHERNAME:
	case GEN_EDIPARTY:
	case GEN_X400:
	case GEN_RID:
	{
	    //
	    // TODO: These types are not supported. If the user wants
	    // them, they have to get at the certificate data. Another
	    // alternative is to DER encode the data (as the Java
	    // certificate does).
	    //
	    break;
	}
	}
	alt.push_back(p);
    }
    sk_GENERAL_NAME_pop_free(gens, GENERAL_NAME_free);
    return alt;
}

PublicKey::PublicKey(EVP_PKEY* key) :
    _key(key)
{
}

PublicKey::~PublicKey()
{
    EVP_PKEY_free(_key);
}

EVP_PKEY*
PublicKey::key() const
{
    return _key;
}

//
// The caller is responsible for incrementing the reference count.
//
Certificate::Certificate(X509* cert) :
    _cert(cert)
{
    assert(_cert != 0);
}

Certificate::~Certificate()
{
    X509_free(_cert);
}

CertificatePtr
Certificate::load(const string& file)
{
    BIO *cert = BIO_new(BIO_s_file());
    if(BIO_read_filename(cert, file.c_str()) <= 0)
    {
	BIO_free(cert);
	throw CertificateReadException(__FILE__, __LINE__, "error opening file");
    }
    
    X509* x = PEM_read_bio_X509_AUX(cert, NULL, NULL, NULL);
    if(x == NULL)
    {
	BIO_free(cert);
	throw CertificateReadException(__FILE__, __LINE__, "error reading file:\n" + getSslErrors(false));
    }
    BIO_free(cert);
    return new Certificate(x);
}

CertificatePtr
Certificate::decode(const string& encoding)
{
    BIO *cert = BIO_new_mem_buf((void*)&encoding[0], encoding.size());
    X509* x = PEM_read_bio_X509_AUX(cert, NULL, NULL, NULL);
    if(x == NULL)
    {
	BIO_free(cert);
	throw CertificateReadException(__FILE__, __LINE__, "error decoding certificate:\n" + getSslErrors(false));
    }
    BIO_free(cert);
    return new Certificate(x);
}

bool
Certificate::operator==(const Certificate& other) const
{
    return X509_cmp(_cert, other._cert) == 0;
}

bool
Certificate::operator!=(const Certificate& other) const
{
    return X509_cmp(_cert, other._cert) != 0;
}

PublicKeyPtr
Certificate::getPublicKey() const
{
    return new PublicKey(X509_get_pubkey(_cert));
}

bool
Certificate::verify(const PublicKeyPtr& key) const
{
    return X509_verify(_cert, key->key()) > 0;
}

string
Certificate::encode() const
{
    BIO* out = BIO_new(BIO_s_mem());
    int i = PEM_write_bio_X509_AUX(out, _cert);
    if(i <= 0)
    {
	BIO_free(out);
	throw CertificateEncodingException(__FILE__, __LINE__, getSslErrors(false));
    }
    BUF_MEM* p;
    BIO_get_mem_ptr(out, &p);
    string result = string(p->data, p->length);
    BIO_free(out);
    return result;
}

bool
Certificate::checkValidity() const
{
    IceUtil::Time now = IceUtil::Time::now();
    return now > getNotBefore() && now <= getNotAfter();
}

bool
Certificate::checkValidity(const IceUtil::Time& now) const
{
    return now > getNotBefore() && now <= getNotAfter();
}

IceUtil::Time
Certificate::getNotAfter() const
{
    return ASMUtcTimeToIceUtilTime(X509_get_notAfter(_cert));
}

IceUtil::Time
Certificate::getNotBefore() const
{
    return ASMUtcTimeToIceUtilTime(X509_get_notBefore(_cert));
}

string
Certificate::getSerialNumber() const
{
    BIGNUM* bn = ASN1_INTEGER_to_BN(X509_get_serialNumber(_cert), 0);
    char* dec = BN_bn2dec(bn);
    string result = dec;
    OPENSSL_free(dec);
    BN_free(bn);

    return result;
}

//string
//Certificate::getSigAlgName() const
//{
//}

//string
//Certificate::getSigAlgOID() const
//{
//}

string
Certificate::getIssuerDN() const
{
    return convertX509NameToString(X509_get_issuer_name(_cert));
}

vector<pair<int, string> >
Certificate::getIssuerAlternativeNames()
{
    return convertGeneralNames(reinterpret_cast<GENERAL_NAMES*>(
    	X509_get_ext_d2i(_cert, NID_issuer_alt_name, 0, 0)));
}

string
Certificate::getSubjectDN() const
{
    return convertX509NameToString(X509_get_subject_name(_cert));
}

vector<pair<int, string> >
Certificate::getSubjectAlternativeNames()
{
    return convertGeneralNames(
    	reinterpret_cast<GENERAL_NAMES*>(X509_get_ext_d2i(_cert, NID_subject_alt_name, 0, 0)));
}

int
Certificate::getVersion() const
{
    return X509_get_version(_cert);
}

string
Certificate::toString() const
{
    ostringstream os;
    os << "serial: " + getSerialNumber() << "\n";
    os << "issuer: " + getIssuerDN() << "\n";
    os << "subject: " + getSubjectDN() << "\n";
    os << "notBefore: " + getNotBefore().toDateTime() << "\n";
    os << "notAfter: " + getNotAfter().toDateTime();

    return os.str();
}

X509*
Certificate::getCert() const
{
    return _cert;
}
