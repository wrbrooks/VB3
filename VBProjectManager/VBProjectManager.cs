﻿using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VBCommon;


namespace VBProjectManager
{

    public partial class VBProjectManager : Extension, IFormState, IPartImportsSatisfiedNotification
    {        
        private Dictionary<string, Boolean> _tabStates;
        private string strPathName;
        private string projectName;
        private VBCommon.Signaller signaller = new VBCommon.Signaller();
        private VBLogger logger;
        private string strLogFile;
        private string strPluginKey = "Project Manager";
        public static string VB2projectsPath = null;

        public VBProjectManager()
        {
            strLogFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VirtualBeach.log");
            VBLogger.SetLogFileName(strLogFile);
            logger = VBLogger.GetLogger();

            signaller = new VBCommon.Signaller();
        }


        public override void Activate()
        {                       

            //Add an item to the application ("File") menu.
            var aboutButton = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "About", AboutVirtualBeach);
            aboutButton.GroupCaption = HeaderControl.ApplicationMenuKey;
            aboutButton.LargeImage = Resources.info_32x32;
            aboutButton.SmallImage = Resources.info_16x16;
            aboutButton.ToolTipText = "Open the 'About VirtualBeach' dialog.";
            App.HeaderControl.Add(aboutButton);
            
            //Add a Save button to the application ("File") menu.
            var saveButton = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Save", Save);
            saveButton.GroupCaption = HeaderControl.ApplicationMenuKey;
            saveButton.LargeImage = Resources.save_32x32;
            saveButton.SmallImage = Resources.save_16x16;
            saveButton.ToolTipText = "Save the current project state.";
            App.HeaderControl.Add(saveButton);

            //Add an Open button to the application ("File") menu.
            var openButton = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Open", Open);
            openButton.GroupCaption = HeaderControl.ApplicationMenuKey;
            openButton.LargeImage = Resources.open_32x32;
            openButton.SmallImage = Resources.open_16x16;
            openButton.ToolTipText = "Open a saved project.";
            App.HeaderControl.Add(openButton);

            //test hide panels
            var btnRb = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Test Hide", rb_Click);
            btnRb.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnRb.LargeImage = Resources.open_32x32;
            btnRb.SmallImage = Resources.open_16x16;
            btnRb.ToolTipText = "test hide panel";
            App.HeaderControl.Add(btnRb);
            
            
            //get plugin type for each plugin
            List<Globals.PluginType> allPluginTypes = new List<Globals.PluginType>();
            
            foreach(DotSpatial.Extensions.IExtension ext in App.Extensions)
            {
                if (ext is IPlugin)
                {
                    IPlugin plugType = (IPlugin)ext;

                    //store pluginType
                    Globals.PluginType PType = plugType.PluginType;
                    allPluginTypes.Add(PType);
                }
            }
            

            //if PType is smallest (datasheet/map), set as activated when open
            int pos = allPluginTypes.IndexOf(allPluginTypes.Min());
            DotSpatial.Extensions.IExtension extension = App.Extensions.ElementAt(pos);
            IPlugin ex = (IPlugin)extension;
            ex.MakeActive();
            

            base.Activate();
        }

       
        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            
            
            base.Deactivate();
        }

        
        private void MessageReceived(object sender, MessageArgs args)
        {
            //Write any messages we receive from the plugins directly to the debug console.
            System.Diagnostics.Debug.WriteLine(args.Message);
        }


        public void rb_Click(object sender, EventArgs e)
        {
            App.HeaderControl.RemoveAll();
            //((VBDockManager.VBDockManager)App.DockManager).HidePanel("PLSPanel");
            //foreach (Extension ext in App.Extensions)
            //{
            //    if (ext is IPlugin)
            //    {
            //        IPlugin plugType = (IPlugin)ext;
            //        //store pluginType
            //        Globals.PluginType PType = plugType.PluginType;
            //        if (PType != Globals.PluginType.Datasheet)
            //        {
            //            //((VBDockManager.VBDockManager)App.DockManager).HidePanel((IPlugin);
            //        }
            //    }
            //}
        }


        public void AboutVirtualBeach(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("this is a test.");
        }


        public Dictionary<string, Boolean> TabStates
        {
            get { return _tabStates; }
            set { _tabStates = value; }
        }


        public string ProjectPathName
        {
            get { return strPathName; }
            set { strPathName = value; }
        }


        public string ProjectName
        {
            get { return projectName; }
            set { projectName = value; }
        }


        //We export this property so that other Plugins can have access to the signaller.
        public VBCommon.Signaller Signaller
        {
            get { return this.signaller; }
        }


        //We export this method so that other Plugins can have access to the signaller.
        [System.ComponentModel.Composition.Export("Signalling.GetSignaller")]
        public VBCommon.Signaller ProvideAccessToSignaller()
        {
            return this.Signaller;
        }


        //This function imports the signaller from the VBProjectManager
        [System.ComponentModel.Composition.Import("Signalling.GetSignaller", AllowDefault = true)]
        public Func<VBCommon.Signaller> GetSignaller
        {
            get;
            set;
        }


        public void OnImportsSatisfied()
        {
            //If we've successfully imported a Signaller, then connect its events to our handlers.
            signaller = GetSignaller();
            signaller.MessageReceived += new VBCommon.Signaller.MessageHandler<MessageArgs>(MessageReceived);
            signaller.ProjectSaved += new VBCommon.Signaller.SerializationEventHandler<VBCommon.SerializationEventArgs>(ProjectSavedListener);
            signaller.ProjectOpened += new VBCommon.Signaller.SerializationEventHandler<VBCommon.SerializationEventArgs>(ProjectOpenedListener); //loop through plugins ck for min pluginType to make that active when plugin opened.
        }
    }
    
    
    public class ProjectChangedStatus : EventArgs
    {
        private string _status;
        public string Status
        {
            set { _status = value; }
            get { return _status; }
        }
    }
}
