using Microsoft.AspNetCore.Mvc;
using Microsoft.Maui.Automation.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.Maui.Automation.Driver;

public static partial class DriverExtensions
{

	public static DriverQuery On(this IDriver driver, Platform automationPlatform)
		=> new DriverQuery(driver, automationPlatform);

	public static DriverQuery Query(this IDriver driver)
		=> new DriverQuery(driver);





	public static Task<IEnumerable<IElement>> All(this IDriver driver, Query query)
		=> driver.AutoWait(query);

	public static Task<IEnumerable<IElement>> Any(this IDriver driver, Query query)
		=> driver.AutoWait(query);

	public static Task<IElement?> First(this IDriver driver, Query query)
		=> driver.AutoWaitFirst(query);


	public static Task<IEnumerable<IElement>> All(this IDriver driver, Predicate<IElement> predicate)
		=> driver.All(Query.By(predicate));

	public static Task<IEnumerable<IElement>> Any(this IDriver driver, Predicate<IElement> predicate)
		=> driver.Any(Query.By(predicate));

	public static Task<IElement?> First(this IDriver driver, Predicate<IElement> predicate)
		=> driver.First(Query.By(predicate));

	public static Task None(this IDriver driver, Query query)
		=> driver.AutoWait(query, waitForNone: true);

	public static Task None(this IDriver driver, Predicate<IElement> predicate)
		=> driver.None(Query.By(predicate));


	static async Task<IElement?> AutoWaitFirst(this IDriver driver, Query query, int autoWaitMs = 3000, int retryDelayMs = 200)
		=> (await driver.AutoWait(query, autoWaitMs, retryDelayMs).ConfigureAwait(false)).FirstOrDefault();


	static async Task<IEnumerable<IElement>> AutoWait(this DriverQuery query, int autoWaitMs = 3000, int retryDelayMs = 200, bool waitForNone = false)
	{
		var elements = await AutoWait(query.Driver, query.Query, autoWaitMs, retryDelayMs).ConfigureAwait(false);


		return query;
	}

	static async Task<IEnumerable<IElement>> AutoWait(this IDriver driver, Query query, int autoWaitMs = 3000, int retryDelayMs = 200, bool waitForNone = false)
	{
		var waited = 0;

		while (waited < autoWaitMs || autoWaitMs <= 0)
		{
			// See which automation platform to use
			var platform = query.AutomationPlatform ?? driver.Configuration.AutomationPlatform;

			var elements = await driver.GetElements(platform).ConfigureAwait(false);

			var results = await query.Execute(driver, elements);

			var anyResults = results.Any();

			if (waitForNone)
			{
				// Wait until no results found
				if (autoWaitMs <= 0 || !anyResults)
				{
					if (anyResults)
						throw new ElementsStillFoundException(query);

					return results;
				}
			}
			else
			{
				// Wait until we find 1 or more
				if (autoWaitMs <= 0 || anyResults)
					return results;
			}

			Thread.Sleep(retryDelayMs);
			waited += retryDelayMs;
		}

		if (waitForNone)
			throw new ElementsStillFoundException(query);
		else
			throw new ElementsNotFoundException(query);
	}
}

