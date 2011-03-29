using System;
using System.Globalization;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class BrightnessStageOperationParametersWidget : StageOperationParametersWidget
	{
		private double mBrightness = 0.9;
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		public double Brightness
		{
			get { return mBrightness; }
			set 
			{
				mBrightness = value;
				brightness_entry.Text = value.ToString();
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
		
		
		public BrightnessStageOperationParametersWidget ()
		{
			this.Build ();
		}
	}
}

