using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public class MauiWindow : Window
    {
		internal MauiWindow(Maui.IWindow window) : base(Platform.MAUI, "")
        {
			PlatformWindow = window.ToAutomationWindow() ?? throw new PlatformNotSupportedException();
			PlatformElement = window;

			Id = PlatformWindow.Id;
			AutomationId = PlatformWindow.AutomationId;
			Type = window.GetType().Name;

			Width = PlatformWindow.Width;
			Height = PlatformWindow.Height;
			Text = PlatformWindow.Text;

			Children = window.GetChildren(Id);
		}

		[JsonIgnore]
		protected IWindow PlatformWindow { get; set; }
    }
}
