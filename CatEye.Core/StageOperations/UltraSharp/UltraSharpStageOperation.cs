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
			//double delta0 = Math.Pow(10, -pm.Base);
			
			// TODO: There should be some quality configuration which should calculate
			// points number value
			int points = 200; 
			
			double power = pm.Power;
			if (pm.Type == UltraSharpStageOperationParameters.SharpType.Soft) 
				power *= -1;
			
			FloatPixmap.ISharpeningSamplingMethod sampler = new FloatPixmap.MonteCarloSharpeningSamplingMethod(points, new Random());
			
			hdp.SharpenLight(pm.Radius, power, pm.LimitUp, pm.LimitDown, sampler, 
				delegate (double progress) {
					return OnReportProgress(progress);
				}
			);
			
			base.OnDo (hdp);
		}

	}
}
