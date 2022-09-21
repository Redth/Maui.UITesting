using AndroidSdk;
using Grpc.Net.Client;
using Microsoft.Maui.Automation.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Idb;
using Grpc.Core;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace Microsoft.Maui.Automation.Driver;


public class MacDriver : IDriver
{
	public MacDriver(IAutomationConfiguration configuration)
	{
		Configuration = configuration;

		var port = configuration.AppAgentPort;
		var address = configuration.AppAgentAddress;


		Name = $"Mac ({configuration.Device})";

		if (string.IsNullOrEmpty(Configuration.AppId))
			Configuration.AppId = AppUtil.GetBundleIdentifier(Configuration.AppFilename)
				?? throw new Exception("AppId not found");

		var channel = GrpcChannel.ForAddress($"http://{address}:10882");
		grpc = new GrpcHost();
	}

	readonly GrpcHost grpc;

	public string Name { get; }

	public IAutomationConfiguration Configuration { get; }

	public Task ClearAppState()
		=> Task.CompletedTask;

	public Task InstallApp()
		=> Task.CompletedTask;

	public Task RemoveApp()
		=> Task.CompletedTask;

	public Task<IDeviceInfo> GetDeviceInfo()
		=> Task.FromResult<IDeviceInfo>(new DeviceInfo());

	public Task LaunchApp()
		=> Process.Start(new ProcessStartInfo("/usr/bin/open", $"-n \"{Configuration.AppFilename}\"")
		{
			CreateNoWindow = true,
			UseShellExecute = true
		})!.WaitForExitAsync();

	public Task StopApp()
		=> Task.CompletedTask;

	public Task OpenUri(string uri)
		=> Process.Start(new ProcessStartInfo("/usr/bin/open", $"{uri}")
		{
			CreateNoWindow = true,
			UseShellExecute = true
		})!.WaitForExitAsync();


	public Task PushFile(string localFile, string destinationDirectory)
	{
		var bundleRoot = Path.Combine(Configuration.AppFilename, "Contents");
		var dest = Path.Combine(bundleRoot, Path.GetFileName(localFile));
		File.Copy(localFile, dest);
		return Task.CompletedTask;
	}

	public Task PullFile(string remoteFile, string localDirectory)
	{
        var bundleRoot = Path.Combine(Configuration.AppFilename, "Contents");
		var src = Path.Combine(bundleRoot, remoteFile);
		var dest = Path.Combine(localDirectory, Path.GetFileName(remoteFile));
		File.Copy(src, dest);
        return Task.CompletedTask;
	}


	public Task InputText(string text)
		=> Task.CompletedTask;

	public Task Back()
		=> Task.CompletedTask;

	public Task KeyPress(char keyCode)
		=> Task.CompletedTask;

	public Task Tap(int x, int y)
		=> Task.CompletedTask;

	public Task Tap(Element element)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, Actions.Tap, element.Id);

	public Task LongPress(int x, int y)
		=> Task.CompletedTask;

	public Task LongPress(Element element)
			=> Tap(element);

	public Task Swipe((int x, int y) start, (int x, int y) end)
		=> Task.CompletedTask;

	public Task<string?> GetProperty(string elementId, string propertyName)
			=> grpc.Client.GetProperty(Configuration.AutomationPlatform, elementId, propertyName);

	public Task<IEnumerable<Element>> GetElements()
		=> grpc.Client.GetElements(Configuration.AutomationPlatform);

	public Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, action, elementId, arguments);

    public Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args)
        => grpc.Client.Backdoor(Configuration.AutomationPlatform, fullyQualifiedTypeName, staticMethodName, args);


    public async void Dispose()
	{
		if (grpc is not null)
			await grpc.Stop();
	}
}
