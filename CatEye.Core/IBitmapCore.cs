using System;

namespace CatEye.Core
{
	public delegate IBitmapCore BitmapCoreFactory();
	
	public interface IBitmapCore : ICloneable
	{
		int Width { get; }
		int Height { get; }

		void ScaleFast(double k, ProgressReporter callback);
		void AmplitudeMultiply(double Amplitude);
		double AmplitudeFindMedian ();
		double AmplitudeFindBlackPoint ();
		void AmplitudeAdd (double delta);
		void CompressLight(double power, double dark_preserving);
		void SharpenLight(double radius_part, double power, double limit_up, double limit_down, int points, ProgressReporter callback);
		void ApplyTone(Tone tone, double HighlightsInvariance);
		void ApplySaturation(double satur_factor);
		void CutBlackPoint(double black, ProgressReporter callback);
		
		/// <summary>
		/// Crop and rotate the image
		/// </summary>
		bool Crotate(double beta, Point c, int crop_w, int crop_h, int quality, ProgressReporter callback);
		bool Resize(int targetWidth, int targetHeight, int quality, ProgressReporter callback);
	}
}

