using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public class MauiWindow : Element
    {
		internal MauiWindow(IApplication application, Maui.IWindow window)
			: base(application, Platform.MAUI, "", parentId: null)
        {
			PlatformWindow = window.ToAutomationWindow(application) ?? throw new PlatformNotSupportedException();
			PlatformElement = window;

			Id = PlatformWindow.Id;
			AutomationId = PlatformWindow.AutomationId;
			Type = window.GetType().Name;

			Width = PlatformWindow.Width;
			Height = PlatformWindow.Height;
			Text = PlatformWindow.Text;

			Children = window.GetChildren(application, Id);
		}

		[JsonIgnore]
		protected IElement PlatformWindow { get; set; }
    }
}
