using System;
using System.IO;
using Xamarin.UITest.Queries;

namespace Xamarin.UITest;

/// <summary>
/// Represents the main gateway to interact with an app. This interface contains shared
/// functionality between <see cref="AndroidApp"/> and <see cref="iOSApp"/>.
/// </summary>
public interface IApp
{
    /// <summary>
    /// Queries view objects using the fluent API. Defaults to only return view objects that are visible.
    /// </summary>
    /// <param name="query">
    /// Entry point for the fluent API to specify the element. If left as <c>null</c> returns all visible view 
    /// objects.
    /// </param>
    /// <returns>An array representing the matched view objects.</returns>
    AppResult[] Query(Func<AppQuery, AppQuery> query = null);

    /// <summary>
    /// Queries view objects using the fluent API. Defaults to only return view objects that are visible.
    /// </summary>
    /// <param name="marked">
    /// Marked selector to match. See <see cref="AppQuery.Marked" /> for more information.
    /// </param>
    /// <returns>An array representing the matched view objects.</returns>
    AppResult[] Query(string marked);

    /// <summary>
    /// Queries web view objects using the fluent API. Defaults to only return view objects that are visible.
    /// </summary>
    /// <param name="query">
    /// Entry point for the fluent API to specify the element. If left as <c>null</c> returns all visible view
    /// objects.
    /// </param>
    /// <returns>An array representing the matched view objects.</returns>
    AppWebResult[] Query(Func<AppQuery, AppWebQuery> query);

    /// <summary>
    /// Queries properties on view objects using the fluent API. 
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the type of the property.</param> 
    /// <typeparam name="T">The type of the property.</typeparam>
    //T[] Query<T>(Func<AppQuery, AppTypedSelector<T>> query);


    /// <summary>
    /// Invokes Javascript on view objects using the fluent API.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the elements.</param>
    /// <returns>An array of strings representing the results.</returns>
    string[] Query(Func<AppQuery, InvokeJSAppQuery> query);

    /// <summary>
    /// Highlights the results of the query by making them flash. Specify view elements using the fluent API. 
    /// Defaults to all view objects that are visible.
    /// </summary>
    /// <param name="query">
    /// Entry point for the fluent API to specify the elements. If left as <c>null</c> flashes all visible view 
    /// objects.
    /// </param>
    /// <returns>An array representing the matched view objects.</returns>
    AppResult[] Flash(Func<AppQuery, AppQuery> query = null);

    /// <summary>
    /// Highlights the results of the query by making them flash. Specify view elements using marked string. 
    /// </summary>
    /// <param name="marked">
    /// Marked selector to match. See <see cref="AppQuery.Marked" /> for more information.
    /// </param>
    /// <returns>An array representing the matched view objects.</returns>
    AppResult[] Flash(string marked);

    /// <summary>
    /// Enters text into the currently focused element.
    /// </summary>
    /// <param name="text">The text to enter.</param>
    void EnterText(string text);

    /// <summary>
    /// Enters text into a matching element that supports it. 
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    /// <param name="text">The text to enter.</param>
    void EnterText(Func<AppQuery, AppQuery> query, string text);

    /// <summary>
    /// Enters text into a matching element that supports it. 
    /// </summary>
    /// <param name="marked">
    /// Marked selector to match. See <see cref="AppQuery.Marked" /> for more information.
    /// </param>
    /// <param name="text">The text to enter.</param>
    void EnterText(string marked, string text);


    /// <summary>
    /// Enters text into a matching element that supports it. 
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    /// <param name="text">The text to enter.</param>
    void EnterText(Func<AppQuery, AppWebQuery> query, string text);

    /// <summary>
    /// Clears text from a matching element that supports it. 
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    void ClearText(Func<AppQuery, AppQuery> query);

    /// <summary>
    /// Clears text from a matching element that supports it. 
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    void ClearText(Func<AppQuery, AppWebQuery> query);

    /// <summary>
    /// Clears text from a matching element that supports it. 
    /// </summary>
    /// <param name="marked">
    /// Marked selector to match. See <see cref="AppQuery.Marked" /> for more information.
    /// </param>
    void ClearText(string marked);

    /// <summary>
    /// Clears text from the currently focused element. 
    /// </summary>
    void ClearText();

    /// <summary>
    /// Presses the enter key in the app. 
    /// </summary>
    void PressEnter();

