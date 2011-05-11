using System;

namespace CatEye.Core
{
	[StageOperationDescription("Crop"), StageOperationID("CropStageOperation")]
	public class CropStageOperation : StageOperation
	{
		public CropStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override FloatPixmap OnDo (FloatPixmap hdp)
		{
			CropStageOperationParameters pm = (CropStageOperationParameters)Parameters;
			
			hdp.Crop(pm.Left, pm.Top, pm.Right, pm.Bottom, 
			         delegate (double progress) {
				return OnReportProgress(progress);
			});
			
			return hdp;
		}
	}
}

