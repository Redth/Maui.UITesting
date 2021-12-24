using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public abstract class Window : IWindow
    {
        public Window(Platform platform, string id)
        {
            Id = id ?? Guid.NewGuid().ToString();
            AutomationId = Id;

            Type = GetType().Name;

            Platform = platform;
            Children = Array.Empty<IView>();
            Width = -1;
            Height = -1;
        }

        public virtual Platform Platform { get; }

        [JsonIgnore]
        public virtual object? PlatformElement { get; protected set; }

        public virtual string Id { get; protected set; }
        public virtual string AutomationId { get; protected set; }

        public virtual string Type { get; protected set; }

        public virtual int Width { get; protected set; }
        public virtual int Height { get; protected set; }
        public virtual string? Text { get; protected set; }
        
        public virtual IView[] Children { get; protected set; }
    }
}