    /// <summary>
    /// Hides keyboard if present
    /// </summary>
    void DismissKeyboard();

    /// <summary>
    /// Performs a tap / touch gesture on the matched element. If multiple elements are matched, the first one
    ///  will be used.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    void Tap(Func<AppQuery, AppQuery> query);

    /// <summary>
    /// Performs a tap / touch gesture on the matched element. If multiple elements are matched, the first one 
    /// will be used.
    /// </summary>
    /// <param name="marked">
    /// Marked selector to match. See <see cref="AppQuery.Marked" /> for more information.
    /// </param>
    void Tap(string marked);


    /// <summary>
    /// Performs a tap / touch gesture on the matched element. If multiple elements are matched, the first one 
    /// will be used.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    void Tap(Func<AppQuery, AppWebQuery> query);

    /// <summary>
    /// Performs a tap / touch gesture on the given coordinates.
    /// </summary>
    /// <param name="x">The x coordinate to tap.</param>
    /// <param name="y">The y coordinate to tap.</param>
    void TapCoordinates(float x, float y);

    /// <summary>
    /// Performs a continuous touch gesture on the matched element. If multiple elements are matched, the first 
    /// one will be used.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    void TouchAndHold(Func<AppQuery, AppQuery> query);

    /// <summary>
    /// Performs a continuous touch gesture on the matched element. If multiple elements are matched, the first 
    /// one will be used.
    /// </summary>
    /// <param name="marked">
    /// Marked selector to match. See <see cref="AppQuery.Marked" /> for more information.
    /// </param>
    void TouchAndHold(string marked);

    /// <summary>
    /// Performs a continuous touch gesture on the given coordinates.
    /// </summary>
    /// <param name="x">The x coordinate to touch.</param>
    /// <param name="y">The y coordinate to touch.</param>
    void TouchAndHoldCoordinates(float x, float y);

    /// <summary>
    /// Performs two quick tap / touch gestures on the matched element. If multiple elements are matched, the 
    /// first one will be used.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    void DoubleTap(Func<AppQuery, AppQuery> query);

    /// <summary>
    /// Performs two quick tap / touch gestures on the matched element. If multiple elements are matched, the 
    /// first one will be used.
    /// </summary>
    /// <param name="marked">
    /// Marked selector to match. See <see cref="AppQuery.Marked" /> for more information.
    /// </param>
    void DoubleTap(string marked);

    /// <summary>
    /// Performs a quick double tap / touch gesture on the given coordinates.
    /// </summary>
    /// <param name="x">The x coordinate to touch.</param>
    /// <param name="y">The y coordinate to touch.</param>
    void DoubleTapCoordinates(float x, float y);

    /// <summary>
    /// Performs a pinch gestures on the matched element to zoom the view in. If multiple elements are matched, 
    /// the first one will be used.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    /// <param name="duration">The <see cref="TimeSpan"/> duration of the pinch gesture.</param>
    void PinchToZoomIn(Func<AppQuery, AppQuery> query, TimeSpan? duration = null);

    /// <summary>
    /// Performs a pinch gestures on the matched element to zoom the view in. If multiple elements are matched, 
    /// the first one will be used.
    /// </summary>
    /// <param name="marked">
    /// Marked selector to match. See <see cref="AppQuery.Marked" /> for more information.
    /// </param>
    /// <param name="duration">The <see cref="TimeSpan"/> duration of the pinch gesture.</param>
    void PinchToZoomIn(string marked, TimeSpan? duration = null);

    /// <summary>
    /// Performs a pinch gestures to zoom the view in on the given coordinates.
    /// </summary>
    /// <param name="x">The x coordinate of the center of the pinch.</param>
    /// <param name="y">The y coordinate of the center of the pinch.</param>
    /// <param name="duration">The <see cref="TimeSpan"/> duration of the pinch gesture.</param>
    void PinchToZoomInCoordinates(float x, float y, TimeSpan? duration);

    /// <summary>
    /// Performs a pinch gestures on the matched element to zoom the view out. If multiple elements are matched,
    ///  the first one will be used.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    /// <param name="duration">The <see cref="TimeSpan"/> duration of the pinch gesture.</param>
    void PinchToZoomOut(Func<AppQuery, AppQuery> query, TimeSpan? duration = null);

