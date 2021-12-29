using Microsoft.Maui.Automation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RemoteAutomationTests
{
    public class MockView : Element
    {
        public MockView(IApplication application, Platform platform, string id, string? parentId = null) 
            : base(application, platform, id, parentId)
        {
            Text = string.Empty;

            PlatformElement = new MockNativeView();
        }

        public readonly List<MockView> MockViews = new List<MockView>();

        
        public override IReadOnlyCollection<IElement> Children
            => new ReadOnlyCollection<IElement>(MockViews.ToList<IElement>());
    }
}