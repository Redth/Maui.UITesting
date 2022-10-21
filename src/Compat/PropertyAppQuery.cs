namespace Xamarin.UITest.Queries;

/// <summary>
/// Fluent query API for specifying view elements predicates for properties.
/// </summary>
public class PropertyAppQuery
{
    readonly QueryPlatform _queryPlatform;
    readonly AppQuery _appQuery;
    readonly string _propertyName;

    /// <summary>
    /// Initial constructor for property queries. Should not be called directly, but used as part of the fluent API in the app classes.
    /// </summary>
    /// <param name="queryPlatform">The query target platform.</param>
    /// <param name="appQuery">The existing <see cref="AppQuery"/> to build on.</param>
    /// <param name="propertyName">The name of the property to filter on.</param>
    public PropertyAppQuery(QueryPlatform queryPlatform, AppQuery appQuery, string propertyName)
    {
        _queryPlatform = queryPlatform;
        _appQuery = appQuery;
        _propertyName = propertyName;
    }

    /// <summary>
    /// Matches properties starting with the given text.
    /// </summary>
    /// <param name="text">The text to match.</param>
    public AppQuery StartsWith(string text)
    {
        return null;
    }

    /// <summary>
    /// Matches properties ending with the given text.
    /// </summary>
    /// <param name="text">The text to match.</param>
    public AppQuery EndsWith(string text)
    {
        return null;
    }

    /// <summary>
    /// Matches properties containing with the given text.
    /// </summary>
    /// <param name="text">The text to match.</param>
    public AppQuery Contains(string text)
    {
        return null;
    }

    /// <summary>
    /// Matches properties like the given text. Supports * wildcards.
    /// </summary>
    /// <param name="text">The text to match.</param>
    public AppQuery Like(string text)
    {
        return null;
    }

    /// <summary>
    /// Matches properties matching the given predicate and text. Allows using unexposed Calabash predicates.
    /// </summary>
    /// <param name="predicate">The Calabash predicate to use.</param>
    /// <param name="text">The text to match.</param>
    public AppQuery Predicate(string predicate, string text)
    {
        return null;
    }

    /// <summary>
    /// Extracts the value of the property.
    /// </summary>
    /// <typeparam name="T">The expected result type of the property.</typeparam>
    //public AppTypedSelector<T> Value<T>()
    //{
    //    return new AppTypedSelector<T>(new AppQuery(_appQuery, new PropertyValueToken<T>(_propertyName)), new object[] { _propertyName }, true);
    //}

    /// <summary>
    /// The target platform of the query. Useful when writing extensions methods for queries for platform differences.
    /// </summary>
    public QueryPlatform QueryPlatform
    {
        get { return _queryPlatform; }
    }
}