    /// <summary>
    /// Performs a pinch gestures on the matched element to zoom the view out. If multiple elements are matched,
    ///  the first one will be used.
    /// </summary>
    /// <param name="marked">
    /// Marked selector to match. See <see cref="AppQuery.Marked" /> for more information.
    /// </param>
    /// <param name="duration">The <see cref="TimeSpan"/> duration of the pinch gesture.</param>
    void PinchToZoomOut(string marked, TimeSpan? duration = null);

    /// <summary>
    /// Performs a pinch gestures to zoom the view in on the given coordinates.
    /// </summary>
    /// <param name="x">The x coordinate of the center of the pinch.</param>
    /// <param name="y">The y coordinate of the center of the pinch.</param>
    /// <param name="duration">The <see cref="TimeSpan"/> duration of the pinch gesture.</param>
    void PinchToZoomOutCoordinates(float x, float y, TimeSpan? duration);

    /// <summary>
    /// Generic wait function that will repeatly call the <c>predicate</c> function until it returns <c>true</c>.
    ///  Throws a <see cref="TimeoutException"/> if the predicate is not fullfilled within the time limit.
    /// </summary>
    /// <param name="predicate">Predicate function that should return <c>true</c> when waiting is complete.</param>
    /// <param name="timeoutMessage">The message used in the <see cref="TimeoutException"/>.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    /// <param name="retryFrequency">The <see cref="TimeSpan"/> to wait between each call to the predicate.</param>
    /// <param name="postTimeout">
    /// The final <see cref="TimeSpan"/> to wait after the predicate returns <c>true</c>.
    /// </param>
    void WaitFor(Func<bool> predicate, string timeoutMessage = "Timed out waiting...", TimeSpan? timeout = null,
        TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null);

    /// <summary>
    /// Wait function that will repeatly query the app until a matching element is found. Throws a 
    /// <see cref="TimeoutException"/> if no element is found within the time limit.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    /// <param name="timeoutMessage">The message used in the <see cref="TimeoutException"/>.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    /// <param name="retryFrequency">The <see cref="TimeSpan"/> to wait between each query call to the app.</param>
    /// <param name="postTimeout">
    /// The final <see cref="TimeSpan"/> to wait after the element has been found.
    /// </param>
    /// <returns>An array representing the matched view objects.</returns>
    AppResult[] WaitForElement(Func<AppQuery, AppQuery> query,
        string timeoutMessage = "Timed out waiting for element...", TimeSpan? timeout = null,
        TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null);

    /// <summary>
    /// Wait function that will repeatly query the app until a matching element is found. Throws a 
    /// <see cref="TimeoutException"/> if no element is found within the time limit.
    /// </summary>
    /// <param name="marked">
    /// Marked selector to match. See <see cref="AppQuery.Marked" /> for more information.
    /// </param>
    /// <param name="timeoutMessage">The message used in the <see cref="TimeoutException"/>.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    /// <param name="retryFrequency">The <see cref="TimeSpan"/> to wait between each query call to the app.</param>
    /// <param name="postTimeout">
    /// The final <see cref="TimeSpan"/> to wait after the element has been found.
    /// </param>
    /// <returns>An array representing the matched view objects.</returns>
    AppResult[] WaitForElement(string marked, string timeoutMessage = "Timed out waiting for element...",
        TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null);

    /// <summary>
    /// Wait function that will repeatly query the app until a matching element is found. Throws a 
    /// <see cref="TimeoutException"/> if no element is found within the time limit.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    /// <param name="timeoutMessage">The message used in the <see cref="TimeoutException"/>.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    /// <param name="retryFrequency">The <see cref="TimeSpan"/> to wait between each query call to the app.</param>
    /// <param name="postTimeout">
    /// The final <see cref="TimeSpan"/> to wait after the element has been found.
    /// </param>
    /// <returns>An array representing the matched view objects.</returns>
    AppWebResult[] WaitForElement(Func<AppQuery, AppWebQuery> query,
        string timeoutMessage = "Timed out waiting for element...", TimeSpan? timeout = null,
        TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null);

