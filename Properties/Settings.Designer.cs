//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MRWCalculator.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.2.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public uint InitCode {
            get {
                return ((uint)(this["InitCode"]));
            }
            set {
                this["InitCode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int DMSOffset {
            get {
                return ((int)(this["DMSOffset"]));
            }
            set {
                this["DMSOffset"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10000")]
        public int DMSOffset_Default {
            get {
                return ((int)(this["DMSOffset_Default"]));
            }
            set {
                this["DMSOffset_Default"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("8.333333")]
        public float DMSGain {
            get {
                return ((float)(this["DMSGain"]));
            }
            set {
                this["DMSGain"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("8.333333")]
        public float DMSGain_Default {
            get {
                return ((float)(this["DMSGain_Default"]));
            }
            set {
                this["DMSGain_Default"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public int[] TempCorrVal {
            get {
                return ((int[])(this["TempCorrVal"]));
            }
            set {
                this["TempCorrVal"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public int TempCorrVal_70_Default {
            get {
                return ((int)(this["TempCorrVal_70_Default"]));
            }
            set {
                this["TempCorrVal_70_Default"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.0005")]
        public float EModuleFactor {
            get {
                return ((float)(this["EModuleFactor"]));
            }
            set {
                this["EModuleFactor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.0005")]
        public float EModuleFactor_Default {
            get {
                return ((float)(this["EModuleFactor_Default"]));
            }
            set {
                this["EModuleFactor_Default"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string FileOpenPath {
            get {
                return ((string)(this["FileOpenPath"]));
            }
            set {
                this["FileOpenPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string FileOpenPathImport {
            get {
                return ((string)(this["FileOpenPathImport"]));
            }
            set {
                this["FileOpenPathImport"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public uint TempCorrBasedOnMRW {
            get {
                return ((uint)(this["TempCorrBasedOnMRW"]));
            }
            set {
                this["TempCorrBasedOnMRW"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TempCorrBasedOnSerialNb {
            get {
                return ((string)(this["TempCorrBasedOnSerialNb"]));
            }
            set {
                this["TempCorrBasedOnSerialNb"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public byte TempCorrBasedOnChannel {
            get {
                return ((byte)(this["TempCorrBasedOnChannel"]));
            }
            set {
                this["TempCorrBasedOnChannel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("COM1")]
        public string SerialPort {
            get {
                return ((string)(this["SerialPort"]));
            }
            set {
                this["SerialPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("9600")]
        public int Baudrate {
            get {
                return ((int)(this["Baudrate"]));
            }
            set {
                this["Baudrate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("8")]
        public byte DataBits {
            get {
                return ((byte)(this["DataBits"]));
            }
            set {
                this["DataBits"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("One")]
        public global::System.IO.Ports.StopBits StopBits {
            get {
                return ((global::System.IO.Ports.StopBits)(this["StopBits"]));
            }
            set {
                this["StopBits"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("500")]
        public ulong TimeoutGetData {
            get {
                return ((ulong)(this["TimeoutGetData"]));
            }
            set {
                this["TimeoutGetData"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public byte LoadCellType {
            get {
                return ((byte)(this["LoadCellType"]));
            }
            set {
                this["LoadCellType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public byte MaxReplies {
            get {
                return ((byte)(this["MaxReplies"]));
            }
            set {
                this["MaxReplies"] = value;
            }
        }
    }
}
