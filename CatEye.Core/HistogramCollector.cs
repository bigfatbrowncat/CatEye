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
		
		public double MaxLight { get { return mMaxLight; } }
		public int Lines { get { return mLines; } }
		
		public HistogramCollector (double maxLight, int lines)
		{
			mLightHistogramData = new double[lines];
			mRedHistogramData = new double[lines];
			mGreenHistogramData = new double[lines];
			mBlueHistogramData = new double[lines];
			mLines = lines;
		}
		
		public void CollectData(IBitmapCore image)
		{
			double light_max = 0, red_max = 0, green_max = 0, blue_max = 0;
			for (int i = 0; i < image.Width; i++)
			for (int j = 0; j < image.Height; j++)
			{
				double light = Math.Sqrt(image.RedChannel[i, j] * image.RedChannel[i, j] +
				                         image.GreenChannel[i, j] * image.GreenChannel[i, j] +
				                         image.BlueChannel[i, j] * image.BlueChannel[i, j]) / Math.Sqrt(3);

				int light_inx = (int)(light / mMaxLight * mLines);
				mLightHistogramData[light_inx] += 1;
				if (mLightHistogramData[light_inx] > light_max) 
					light_max = mLightHistogramData[light_inx];

				int red_inx = (int)(image.RedChannel[i, j] / mMaxLight * mLines);
				mRedHistogramData[red_inx] += 1;
				if (mRedHistogramData[red_inx] > red_max)
					red_max = mRedHistogramData[red_inx];

				int green_inx = (int)(image.GreenChannel[i, j] / mMaxLight * mLines);
				mGreenHistogramData[green_inx] += 1;
				if (mGreenHistogramData[green_inx] > green_max) 
					green_max = mGreenHistogramData[green_inx];

				int blue_inx = (int)(image.BlueChannel[i, j] / mMaxLight * mLines);
				mBlueHistogramData[blue_inx] += 1;
				if (mBlueHistogramData[blue_inx] > blue_max) 
					blue_max = mBlueHistogramData[blue_inx];
			}
		}
	}
}

