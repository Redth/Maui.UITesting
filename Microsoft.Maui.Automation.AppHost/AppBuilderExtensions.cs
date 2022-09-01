using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Microsoft.Maui.Automation.Remote;
using Grpc.Core;

namespace Microsoft.Maui.Automation
{
    public static class AutomationAppBuilderExtensions
    {
        static GrpcRemoteAppHost host;

        static IApplication CreateApp(
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

            var multiApp = new MultiPlatformApplication(Platform.Maui, new[]
            {
                ( Platform.Maui, new MauiApplication(app)),
                ( platform, platformApp )
            });

            return multiApp;
        }

        public static void StartAutomationServiceListener(this Maui.IApplication mauiApplication, int port = 10882)
        {
            var address = IPAddress.Any;

            var multiApp = CreateApp(mauiApplication
#if ANDROID
                , (Android.App.Application.Context as Android.App.Application)
                    ?? Microsoft.Maui.MauiApplication.Current
#endif
                );

            host = new GrpcRemoteAppHost(multiApp);

            var server = new Server
            {
                Services = { RemoteGrpc.RemoteApp.BindService(host) },
                Ports = { new ServerPort(address.ToString(), port, ServerCredentials.Insecure) }
            };

            server.Start();
        }
    }
}
