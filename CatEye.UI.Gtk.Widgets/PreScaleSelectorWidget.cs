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
			if (preScale_hscale.Value > 1)
				value_label.Text = (1.0 / preScale_hscale.Value * 100).ToString("0.0") + "%";
			else
				value_label.Text = (1.0 / preScale_hscale.Value * 100).ToString("0") + "%";
				
		}
	}
}

