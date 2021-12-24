using Microsoft.Maui.Automation;
using System.Collections.Generic;
using System.Linq;

namespace RemoteAutomationTests
{
    public class MockView : View
    {
        public MockView(Platform platform, string windowId, string id) 
            : base(platform, windowId, id)
        {
            WindowId = windowId;
            Text = string.Empty;

            PlatformElement = new MockNativeView();
        }

        public readonly List<MockView> MockViews = new List<MockView>();

        public override IView[] Children
            => MockViews.ToArray();
    }
}