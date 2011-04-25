using System;

namespace CatEye.Core
{
	[StageOperationDescription("Tone"), StageOperationID("ToneStageOperation")]
	public class ToneStageOperation : StageOperation
	{
		public ToneStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override void OnDo (FloatPixmap hdp)
		{
			ToneStageOperationParameters pm = (ToneStageOperationParameters)Parameters;
			
			Console.WriteLine("Basic operations: toning... " + pm.BluePart);
			hdp.ApplyChannelsScale(pm.RedPart, pm.GreenPart, pm.BluePart);
			//if (!OnReportProgress(1)) throw new UserCancelException();
				
			base.OnDo (hdp);
		}

	}
}
