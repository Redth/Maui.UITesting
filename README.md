# .NET MAUI / Xamarin WebDriver

What if we had a Selenium/Appium WebDriver implementation for .NET MAUI / Xamarin written directly against the C# view API's?  What if this also included a WebDriver that had knowledge of MAUI API's as well?

This is a brief experiment to see what the effort involved might be to create these drivers.

## So far

- IPlatformElement - an extension of IWebElement with a bit more info
- AndroidElement / iOSElement / MauiElement - Implementations of IPlatformElement which map to 'native' view data
- PlatformDriverBase - an abstract driver that knows how to query for IPlatformElement's
- AndroidDriver / iOSDriver / MauiDriver - implementations of the base driver providing a root to the visual element tree

## Todo

- WebDriver host HTTP Server - to run within the app, for the remote driver to connect to, which implements the necessary HTTP endpoints and proxies them to the correct AndroidDriver / iOSDriver / MauiDriver
- Remote Drivers for Android/iOS/Maui - Create a subclass of RemoteWebDriver to connect to the platform specific HTTP servers
- MacCatalyst
- Windows