    /// <summary>
    /// Wait function that will repeatly query the app until a matching element is no longer found. Throws a 
    /// <see cref="TimeoutException"/> if the element is visible at the end of the time limit.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    /// <param name="timeoutMessage">The message used in the <see cref="TimeoutException"/>.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    /// <param name="retryFrequency">The <see cref="TimeSpan"/> to wait between each query call to the app.</param>
    /// <param name="postTimeout">
    /// The final <see cref="TimeSpan"/> to wait after the element is no longer visible.
    /// </param>
    void WaitForNoElement(Func<AppQuery, AppQuery> query,
        string timeoutMessage = "Timed out waiting for no element...", TimeSpan? timeout = null,
        TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null);

    /// <summary>
    /// Wait function that will repeatly query the app until a matching element is no longer found. Throws a 
    /// <see cref="TimeoutException"/> if the element is visible at the end of the time limit.
    /// </summary>
    /// <param name="marked">
    /// Marked selector to match. See <see cref="AppQuery.Marked" /> for more information.
    /// </param>
    /// <param name="timeoutMessage">The message used in the <see cref="TimeoutException"/>.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    /// <param name="retryFrequency">The <see cref="TimeSpan"/> to wait between each query call to the app.</param>
    /// <param name="postTimeout">
    /// The final <see cref="TimeSpan"/> to wait after the element is no longer visible.
    /// </param>
    void WaitForNoElement(string marked, string timeoutMessage = "Timed out waiting for no element...",
        TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null);

    /// <summary>
    /// Wait function that will repeatly query the app until a matching element is no longer found. Throws a 
    /// <see cref="TimeoutException"/> if the element is visible at the end of the time limit.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    /// <param name="timeoutMessage">The message used in the <see cref="TimeoutException"/>.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    /// <param name="retryFrequency">The <see cref="TimeSpan"/> to wait between each query call to the app.</param>
    /// <param name="postTimeout">
    /// The final <see cref="TimeSpan"/> to wait after the element is no longer visible.
    /// </param>
    void WaitForNoElement(Func<AppQuery, AppWebQuery> query,
        string timeoutMessage = "Timed out waiting for no element...", TimeSpan? timeout = null,
        TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null);

    /// <summary>
    /// Takes a screenshot of the app in it's current state. This is used to denote test steps in the Xamarin 
    /// Test Cloud. When executed locally, the <see cref="FileInfo"/> returned is the file the screenshot has
    /// been saved to.
    /// </summary>
    /// <param name="title">The title of screenshot, used as step name.</param>
    /// <returns>The screenshot file when executing locally or a dummy file when executing in AppCenter.</returns>
    FileInfo Screenshot(string title);

    /// <summary>
    /// Performs a left to right swipe gesture.
    /// </summary>
    /// <param name="swipePercentage">How far across the screen to swipe (from 0.0 to 1.0).</param> 
    /// <param name="swipeSpeed">The speed of the gesture.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia.</param>
    void SwipeLeftToRight(double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed, bool withInertia = true);

    /// <summary>
    /// Performs a left to right swipe gesture on the matching element. If multiple elements are matched, the 
    /// first one will be used.
    /// </summary>
    /// <param name="marked">
    /// Marked selector to match. See <see cref="AppQuery.Marked" /> for more information.
    /// </param>
    /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param> 
    /// <param name="swipeSpeed">The speed of the gesture.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia.</param>
    void SwipeLeftToRight(string marked, double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed, bool withInertia = true);

    /// <summary>
    /// Performs a right to left swipe gesture.
    /// </summary>
    /// <param name="swipePercentage">How far across the screen to swipe (from 0.0 to 1.0).</param> 
    /// <param name="swipeSpeed">The speed of the gesture.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia.</param>
    void SwipeRightToLeft(double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed, bool withInertia = true);

    /// <summary>
    /// Performs a right to left swipe gesture on the matching element. If multiple elements are matched, the 
    /// first one will be used.
    /// </summary>
    /// <param name="marked">
    /// Marked selector to match. See <see cref="AppQuery.Marked" /> for more information.
    /// </param>
    /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param> 
    /// <param name="swipeSpeed">The speed of the gesture.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia.</param>
    void SwipeRightToLeft(string marked, double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed, bool withInertia = true);

    /// <summary>
    /// Performs a left to right swipe gesture on an element matched by 'query'.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param> 
    /// <param name="swipeSpeed">The speed of the gesture.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia.</param>
    void SwipeLeftToRight(
        Func<AppQuery, AppQuery> query,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true);

