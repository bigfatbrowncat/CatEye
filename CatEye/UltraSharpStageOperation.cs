using System;

namespace CatEye
{
	public class UltraSharpStageOperation : StageOperation
	{
		public UltraSharpStageOperation (int index, UltraSharpStageOperationParametersWidget parametersWidget)
			: base (index, parametersWidget)
		{
		}
		
		public override void OnDo (DoublePixmap hdp)
		{
			UltraSharpStageOperationParametersWidget pw = (UltraSharpStageOperationParametersWidget)mParametersWidget;
			
			Console.WriteLine("Ultra sharpening...");
			hdp.SharpenLight(pw.Radius, pw.Power, pw.Weight, pw.Limit,
					         new DoublePixmap.MonteCarloSharpeningSamplingMethod(200, new Random()));
			
			base.OnDo (hdp);
		}

	}
}
