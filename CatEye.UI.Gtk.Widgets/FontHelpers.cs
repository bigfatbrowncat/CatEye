using System;
using Gtk;
using Pango;

namespace CatEye.UI.Gtk.Widgets
{
	public static class FontHelpers
	{
		public static double WindowsFontScaleBase = 0.9;
		
		public static FontDescription ScaleFontSize(Widget w, double scaleK)
		{
			double k = scaleK;
			if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
				Environment.OSVersion.Platform == PlatformID.Win32S ||
				Environment.OSVersion.Platform == PlatformID.Win32Windows ||
				Environment.OSVersion.Platform == PlatformID.WinCE)
			{
				k *= WindowsFontScaleBase;
			}
			
			string fd_string = w.PangoContext.FontDescription.Family + " " +
				((int)(k * w.PangoContext.FontDescription.Size / Pango.Scale.PangoScale));
			Pango.FontDescription fd = Pango.FontDescription.FromString(fd_string);
			return fd;
		}
	}
}

