using System;

namespace CatEye.Core
{
	[StageOperationDescription("Compression"), StageOperationID("CompressionStageOperation")]
	public class CompressionStageOperation : StageOperation
	{
		public CompressionStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return hdp.Width * hdp.Height;
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			CompressionStageOperationParameters pm = (CompressionStageOperationParameters)Parameters;
			
			Console.WriteLine("Compressing...");
			hdp.CompressLight(pm.Power, pm.DarkPreserving, 
				delegate (double progress) {
					return OnReportProgress(progress);
				}
			);			
		}
		public override Type GetParametersType ()
		{
			return typeof(CompressionStageOperationParameters);
		}

	}
}
