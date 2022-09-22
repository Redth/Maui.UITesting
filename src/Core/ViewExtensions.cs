namespace Microsoft.Maui.Automation
{
	public static class ViewExtensions
	{
		public static bool IsTopLevel(this Element element)
			=> element?.ParentId == element?.Id;


		public static string ToString(this Element element, int depth, int indentSpaces = 2)
		{
			var v = element;
			var t = element.IsTopLevel() ? "window" : "view";
			var s = "\r\n" + new string(' ', (depth * indentSpaces) + indentSpaces);
			return $"[{t}:{v.Type} id='{v.Id}',{s}parentId='{v.ParentId}',{s}automationId='{v.AutomationId}',{s}visible='{v.Visible}',{s}enabled='{v.Enabled}',{s}focused='{v.Focused}',{s}frame='{v.ScreenFrame.X}x,{v.ScreenFrame.Y}y,{v.ScreenFrame.Width}w,{v.ScreenFrame.Height}h',{s}children={v.Children.Count}]";
		}

	}
}
