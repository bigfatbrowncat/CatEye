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
			
			Console.WriteLine("Эту фигню я пишу здесь для вида :))");
			Console.WriteLine("Еще одна");
			
			Console.WriteLine("Setting brightness...");
			hdp.AmplitudeAdd(-hdp.AmplitudeFindBlackPoint());
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

