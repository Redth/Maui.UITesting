using Grpc.Net.Client;
using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;
using Spectre.Console;
using System.Net;

var address = "http://localhost:10882";

Console.WriteLine($"REPL> Connecting to {address}...");
var platform = Platform.Maui;

var grpc = new GrpcRemoteAppClient();

Console.WriteLine("Connected.");


while (true)
{
	var input = Console.ReadLine() ?? string.Empty;

	try
	{
		if (input.StartsWith("tree"))
		{
			var children = await grpc.GetElements(platform);

			foreach (var w in children)
			{
				var tree = new Tree(w.ToTable(ConfigureTable));


				foreach (var d in w.Children)
				{
					PrintTree(tree, d, 1);
				}

				AnsiConsole.Write(tree);
			}

		}
		else if (input.StartsWith("windows"))
		{
			var children = await grpc.GetElements(platform);

			foreach (var w in children)
			{
				var tree = new Tree(w.ToTable(ConfigureTable));

				AnsiConsole.Write(tree);
			}
		}
		else if (input.StartsWith("test"))
		{
			var elements = await grpc.FindElements(platform, "AutomationId", "buttonOne");
			
			foreach (var w in elements)
			{
				var tree = new Tree(w.ToTable(ConfigureTable));

				AnsiConsole.Write(tree);
			}
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex);
	}

	if (input != null && (input.Equals("quit", StringComparison.OrdinalIgnoreCase)
		|| input.Equals("q", StringComparison.OrdinalIgnoreCase)
		|| input.Equals("exit", StringComparison.OrdinalIgnoreCase)))
		break;
}

await grpc.Shutdown();


void PrintTree(IHasTreeNodes node, Element element, int depth)
{
	var subnode = node.AddNode(element.ToTable(ConfigureTable));

	foreach (var c in element.Children)
		PrintTree(subnode, c, depth);
}

static void ConfigureTable(Table table)
{
	table.Border(TableBorder.Rounded);
}