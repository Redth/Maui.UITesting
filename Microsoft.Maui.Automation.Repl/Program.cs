using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;
using Spectre.Console;
using System.Net;

var port = TcpRemoteApplication.DefaultPort;

Console.WriteLine($"REPL> Waiting for remote automation connection on port {port}...");

var remote = new TcpRemoteApplication(IPAddress.Any, port);

Console.WriteLine("Connected.");


while(true)
{
    var input = Console.ReadLine() ?? string.Empty;

try {
    if (input.StartsWith("tree"))
    {
        var windows = await remote.Windows();

        

        foreach (var w in windows)
        {
            var tree = new Tree(w.ToTable(ConfigureTable));

            await foreach (var d in remote.Descendants(w.Id))
            {
                //var node = tree.AddNode(d.ToMarkupString(0, 0));

                PrintTree(tree, d, 1);
            }

            AnsiConsole.Write(tree);
        }

    }
    else if (input.StartsWith("windows"))
    {
        var windows = await remote.Windows();

        foreach (var w in windows)
        {
            var tree = new Tree(w.ToTable(ConfigureTable));

            AnsiConsole.Write(tree);
        }
    }
} catch (Exception ex)
{
    Console.WriteLine(ex);
}

    if (input != null && (input.Equals("quit", StringComparison.OrdinalIgnoreCase)
        || input.Equals("q", StringComparison.OrdinalIgnoreCase)
        || input.Equals("exit", StringComparison.OrdinalIgnoreCase)))
        break;
}


void PrintTree(IHasTreeNodes node, IView view, int depth)
{
    var subnode = node.AddNode(view.ToTable(ConfigureTable));

    foreach (var c in view.Children)
        PrintTree(subnode, c, depth);
}

static void ConfigureTable(Table table)
{
    table.Border(TableBorder.Rounded);
}