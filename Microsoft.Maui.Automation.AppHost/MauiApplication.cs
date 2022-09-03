using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Automation.RemoteGrpc;
using Microsoft.Maui.Dispatching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
	public class MauiApplication : Application
	{
		public MauiApplication(Maui.IApplication? mauiApp = default) : base()
		{
			MauiPlatformApplication = mauiApp
				?? App.GetCurrentMauiApplication() ?? throw new PlatformNotSupportedException();
		}

		Task<TResult> Dispatch<TResult>(Func<Task<TResult>> action)
		{
			var dispatcher = MauiPlatformApplication.Handler.MauiContext.Services.GetService<Dispatching.IDispatcher>() ?? throw new Exception("Unable to locate Dispatcher");

			return dispatcher.DispatchAsync(action);
		}

		public override Platform DefaultPlatform => Platform.Maui;

		public readonly Maui.IApplication MauiPlatformApplication;

		public override Task<IEnumerable<Element>> GetElements(Platform platform)
			=> Dispatch<IEnumerable<Element>>(() =>
			{
				var windows = new List<Element>();

				foreach (var window in MauiPlatformApplication.Windows)
				{
					var w = window.GetMauiElement(this, currentDepth: 1, maxDepth: -1);
					windows.Add(w);
				}

				return Task.FromResult<IEnumerable<Element>>(windows);
			});

		public override Task<IEnumerable<Element>> FindElements(Platform platform, Func<Element, bool> matcher)
			=> Dispatch<IEnumerable<Element>>(() =>
			{
				var windows = new List<Element>();

				foreach (var window in MauiPlatformApplication.Windows)
				{
					var w = window.GetMauiElement(this, currentDepth: 1, maxDepth: 1);
					windows.Add(w);
				}

				var matches = new List<Element>();
				Traverse(platform, windows, matches, matcher);

				return Task.FromResult<IEnumerable<Element>>(matches);
			});

		void Traverse(Platform platform, IEnumerable<Element> elements, IList<Element> matches, Func<Element, bool> matcher)
		{
			foreach (var e in elements)
			{
				if (matcher(e))
					matches.Add(e);

				if (e.PlatformElement is IView view)
				{
					var children = view.GetChildren(this, e.Id, 1, 1);
					Traverse(platform, children, matches, matcher);
				}
				else if (e.PlatformElement is IWindow window)
				{
					var children = window.GetChildren(this, e.Id, 1, 1);
					Traverse(platform, children, matches, matcher);
				}
			}
		}

		public override Task<string> GetProperty(Platform platform, string elementId, string propertyName)
		{
			throw new NotImplementedException();
		}
	}
}

