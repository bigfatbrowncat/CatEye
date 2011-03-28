using System;

namespace CatEye
{
	[StageOperationDescription("Compression")]
	public class CompressionStageOperation : StageOperation
	{
		public CompressionStageOperation (CompressionStageOperationParametersWidget parametersWidget)
			: base (parametersWidget)
		{
		}
		
		protected internal override void OnDo (DoublePixmap hdp)
		{
			CompressionStageOperationParametersWidget pw = (CompressionStageOperationParametersWidget)ParametersWidget;
			
			Console.WriteLine("Compressing...");
			hdp.CompressLight(pw.Power, pw.Bloha);			
			
			base.OnDo (hdp);
		}

	}
}
