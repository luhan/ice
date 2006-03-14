// **********************************************************************
//
// Copyright (c) 2003-2006 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

using System;

class Twoways
{
    private static void test(bool b)
    {
	if(!b)
	{
	    throw new System.Exception();
	}
    }
    
    internal static void twoways(Ice.Communicator communicator, Test.MyClassPrx p)
    {
	{
	    p.opVoid();
	}
	
	{
	    byte b;
	    byte r;
	    
	    r = p.opByte(0xff, 0x0f, out b);
	    test(b == 0xf0);
	    test(r == 0xff);
	}
	
	{
	    bool b;
	    bool r;
	    
	    r = p.opBool(true, false, out b);
	    test(b);
	    test(!r);
	}
	
	{
	    short s;
	    int i;
	    long l;
	    long r;
	    
	    r = p.opShortIntLong(10, 11, 12L, out s, out i, out l);
	    test(s == 10);
	    test(i == 11);
	    test(l == 12);
	    test(r == 12L);
	    
	    r = p.opShortIntLong(Int16.MinValue, Int32.MinValue, Int64.MinValue, out s, out i, out l);
	    test(s == Int16.MinValue);
	    test(i == Int32.MinValue);
	    test(l == Int64.MinValue);
	    test(r == Int64.MinValue);
	    
	    r = p.opShortIntLong(Int16.MaxValue, Int32.MaxValue, Int64.MaxValue, out s, out i, out l);
	    test(s == Int16.MaxValue);
	    test(i == Int32.MaxValue);
	    test(l == Int64.MaxValue);
	    test(r == Int64.MaxValue);
	}
	
	{
	    float f;
	    double d;
	    double r;
	    
	    r = p.opFloatDouble(3.14f, 1.1e10, out f, out d);
	    test(f == 3.14f);
	    test(d == 1.1e10);
	    test(r == 1.1e10);
	    
	    r = p.opFloatDouble(Single.Epsilon, Double.MinValue, out f, out d);
	    test(f == Single.Epsilon);
	    test(d == Double.MinValue);
	    test(r == Double.MinValue);
	    
	    r = p.opFloatDouble(Single.MaxValue, Double.MaxValue, out f, out d);
	    test(f == Single.MaxValue);
	    test(d == Double.MaxValue);
	    test(r == Double.MaxValue);
	}
	
	{
	    string s;
	    string r;
	    
	    r = p.opString("hello", "world", out s);
	    test(s.Equals("world hello"));
	    test(r.Equals("hello world"));
	}
	
	{
	    Test.MyEnum e;
	    Test.MyEnum r;
	    
	    r = p.opMyEnum(Test.MyEnum.enum2, out e);
	    test(e == Test.MyEnum.enum2);
	    test(r == Test.MyEnum.enum3);
	}
	
	{
	    Test.MyClassPrx c1;
	    Test.MyClassPrx c2;
	    Test.MyClassPrx r;
	    
	    r = p.opMyClass(p, out c1, out c2);
	    test(Ice.Util.proxyIdentityAndFacetCompare(c1, p) == 0);
	    test(Ice.Util.proxyIdentityAndFacetCompare(c2, p) != 0);
	    test(Ice.Util.proxyIdentityAndFacetCompare(r, p) == 0);
	    test(c1.ice_getIdentity().Equals(Ice.Util.stringToIdentity("test")));
	    test(c2.ice_getIdentity().Equals(Ice.Util.stringToIdentity("noSuchIdentity")));
	    test(r.ice_getIdentity().Equals(Ice.Util.stringToIdentity("test")));
	    r.opVoid();
	    c1.opVoid();
	    try
	    {
		c2.opVoid();
		test(false);
	    }
	    catch(Ice.ObjectNotExistException)
	    {
	    }
	    
	    r = p.opMyClass(null, out c1, out c2);
	    test(c1 == null);
	    test(c2 != null);
	    test(Ice.Util.proxyIdentityAndFacetCompare(r, p) == 0);
	    r.opVoid();
	}
	
	{
	    Test.Structure si1 = new Test.Structure();
	    si1.p = p;
	    si1.e = Test.MyEnum.enum3;
	    si1.s = new Test.AnotherStruct();
	    si1.s.s = "abc";
	    Test.Structure si2 = new Test.Structure();
	    si2.p = null;
	    si2.e = Test.MyEnum.enum2;
	    si2.s = new Test.AnotherStruct();
	    si2.s.s = "def";
	    
	    Test.Structure so;
	    Test.Structure rso = p.opStruct(si1, si2, out so);
	    test(rso.p == null);
	    test(rso.e == Test.MyEnum.enum2);
	    test(rso.s.s.Equals("def"));
	    test(so.p.Equals(p));
	    test(so.e == Test.MyEnum.enum3);
	    test(so.s.s.Equals("a new string"));
	    so.p.opVoid();
	}
	
	{
	    byte[] tbsi1 = new byte[] { 0x01, 0x11, 0x12, 0x22 };
	    Test.ByteS bsi1 = new Test.ByteS(tbsi1);
	    byte[] tbsi2 = new byte[] { 0xf1, 0xf2, 0xf3, 0xf4 };
	    Test.ByteS bsi2 = new Test.ByteS(tbsi2);
	    
	    Test.ByteS bso;
	    Test.ByteS rso;
	    
	    rso = p.opByteS(bsi1, bsi2, out bso);
	    test(bso.Count == 4);
	    test(bso[0] == 0x22);
	    test(bso[1] == 0x12);
	    test(bso[2] == 0x11);
	    test(bso[3] == 0x01);
	    test(rso.Count == 8);
	    test(rso[0] == 0x01);
	    test(rso[1] == 0x11);
	    test(rso[2] == 0x12);
	    test(rso[3] == 0x22);
	    test(rso[4] == 0xf1);
	    test(rso[5] == 0xf2);
	    test(rso[6] == 0xf3);
	    test(rso[7] == 0xf4);
	}
	
	{
	    bool[] tbsi1 = new bool[] { true, true, false };
	    Test.BoolS bsi1 = new Test.BoolS(tbsi1);
	    bool[] tbsi2 = new bool[] { false };
	    Test.BoolS bsi2 = new Test.BoolS(tbsi2);
	    
	    Test.BoolS bso;
	    Test.BoolS rso;
	    
	    rso = p.opBoolS(bsi1, bsi2, out bso);
	    test(bso.Count == 4);
	    test(bso[0]);
	    test(bso[1]);
	    test(!bso[2]);
	    test(!bso[3]);
	    test(rso.Count == 3);
	    test(!rso[0]);
	    test(rso[1]);
	    test(rso[2]);
	}
	
	{
	    short[] tssi = new short[] { 1, 2, 3 };
	    Test.ShortS ssi = new Test.ShortS(tssi);
	    int[] tisi = new int[] { 5, 6, 7, 8 };
	    Test.IntS isi = new Test.IntS(tisi);
	    long[] tlsi = new long[] { 10, 30, 20 };
	    Test.LongS lsi = new Test.LongS(tlsi);
	    
	    Test.ShortS sso;
	    Test.IntS iso;
	    Test.LongS lso;
	    Test.LongS rso;
	    
	    rso = p.opShortIntLongS(ssi, isi, lsi, out sso, out iso, out lso);
	    test(sso.Count == 3);
	    test(sso[0] == 1);
	    test(sso[1] == 2);
	    test(sso[2] == 3);
	    test(iso.Count == 4);
	    test(iso[0] == 8);
	    test(iso[1] == 7);
	    test(iso[2] == 6);
	    test(iso[3] == 5);
	    test(lso.Count == 6);
	    test(lso[0] == 10);
	    test(lso[1] == 30);
	    test(lso[2] == 20);
	    test(lso[3] == 10);
	    test(lso[4] == 30);
	    test(lso[5] == 20);
	    test(rso.Count == 3);
	    test(rso[0] == 10);
	    test(rso[1] == 30);
	    test(rso[2] == 20);
	}
	
	{
	    float[] tfsi = new float[] { 3.14f, 1.11f };
	    Test.FloatS fsi = new Test.FloatS(tfsi);
	    double[] tdsi = new double[] { 1.1e10, 1.2e10, 1.3e10 };
	    Test.DoubleS dsi = new Test.DoubleS(tdsi);
	    
	    Test.FloatS fso;
	    Test.DoubleS dso;
	    Test.DoubleS rso;
	    
	    rso = p.opFloatDoubleS(fsi, dsi, out fso, out dso);
	    test(fso.Count == 2);
	    test(fso[0] == 3.14f);
	    test(fso[1] == 1.11f);
	    test(dso.Count == 3);
	    test(dso[0] == 1.3e10);
	    test(dso[1] == 1.2e10);
	    test(dso[2] == 1.1e10);
	    test(rso.Count == 5);
	    test(rso[0] == 1.1e10);
	    test(rso[1] == 1.2e10);
	    test(rso[2] == 1.3e10);
	    test((float)rso[3] == 3.14f);
	    test((float)rso[4] == 1.11f);
	}
	
	{
	    string[] tssi1 = new string[] { "abc", "de", "fghi" };
	    Test.StringS ssi1 = new Test.StringS(tssi1);
	    string[] tssi2 = new string[] { "xyz" };
	    Test.StringS ssi2 = new Test.StringS(tssi2);
	    
	    Test.StringS sso;
	    Test.StringS rso;
	    
	    rso = p.opStringS(ssi1, ssi2, out sso);
	    test(sso.Count == 4);
	    test(sso[0].Equals("abc"));
	    test(sso[1].Equals("de"));
	    test(sso[2].Equals("fghi"));
	    test(sso[3].Equals("xyz"));
	    test(rso.Count == 3);
	    test(rso[0].Equals("fghi"));
	    test(rso[1].Equals("de"));
	    test(rso[2].Equals("abc"));
	}
	
	{
	    Test.ByteS s11 = new Test.ByteS(new byte[] { 0x01, 0x11, 0x12 });
	    Test.ByteS s12 = new Test.ByteS(new byte[] { 0xff });
	    Test.ByteSS bsi1 = new Test.ByteSS(new Test.ByteS[] { s11, s12 });

	    Test.ByteS s21 = new Test.ByteS(new byte[] { 0x0e });
	    Test.ByteS s22 = new Test.ByteS(new byte[] { 0xf2, 0xf1 });
	    Test.ByteSS bsi2 = new Test.ByteSS(new Test.ByteS[] { s21, s22 });
	    
	    Test.ByteSS bso;
	    Test.ByteSS rso;
	    
	    rso = p.opByteSS(bsi1, bsi2, out bso);
	    test(bso.Count == 2);
	    test(bso[0].Count == 1);
	    test(bso[0][0] == 0xff);
	    test(bso[1].Count == 3);
	    test(bso[1][0] == 0x01);
	    test(bso[1][1] == 0x11);
	    test(bso[1][2] == 0x12);
	    test(rso.Count == 4);
	    test(rso[0].Count == 3);
	    test(rso[0][0] == 0x01);
	    test(rso[0][1] == 0x11);
	    test(rso[0][2] == 0x12);
	    test(rso[1].Count == 1);
	    test(rso[1][0] == 0xff);
	    test(rso[2].Count == 1);
	    test(rso[2][0] == 0x0e);
	    test(rso[3].Count == 2);
	    test(rso[3][0] == 0xf2);
	    test(rso[3][1] == 0xf1);
	}
	
	{
	    Test.FloatS f11 = new Test.FloatS(new float[] { 3.14f });
	    Test.FloatS f12 = new Test.FloatS(new float[] { 1.11f });
	    Test.FloatS f13 = new Test.FloatS(new float[] { });
	    Test.FloatSS fsi = new Test.FloatSS(new Test.FloatS[] { f11, f12, f13 });

	    Test.DoubleS d11 = new Test.DoubleS(new double[] { 1.1e10, 1.2e10, 1.3e10 });
	    Test.DoubleSS dsi = new Test.DoubleSS(new Test.DoubleS[] { d11 });
	    
	    Test.FloatSS fso;
	    Test.DoubleSS dso;
	    Test.DoubleSS rso;
	    
	    rso = p.opFloatDoubleSS(fsi, dsi, out fso, out dso);
	    test(fso.Count == 3);
	    test(fso[0].Count == 1);
	    test(fso[0][0] == 3.14f);
	    test(fso[1].Count == 1);
	    test(fso[1][0] == 1.11f);
	    test(fso[2].Count == 0);
	    test(dso.Count == 1);
	    test(dso[0].Count == 3);
	    test(dso[0][0] == 1.1e10);
	    test(dso[0][1] == 1.2e10);
	    test(dso[0][2] == 1.3e10);
	    test(rso.Count == 2);
	    test(rso[0].Count == 3);
	    test(rso[0][0] == 1.1e10);
	    test(rso[0][1] == 1.2e10);
	    test(rso[0][2] == 1.3e10);
	    test(rso[1].Count == 3);
	    test(rso[1][0] == 1.1e10);
	    test(rso[1][1] == 1.2e10);
	    test(rso[1][2] == 1.3e10);
	}
	
	{
	    Test.StringS s11 = new Test.StringS();
	    s11.Add("abc");
	    Test.StringS s12 = new Test.StringS();
	    s12.Add("de");
	    s12.Add("fghi");
	    Test.StringSS ssi1 = new Test.StringSS();
	    ssi1.Add(s11);
	    ssi1.Add(s12);

	    Test.StringS s21 = new Test.StringS();
	    Test.StringS s22 = new Test.StringS();
	    Test.StringS s23 = new Test.StringS();
	    s23.Add("xyz");
	    Test.StringSS ssi2 = new Test.StringSS();
	    ssi2.Add(s21);
	    ssi2.Add(s22);
	    ssi2.Add(s23);

	    Test.StringSS sso;
	    Test.StringSS rso;
	    
	    rso = p.opStringSS(ssi1, ssi2, out sso);
	    test(sso.Count == 5);
	    test(sso[0].Count == 1);
	    test(sso[0][0].Equals("abc"));
	    test(sso[1].Count == 2);
	    test(sso[1][0].Equals("de"));
	    test(sso[1][1].Equals("fghi"));
	    test(sso[2].Count == 0);
	    test(sso[3].Count == 0);
	    test(sso[4].Count == 1);
	    test(sso[4][0].Equals("xyz"));
	    test(rso.Count == 3);
	    test(rso[0].Count == 1);
	    test(rso[0][0].Equals("xyz"));
	    test(rso[1].Count == 0);
	    test(rso[2].Count == 0);
	}
	
	{
	    Test.StringSS[] sssi1 = { new Test.StringSS(), new Test.StringSS() };
	    sssi1[0].Add(new Test.StringS());
	    sssi1[0].Add(new Test.StringS());
	    sssi1[0][0].Add("abc");
	    sssi1[0][0].Add("de");
	    sssi1[0][1].Add("xyz");
	    sssi1[1].Add(new Test.StringS());
	    sssi1[1][0].Add("hello");
	    Test.StringSS[] sssi2 = { new Test.StringSS(), new Test.StringSS(), new Test.StringSS() };
	    sssi2[0].Add(new Test.StringS());
	    sssi2[0].Add(new Test.StringS());
	    sssi2[0][0].Add("");
	    sssi2[0][0].Add("");
	    sssi2[0][1].Add("abcd");
	    sssi2[1].Add(new Test.StringS());
	    sssi2[1][0].Add("");

            Test.StringSS[] ssso;
	    Test.StringSS[] rsso = p.opStringSSS(sssi1, sssi2, out ssso);
	    test(ssso.Length == 5);
	    test(ssso[0].Count == 2);
	    test(ssso[0][0].Count == 2);
	    test(ssso[0][1].Count == 1);
	    test(ssso[1].Count == 1);
	    test(ssso[1][0].Count == 1);
	    test(ssso[2].Count == 2);
	    test(ssso[2][0].Count == 2);
	    test(ssso[2][1].Count == 1);
	    test(ssso[3].Count == 1);
	    test(ssso[3][0].Count == 1);
	    test(ssso[4].Count == 0);
	    test(ssso[0][0][0].Equals("abc"));
	    test(ssso[0][0][1].Equals("de"));
	    test(ssso[0][1][0].Equals("xyz"));
	    test(ssso[1][0][0].Equals("hello"));
	    test(ssso[2][0][0].Equals(""));
	    test(ssso[2][0][1].Equals(""));
	    test(ssso[2][1][0].Equals("abcd"));
	    test(ssso[3][0][0].Equals(""));

	    test(rsso.Length == 3);
	    test(rsso[0].Count == 0);
	    test(rsso[1].Count == 1);
	    test(rsso[1][0].Count == 1);
	    test(rsso[2].Count == 2);
	    test(rsso[2][0].Count == 2);
	    test(rsso[2][1].Count == 1);
	    test(rsso[1][0][0].Equals(""));
	    test(rsso[2][0][0].Equals(""));
	    test(rsso[2][0][1].Equals(""));
	    test(rsso[2][1][0].Equals("abcd"));
	}

	{
	    Test.ByteBoolD di1 = new Test.ByteBoolD();
	    di1[10] = true;
	    di1[100] = false;
	    Test.ByteBoolD di2 = new Test.ByteBoolD();
	    di2[10] = true;
	    di2[11] = false;
	    di2[101] = true;
	    
	    Test.ByteBoolD _do;
	    Test.ByteBoolD ro = p.opByteBoolD(di1, di2, out _do);
	    
	    test(_do.Equals(di1));
	    test(ro.Count == 4);
	    test(ro[10] == true);
	    test(ro[11] == false);
	    test(ro[100] == false);
	    test(ro[101] == true);
	}
	
	{
	    Test.ShortIntD di1 = new Test.ShortIntD();
	    di1[110] = -1;
	    di1[1100] = 123123;
	    Test.ShortIntD di2 = new Test.ShortIntD();
	    di2[110] = 1;
	    di2[111] = -100;
	    di2[1101] = 0;
	    
	    Test.ShortIntD _do;
	    Test.ShortIntD ro = p.opShortIntD(di1, di2, out _do);
	    
	    test(_do.Equals(di1));
	    test(ro.Count == 4);
	    test(ro[110] == -1);
	    test(ro[111] == -100);
	    test(ro[1100] == 123123);
	    test(ro[1101] == 0);
	}
	
	{
	    Test.LongFloatD di1 = new Test.LongFloatD();
	    di1[999999110L] = -1.1f;
	    di1[999999111L] = 123123.2f;
	    Test.LongFloatD di2 = new Test.LongFloatD();
	    di2[999999110L] = -1.1f;
	    di2[999999120L] = -100.4f;
	    di2[999999130L] = 0.5f;
	    
	    Test.LongFloatD _do;
	    Test.LongFloatD ro = p.opLongFloatD(di1, di2, out _do);
	    
	    test(_do.Equals(di1));
	    test(ro.Count == 4);
	    test(ro[999999110L] == -1.1f);
	    test(ro[999999120L] == -100.4f);
	    test(ro[999999111L] == 123123.2f);
	    test(ro[999999130L] == 0.5f);
	}
	
	{
	    Test.StringStringD di1 = new Test.StringStringD();
	    di1["foo"] = "abc -1.1";
	    di1["bar"] = "abc 123123.2";
	    Test.StringStringD di2 = new Test.StringStringD();
	    di2["foo"] = "abc -1.1";
	    di2["FOO"] = "abc -100.4";
	    di2["BAR"] = "abc 0.5";
	    
	    Test.StringStringD _do;
	    Test.StringStringD ro = p.opStringStringD(di1, di2, out _do);
	    
	    test(_do.Equals(di1));
	    test(ro.Count == 4);
	    test(ro["foo"].Equals("abc -1.1"));
	    test(ro["FOO"].Equals("abc -100.4"));
	    test(ro["bar"].Equals("abc 123123.2"));
	    test(ro["BAR"].Equals("abc 0.5"));
	}
	
	{
	    Test.StringMyEnumD di1 = new Test.StringMyEnumD();
	    di1["abc"] = Test.MyEnum.enum1;
	    di1[""] = Test.MyEnum.enum2;
	    Test.StringMyEnumD di2 = new Test.StringMyEnumD();
	    di2["abc"] = Test.MyEnum.enum1;
	    di2["qwerty"] = Test.MyEnum.enum3;
	    di2["Hello!!"] = Test.MyEnum.enum2;
	    
	    Test.StringMyEnumD _do;
	    Test.StringMyEnumD ro = p.opStringMyEnumD(di1, di2, out _do);
	    
	    test(_do.Equals(di1));
	    test(ro.Count == 4);
	    test(ro["abc"] == Test.MyEnum.enum1);
	    test(ro["qwerty"] == Test.MyEnum.enum3);
	    test(ro[""] == Test.MyEnum.enum2);
	    test(ro["Hello!!"] == Test.MyEnum.enum2);
	}
	
	{
	    int[] lengths = new int[] { 0, 1, 2, 126, 127, 128, 129, 253, 254, 255, 256, 257, 1000 };
	    
	    for(int l = 0; l < lengths.Length; ++l)
	    {
		Test.IntS s = new Test.IntS();
		for(int i = 0; i < lengths[l]; ++i)
		{
		    s.Add(i);
		}
		Test.IntS r = p.opIntS(s);
		test(r.Count == lengths[l]);
		for(int j = 0; j < r.Count; ++j)
		{
		    test(r[j] == -j);
		}
	    }
	}

	{
	    Ice.Context ctx = new Ice.Context();
	    ctx["one"] = "ONE";
	    ctx["two"] = "TWO";
	    ctx["three"] = "THREE";
	    {
		test(p.ice_getContext().Count == 0);
		Ice.Context r = p.opContext();
		test(!r.Equals(ctx));
	    }
	    {
		Ice.Context r = p.opContext(ctx);
		test(p.ice_getContext().Count == 0);
		test(r.Equals(ctx));
	    }
	    {
		Test.MyClassPrx p2 = Test.MyClassPrxHelper.checkedCast(p.ice_newContext(ctx));
		test(p2.ice_getContext().Equals(ctx));
		Ice.Context r = p2.opContext();
		test(r.Equals(ctx));
		r = p2.opContext(ctx);
		test(r.Equals(ctx));
	    }
	    {
		//
		// Test that default context is obtained correctly from communicator.
		//
		Ice.Context dflt = new Ice.Context();
		dflt["a"] = "b";
		communicator.setDefaultContext(dflt);
		test(!p.opContext().Equals(dflt));

		Test.MyClassPrx p2 = Test.MyClassPrxHelper.uncheckedCast(p.ice_newContext(new Ice.Context()));
		test(p2.opContext().Count == 0);

		p2 = Test.MyClassPrxHelper.uncheckedCast(p.ice_defaultContext());
		test(p2.opContext().Equals(dflt));

		communicator.setDefaultContext(new Ice.Context());
		test(p2.opContext().Count != 0);

		communicator.setDefaultContext(dflt);
		Test.MyClassPrx c = Test.MyClassPrxHelper.checkedCast(
					communicator.stringToProxy("test:default -p 12010 -t 10000"));
		test(c.opContext().Equals(dflt));

		dflt["a"] = "c";
		Test.MyClassPrx c2 = Test.MyClassPrxHelper.uncheckedCast(c.ice_newContext(dflt));
		test(c2.opContext()["a"].Equals("c"));

		dflt.Clear();
		Test.MyClassPrx c3 = Test.MyClassPrxHelper.uncheckedCast(c2.ice_newContext(dflt));
		test(c3.opContext()["a"] == null);

		Test.MyClassPrx c4 = Test.MyClassPrxHelper.uncheckedCast(c2.ice_defaultContext());
		test(c4.opContext()["a"].Equals("b"));

		dflt["a"] = "d";
		communicator.setDefaultContext(dflt);

		Test.MyClassPrx c5 = Test.MyClassPrxHelper.uncheckedCast(c2.ice_defaultContext());
		test(c5.opContext()["a"].Equals("d"));

		communicator.setDefaultContext(new Ice.Context());
	    }
	}
    }
}