    /// <summary>
    /// Performs a left to right swipe gesture on an element matched by 'query'.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param> 
    /// <param name="swipeSpeed">The speed of the gesture.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia.</param>
    void SwipeLeftToRight(
        Func<AppQuery, AppWebQuery> query,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true);

    /// <summary>
    /// Performs a right to left swipe gesture on an element matched by 'query'.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param> 
    /// <param name="swipeSpeed">The speed of the gesture.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia.</param>
    void SwipeRightToLeft(
        Func<AppQuery, AppQuery> query,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true);

    /// <summary>
    /// Performs a right to left swipe gesture on an element matched by 'query'.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param> 
    /// <param name="swipeSpeed">The speed of the gesture.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia.</param>
    void SwipeRightToLeft(
        Func<AppQuery, AppWebQuery> query,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true);

    /// <summary>
    /// Scrolls up on the first element matching query.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the elements.</param>
    /// <param name="strategy">Strategy for scrolling element</param>
    /// <param name="swipePercentage">
    /// How far across the element to swipe (from 0.0 to 1.0).  Ignored for programmatic scrolling.
    /// </param> 
    /// <param name="swipeSpeed">The speed of the gesture.  Ignored for programmatic scrolling.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia. Ignored for programmatic scrolling.</param>
    void ScrollUp(
        Func<AppQuery, AppQuery> query = null,
        ScrollStrategy strategy = ScrollStrategy.Auto,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true);

    /// <summary>
    /// Scrolls up on the first element matching query.
    /// </summary>
    /// <param name="withinMarked">
    /// Marked selector to select what element to scroll within. See <see cref="AppQuery.Marked" /> for more 
    /// information.
    /// </param>
    /// <param name="strategy">Strategy for scrolling element.</param>
    /// <param name="swipePercentage">
    /// How far across the element to swipe (from 0.0 to 1.0).  Ignored for programmatic scrolling.
    /// </param> 
    /// <param name="swipeSpeed">The speed of the gesture.  Ignored for programmatic scrolling.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia. Ignored for programmatic scrolling.</param>
    void ScrollUp(
        string withinMarked,
        ScrollStrategy strategy = ScrollStrategy.Auto,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true);

    /// <summary>
    /// Scrolls down on the first element matching query.
    /// </summary>
    /// <param name="withinQuery">
    /// Entry point for the fluent API to specify the what element to scroll within.
    /// </param>
    /// <param name="strategy">Strategy for scrolling element.</param>
    /// <param name="swipePercentage">
    /// How far across the element to swipe (from 0.0 to 1.0).  Ignored for programmatic scrolling.
    /// </param> 
    /// <param name="swipeSpeed">The speed of the gesture.  Ignored for programmatic scrolling.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia. Ignored for programmatic scrolling.</param>
    void ScrollDown(
        Func<AppQuery, AppQuery> withinQuery = null,
        ScrollStrategy strategy = ScrollStrategy.Auto,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true);

    /// <summary>
    /// Scrolls down on the first element matching query.
    /// </summary>
    /// <param name="withinMarked">
    /// Marked selector to select what element to scroll within. See <see cref="AppQuery.Marked" /> for more
    /// information.
    /// </param>
    /// <param name="strategy">Strategy for scrolling element.</param>
    /// <param name="swipePercentage">
    /// How far across the element to swipe (from 0.0 to 1.0).  Ignored for programmatic scrolling.
    /// </param> 
    /// <param name="swipeSpeed">The speed of the gesture.  Ignored for programmatic scrolling.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia. Ignored for programmatic scrolling.</param>
    void ScrollDown(
        string withinMarked,
        ScrollStrategy strategy = ScrollStrategy.Auto,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true);

