using System;
using System.Linq;
using System.Text;

namespace Xamarin.UITest.Queries;

/// <summary>
/// Helper interface for exposing property from the fluent query API without cluttering the fluent API itself (when using explicit interface implementation). 
/// </summary>
interface IInvokeJSAppQuery
{
    string Javascript { get; }
    AppQuery AppQuery { get; }
}

/// <summary>
/// Fluent query API for invoking javascipt on Webviews.
/// </summary>
public class InvokeJSAppQuery : IInvokeJSAppQuery
{
    private readonly AppQuery _appQuery;
    private readonly string _javascript;

    /// <summary>
    /// Initial constructor. Should not be called directly, but used as part of the fluent API in the app classes.
    /// </summary>
    public InvokeJSAppQuery(AppQuery appQuery, string javascript)
    {
        _javascript = javascript;
        _appQuery = appQuery;
    }

    string IInvokeJSAppQuery.Javascript
    {
        get { return _javascript; }
    }

    AppQuery IInvokeJSAppQuery.AppQuery
    {
        get { return _appQuery; }
    }
}