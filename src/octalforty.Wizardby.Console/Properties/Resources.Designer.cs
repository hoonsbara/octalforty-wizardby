﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3082
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace octalforty.Wizardby.Console.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("octalforty.Wizardby.Console.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Ambiguous command &apos;{0}&apos;. Could be &apos;{1}&apos;..
        /// </summary>
        internal static string AmbiguousCommand {
            get {
                return ResourceManager.GetString("AmbiguousCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection string: {0}.
        /// </summary>
        internal static string ConnectionStringInformation {
            get {
                return ResourceManager.GetString("ConnectionStringInformation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to octalforty Wizardby {0} Alpha 2
        ///Copyright (c) 2009 octalforty studios.
        /// </summary>
        internal static string CopyrightInformation {
            get {
                return ResourceManager.GetString("CopyrightInformation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find &apos;database.wdi&apos; in &apos;{0}&apos;..
        /// </summary>
        internal static string CouldNotFindDatabaseWdi {
            get {
                return ResourceManager.GetString("CouldNotFindDatabaseWdi", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not resolve Platform Alias &apos;{0}&apos;..
        /// </summary>
        internal static string CouldNotResolvePlatformAlias {
            get {
                return ResourceManager.GetString("CouldNotResolvePlatformAlias", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Current database version: {0}.
        /// </summary>
        internal static string CurrentDatabaseVersionInfo {
            get {
                return ResourceManager.GetString("CurrentDatabaseVersionInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database is not versioned or no migrations were applied yet..
        /// </summary>
        internal static string DatabaseIsNotVersioned {
            get {
                return ResourceManager.GetString("DatabaseIsNotVersioned", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Downgraded to version {0} ({1:N2} sec.).
        /// </summary>
        internal static string DowngradedToVersion {
            get {
                return ResourceManager.GetString("DowngradedToVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Downgrading from version {0}.
        /// </summary>
        internal static string DowngradingFromVersion {
            get {
                return ResourceManager.GetString("DowngradingFromVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Environment &apos;{0}&apos; does not allow downgrades to be performed. See `{1}` property in &apos;database.wdi&apos;..
        /// </summary>
        internal static string EnvironmentDoesNotAllowDowngrades {
            get {
                return ResourceManager.GetString("EnvironmentDoesNotAllowDowngrades", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Environment: {0}.
        /// </summary>
        internal static string EnvironmentInformation {
            get {
                return ResourceManager.GetString("EnvironmentInformation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generated &apos;{0}&apos;..
        /// </summary>
        internal static string GeneratedFile {
            get {
                return ResourceManager.GetString("GeneratedFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generated directory for Native SQL resources: &apos;{0}&apos;.
        /// </summary>
        internal static string GeneratedNativeSqlResourcesDirectory {
            get {
                return ResourceManager.GetString("GeneratedNativeSqlResourcesDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generated version {0}..
        /// </summary>
        internal static string GeneratedVersion {
            get {
                return ResourceManager.GetString("GeneratedVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to migration &quot;{0}&quot; revision =&gt; 1:
        ///    defaults:
        ///        default-primary-key ID type =&gt; Int32, primary-key =&gt; true, nullable =&gt; false
        ///
        ///    version {1}:
        ///        /* Start writing migrations */.
        /// </summary>
        internal static string MdlTemplate {
            get {
                return ResourceManager.GetString("MdlTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Migration Definition: {0}.
        /// </summary>
        internal static string MigrationDefinitionInformation {
            get {
                return ResourceManager.GetString("MigrationDefinitionInformation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No Migration Definition found in &apos;{0}&apos;..
        /// </summary>
        internal static string NoMigrationDefinition {
            get {
                return ResourceManager.GetString("NoMigrationDefinition", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Platform: {0}.
        /// </summary>
        internal static string PlatformInformation {
            get {
                return ResourceManager.GetString("PlatformInformation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to     {0}.
        /// </summary>
        internal static string RegisteredDatabaseVersionInfo {
            get {
                return ResourceManager.GetString("RegisteredDatabaseVersionInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Registered versions:.
        /// </summary>
        internal static string RegisteredDatabaseVersionsInfo {
            get {
                return ResourceManager.GetString("RegisteredDatabaseVersionsInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Registered version {0}.
        /// </summary>
        internal static string RegisteredVersion {
            get {
                return ResourceManager.GetString("RegisteredVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown command &apos;{0}&apos;..
        /// </summary>
        internal static string UnknownCommand {
            get {
                return ResourceManager.GetString("UnknownCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Upgraded to version {0} ({1:N2} sec.).
        /// </summary>
        internal static string UpgradedToVersion {
            get {
                return ResourceManager.GetString("UpgradedToVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Upgrading to version {0}.
        /// </summary>
        internal static string UpgradingToVersion {
            get {
                return ResourceManager.GetString("UpgradingToVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Usage: 
        ///    wizardby &lt;command&gt; &lt;version-or-step&gt; [/mdl:&lt;mdl-file&gt;] 
        ///        [/connection:&lt;connection-string&gt;] [/platform:&lt;platform-alias&gt;] 
        ///        [/environment:&lt;environment-name&gt;]
        ///				
        ///    /mdl or /m          - Specifies the name of a MDL file	 
        ///    /connection or /c   - Specifies the connection string
        ///    /platform or /p     - Specifies the platform alias
        ///    /environment or /e  - Specifies the environment name or prefix 
        ///                          within a &quot;database.wdi&quot; file.
        ///	
        ///    If &quot;/m&quot; is [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string UsageInformation {
            get {
                return ResourceManager.GetString("UsageInformation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to deployment:
        ///    environment development
        ///        platform            =&gt; sqlserver2005
        ///        host                =&gt; &quot;(local)\sqlexpress&quot;
        ///        database            =&gt; {0}
        ///        integrated-security =&gt; true
        ///        
        ///    environment staging
        ///        platform            =&gt; sqlserver2005
        ///        host                =&gt; &quot;(local)\sqlexpress&quot;
        ///        database            =&gt; {0}_staging
        ///        integrated-security =&gt; true
        ///        
        ///    environment production
        ///        platform            =&gt; sqlserver2005        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string WdiTemplate {
            get {
                return ResourceManager.GetString("WdiTemplate", resourceCulture);
            }
        }
    }
}
