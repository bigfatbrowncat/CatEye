using System;

namespace CatEye.Core
{
	[StageOperationDescription("Scale"), StageOperationID("ScaleStageOperation")]
	public class ScaleStageOperation : StageOperation
	{
		public ScaleStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override FloatPixmap OnDo (FloatPixmap hdp)
		{
			ScaleStageOperationParameters pm = (ScaleStageOperationParameters)Parameters;
			
			Console.WriteLine("Resizing...");
			FloatPixmap res = hdp.Resize(
				FloatPixmap.ResizeMode.Disproportional,
				FloatPixmap.ResizeMeasure.Pixels,
				pm.NewWidth, pm.NewHeight, 

				delegate (double progress) {
					return OnReportProgress(progress);
				});				
			
			return res;
		}

	}

}

