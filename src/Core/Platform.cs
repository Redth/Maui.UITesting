using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Microsoft.Maui.Automation
{
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
