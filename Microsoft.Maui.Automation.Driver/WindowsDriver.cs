﻿using System;
using AndroidSdk;
using Grpc.Net.Client;
using Microsoft.Maui.Automation.Remote;
using System.Net;

namespace Microsoft.Maui.Automation.Driver
{
    public class WindowsDriver : IDriver
    {
		public WindowsDriver(IAutomationConfiguration configuration)
		{
            Configuration = configuration;

            grpc = new GrpcHost();
        }

        readonly GrpcHost grpc;


        public string Name => "Windows";

        public IAutomationConfiguration Configuration { get; }

        public Task Back()
            => Task.CompletedTask;

        public Task ClearAppState(string appId)
        {
            throw new NotImplementedException();
        }

        public Task<IDeviceInfo> GetDeviceInfo()
        {
            throw new NotImplementedException();
        }

        public Task InputText(string text)
        {
            throw new NotImplementedException();
        }

        public Task InstallApp(string file, string appId)
        {
            throw new NotImplementedException();
        }

        public Task KeyPress(char keyCode)
        {
            throw new NotImplementedException();
        }

        public Task LaunchApp(string appId)
        {
            throw new NotImplementedException();
        }

        public Task LongPress(int x, int y)
        {
            throw new NotImplementedException();
        }

        public Task OpenUri(string uri)
        {
            throw new NotImplementedException();
        }

        public Task PullFile(string appId, string remoteFile, string localDirectory)
        {
            throw new NotImplementedException();
        }

        public Task PushFile(string appId, string localFile, string destinationDirectory)
        {
            throw new NotImplementedException();
        }

        public Task RemoveApp(string appId)
        {
            throw new NotImplementedException();
        }

        public Task StopApp(string appId)
        {
            throw new NotImplementedException();
        }

        public Task Swipe((int x, int y) start, (int x, int y) end)
        {
            throw new NotImplementedException();
        }

        public Task Tap(int x, int y)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetProperty(Platform platform, string elementId, string propertyName)
            => grpc.Client.GetProperty(platform, elementId, propertyName);

        public Task<IEnumerable<Element>> GetElements(Platform platform)
            => grpc.Client.GetElements(platform);

        public Task<IEnumerable<Element>> FindElements(Platform platform, string propertyName, string pattern, bool isExpression = false, string ancestorId = "")
            => grpc.Client.FindElements(platform, propertyName, pattern, isExpression, ancestorId);

    }
}

