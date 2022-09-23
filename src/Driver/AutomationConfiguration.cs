using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Maui.Automation.Driver
{
	public static class ConfigurationKeys
	{
		public const string GrpcHostLoggingEnabled = "GRPC_HOST_LOGGING_ENABLED";
	}

	public class AutomationConfiguration : Dictionary<string, object>, IAutomationConfiguration
	{

		public static AutomationConfiguration FromYaml(string yamlFilename)
		{
			var serializer = new YamlDotNet.Serialization.Deserializer();

			var yaml = File.ReadAllText(yamlFilename);

			return serializer.Deserialize<AutomationConfiguration>(yaml)
				?? throw new InvalidDataException($"Failed to load YAML configuration from: {yamlFilename}");
		}

		public static AutomationConfiguration FromJson(string jsonFilename)
		{
			using var json = File.OpenRead(jsonFilename);

			return JsonSerializer.Deserialize<AutomationConfiguration>(
				json,
				new JsonSerializerOptions
				{
					AllowTrailingCommas = true,
					PropertyNameCaseInsensitive = true,
					DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
					IgnoreReadOnlyFields = true,
					IgnoreReadOnlyProperties = true,
					NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals,
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					ReadCommentHandling = JsonCommentHandling.Skip,
				})
					?? throw new InvalidDataException($"Failed to load JSON configuration from: {jsonFilename}");
		}

		public AutomationConfiguration()
			: base()
		{
		}

		public AutomationConfiguration(string appId, string appFilename, Platform devicePlatform, string? device = null, Platform? automationPlatform = null)
		{
			AppFilename = appFilename;
			AppId = appId;
			DevicePlatform = devicePlatform;
			if (!string.IsNullOrEmpty(device))
				Device = device;
			AutomationPlatform = automationPlatform ?? devicePlatform;
		}

		public string? AppAgentAddress
		{
			get => GetOrDefault(nameof(AppAgentAddress), IPAddress.Loopback.ToString())?.ToString();
			set => Set(nameof(AppAgentAddress), value);
		}

		public int AppAgentPort
		{
			get => GetOrDefaultInt(nameof(AppAgentPort), 5000);
			set => this[nameof(AppAgentPort)] = value;
		}

		public string? Device
		{
			get => GetOrDefault(nameof(Device), IPAddress.Any.ToString())?.ToString();
			set => Set(nameof(Device), value);
		}

		public Platform DevicePlatform
		{
			get => GetOrDefaultEnum<Platform>(nameof(DevicePlatform),() => AppUtil.InferDevicePlatformFromFilename(AppFilename));
			set => Set(nameof(DevicePlatform), value);
		}

		public Platform AutomationPlatform
		{
			get => GetOrDefaultEnum<Platform>(nameof(AutomationPlatform), Platform.Maui);
			set => this[nameof(AutomationPlatform)] = Enum.GetName<Platform>(value) ?? nameof(Platform.Maui);
		}

		public string? AppId
		{
			get => GetOrDefault(nameof(AppId), null)?.ToString();
			set => Set(nameof(AppId), value);
		}

		public string? AppFilename
		{
			get => GetOrDefault(nameof(AppFilename), null)?.ToString();
			set => Set(nameof(AppFilename), value);
		}

		public string? Get(string key, string? defaultValue)
			=> GetOrDefault(key, defaultValue)?.ToString();

		int GetOrDefaultInt(string key, int defaultValue)
		{
			var str = GetOrDefault(key, (string?)null)?.ToString();

			if (string.IsNullOrEmpty(str))
				return defaultValue;

			if (int.TryParse(str, out var v))
				return v;

			return defaultValue;
		}

		T GetOrDefaultEnum<T>(string key, T defaultValue) where T : struct
			=> GetOrDefaultEnum<T>(key, () => defaultValue);

		T GetOrDefaultEnum<T>(string key, Func<T> defaultValue) where T : struct
		{
			var str = GetOrDefault(key, (string?)null)?.ToString();

			if (string.IsNullOrEmpty(str))
				return defaultValue();

			if (Enum.TryParse<T>(str, out var v))
				return v;

			return defaultValue();
		}

		private object? GetOrDefault(string key, object? defaultValue)
		{
			if (this.ContainsKey(key))
				return this[key];
			return defaultValue;
		}

		void Set(string key, string? value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				this[key] = value;
			}
			else
			{
				if (this.ContainsKey(key))
					this.Remove(key);
			}
		}

		void Set(string key, object? value)
		{
			if (value is not null)
			{
				this[key] = value;
			}
			else
			{
				if (this.ContainsKey(key))
					this.Remove(key);
			}
		}

		public bool Get(string key, bool defaultValue = false)
		{
			if (bool.TryParse(Get(key, defaultValue.ToString()), out var val))
				return val;
			return defaultValue;
		}

		void IAutomationConfiguration.Set(string key, string? value)
		{
			if (string.IsNullOrEmpty(value))
			{
				if (ContainsKey(key))
					Remove(key);
			}
			else
				this[key] = value;
		}

		public void Set(string key, bool defaultValue)
		{
			this[key] = defaultValue;
		}
	}
}

