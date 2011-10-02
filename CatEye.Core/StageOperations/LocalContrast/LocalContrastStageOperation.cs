using System;

namespace CatEye.Core
{
	[StageOperationDescription("Local contrast", "Lets you to make the image more impressive by increasing/decreasing of its contrast")]
	[StageOperationID("LocalContrastStageOperation")]
	public class LocalContrastStageOperation : StageOperation
	{
		// TODO: There should be some quality configuration which should calculate
		// points number value
		int points = 300; // 260 is perfect

		public LocalContrastStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return (double)hdp.Width * hdp.Height * points;
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			LocalContrastStageOperationParameters pm = (LocalContrastStageOperationParameters)Parameters;
			
			Console.WriteLine("Applying Local Contrast...");
			
			// Making delta0 from base
			//double delta0 = Math.Pow(10, -pm.Base);
			
			
			double pressure = pm.Pressure;
			double contrast = pm.Contrast;
			if (pm.Type == LocalContrastStageOperationParameters.SharpType.Soft) 
				pressure *= -1;
			
			hdp.SharpenLight(pm.Radius, pressure, contrast, points, 
				delegate (double progress) {
					return OnReportProgress(progress);
				}
			);
		}

	}
}
