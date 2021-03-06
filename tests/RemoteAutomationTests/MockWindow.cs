using Microsoft.Maui.Automation;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RemoteAutomationTests
{
    public class MockWindow : Element
    {
        public MockWindow(IApplication application, Platform platform, string id, string? automationId, string? title = null)
            : base(application, platform, id)
        {
            AutomationId = automationId ?? Id;

            Text = title ?? string.Empty;

            PlatformElement = new MockNativeWindow();
        }

        public readonly List<MockView> MockViews = new List<MockView>();

        Platform platform;

        public override Platform Platform => platform;

        public override IReadOnlyCollection<IElement> Children
            => new ReadOnlyCollection<IElement>(MockViews.ToList<IElement>());
    }

    public static class MockWindowExtensions
    {
        public static MockWindow WithView(this MockWindow window, MockView view)
        {
            window.MockViews.Add(view);
            return window;
        }
        public static MockWindow WithView(this MockWindow window, string windowId, string id)
        {
            window.MockViews.Add(new MockView(window.Application, window.Platform, windowId, id));
            return window;
        }
    }
}