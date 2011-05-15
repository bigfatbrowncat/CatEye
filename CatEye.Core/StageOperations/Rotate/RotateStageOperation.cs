using System;

namespace CatEye.Core
{
	[StageOperationDescription("Rotate"), StageOperationID("RotateStageOperation")]
	public class RotateStageOperation : StageOperation
	{
		public RotateStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override void OnDo (FloatPixmap hdp)
		{
			RotateStageOperationParameters pm = (RotateStageOperationParameters)Parameters;
			
			Console.WriteLine("Rotating...");
			hdp.Rotate(pm.Angle,
				delegate (double progress) {
					return OnReportProgress(progress);
				});
		}
	}
}

