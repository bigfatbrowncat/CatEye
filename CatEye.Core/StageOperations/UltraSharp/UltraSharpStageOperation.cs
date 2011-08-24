using System;

namespace CatEye.Core
{
	[StageOperationDescription("Ultra sharp"), StageOperationID("UltraSharpStageOperation")]
	public class UltraSharpStageOperation : StageOperation
	{
		// TODO: There should be some quality configuration which should calculate
		// points number value
		int points = 150; // 260 is perfect

		public UltraSharpStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return hdp.Width * hdp.Height * points;
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			UltraSharpStageOperationParameters pm = (UltraSharpStageOperationParameters)Parameters;
			
			Console.WriteLine("Ultra sharpening...");
			
			// Making delta0 from base
			//double delta0 = Math.Pow(10, -pm.Base);
			
			
			double pressure = pm.Pressure;
			double contrast = pm.Contrast;
			if (pm.Type == UltraSharpStageOperationParameters.SharpType.Soft) 
				pressure *= -1;
			
			hdp.SharpenLight(pm.Radius, pressure, contrast, points, 
				delegate (double progress) {
					return OnReportProgress(progress);
				}
			);
		}
		public override Type GetParametersType ()
		{
			return typeof(UltraSharpStageOperationParameters);
		}
	}
}
