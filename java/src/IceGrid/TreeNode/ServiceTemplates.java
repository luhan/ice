// **********************************************************************
//
// Copyright (c) 2003-2006 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************
package IceGrid.TreeNode;

import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;
import javax.swing.Action;
import javax.swing.JMenuItem;
import javax.swing.JPopupMenu;

import IceGrid.Model;
import IceGrid.ServiceDescriptor;
import IceGrid.TemplateDescriptor;

class ServiceTemplates extends Templates
{
    static public java.util.HashMap
    copyDescriptors(java.util.Map descriptors)
    {
	java.util.HashMap copy = new java.util.HashMap();
	java.util.Iterator p = descriptors.entrySet().iterator();
	while(p.hasNext())
	{
	    java.util.Map.Entry entry = (java.util.Map.Entry)p.next();
	    
	    copy.put(entry.getKey(), 
		     ServiceTemplate.copyDescriptor(
			 (TemplateDescriptor)entry.getValue()));
	}
	return copy;
    }
    

    //
    // Actions
    //
    public boolean[] getAvailableActions()
    {
	boolean[] actions = new boolean[ACTION_COUNT];
	actions[NEW_TEMPLATE_SERVICE] = true;
	
	Object clipboard = _model.getClipboard();
	if(clipboard != null && clipboard instanceof TemplateDescriptor)
	{
	    TemplateDescriptor d = (TemplateDescriptor)clipboard;
	    actions[PASTE] = d.descriptor instanceof ServiceDescriptor;
	}
	return actions;
    }
    public JPopupMenu getPopupMenu()
    {
	if(_popup == null)
	{
	    _popup = new PopupMenu(_model);
	    JMenuItem item = new JMenuItem(_model.getActions()[NEW_TEMPLATE_SERVICE]);
	    item.setText("New service template");
	    _popup.add(item);
	}
	return _popup;
    }
    public void newTemplateService()
    {
	ServiceDescriptor sd = new ServiceDescriptor(
	    new java.util.LinkedList(),
	    new java.util.LinkedList(),
	    new java.util.LinkedList(),
	    "",
	    "",
	    "");
	    
	newServiceTemplate(new TemplateDescriptor(
			       sd, new java.util.LinkedList(), new java.util.TreeMap()));

    }
    public void paste()
    {
	Object descriptor =  _model.getClipboard();
	TemplateDescriptor td = (TemplateDescriptor)descriptor;
	newServiceTemplate(td);
    }


    ServiceTemplates(java.util.Map descriptors, Model model)
	throws UpdateFailedException
    {
	super("Service templates", model);

	_descriptors = descriptors;

	java.util.Iterator p = _descriptors.entrySet().iterator();
	
	while(p.hasNext())
	{
	    java.util.Map.Entry entry = (java.util.Map.Entry)p.next();
	    addChild(new ServiceTemplate(false, (String)entry.getKey(),
					 (TemplateDescriptor)entry.getValue(),
					 _model));
	}
    }

    void newServiceTemplate(TemplateDescriptor descriptor)
    {
	String id = makeNewChildId("NewServiceTemplate");
	
	ServiceTemplate t = new ServiceTemplate(id, descriptor, _model);
	try
	{
	    addChild(t, true);
	}
	catch(UpdateFailedException e)
	{
	    assert false;
	}
	_model.setSelectionPath(t.getPath());
    }

    void tryAdd(String newId, TemplateDescriptor descriptor)
	throws UpdateFailedException
    {
	try
	{
	    addChild(new ServiceTemplate(true, newId, descriptor, _model),
		     true);
	}
	catch(UpdateFailedException e)
	{
	    e.addParent(this);
	    throw e;
	}
	_descriptors.put(newId, descriptor);
    }

    protected java.util.List findAllTemplateInstances(String templateId)
    {
	return getApplication().findServiceInstances(templateId);
    }
    

    java.util.Map getUpdates()
    {
	java.util.Map updates = new java.util.HashMap();
	java.util.Iterator p = _children.iterator();
	while(p.hasNext())
	{
	    ServiceTemplate t = (ServiceTemplate)p.next();
	    if(t.isNew() || t.isModified())
	    {
		updates.put(t.getId(), t.getDescriptor());
	    }
	}
	return updates;
    }

    void update(java.util.Map descriptors, String[] removeTemplates)
	throws UpdateFailedException
    {
	//
	// Note: _descriptors is updated by Application
	//
	
	//
	// One big set of removes
	//
	removeChildren(removeTemplates);

	//
	// One big set of updates, followed by inserts
	//
	java.util.Vector newChildren = new java.util.Vector();
	java.util.Vector updatedChildren = new java.util.Vector();
	
	java.util.Iterator p = descriptors.entrySet().iterator();
	while(p.hasNext())
	{
	    java.util.Map.Entry entry = (java.util.Map.Entry)p.next();
	    String name = (String)entry.getKey();
	    TemplateDescriptor templateDescriptor 
		= (TemplateDescriptor)entry.getValue();
	    ServiceTemplate child = (ServiceTemplate)findChild(name);
	    if(child == null)
	    {
		newChildren.add(new ServiceTemplate(false, name, 
						    templateDescriptor, _model));
	    }
	    else
	    {
		child.rebuild(templateDescriptor);
		updatedChildren.add(child);
	    }
	}
	
	updateChildren((CommonBaseI[])updatedChildren.toArray
		       (new CommonBaseI[0]));
	addChildren((CommonBaseI[])newChildren.toArray(new CommonBaseI[0]));

    }

    void removeDescriptor(String id)
    {
	_descriptors.remove(id);
    }
  
    private java.util.Map _descriptors;

    static private JPopupMenu _popup;
}
