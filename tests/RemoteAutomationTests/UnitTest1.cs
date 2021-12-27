using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace RemoteAutomationTests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            // Start the 'test runner' as a server, listening for the 'app/device' to connect
            var tcsHost = new TaskCompletionSource<TcpRemoteApplication>();
            _ = Task.Run(() =>
                tcsHost.TrySetResult(new TcpRemoteApplication(IPAddress.Any)));

            // Build a mock app
            var app = new MockApplication()
                .WithWindow("window1", "Window", "Window Title")
                .WithView("view1");

            // Create our app host service implementation
            var service = new RemoteAutomationService(app);

            // Connect the 'app/device' as a client
            var device = new TcpRemoteApplication(IPAddress.Loopback, listen: false, remoteAutomationService: service);

            // Wait until the client connects to the host to proceed
            var runner = await tcsHost.Task;

            // Query the remote host
            var windows = await runner.Windows();

            Assert.NotEmpty(windows);
        }
    }
}