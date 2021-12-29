namespace Microsoft.Maui.Automation
{
    public static class ViewExtensions
    {
        public static bool IsTopLevel(this IElement element)
            => element?.ParentId == element?.Id;


        public static string ToString(this IElement element, int depth, int indentSpaces = 2)
        {
            var v = element;
            var t = element.IsTopLevel() ? "window" : "view";
            var s = "\r\n" + new string(' ', (depth * indentSpaces) + indentSpaces);
            return $"[{t}:{v.Type} id='{v.Id}',{s}parentId='{v.ParentId}',{s}automationId='{v.AutomationId}',{s}visible='{v.Visible}',{s}enabled='{v.Enabled}',{s}focused='{v.Focused}',{s}frame='{v.X}x,{v.Y}y,{v.Width}w,{v.Height}h',{s}children={v.Children.Count}]";
        }

    }
}
