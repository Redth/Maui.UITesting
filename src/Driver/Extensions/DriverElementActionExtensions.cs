using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation.Driver;

public static partial class DriverExtensions
{
	public static async Task<IElement?> Tap(this Task<IElement?> element, IDriver driver)
	{
		var e = await element;
		if (e is not null)
			await driver.Tap(e);
		return e;
	}

	public static async Task<IElement?> Tap(this IElement? element, IDriver driver)
	{
		if (element is not null)
			await driver.Tap(element);
		return element;
	}

	public static async Task<IElement?> LongPress(this Task<IElement?> element, IDriver driver)
	{
		var e = await element;
		if (e is not null)
			await driver.LongPress(e);
		return e;
	}

	public static async Task<IElement?> LongPress(this IElement? element, IDriver driver)
	{
		if (element is not null)
			await driver.LongPress(element);
		return element;
	}

	public static async Task<IElement?> InputText(this Task<IElement?> element, IDriver driver, string text)
	{
		var e = await element;
		if (e is not null)
			await driver.InputText(e, text);
		return e;
	}
	public static async Task<IElement?> InputText(this IElement? element, IDriver driver, string text)
	{
		if (element is not null)
			await driver.InputText(element, text);
		return element;
	}

	public static async Task<IElement?> ClearText(this Task<IElement?> element, IDriver driver, string text)
	{
		var e = await element;
		if (e is not null)
			await driver.ClearText(e);
		return e;
	}
	public static async Task<IElement?> ClearText(this IElement? element, IDriver driver, string text)
	{
		if (element is not null)
			await driver.ClearText(element);
		return element;
	}
}
