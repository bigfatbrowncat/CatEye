using System;

namespace CatEye.Core
{
	[StageOperationDescription("Black point"), StageOperationID("BlackPointStageOperation")]
	public class BlackPointStageOperation : StageOperation
	{
		public BlackPointStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override FloatPixmap OnDo (FloatPixmap hdp)
		{
			BlackPointStageOperationParameters pm = (BlackPointStageOperationParameters)Parameters;
			
			hdp.CutBlackPoint(pm.Black,
			         delegate (double progress) {
				return OnReportProgress(progress);
			});
			
			return hdp;
		}
	}

}

