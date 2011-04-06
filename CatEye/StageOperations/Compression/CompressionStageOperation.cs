using System;

namespace CatEye
{
	[StageOperationDescription("Compression")]
	public class CompressionStageOperation : StageOperation
	{
		public CompressionStageOperation (CompressionStageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		protected internal override void OnDo (DoublePixmap hdp)
		{
			CompressionStageOperationParameters pm = (CompressionStageOperationParameters)Parameters;
			
			Console.WriteLine("Compressing...");
			hdp.CompressLight(pm.Power, pm.Bloha);			
			
			base.OnDo (hdp);
		}

	}
}
