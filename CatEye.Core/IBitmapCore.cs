using System;

namespace CatEye.Core
{
	public interface IBitmapCore : ICloneable
	{
		int Width { get; }
		int Height { get; }

		double AmplitudeFindMedian ();
		double AmplitudeFindBlackPoint ();

		void ScaleFast(double k, ProgressReporter callback);
		void AmplitudeMultiply(double Amplitude, ProgressReporter callback);
		void AmplitudeAdd (double delta);
		void CompressLight(double curve, ProgressReporter callback);
		void SharpenLight(double radius_part, double pressure, double contrast, int points, ProgressReporter callback);
		void ApplyTone(Tone tone, double edge, double softness, ProgressReporter callback);
		void ApplySaturation(double satur_factor, ProgressReporter callback);
		void CutBlackPoint(double cut, ProgressReporter callback);
		
		/// <summary>
		/// Crop and rotate the image
		/// </summary>
		void Crotate(double beta, Point c, int crop_w, int crop_h, int quality, ProgressReporter callback);
		bool Resize(int targetWidth, int targetHeight, int quality, ProgressReporter callback);
	}
}

