using Android.App;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
	public class AndroidWindow : Element
	{
		public AndroidWindow(IApplication application, Activity activity)
			: base(application, Platform.Android, activity.GetWindowId())
		{
			PlatformWindow = activity;
			AutomationId = activity.GetAutomationId();

			Children = activity.GetChildren(application, Id);
			X = (int)(activity.Window?.DecorView?.GetX() ?? -1f);
			Y = (int)(activity.Window?.DecorView?.GetY() ?? -1f);
			Width = activity.Window?.DecorView?.Width ?? -1;
			Height = activity.Window?.DecorView?.Height ?? -1;
			Text = PlatformWindow.Title;

			if (application is AndroidApplication androidApp)
			{
				var isCurrent = androidApp.IsActivityCurrent(activity);
				Visible = isCurrent;
				Enabled = isCurrent;
				Focused = isCurrent;
			}
		}

		[JsonIgnore]
		protected Activity PlatformWindow { get; set; }
	}
}
