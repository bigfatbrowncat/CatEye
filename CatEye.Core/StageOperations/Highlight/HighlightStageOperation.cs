using System;

namespace CatEye.Core
{
	[StageOperationDescription("Cut highlights", "Controlling highlights edge")]
	[StageOperationID("HighlightStageOperation")]
	public class HighlightStageOperation : StageOperation
	{
		public HighlightStageOperation (StageOperationParameters parameters)
			: base(parameters)
		{
		}
		
		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return hdp.Width * hdp.Height;
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			int lines = 1024;
			double tailValueAtLeast = 0.01;
			
			HighlightStageOperationParameters sop = (HighlightStageOperationParameters)Parameters;
			hdp.CutHighlights(sop.Cut, 
			                  sop.Softness, 
			                  lines, 
			                  tailValueAtLeast,
			                  delegate (double progress) {
				return OnReportProgress(progress);
			}
			);
		}
	}
}

