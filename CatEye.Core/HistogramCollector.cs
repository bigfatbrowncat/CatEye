using System;

namespace CatEye.Core
{
	public class HistogramCollector
	{
		private int mLines;
		private double mMaxLight;
		private double[] mLightHistogramData;
		private double[] mRedHistogramData;
		private double[] mGreenHistogramData;
		private double[] mBlueHistogramData;
		
		public double GetLightHistogramLine(int line)
		{
			return mLightHistogramData[line];
		}
		public double GetRedHistogramLine(int line)
		{
			return mRedHistogramData[line];
		}
		public double GetGreenHistogramLine(int line)
		{
			return mGreenHistogramData[line];
		}
		public double GetBlueHistogramLine(int line)
		{
			return mBlueHistogramData[line];
		}
		public double LineToScale(int line)
		{
			return mMaxLight * line / mLines;
		}
		
		public double MaxLight { get { return mMaxLight; } }
		public int Lines { get { return mLines; } }
		
		public HistogramCollector (double maxLight, int lines)
		{
			mMaxLight = maxLight;
			mLightHistogramData = new double[lines];
			mRedHistogramData = new double[lines];
			mGreenHistogramData = new double[lines];
			mBlueHistogramData = new double[lines];
			mLines = lines;
		}

		public int FindLowTailLightness(double valueAtLeast)
		{
			for (int i = 0; i < mLines; i++)
			{
				if (GetLightHistogramLine(i) > valueAtLeast) return i;
			}
			return 0;
		}
		public int FindLowTailRed(double valueAtLeast)
		{
			for (int i = 0; i < mLines; i++)
			{
				if (GetRedHistogramLine(i) > valueAtLeast) return i;
			}
			return 0;
		}
		public int FindLowTailGreen(double valueAtLeast)
		{
			for (int i = 0; i < mLines; i++)
			{
				if (GetGreenHistogramLine(i) > valueAtLeast) return i;
			}
			return 0;
		}
		public int FindLowTailBlue(double valueAtLeast)
		{
			for (int i = 0; i < mLines; i++)
			{
				if (GetBlueHistogramLine(i) > valueAtLeast) return i;
			}
			return 0;
		}
		
		public int FindHighTailLightness(double valueAtLeast)
		{
			for (int i = Lines - 1; i >= 0; i--)
			{
				if (GetLightHistogramLine(i) > valueAtLeast) return i;
			}
			return 0;
		}
		public int FindHighTailRed(double valueAtLeast)
		{
			for (int i = Lines - 1; i >= 0; i--)
			{
				if (GetRedHistogramLine(i) > valueAtLeast) return i;
			}
			return 0;
		}
		public int FindHighTailGreen(double valueAtLeast)
		{
			for (int i = Lines - 1; i >= 0; i--)
			{
				if (GetGreenHistogramLine(i) > valueAtLeast) return i;
			}
			return 0;
		}
		public int FindHighTailBlue(double valueAtLeast)
		{
			for (int i = Lines - 1; i >= 0; i--)
			{
				if (GetBlueHistogramLine(i) > valueAtLeast) return i;
			}
			return 0;
		}
						
		public void CollectData(IBitmapCore image)
		{
			double light_max = 0;
			for (int i = 0; i < image.Width; i++)
			for (int j = 0; j < image.Height; j++)
			{
				double light = Math.Sqrt(image.RedChannel[i, j] * image.RedChannel[i, j] +
				                         image.GreenChannel[i, j] * image.GreenChannel[i, j] +
				                         image.BlueChannel[i, j] * image.BlueChannel[i, j]) / Math.Sqrt(3);
				
				int light_inx = (int)(light / mMaxLight * mLines);
				if (light_inx >= mLines) light_inx = mLines - 1;
				mLightHistogramData[light_inx] += 1;
				if (mLightHistogramData[light_inx] > light_max) 
					light_max = mLightHistogramData[light_inx];

				int red_inx = (int)(image.RedChannel[i, j] / mMaxLight * mLines);
				if (red_inx >= mLines) red_inx = mLines - 1;
				mRedHistogramData[red_inx] += 1;

				int green_inx = (int)(image.GreenChannel[i, j] / mMaxLight * mLines);
				if (green_inx >= mLines) green_inx = mLines - 1;
				mGreenHistogramData[green_inx] += 1;

				int blue_inx = (int)(image.BlueChannel[i, j] / mMaxLight * mLines);
				if (blue_inx >= mLines) blue_inx = mLines - 1;
				mBlueHistogramData[blue_inx] += 1;
			}
			for (int i = 0; i < mLines; i++)
			{
				mLightHistogramData[i] /= light_max;
				mRedHistogramData[i] /= light_max;
				mGreenHistogramData[i] /= light_max;
				mBlueHistogramData[i] /= light_max;
			}
		}
	}
}

