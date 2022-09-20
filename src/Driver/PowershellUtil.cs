public class PowershellUtil
{
	public static void Run(string script)
	{
		var moduleFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell", "v1.0", "Modules", "Appx", "Appx.psd1");
		var ps = System.Management.Automation.PowerShell.Create();
		ps.Invoke();
		
		ps.Invoke();
		if (ps.HadErrors)
			throw new AggregateException(ps.Streams.Error.Select(s => s.Exception).ToList());
	}
}
