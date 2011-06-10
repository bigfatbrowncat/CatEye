using System;

namespace CatEye.Core
{
	[StageOperationDescription("Tone"), StageOperationID("ToneStageOperation")]
	public class ToneStageOperation : StageOperation
	{
		public ToneStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return hdp.Width * hdp.Height;
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			ToneStageOperationParameters pm = (ToneStageOperationParameters)Parameters;
			
			Console.WriteLine("Basic operations: toning... ");
			hdp.ApplyTone(pm.Tone, pm.HighlightsInvariance, 
				delegate (double progress) {
					return OnReportProgress(progress);
				}
			);
		}
		public override Type GetParametersType ()
		{
			return typeof(ToneStageOperationParameters);
		}

	}
}
