using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Xamarin.UITest.Queries;

/// <summary>
/// Fluent query API for specifying view elements to match for queries and gestures.
/// </summary>
public class AppQuery
{
    readonly QueryPlatform _queryPlatform;

    /// <summary>
    /// Initial constructor for queries. Should not be called directly, but used as part of the fluent API in the app classes.
    /// </summary>
    /// <param name="queryPlatform">The query target platform.</param>
    public AppQuery(QueryPlatform queryPlatform)
    {
        _queryPlatform = queryPlatform;
        
    }


    /// <summary>
    /// Matches a button. 
    /// For Android: An element that has class (or inherits from) <c>android.widget.Button</c>. 
    /// For iOS: An element with class <c>UIButton</c>.
    /// </summary>
    /// <param name="marked">Optional argument for matching using marked classification. See <see cref="Marked"/> for more.</param>
    public AppQuery Button(string marked = null)
    {
        return null;
    }

    /// <summary>
    /// Matches a TextField. 
    /// For Android: An element that has class (or inherits from) <c>android.widget.EditText</c>. 
    /// For iOS: An element with class <c>UITextField</c>.
    /// </summary>
    /// <param name="marked">Optional argument for matching using marked classification. See <see cref="Marked"/> for more.</param>
    public AppQuery TextField(string marked = null)
    {
        return null;
    }

    /// <summary>
    /// Matches a Switch. 
    /// For Android: An element that inherits from <c>android.widget.CompoundButton</c>. 
    /// For iOS: An element with class <c>UISwitch</c>.
    /// </summary>
    /// <param name="marked">Optional argument for matching using marked classification. See <see cref="Marked"/> for more.</param>
    public AppQuery Switch(string marked = null)
    {
        return null;
    }

    /// <summary>
    /// Matches element class.
    /// For Android (no '.' in className): An element that has a class name of the given value (case insensitive).
    /// For Android ('.'s in className): An element which has a class (or super class) fully qualified name that matches the value.
    /// For iOS (first char lowercase): An element that has the class (or super class) name of the given value prepended with "UI". Example: <c>button</c> becomes <c>UIButton</c>.
    /// For iOS (first char uppercase): An element that has the class (or super class) name of the given value.
    /// </summary>
    /// <param name="className">The class name to match.</param>
    public AppQuery Class(string className)
    {
        return null;
    }

    /// <summary>
    /// Matches element class.
    /// For Android (no '.' in className): An element that has a class name of the given value (case insensitive).
    /// For Android ('.'s in className): An element which has a class (or super class) fully qualified name that matches the value.
    /// For iOS: An element that has the class (or super class) name of the given value.
    /// </summary>
    /// <param name="className">The class name to match.</param>
    public AppQuery ClassFull(string className)
    {
        return null;
    }

    /// <summary>
    /// Matches common values. 
    /// For Android: An element with the given value as either <c>id</c>, <c>contentDescription</c> or <c>text</c>.
    /// For iOS: An element with the given value as either <c>accessibilityLabel</c> or <c>accessibilityIdentifier</c>.
    /// </summary>
    /// <param name="text">The value to match.</param>
    public AppQuery Marked(string text)
    {
        return null;
    }

    /// <summary>
    /// Matches element id. 
    /// For Android: An element with the given value as <c>id</c>.
    /// For iOS: An element with the given value as <c>accessibilityIdentifier</c>.
    /// </summary>
    /// <param name="id">The value to match.</param>
    public AppQuery Id(string id)
    {
        return null;
    }

    /// <summary>
    /// Matches element id. 
    /// For Android: An element with the given value as <c>id</c>.  Allows properties of 
    /// an Android App project's `Resource.Id` to be used in `Id()` queries.
    /// For iOS: An element with the string version of the given value as
    /// <c>accessibilityIdentifier</c>.
    /// </summary>
    /// <param name="id">The value to match.</param>
    public AppQuery Id(int id)
    {
        return null;
    }

    /// <summary>
    /// Changes the query to return sibling elements of the currently matched ones.
    /// </summary>
    /// <param name="className">Optional class name of elements to match.</param>
    public AppQuery Sibling(string className = null)
    {
        return null;
    }

    /// <summary>
    /// Changes the query to return the n'th sibling element of the currently matched ones.
    /// </summary>
    /// <param name="index">The zero-based index of the sibling to return.</param>
    /// <returns></returns>
    public AppQuery Sibling(int index)
    {
        return null;
    }

    /// <summary>
    /// Changes the query to return descendant elements of the currently matched ones.
    /// </summary>
    /// <param name="className">Optional class name of elements to match.</param>
    public AppQuery Descendant(string className = null)
    {
        return null;
    }

