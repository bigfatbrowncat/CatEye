using System;
using System.Globalization;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class UltraSharpStageOperationParametersWidget : StageOperationParametersWidget
	{
		private double mPower = 0.3, mRadius = 0.1, mWeight = 80, mLimit = 0.3;
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
				radius_entry.Text = value.ToString();
			}
		}
		public double Weight
		{
			get { return mWeight; }
			set 
			{
				mWeight = value;
				weight_entry.Text = value.ToString();
			}
		}
		public double Limit
		{
			get { return mLimit; }
			set 
			{
				mLimit = value;
				limit_entry.Text = value.ToString();
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
		
		protected virtual void OnRadiusEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(radius_entry.Text, NumberStyles.Float, nfi, out res))
			{
				mRadius = res;
				OnUserModified();
			}
		}
		
		protected virtual void OnWeightEntryChanged (object sender, System.EventArgs e)
		{
			double res = 1;
			if (double.TryParse(weight_entry.Text, NumberStyles.Float, nfi, out res))
			{
				mWeight = res;
				OnUserModified();
			}
		}
		
		protected virtual void OnLimitEntryChanged (object sender, System.EventArgs e)
		{
			double res = 1;
			if (double.TryParse(limit_entry.Text, NumberStyles.Float, nfi, out res))
			{
				mLimit = res;
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
		
		
		
		
		
	}
}
