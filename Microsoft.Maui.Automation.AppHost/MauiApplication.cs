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
		public MauiApplication() : base()
		{
#if ANDROID
            MauiPlatformApplication = Maui.MauiApplication.Current.Application;
#elif IOS || MACCATALYST
			MauiPlatformApplication = MauiUIApplicationDelegate.Current.Application;
#elif WINDOWS
			MauiPlatformApplication = MauiWinUIApplication.Current.Application;
#endif
		}

		public MauiApplication(Maui.IApplication mauiPlatformApplication) : base()
		{
			MauiPlatformApplication = mauiPlatformApplication;
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

		public readonly Maui.IApplication MauiPlatformApplication;

		public override Task<IWindow[]> Windows()
			=> Dispatch(() => MauiPlatformApplication.Windows.Select(w => new MauiWindow(this, w)).ToArray<IWindow>());

        public override Task<IWindow> CurrentWindow()
            => Task.FromResult<IWindow>(new MauiWindow(this, MauiPlatformApplication.Windows.First()));

        public override Task<IActionResult> Invoke(IView view, IAction action)
        {
            throw new NotImplementedException();
        }
    }
}

