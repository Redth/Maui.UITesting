using System;
using System.Diagnostics;

namespace Microsoft.Maui.Automation.Driver;

internal class ProcessRunner
{
	readonly List<string> standardOutput;
	readonly List<string> standardError;
	readonly Process process;

	public ProcessRunner(string executable, params string[] args)
		: this(executable, args, System.Threading.CancellationToken.None)
	{ }

	public ProcessRunner(string executable, string[] args, System.Threading.CancellationToken cancelToken, bool redirectStandardInput = false)
	{
		standardOutput = new List<string>();
		standardError = new List<string>();

		//* Create your Process
		process = new Process();
		process.StartInfo.FileName = executable;
		process.StartInfo.Arguments = string.Join(" ", args);
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;

		if (redirectStandardInput)
			process.StartInfo.RedirectStandardInput = true;

		process.OutputDataReceived += (s, e) =>
		{
			if (e.Data != null)
			{
				standardOutput.Add(e.Data);
				OutputLine?.Invoke(this, e.Data);
			}
		};
		process.ErrorDataReceived += (s, e) =>
		{
			if (e.Data != null)
			{
				standardError.Add(e.Data);
				OutputLine?.Invoke(this, e.Data);
			}
		};
		process.Start();
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();

		if (cancelToken != System.Threading.CancellationToken.None)
		{
			cancelToken.Register(() =>
			{
				try { process.Kill(); }
				catch { }
			});
		}
	}

	public int ExitCode
		=> process.HasExited ? process.ExitCode : -1;

	public bool HasExited
		=> process?.HasExited ?? false;

	public void Kill()
		=> process?.Kill();

	public event EventHandler<string> OutputLine;

	public IEnumerable<string> Output
		=> standardOutput.Concat(standardError);

	public void StandardInputWrite(string input)
	{
		if (!process.StartInfo.RedirectStandardInput)
			throw new InvalidOperationException();

		process.StandardInput.Write(input);
	}

	public void StandardInputWriteLine(string input)
	{
		if (!process.StartInfo.RedirectStandardInput)
			throw new InvalidOperationException();

		process.StandardInput.WriteLine(input);
	}

	public ProcessResult WaitForExit()
	{
		process.WaitForExit();

		return new ProcessResult(standardOutput, standardError, process.ExitCode);
	}

	public Task<ProcessResult> WaitForExitAsync()
	{
		var tcs = new TaskCompletionSource<ProcessResult>();

		Task.Run(() =>
		{
			var r = WaitForExit();
			tcs.TrySetResult(r);
		});

		return tcs.Task;
	}
}

internal class ProcessResult
{
	public readonly List<string> StandardOutput;
	public readonly List<string> StandardError;

	public readonly int ExitCode;

	public bool Success
		=> ExitCode == 0;

	public string GetAllOutput()
		=> string.Join(Environment.NewLine, StandardOutput.Concat(StandardError));

	public string GetOutput()
		=> string.Join(Environment.NewLine, StandardOutput);

	internal ProcessResult(List<string> stdOut, List<string> stdErr, int exitCode)
	{
		StandardOutput = stdOut;
		StandardError = stdErr;
		ExitCode = exitCode;
	}
}

