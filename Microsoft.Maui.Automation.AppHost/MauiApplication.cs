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
            Application = Maui.MauiApplication.Current.Application;
#elif IOS || MACCATALYST
			Application = MauiUIApplicationDelegate.Current.Application;
#elif WINDOWS
			Application = MauiWinUIApplication.Current.Application;
#endif
		}

		public MauiApplication(Maui.IApplication application) : base()
		{
			Application = application;
        }

		Task<TResult> Dispatch<TResult>(Func<TResult> action)
        {
			var tcs = new TaskCompletionSource<TResult>();

			var dispatcher = Application.Handler.MauiContext.Services.GetService<Dispatching.IDispatcher>() ?? throw new Exception("Unable to locate Dispatcher");

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

		public readonly Maui.IApplication Application;

		public override Task<IWindow[]> Windows()
			=> Dispatch(() => Application.Windows.Select(w => new MauiWindow(w)).ToArray<IWindow>());

        public override Task<IWindow> CurrentWindow()
            => Task.FromResult<IWindow>(new MauiWindow(Application.Windows.First()));

        public override Task<IActionResult> Invoke(IView view, IAction action)
        {
            throw new NotImplementedException();
        }
    }
}

