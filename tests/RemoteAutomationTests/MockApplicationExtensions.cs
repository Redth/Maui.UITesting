using Microsoft.Maui.Automation;

namespace RemoteAutomationTests
{
    public class MockWindow
    {
        public string Title { get; set; }
    }

    public static class MockApplicationExtensions
    {
        public static MockApplication WithWindow(this MockApplication app, Element window)
        {
            app.MockWindows.Add(window);
            return app;
        }

        public static MockApplication WithWindow(this MockApplication app, string id, string? automationId, string? title)
        {
            var w = new Element(app, app.DefaultPlatform, id, new MockWindow());
            w.Text = title;

            app.MockWindows.Add(w);
            app.CurrentMockWindow = w;
            return app;
        }

        public static MockApplication WithView(this MockApplication app, Element view)
        {
            app.CurrentMockWindow!.Children.Add(view);
            return app;
        }

        public static MockApplication WithView(this MockApplication app, string id)
        {
            var window = app.CurrentMockWindow!;
            window.Children.Add(new Element(app, app.DefaultPlatform, id, new MockWindow(), window.Id));
            return app;
        }
    }
}