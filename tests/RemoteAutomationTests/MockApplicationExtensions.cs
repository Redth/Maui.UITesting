namespace RemoteAutomationTests
{
    public static class MockApplicationExtensions
    {
        public static MockApplication WithWindow(this MockApplication app, MockWindow window)
        {
            app.MockWindows.Add(window);
            return app;
        }

        public static MockApplication WithWindow(this MockApplication app, string id, string? automationId, string? title)
        {
            app.MockWindows.Add(new MockWindow(app.CurrentPlatform, id, automationId, title));
            return app;
        }

        public static MockApplication WithView(this MockApplication app, MockView view)
        {
            app.CurrentMockWindow!.MockViews.Add(view);
            return app;
        }

        public static MockApplication WithView(this MockApplication app, string id)
        {
            var window = app.CurrentMockWindow!;
            window.MockViews.Add(new MockView(app.CurrentPlatform, window.Id, id));
            return app;
        }
    }
}