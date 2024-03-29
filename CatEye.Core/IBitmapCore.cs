using System;

namespace CatEye.Core
{
	public interface IBitmapCore : ICloneable
	{
		int Width { get; }
		int Height { get; }
		
		float[,] RedChannel { get; }
		float[,] GreenChannel { get; }
		float[,] BlueChannel { get; }
		
		double AmplitudeFindMedian ();
		double AmplitudeFindBlackPoint ();

		void ScaleFast(double k, ProgressReporter callback);
		void AmplitudeMultiply(double Amplitude, ProgressReporter callback);
		void AmplitudeAdd (double delta);
		void CompressLight(double curve, ProgressReporter callback);
		void SharpenLight(double curve, double noise_gate, double pressure, double contrast, ProgressReporter callback);
		void SharpenEdges(double level, int blur_radius, ProgressReporter callback);
		Tone FindLightTone(Tone dark_tone, double edge, double softness, Point light_center, double light_radius, int points);
		Tone FindDarkTone(Tone light_tone, double edge, double softness, Point dark_center, double dark_radius, int points);
		void ApplyTone(Tone dark_tone, Tone light_tone, double edge, double softness, ProgressReporter callback);
		void ApplySaturation(double satur_factor, ProgressReporter callback);
		void CutBlackPoint(double cut, int blur_radius, double blur_dark_level, int lines, double tailValueAtLeast, ProgressReporter callback);
		void CutHighlights(double cut, double softness, int lines, double tailValueAtLeast, ProgressReporter callback);
		
		/// <summary>
		/// Crop and rotate the image
		/// </summary>
		void Crotate(double beta, Point c, int crop_w, int crop_h, int quality, ProgressReporter callback);
		bool Resize(int targetWidth, int targetHeight, int quality, ProgressReporter callback);
	}
}

