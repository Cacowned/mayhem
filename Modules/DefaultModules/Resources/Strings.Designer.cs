﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DefaultModules.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DefaultModules.Resources.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot monitor the folder; it does not exist..
        /// </summary>
        internal static string FolderChange_FolderDoesntExist {
            get {
                return ResourceManager.GetString("FolderChange_FolderDoesntExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid url.
        /// </summary>
        internal static string Internet_InvalidUrl {
            get {
                return ResourceManager.GetString("Internet_InvalidUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is no Internet connection; {0} Alert event will start when Internet is detected..
        /// </summary>
        internal static string Internet_NotConnected {
            get {
                return ResourceManager.GetString("Internet_NotConnected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Url: &quot;{0}&quot;.
        /// </summary>
        internal static string OpenUrl_ConfigString {
            get {
                return ResourceManager.GetString("OpenUrl_ConfigString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not open the URL.
        /// </summary>
        internal static string OpenUrl_CouldntOpenUrl {
            get {
                return ResourceManager.GetString("OpenUrl_CouldntOpenUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} Enter the path for the audio file:.
        /// </summary>
        internal static string PlaySound_CliConfig_AudioPath {
            get {
                return ResourceManager.GetString("PlaySound_CliConfig_AudioPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The sound file doesn&apos;t exist..
        /// </summary>
        internal static string PlaySound_FileNotFound {
            get {
                return ResourceManager.GetString("PlaySound_FileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not start the application.
        /// </summary>
        internal static string RunProgram_CantStartProgram {
            get {
                return ResourceManager.GetString("RunProgram_CantStartProgram", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File name: &quot;{0}&quot;.
        /// </summary>
        internal static string RunProgram_ConfigString {
            get {
                return ResourceManager.GetString("RunProgram_ConfigString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The application does not exist at the specified location..
        /// </summary>
        internal static string RunProgram_FileNotFound {
            get {
                return ResourceManager.GetString("RunProgram_FileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Message: &quot;{0}&quot;.
        /// </summary>
        internal static string TextToSpeech_ConfigString {
            get {
                return ResourceManager.GetString("TextToSpeech_ConfigString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t set timer interval. Exception: {0}.
        /// </summary>
        internal static string Timer_CantSetInterval {
            get {
                return ResourceManager.GetString("Timer_CantSetInterval", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} Please enter the number of hours to wait:.
        /// </summary>
        internal static string Timer_CliConfig_HoursToWait {
            get {
                return ResourceManager.GetString("Timer_CliConfig_HoursToWait", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} Please enter the number of minutes to wait:.
        /// </summary>
        internal static string Timer_CliConfig_MinutesToWait {
            get {
                return ResourceManager.GetString("Timer_CliConfig_MinutesToWait", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} Please enter the number of seconds to wait:.
        /// </summary>
        internal static string Timer_CliConfig_SecondsToWait {
            get {
                return ResourceManager.GetString("Timer_CliConfig_SecondsToWait", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} hours, {1} minutes, {2} seconds.
        /// </summary>
        internal static string Timer_ConfigString {
            get {
                return ResourceManager.GetString("Timer_ConfigString", resourceCulture);
            }
        }
    }
}
