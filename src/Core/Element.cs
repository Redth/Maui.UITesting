using Microsoft.Maui.Automation.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.Maui.Automation;

public partial class Element
{
	public Element(IApplication application, Platform platform, string id, object platformElement, string parentId = "")
		: base()
	{
		Application = application;
		ParentId = parentId;
		Id = id;
		AutomationId = string.Empty;
		Type = platformElement.GetType().Name;
		FullType = platformElement.GetType().FullName ?? Type;

		Visible = false;
		Enabled = false;
		Focused = false;
		X = -1;
		Y = -1;
		Platform = platform;
		Width = -1;
		Height = -1;
		PlatformElement = platformElement;
	}

	public IApplication? Application { get; set; }
	public object? PlatformElement { get; set; }

	public IDriver? Driver { get; set; }
}
