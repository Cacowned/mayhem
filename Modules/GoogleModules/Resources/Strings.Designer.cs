﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GoogleModules.Resources {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GoogleModules.Resources.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to AIzaSyBED0KUmlEcX8qEmv7AYZtUsd6A0fNRpds.
        /// </summary>
        internal static string GooglePlus_ApiKey {
            get {
                return ResourceManager.GetString("GooglePlus_ApiKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error at monitoring the Google+ feed activity..
        /// </summary>
        internal static string GooglePlus_ErrorMonitoringNewActivity {
            get {
                return ResourceManager.GetString("GooglePlus_ErrorMonitoringNewActivity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error at monitoring the Google+ comments..
        /// </summary>
        internal static string GooglePlus_ErrorMonitoringNewComment {
            get {
                return ResourceManager.GetString("GooglePlus_ErrorMonitoringNewComment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Profile ID must contain at least one character..
        /// </summary>
        internal static string GooglePlus_ProfileID_NoCharacter {
            get {
                return ResourceManager.GetString("GooglePlus_ProfileID_NoCharacter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Profile ID is too long..
        /// </summary>
        internal static string GooglePlus_ProfileID_TooLong {
            get {
                return ResourceManager.GetString("GooglePlus_ProfileID_TooLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Profile ID is incorrect..
        /// </summary>
        internal static string GooglePlus_ProfileIDIncorrect {
            get {
                return ResourceManager.GetString("GooglePlus_ProfileIDIncorrect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Google+: New Activity.
        /// </summary>
        internal static string GooglePlusNewActivityTitle {
            get {
                return ResourceManager.GetString("GooglePlusNewActivityTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Google+: Receive Comment.
        /// </summary>
        internal static string GooglePlusNewCommentTitle {
            get {
                return ResourceManager.GetString("GooglePlusNewCommentTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Profile ID: {0}..
        /// </summary>
        internal static string ProfileID_ConfigString {
            get {
                return ResourceManager.GetString("ProfileID_ConfigString", resourceCulture);
            }
        }
    }
}
