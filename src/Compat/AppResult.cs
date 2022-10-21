//using Xamarin.UITest.Queries.PlatformSpecific;

using Microsoft.Maui.Automation;

namespace Xamarin.UITest.Queries;

/// <summary>
/// Representation of a view element in the app.
/// </summary>
public class AppResult
{
    public static AppResult FromElement(IElement element)
        => new AppResult
        {
            Id = element.Id,
            Description = element.Text,
            Rect = AppRect.FromFrame(element.WindowFrame),
            Enabled = element.Enabled,
            Label = element.AutomationId,
            Class = element.Type,
            Text = element.Text,
        };

    /// <summary>
    /// Initializes a new instance of the <see cref="AppResult"/> class.
    /// </summary>
    public AppResult()
    {
    }


    /// <summary>
    /// The identifier of the view element.
    /// For Android: The id of the element.
    /// For iOS: The accessibilityIdentifier of the element.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// A platform specific text representation of the view element.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The <see cref="AppRect"/> rectangle representing the elements position and size. 
    /// </summary>
    public AppRect Rect { get; set; }

    /// <summary>
    /// The label of the view element.
    /// For Android: The contentDescription of the element.
    /// For iOS: The accessibilityLabel of the element.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// The text of the view element.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// The class of the view element.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Whether the element is enabled or not.
    /// </summary>
    public bool Enabled { get; set; }
}