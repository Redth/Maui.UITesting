using Grpc.Net.Client;
using Microsoft.Maui.Automation;
using Spectre.Console;
using System.Net;

var address = "http://localhost:10882";

Console.WriteLine($"REPL> Connecting to {address}...");
var platform = Platform.Maui;



var grpc = GrpcChannel.ForAddress(address);

var client = new Microsoft.Maui.Automation.RemoteGrpc.RemoteApp.RemoteAppClient(grpc);


Console.WriteLine("Connected.");


while (true)
{
	var input = Console.ReadLine() ?? string.Empty;

	try
	{
		if (input.StartsWith("tree"))
		{
			var children = await client.GetElementsAsync(new Microsoft.Maui.Automation.RemoteGrpc.ElementsRequest());

			foreach (var w in children.Elements)
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
			var children = await client.GetElementsAsync(new Microsoft.Maui.Automation.RemoteGrpc.ElementsRequest());

			foreach (var w in children.Elements)
			{
				var tree = new Tree(w.ToTable(ConfigureTable));

				AnsiConsole.Write(tree);
			}
		}
		else if (input.StartsWith("test"))
		{
			var children = await client.GetElementsAsync(new Microsoft.Maui.Automation.RemoteGrpc.ElementsRequest());

			foreach (var w in children.Elements)
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