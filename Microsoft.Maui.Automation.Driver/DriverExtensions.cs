using System;
using System.Collections.Generic;
using System.Linq;
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

	

	public static Task<IEnumerable<Element>> ByType(this IDriver driver, string elementType)
		=> driver.FindElements("Type", elementType);
}
