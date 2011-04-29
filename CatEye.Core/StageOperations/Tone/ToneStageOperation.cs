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
			
			Console.WriteLine("Basic operations: toning... ");
			hdp.ApplyTone(pm.Tone);
				
			base.OnDo (hdp);
		}

	}
}
