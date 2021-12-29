using Microsoft.Maui.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteAutomationTests
{
    public class MockApplication : Application
    {
        public MockApplication(Platform defaultPlatform = Platform.MAUI) : base()
        {
            DefaultPlatform = defaultPlatform;
        }

        public readonly List<MockWindow> MockWindows = new ();

        public MockWindow? CurrentMockWindow { get; set; }

        public override Platform DefaultPlatform { get; }

        public override async IAsyncEnumerable<IElement> Children(Platform platform)
        {
            foreach (var w in MockWindows)
                yield return w;
        }

        public Func<Platform, string, IAction, Task<IActionResult>>? PerformHandler { get; set; }

        public override Task<IActionResult> Perform(Platform platform, string elementId, IAction action)
        {
            if (PerformHandler is not null)
            {
                return PerformHandler.Invoke(platform, elementId, action);
            }

            return Task.FromResult<IActionResult>(new ActionResult(ActionResultStatus.Error, "No Handler specified."));
            
        }
    }
}