using Spectre.Console;

namespace Microsoft.Maui.Automation
{
    public static class ViewExtensions
    {
        public static Table ToTable(this Element element, Action<Table>? config = null)
        {
            var table = new Table()
                .AddColumn("[bold]" + element.Type + "[/]")
                .AddColumn("");

            if (config != null)
                config(table);

            table.AddRow("id", element.Id);

            if (!string.IsNullOrEmpty(element.ParentId))
               table.AddRow("parentId", element.ParentId);
            
            if (!string.IsNullOrEmpty(element.AutomationId) && element.AutomationId != element.Id)
                table.AddRow("automationId", element.AutomationId);
            
            if (!string.IsNullOrEmpty(element.Text))
                table.AddRow("text", element.Text);

            table.AddRow("visible", element.Visible.ToString());
            table.AddRow("enabled", element.Enabled.ToString());
            table.AddRow("focused", element.Focused.ToString());

            if (element.X >= 0 || element.Y >= 0 || element.Width >= 0 || element.Height >= 0)
                table.AddRow("frame", $"{element.X}x,{element.Y}y,{element.Width}w,{element.Height}h");

            if (element.Children?.Any() ?? false)
                table.AddRow("children", (element.Children?.Count() ?? 0).ToString());

            return table;
        }
    }
}
