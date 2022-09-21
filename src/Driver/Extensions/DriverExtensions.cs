using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation.Driver;

public static class DriverExtensions
{
	public static async Task<Element?> FirstById(this IDriver driver, string id)
	{
		var elements = await driver.GetElements();

		return elements.FirstById(id);
	}

	public static async Task<Element?> FirstByAutomationId(this IDriver driver, string automationId)
	{
		var elements = await driver.GetElements();

		var e = elements.FirstByAutomationId(automationId);
		return e;
	}

	public static async Task<IEnumerable<Element>> By(this IDriver driver, Predicate<Element> matching)
	{
		var elements = await driver.GetElements();

		return elements.Find(matching);
	}

	public static DriverTask<IEnumerable<Element>> Elements(this IDriver driver)
		=> new(driver, driver.GetElements());

	public static async Task<DriverTask<Element?>> FirstById(this DriverTask<IEnumerable<Element>> elements, string elementId)
	{
		var actualElements = await elements;
		
		var result = actualElements.FirstOrDefault(e => e.Id.Equals(elementId));

		return new DriverTask<Element?>(elements.Driver, Task.FromResult(result));
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

    public static async Task<DriverTask<Element?>> FirstBy(this DriverTask<IEnumerable<Element>> elements, Predicate<Element> predicate)
    {
		var actualElements = await elements;
        var results = actualElements.FirstOrDefault(e => predicate(e));

        return new DriverTask<Element?>(elements.Driver, Task.FromResult(results));
    }

    public static async Task<Element?> Element(this Task<DriverTask<Element?>> element)
    {
        var p = await element;
		return await p.Value;
    }

    public static async Task<Element?> Element(this DriverTask<Element?> element)
    {
        return await element;
    }

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
		return await e.InputText(text);
    }

    public static async Task<DriverTask> InputText(this DriverTask<Element?> element, string text)
    {
        var p = await element;

        return new DriverTask(element.Driver, element.Driver.InputText(text));
    }
}

public class DriverTask<T>
{
	public DriverTask(IDriver driver, Task<T> value)
	{
		Value = value;
		Driver = driver;

	}

	public readonly Task<T> Value;

	public readonly IDriver Driver;

	public TaskAwaiter<T> GetAwaiter()
		=> Value.GetAwaiter();
}

public class DriverTask
{
	public DriverTask(IDriver driver, Task value)
	{
		Value = value;
		Driver = driver;

	}

	public readonly Task Value;

	public readonly IDriver Driver;

	public TaskAwaiter GetAwaiter()
		=> Value.GetAwaiter();
}
