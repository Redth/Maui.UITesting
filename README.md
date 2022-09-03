# .NET MAUI UI Testing

.NET MAUI is the evolution of Xamarin.Forms, now it's time for the evolution of UI Testing .NET apps.

This project aims to bring an existing UI Testing spike and idea closer to a usable tool for creating UI Automation Tests for .NET MAUI apps. It is a simpler architecture than its predecessor Xamarin.UITest, and has a much cleaner integration than Appium, built with .NET MAUI apps first in mind.

## Goals / Features
- Support for: Android, iOS, MacCatalyst, WindowsAppSDK
- gRPC communication channel between Driver and AppHost
- .NET MAUI first view tree inspection and querying
- Android - ADB integrations for advanced test manipulation
- iOS - idb_companion for advanced test manipulation
- Fluent Driver API for view querying/actions
- CLI REPL for quickly navigating/testing/inspecting apps
- YAML test syntax as an alternative approach to writing tests and rapidly iterating on them with file watching / 'hot reload' of tests

## So far
- [x] View hierarchy walking/parsing for MAUI, Android, iOS/MacCatalyst, Windows
- [x] gRPC communication channel between driver and app host
- [x] View hierarchy walking for MAUI, Android, iOS/MacCatalyst, Windows
- [x] Basic Android Driver functions with ADB
- [x] Simple REPL with hard coded commands for printing a tree