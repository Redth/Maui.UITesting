using OpenQA.Selenium;
using System;
using Newtonsoft.Json.Linq;

namespace Microsoft.Maui.WebDriver.HttpHost
{
    internal class SessionTimouts : ITimeouts
    {
        public TimeSpan ImplicitWait { get; set; } = TimeSpan.Zero;
		public TimeSpan AsynchronousJavaScript { get; set; } = TimeSpan.FromSeconds(30);
		public TimeSpan PageLoad { get; set; } = TimeSpan.FromSeconds(60);

		public JObject AsJson()
			=> new (
				new JProperty("script", (int)AsynchronousJavaScript.TotalMilliseconds),
				new JProperty("pageLoad", (int)PageLoad.TotalMilliseconds),
				new JProperty("implicit", (int)ImplicitWait.TotalMilliseconds));
    }


}
