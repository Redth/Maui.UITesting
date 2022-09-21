using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Driver;
using Microsoft.Maui.Automation.Remote;
using Spectre.Console;
using System.Net;


var driver = new AppDriverBuilder()
	//.AppId("com.companyname.samplemauiapp")
	.AppFilename("/Users/redth/code/Maui.UITesting/samples/SampleMauiApp/bin/Debug/net6.0-maccatalyst/maccatalyst-x64/SampleMauiApp.app")
	.DevicePlatform(Platform.Ios)
	.AutomationPlatform(Platform.Maui)
	.Device("mac")
	.Build();

Task<string?>? readTask = null;
CancellationTokenSource ctsMain = new CancellationTokenSource();

Console.CancelKeyPress += (s, e) =>
{
	ctsMain.Cancel();
};


//var config = new AutomationConfiguration
//{
//	AppAgentPort = 5000,
//	DevicePlatform = Platform.Winappsdk,
//	AutomationPlatform = platform,
//	Device = "windows",
//	AppId = "D05ADD49-B96D-49E5-979C-FA3A3F42F8E0_9zz4h110yvjzm!App"
//};

//var config = new AutomationConfiguration
//{
//	AppAgentPort = 5000,
//	DevicePlatform = Platform.Maccatalyst,
//	AutomationPlatform = Platform.Maui,
//	Device = "mac",
//	AppId = "com.companyname.samplemauiapp",
//	AppFilename = "/Users/redth/code/Maui.UITesting/samples/SampleMauiApp/bin/Debug/net6.0-maccatalyst/maccatalyst-x64/SampleMauiApp.app"
//};


await driver.InstallApp();
await driver.LaunchApp();

var mappings = new Dictionary<string, Func<Task>>
{
	{ "tree", Tree },
	{ "windows", Windows },
	{ "test", async () =>
		{
			await driver
				.Elements()
				.FirstById("buttonOne")
				.Tap();


			var label = await driver
				.Elements()
				.FirstBy(e => e.Type == "Label" && e.Text.Contains("1"))
				.Element();

			Console.WriteLine(label?.Text);
		}
	}
};


while (!ctsMain.Token.IsCancellationRequested)
{
	var input = await ReadLineAsync(ctsMain.Token);

	try
	{
		foreach (var kvp in mappings)
		{
			if (input?.StartsWith(kvp.Key) ?? false)
			{
				_ = Task.Run(async () =>
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
	{
		ctsMain.Cancel();
	}
}

driver.Dispose();

Environment.Exit(0);

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


async Task<string?> ReadLineAsync(CancellationToken cancellationToken = default)
{
	try
	{
		readTask = Task.Run(() => Console.ReadLine());

		await Task.WhenAny(readTask, Task.Delay(-1, cancellationToken));

		if (readTask.IsCompletedSuccessfully)
			return readTask.Result;
	}
	catch (Exception ex)
	{
		throw ex;
	}

	return null;

}