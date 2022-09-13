using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Markup;

namespace Microsoft.Maui.Automation
{
	internal static class WindowsUIExtensions
	{
		public static IEnumerable<FrameworkElement> FindChildren(this FrameworkElement element, bool recursive = false)
		{
		Start:

			if (element is Panel panel)
			{
				foreach (UIElement child in panel.Children)
				{
					if (child is not FrameworkElement current)
						continue;

					yield return current;

					if (recursive)
					{
						foreach (FrameworkElement childOfChild in FindChildren(current))
							yield return childOfChild;
					}
				}
			}
			else if (element is ItemsControl itemsControl)
			{
				foreach (object item in itemsControl.Items)
				{
					if (item is not FrameworkElement current)
						continue;

					yield return current;

					if (recursive)
					{
						foreach (FrameworkElement childOfChild in FindChildren(current))
							yield return childOfChild;
					}
				}
			}
			else if (element is ContentControl contentControl)
			{
				if (contentControl.Content is FrameworkElement content)
				{
					yield return content;

					element = content;

					goto Start;
				}
			}
			else if (element is Microsoft.UI.Xaml.Controls.Border border)
			{
				if (border.Child is FrameworkElement child)
				{
					yield return child;

					element = child;

					goto Start;
				}
			}
			else if (element is Microsoft.UI.Xaml.Controls.ContentPresenter contentPresenter)
			{
				if (contentPresenter.Content is FrameworkElement content)
				{
					yield return content;

					element = content;

					goto Start;
				}
			}
			else if (element is Viewbox viewbox)
			{
				if (viewbox.Child is FrameworkElement child)
				{
					yield return child;

					element = child;

					goto Start;
				}
			}
			else if (element is UserControl userControl)
			{
				if (userControl.Content is FrameworkElement content)
				{
					yield return content;

					element = content;

					goto Start;
				}
			}
			else if (element.GetContentControl() is FrameworkElement containedControl)
			{
				yield return containedControl;

				element = containedControl;

				goto Start;
			}
		}

		public static UIElement? GetContentControl(this FrameworkElement element)
		{
			Type type = element.GetType();
			TypeInfo? typeInfo = type.GetTypeInfo();

			while (typeInfo is not null)
			{
				// We need to manually explore the custom attributes this way as the target one
				// is not returned by any of the other available GetCustomAttribute<T> APIs.
				foreach (CustomAttributeData attribute in typeInfo.CustomAttributes)
				{
					if (attribute.AttributeType == typeof(ContentPropertyAttribute))
					{
						string propertyName = (string)attribute.NamedArguments[0].TypedValue.Value;
						PropertyInfo? propertyInfo = type.GetProperty(propertyName);

						return propertyInfo?.GetValue(element) as UIElement;
					}
				}

				typeInfo = typeInfo.BaseType?.GetTypeInfo();
			}

			return null;
		}
	}
}
