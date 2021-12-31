using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public class WindowsAppSdkApplication : Application
    {
        public override Platform DefaultPlatform => Platform.WinAppSdk;

        public override Task<IActionResult> Perform(Platform platform, string elementId, IAction action)
            => RunOnMainThreadAsync(() =>
            {
                // TODO: Handle platform specific actions
                return Task.FromResult<IActionResult>(new ActionResult(ActionResultStatus.Unknown));
            });

        public override async Task<IEnumerable<IElement>> Children(Platform platform)
            => new List<IElement>() { await RunOnMainThreadAsync(() => Task.FromResult(new WindowsAppSdkWindow(this, UI.Xaml.Window.Current))) };

        public override Task<IEnumerable<IElement>> Descendants(Platform platform, string ofElementId = null, IElementSelector selector = null)
            => RunOnMainThreadAsync(() => base.Descendants(platform, ofElementId, selector));


        public override Task<object> GetProperty(Platform platform, string elementId, string propertyName)
            => RunOnMainThreadAsync(() => base.GetProperty(platform, elementId, propertyName));

        public override Task<IElement> Element(Platform platform, string elementId)
            => RunOnMainThreadAsync(() => base.Element(platform, elementId));

        async Task<T> RunOnMainThreadAsync<T>(Func<Task<T>> action)
        {
            var tcs = new TaskCompletionSource<T>();

#pragma warning disable VSTHRD101 // Avoid unsupported async delegates
            _ = UI.Xaml.Window.Current.Dispatcher.RunAsync(global::Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    tcs.TrySetResult(await action());
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });
#pragma warning restore VSTHRD101 // Avoid unsupported async delegates

            return await tcs.Task;
        }



    }
}