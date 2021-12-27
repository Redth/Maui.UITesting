using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Maui.Automation
{
    public interface IMultiPlatformApplication : IApplication
    {
        public Task Platform(Platform platform);
        public Task<Platform> CurrentPlatform();
    }

    public interface IApplication : IApplicationInterop
    {
        public IAsyncEnumerable<IView> Descendants(IElement of, Predicate<IView>? selector = null);

        public Task<IView?> Descendant(IElement of, Predicate<IView>? selector = null);

        public Task<IActionResult> Invoke(IView view, IAction action);

        public Task<object?> GetProperty(IView element, string propertyName);
    }
}