﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.237
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OfficeModules.Resources
{


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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("OfficeModules.Resources.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to Unable to find the open Outlook application..
        /// </summary>
        internal static string Outlook_ApplicationNotFound {
            get {
                return ResourceManager.GetString("Outlook_ApplicationNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to find the open PowerPoint application..
        /// </summary>
        internal static string PowerPoint_ApplicationNotFound {
            get {
                return ResourceManager.GetString("PowerPoint_ApplicationNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t go to the next slide..
        /// </summary>
        internal static string PowerPoint_CantChangeSlidesNext {
            get {
                return ResourceManager.GetString("PowerPoint_CantChangeSlidesNext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t go to the previous slide..
        /// </summary>
        internal static string PowerPoint_CantChangeSlidesPrevious {
            get {
                return ResourceManager.GetString("PowerPoint_CantChangeSlidesPrevious", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to More than one slideshow window detected, using the first one..
        /// </summary>
        internal static string PowerPoint_MoreThanOneWindow {
            get {
                return ResourceManager.GetString("PowerPoint_MoreThanOneWindow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must be in a slideshow view to change slide..
        /// </summary>
        internal static string PowerPoint_NoWindowCantChange {
            get {
                return ResourceManager.GetString("PowerPoint_NoWindowCantChange", resourceCulture);
            }
        }
    }
}
