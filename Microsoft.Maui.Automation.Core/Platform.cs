using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Microsoft.Maui.Automation
{
  //  public enum Platform
  //  {
  //      [EnumMember(Value = "MAUI")]
		//MAUI = 0,
        
  //      [EnumMember(Value = "iOS")]
  //      iOS = 100,
        
  //      [EnumMember(Value = "MacCatalyst")]
  //      MacCatalyst = 200,

  //      [EnumMember(Value = "macOS")]
  //      MacOS = 210,

  //      [EnumMember(Value = "tvOS")]
  //      tvOS = 300,

  //      [EnumMember(Value = "Android")]
  //      Android = 400,

  //      [EnumMember(Value = "WindowsAppSdk")]
  //      WinAppSdk = 500
  //  }

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
