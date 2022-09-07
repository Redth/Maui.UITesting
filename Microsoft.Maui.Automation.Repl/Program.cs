using Grpc.Core.Logging;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Driver;
using Microsoft.Maui.Automation.Remote;
using Spectre.Console;
using System.Net;



var platform = Platform.Maui;


var config = new AutomationConfiguration
{
	AppAgentPort = 5000,
	DevicePlatform = Platform.Android,
	AutomationPlatform = platform,
	Device = "emulator-5554"
};
var driver = new AppDriver(config);

var mappings = new Dictionary<string, Func<Task>>
{
	{ "tree", Tree },
	{ "windows", Windows },
	{ "test2", async () =>
		{
			var button = await driver.FirstByAutomationId("buttonOne");

			await driver.Tap(button!);

			var label = await driver.FirstByAutomationId("labelCount");

			Console.WriteLine(label.Text);
		}
	},
	{ "test", async () =>
		{
			var elements = await driver.FindElements("AutomationId", "buttonOne");

			foreach (var w in elements)
			{
				var tree = new Tree(w.ToTable(ConfigureTable));

				AnsiConsole.Write(tree);
			}
		}
	}
};

while (true)
{
	var input = Console.ReadLine() ?? string.Empty;

	try
	{
		foreach (var kvp in mappings)
		{
			if (input.StartsWith(kvp.Key))
			{
				Task.Run(async () =>
				{
					try
					{
						await kvp.Value();
					}
					catch (Exception ex)
					{
						AnsiConsole.WriteException(ex);
					}
				});
				
				break;
			}
		}
	}
	catch (Exception ex)
	{
		AnsiConsole.WriteException(ex);
	}

	if (input != null && (input.Equals("quit", StringComparison.OrdinalIgnoreCase)
		|| input.Equals("q", StringComparison.OrdinalIgnoreCase)
		|| input.Equals("exit", StringComparison.OrdinalIgnoreCase)))
		break;
}

driver.Dispose();

async Task Tree()
{
	var children = await driver.GetElements();

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

async Task Windows()
{
	var children = await driver.GetElements();

	foreach (var w in children)
	{
		var tree = new Tree(w.ToTable(ConfigureTable));

		AnsiConsole.Write(tree);
	}
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