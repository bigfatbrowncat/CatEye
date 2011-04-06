using System;

namespace CatEye
{
	public class UltraSharpStageOperationParameters : StageOperationParameters
	{
		private double mPower = 0.3, mRadius = 0.1, mDelta0 = 0.01;
		private int mPoints = 100;
		
		public double Power
		{
			get { return mPower; }
			set 
			{
				mPower = value;
				OnChanged();
			}
		}

		public double Radius
		{
			get { return mRadius; }
			set 
			{
				mRadius = value;
				OnChanged();
			}
		}
		public double Delta0
		{
			get { return mDelta0; }
			set 
			{
				mDelta0 = value;
				OnChanged();
			}
		}
		public int Points
		{
			get { return mPoints; }
			set 
			{
				mPoints = value;
				OnChanged();
			}
		}

		public UltraSharpStageOperationParameters ()
		{
		}
	}
}

