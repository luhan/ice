// **********************************************************************
//
// Copyright (c) 2001
// MutableRealms, Inc.
// Huntsville, AL, USA
//
// All Rights Reserved
//
// **********************************************************************

#ifndef GEN_H
#define GEN_H

#include <Slice/JavaUtil.h>

namespace Slice
{

class JavaVisitor : public JavaGenerator, public ParserVisitor
{
public:

    virtual ~JavaVisitor();

protected:

    JavaVisitor(const std::string&, const std::string&);

    //
    // Compose the parameter list for an operation
    //
    std::string getParams(const OperationPtr&, const std::string&);

    //
    // Compose the argument list for an operation
    //
    std::string getArgs(const OperationPtr&, const std::string&);

    //
    // Generate a throws clause containing only non-local exceptions
    //
    void writeThrowsClause(const std::string&, const ExceptionList&);

    //
    // Generate a throws clause for delegate operations containing only
    // non-local exceptions
    //
    void writeDelegateThrowsClause(const std::string&, const ExceptionList&);

    //
    // Generate code to compute a hash code for a type
    //
    void writeHashCode(::IceUtil::Output&, const TypePtr&, const std::string&, int&);

    //
    // Generate dispatch methods for a class or interface
    //
    void writeDispatch(::IceUtil::Output&, const ClassDefPtr&);
};

class Gen : public ::IceUtil::noncopyable
{
public:

    Gen(const std::string&,
        const std::string&,
        const std::vector<std::string>&,
        const std::string&,
        const std::string&);
    ~Gen();

    bool operator!() const; // Returns true if there was a constructor error

    void generate(const UnitPtr&);
    void generateImpl(const UnitPtr&);

private:

    std::string _base;
    std::vector<std::string> _includePaths;
    std::string _package;
    std::string _dir;

    class TypesVisitor : public JavaVisitor
    {
    public:

        TypesVisitor(const std::string&, const std::string&);

        virtual bool visitClassDefStart(const ClassDefPtr&);
        virtual void visitClassDefEnd(const ClassDefPtr&);
        virtual void visitOperation(const OperationPtr&);
        virtual bool visitExceptionStart(const ExceptionPtr&);
        virtual void visitExceptionEnd(const ExceptionPtr&);
        virtual bool visitStructStart(const StructPtr&);
        virtual void visitStructEnd(const StructPtr&);
        virtual void visitEnum(const EnumPtr&);
        virtual void visitDataMember(const DataMemberPtr&);
    };

    class HolderVisitor : public JavaVisitor
    {
    public:

        HolderVisitor(const std::string&, const std::string&);

        virtual bool visitClassDefStart(const ClassDefPtr&);
        virtual bool visitStructStart(const StructPtr&);
        virtual void visitSequence(const SequencePtr&);
        virtual void visitDictionary(const DictionaryPtr&);
        virtual void visitEnum(const EnumPtr&);

    private:

        void writeHolder(const TypePtr&);
    };

    class HelperVisitor : public JavaVisitor
    {
    public:

        HelperVisitor(const std::string&, const std::string&);

        virtual bool visitClassDefStart(const ClassDefPtr&);
        virtual void visitSequence(const SequencePtr&);
        virtual void visitDictionary(const DictionaryPtr&);
    };

    class ProxyVisitor : public JavaVisitor
    {
    public:

        ProxyVisitor(const std::string&, const std::string&);

        virtual bool visitClassDefStart(const ClassDefPtr&);
        virtual void visitClassDefEnd(const ClassDefPtr&);
        virtual void visitOperation(const OperationPtr&);
    };

    class DelegateVisitor : public JavaVisitor
    {
    public:

        DelegateVisitor(const std::string&, const std::string&);

        virtual bool visitClassDefStart(const ClassDefPtr&);
    };

    class DelegateMVisitor : public JavaVisitor
    {
    public:

        DelegateMVisitor(const std::string&, const std::string&);

        virtual bool visitClassDefStart(const ClassDefPtr&);
    };

    class DelegateDVisitor : public JavaVisitor
    {
    public:

        DelegateDVisitor(const std::string&, const std::string&);

        virtual bool visitClassDefStart(const ClassDefPtr&);
    };

    class DispatcherVisitor : public JavaVisitor
    {
    public:

        DispatcherVisitor(const std::string&, const std::string&);

        virtual bool visitClassDefStart(const ClassDefPtr&);
    };

    class ImplVisitor : public JavaVisitor
    {
        //
        // Generate code to assign a value
        //
        void writeAssign(::IceUtil::Output&, const std::string&, const TypePtr&,
                         const std::string&, int&);

    public:

        ImplVisitor(const std::string&, const std::string&);

        virtual bool visitClassDefStart(const ClassDefPtr&);
    };
};

}

#endif
