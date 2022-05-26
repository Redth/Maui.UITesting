using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Microsoft.Maui.Automation
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Platform
    {
        [EnumMember(Value = "MAUI")]
		MAUI = 0,
        
        [EnumMember(Value = "iOS")]
        iOS = 1,
        
        [EnumMember(Value = "MacCatalyst")]
        MacCatalyst = 2,

        [EnumMember(Value = "macOS")]
        MacOS = 3,

        [EnumMember(Value = "tvOS")]
        tvOS = 4,

        [EnumMember(Value = "Android")]
        Android = 10,

        [EnumMember(Value = "WindowsAppSdk")]
        WinAppSdk = 20
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ActionResultStatus
    {
        [EnumMember(Value = "Unknown")]
        Unknown,

        [EnumMember(Value = "Ok")]
        Ok,

        [EnumMember(Value = "Error")]
        Error
    }
}
