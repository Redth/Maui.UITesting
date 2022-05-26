using Microsoft.Extensions.DependencyInjection;
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

		Task<TResult> Dispatch<TResult>(Func<TResult> action)
        {
			var tcs = new TaskCompletionSource<TResult>();

			var dispatcher = MauiPlatformApplication.Handler.MauiContext.Services.GetService<Dispatching.IDispatcher>() ?? throw new Exception("Unable to locate Dispatcher");

			dispatcher.Dispatch(() =>
			{
				try
				{
					var r = action();
					tcs.TrySetResult(r);
				}
				catch (Exception ex)
                {
					tcs.TrySetException(ex);
                }
			});

			return tcs.Task;
        }

		public override Platform DefaultPlatform => Platform.MAUI;

        public readonly Maui.IApplication MauiPlatformApplication;

		public override async Task<IEnumerable<IElement>> Children(Platform platform)
		{
			var windows = await Dispatch(() =>
			{
				var result = new List<MauiWindow>();

				foreach (var window in MauiPlatformApplication.Windows)
				{
					var w = new MauiWindow(this, window);
					result.Add(w);
				}

				return result;
			});

			return windows;
		}

        public override Task<IActionResult> Perform(Platform platform, string elementId, IAction action)
        {
            throw new NotImplementedException();
        }
    }
}

