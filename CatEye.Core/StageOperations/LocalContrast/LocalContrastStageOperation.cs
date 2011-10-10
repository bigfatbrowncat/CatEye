using System;

namespace CatEye.Core
{
	[StageOperationDescription("Local contrast", "Lets you to make the image more impressive by increasing/decreasing of its contrast")]
	[StageOperationID("LocalContrastStageOperation")]
	public class LocalContrastStageOperation : StageOperation
	{
		public LocalContrastStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return (double)hdp.Width * hdp.Height;
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			LocalContrastStageOperationParameters pm = (LocalContrastStageOperationParameters)Parameters;
			
			Console.WriteLine("Applying Local Contrast...");
			
			//if (pm.Type == LocalContrastStageOperationParameters.SharpType.Soft) 
			//	pressure *= -1;
			
			hdp.SharpenLight(pm.Curve, pm.Contrast, pm.Pressure, 
				delegate (double progress) {
					return OnReportProgress(progress);
				}
			);
		}

	}
}
