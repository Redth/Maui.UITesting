using System;
using System.Net;

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

        public AutomationConfiguration()
            : base()
        {
        }

        public string AppAgentAddress
        {
            get => this?[nameof(AppAgentAddress)]?.ToString() ?? IPAddress.Any.ToString();
            set => this[nameof(AppAgentAddress)] = value;
        }

        public int AppAgentPort
        {
            get => int.TryParse(this?[nameof(AppAgentPort)]?.ToString(), out var v) ? v : 10882;
            set => this[nameof(AppAgentPort)] = value;
        }

        public string Device
        {
            get => this?[nameof(Device)]?.ToString() ?? IPAddress.Any.ToString();
            set => this[nameof(Device)] = value;
        }

        public Platform DevicePlatform
        {
            get => Enum.TryParse<Platform>(this?[nameof(DevicePlatform)]?.ToString(), out var v) ? v : Platform.Maccatalyst;
            set => this[nameof(DevicePlatform)] = Enum.GetName<Platform>(value) ?? nameof(Platform.Maui);
        }

        public Platform AutomationPlatform
        {
            get => Enum.TryParse<Platform>(this?[nameof(AutomationPlatform)]?.ToString(), out var v) ? v : Platform.Maccatalyst;
            set => this[nameof(AutomationPlatform)] = Enum.GetName<Platform>(value) ?? nameof(Platform.Maui);
        }
    }
}

