using Microsoft.Maui.Automation;
using System.IO;
using System.Text.Json;

namespace Streamer
{
    public class Channel
    {
        public static ClientChannel CreateClient(Stream stream)
            => new ClientChannel(stream);

        public static ServerChannel CreateServer()
            => new ServerChannel();

        public static JsonSerializerOptions GetJsonSerializerOptions()
		    => new JsonSerializerOptions
			{
                WriteIndented = true,
				Converters = { new PolyJsonConverter(), new ElementConverter() }
			};

		static Newtonsoft.Json.JsonSerializerSettings GetJsonSerializerSettings()
			=> new Newtonsoft.Json.JsonSerializerSettings
			{
				TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All,
				NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
				ContractResolver = new JsonProtectedResolver()
			};

        public static Response ReadResponse(Stream stream)
		{
			try
			{
				// Read length
				var msgLenBuffer = new byte[4];
				if (stream.Read(msgLenBuffer, 0, 4) != 4)
					throw new Exception();
				var messageLength = BitConverter.ToInt32(msgLenBuffer);

				// Read contents
				var messageBuffer = new byte[messageLength];
				if (stream.Read(messageBuffer, 0, messageLength) != messageLength)
					throw new Exception();

				var json = System.Text.Encoding.UTF8.GetString(messageBuffer);

				var resp = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(json, GetJsonSerializerSettings());
				return resp;
				//return JsonSerializer.Deserialize<Response>(json, GetJsonSerializerOptions());
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				throw ex;
			}
		}

		public static Request ReadRequest(Stream stream)
		{
			try
			{

				// Read length
				var msgLenBuffer = new byte[4];
				if (stream.Read(msgLenBuffer, 0, 4) != 4)
					throw new Exception();
				var messageLength = BitConverter.ToInt32(msgLenBuffer);

				// Read contents
				var messageBuffer = new byte[messageLength];
				if (stream.Read(messageBuffer, 0, messageLength) != messageLength)
					throw new Exception();

				var json = System.Text.Encoding.UTF8.GetString(messageBuffer);

				return Newtonsoft.Json.JsonConvert.DeserializeObject<Request>(json, GetJsonSerializerSettings());
				//return JsonSerializer.Deserialize<Request>(json, GetJsonSerializerOptions());
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				throw ex;
			}
		}

		public static void WriteRequest(Stream stream, Request message)
		{
			try
			{
				var json = Newtonsoft.Json.JsonConvert.SerializeObject(message, GetJsonSerializerSettings());

				//var json = JsonSerializer.Serialize<Request>(message, GetJsonSerializerOptions());
				var data = System.Text.Encoding.UTF8.GetBytes(json);
				var msgLen = BitConverter.GetBytes(data.Length);
				stream.Write(msgLen, 0, msgLen.Length);
				stream.Write(data, 0, data.Length);
				stream.Flush();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				throw ex;
			}
		}

		public static void WriteResponse(Stream stream, Response message)
		{
			try
			{
				//var json = JsonSerializer.Serialize<Response>(message, GetJsonSerializerOptions());
				var json = Newtonsoft.Json.JsonConvert.SerializeObject(message, GetJsonSerializerSettings());
				var data = System.Text.Encoding.UTF8.GetBytes(json);
				var msgLen = BitConverter.GetBytes(data.Length);
				stream.Write(msgLen, 0, msgLen.Length);
				stream.Write(data, 0, data.Length);
				stream.Flush();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				throw ex;
			}
		}
	}
}