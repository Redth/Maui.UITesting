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
		public AndroidWindow(Activity activity)
			: base(Platform.Android, activity.Window?.DecorView?.Id.ToString() ?? activity.ComponentName.FlattenToString())
        {
			PlatformWindow = activity;
			AutomationId = activity.Window?.DecorView?.GetAutomationId() ?? Id;

			Children = activity.GetChildren(Id);
			Width = activity.Window?.DecorView?.Width ?? -1;
			Height = activity.Window?.DecorView?.Height ?? -1;
			Text = PlatformWindow.Title;
		}

		[JsonIgnore]
		protected Activity PlatformWindow { get; set; }
    }
}
