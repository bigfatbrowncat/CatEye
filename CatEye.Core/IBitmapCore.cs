using System;

namespace CatEye.Core
{
	public delegate IBitmapCore BitmapCoreFactory();
	
	public interface IBitmapCore : ICloneable
	{
		int Width { get; }
		int Height { get; }

		double AmplitudeFindMedian ();
		double AmplitudeFindBlackPoint ();

		void ScaleFast(double k, ProgressReporter callback);
		void AmplitudeMultiply(double Amplitude, ProgressReporter callback);
		void AmplitudeAdd (double delta);
		void CompressLight(double power, double dark_preserving, ProgressReporter callback);
		void SharpenLight(double radius_part, double power, double limit_up, double limit_down, int points, ProgressReporter callback);
		void ApplyTone(Tone tone, double HighlightsInvariance, ProgressReporter callback);
		void ApplySaturation(double satur_factor, ProgressReporter callback);
		void CutBlackPoint(double black, ProgressReporter callback);
		
		/// <summary>
		/// Crop and rotate the image
		/// </summary>
		bool Crotate(double beta, Point c, int crop_w, int crop_h, int quality, ProgressReporter callback);
		bool Resize(int targetWidth, int targetHeight, int quality, ProgressReporter callback);
	}
}
