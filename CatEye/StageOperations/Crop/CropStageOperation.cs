using System;

namespace CatEye
{
	[StageOperationDescription("Crop")]
	public class CropStageOperation : StageOperation
	{
		public CropStageOperation (CropStageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		protected internal override void OnDo (DoublePixmap hdp)
		{
			CropStageOperationParameters pm = (CropStageOperationParameters)Parameters;
			
			hdp.Crop(pm.Left, pm.Top, pm.Right, pm.Bottom, 
			         delegate (double progress) {
				return OnReportProgress(progress);
			});
			
			base.OnDo (hdp);
		}
	}
}

