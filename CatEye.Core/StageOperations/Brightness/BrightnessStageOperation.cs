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

		public override void OnDo (DoublePixmap hdp)
		{
			BrightnessStageOperationParameters pm = (BrightnessStageOperationParameters)Parameters;
			
			
			Console.WriteLine("Calculating current median...");
			pm.Median = hdp.AmplitudeFindMedian();

			Console.WriteLine("Setting brightness...");
			if (pm.Normalize)
			{
				hdp.AmplitudeMultiply(pm.Brightness * 0.5 / pm.Median);
			}
			else
			{
				hdp.AmplitudeMultiply(pm.Brightness);
			}
			
			base.OnDo (hdp);
		}
		
	}
}

