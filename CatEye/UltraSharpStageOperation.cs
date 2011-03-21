using System;

namespace CatEye
{
	[StageOperationDescription("Ultra sharping")]
	public class UltraSharpStageOperation : StageOperation
	{
		public UltraSharpStageOperation (UltraSharpStageOperationParametersWidget parametersWidget, Stages owner)
			: base (parametersWidget, owner)
		{
		}
		
		protected internal override void OnDo (DoublePixmap hdp)
		{
			UltraSharpStageOperationParametersWidget pw = (UltraSharpStageOperationParametersWidget)ParametersWidget;
			
			Console.WriteLine("Ultra sharpening...");
			hdp.SharpenLight(pw.Radius, pw.Power, pw.Delta0,
					         new DoublePixmap.MonteCarloSharpeningSamplingMethod(pw.Points, new Random()), 
			                 delegate (double progress) {
				return OnReportProgress(progress);
			});
			
			base.OnDo (hdp);
		}

	}
}
