using Newtonsoft.Json;

namespace Microsoft.Maui.Automation.Remote
{
    public class RemoteWindow : IWindow
    {
        public static RemoteWindow? From(IApplication application, IWindow? window)
           => window == null ? null : new RemoteWindow
           {
               Application = application,
               Id = window.Id,
               Platform = window.Platform,
               Children = window.Children?.Select(c => RemoteView.From(application, c)!)?.ToArray() ?? Array.Empty<IView>(),
               AutomationId = window.AutomationId,
               Type = window.Type,
               Text = window.Text,
               Width = window.Width,
               Height = window.Height
           };

        [JsonIgnore]
        public IApplication? Application { get; set; }

        public Platform Platform { get; set; }
        
        [JsonIgnore]
        public object? PlatformElement { get; set; }

        [JsonIgnore]
        public IView[] Children { get; set; } = Array.Empty<IView>();

        public RemoteView[] RemoteChildren
        {
            get => Children?.Select(c => RemoteView.From(Application, c))?.ToArray() ?? Array.Empty<RemoteView>();
            set => Children = value.ToArray();
        }
        public string Id { get; set; } = string.Empty;
        public string AutomationId { get; set; } = string.Empty;
        public string Type { get; set; } = typeof(RemoteWindow).Name;
        public string? Text { get; set; }
        public int Width { get; set; } = -1;
        public int Height { get; set; } = -1;
    }
}