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

		protected void OnPreScaleHscaleValueChanged (object sender, System.EventArgs e)
		{
			preScale_hscale.Value = Math.Round(preScale_hscale.Value);
			value_label.UseMarkup = true;
			if (preScale_hscale.Value > 1)
				value_label.Markup = "<b>" + (1.0 / preScale_hscale.Value * 100).ToString("0.0") + "%</b>";
			else
				value_label.Markup = "<b>" + (1.0 / preScale_hscale.Value * 100).ToString("0") + "%</b>";
				
		}
	}
}

