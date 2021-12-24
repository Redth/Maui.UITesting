using Spectre.Console;

namespace Microsoft.Maui.Automation
{
    public static class WindowExtensions
    {
        public static Table ToTable(this IWindow window, Action<Table>? config)
        {
            var table = new Table()
                .AddColumn(window.Type)
                .AddColumn("");

            if (config != null)
                config(table);

            table.AddRow("id", window.Id);

            if (!string.IsNullOrEmpty(window.AutomationId) && window.AutomationId != window.Id)
                table.AddRow("automationId", window.AutomationId);
            if (!string.IsNullOrEmpty(window.Text))
                table.AddRow("text", window.Text ?? "");
            if (window.Width >= 0 || window.Height >= 0)
                table.AddRow("size", $"{window.Width}w,{window.Height}h");
            if (window.Children?.Any() ?? false)
                table.AddRow("children", (window.Children?.Count() ?? 0).ToString());

            return table;
        }
    }
}
