using Microsoft.Maui.Automation.Remote;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public class RemoteAutomationService : IRemoteAutomationService
    {
        public RemoteAutomationService(IApplication app)
        {
            PlatformApp = app;
        }
        
        readonly IApplication PlatformApp;



        public async Task<RemoteElement?> Element(Platform platform, string elementId)
        {
            var platformElement = await PlatformApp.Element(platform, elementId);

            if (platformElement is null)
                return null;

            return new RemoteElement(PlatformApp, platformElement, platformElement.ParentId);
        }

        public async Task<RemoteElement[]> Children(Platform platform)
        {
            var children = new List<RemoteElement>();

            await foreach (var c in PlatformApp.Children(platform))
            {
                children.Add(new RemoteElement(PlatformApp, c, c.ParentId));
            }

            return children.ToArray();
        }

        public Task<IActionResult> Perform(Platform platform, string elementId, IAction action)
            => PlatformApp.Perform(platform, elementId, action);

        public Task<object?> GetProperty(Platform platform, string elementId, string propertyName)
            => PlatformApp.GetProperty(platform, elementId, propertyName);

        void ConvertChildren(RemoteElement parent, IEnumerable<IElement> toConvert, IElementSelector? selector, string? parentId = null)
        {
            selector ??= new DefaultViewSelector();

            var converted = toConvert
                .Where(c => selector.Matches(c))
                .Select(c => new RemoteElement(PlatformApp, c, parentId)!);

            parent.SetChildren(converted);

            foreach (var v in converted)
                ConvertChildren(v, v.Children, selector);
        }

        public async Task<RemoteElement[]> Descendants(Platform platform, string? elementId = null, IElementSelector? selector = null)
        {
            if (!string.IsNullOrEmpty(elementId))
            {
                var view = await PlatformApp.Element(platform, elementId);

                if (view == null)
                    return Array.Empty<RemoteElement>();

                var remoteView = new RemoteElement(PlatformApp, view, view.ParentId);

                ConvertChildren(remoteView, remoteView.Children, selector);

                return remoteView.Children.Cast<RemoteElement>().ToArray();
            }
            else
            {
                var children = new List<RemoteElement>();

                foreach (var c in await Children(platform))
                {
                    var remoteView = new RemoteElement(PlatformApp, c, c.ParentId);

                    ConvertChildren(remoteView, remoteView.Children, selector);

                    children.Add(c);
                }

                return children.ToArray();
            }
        }
    }
}
