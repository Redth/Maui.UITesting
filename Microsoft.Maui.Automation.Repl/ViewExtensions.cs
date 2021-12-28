using Spectre.Console;

namespace Microsoft.Maui.Automation
{
    public static class ViewExtensions
    {
        public static Table ToTable(this IView view, Action<Table>? config = null)
        {
            var table = new Table()
                .AddColumn("[bold]" + view.Type + "[/]")
                .AddColumn("");

            if (config != null)
                config(table);

            table.AddRow("id", view.Id);
            table.AddRow("windowId", view.WindowId);
            
            if (!string.IsNullOrEmpty(view.AutomationId) && view.AutomationId != view.Id)
                table.AddRow("automationId", view.AutomationId);
            
            if (!string.IsNullOrEmpty(view.Text))
                table.AddRow("text", view.Text);

            table.AddRow("visible", view.Visible.ToString());
            table.AddRow("enabled", view.Enabled.ToString());
            table.AddRow("focused", view.Focused.ToString());

            if (view.X >= 0 || view.Y >= 0 || view.Width >= 0 || view.Height >= 0)
                table.AddRow("frame", $"{view.X}x,{view.Y}y,{view.Width}w,{view.Height}h");

            if (view.Children?.Any() ?? false)
                table.AddRow("children", (view.Children?.Count() ?? 0).ToString());

            return table;
        }
    }
}
