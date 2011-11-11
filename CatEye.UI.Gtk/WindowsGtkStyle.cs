using System;
using System.Runtime.InteropServices;
using System.Security;
using Gtk;
using Gdk;

namespace CatEye.UI.Gtk
{
	public class WindowsGtkStyle
	{
        [SuppressUnmanagedCodeSecurity, DllImport("libgdk-win32-2.0-0.dll")]
        private static extern IntPtr gdk_win32_drawable_get_handle(IntPtr d);

		private static string ColorToHex(System.Drawing.Color c)
		{
			return "#" + c.R.ToString("x2") + c.G.ToString("x2") + c.B.ToString("x2");
		}
		public WindowsGtkStyle(string gtkrc_filename)
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
				Environment.OSVersion.Platform == PlatformID.Win32S ||
				Environment.OSVersion.Platform == PlatformID.Win32Windows ||
				Environment.OSVersion.Platform == PlatformID.WinCE)
			{
				Rc.AddDefaultFile(gtkrc_filename);
				/*Microsoft.Win32.SystemEvents.DisplaySettingsChanged += delegate {
					UpdateStyle();
				};*/
			}
		}
		public void UpdateStyle(Gdk.Window win, MenuBar bar)
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
				Environment.OSVersion.Platform == PlatformID.Win32S ||
				Environment.OSVersion.Platform == PlatformID.Win32Windows ||
				Environment.OSVersion.Platform == PlatformID.WinCE)
			{
				Settings.Default.SetStringProperty("gtk-color-scheme", 
				      "fg_color:" + ColorToHex(System.Drawing.SystemColors.ControlText) + "\n" +
				      "bg_color:" + ColorToHex(System.Drawing.SystemColors.Control) + "\n" + 
				      "base_color:" + ColorToHex(System.Drawing.SystemColors.Window) + "\n" + 
				      "text_color:" + ColorToHex(System.Drawing.SystemColors.ControlText) + "\n" + 
				      "selected_bg_color:" + ColorToHex(System.Drawing.SystemColors.Highlight) + "\n" + 
				      "selected_fg_color:" + ColorToHex(System.Drawing.SystemColors.HighlightText), null);

				// TODO: Implement Glass skins for Vista and Win7
				if (false && Environment.OSVersion.Version.Major >= 6)
				{
					// This is Vista or later. We can use glass styling
					IntPtr hwnd = gdk_win32_drawable_get_handle(win.Handle);
			
					int mw, mh;
					bar.GdkWindow.GetSize(out mw, out mh);
					
					DwmApi.MARGINS m = new DwmApi.MARGINS(0, mh + 1 , 0, 0);
					DwmApi.DwmExtendFrameIntoClientArea(hwnd, m);
					
					DwmApi.DWM_BLURBEHIND bb = new DwmApi.DWM_BLURBEHIND();
					bb.fEnable = true;
					bb.dwFlags = DwmApi.DWM_BLURBEHIND.DWM_BB_ENABLE | DwmApi.DWM_BLURBEHIND.DWM_BB_BLURREGION;
					bb.hRegionBlur = DwmApi.CreateRectRgn(30, 30, 150, 300);
					
					
					DwmApi.DwmEnableBlurBehindWindow(hwnd, bb);
				}
			}
		}
	}
}

