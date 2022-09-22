using Android.App;
using Android.Views;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
	public static class AndroidExtensions
	{
		public static IReadOnlyCollection<Element> GetChildren(this Android.Views.View nativeView, IApplication application, string parentId = "", int currentDepth = -1, int maxDepth = -1)
		{
			var c = new List<Element>();

			if (nativeView is ViewGroup vg)
			{
				for (int i = 0; i < vg.ChildCount; i++)
					c.Add(vg.GetChildAt(i).GetElement(application, parentId, currentDepth, maxDepth));
			}

			return new ReadOnlyCollection<Element>(c.ToList());
		}

		public static IReadOnlyCollection<Element> GetChildren(this Android.App.Activity activity, IApplication application, string parentId = "", int currentDepth = -1, int maxDepth = -1)
		{
			var rootView = activity.Window?.DecorView?.RootView ??
					activity.FindViewById(Android.Resource.Id.Content)?.RootView ??
					activity.Window?.DecorView?.FindViewById(Android.Resource.Id.Content);

			if (rootView is not null)
				return new ReadOnlyCollection<Element>(
					new List<Element> { rootView.GetElement(application, parentId, currentDepth, maxDepth) });

			return new ReadOnlyCollection<Element>(new List<Element>());
		}

		public static string GetText(this Android.Views.View view)
		{
			if (view is Android.Widget.TextView tv)
				return tv.Text;

			return string.Empty;
		}

		internal static string EnsureUniqueId(this Android.Views.View view)
		{
			var id = view.GetTag(Agent.Resource.Id.maui_automation_unique_identifier)?.ToString();

			if (string.IsNullOrEmpty(id))
			{
				id = Guid.NewGuid().ToString();
				view.SetTag(Agent.Resource.Id.maui_automation_unique_identifier, id);
			}

			return id;
		}

		public static string GetAutomationId(this Android.App.Activity activity)
			=> activity.GetRootView().GetAutomationId();

		public static string GetAutomationId(this View view)
		{
			var id = string.Empty;

			if (string.IsNullOrEmpty(id))
				id = view.ContentDescription;

			if (string.IsNullOrEmpty(id) && view.Tag is Java.Lang.String str)
				id = str.ToString();

			if (string.IsNullOrEmpty(id))
				id = view.EnsureUniqueId();

			return id;
		}

		static Android.Views.View GetRootView(this Android.App.Activity activity)
			=> activity.Window?.DecorView?.RootView ??
				activity.FindViewById(Android.Resource.Id.Content)?.RootView ??
					activity.Window?.DecorView?.FindViewById(Android.Resource.Id.Content);

		public static string GetWindowId(this Android.App.Activity activity)
		{
			var rootView = activity.GetRootView();

			return rootView.EnsureUniqueId();
		}


		public static Element GetElement(this Android.Views.View androidview, IApplication application, string parentId = "", int currentDepth = -1, int maxDepth = -1)
		{
			var frames = androidview.GetFrames();

			var e = new Element(application, Platform.Android, androidview.EnsureUniqueId(), androidview, parentId)
			{
				AutomationId = androidview.GetAutomationId(),
				Enabled = androidview.Enabled,
				Visible = androidview.Visibility == ViewStates.Visible,
				Focused = androidview.Selected,
				ViewFrame = frames.viewFrame,
				WindowFrame = frames.windowFrame,
				ScreenFrame = frames.screenFrame,
				Density = androidview.Resources?.DisplayMetrics?.Density ?? 1.0,
				Text = androidview.GetText(),
			};

			var windowLoc = new int[2];
			androidview?.GetLocationInWindow(windowLoc);
			if (windowLoc != null && windowLoc.Length >= 2)
			{
				e.WindowFrame.X = windowLoc[0];
				e.WindowFrame.Y = windowLoc[1];
			}

			var screenLoc = new int[2];
			androidview?.GetLocationOnScreen(screenLoc);
			if (screenLoc != null && screenLoc.Length >= 2)
			{
				e.ScreenFrame.X = screenLoc[0];
				e.ScreenFrame.Y = screenLoc[1];
			}

			if (maxDepth <= 0 || (currentDepth + 1 <= maxDepth))
				e.Children.AddRange(androidview.GetChildren(e.Application, e.Id, currentDepth + 1, maxDepth));
			return e;
		}

		public static (Frame viewFrame, Frame windowFrame, Frame screenFrame) GetFrames(this Android.Views.View view)
		{
			if (view is null)
				return (new Frame(), new Frame(), new Frame());

			var viewFrame = new Frame {
				X = (int)view.GetX(),
				Y = (int)view.GetY(),
				Width = view.MeasuredWidth,
				Height = view.MeasuredHeight
			};
			var windowFrame = new Frame { Width = view.MeasuredWidth, Height = view.MeasuredHeight };
			var screenFrame = new Frame { Width = view.MeasuredWidth, Height = view.MeasuredHeight };

			var windowLoc = new int[2];
			view?.GetLocationInWindow(windowLoc);
			if (windowLoc != null && windowLoc.Length >= 2)
			{
				windowFrame.X = windowLoc[0];
				windowFrame.Y = windowLoc[1];
			}

			var screenLoc = new int[2];
			view?.GetLocationOnScreen(screenLoc);
			if (screenLoc != null && screenLoc.Length >= 2)
			{
				screenFrame.X = screenLoc[0];
				screenFrame.Y = screenLoc[1];
			}

			return (viewFrame, windowFrame, screenFrame);
		}

		public static Element GetElement(this Activity activity, IApplication application, int currentDepth = -1, int maxDepth = -1)
		{
			var frames = activity.GetRootView()?.GetFrames() ?? (new Frame(), new Frame(), new Frame());

			var e = new Element(application, Platform.Android, activity.GetWindowId(), activity)
			{
				AutomationId = activity.GetAutomationId(),
				ViewFrame = frames.viewFrame,
				WindowFrame = frames.windowFrame,
				ScreenFrame = frames.screenFrame,
				Density = activity.Resources?.DisplayMetrics?.Density ?? 1.0,
				Text = activity.Title
			};

			if (application is AndroidApplication androidApp)
			{
				var isCurrent = androidApp.IsActivityCurrent(activity);
				e.Visible = isCurrent;
				e.Enabled = isCurrent;
				e.Focused = isCurrent;
			}

			if (maxDepth <= 0 || (currentDepth + 1 <= maxDepth))
				e.Children.AddRange(activity.GetChildren(e.Application, e.Id, currentDepth + 1, maxDepth));
			return e;
		}


		public static async Task<PerformActionResult> PerformAction(this Android.Views.View view, string action, string elementId, params string[] arguments)
		{
			if (action == Actions.Tap)
			{
				await view.PostAsync(() => view.PerformClick());
				return PerformActionResult.Ok();
			}

			throw new NotSupportedException($"PerformAction {action} is not supported.");
		}

		public static Task PostAsync(this Android.Views.View view, Action action)
		{
			var r = new AsyncThreadRunner(() => view.PerformClick());
			view.Post(r);
			return r.WaitAsync();
		}
	}

	class AsyncThreadRunner : Java.Lang.Object, IRunnable
	{
		public AsyncThreadRunner(Action action)
		{
			runner = action;
			tcsRunner = new();
		}

		readonly Action runner;

		readonly TaskCompletionSource<bool> tcsRunner;

		public Task WaitAsync()
			=> tcsRunner.Task;

		public void Run()
		{
			runner?.Invoke();
			tcsRunner.TrySetResult(true);
		}
	}
}
