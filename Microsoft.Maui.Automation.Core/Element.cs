using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Microsoft.Maui.Automation
{
    public abstract class Element : IElement
    {
        public Element(IApplication application, Platform platform, string id, string? parentId = null)
        {
            Application = application;
            ParentId = parentId;
            Id = id;
            AutomationId = Id;
            Type = GetType().Name;
            FullType = GetType().FullName ?? Type;

            Visible = false;
            Enabled = false;
            Focused = false;
            X = -1;
            Y = -1;
            Platform = platform;
            Children = new ReadOnlyCollection<IElement>(new List<IElement>());
            Width = -1;
            Height = -1;
        }

        [JsonIgnore]
        public virtual IApplication Application { get; protected set; }

        public virtual string? ParentId { get; protected set; }

        public virtual bool Visible { get; protected set; }
        public virtual bool Enabled { get; protected set; }
        public virtual bool Focused { get; protected set; }
        public virtual int X { get; protected set; }
        public virtual int Y { get; protected set; }
        public virtual Platform Platform { get; protected set; }

        [JsonIgnore]
        public virtual object? PlatformElement { get; protected set; }
        public virtual IReadOnlyCollection<IElement> Children { get; protected set; }
        public virtual string Id { get; protected set; }
        public virtual string AutomationId { get; protected set; }
        public virtual string Type { get; protected set; }
        public virtual string FullType { get; protected set; }
        public virtual string? Text { get; protected set; }
        public virtual int Width { get; protected set; }
        public virtual int Height { get; protected set; }
    }
}
