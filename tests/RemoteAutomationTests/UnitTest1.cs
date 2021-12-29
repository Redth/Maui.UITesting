using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;
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
            // Start the 'test runner' as a server, listening for the 'app/device' to connect
            var tcsHost = new TaskCompletionSource<TcpRemoteApplication>();
            _ = Task.Run(() =>
                tcsHost.TrySetResult(new TcpRemoteApplication(Platform.MAUI, IPAddress.Any)));

            // Build a mock app
            var app = new MockApplication()
                .WithWindow("window1", "Window", "Window Title")
                .WithView("view1");

            // Create our app host service implementation
            var service = new RemoteAutomationService(app);

            // Connect the 'app/device' as a client
            var device = new TcpRemoteApplication(Platform.MAUI, IPAddress.Loopback, listen: false, remoteAutomationService: service);

            // Wait until the client connects to the host to proceed
            var runner = await tcsHost.Task;

            var windows = new List<IElement>();
            // Query the remote host
            await foreach (var window in runner.Children(Platform.MAUI))
            {
                windows.Add(window);
            }

            Assert.NotEmpty(windows);
        }

        [Fact]
        public async Task IdSelectorTest()
        {
            // Start the 'test runner' as a server, listening for the 'app/device' to connect
            var tcsHost = new TaskCompletionSource<TcpRemoteApplication>();
            _ = Task.Run(() =>
                tcsHost.TrySetResult(new TcpRemoteApplication(Platform.MAUI, IPAddress.Any)));

            // Build a mock app
            var app = new MockApplication()
                .WithWindow("window1", "Window", "Window Title")
                .WithView("view1");

            var window = app.CurrentMockWindow;

            // Create our app host service implementation
            var service = new RemoteAutomationService(app);

            // Connect the 'app/device' as a client
            var device = new TcpRemoteApplication(Platform.MAUI, IPAddress.Loopback, listen: false, remoteAutomationService: service);

            // Wait until the client connects to the host to proceed
            var runner = await tcsHost.Task;

            var e = await runner.ById(Platform.MAUI, "view1");
            
            Assert.NotNull(e);
        }
    }
}