using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Driver;
using Microsoft.Maui.Automation.Remote;
using Spectre.Console;
using System.Net;


//var driver = new AppDriverBuilder()
//	.AppFilename(@"C:\code\Maui.UITesting\samples\SampleMauiApp\bin\Debug\net7.0-android\com.companyname.samplemauiapp-Signed.apk")
//	.Device("Pixel_5_API_31")
//	//.Device("9B141FFAZ008CJ")
//	.Build();

var driver = new AppDriverBuilder()
	.AppId("D05ADD49-B96D-49E5-979C-FA3A3F42F8E0_yn9kjvr01ms9j!App")
	.DevicePlatform(Platform.Winappsdk)
	.ConfigureLogging(log =>
	{
		log.ClearProviders();
		log.AddConsole();
	})
	.ConfigureDriver(c => c.Set(ConfigurationKeys.GrpcHostLoggingEnabled, true))
	.Build();

Task<string?>? readTask = null;
CancellationTokenSource ctsMain = new CancellationTokenSource();

Console.CancelKeyPress += (s, e) =>
{
	ctsMain.Cancel();
};


await driver.Start();

var mappings = new Dictionary<string, Func<Task>>
{
	{ "tree", Tree },
	{ "windows", Windows },
	{ "test", async () =>
		{
			
			await driver
				.First(By.AutomationId("entryUsername"))
				.InputText("xamarin");

			await driver
				.First(By.AutomationId("entryPassword"))
				.InputText("1234");

			await driver
				.First(By.ContainingText("Login"))
				.Tap();

			await driver.None(By.AutomationId("entryUsername"));

			var label = await driver.First(By.ContainingText("Hello, World!"));

			var button = await driver.First(By.AutomationId("buttonOne"));

			await button.Tap();

			await driver.Any(By.Type("Label").ThenContainingText("Current count: 1"));

			Console.WriteLine(label.Text);
		}
	},
	{ "perf", async () =>
		{
			var ind = new List<double>();

			var start = DateTime.UtcNow;
			for (int i = 0; i < 1000; i++)
			{
				var indstart = DateTime.UtcNow;
				var f = await driver.First(e => e.Type == "Label" && e.Text.Contains("count"));
				var t = f.Text;
				var indtotal = DateTime.UtcNow - indstart;
				ind.Add(indtotal.TotalMilliseconds);
			}
			var total = DateTime.UtcNow - start;

			var avg = ind.Average();
			Console.WriteLine($"Time: {total.TotalMilliseconds}  Avg: {avg}");
		}
	}
};

//await mappings["test"]();

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