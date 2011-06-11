using System;
namespace CatEye
{
	public partial class AboutBox : Gtk.Dialog
	{
		public AboutBox ()
		{
			this.Build ();
			
			title_label.ModifyFont(Pango.FontDescription.FromString("19"));
		}
	}
}

