using System;

namespace CatEye
{
	[StageOperationDescription("Brightness")]
	public class BrightnessStageOperation : StageOperation
	{
		public BrightnessStageOperation (BrightnessStageOperationParameters parameters)
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

