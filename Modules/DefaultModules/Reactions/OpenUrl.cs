using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using DefaultModules.Resources;
using MayhemWpf.UserControls;
using System.Globalization;
using DefaultModules.Wpf;

namespace DefaultModules.Reactions
{
    [DataContract]
    [MayhemModule("Open Website", "Opens the default browser to the specified Url")]
    public class OpenUrl : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string Url
        {
            get;
            set;
        }

        public override void Perform()
        {
            try 
            {
                System.Diagnostics.Process.Start(Url);
            } 
            catch(Exception exc1)
            {      
                // System.ComponentModel.Win32Exception is a known exception that occurs when Firefox is default browser.   
                // It actually opens the browser but STILL throws this exception so we can just ignore it.  If not this exception,      
                // then attempt to open the URL in IE instead.     
                if (exc1.GetType() == typeof(System.ComponentModel.Win32Exception))   
                {        
                    // sometimes throws exception so we have to just ignore     
                    // this is a common .NET bug that no one online really has a great reason for so now we just need to try to open      
                    // the URL using IE if we can.    
                    try      
                    {         
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("IExplore.exe", Url);       
                        System.Diagnostics.Process.Start(startInfo);       
                        startInfo = null;    
                    }     
                    catch (Exception exc2)         
                    {
                        ErrorLog.AddError(ErrorType.Failure, Strings.OpenUrl_CouldntOpenUrl);       
                    } 
                }  
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new OpenUrlConfig(Url); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            OpenUrlConfig config = configurationControl as OpenUrlConfig;
            Url = config.Url;
        }

        public string GetConfigString()
        {
            return String.Format(CultureInfo.CurrentCulture, Strings.OpenUrl_ConfigString, Url);
        }
    }
}
