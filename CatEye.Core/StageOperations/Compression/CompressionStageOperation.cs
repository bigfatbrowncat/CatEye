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
		
		public override void OnDo (IBitmapCore hdp)
		{
			CompressionStageOperationParameters pm = (CompressionStageOperationParameters)Parameters;
			
			Console.WriteLine("Compressing...");
			hdp.CompressLight(pm.Power, pm.DarkPreserving, 
				delegate (double progress) {
					return OnReportProgress(progress);
				}
			);			
			
			base.OnDo (hdp);
		}
		public override Type GetParametersType ()
		{
			return typeof(CompressionStageOperationParameters);
		}

	}
}
