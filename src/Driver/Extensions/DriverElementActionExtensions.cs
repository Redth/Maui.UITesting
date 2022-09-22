using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation.Driver;

public static partial class DriverExtensions
{
	public static async Task<Element?> Tap(this Task<Element?> element)
	{
		var e = await element;
		if (e?.Driver is not null)
			await e.Driver.Tap(e);
		return e;
	}

	public static async Task<Element?> Tap(this Element? element)
	{
		if (element?.Driver is not null)
			await element.Driver.Tap(element);
		return element;
	}

	public static async Task<Element?> LongPress(this Task<Element?> element)
	{
		var e = await element;
		if (e?.Driver is not null)
			await e.Driver.LongPress(e);
		return e;
	}

	public static async Task<Element?> LongPress(this Element? element)
	{
		if (element?.Driver is not null)
			await element.Driver.LongPress(element);
		return element;
	}

	public static async Task<Element?> InputText(this Task<Element?> element, string text)
	{
		var e = await element;
		if (e?.Driver is not null)
			await e.Driver.InputText(e, text);
		return e;
	}
	public static async Task<Element?> InputText(this Element? element, string text)
	{
		if (element?.Driver is not null)
			await element.Driver.InputText(element, text);
		return element;
	}
}
