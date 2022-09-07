using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Automation.RemoteGrpc;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Platform;
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
		public MauiApplication(IApplication platformApp, Maui.IApplication mauiApp = default) : base()
		{
			PlatformApplication = platformApp;

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

		public readonly IApplication PlatformApplication;

		public override Task<IEnumerable<Element>> GetElements()
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

		public override Task<IEnumerable<Element>> FindElements(Func<Element, bool> matcher)
			=> Dispatch<IEnumerable<Element>>(() =>
			{
				var windows = new List<Element>();

				foreach (var window in MauiPlatformApplication.Windows)
				{
					var w = window.GetMauiElement(this, currentDepth: 1, maxDepth: 1);
					windows.Add(w);
				}

				var matches = new List<Element>();
				Traverse(windows, matches, matcher);

				return Task.FromResult<IEnumerable<Element>>(matches);
			});

		void Traverse(IEnumerable<Element> elements, IList<Element> matches, Func<Element, bool> matcher)
		{
			foreach (var e in elements)
			{
				if (matcher(e))
					matches.Add(e);

				if (e.PlatformElement is IView view)
				{
					var children = view.GetChildren(this, e.Id, 1, 1);
					Traverse(children, matches, matcher);
				}
				else if (e.PlatformElement is IWindow window)
				{
					var children = window.GetChildren(this, e.Id, 1, 1);
					Traverse(children, matches, matcher);
				}
			}
		}

		public override async Task<string> GetProperty(string elementId, string propertyName)
		{
			var element = await this.FirstById(elementId);

			return element.PlatformElement
				.GetType()
				.GetProperty(propertyName, System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
					?.GetValue(element.Platform)?.ToString() ?? String.Empty;
		}

		public override Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
			=> action switch
			{
				Actions.Tap => PerformTap(elementId, arguments),
				_ => throw new NotImplementedException()
			};

		async Task<PerformActionResult> PerformTap(string elementId, params string[] arguments)
		{
			if (!string.IsNullOrEmpty(elementId))
			{
				var element = await this.FirstById(elementId);

				if (element.PlatformElement is IElement mauiElement)
				{
#if ANDROID
					if (mauiElement.Handler?.PlatformView is Android.Views.View androidView)
						return await androidView.PerformAction(Actions.Tap, elementId, arguments);
#endif
				}
			}

			throw new NotImplementedException();
		}
	}
}

