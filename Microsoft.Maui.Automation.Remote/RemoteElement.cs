using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Microsoft.Maui.Automation.Remote
{
    public class RemoteElement : Element
    {
        [JsonConstructor]
        public RemoteElement(Platform platform = Platform.MAUI)
            : base(new NullApplication(platform), platform, "")
        { }

        public RemoteElement(IApplication application, IElement from, string? parentId = null)
            : base(application, from.Platform, from.Id, parentId)
        {
            Id = from.Id;
            AutomationId = from.AutomationId;

            Type = from.Type;
            FullType = from.FullType;

            Visible = from.Visible;
            Enabled = from.Enabled;
            Focused = from.Focused;

            Text = from.Text;

            X = from.X;
            Y = from.Y;
            Width = from.Width;
            Height = from.Height;

            var children = from.Children?.Select(c => new RemoteElement(application, c, Id))
                ?? Enumerable.Empty<RemoteElement>();

            Children = new ReadOnlyCollection<IElement>(children.ToList<IElement>());

        }

        internal void SetChildren(IEnumerable<RemoteElement> children)
        {
            Children = new ReadOnlyCollection<IElement>(children.ToList<IElement>());
        }
    }
}