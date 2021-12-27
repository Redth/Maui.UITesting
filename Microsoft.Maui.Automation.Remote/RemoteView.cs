using Newtonsoft.Json;

namespace Microsoft.Maui.Automation.Remote
{
    public class RemoteView : IView
    {
        public static RemoteView? From(IView? view)
            => view == null ? null : new RemoteView
            {
                WindowId = view.WindowId,
                Visible = view.Visible,
                Enabled = view.Enabled,
                Focused = view.Focused,
                X = view.X,
                Y = view.Y,
                Platform = view.Platform,
                Children = view.Children?.Select(c => RemoteView.From(c)!)?.ToArray() ?? Array.Empty<IView>(),
                Id = view.Id,
                AutomationId = view.AutomationId,
                Type = view.Type,
                Text = view.Text,
                Width = view.Width,
                Height = view.Height,
            };

        public string WindowId { get; set; } = string.Empty;
        public bool Visible { get; set; } = false;
        public bool Enabled { get; set; } = false;
        public bool Focused { get; set; } = false;
        public int X { get; set; } = -1;
        public int Y { get; set; } = -1;
        public Platform Platform { get; set; }

        [JsonIgnore]
        public object? PlatformElement { get; set; }

        [JsonIgnore]
        public IView[] Children { get; set; } = Array.Empty<IView>();

        public RemoteView[] RemoteChildren
        {
            get => Children?.Select(c => RemoteView.From(c))?.ToArray() ?? Array.Empty<RemoteView>();
            set => Children = value.ToArray();
        }

        public string Id { get; set; } = string.Empty;
        public string AutomationId { get; set; } = string.Empty;
        public string Type { get; set; } = typeof(RemoteView).Name;
        public string? Text { get; set; }
        public int Width { get; set; } = -1;
        public int Height { get; set; } = -1;
    }
}