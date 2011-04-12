using System;

namespace CatEye
{
	[StageOperationDescription("Tone"), StageOperationID("ToneStageOperation")]
	public class ToneStageOperation : StageOperation
	{
		public ToneStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		protected internal override void OnDo (DoublePixmap hdp)
		{
			ToneStageOperationParameters pm = (ToneStageOperationParameters)Parameters;
			
			Console.WriteLine("Basic operations: toning... " + pm.BluePart);
			hdp.ApplyChannelsScale(pm.RedPart, pm.GreenPart, pm.BluePart);
			//if (!OnReportProgress(1)) throw new UserCancelException();
				
			base.OnDo (hdp);
		}

	}
}
