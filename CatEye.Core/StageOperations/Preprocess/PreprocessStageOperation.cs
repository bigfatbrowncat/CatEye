using System;

namespace CatEye.Core
{
	[StageOperationDescription("Preprocess", "Preprocessing the photo")]
	[StageOperationID("PreprocessStageOperation")]
	public class PreprocessStageOperation : StageOperation
	{
		public PreprocessStageOperation (StageOperationParameters parameters)
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
			
			PreprocessStageOperationParameters sop = (PreprocessStageOperationParameters)Parameters;
			hdp.CutHighlights(sop.HighlightsCut, 
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

