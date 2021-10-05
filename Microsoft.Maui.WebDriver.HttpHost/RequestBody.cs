using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Microsoft.Maui.WebDriver.HttpHost
{
    internal class RequestBody
    {
		public static RequestBody Parse(HttpListenerContext context)
        {
			var raw = string.Empty;

			using (var streamReader = new StreamReader(context.Request.InputStream))
				raw = streamReader.ReadToEnd();

			return new RequestBody(raw);
        }

		public RequestBody(string rawContent)
			=> Raw = rawContent;

		public readonly string Raw;

		JObject root = default;

		public JObject Root
			=> root ??= JObject.Parse(Raw);

		JObject capabilities = default;

		public JObject Capabilities
			=> capabilities ??= Root["capabilities"] as JObject;
	}


}
