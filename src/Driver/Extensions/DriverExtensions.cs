using Microsoft.AspNetCore.Mvc;
using Microsoft.Maui.Automation.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.Maui.Automation.Driver;

public static partial class DriverExtensions
{
	public static DriverQuery On(this IDriver driver, Platform automationPlatform)
		=> new DriverQuery(driver, automationPlatform);

	public static DriverQuery Query(this IDriver driver)
		=> new DriverQuery(driver);

	public static DriverQuery Query(this IDriver driver, Platform automationPlatform)
		=> new DriverQuery(driver, automationPlatform);


	public static DriverQuery ChildrenBy(this IDriver driver, Predicate<IElement> predicate)
		=> new DriverQuery(driver).Children(predicate);

	public static DriverQuery ChildrenByAutomationId(this IDriver driver, string automationId)
		=> new DriverQuery(driver).ChildrenByAutomationId(automationId);

	public static DriverQuery ChildrenById(this IDriver driver, string id)
		=> new DriverQuery(driver).ChildrenById(id);

	public static DriverQuery ChildrenOfType(this IDriver driver, string typeName)
		=> new DriverQuery(driver).ChildrenOfType(typeName);

	public static DriverQuery ChildrenOfFullType(this IDriver driver, string fullTypeName)
		=> new DriverQuery(driver).ChildrenOfFullType(fullTypeName);

	public static DriverQuery ChildrenContainingText(this IDriver driver, string text, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
		=> new DriverQuery(driver).ChildrenContainingText(text, comparisonType);

	public static DriverQuery ChildrenMarked(this IDriver driver, string marked, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
		=> new DriverQuery(driver).ChildrenMarked(marked, comparisonType);


	public static DriverQuery By(this IDriver driver, Predicate<IElement>? predicate = null)
		=> new DriverQuery(driver).By(predicate);

    public static DriverQuery AutomationId(this IDriver driver, string automationId)
        => new DriverQuery(driver).AutomationId(automationId);

    public static DriverQuery Id(this IDriver driver, string id)
        => new DriverQuery(driver).Id(id);

    public static DriverQuery Type(this IDriver driver, string typeName)
        => new DriverQuery(driver).Type(typeName);

    public static DriverQuery FullType(this IDriver driver, string fullTypeName)
        => new DriverQuery(driver).FullType(fullTypeName);

    public static DriverQuery Text(this IDriver driver, string text, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        => new DriverQuery(driver).Text(text, comparisonType);

    public static DriverQuery ContainsText(this IDriver driver, string text, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        => new DriverQuery(driver).ContainsText(text, comparisonType);

    public static DriverQuery Marked(this IDriver driver, string marked, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        => new DriverQuery(driver).Marked(marked, comparisonType);
}
