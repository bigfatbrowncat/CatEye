using System;
using System.Globalization;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class UltraSharpStageOperationParametersWidget : StageOperationParametersWidget
	{
		private double mPower = 0.3, mRadius = 0.1, mDelta0 = 0.3;
		private int mPoints = 200;
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		public UltraSharpStageOperationParametersWidget ()
		{
			this.Build ();
		}
		
		public double Power
		{
			get { return mPower; }
			set 
			{
				mPower = value;
				power_entry.Text = value.ToString();
			}
		}

		public double Radius
		{
			get { return mRadius; }
			set 
			{
				mRadius = value;
				radius_hscale.Value = value;
			}
		}
		public double Delta0
		{
			get { return mDelta0; }
			set 
			{
				mDelta0 = value;
				delta_0_entry.Text = value.ToString();
			}
		}
		public int Points
		{
			get { return mPoints; }
			set 
			{
				mPoints = value;
				points_entry.Text = value.ToString();
			}
		}
		
		protected virtual void OnPowerEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(power_entry.Text, NumberStyles.Float, nfi, out res))
			{
				mPower = res;
				OnUserModified();
			}
		}
		
		protected virtual void OnLimitEntryChanged (object sender, System.EventArgs e)
		{
			double res = 1;
			if (double.TryParse(delta_0_entry.Text, NumberStyles.Float, nfi, out res))
			{
				mDelta0 = res;
				OnUserModified();
			}
		}
		
		protected virtual void OnPointsEntryChanged (object sender, System.EventArgs e)
		{
			int res = 100;
			if (int.TryParse(points_entry.Text, NumberStyles.Integer, nfi, out res))
			{
				mPoints = res;
				OnUserModified();
			}
		}
		
		protected virtual void OnRadiusHscaleChangeValue (object o, Gtk.ChangeValueArgs args)
		{
			
		}
		
		protected virtual void OnRadiusHscaleMoveSlider (object o, Gtk.MoveSliderArgs args)
		{
			mRadius = radius_hscale.Value;
			OnUserModified();
		}
		
		
		
		
		
		
		
	}
}
