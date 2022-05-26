using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;
using Spectre.Console;
using System.Net;

var port = TcpRemoteApplication.DefaultPort;

Console.WriteLine($"REPL> Waiting for remote automation connection on port {port}...");
var platform = Platform.MAUI;
var remote = new TcpRemoteApplication(platform, IPAddress.Any, port);

Console.WriteLine("Connected.");


while(true)
{
    var input = Console.ReadLine() ?? string.Empty;

try {
    if (input.StartsWith("tree"))
    {
        var children = await remote.Children(platform);

        foreach (var w in children)
        {
            var tree = new Tree(w.ToTable(ConfigureTable));

            var descendants = await remote.Descendants(platform, w.Id);
            foreach (var d in descendants)
            {
                //var node = tree.AddNode(d.ToMarkupString(0, 0));

                PrintTree(tree, d, 1);
            }

            AnsiConsole.Write(tree);
        }

    }
    else if (input.StartsWith("windows"))
    {
        foreach (var w in await remote.Children(platform))
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


void PrintTree(IHasTreeNodes node, IElement element, int depth)
{
    var subnode = node.AddNode(element.ToTable(ConfigureTable));

    foreach (var c in element.Children)
        PrintTree(subnode, c, depth);
}

static void ConfigureTable(Table table)
{
    table.Border(TableBorder.Rounded);
}