#if IOS || MACCATALYST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Microsoft.Maui.Automation
{
	internal static class iOSExtensions
	{
		static string[] possibleTextPropertyNames = new string[]
		{
			"Title", "Text",
		};

		internal static string GetText(this UIView view)
			=> view switch
			{
				IUITextInput ti => TextFromUIInput(ti),
				UIButton b => b.CurrentTitle,
				_ => TextViaReflection(view, possibleTextPropertyNames)
			};

		static string TextViaReflection(UIView view, string[] propertyNames)
		{
			foreach (var name in propertyNames)
			{
				var prop = view.GetType().GetProperty("Text", typeof(string));
				if (prop is null)
					continue;
				if (!prop.CanRead)
					continue;
				if (prop.PropertyType != typeof(string))
					continue;
				return prop.GetValue(view) as string ?? "";
			}
			return "";
		}

		static string TextFromUIInput(IUITextInput ti)
		{
			var start = ti.BeginningOfDocument;
			var end = ti.EndOfDocument;
			var range = ti.GetTextRange(start, end);
			return ti.TextInRange(range);
		}
	}
}
#endif
