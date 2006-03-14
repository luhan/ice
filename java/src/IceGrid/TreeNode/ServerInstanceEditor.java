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
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JFrame;
import javax.swing.JTextField;

import javax.swing.event.ListDataEvent;
import javax.swing.event.ListDataListener;

import com.jgoodies.forms.builder.DefaultFormBuilder;

import IceGrid.Model;
import IceGrid.ParametersDialog;
import IceGrid.ServerDescriptor;
import IceGrid.ServerInstanceDescriptor;
import IceGrid.TemplateDescriptor;
import IceGrid.Utils;

class ServerInstanceEditor extends AbstractServerEditor
{
    protected void writeDescriptor()
    {
	ServerInstanceDescriptor descriptor = getDescriptor();
	ServerTemplate t = (ServerTemplate)_template.getSelectedItem();
	
	descriptor.template = t.getId();
	descriptor.parameterValues = _parameterValuesMap;
	((Server)_target).setServerDescriptor(
	    (ServerDescriptor)((TemplateDescriptor)t.getDescriptor()).descriptor);
    }
    
    protected boolean isSimpleUpdate()
    {
	return false;
    }

    ServerInstanceEditor(Model model, JFrame parentFrame)
    {
	super(model);
	_subEditor = new ServerSubEditor(this, parentFrame);
	
	_template.setToolTipText("Server template");
	_parameterValues.setEditable(false);
	
	//
	// Template
	//
	Action gotoTemplate = new AbstractAction(
	    "", Utils.getIcon("/icons/16x16/goto.png"))
	    {
		public void actionPerformed(ActionEvent e) 
		{
		    CommonBase t = (CommonBase)_template.getSelectedItem();
		    if(t != null)
		    {
			t.getModel().setSelectionPath(t.getPath());
		    }
		}
	    };
	gotoTemplate.putValue(Action.SHORT_DESCRIPTION, 
			      "Goto this server template");
	_templateButton = new JButton(gotoTemplate);

	//
	// Parameter values
	//
	_parametersDialog = new ParametersDialog(parentFrame, 
					    "Parameter values",
					    "Value", false, "Use default");
	
	Action openParametersDialog = new AbstractAction("...")
	    {
		public void actionPerformed(ActionEvent e) 
		{
		    if(_parametersDialog.show(_parameterList, _parameterValuesMap, 
					      getProperties()))
		    {
			updated();
			setParameterValuesField();
			//
			// No need to redisplay details: since it's editable,
			// we're not substituting variables or parameters
			//
		    }
		}
	    };
	openParametersDialog.putValue(Action.SHORT_DESCRIPTION, 
				      "Edit parameters");

	_parametersButton = new JButton(openParametersDialog);
    }

    ServerInstanceDescriptor getDescriptor()
    {
	return ((Server)_target).getInstanceDescriptor();
    }

    //
    // From Editor:
    //

    Utils.Resolver getDetailResolver()
    {
	Server server = (Server)_target;
	if(server.getModel().substitute())
	{
	    return server.getResolver();
	}
	else
	{
	    return null;
	}
    }

    void appendProperties(DefaultFormBuilder builder)
    { 
	builder.append("Template", _template);
	builder.append(_templateButton);
	builder.nextLine();
	
	builder.append("Parameter values", _parameterValues);
	builder.append(_parametersButton);
	builder.nextLine();
	
	builder.appendSeparator();
	builder.nextLine();
	
	_subEditor.appendProperties(builder);
    }

    Object getSubDescriptor()
    {
	ServerTemplate template = (ServerTemplate)_template.getSelectedItem();
	
	TemplateDescriptor descriptor = (TemplateDescriptor)template.getDescriptor();
	return descriptor.descriptor;
    }

    void show(Server server)
    {
	detectUpdates(false);
	setTarget(server);

	ServerInstanceDescriptor descriptor = getDescriptor();
	Model model = server.getModel();
	boolean isEditable = !model.substitute();
	
	//
	// Need to make control enabled before changing it
	//
	_template.setEnabled(true);

	ServerTemplates serverTemplates =
	    server.getApplication().getServerTemplates();
	_template.setModel(serverTemplates.createComboBoxModel());

	ServerTemplate t = (ServerTemplate)
	    serverTemplates.findChild(descriptor.template);
	assert t != null;
	_template.setSelectedItem(t);
	
	ListDataListener templateListener = new ListDataListener()
	    {
		public void contentsChanged(ListDataEvent e)
		{
		    updated();
		    
		    ServerTemplate t = 
			(ServerTemplate)_template.getModel().getSelectedItem();
		    
		    TemplateDescriptor td = (TemplateDescriptor)t.getDescriptor();
		    
		    //
		    // Replace parameters but keep existing values
		    //
		    _parameterList = td.parameters;
		    _parameterValuesMap = makeParameterValues(_parameterValuesMap, 
							      _parameterList);
		    setParameterValuesField();

		    //
		    // Redisplay details
		    //
		    _subEditor.show(false);
		}

		public void intervalAdded(ListDataEvent e)
		{}
         
		public void intervalRemoved(ListDataEvent e)
		{}
	    };

	_template.getModel().addListDataListener(templateListener);
	_template.setEnabled(isEditable);
	
	_parameterList =  ((TemplateDescriptor)t.getDescriptor()).parameters;
	_parameterValuesMap = new java.util.HashMap(descriptor.parameterValues);
	setParameterValuesField();
	_parametersButton.setEnabled(isEditable);
	
	_subEditor.show(false);

	_applyButton.setEnabled(server.isEphemeral());
	_discardButton.setEnabled(server.isEphemeral());	  
	detectUpdates(true);

	refreshCurrentStatus();
    }

    void setParameterValuesField()
    {
	Server server = (Server)_target;

	final Utils.Resolver resolver = server.getModel().substitute() ? 
	    server.getParentResolver() : null;
	
	Ice.StringHolder toolTipHolder = new Ice.StringHolder();
	Utils.Stringifier stringifier = new Utils.Stringifier()
	    {
		public String toString(Object obj)
		{
		    String name = (String)obj;
		    String val = (String)_parameterValuesMap.get(name);
		    if(val != null)
		    {
			return name + "=" + Utils.substitute(val, resolver);
		    }
		    else
		    {
			return null;
		    }
		}
	    };
	
	_parameterValues.setText(
	    Utils.stringify(_parameterList, stringifier, ", ", toolTipHolder));
	_parameterValues.setToolTipText(toolTipHolder.value);
    }

    private ServerSubEditor _subEditor;

    private JComboBox _template = new JComboBox();
    private JButton _templateButton;
    private JTextField _parameterValues = new JTextField(20);

    private java.util.List _parameterList;
    private java.util.Map _parameterValuesMap;

    private ParametersDialog _parametersDialog;
    private JButton _parametersButton;   
}
