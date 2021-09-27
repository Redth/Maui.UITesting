using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.WebDriver.Host
{
    internal class Sample
    {
        public static void Do()
        {

            var driver = new MauiDriver();

            var e = driver.FindElementById("automationIdHere") as IPlatformElement;

            Console.WriteLine(e.Text);

        }
    }
}
