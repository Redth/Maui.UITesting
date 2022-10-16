using Google.Protobuf;
using Google.Protobuf.Collections;
using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation
{
	public interface IElement
	{
		IApplication? Application { get; }
		string AutomationId { get; }
		IReadOnlyList<IElement> Children { get; }
		double Density { get; }
		bool Enabled { get; }
		bool Focused { get; }
		string FullType { get; }
		string Id { get; }
		string ParentId { get; }
		Platform Platform { get; }
		object? PlatformElement { get; }
		Frame ScreenFrame { get; }
		string Text { get; }
		string Type { get; }
		Frame ViewFrame { get; }
		bool Visible { get; }
		Frame WindowFrame { get; }
	}
}