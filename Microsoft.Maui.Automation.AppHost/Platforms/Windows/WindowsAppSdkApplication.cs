using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public class WindowsAppSdkApplication : Application
    {
        public override Platform DefaultPlatform => Platform.Winappsdk;

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

        public override async Task<IEnumerable<Element>> GetElements(Platform platform, string elementId = null, int depth = 0)
        {
            var root = await GetRootElements();

            if (string.IsNullOrEmpty(elementId))
                return root;

            return root.FindDepthFirst(new IdSelector(elementId));
        }

        public override async Task<string> GetProperty(Platform platform, string elementId, string propertyName)
        {
            var roots = await GetRootElements();

            var element = roots.FindDepthFirst(new IdSelector(elementId))?.FirstOrDefault();

            return element.GetType().GetProperty(propertyName)?.GetValue(element)?.ToString() ?? string.Empty;
        }

        async Task<IEnumerable<Element>> GetRootElements()
        {
            var e = await RunOnMainThreadAsync(() => Task.FromResult(UI.Xaml.Window.Current.GetElement(this)));
            return new[] { e };
        }
    }
}