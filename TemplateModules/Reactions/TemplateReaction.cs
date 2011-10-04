using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace TemplateModules.Reactions
{
    // This attribute should be on any event or reaction (or classes they inherit from) that needs
    // to save variables when Mayhem is shut down so the module can remember it's configuration
    // when it is relaunched.
    // This attribute is from System.Runtime.Serialization
    [DataContract]

    // This attribute should exist on any event or reaction that will show up in the module lists.
    // The first parameter is the name of the module. The name will show up as the main text in
    // the module list, as well as the text on the connection in the main application window.
    // The second parameter is the description and appears as the subtext in the module list.
    // This attribute exists inside of MayhemCore
    [MayhemModule("Name of Reaction", "Description of Reaction")]

    // In order to mark the class as a Reaction, the class must extend ReactionBase from MayhemCore
    // To be configurable from the Mayhem application, the class must extend IWpfConfigurable from
    // MayhemWpf.ModuleTypes
    class TemplateReaction : ReactionBase, IWpfConfigurable
    {
        // On variables that store configuration values for the module, they should be marked with
        // the DataMember attribute. These variables will be saved when the application is shut
        // down, and will be brought back when the application comes back up
        [DataMember]
        private int variableToSave;

        // This variable is not saved when the application is shut down and started up again.
        private object variableToNotSave;

        // The following four methods are the methods you can use to set up and configure your
        // module. You should never use a constructor in modules. This is because there is no
        // guarantee as to when the constructor is called when deserializing. There are two ways
        // your module can be created. The first of which is instantiating a new instance. In that
        // case the following methods are called in this order: 
        // OnBeforeLoad, OnLoadDefaults, OnAfterLoad
        // Your module can also be created from deserialization (when your object saves state and
        // is recreated with the same state. In that case, the following methods are called in this
        // order:
        // OnBeforeLoad, OnLoadFromSaved, OnAfterLoad

        // This method is called first, you should use it to instantiate any variables that are
        // needed to load your configuration and setup. For example, if you will need to add saved
        // items to an object, you should instantiate that object in this method so that it will
        // exist in OnLoadDefaults and OnLoadFromSaved.
        protected override void OnBeforeLoad() { }

        // This method is called after OnBeforeLoad when your module object is created because of
        // standard instantiation. Use this method to set the defaults on your configuration
        // variables.
        protected override void OnLoadDefaults() { }

        // This method is called after OnBeforeLoad when your module object is created because of
        // deserialization. In this method you are guaranteed that all variables marked with the
        // DataMember attribute are set. 
        protected override void OnLoadFromSaved() { }

        // This method is called after both OnLoadDefaults and OnLoadFromSaved. This method is
        // guaranteed that all configuration properties are set.
        protected override void OnAfterLoad() { }

        // This method is called before enabling the module. The module should verify that it can
        // be started up. If for whatever reason it cannot, then e.Cancel should be set to true.
        // The property WasConfiguring is set to true in the case that we are re-enabling after
        // the configuration window was closed.
        protected override void OnEnabling(EnablingEventArgs e) { }

        // This method is called when the module is disabled; on application shutdown, on
        // connection deleted, and when the configuration for the module is opened. In the case
        // that it is called when configuration is opened, e.IsConfiguring will be set to true.
        protected override void OnDisabled(DisabledEventArgs e) { }

        // This method is called after the connection containing this module has been deleted.
        protected override void OnDeleted() { }

        // This method gets called when the reaction is triggered. 
        // Implement your reaction functionality in this method. 
        public override void Perform() { }

        #region IWpfConfigurable Methods

        // This method returns an instance of the configuration control your module produces so
        // users can set your configuration settings.
        public WpfConfiguration ConfigurationControl
        {
            get { throw new NotImplementedException(); }
        }

        // This method is called if and when the user saves your configuration window. The 
        // configurationControl property contains the instance of the control you returned
        // in the ConfigurationControl property. You will likely need to cast the user control
        // to the type of configuration control class you created. You should take the settings
        // from your user control and set them on this object
        public void OnSaved(WpfConfiguration configurationControl)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IConfigurable Members

        // This method needs to return a string that is shown to the user on the main application
        // window that contains a description of the current configuration of the module. 
        public string GetConfigString() { return variableToSave.ToString(); }

        #endregion

      
    }
}
