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
		
		public override void OnDo (DoublePixmap hdp)
		{
			CompressionStageOperationParameters pm = (CompressionStageOperationParameters)Parameters;
			
			Console.WriteLine("Compressing...");
			hdp.CompressLight(pm.Power, pm.Bloha);			
			
			base.OnDo (hdp);
		}

	}
}
