using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public class WindowsAppSdkApplication : Application
    {
        public override Task<IActionResult> Invoke(IView view, IAction action)
        {
            throw new System.NotImplementedException();
        }

        public override Task<IWindow[]> Windows()
            => Task.FromResult(Array.Empty<IWindow>());
    }
}