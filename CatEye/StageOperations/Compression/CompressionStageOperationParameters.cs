using System;

namespace CatEye
{
	public class CompressionStageOperationParameters : StageOperationParameters
	{
		private double mPower = 0.7, mBloha = 0.01;

		public double Power
		{
			get { return mPower; }
			set 
			{
				mPower = value;
				OnChanged();
			}
		}
		public double Bloha
		{
			get { return mBloha; }
			set 
			{
				mBloha = value;
				OnChanged();
			}
		}
		
		public CompressionStageOperationParameters ()
		{
		}
	}
}

