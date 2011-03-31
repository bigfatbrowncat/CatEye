using System;
using System.Globalization;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class BasicOpsStageOperationParametersWidget : StageOperationParametersWidget
	{
		private double mBrightness = 1, mSaturation = 1, mRedPart = 1, mGreenPart = 1, mBluePart = 1;
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		public BasicOpsStageOperationParametersWidget ()
		{
			this.Build ();
		}
		
		public double Brightness
		{
			get { return mBrightness; }
			set 
			{
				mBrightness = value;
				brightness_entry.Text = value.ToString();
			}
		}
		public double Saturation
		{
			get { return mSaturation; }
			set
			{
				mSaturation = value;
				saturation_entry.Text = value.ToString();
			}
		}
		public double RedPart
		{
			get { return mRedPart; }
			set
			{
				mRedPart = value;
				red_entry.Text = value.ToString();
			}
		}
		public double GreenPart
		{
			get { return mGreenPart; }
			set
			{
				mGreenPart = value;
				green_entry.Text = value.ToString();
			}
		}
		public double BluePart
		{
			get { return mBluePart; }
			set
			{
				mBluePart = value;
				blue_entry.Text = value.ToString();
			}
		}
		
		protected virtual void OnBrightnessEntryChanged (object sender, System.EventArgs e)
		{
			double res = 1;
			if (double.TryParse(brightness_entry.Text, NumberStyles.Float, nfi, out res))
			{
				mBrightness = res;
				OnUserModified();
			}
		}
		
		protected virtual void OnSaturationEntryChanged (object sender, System.EventArgs e)
		{
			double res = 1;
			if (double.TryParse(saturation_entry.Text, NumberStyles.Float, nfi, out res))
			{
				mSaturation = res;
				OnUserModified();
			}
		}
		
		protected virtual void OnRedEntryChanged (object sender, System.EventArgs e)
		{
			double res = 1;
			if (double.TryParse(red_entry.Text, NumberStyles.Float, nfi, out res))
			{
				mRedPart = res;
				OnUserModified();
			}
		}
		
		protected virtual void OnGreenEntryChanged (object sender, System.EventArgs e)
		{
			double res = 1;
			if (double.TryParse(green_entry.Text, NumberStyles.Float, nfi, out res))
			{
				mGreenPart = res;
				OnUserModified();
			}
		}
		
		protected virtual void OnBlueEntryChanged (object sender, System.EventArgs e)
		{
			double res = 1;
			if (double.TryParse(blue_entry.Text, NumberStyles.Float, nfi, out res))
			{
				mBluePart = res;
				OnUserModified();
			}
		}
	}
}
