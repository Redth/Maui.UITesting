using Microsoft.Maui.Automation.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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


        public async Task<IEnumerable<IElement>> Execute()
			=> await Query.Execute(Driver, await Driver.GetElements().ConfigureAwait(false)).ConfigureAwait(false);


        public TaskAwaiter<IEnumerable<IElement>> GetAwaiter()
		{
			return Execute().GetAwaiter();
		}


    }
}
