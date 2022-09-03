using Microsoft.Maui.Automation;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace RemoteAutomationTests
{
    public class UnitTest1
    {
        [Fact]
        public async Task ListWindowsTest()
        {
            // Build a mock app
            var app = new MockApplication()
                .WithWindow("window1", "Window", "Window Title")
                .WithView("view1");
            
            var windows = new List<Element>();
            var elems = await app.GetElements(Platform.Maui);

            // Query the remote host
            foreach (var window in elems)
            {
                windows.Add(window);
            }

            Assert.NotEmpty(windows);
        }
    }
}