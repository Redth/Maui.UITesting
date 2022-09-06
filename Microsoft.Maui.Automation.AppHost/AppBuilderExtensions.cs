using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Grpc.Core;
using System.Net.Http;

namespace Microsoft.Maui.Automation
{
    public static class AutomationAppBuilderExtensions
    {
        static GrpcRemoteAppAgent client;

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

        public static void StartAutomationServiceListener(this Maui.IApplication mauiApplication, string address)
        {
            var multiApp = CreateApp(mauiApplication
#if ANDROID
                , (Android.App.Application.Context as Android.App.Application)
                    ?? Microsoft.Maui.MauiApplication.Current
#endif
                );

#if ANDROID
            client = new GrpcRemoteAppAgent(multiApp, address, new MyAndroidHandler());
#else
            client = new GrpcRemoteAppAgent(multiApp, address);
#endif
		}

	}

#if ANDROID
    public class MyAndroidHandler : Xamarin.Android.Net.AndroidMessageHandler
    {
        MyHostnameVerifier verifier = new MyHostnameVerifier();

        protected override Javax.Net.Ssl.IHostnameVerifier GetSSLHostnameVerifier(Javax.Net.Ssl.HttpsURLConnection connection)
        {
            return verifier;
        }
        protected override Javax.Net.Ssl.SSLSocketFactory ConfigureCustomSSLSocketFactory(Javax.Net.Ssl.HttpsURLConnection connection)
        {
			return Android.Net.SSLCertificateSocketFactory.GetInsecure(1000, null);
		}

        class MyHostnameVerifier : Java.Lang.Object, Javax.Net.Ssl.IHostnameVerifier
        {
            public bool Verify(string hostname, Javax.Net.Ssl.ISSLSession session)
            {
                return true;
            }
        }
    }
#endif
}
