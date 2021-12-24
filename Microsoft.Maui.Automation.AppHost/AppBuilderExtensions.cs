using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Microsoft.Maui.Automation.Remote;

namespace Microsoft.Maui.Automation
{
    public static class AutomationAppBuilderExtensions
	{
        static IRemoteAutomationService RemoteAutomationService;
        static IApplication App;

        static IMultiPlatformApplication CreateMultiApp(
            Maui.IApplication app
#if ANDROID
            , Android.App.Application application
#endif
            )
        {

            var platformApp = Automation.App.CreateForCurrentPlatform
                (
#if ANDROID
                application
#endif
                );

            var platform = Automation.App.GetCurrentPlatform();

            var multiApp = new MultiPlatformApplication(new[]
            {
                ( Platform.MAUI, new MauiApplication(app)),
                ( platform, platformApp )
            });

            return multiApp;
        }

        public static void StartAutomationServiceConnection(this Maui.IApplication mauiApplication, string host = null, int port = TcpRemoteApplication.DefaultPort)
        {
            IPAddress address = IPAddress.Any;
            if (!string.IsNullOrEmpty(host) && IPAddress.TryParse(host, out var ip))
                address = ip;

            var multiApp = CreateMultiApp(mauiApplication
#if ANDROID
                , (Android.App.Application.Context as Android.App.Application)
                    ?? Microsoft.Maui.MauiApplication.Current
#endif
                );

            RemoteAutomationService = new RemoteAutomationService(multiApp);
            App = new TcpRemoteApplication(address, port, false, RemoteAutomationService);
        }

        public static void StartAutomationServiceListener(this Maui.IApplication mauiApplication, int port = TcpRemoteApplication.DefaultPort)
        {
            var address = IPAddress.Any;

            var multiApp = CreateMultiApp(mauiApplication
#if ANDROID
                , (Android.App.Application.Context as Android.App.Application)
                    ?? Microsoft.Maui.MauiApplication.Current
#endif
                );

            RemoteAutomationService = new RemoteAutomationService(multiApp);
            App = new TcpRemoteApplication(address, port, true, RemoteAutomationService);
        }
    }
}
