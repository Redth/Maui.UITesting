using Microsoft.AspNetCore.Mvc;
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

	public static async Task<IEnumerable<Element>> All(this IDriver driver, Query query)
	{
		var elements = await driver.GetElements();

		return query.Execute(elements);
	}

	public static async Task<Element?> First(this IDriver driver, Query query)
	{
		var elements = await driver.GetElements();

		var r = query.Execute(elements);
		return r.FirstOrDefault();
	}


	public static Task<IEnumerable<Element>> All(this IDriver driver, Predicate<Element> predicate)
		=> driver.All(Query.By(predicate));


	public static Task<Element?> First(this IDriver driver, Predicate<Element> predicate)
		=> driver.First(Query.By(predicate));

	//static async Task<IEnumerable<Element>> AutoWait(this IDriver driver, Predicate<Element> matching, int autoWaitMs = 3000, int retryDelayMs = 200)
	//{
	//	var waited = 0;

	//	while (waited < autoWaitMs || autoWaitMs <= 0)
	//	{
	//		var elements = await driver.GetElements();

	//		var r = elements.Find(matching);

	//		if (autoWaitMs <= 0 || r.Any())
	//			return r;

	//		Thread.Sleep(retryDelayMs);
	//		waited += retryDelayMs;
	//	}

	//	return Enumerable.Empty<Element>();
	//}

	//static async Task<DriverTask<IEnumerable<Element>>> AutoWait(this Task<DriverTask<IEnumerable<Element>>> elements, Predicate<Element> matching, int autoWaitMs = 3000, int retryDelayMs = 200)
	//{
	//	var inner = await elements;
	//	return await inner.AutoWait(matching, autoWaitMs, retryDelayMs);
	//}

	//static async Task<DriverTask<Element?>> AutoWaitFirst(this Task<DriverTask<IEnumerable<Element>>> elements, Predicate<Element> matching, int autoWaitMs = 3000, int retryDelayMs = 200)
	//{
	//	var inner = await elements;
	//	var w = await inner.AutoWait(matching, autoWaitMs, retryDelayMs);
	//	return new DriverTask<Element?>(w.Driver, Task.FromResult((await w.Value).FirstOrDefault()));
	//}

	//static async Task<DriverTask<Element?>> AutoWaitFirst(this DriverTask<IEnumerable<Element>> elements, Predicate<Element> matching, int autoWaitMs = 3000, int retryDelayMs = 200)
	//{
	//	var w = await elements.AutoWait(matching, autoWaitMs, retryDelayMs);
	//	return new DriverTask<Element?>(w.Driver, Task.FromResult((await w.Value).FirstOrDefault()));
	//}

	//static async Task<DriverTask<IEnumerable<Element>>> AutoWait(this DriverTask<IEnumerable<Element>> elements, Predicate<Element> matching, int autoWaitMs = 3000, int retryDelayMs = 200)
	//{
	//	var driver = elements.Driver;
	//	var waited = 0;

	//	while (waited < autoWaitMs || autoWaitMs <= 0)
	//	{
	//		var matches = new List<Element>();



	//		// Get a free tree
	//		var tree = await driver.GetElements();
	//		var targets = await elements.Value;

	//		foreach (var target in targets)
	//		{
	//			var treeTargets = tree.Find(t => t.Id == target.Id);

	//			var targetMatches = treeTargets.Where(s => matching(s));

	//			if (targetMatches.Any())
	//				matches.AddRange(targetMatches);
	//		}

	//		if (autoWaitMs <= 0 || matches.Any())
	//			return new DriverTask<IEnumerable<Element>>(driver, Task.FromResult<IEnumerable<Element>>(matches));

	//		Thread.Sleep(retryDelayMs);
	//		waited += retryDelayMs;
	//	}

	//	return new DriverTask<IEnumerable<Element>>(driver, Task.FromResult<IEnumerable<Element>>(Enumerable.Empty<Element>()));
	//}

}

