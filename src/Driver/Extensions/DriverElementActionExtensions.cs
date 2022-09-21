using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation.Driver;

public static partial class DriverExtensions
{
	public static async Task<DriverTask> Tap(this Task<DriverTask<Element?>> element)
	{
		var e = await element;
		return await e.Tap();
	}

	public static async Task<DriverTask> Tap(this DriverTask<Element?> element)
	{
		var p = await element;
		return new DriverTask(element.Driver, element.Driver.Tap(p!));
	}


	public static async Task<DriverTask> LongPress(this Task<DriverTask<Element?>> element)
	{
		var e = await element;
		return await e.LongPress();
	}

	public static async Task<DriverTask> LongPress(this DriverTask<Element?> element)
	{
		var p = await element;
		return new DriverTask(element.Driver, element.Driver.LongPress(p!));
	}

	public static async Task<DriverTask> InputText(this Task<DriverTask<Element?>> element, string text)
	{
		var e = await element;
		return new DriverTask(e.Driver, e.Driver.InputText(text));
	}

	public static Task<DriverTask> InputText(this DriverTask<Element?> element, string text)
		=> Task.FromResult(new DriverTask(element.Driver, element.Driver.InputText(text)));
}
