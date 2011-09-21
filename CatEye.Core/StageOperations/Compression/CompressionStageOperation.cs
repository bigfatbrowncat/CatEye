using System;

namespace CatEye.Core
{
	[StageOperationDescription("Compression", "Lowers the global image contrast, so dark details could be recognized"), 
	 StageOperationID("CompressionStageOperation")]
	public class CompressionStageOperation : StageOperation
	{
		public CompressionStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return (double)hdp.Width * hdp.Height;
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			CompressionStageOperationParameters pm = (CompressionStageOperationParameters)Parameters;
			
			Console.WriteLine("Compressing...");
			hdp.CompressLight(pm.Curve, 
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
