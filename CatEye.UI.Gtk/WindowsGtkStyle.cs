using System;
using Gtk;

namespace CatEye.UI.Gtk
{
	public class WindowsGtkStyle
	{
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
				Microsoft.Win32.SystemEvents.DisplaySettingsChanged += delegate {
					UpdateStyle();
				};
			}
		}
		public void UpdateStyle()
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
			}
		}
	}
}

