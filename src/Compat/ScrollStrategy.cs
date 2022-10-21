namespace Xamarin.UITest;

/// <summary>
/// Strategy for scrolling 
/// </summary>
public enum ScrollStrategy
{
    /// <summary>
    /// Use any combination of <c>Programmatically</c> and <c>Gesture</c> when trying to scroll. Biased towards <c>Programmatically</c>
    /// </summary>
    Auto,
    /// <summary>
    /// Scroll programmatically, will not mimic real user interactions, but are as fast as possible.
    /// </summary>
    Programmatically,
    /// <summary>
    /// Scroll using gestures, tries to mimic user interaction as closely as possible.
    /// </summary>
    Gesture
}