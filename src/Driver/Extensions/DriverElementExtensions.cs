using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation.Driver;

public static partial class DriverExtensions
{
	public static async Task<string?> Text(this Task<DriverTask<Element?>> element)
		=> (await (await element).Value)?.Text;

	public static async Task<string?> Text(this DriverTask<Element?> element)
		=> (await element)?.Text;

	public static async Task<string?> Id(this Task<DriverTask<Element?>> element)
		=> (await (await element).Value)?.Text;

	public static async Task<string?> Id(this DriverTask<Element?> element)
		=> (await element)?.Id;
}
