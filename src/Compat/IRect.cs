namespace Xamarin.UITest.Queries;

/// <summary>
/// Representation of a view elements position and size.
/// </summary>
public interface IRect
{
    /// <summary>
    /// The width of the element.
    /// </summary>
    float Width { get; set; }

    /// <summary>
    /// The height of the element.
    /// </summary>
    float Height { get; set; }

    /// <summary>
    /// The X coordinate of the top left corner of the element.
    /// </summary>
    float X { get; set; }

    /// <summary>
    /// The Y coordinate of the top left corner of the element.
    /// </summary>
    float Y { get; set; }

    /// <summary>
    /// The X coordinate of the center of the element.
    /// </summary>
    float CenterX { get; set; }

    /// <summary>
    /// The Y coordinate of the center of the element.
    /// </summary>
    /// <value>The center y.</value>
    float CenterY { get; set; }
}