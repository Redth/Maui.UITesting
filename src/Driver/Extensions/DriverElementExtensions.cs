//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Microsoft.Maui.Automation.Driver;

//public static partial class DriverExtensions
//{
//	public static async Task<Element?> Element(this Task<DriverTask<Element?>> element)
//		=> await (await element).Value;

//	public static async Task<Element?> Element(this DriverTask<Element?> element)
//		=> await element;


//	public static async Task<string?> GetText(this Task<DriverTask<Element?>> element)
//		=> (await element.Element())?.Text;

//	public static async Task<string?> GetText(this DriverTask<Element?> element)
//		=> (await element)?.Text;

//	public static async Task<string?> GetId(this Task<DriverTask<Element?>> element)
//		=> (await element.Element())?.Id;

//	public static async Task<string?> GetId(this DriverTask<Element?> element)
//		=> (await element)?.Id;

//	public static async Task<string?> GetAutomationId(this Task<DriverTask<Element?>> element)
//		=> (await element.Element())?.AutomationId;

//	public static async Task<string?> GetAutomationId(this DriverTask<Element?> element)
//		=> (await element)?.AutomationId;

//	public static async Task<IEnumerable<Element>> GetChildren(this Task<DriverTask<Element?>> element)
//		=> (await element.Element())?.Children ?? Enumerable.Empty<Element>();

//	public static async Task<IEnumerable<Element>> GetChildren(this DriverTask<Element?> element)
//		=> (await element)?.Children ?? Enumerable.Empty<Element>();
//}
