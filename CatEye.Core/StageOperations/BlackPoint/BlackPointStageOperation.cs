using System;

namespace CatEye.Core
{
	[StageOperationDescription("Black point", "Sets the image's dark point as \"black\" to avoid smoke or to increase global contrast")]
	[StageOperationID("BlackPointStageOperation")]
	public class BlackPointStageOperation : StageOperation
	{
		public BlackPointStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}

		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return (double)hdp.Width * hdp.Height;
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			BlackPointStageOperationParameters pm = (BlackPointStageOperationParameters)Parameters;
			
			hdp.CutBlackPoint(pm.Cut,
			         delegate (double progress) {
				return OnReportProgress(progress);
			});
			
		}
	}

}

