using System;

namespace CatEye.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ZoomWidget : Gtk.Bin
	{
		private double[] mGoodValues = new double[] { 0.125/2, 0.125, 0.25, 0.5, 0.75, 1 };
		
		private double mValue = 1;
		private bool setting_divider = false;

		
		public event EventHandler<EventArgs> ValueChanged;
		
		private bool ValueIsNear(double val)
		{
			return (Value > val - 0.01) && (Value < val + 0.01);
		}
			

		private void UpdateDividerView()
		{
			if (!setting_divider)
			{
				setting_divider = true;
				if (mValue >= 0.99 || Math.Abs(mValue*100 - Math.Round(mValue * 100)) < 0.01)
				{
					zoom_label.Text = (100.0 * mValue).ToString("0") + "%";
				}
				else
				{
					zoom_label.Text = (100.0 * mValue).ToString("0.0") + "%";
				}
				zoom_hscale.Value = (double)mValue;
				setting_divider = false;
			}
		}
		
		public double Value
		{
			get { return mValue; }
			set
			{
				mValue = value;
				if (mValue < mGoodValues[0]) mValue = mGoodValues[0];
				if (mValue > mGoodValues[mGoodValues.Length - 1]) 
					mValue = mGoodValues[mGoodValues.Length - 1];
				if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
				UpdateDividerView();
			}
		}

		public ZoomWidget ()
		{
			this.Build ();
		}

		protected void OnZoomHscaleValueChanged (object sender, System.EventArgs e)
		{
			Value = zoom_hscale.Value;
			
			UpdateDividerView();
		}

		protected void OnZoom100ButtonClicked (object sender, System.EventArgs e)
		{
			Value = 1;
		}

		protected void OnZoomInButtonClicked (object sender, System.EventArgs e)
		{
			for (int i = 0; i < mGoodValues.Length - 1; i++)
			{
				if ((ValueIsNear(mGoodValues[i]) || Value > mGoodValues[i]) && Value < mGoodValues[i + 1])
				{
					Value = mGoodValues[i + 1];
					break;
				}
			}
		}

		protected void OnZoomOutButtonClicked (object sender, System.EventArgs e)
		{
			for (int i = 0; i < mGoodValues.Length - 1; i++)
			{
				if (Value > mGoodValues[i] && (ValueIsNear(mGoodValues[i + 1]) || Value < mGoodValues[i + 1]))
				{
					Value = mGoodValues[i];
					break;
				}
			}
		}
	}
}

