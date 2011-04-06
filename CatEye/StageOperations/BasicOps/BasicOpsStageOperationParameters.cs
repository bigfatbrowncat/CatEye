using System;

namespace CatEye
{
	public class BasicOpsStageOperationParameters : StageOperationParameters
	{
		private double mBrightness = 1, mSaturation = 1, mRedPart = 1, mGreenPart = 1, mBluePart = 1;
		
		public double Brightness
		{
			get { return mBrightness; }
			set 
			{
				mBrightness = value;
				OnChanged();
			}
		}
		public double Saturation
		{
			get { return mSaturation; }
			set
			{
				mSaturation = value;
				OnChanged();
			}
		}
		public double RedPart
		{
			get { return mRedPart; }
			set
			{
				mRedPart = value;
				OnChanged();
			}
		}
		public double GreenPart
		{
			get { return mGreenPart; }
			set
			{
				mGreenPart = value;
				OnChanged();
			}
		}
		public double BluePart
		{
			get { return mBluePart; }
			set
			{
				mBluePart = value;
				OnChanged();
			}
		}
		
		public BasicOpsStageOperationParameters ()
		{
		}
	}
}

