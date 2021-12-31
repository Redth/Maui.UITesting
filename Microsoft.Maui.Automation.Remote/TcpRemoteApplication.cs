using System.Net;
using System.Net.Sockets;

namespace Microsoft.Maui.Automation.Remote
{
    public class TcpRemoteApplication : IApplication
    {
        public const int DefaultPort = 4327;

        public TcpRemoteApplication(Platform defaultPlatform, IPAddress address, int port = DefaultPort, bool listen = true, IRemoteAutomationService? remoteAutomationService = null)
        {
            DefaultPlatform = defaultPlatform;
            if (listen)
            {
                tcpListener = new TcpListener(address, port);
                tcpListener.Start();
                client = tcpListener.AcceptTcpClient();
                stream = client.GetStream();
                remoteApplication = new RemoteApplication(DefaultPlatform, stream, remoteAutomationService);
                tcpListener.Stop();
            }
            else
            {
                tcpListener = null;
                client = new TcpClient();
                client.Connect(address, port);
                stream = client.GetStream();
                remoteApplication = new RemoteApplication(DefaultPlatform, stream, remoteAutomationService);
            }
        }

        readonly Stream stream;
        readonly TcpListener? tcpListener;
        readonly RemoteApplication remoteApplication;
        readonly TcpClient client;

        public Platform DefaultPlatform { get; }

        public Task<IEnumerable<IElement>> Children(Platform platform)
            => remoteApplication.Children(platform);

        public Task<IElement?> Element(Platform platform, string elementId)
            => remoteApplication.Element(platform, elementId);

        public Task<IEnumerable<IElement>> Descendants(Platform platform, string? elementId = null, IElementSelector? selector = null)
            => remoteApplication.Descendants(platform, elementId, selector);

        public Task<IActionResult> Perform(Platform platform, string elementId, IAction action)
            => remoteApplication.Perform(platform, elementId, action);

        public Task<object?> GetProperty(Platform platform, string elementId, string propertyName)
            => remoteApplication.GetProperty(platform, elementId, propertyName);
    }
}