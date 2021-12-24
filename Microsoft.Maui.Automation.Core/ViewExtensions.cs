namespace Microsoft.Maui.Automation
{
    public static class ViewExtensions
    {
        public static string ToString(this IView view, int depth, int indentSpaces = 2)
        {
            var v = view;
            var s = "\r\n" + new string(' ', (depth * indentSpaces) + indentSpaces);
            return $"[view:{v.Type} id='{v.Id}',{s}windowId='{v.WindowId}',{s}automationId='{v.AutomationId}',{s}visible='{v.Visible}',{s}enabled='{v.Enabled}',{s}focused='{v.Focused}',{s}frame='{v.X}x,{v.Y}y,{v.Width}w,{v.Height}h',{s}children={v.Children.Length}]";
        }

    }
}
