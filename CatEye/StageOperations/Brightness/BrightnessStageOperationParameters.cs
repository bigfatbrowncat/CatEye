using System;

namespace CatEye
{
	public class BrightnessStageOperationParameters : StageOperationParameters
	{
		private double mBrightness = 1;
		
		public double Brightness
		{
			get { return mBrightness; }
			set 
			{
				mBrightness = value;
				OnChanged();
			}
		}

		public BrightnessStageOperationParameters ()
		{
		}
	}
}