    /// <summary>
    /// Scroll until an element that matches the <c>toMarked</c> is shown on the screen. 
    /// </summary>
    /// <param name="toMarked">
    /// Marked selector to select what element to bring on screen. See <see cref="AppQuery.Marked" /> for more
    /// information.
    /// </param>
    /// <param name="withinMarked">
    /// Marked selector to select what element to scroll within. See <see cref="AppQuery.Marked" /> for more 
    /// information.
    /// </param>
    /// <param name="strategy">Strategy for scrolling element.</param>
    /// <param name="swipePercentage">
    /// How far across the element to swipe (from 0.0 to 1.0).  Ignored for programmatic scrolling.
    /// </param> 
    /// <param name="swipeSpeed">The speed of the gesture.  Ignored for programmatic scrolling.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia. Ignored for programmatic scrolling.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    void ScrollTo(
        string toMarked,
        string withinMarked = null,
        ScrollStrategy strategy = ScrollStrategy.Auto,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true,
        TimeSpan? timeout = null);

    /// <summary>
    /// Scroll up until an element that matches the <c>toMarked</c> is shown on the screen. 
    /// </summary>
    /// <param name="toMarked">
    /// Marked selector to select what element to bring on screen. See <see cref="AppQuery.Marked" /> for more 
    /// information.
    /// </param>
    /// <param name="withinMarked">
    /// Marked selector to select what element to scroll within. See <see cref="AppQuery.Marked" /> for more 
    /// information.
    /// </param>
    /// <param name="strategy">Strategy for scrolling element.</param>
    /// <param name="swipePercentage">
    /// How far across the element to swipe (from 0.0 to 1.0).  Ignored for programmatic scrolling.
    /// </param> 
    /// <param name="swipeSpeed">The speed of the gesture.  Ignored for programmatic scrolling.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia. Ignored for programmatic scrolling.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    void ScrollUpTo(
        string toMarked,
        string withinMarked = null,
        ScrollStrategy strategy = ScrollStrategy.Auto,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true,
        TimeSpan? timeout = null);

    /// <summary>
    /// Scroll up until an element that matches the <c>toMarked</c> is shown on the screen. 
    /// </summary>
    /// <param name="toQuery">Entry point for the fluent API to specify the element to bring on screen.</param>
    /// <param name="withinMarked">
    /// Marked selector to select what element to scroll within. See <see cref="AppQuery.Marked" /> for more
    /// information.
    /// </param>
    /// <param name="strategy">Strategy for scrolling element.</param>
    /// <param name="swipePercentage">
    /// How far across the element to swipe (from 0.0 to 1.0).  Ignored for programmatic scrolling.
    /// </param> 
    /// <param name="swipeSpeed">The speed of the gesture.  Ignored for programmatic scrolling.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia. Ignored for programmatic scrolling.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    void ScrollUpTo(
        Func<AppQuery, AppWebQuery> toQuery,
        string withinMarked,
        ScrollStrategy strategy = ScrollStrategy.Auto,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true,
        TimeSpan? timeout = null);

    /// <summary>
    /// Scroll down until an element that matches the <c>toMarked</c> is shown on the screen. 
    /// </summary>
    /// <param name="toMarked">
    /// Marked selector to select what element to bring on screen. See <see cref="AppQuery.Marked" /> for more 
    /// information.
    /// </param>
    /// <param name="withinMarked">
    /// Marked selector to select what element to scroll within. See <see cref="AppQuery.Marked" /> for more 
    /// information.
    /// </param>
    /// <param name="strategy">Strategy for scrolling element.</param>
    /// <param name="swipePercentage">
    /// How far across the element to swipe (from 0.0 to 1.0).  Ignored for programmatic scrolling.
    /// </param> 
    /// <param name="swipeSpeed">The speed of the gesture.  Ignored for programmatic scrolling.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia. Ignored for programmatic scrolling.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    void ScrollDownTo(
        string toMarked,
        string withinMarked = null,
        ScrollStrategy strategy = ScrollStrategy.Auto,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true,
        TimeSpan? timeout = null);

    /// <summary>
    /// Scroll down until an element that matches the <c>toMarked</c> is shown on the screen. 
    /// </summary>
    /// <param name="toQuery">Entry point for the fluent API to specify the element to bring on screen.</param>
    /// <param name="withinMarked">
    /// Marked selector to select what element to scroll within. See <see cref="AppQuery.Marked" /> for more
    /// information.
    /// </param>
    /// <param name="strategy">Strategy for scrolling element.</param>
    /// <param name="swipePercentage">
    /// How far across the element to swipe (from 0.0 to 1.0).  Ignored for programmatic scrolling.
    /// </param> 
    /// <param name="swipeSpeed">The speed of the gesture.  Ignored for programmatic scrolling.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia. Ignored for programmatic scrolling.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    void ScrollDownTo(
        Func<AppQuery, AppWebQuery> toQuery,
        string withinMarked,
        ScrollStrategy strategy = ScrollStrategy.Auto,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true,
        TimeSpan? timeout = null);


    /// <summary>
    /// Scroll up until an element that matches the <c>toQuery</c> is shown on the screen. 
    /// </summary>
    /// <param name="toQuery">Entry point for the fluent API to specify the element to bring on screen.</param>
    /// <param name="withinQuery">Entry point for the fluent API to specify what element to scroll within.</param>
    /// <param name="strategy">Strategy for scrolling element.</param>
    /// <param name="swipePercentage">
    /// How far across the element to swipe (from 0.0 to 1.0).  Ignored for programmatic scrolling.
    /// </param> 
    /// <param name="swipeSpeed">The speed of the gesture.  Ignored for programmatic scrolling.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia. Ignored for programmatic scrolling.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    void ScrollUpTo(
        Func<AppQuery, AppQuery> toQuery,
        Func<AppQuery, AppQuery> withinQuery = null,
        ScrollStrategy strategy = ScrollStrategy.Auto,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true,
        TimeSpan? timeout = null);

    /// <summary>
    /// Scroll up until an element that matches the <c>toQuery</c> is shown on the screen. 
    /// </summary>
    /// <param name="toQuery">Entry point for the fluent API to specify the element to bring on screen.</param>
    /// <param name="withinQuery">Entry point for the fluent API to specify what element to scroll within.</param>
    /// <param name="strategy">Strategy for scrolling element.</param>
    /// <param name="swipePercentage">
    /// How far across the element to swipe (from 0.0 to 1.0).  Ignored for programmatic scrolling.
    /// </param> 
    /// <param name="swipeSpeed">The speed of the gesture.  Ignored for programmatic scrolling.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia. Ignored for programmatic scrolling.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    void ScrollUpTo(
        Func<AppQuery, AppWebQuery> toQuery,
        Func<AppQuery, AppQuery> withinQuery = null,
        ScrollStrategy strategy = ScrollStrategy.Auto,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true,
        TimeSpan? timeout = null);

    /// <summary>
    /// Scroll down until an element that matches the <c>toQuery</c> is shown on the screen. 
    /// </summary>
    /// <param name="toQuery">Entry point for the fluent API to specify the element to bring on screen.</param>
    /// <param name="withinQuery">Entry point for the fluent API to specify what element to scroll within.</param>
    /// <param name="strategy">Strategy for scrolling element.</param>
    /// <param name="swipePercentage">
    /// How far across the element to swipe (from 0.0 to 1.0).  Ignored for programmatic scrolling.
    /// </param> 
    /// <param name="swipeSpeed">The speed of the gesture.  Ignored for programmatic scrolling.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia. Ignored for programmatic scrolling.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    void ScrollDownTo(
        Func<AppQuery, AppQuery> toQuery,
        Func<AppQuery, AppQuery> withinQuery = null,
        ScrollStrategy strategy = ScrollStrategy.Auto,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true,
        TimeSpan? timeout = null);

    /// <summary>
    /// Scroll down until an element that matches the <c>toQuery</c> is shown on the screen. 
    /// </summary>
    /// <param name="toQuery">Entry point for the fluent API to specify the element to bring on screen.</param>
    /// <param name="withinQuery">Entry point for the fluent API to specify what element to scroll within.</param>
    /// <param name="strategy">Strategy for scrolling element.</param>
    /// <param name="swipePercentage">
    /// How far across the element to swipe (from 0.0 to 1.0).  Ignored for programmatic scrolling.
    /// </param> 
    /// <param name="swipeSpeed">The speed of the gesture.  Ignored for programmatic scrolling.</param> 
    /// <param name="withInertia">Whether swipes should cause inertia. Ignored for programmatic scrolling.</param>
    /// <param name="timeout">The <see cref="TimeSpan"/> to wait before failing.</param>
    void ScrollDownTo(
        Func<AppQuery, AppWebQuery> toQuery,
        Func<AppQuery, AppQuery> withinQuery = null,
        ScrollStrategy strategy = ScrollStrategy.Auto,
        double swipePercentage = UITestConstants.DefaultSwipePercentage,
        int swipeSpeed = UITestConstants.DefaultSwipeSpeed,
        bool withInertia = true,
        TimeSpan? timeout = null);

    /// <summary>
    /// Changes the device (iOS) or current activity (Android) orientation to portrait mode.
    /// </summary>
    void SetOrientationPortrait();

    /// <summary>
    /// Changes the device (iOS) or current activity (Android) orientation to landscape mode.
    /// </summary>
    void SetOrientationLandscape();

    /// <summary>
    /// Contains helper methods for outputting the result of queries instead of resorting to 
    /// <see cref="System.Console"/>.
    /// </summary>
    //AppPrintHelper Print { get; }

    /// <summary>
    /// Starts an interactive REPL (Read-Eval-Print-Loop) for app exploration and pauses test execution until it
    /// is closed.
    /// </summary>
    void Repl();

    /// <summary>
    /// Device information and Control
    /// </summary>
    IDevice Device { get; }

    /// <summary>
    /// Navigate back on the device. 
    /// </summary>
    void Back();

    /// <summary>
    /// Presses the volume up button on the device.
    /// </summary>
    void PressVolumeUp();

    /// <summary>
    /// Presses the volume down button on the device.
    /// </summary>
    void PressVolumeDown();

    /// <summary>
    /// Allows HTTP access to the test server running on the device.
    /// </summary>
    //ITestServer TestServer { get; }

    /// <summary>
    /// Invokes a method on the app's main activity for Android and app delegate for iOS. For Xamarin apps,
    /// methods must be exposed using attributes as shown below.
    /// 
    /// Android example in activity:
    /// 
    /// <code>
    /// [Export]
    /// public string MyInvokeMethod(string arg)
    /// {
    ///     return "uitest";
    /// }
    /// </code>
    /// 
    /// iOS example in app delegate:
    /// 
    /// <code>
    /// [Export("myInvokeMethod:")]
    /// public NSString MyInvokeMethod(NSString arg)
    /// {
    ///     return new NSString("uitest");
    /// }
    /// </code>
    /// 
    /// </summary>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="argument">The argument to pass to the method.</param>
    /// <returns>The result of the invocation.</returns>
    object Invoke(string methodName, object argument = null);

    /// <summary>
    /// Invokes a method on the app's main activity for Android and app delegate for iOS. For Xamarin apps,
    /// methods must be exposed using attributes as shown below.
    /// 
    /// Android example in activity:
    /// 
    /// <code>
    /// [Export]
    /// public string MyInvokeMethod(string arg, string arg2)
    /// {
    ///     return "uitest";
    /// }
    /// </code>
    /// 
    /// iOS example in app delegate:
    /// 
    /// <code>
    /// [Export("myInvokeMethod:")]
    /// public NSString MyInvokeMethod(NSString arg, NSString arg2)
    /// {
    ///     return new NSString("uitest");
    /// }
    /// </code>
    /// 
    /// </summary>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="arguments">The arguments to pass to the method.</param>
    /// <returns>The result of the invocation.</returns>
    object Invoke(string methodName, object[] arguments);

    /// <summary>
    /// Performs a continuous drag gesture between 2 points.
    /// </summary>
    /// <param name="fromX">The x coordinate to start dragging from.</param>
    /// <param name="fromY">The y coordinate to start dragging from.</param>
    /// <param name="toX">The x coordinate to drag to.</param>
    /// <param name="toY">The y coordinate to drag to.</param>
    void DragCoordinates(float fromX, float fromY, float toX, float toY);

    /// <summary>
    /// Drags the from element to the to element.
    /// </summary>
    /// <param name="from">Entry point for the fluent API to specify the from element.</param>
    /// <param name="to">Entry point for the fluent API to specify the to element.</param>
    void DragAndDrop(Func<AppQuery, AppQuery> from, Func<AppQuery, AppQuery> to);

    /// <summary>
    /// Drags the from element to the to element.
    /// </summary>
    /// <param name="from">Marked selector of the from element.</param>
    /// <param name="to">Marked selector of the to element.</param>
    void DragAndDrop(string from, string to);

    /// <summary>
    /// Sets the value of a slider element that matches <c>marked</c>.
    /// </summary>
    /// <param name="marked">Marked selector of the slider element to update.</param>
    /// <param name="value">The value to set the slider to.</param>
    void SetSliderValue(string marked, double value);

    /// <summary>
    /// Sets the value of a slider element that matches <c>query</c>.
    /// </summary>
    /// <param name="query">Entry point for the fluent API to specify the element.</param>
    /// <param name="value">The value to set the slider to.</param>
    void SetSliderValue(Func<AppQuery, AppQuery> query, double value);
}