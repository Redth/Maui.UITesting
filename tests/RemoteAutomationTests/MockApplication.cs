using Microsoft.Maui.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RemoteAutomationTests
{
    public class MockApplication : Application
    {
        public MockApplication(Platform defaultPlatform = Platform.Maui) : base()
        {
            DefaultPlatform = defaultPlatform;
        }

        public readonly List<Element> MockWindows = new();

        public Element? CurrentMockWindow { get; set; }

        public override Platform DefaultPlatform { get; }

        public Func<Platform, string, IAction, Task<IActionResult>>? PerformHandler { get; set; }

        //public override Task<IActionResult> Perform(Platform platform, string elementId, IAction action)
        //{
        //    if (PerformHandler is not null)
        //    {
        //        return PerformHandler.Invoke(platform, elementId, action);
        //    }

        //    return Task.FromResult<IActionResult>(new ActionResult(ActionResultStatus.Error, "No Handler specified."));

        //}

        public override Task<string> GetProperty(Platform platform, string elementId, string propertyName)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<Element>> GetElements(Platform platform)
            => Task.FromResult<IEnumerable<Element>>(MockWindows);

        public override Task<IEnumerable<Element>> FindElements(Platform platform, Func<Element, bool> matcher)
        {
            var windows = MockWindows;

            var matches = new List<Element>();
            Traverse(platform, windows, matches, matcher);

            return Task.FromResult<IEnumerable<Element>>(matches);
        }

        void Traverse(Platform platform, IEnumerable<Element> elements, IList<Element> matches, Func<Element, bool> matcher)
        {
            foreach (var e in elements)
            {
                if (matcher(e))
                    matches.Add(e);

                Traverse(platform, e.Children, matches, matcher);
            }
        }
    }
}