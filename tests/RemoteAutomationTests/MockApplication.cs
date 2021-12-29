using Microsoft.Maui.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteAutomationTests
{
    public class MockApplication : MultiPlatformApplication
    {
        public MockApplication(Platform defaultPlatform = Platform.MAUI) : base(defaultPlatform)
        {
            PlatformApps.Add(defaultPlatform, this);
            CurrentPlatform = defaultPlatform;
        }

        public readonly List<MockWindow> MockWindows = new ();

        public MockWindow? CurrentMockWindow { get; set; }

        public Platform CurrentPlatform { get; set; }

        public override async IAsyncEnumerable<IElement> Children(Platform platform)
        {
            foreach (var w in MockWindows)
                yield return w;
        }
    }
}