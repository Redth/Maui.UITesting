namespace Microsoft.Maui.Automation.Driver
{
    public interface IAutomationConfiguration
    {
        string AppAgentAddress { get; set; }
        int AppAgentPort { get; set; }

        string Device { get; set; }

        Platform DevicePlatform { get; set; }

        Platform AutomationPlatform { get; set; }

        string AppId { get; set; }
        string AppFilename { get; set; }
    }
}