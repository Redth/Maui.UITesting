using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Maui.Automation.Driver
{
    public class AutomationConfiguration : Dictionary<string, object>, IAutomationConfiguration
    {
        public static AutomationConfiguration FromYaml(string yamlFilename)
        {
            var serializer = new SharpYaml.Serialization.Serializer();

            using var yaml = File.OpenRead(yamlFilename);

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
            AppId = appId;
            AppFilename = appFilename;
            DevicePlatform = devicePlatform;
            if (!string.IsNullOrEmpty(device))
                Device = device;
            AutomationPlatform = automationPlatform ?? devicePlatform;
        }

        public string AppAgentAddress
        {
            get => GetOrDefault(nameof(AppAgentAddress), IPAddress.Loopback.ToString()).ToString();
            set => this[nameof(AppAgentAddress)] = value;
        }

        public int AppAgentPort
        {
            get => int.Parse(GetOrDefault(nameof(AppAgentPort), 5000).ToString());
            set => this[nameof(AppAgentPort)] = value;
        }

        public string Device
        {
            get => GetOrDefault(nameof(Device), IPAddress.Any.ToString()).ToString();
            set => this[nameof(Device)] = value;
        }

        public Platform DevicePlatform
        {
            get => Enum.Parse<Platform>(GetOrDefault(nameof(DevicePlatform), "maccatalyst").ToString());
            set => this[nameof(DevicePlatform)] = Enum.GetName<Platform>(value) ?? nameof(Platform.Maui);
        }

        public Platform AutomationPlatform
        {
            get => Enum.Parse<Platform>(GetOrDefault(nameof(AutomationPlatform), "maccatalyst").ToString());
            set => this[nameof(AutomationPlatform)] = Enum.GetName<Platform>(value) ?? nameof(Platform.Maui);
        }

        public string AppId
        {
            get => GetOrDefault(nameof(AppId), "").ToString();
            set => this[nameof(AppId)] = value;
        }

        public string AppFilename
        {
            get => GetOrDefault(nameof(AppFilename), "").ToString();
            set => this[nameof(AppFilename)] = value;
        }

        public string Get(string key, string defaultValue)
            => GetOrDefault(key, defaultValue)?.ToString();

		private object GetOrDefault(string key, object defaultValue)
        {
            if (this.ContainsKey(key))
                return this[key];
            return defaultValue;
        }
    }
}

