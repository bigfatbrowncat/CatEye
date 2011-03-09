using System;

namespace CatEye
{
	public class CompressionStageOperation : StageOperation
	{
		public CompressionStageOperation (int index, CompressionStageOperationParametersWidget parametersWidget)
			: base (index, parametersWidget)
		{
		}
		
		public override void OnDo (DoublePixmap hdp)
		{
			CompressionStageOperationParametersWidget pw = (CompressionStageOperationParametersWidget)mParametersWidget;
			
			Console.WriteLine("Compressing...");
			hdp.CompressLight(pw.Power, pw.Bloha);			
			
			base.OnDo (hdp);
		}

	}
}
