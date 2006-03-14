// **********************************************************************
//
// Copyright (c) 2003-2006 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

namespace Ice
{
	
    using System.Collections;
    using System.Diagnostics;

    public interface PluginFactory
    {
        Plugin create(Communicator communicator, string name, string[] args);
    }

    public sealed class PluginManagerI : LocalObjectImpl, PluginManager
    {
	private static string _kindOfObject = "plug-in";

	public Plugin getPlugin(string name)
	{
	    lock(this)
	    {
		if(_communicator == null)
		{
		    throw new CommunicatorDestroyedException();
		}
	    
		Plugin p = (Plugin)_plugins[name];
		if(p != null)
		{
		    return p;
		}
		NotRegisteredException ex = new NotRegisteredException();
		ex.id = name;
		ex.kindOfObject = _kindOfObject;
		throw ex;
	    }
	}

	public void addPlugin(string name, Plugin plugin)
	{
	    lock(this)
	    {
		if(_communicator == null)
		{
		    throw new CommunicatorDestroyedException();
		}
	    
		if(_plugins.Contains(name))
		{
		    AlreadyRegisteredException ex = new AlreadyRegisteredException();
		    ex.id = name;
		    ex.kindOfObject = _kindOfObject;
		    throw ex;
		}
		_plugins[name] = plugin;
	    }
	}

	public void destroy()
	{
	    lock(this)
	    {
		if(_communicator != null)
		{
		    foreach(Plugin plugin in _plugins.Values)
		    {
			plugin.destroy();
		    }
		
		    _communicator = null;
		}
	    }
	}
	
	public PluginManagerI(Communicator communicator)
	{
	    _communicator = communicator;
	    _plugins = new Hashtable();
	}

	public void loadPlugins(ref string[] cmdArgs)
	{
	    Debug.Assert(_communicator != null);
	    
	    //
	    // Load and initialize the plug-ins defined in the property set
	    // with the prefix "Ice.Plugin.". These properties should
	    // have the following format:
	    //
	    // Ice.Plugin.name=entry_point [args]
	    //
	    // The code below is different from the Java/C++ algorithm
	    // because C# must support full assembly names such as:
	    //
	    // Ice.Plugin.Logger=logger, Version=0.0.0.0, Culture=neutral:LoginPluginFactory

	    //
	    string prefix = "Ice.Plugin.";
	    Ice.Properties properties = _communicator.getProperties();
	    PropertyDict plugins = properties.getPropertiesForPrefix(prefix);
	    foreach(DictionaryEntry entry in plugins)
	    {
		string name = ((string)entry.Key).Substring(prefix.Length);
		string val = (string)entry.Value;
		
		//
		// Separate the entry point from the arguments. First
		// look for the :, then for the next whitespace. This
		// represents the end of the entry point.
		//
		// The remainder of the configuration line represents
		// the arguments.
		//
		string entryPoint = val;
		string[] args = new string[0];
		int start = val.IndexOf(':');
		if(start != -1)
		{
		    //
		    // Find the whitespace.
		    //
		    int pos = val.IndexOf(' ', start);
		    if(pos == -1)
		    {
			pos = val.IndexOf('\t', start);
		    }
		    if(pos == -1)
		    {
			pos = val.IndexOf('\n', start);
		    }
		    if(pos != -1)
		    {
			entryPoint = val.Substring(0, pos);
			char[] delims = { ' ', '\t', '\n' };
			args = val.Substring(pos).Trim().Split(delims, pos);
		    }
		}
		
		//
		// Convert command-line options into properties. First
		// we convert the options from the plug-in
		// configuration, then we convert the options from the
		// application command-line.
		//
		args = properties.parseCommandLineOptions(name, args);
		cmdArgs = properties.parseCommandLineOptions(name, cmdArgs);
		
		loadPlugin(name, entryPoint, args);
	    }
	}
	
	private void loadPlugin(string name, string entryPoint, string[] args)
	{
	    Debug.Assert(_communicator != null);
	    
	    //
	    // Retrieve the assembly name and the type.
	    //
	    string err = "unable to load plugin '" + entryPoint + "': ";
	    int sepPos = entryPoint.IndexOf(':');
	    if (sepPos == -1)
	    {
		PluginInitializationException e = new PluginInitializationException();
		e.reason = err + "invalid entry point format";
		throw e;
	    }
	    
	    System.Reflection.Assembly pluginAssembly = null;
	    string assemblyName = entryPoint.Substring(0, sepPos);
	    try
	    {
		if (System.IO.File.Exists(assemblyName))
		{
		    pluginAssembly = System.Reflection.Assembly.LoadFrom(assemblyName);
		}
		else
		{
		    pluginAssembly = System.Reflection.Assembly.Load(assemblyName);
		}
	    }
	    catch(System.Exception ex)
	    {
		PluginInitializationException e = new PluginInitializationException();
		e.reason = err + "unable to load assembly: '" + assemblyName + "': " + ex.ToString();
		throw e;
	    }
	    
	    //
	    // Instantiate the class.
	    //
	    PluginFactory factory = null;
	    string className = entryPoint.Substring(sepPos + 1);
	    System.Type c = pluginAssembly.GetType(className);
            if(c == null)
            {
                PluginInitializationException e = new PluginInitializationException();
                e.reason = err + "GetType failed for '" + className + "'";
                throw e;
            }

	    try
	    {

		factory = (PluginFactory)IceInternal.AssemblyUtil.createInstance(c);
	    }
	    catch(System.InvalidCastException ex)
	    {
		PluginInitializationException e = new PluginInitializationException(ex);
		e.reason = err + "InvalidCastException to Ice.PluginFactory";
		throw e;
	    }
	    catch(System.UnauthorizedAccessException ex)
	    {
		PluginInitializationException e = new PluginInitializationException(ex);
		e.reason = err + "UnauthorizedAccessException: " + ex.ToString();
		throw e;
	    }
	    catch(System.Exception ex)
	    {
		PluginInitializationException e = new PluginInitializationException(ex);
		e.reason = err + "System.Exception: " + ex.ToString();
		throw e;
	    }
	    
	    //
	    // Invoke the factory.
	    //
	    Plugin plugin = null;
	    try
	    {
		plugin = factory.create(_communicator, name, args);
	    }
	    catch(PluginInitializationException ex)
	    {
		ex.reason = err + ex.reason;
		throw ex;
	    }
	    catch(System.Exception ex)
	    {
		PluginInitializationException e = new PluginInitializationException(ex);
		e.reason = err + "System.Exception in factory.create: " + ex.ToString();
		throw e;
	    }
	    
	    if(plugin == null)
	    {
		PluginInitializationException ex = new PluginInitializationException();
		ex.reason = err + "factory.create returned null plug-in";
		throw ex;
	    }

	    _plugins[name] = plugin;
	}
	
	private Communicator _communicator;
	private Hashtable _plugins;
    }

}
