# .NET MAUI UI Testing

.NET MAUI is the evolution of Xamarin.Forms, now it's time for the evolution of UI Testing .NET apps.

This project aims to bring an existing UI Testing spike and idea closer to a usable tool for creating UI Automation Tests for .NET MAUI apps. It is a simpler architecture than its predecessor Xamarin.UITest, and has a much cleaner integration than Appium, built with .NET MAUI apps first in mind.

[![.NET MAUI UI Testing - HackWeek](https://yt-embed.herokuapp.com/embed?v=9LxQwpjKxhE)](https://www.youtube.com/embed/9LxQwpjKxhE ".NET MAUI UI Testing - HackWeek")

## NuGet.org Packages
![Nuget: Interactive](https://img.shields.io/nuget/vpre/Redth.Microsoft.Maui.Automation.Interactive?color=blue&label=.NET%20Interactive%20/%20Workbooks)
For .NET Interactive Notebooks

![Nuget: Unit Test Driver](https://img.shields.io/nuget/vpre/Redth.Microsoft.Maui.Automation.Driver?color=blue&label=Unit%20Test%20Driver)
xUnit/NUnit Unit Test projects

![Nuget: App Agent](https://img.shields.io/nuget/vpre/Redth.Microsoft.Maui.Automation.Agent?color=blue&label=App%20Agent)
Include in your app project.

## Features
- Support for: Android, iOS, WindowsAppSDK, _MacCatalyst * (Incomplete Driver)_
- gRPC communication channel between Driver and App Agent
- .NET MAUI first view tree inspection and querying
- Android - ADB integrations for advanced test manipulation
- iOS - idb_companion for advanced test manipulation
- Windows - WinAppDriver for additional manipulation
- Fluent Driver API for view querying/actions
- .NET Interactive Notebook for iterating on test writing / view tree & app exploration


## Future ideas
- Create native Android and iOS App Agent libraries that implement the gRPC spec so non-MAUI apps can be tested



