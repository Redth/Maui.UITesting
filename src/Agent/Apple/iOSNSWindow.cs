#if IOS || MACCATALYST
using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Microsoft.Maui.Automation
{
	internal class UINSWindow
	{
		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend_stret")]
		public static extern CGRect CGRect_objc_msgSend(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend_stret")]
		public static extern CGRect CGRect_objc_msgSend_CGRect(IntPtr receiver, IntPtr selector, CGRect arg1);

		static NativeHandle? nsApplicationHandle;
		static NativeHandle NSApplicationHandle
			=> nsApplicationHandle ??= Class.GetHandle("NSApplication");

		static Selector? sharedApplicationSelector;
		static Selector SharedApplicationSelector
			=> sharedApplicationSelector ??= new Selector("sharedApplication");

		static Selector? windowsSelector;
		static Selector WindowsSelector
			=> windowsSelector ??= new Selector("windows");

		static Selector? uiWindowsSelector;
		static Selector UIWindowsSelector
			=> uiWindowsSelector ??= new Selector("uiWindows");

		internal static UINSWindow? From(UIWindow uiWindow)
		{
			var nsapp = Runtime.GetNSObject(NSApplicationHandle);
			if (nsapp is null)
				return null;

			var sharedApp = nsapp.PerformSelector(SharedApplicationSelector);
			var windows = sharedApp.PerformSelector(WindowsSelector) as NSArray;

			for (nuint i = 0; i < windows!.Count; i++)
			{
				var nswin = windows.GetItem<NSObject>(i);

				var uiwindows = nswin.PerformSelector(UIWindowsSelector) as NSArray;

				for (nuint j = 0; j < uiwindows!.Count; j++)
				{
					var uiwin = uiwindows.GetItem<UIWindow>(j);

					if (uiwin.Handle == uiWindow.Handle)
						return new UINSWindow(nswin);
				}
			}

			return null;
		}

		public UINSWindow(NSObject nsWindow)
		{
			NSWindow = nsWindow;
		}

		protected readonly NSObject NSWindow;

		Selector? frameSelector = null;
		Selector FrameSelector
			=> frameSelector ??= new Selector("frame");

		Selector? convertRectToScreenSelector = null;
		Selector ConvertRectToScreenSelector
			=> convertRectToScreenSelector ??= new Selector("convertRectToScreen:");

		public CGRect Frame
			=> CGRect_objc_msgSend(NSWindow.Handle.Handle, FrameSelector.Handle);

		public CGRect ConvertRectToScreen(CGRect rect)
			=> CGRect_objc_msgSend_CGRect(NSWindow.Handle.Handle, ConvertRectToScreenSelector.Handle, rect);
	}
}
#endif