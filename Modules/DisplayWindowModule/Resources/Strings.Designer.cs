﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.261
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DisplayWindowModule.Resources {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DisplayWindowModule.Resources.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to The window could not be created..
        /// </summary>
        internal static string DispayWindow_CantCreateWindow {
            get {
                return ResourceManager.GetString("DispayWindow_CantCreateWindow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The reaction could not be performed.
        /// </summary>
        internal static string DisplayWindow_CantPerformReaction {
            get {
                return ResourceManager.GetString("DisplayWindow_CantPerformReaction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Message: {0}..
        /// </summary>
        internal static string DisplayWindow_ConfigString {
            get {
                return ResourceManager.GetString("DisplayWindow_ConfigString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The message must contain at least one character..
        /// </summary>
        internal static string DisplayWindow_Message_NoCharacter {
            get {
                return ResourceManager.GetString("DisplayWindow_Message_NoCharacter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The message must be 100 characters long or smaller..
        /// </summary>
        internal static string DisplayWindow_Message_TooLong {
            get {
                return ResourceManager.GetString("DisplayWindow_Message_TooLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of sencods can&apos;t be 0..
        /// </summary>
        internal static string DisplayWindow_Seconds_GreaterThanZero {
            get {
                return ResourceManager.GetString("DisplayWindow_Seconds_GreaterThanZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of seconds is invalid..
        /// </summary>
        internal static string DisplayWindow_SecondsInvalid {
            get {
                return ResourceManager.GetString("DisplayWindow_SecondsInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Display Window.
        /// </summary>
        internal static string DisplayWindow_Title {
            get {
                return ResourceManager.GetString("DisplayWindow_Title", resourceCulture);
            }
        }
    }
}
