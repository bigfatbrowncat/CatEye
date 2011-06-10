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

		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return hdp.Width * hdp.Height;
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			BlackPointStageOperationParameters pm = (BlackPointStageOperationParameters)Parameters;
			
			hdp.CutBlackPoint(pm.Black,
			         delegate (double progress) {
				return OnReportProgress(progress);
			});
			
		}
		public override Type GetParametersType ()
		{
			return typeof(BlackPointStageOperationParameters);
		}
	}

}

