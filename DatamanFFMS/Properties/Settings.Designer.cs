﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AstralFFMS.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://175.111.183.68:9047/selzer_live/WS/Shell%20Organics_live_2019%20/Page/Inte" +
            "gration_Customer_Card")]
        public string DatamanFFMS_WebReference_Integration_Customer_Card_Service {
            get {
                return ((string)(this["DatamanFFMS_WebReference_Integration_Customer_Card_Service"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://175.111.183.68:9047/selzer_live/WS/Shell%20Organics_live_2019%20/Page/Inte" +
            "gration_Sales_Order")]
        public string DatamanFFMS_SaleOrderReference_Integration_Sales_Order_Service {
            get {
                return ((string)(this["DatamanFFMS_SaleOrderReference_Integration_Sales_Order_Service"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://52.183.136.250:6048/baker_live/WS/Bakers%20Agrifoods%20Pvt%20Ltd/Page/Inte" +
            "gration_Sales_Order")]
        public string DatamanFFMS_Baker_SalesOrderLive_Integration_Sales_Order_Service {
            get {
                return ((string)(this["DatamanFFMS_Baker_SalesOrderLive_Integration_Sales_Order_Service"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://52.183.136.250:9047/bakersagrifoods_demo/WS/Bakers%20Agrifoods%20Pvt%20Ltd" +
            "/Page/Integration_Sales_Order")]
        public string DatamanFFMS_Baker_SalesOrderDemo_Integration_Sales_Order_Service {
            get {
                return ((string)(this["DatamanFFMS_Baker_SalesOrderDemo_Integration_Sales_Order_Service"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://175.111.183.68:9047/selzer_live/WS/Shell%20Organics_live_2019%20/Page/Inte" +
            "gration_Customer_Card")]
        public string DatamanFFMS_PartyReference_Integration_Customer_Card_Service {
            get {
                return ((string)(this["DatamanFFMS_PartyReference_Integration_Customer_Card_Service"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://103.66.74.126:9047/selzer_live/WS/Selzer%20Innovex%20Pvt.%20Ltd./Page/Inte" +
            "gration_Sales_Order")]
        public string DatamanFFMS_Selzer_SalesorderLive_Integration_Sales_Order_Service {
            get {
                return ((string)(this["DatamanFFMS_Selzer_SalesorderLive_Integration_Sales_Order_Service"]));
            }
        }
    }
}