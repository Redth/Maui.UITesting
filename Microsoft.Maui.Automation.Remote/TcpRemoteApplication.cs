using System.Net;
using System.Net.Sockets;

namespace Microsoft.Maui.Automation.Remote
{
    public class TcpRemoteApplication : IApplication
    {
        public const int DefaultPort = 4327;

        public TcpRemoteApplication(IPAddress address, int port = DefaultPort, bool listen = true, IRemoteAutomationService? remoteAutomationService = null)
        {
            if (listen)
            {
                tcpListener = new TcpListener(address, port);
                tcpListener.Start();
                client = tcpListener.AcceptTcpClient();
                stream = client.GetStream();
                remoteApplication = new RemoteApplication(stream, remoteAutomationService);
                tcpListener.Stop();
            }
            else
            {
                tcpListener = null;
                client = new TcpClient();
                client.Connect(address, port);
                stream = client.GetStream();
                remoteApplication = new RemoteApplication(stream, remoteAutomationService);
            }
        }

        readonly Stream stream;
        readonly TcpListener? tcpListener;
        readonly RemoteApplication remoteApplication;
        readonly TcpClient client;

        public Task<IWindow?> CurrentWindow()
            => remoteApplication.CurrentWindow();

        public Task<IWindow[]> Windows()
            => remoteApplication.Windows();

        public IAsyncEnumerable<IView> Descendants(IElement of, Predicate<IView>? selector = null)
            => remoteApplication.Descendants(of, selector);

        public Task<IView?> Descendant(IElement of, Predicate<IView>? selector = null)
            => remoteApplication.Descendant(of, selector);

        public Task<IActionResult> Invoke(IView view, IAction action)
            => remoteApplication.Invoke(view, action);

        public Task<object?> GetProperty(IView view, string propertyName)
            => remoteApplication.GetProperty(view, propertyName);

        public Task<IWindow?> Window(string windowId)
            => remoteApplication.Window(windowId);

        public Task<IView?> View(string windowId, string viewId)
            => remoteApplication.View(windowId, viewId);

        public IAsyncEnumerable<IView> Descendants(string windowId, string? viewId = null)
            => remoteApplication.Descendants(windowId, viewId);

        public Task<IActionResult> Invoke(string windowId, string elementId, IAction action)
            => remoteApplication.Invoke(windowId, elementId, action);

        public Task<object?> GetProperty(string windowId, string elementId, string propertyName)
            => remoteApplication.GetProperty(windowId, elementId, propertyName);
    }
}