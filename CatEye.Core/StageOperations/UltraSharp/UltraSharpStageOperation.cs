using System;

namespace CatEye.Core
{
	[StageOperationDescription("Ultra sharp"), StageOperationID("UltraSharpStageOperation")]
	public class UltraSharpStageOperation : StageOperation
	{
		public UltraSharpStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override void OnDo (DoublePixmap hdp)
		{
			UltraSharpStageOperationParameters pm = (UltraSharpStageOperationParameters)Parameters;
			
			Console.WriteLine("Ultra sharpening...");
			hdp.SharpenLight(pm.Radius, pm.Power, pm.Delta0,
					         new DoublePixmap.MonteCarloSharpeningSamplingMethod(pm.Points, new Random()), 
			                 delegate (double progress) {
				return OnReportProgress(progress);
			});
			
			base.OnDo (hdp);
		}

	}
}