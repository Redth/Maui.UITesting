using Microsoft.UI.Xaml;

namespace Microsoft.Maui.Automation;

public static class AutomationContext
{
	public static readonly DependencyProperty AutomationUidProperty =
		DependencyProperty.Register(
			"AutomationUid",
			typeof(string),
			typeof(UIElement),
			new PropertyMetadata(string.Empty));

	public static string GetAutomationUid(this UIElement target)
		=> (string)target.GetValue(AutomationUidProperty);
	
	public static void SetAutomationUid(this UIElement target, string value) =>
		target.SetValue(AutomationUidProperty, value);

}
