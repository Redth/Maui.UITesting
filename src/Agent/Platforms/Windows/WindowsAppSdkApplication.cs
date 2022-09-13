using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Microsoft.Maui.Automation
{
    public class WindowsAppSdkApplication : Application
    {
        public override Platform DefaultPlatform => Platform.Winappsdk;


        public override Task<IEnumerable<Element>> GetElements()
            => Task.FromResult<IEnumerable<Element>>(new[] { UI.Xaml.Window.Current.GetElement(this, 1, -1) });

        public override async Task<string> GetProperty(string elementId, string propertyName)
        {
            var matches = await FindElements(e => e.Id?.Equals(elementId) ?? false);

            var match = matches?.FirstOrDefault();

            if (match is null)
                return "";

            return match.GetType().GetProperty(propertyName)?.GetValue(match)?.ToString() ?? string.Empty;
        }


        public override async Task<IEnumerable<Element>> FindElements(Predicate<Element> matcher)
        {
            var windows = new[] { UI.Xaml.Window.Current.GetElement(this, 1, 1) };

            var matches = new List<Element>();
            
            await Traverse(windows, matches, matcher);

            return matches;
        }

        public override Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
        {
            return Task.FromResult(PerformActionResult.Error());
        }

        async Task Traverse(IEnumerable<Element> elements, IList<Element> matches, Predicate<Element> matcher)
        {
            foreach (var e in elements)
            {
                if (matcher(e))
                    matches.Add(e);

                if (e.PlatformElement is FrameworkElement fwElement)
                {
                    var children = new List<Element>();

                    foreach (var child in fwElement.FindChildren(false))
                    {
                        var c = child.GetElement(this, e.Id, 1, 1);
                        children.Add(c);
                    }

                    await Traverse(children, matches, matcher);
                }
            }
        }
    }
}