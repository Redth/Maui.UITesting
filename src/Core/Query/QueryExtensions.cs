using Microsoft.Maui.Automation.Querying;

namespace Microsoft.Maui.Automation;

public static class QueryExtensions
{
	public static Query ThenBy(this Query query, Predicate<IElement> predicate)
		=> query.Append(predicate);

	public static Query ThenByAutomationId(this Query query, string automationId)
		=> query.Append(e => e.AutomationId == automationId);

	public static Query ThenById(this Query query, string id)
		=> query.Append(e => e.Id == id);

	public static Query ThenOfType(this Query query, string typeName)
		=> query.Append(e => e.Type == typeName);

	public static Query ThenOfFullType(this Query query, string fullTypeName)
		=> query.Append(e => e.FullType == fullTypeName);

	public static Query ThenContainingText(this Query query, string text, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
		=> query.Append(e => e.Text.Contains(text, comparisonType));

	public static Query ThenMarked(this Query query, string marked, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
		=> query.Append(e => e.Id.Equals(marked, comparisonType)
			|| e.AutomationId.Equals(marked, comparisonType)
			|| e.Text.Equals(marked, comparisonType));

	public static Query Siblings(this Query query, Predicate<IElement>? predicate = null)
		=> query.Append(new SiblingsQueryStep(predicate));

	public static Query AtIndex(this Query query, int index)
		=> query.Append(new IndexQueryStep(index));

	public static Query Tap(this Query query)
		=> query.Append(new InteractionQueryStep((driver, element) => driver.Tap(element)));

	public static Query LongPress(this Query query)
		=> query.Append(new InteractionQueryStep((driver, element) => driver.LongPress(element)));

	public static Query InputText(this Query query, string text)
		=> query.Append(new InteractionQueryStep((driver, element) => driver.InputText(element, text)));

	public static Query ClearText(this Query query, string text)
	=> query.Append(new InteractionQueryStep((driver, element) => driver.ClearText(element)));
}


public static class DriverQueryExtensions
{
	static DriverQuery AppendChildren(this DriverQuery query, Predicate<IElement> predicate)
	{
		query.Query.Append(predicate);
		return query;
	}

	static DriverQuery Append(this DriverQuery query, IQueryStep step)
	{
		query.Query.Append(step);
		return query;
	}


	public static DriverQuery By(this DriverQuery query, Predicate<IElement>? predicate = null)
		=> query.Append(new DescendantsQueryStep(predicate));


	public static DriverQuery AutomationId(this DriverQuery query, string automationId)
		=> query.By(e => e.AutomationId == automationId);

	public static DriverQuery Id(this DriverQuery query, string id)
		=> query.By(e => e.Id == id);

	public static DriverQuery Type(this DriverQuery query, string typeName)
		=> query.By(e => e.Type == typeName);

	public static DriverQuery FullType(this DriverQuery query, string fullTypeName)
		=> query.By(e => e.FullType == fullTypeName);

	public static DriverQuery Text(this DriverQuery query, string text, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
		=> query.By(e => e.Text.Equals(text, comparisonType));

    public static DriverQuery ContainsText(this DriverQuery query, string text, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        => query.By(e => e.Text.Contains(text, comparisonType));

    public static DriverQuery Marked(this DriverQuery query, string marked, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
		=> query.By(e => e.Id.Equals(marked, comparisonType)
			|| e.AutomationId.Equals(marked, comparisonType)
			|| e.Text.Equals(marked, comparisonType));

	public static DriverQuery First(this DriverQuery query)
		=> query.Append(new FirstQueryStep());


	public static DriverQuery Children(this DriverQuery query, Predicate<IElement> predicate)
		=> query.AppendChildren(predicate);

	public static DriverQuery ChildrenByAutomationId(this DriverQuery query, string automationId)
		=> query.AppendChildren(e => e.AutomationId == automationId);

	public static DriverQuery ChildrenById(this DriverQuery query, string id)
		=> query.AppendChildren(e => e.Id == id);

	public static DriverQuery ChildrenOfType(this DriverQuery query, string typeName)
		=> query.AppendChildren(e => e.Type == typeName);

	public static DriverQuery ChildrenOfFullType(this DriverQuery query, string fullTypeName)
		=> query.AppendChildren(e => e.FullType == fullTypeName);

	public static DriverQuery ChildrenContainingText(this DriverQuery query, string text, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
		=> query.AppendChildren(e => e.Text.Contains(text, comparisonType));

	public static DriverQuery ChildrenMarked(this DriverQuery query, string marked, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
		=> query.AppendChildren(e => e.Id.Equals(marked, comparisonType)
			|| e.AutomationId.Equals(marked, comparisonType)
			|| e.Text.Equals(marked, comparisonType));

	public static DriverQuery Siblings(this DriverQuery query, Predicate<IElement>? predicate = null)
		=> query.Append(new SiblingsQueryStep(predicate));

	public static DriverQuery Index(this DriverQuery query, int index)
		=> query.Append(new IndexQueryStep(index));

	public static DriverQuery Tap(this DriverQuery query)
		=> query.Append(new InteractionQueryStep((driver, element) => driver.Tap(element)));

	public static DriverQuery LongPress(this DriverQuery query)
		=> query.Append(new InteractionQueryStep((driver, element) => driver.LongPress(element)));

	public static DriverQuery InputText(this DriverQuery query, string text)
		=> query.Append(new InteractionQueryStep((driver, element) => driver.InputText(element, text)));

	public static DriverQuery ClearText(this DriverQuery query)
	=> query.Append(new InteractionQueryStep((driver, element) => driver.ClearText(element)));
}

public static class On
{
	public static Query Platform(Platform automationPlatform)
		=> Query.On(automationPlatform);
}

public static class By
{
	public static Query AutomationId(string automationId)
		=> Query.By(e => e.AutomationId == automationId);

	public static Query Id(string id)
		=> Query.By(e => e.Id == id);

	public static Query Type(string type)
		=> Query.By(e => e.Type == type);

	public static Query ContainingText(string text, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
		=> Query.By(e => e.Text.Contains(text, comparisonType));

	public static Query Marked(string marked, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
		=> Query.By(e => e.Id.Equals(marked, comparisonType)
			|| e.AutomationId.Equals(marked, comparisonType)
			|| e.Text.Equals(marked, comparisonType));
}

