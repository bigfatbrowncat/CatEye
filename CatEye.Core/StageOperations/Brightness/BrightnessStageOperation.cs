using System;

namespace CatEye.Core
{
	[StageOperationDescription("Brightness", 
		"Makes the image brighter or darker. Can also normalize the brightness to an average value.")]
	[StageOperationID("BrightnessStageOperation")]
	public class BrightnessStageOperation : StageOperation
	{
		public BrightnessStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return (double)hdp.Width * hdp.Height;
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			BrightnessStageOperationParameters pm = (BrightnessStageOperationParameters)Parameters;
			
			
			Console.WriteLine("Calculating current median...");
			double median = hdp.AmplitudeFindMedian();

			Console.WriteLine("Setting brightness...");

			if (pm.Normalize)
			{
				hdp.AmplitudeMultiply(pm.Brightness * 0.5 / median, 
					delegate (double progress) {
						return OnReportProgress(progress);
					}
				);
			}
			else
			{
				hdp.AmplitudeMultiply(pm.Brightness, 
					delegate (double progress) {
						return OnReportProgress(progress);
					}
				);
			}
		}
		public override Type GetParametersType ()
		{
			return typeof(BrightnessStageOperationParameters);
		}
		
	}
}

