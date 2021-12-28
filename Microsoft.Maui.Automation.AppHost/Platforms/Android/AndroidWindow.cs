using Android.App;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public class AndroidWindow : Window
    {
		public AndroidWindow(IApplication application, Activity activity)
			: base(application, Platform.Android, activity.GetWindowId())
        {
			PlatformWindow = activity;
			AutomationId = activity.GetAutomationId();

			Children = activity.GetChildren(application, Id);
			Width = activity.Window?.DecorView?.Width ?? -1;
			Height = activity.Window?.DecorView?.Height ?? -1;
			Text = PlatformWindow.Title;
		}

		[JsonIgnore]
		protected Activity PlatformWindow { get; set; }
    }
}
