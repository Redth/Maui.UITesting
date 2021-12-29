using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public class ElementNotFoundException : Exception
    {
        public ElementNotFoundException(string elementId)
            : base($"Element with the ID: '{elementId}' was not found.")
        {
            ElementId = elementId;
        }

        public readonly string ElementId;
    }

    public abstract class Application : IApplication
    {
        public virtual void Close()
        {
        }

        ~Application()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            Dispose(true);
        }

        bool disposed;
        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    DisposeManagedResources();
                DisposeUnmanagedResources();
                disposed = true;
            }
        }

        protected virtual void DisposeManagedResources()
        {
        }

        protected virtual void DisposeUnmanagedResources()
        {
        }

        public abstract Platform DefaultPlatform { get; }

        public abstract IAsyncEnumerable<IElement> Children(Platform platform);

        public abstract Task<IActionResult> Perform(Platform platform, string elementId, IAction action);

        protected async Task<IElement> FindElementOrThrow(Platform platform, string elementId)
        {
            var element = await Element(platform, elementId);

            if (element is null)
                throw new ElementNotFoundException(elementId);

            return element;
        }

        public virtual async Task<object?> GetProperty(Platform platform, string elementId, string propertyName)
        {
            var element = await FindElementOrThrow(platform, elementId);

            var t = element.PlatformElement?.GetType();

            if (t != null)
            {
                var prop = t.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                if (prop != null)
                {
                    return Task.FromResult(prop.GetValue(element.PlatformElement));
                }
            }

            return Task.FromResult<object?>(null);
        }

        public virtual async Task<IElement?> Element(Platform platform, string elementId)
        {
            await foreach (var d in Descendants(platform, elementId))
            {
                return d;
            }

            return null;
        }

        public async IAsyncEnumerable<IElement> Descendants(Platform platform, string? ofElementId = null, IElementSelector? selector = null)
        {
            if (string.IsNullOrEmpty(ofElementId))
            {
                await foreach (var c in Children(platform).FindBreadthFirst(selector))
                    yield return c;
            }
            else
            {
                var element = await FindElementOrThrow(platform, ofElementId);

                await foreach (var c in element.Children.AsEnumerable().FindBreadthFirst(selector))
                    yield return c;
            }
        }
    }
}
