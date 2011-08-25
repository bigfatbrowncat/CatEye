using System;
using Gtk;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class PreScaleSelectorWidget : Bin
	{
		public int Value
		{
			get { return (int)preScale_hscale.Value; }
			set { preScale_hscale.Value = value; }				
		}
		
		public PreScaleSelectorWidget ()
		{
			this.Build ();
		}

	}
}

