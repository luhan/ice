// **********************************************************************
//
// Copyright (c) 2003-2005 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

package Ice;

public class _ObjectDel
{
    public boolean
    ice_isA(String __id, java.util.Hashtable __context)
        throws IceInternal.NonRepeatable
    {
        IceInternal.Outgoing __og = __connection.getOutgoing(__reference, "ice_isA", OperationMode.Nonmutating,
							     __context);
	try
	{
	    try
	    {
		IceInternal.BasicStream __os = __og.os();
		__os.writeString(__id);
	    }
	    catch(Ice.LocalException __ex)
	    {
		__og.abort(__ex);
	    }
	    boolean __ok = __og.invoke();
	    try
	    {
		IceInternal.BasicStream __is = __og.is();
		if(!__ok)
		{
		    throw new UnknownUserException();
		}
		return __is.readBool();
	    }
	    catch(LocalException __ex)
	    {
		throw new IceInternal.NonRepeatable(__ex);
	    }
	}
	finally
	{
	    __connection.reclaimOutgoing(__og);
	}
    }

    public void
    ice_ping(java.util.Hashtable __context)
        throws IceInternal.NonRepeatable
    {
        IceInternal.Outgoing __og = __connection.getOutgoing(__reference, "ice_ping", OperationMode.Nonmutating,
							     __context);
	try
	{
	    boolean __ok = __og.invoke();
	    try
	    {
		if(!__ok)
		{
		    throw new UnknownUserException();
		}
	    }
	    catch(LocalException __ex)
	    {
		throw new IceInternal.NonRepeatable(__ex);
	    }
	}
	finally
	{
	    __connection.reclaimOutgoing(__og);
	}
    }

    public String[]
    ice_ids(java.util.Hashtable __context)
        throws IceInternal.NonRepeatable
    {
        IceInternal.Outgoing __og = __connection.getOutgoing(__reference, "ice_ids", OperationMode.Nonmutating,
							     __context);
	try
	{
	    boolean __ok = __og.invoke();
	    try
	    {
		IceInternal.BasicStream __is = __og.is();
		if(!__ok)
		{
		    throw new UnknownUserException();
		}
		return __is.readStringSeq();
	    }
	    catch(LocalException __ex)
	    {
		throw new IceInternal.NonRepeatable(__ex);
	    }
	}
	finally
	{
	    __connection.reclaimOutgoing(__og);
	}
    }

    public String
    ice_id(java.util.Hashtable __context)
        throws IceInternal.NonRepeatable
    {
        IceInternal.Outgoing __og = __connection.getOutgoing(__reference, "ice_id", OperationMode.Nonmutating,
							     __context);
	try
	{
	    boolean __ok = __og.invoke();
	    try
	    {
		IceInternal.BasicStream __is = __og.is();
		if(!__ok)
		{
		    throw new UnknownUserException();
		}
		return __is.readString();
	    }
	    catch(LocalException __ex)
	    {
		throw new IceInternal.NonRepeatable(__ex);
	    }
	}
	finally
	{
	    __connection.reclaimOutgoing(__og);
	}
    }

    public boolean
    ice_invoke(String operation, OperationMode mode, byte[] inParams, ByteSeqHolder outParams,
	       java.util.Hashtable __context)
        throws IceInternal.NonRepeatable
    {
        IceInternal.Outgoing __og = __connection.getOutgoing(__reference, operation, mode, __context);
        try
        {
            if(inParams != null)
            {
		try
		{
		    IceInternal.BasicStream __os = __og.os();
		    __os.writeBlob(inParams);
		}
		catch(Ice.LocalException __ex)
		{
		    __og.abort(__ex);
		}
            }
            boolean ok = __og.invoke();
            if(__reference.getMode() == IceInternal.Reference.ModeTwoway)
            {
                try
                {
                    IceInternal.BasicStream __is = __og.is();
                    int sz = __is.getReadEncapsSize();
                    if(outParams != null)
                    {
                        outParams.value = __is.readBlob(sz);
                    }
                }
                catch(LocalException __ex)
                {
                    throw new IceInternal.NonRepeatable(__ex);
                }
            }
            return ok;
        }
        finally
        {
            __connection.reclaimOutgoing(__og);
        }
    }

    public Connection
    ice_connection()
    {
	return __connection;
    }

    //
    // Only for use by ObjectPrx
    //
    final void
    __copyFrom(_ObjectDel from)
    {
        //
        // No need to synchronize "from", as the delegate is immutable
        // after creation.
        //

        //
        // No need to synchronize, as this operation is only called
        // upon initialization.
        //

	if(IceUtil.Debug.ASSERT)
	{
	    IceUtil.Debug.Assert(__reference == null);
	    IceUtil.Debug.Assert(__connection == null);
	}

        __reference = from.__reference;
        __connection = from.__connection;
    }

    protected IceInternal.Reference __reference;
    protected Connection __connection;

    public void
    setup(IceInternal.Reference ref)
    {
        //
        // No need to synchronize, as this operation is only called
        // upon initialization.
        //

	if(IceUtil.Debug.ASSERT)
	{
	    IceUtil.Debug.Assert(__reference == null);
	    IceUtil.Debug.Assert(__connection == null);
	}
	__reference = ref;
	__connection = __reference.getConnection();
    }
}
