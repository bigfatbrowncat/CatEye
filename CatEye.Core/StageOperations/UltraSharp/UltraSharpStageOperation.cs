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
		
		public override void OnDo (FloatPixmap hdp)
		{
			UltraSharpStageOperationParameters pm = (UltraSharpStageOperationParameters)Parameters;
			
			Console.WriteLine("Ultra sharpening...");
			
			// Making delta0 from base
			double delta0 = Math.Pow(10, -pm.Base);
			
			hdp.SharpenLight(pm.Radius, pm.Power, delta0,
					         new FloatPixmap.MonteCarloSharpeningSamplingMethod(pm.Points, new Random()), 
			                 delegate (double progress) {
				return OnReportProgress(progress);
			});
			
			base.OnDo (hdp);
		}

	}
}
