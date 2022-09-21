using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace Microsoft.Maui.Automation.Driver;

public static partial class DriverExtensions
{
	public static DriverTask<IEnumerable<Element>> Elements(this IDriver driver)
		=> new(driver, driver.GetElements());


	public static async Task<IEnumerable<Element>> By(this IDriver driver, Predicate<Element> matching)
	{
		var elements = await driver.GetElements();
		return elements.Find(matching);
	}

	public static async Task<DriverTask<IEnumerable<Element>>> By(this DriverTask<IEnumerable<Element>> elements, string propertyName, string pattern, bool isRegularExpression = false)
	{
		var actualElements = await elements;
		var results = actualElements.Where(e => e.PropertyMatches(propertyName, pattern, isRegularExpression));

		return new DriverTask<IEnumerable<Element>>(elements.Driver, Task.FromResult(results));
	}

	public static async Task<DriverTask<IEnumerable<Element>>> By(this DriverTask<IEnumerable<Element>> elements, Predicate<Element> predicate)
	{
		var actualElements = await elements;
		var results = actualElements.Where(e => predicate(e));

		return new DriverTask<IEnumerable<Element>>(elements.Driver, Task.FromResult(results));
	}


	public static async Task<Element?> FirstBy(this IDriver driver, Predicate<Element> matching)
	{
		var elements = await driver.GetElements();
		return elements.Find(matching)?.FirstOrDefault();
	}

	public static async Task<DriverTask<Element?>> FirstBy(this DriverTask<IEnumerable<Element>> elements, Predicate<Element> predicate)
	{
		var actualElements = await elements;
		var result = actualElements.Find(predicate)?.FirstOrDefault();

		return new DriverTask<Element?>(elements.Driver, Task.FromResult(result));
	}



	public static Task<Element?> FirstById(this IDriver driver, string id)
		=> driver.FirstBy(e => e.Id == id);

	public static Task<Element?> FirstByAutomationId(this IDriver driver, string automationId)
		=> driver.FirstBy(e => e.AutomationId == automationId);

	public static Task<Element?> FirstWithText(this IDriver driver, string text)
		=> driver.FirstBy(e => e.HasText && e.Text.Contains(text));


	public static Task<DriverTask<Element?>> FirstById(this DriverTask<IEnumerable<Element>> elements, string elementId)
		=> elements.FirstBy(e => e.Id == elementId);

	public static Task<DriverTask<Element?>> FirstByAutomationId(this DriverTask<IEnumerable<Element>> elements, string automationId)
		=> elements.FirstBy(e => e.AutomationId == automationId);

	public static Task<DriverTask<Element?>> FirstWithText(this DriverTask<IEnumerable<Element>> elements, string text)
		=> elements.FirstBy(e => e.HasText && e.Text.Contains(text));

}

