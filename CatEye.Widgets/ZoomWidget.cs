using System;

namespace CatEye.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ZoomWidget : Gtk.Bin
	{
		
		private int mDivider = 1;
		private bool setting_divider = false;
		
		public event EventHandler<EventArgs> DividerChanged;
		
		private void UpdateDividerView()
		{
			if (!setting_divider)
			{
				setting_divider = true;
				zoom_label.Text = (100.0 / mDivider).ToString("0") + "%";
				zoom_hscale.Value = 11 - mDivider;
				if (DividerChanged != null) DividerChanged(this, EventArgs.Empty);
				setting_divider = false;
			}
		}
		
		public int Divider
		{
			get { return mDivider; }
			set
			{
				mDivider = value;
				if (mDivider < 1) mDivider = 1;
				if (mDivider > 10) mDivider = 10;
				UpdateDividerView();
			}
		}
		
		public ZoomWidget ()
		{
			this.Build ();
		}

		protected void OnZoomHscaleValueChanged (object sender, System.EventArgs e)
		{
			mDivider = 11 - (int)zoom_hscale.Value;
			UpdateDividerView();
		}

		protected void OnZoom100ButtonClicked (object sender, System.EventArgs e)
		{
			Divider = 1;
		}

		protected void OnZoomInButtonClicked (object sender, System.EventArgs e)
		{
			Divider --;
		}

		protected void OnZoomOutButtonClicked (object sender, System.EventArgs e)
		{
			Divider ++;
		}
	}
}

