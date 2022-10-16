using Microsoft.Maui.Automation.Querying;
using System.Runtime.CompilerServices;

namespace Microsoft.Maui.Automation.Driver;


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