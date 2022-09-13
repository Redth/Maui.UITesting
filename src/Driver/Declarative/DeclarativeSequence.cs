using System;
using YamlDotNet.Serialization;

namespace Microsoft.Maui.Automation.Driver.Declarative;

public class DeclarativeSequence
{
    [YamlMember(Alias = "config")]
    public AutomationConfiguration Config { get; set; } = new();

    [YamlMember(Alias = "steps")]
    public List<Step> Steps { get; set; } = new ();

    public string Serialize()
    {
        var s = new SerializerBuilder()
            .WithTagMapping("assertVisible", typeof(VisibleAssertionStep))
            .WithTagMapping("assertNotVisible", typeof(NotVisibleAssertionStep))
            .WithTagMapping("tap", typeof(TapStep))
            .Build();

        return s.Serialize(this);
    }
}


public abstract class Step
{
    public abstract void Run(IDriver driver);
}

public class VisibleAssertionStep : VisibilityAssertionStep
{
    public override void Run(IDriver driver)
        => base.Run(driver, true);
}

public class NotVisibleAssertionStep : VisibilityAssertionStep
{
    public override void Run(IDriver driver)
        => base.Run(driver, false);
}


public abstract class VisibilityAssertionStep : AssertionStep
{
    [YamlMember(Alias = "id", Order = 1, DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Id { get; set; } = null;

    [YamlMember(Alias = "automationId", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? AutomationId { get; set; } = null;

    [YamlMember(Alias = "text", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Text { get; set; } = null;

    
    [YamlMember(Alias = "regex", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public bool Regex { get; set; }

    [YamlMember(Alias = "index", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public int Index { get; set; }

    [YamlMember(Alias = "optional", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public bool Optional { get; set; }

    protected void Run(IDriver driver, bool isVisible)
    {

    }
}


public abstract class AssertionStep : Step
{
	
}

public abstract class ActionStep : Step
{
}


public class TapStep : ActionStep
{
    [YamlMember(Alias = "id", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Id { get; set; } = null;

    [YamlMember(Alias = "automationId", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? AutomationId { get; set; } = null;

    [YamlMember(Alias = "text", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Text { get; set; } = null;


    [YamlMember(Alias = "regex", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public bool Regex { get; set; }

    [YamlMember(Alias = "index", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public int Index { get; set; }

    [YamlMember(Alias = "optional", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public bool Optional { get; set; }

    public override void Run(IDriver driver)
    {
        
    }
}

