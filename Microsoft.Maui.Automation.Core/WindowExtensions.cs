namespace Microsoft.Maui.Automation
{
    public static class WindowExtensions
    {
        public static string ToString(this IWindow window, int depth, int indentSpaces = 2)
        {
            var w = window;
            var s = "\r\n" + new string(' ', (depth * indentSpaces) + indentSpaces);
            return $"[window:{w.Type}{s}id='{w.Id}',{s}automationId='{w.AutomationId}',{s}text='{w.Text}',{s}size='{w.Width}w,{w.Height}h',{s}children={w.Children.Length}]";
        }

    }
}
