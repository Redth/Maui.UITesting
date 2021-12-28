using Microsoft.Maui.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteAutomationTests
{
    public class MockApplication : MultiPlatformApplication
    {
        public MockApplication() : base()
        {
            PlatformApps.Add(Platform.MAUI, this);
        }

        public readonly List<MockWindow> MockWindows = new ();

        public MockWindow? CurrentMockWindow { get; set; }

        public override async Task<IWindow?> CurrentWindow()
            => CurrentMockWindow ?? (await Windows()).FirstOrDefault();

        public override Task<IWindow[]> Windows()
            => Task.FromResult(MockWindows.ToArray<IWindow>());

        public override Task<IWindow?> Window(string id)
            => Task.FromResult<IWindow?>(MockWindows.FirstOrDefault(w => w.Id == id));
    }
}