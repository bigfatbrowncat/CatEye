using System;
using CatEye.UI.Gtk.Widgets;

namespace CatEye
{
	public partial class AboutBox : Gtk.Dialog
	{
		public AboutBox ()
		{
			this.Build ();
			
			title_label.ModifyFont(FontHelpers.ScaleFontSize(title_label, 2));
		}
	}
}

