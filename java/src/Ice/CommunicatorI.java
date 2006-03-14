// **********************************************************************
//
// Copyright (c) 2003-2006 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

package Ice;

public final class CommunicatorI extends LocalObjectImpl implements Communicator
{
    public void
    destroy()
    {
	_instance.destroy();
    }

    public void
    shutdown()
    {
	_instance.objectAdapterFactory().shutdown();
    }

    public void
    waitForShutdown()
    {
	_instance.objectAdapterFactory().waitForShutdown();
    }

    public Ice.ObjectPrx
    stringToProxy(String s)
    {
        return _instance.proxyFactory().stringToProxy(s);
    }

    public String
    proxyToString(Ice.ObjectPrx proxy)
    {
        return _instance.proxyFactory().proxyToString(proxy);
    }

    public ObjectAdapter
    createObjectAdapter(String name)
    {
	return createObjectAdapterWithEndpoints(name, getProperties().getProperty(name + ".Endpoints"));
    }

    public ObjectAdapter
    createObjectAdapterWithEndpoints(String name, String endpoints)
    {
	return _instance.objectAdapterFactory().createObjectAdapter(name, endpoints);
    }

    public void
    addObjectFactory(ObjectFactory factory, String id)
    {
        _instance.servantFactoryManager().add(factory, id);
    }

    public void
    removeObjectFactory(String id)
    {
        _instance.servantFactoryManager().remove(id);
    }

    public ObjectFactory
    findObjectFactory(String id)
    {
        return _instance.servantFactoryManager().find(id);
    }

    public Properties
    getProperties()
    {
        return _instance.properties();
    }

    public Logger
    getLogger()
    {
        return _instance.logger();
    }

    public void
    setLogger(Logger logger)
    {
        _instance.logger(logger);
    }

    public Stats
    getStats()
    {
        return _instance.stats();
    }

    public void
    setStats(Stats stats)
    {
        _instance.stats(stats);
    }

    public RouterPrx
    getDefaultRouter()
    {
        return _instance.referenceFactory().getDefaultRouter();
    }

    public void
    setDefaultRouter(RouterPrx router)
    {
        _instance.referenceFactory().setDefaultRouter(router);
    }

    public LocatorPrx
    getDefaultLocator()
    {
        return _instance.referenceFactory().getDefaultLocator();
    }

    public void
    setDefaultLocator(LocatorPrx locator)
    {
        _instance.referenceFactory().setDefaultLocator(locator);
    }

    public java.util.Map
    getDefaultContext()
    {
	return _instance.getDefaultContext();
    }

    public void
    setDefaultContext(java.util.Map ctx)
    {
	_instance.setDefaultContext(ctx);
    }

    public PluginManager
    getPluginManager()
    {
        return _instance.pluginManager();
    }

    public void
    flushBatchRequests()
    {
        _instance.flushBatchRequests();
    }

    CommunicatorI(Properties properties, Logger logger)
    {
        _instance = new IceInternal.Instance(this, properties, logger);
    }

    /**
      * For compatibility with C#, we do not invoke methods on other objects
      * from within a finalizer.
      *
    protected synchronized void
    finalize()
        throws Throwable
    {
	if(!_instance.destroyed())
	{
	    _instance.logger().warning("Ice::Communicator::destroy() has not been called");
	}

        super.finalize();
    }
      */

    //
    // Certain initialization tasks need to be completed after the
    // constructor.
    //
    void
    finishSetup(StringSeqHolder args)
    {
	try
	{
	    _instance.finishSetup(args);
	}
	catch(RuntimeException ex)
	{
	    _instance.destroy();
	    throw ex;
	}
    }

    //
    // For use by Util.getInstance()
    //
    IceInternal.Instance
    getInstance()
    {
        return _instance;
    }

    private IceInternal.Instance _instance;
}
