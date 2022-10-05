using Microsoft.Maui.Automation.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation.Querying
{
	public class DriverQuery
	{
		public DriverQuery(IDriver driver)
		{
			Driver = driver;
			Query = new Query();
		}

		public DriverQuery(IDriver driver, Query query)
		{
			Driver = driver;
			Query = query;
		}

		public DriverQuery(IDriver driver, Platform automationPlatform)
		{
			Driver = driver;
			Query = new Query(automationPlatform);
		}

		public readonly IDriver Driver;
		public readonly Query Query;
	}
}
