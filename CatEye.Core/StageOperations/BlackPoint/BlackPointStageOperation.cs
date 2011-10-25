using System;

namespace CatEye.Core
{
	[StageOperationDescription("Black point", "Sets the image's dark point as \"black\" to avoid smoke or to increase global contrast")]
	[StageOperationID("BlackPointStageOperation")]
	public class BlackPointStageOperation : StageOperation
	{
		private int blur_radius = 1;
		
		public BlackPointStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}

		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return (double)hdp.Width * hdp.Height * (2 * blur_radius * 2 * blur_radius + 1);
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			BlackPointStageOperationParameters pm = (BlackPointStageOperationParameters)Parameters;
			
			hdp.CutBlackPoint(pm.Cut, blur_radius, 0.2, 1024, 0.01,
			         delegate (double progress) {
				return OnReportProgress(progress);
			});
			
		}
	}

}

