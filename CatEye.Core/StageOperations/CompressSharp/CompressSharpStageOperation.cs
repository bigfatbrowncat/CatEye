using System;

namespace CatEye.Core
{
	[StageOperationDescription("Compress & Sharp", "Changes the global contrast. Also lets you to make the image more impressive by increasing the local contrast")]
	[StageOperationID("CompressSharpStageOperation")]
	public class CompressSharpStageOperation : StageOperation
	{
		public CompressSharpStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override double CalculateEfforts (IBitmapCore hdp)
		{
			CompressSharpStageOperationParameters pm = (CompressSharpStageOperationParameters)Parameters;

			return (double)hdp.Width * hdp.Height * (5 * pm.Pressure + 1) * 5;
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			CompressSharpStageOperationParameters pm = (CompressSharpStageOperationParameters)Parameters;
			
			Console.WriteLine("Compressing and sharpening...");
			
			//if (pm.Type == LocalContrastStageOperationParameters.SharpType.Soft) 
			//	pressure *= -1;
			
			hdp.SharpenLight(pm.Curve, pm.NoiseGate, pm.Pressure, 
				delegate (double progress) {
					return OnReportProgress(progress);
				}
			);
		}

	}
}
