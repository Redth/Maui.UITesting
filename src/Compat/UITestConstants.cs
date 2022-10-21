using System;

namespace Xamarin.UITest;

/// <summary>
/// Default parameter values for various UITest API functions
/// </summary>
public static class UITestConstants
{
    /// <summary>
    /// The IPAddress to use if the user has not specified one.
    /// </summary>
    public const string DefaultDeviceIp = "127.0.0.1";

    /// <summary>
    /// Default percent (from 0 to 1.0) of view to scroll/swipe interactions.
    /// </summary> 
    public const double DefaultSwipePercentage = 0.67;

    /// <summary>
    /// Default speed (pixels/points per second) for scroll/swipe interactions.
    /// </summary>
    public const int DefaultSwipeSpeed = 500;

    /// <summary>
    /// A dummy launch argument use to identify apps running on an iOS simulator
    /// that were started by UITest.
    /// </summary>
    public const string AUTArgIdentifier = "ApplicationUnderTestLaunchedByUITest";
}