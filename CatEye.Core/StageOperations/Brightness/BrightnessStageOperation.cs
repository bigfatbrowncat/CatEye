using System;

namespace CatEye.Core
{
	[StageOperationDescription("Brightness"), StageOperationID("BrightnessStageOperation")]
	public class BrightnessStageOperation : StageOperation
	{
		public BrightnessStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}

		public override void OnDo (IBitmapCore hdp)
		{
			BrightnessStageOperationParameters pm = (BrightnessStageOperationParameters)Parameters;
			
			
			Console.WriteLine("Calculating current median...");
			pm.Median = hdp.AmplitudeFindMedian();

			Console.WriteLine("Setting brightness...");
			hdp.AmplitudeAdd(-hdp.AmplitudeFindBlackPoint());
			if (pm.Normalize)
			{
				hdp.AmplitudeMultiply(pm.Brightness * 0.5 / pm.Median, 
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
			
			base.OnDo (hdp);
		}
		public override Type GetParametersType ()
		{
			return typeof(BrightnessStageOperationParameters);
		}
		
	}
}

