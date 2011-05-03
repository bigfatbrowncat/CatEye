using System;

namespace CatEye.Core
{
	[StageOperationDescription("Hard cut"), StageOperationID("HardCutStageOperation")]
	public class HardCutStageOperation : StageOperation
	{
		public HardCutStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override void OnDo (FloatPixmap hdp)
		{
			HardCutStageOperationParameters pm = (HardCutStageOperationParameters)Parameters;
			
			hdp.HardCut(pm.Black, pm.White,
			         delegate (double progress) {
				return OnReportProgress(progress);
			});
			
			base.OnDo (hdp);
		}
	}

}

