using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Microsoft.Maui.Automation
{
    public abstract class Element : IElement
    {
        protected Element()
		{
            Id = Guid.NewGuid().ToString();
            Platform = Platform.MAUI;
            Application = new NullApplication(Platform);
            ParentId = string.Empty;

			AutomationId = Id;
			Type = GetType().Name;
			FullType = GetType().FullName ?? Type;

			Visible = false;
			Enabled = false;
			Focused = false;
			X = -1;
			Y = -1;
			Platform = Platform.MAUI;
			Children = new ReadOnlyCollection<IElement>(new List<IElement>());
			Width = -1;
			Height = -1;
		}

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

		[Newtonsoft.Json.JsonIgnore]
		[JsonIgnore]
        public virtual IApplication Application { get; protected set; }

		[JsonInclude]
		public virtual string? ParentId { get; protected set; }

		[JsonInclude]
		public virtual bool Visible { get; protected set; }

		[JsonInclude]
		public virtual bool Enabled { get; protected set; }

		[JsonInclude]
		public virtual bool Focused { get; protected set; }

		[JsonInclude]
		public virtual int X { get; protected set; }

		[JsonInclude]
		public virtual int Y { get; protected set; }

		[JsonInclude]
		public virtual Platform Platform { get; protected set; }

		[Newtonsoft.Json.JsonIgnore]
		[JsonIgnore]
        public virtual object? PlatformElement { get; protected set; }

		[JsonInclude]
		[Newtonsoft.Json.JsonProperty]
		public virtual IReadOnlyCollection<IElement> Children { get; set; }

		[JsonInclude]
		public virtual string Id { get; protected set; }

		[JsonInclude]
		public virtual string AutomationId { get; protected set; }

		[JsonInclude]
		public virtual string Type { get; protected set; }

		[JsonInclude]
		public virtual string FullType { get; protected set; }

		[JsonInclude]
		public virtual string? Text { get; protected set; }

		[JsonInclude]
        public virtual int Width { get; protected set; }

		[JsonInclude]
		public virtual int Height { get; protected set; }
    }
}
