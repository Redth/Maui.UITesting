namespace Xamarin.UITest.Queries;

/// <summary>
/// Representation of a web element in the app.
/// </summary>
public class AppWebResult
{
    /// <summary>
    /// The HTML id of the element.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The node type of the element.
    /// </summary>
    public string NodeType { get; set; }

    /// <summary>
    /// The tag name of the element.
    /// </summary>
    public string NodeName { get; set; }

    /// <summary>
    /// The CSS class of the element.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// The raw HTML of the element.
    /// </summary>
    public string Html { get; set; }

    /// <summary>
    /// The form value of the element.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// A platform specific text representation of the containing web view.
    /// </summary>
    public string WebView { get; set; }

    /// <summary>
    /// The text content of the element.
    /// </summary>
    public string TextContent { get; set; }

    /// <summary>
    /// The <see cref="AppWebRect"/> rectangle representing the elements position and size. 
    /// </summary>
    public AppWebRect Rect { get; set; }
}