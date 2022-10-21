using System;
using System.Collections.Generic;
using System.Linq;
//using Xamarin.UITest.Queries.Tokens;
//using Xamarin.UITest.Utils;

namespace Xamarin.UITest.Queries;

/// <summary>
/// Fluent query API for specifying view elements predicates for web elements.
/// </summary>
public class AppWebQuery
{
    readonly QueryPlatform _queryPlatform;
    
    /// <summary>
    /// The target platform of the query. Useful when writing extensions methods for queries for platform differences.
    /// </summary>
    public QueryPlatform Platform
    {
        get { return _queryPlatform; }
    }

    /// <summary>
    /// Matches the nth element of the currently matched elements.
    /// </summary>
    /// <param name="index">The zero-based index of the element to match.</param>
    public AppWebQuery Index(int index)
    {
        return null;
    }
}