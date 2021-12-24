using Newtonsoft.Json;

namespace Microsoft.Maui.Automation.Remote
{
    public class RemoteWindow : IWindow
    {
        public static RemoteWindow? From(IWindow? window)
           => window == null ? null : new RemoteWindow
           {
               Id = window.Id,
               Platform = window.Platform,
               Children = window.Children?.Select(c => RemoteView.From(c)!)?.ToArray() ?? Array.Empty<IView>(),
               AutomationId = window.AutomationId,
               Type = window.Type,
               Text = window.Text,
               Width = window.Width,
               Height = window.Height
           };

        public Platform Platform { get; set; }
        
        [JsonIgnore]
        public object? PlatformElement { get; set; }
        public IView[] Children { get; set; } = Array.Empty<IView>();
        public string Id { get; set; } = string.Empty;
        public string AutomationId { get; set; } = string.Empty;
        public string Type { get; set; } = typeof(RemoteWindow).Name;
        public string? Text { get; set; }
        public int Width { get; set; } = -1;
        public int Height { get; set; } = -1;
    }
}