    /// <summary>
    /// Changes the query to return the n'th descendant element of the currently matched ones.
    /// </summary>
    /// <param name="index">The zero-based index of the descendant to return.</param>
    /// <returns></returns>
    public AppQuery Descendant(int index)
    {
        return null;
    }

    /// <summary>
    /// Changes the query to return parent elements of the currently matched ones.
    /// </summary>
    /// <param name="className">Optional class name of elements to match.</param>
    public AppQuery Parent(string className = null)
    {
        return null;
    }

    /// <summary>
    /// Changes the query to return the n'th parent element of the currently matched ones.
    /// </summary>
    /// <param name="index">The zero-based index of the parent to return.</param>
    /// <returns></returns>
    public AppQuery Parent(int index)
    {
        return null;
    }

    /// <summary>
    /// Changes the query to return child elements of the currently matched ones.
    /// </summary>
    /// <param name="className">Optional class name of elements to match.</param>
    public AppQuery Child(string className = null)
    {
        return null;
    }

    /// <summary>
    /// Changes the query to return the n'th child element of the currently matched ones.
    /// </summary>
    /// <param name="index">The zero-based index of the child to return.</param>
    /// <returns></returns>
    public AppQuery Child(int index)
    {
        return null;
    }

    /// <summary>
    /// Changes the query to return all elements instead of just the visible ones.
    /// </summary>
    /// <param name="className">Optional class name of elements to match.</param>
    public AppQuery All(string className = null)
    {
        return null;
    }

    /// <summary>
    /// Matches element text. 
    /// </summary>
    /// <param name="text">The value to match.</param>
    public AppQuery Text(string text)
    {
        return null;
    }

    /// <summary>
    /// Matches the nth element of the currently matched elements.
    /// </summary>
    /// <param name="index">The zero-based index of the element to match.</param>
    public AppQuery Index(int index)
    {
        return null;
    }

    /// <summary>
    /// Matches WebViews
    /// </summary>
    public AppQuery WebView()
    {
        return null;
    }

    /// <summary>
    /// Matches the nth WebView
    /// </summary>
    /// <param name="index">The zero-based index of the webview to return.</param>
    public AppQuery WebView(int index)
    {
        return null;
    }

    /// <summary>
    /// Invokes javascript on the view elements matched by the query. If view elements other than WebViews are encountered, the execution will halt and an Exception will be thrown.
    /// </summary>
    /// <param name="javascript">The javascript to invoke on the views</param>
    /// <returns></returns>
    public InvokeJSAppQuery InvokeJS(string javascript)
    {
        return new InvokeJSAppQuery(this, javascript);
    }

    /// <summary>
    /// Matches elements in web views matching the given css selector. Must be used on web view elements. If used alone, will default to <c>android.webkit.WebView</c> for Android and <c>UIWebView</c> for iOS.
    /// </summary>
    /// <param name="cssSelector">The css selector to match.</param>
    public AppWebQuery Css(string cssSelector)
    {
        return null;
    }

    /// <summary>
    /// Matches a Frame/IFrame, allowing subsequent Css queries to execute within that frame. Must be used on web view elements. 
    /// If used alone, will default to <c>android.webkit.WebView</c> for Android and <c>UIWebView</c> for iOS.
    /// </summary>
    /// <param name="cssSelector">The css selector to match. Should refer to an html Frame/IFrame</param>
    public AppQuery Frame(string cssSelector)
    {
        return null;
    }

    /// <summary>
    /// Matches elements in web views matching the given XPath selector. Must be used on web view elements. If used alone, will default to <c>android.webkit.WebView</c> for Android and <c>UIWebView</c> for iOS.
    /// </summary>
    /// <param name="xPathSelector">The css selector to match.</param>
    public AppWebQuery XPath(string xPathSelector)
    {
        return null;
    }

    /// <summary>
    /// Matches a property or getter method value on the element. 
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="value">The value to match.</param>
    public AppQuery Property(string propertyName, string value)
    {
        return null;
    }

    /// <summary>
    /// Matches a property or getter method value on the element. 
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="value">The value to match.</param>
    public AppQuery Property(string propertyName, int value)
    {
        return null;
    }

    /// <summary>
    /// Matches a property or getter method value on the element. 
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="value">The value to match.</param>
    public AppQuery Property(string propertyName, bool value)
    {
        return null;
    }

    /// <summary>
    /// Allows for further filtering on a given property value.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    public PropertyAppQuery Property(string propertyName)
    {
        return null;
    }

    /// <summary>
    /// The target platform of the query. Useful when writing extensions methods for queries for platform differences.
    /// </summary>
    public QueryPlatform QueryPlatform
    {
        get { return _queryPlatform; }
    }

    /// <summary>
    /// Converts the string into it's Calabash query equivalent.
    /// </summary>
    public override string ToString()
    {
        return null;
    }
}