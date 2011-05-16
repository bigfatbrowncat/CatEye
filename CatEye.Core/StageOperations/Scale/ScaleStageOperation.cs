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
		
		public override void OnDo (FloatPixmap hdp)
		{
			ScaleStageOperationParameters pm = (ScaleStageOperationParameters)Parameters;
			
			Console.WriteLine("Resizing...");
			hdp.Resize(
				FloatPixmap.ResizeMode.Disproportional,
				FloatPixmap.ResizeMeasure.Pixels,
				pm.NewWidth, pm.NewHeight, 2, 

				delegate (double progress) {
					return OnReportProgress(progress);
				});				
		}

	}

}
