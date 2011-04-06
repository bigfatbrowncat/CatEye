using System;

namespace CatEye
{
	[StageOperationDescription("Brightness"), StageOperationID("BrightnessStageOperation")]
	public class BrightnessStageOperation : StageOperation
	{
		public BrightnessStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}

		protected internal override void OnDo (DoublePixmap hdp)
		{
			BrightnessStageOperationParameters pm = (BrightnessStageOperationParameters)Parameters;
			
			Console.WriteLine("Setting brightness...");
			hdp.ScaleLight(pm.Brightness);
			
			base.OnDo (hdp);
		}
		
	}
}

