using Microsoft.Maui.Automation.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.Maui.Automation;

public partial class Element : IElement
{
	public Element(IElement element)
	{
		this.Application = element.Application;
		this.AutomationId = element.AutomationId;
		this.Children.AddRange(element.Children.Select(c => new Element(c)));
		this.Density = element.Density;
		this.Enabled = element.Enabled;
		this.Focused = element.Focused;
		this.FullType = element.FullType;
		this.Id = element.Id;
		this.ParentId = element.ParentId;
		this.Platform = element.Platform;
		this.PlatformElement = element.PlatformElement;
		this.ScreenFrame = element.ScreenFrame;
		this.Text = element.Text;
		this.Type = element.Type;
		this.ViewFrame	= element.ViewFrame;
		this.Visible = element.Visible;
		this.WindowFrame = element.WindowFrame;
	}

	public Element(IApplication application, Platform platform, string id, object platformElement, string parentId = "")
		: base()
	{
		Platform = platform;
		Application = application;
		ParentId = parentId;
		Id = id;
		AutomationId = string.Empty;
		Type = platformElement.GetType().Name;
		FullType = platformElement.GetType().FullName ?? Type;

		Visible = false;
		Enabled = false;
		Focused = false;
		ViewFrame = new Frame();
		WindowFrame = new Frame();
		ScreenFrame = new Frame();
		PlatformElement = platformElement;
	}

	public IApplication? Application { get; set; }
	public object? PlatformElement { get; set; }

	IReadOnlyList<IElement> IElement.Children
		=> Children.ToList();
}
