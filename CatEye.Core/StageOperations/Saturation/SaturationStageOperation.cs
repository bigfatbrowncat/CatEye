using System;

namespace CatEye.Core
{
	[StageOperationDescription("Saturation"), StageOperationID("SaturationStageOperation")]
	public class SaturationStageOperation : StageOperation
	{
		public SaturationStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override void OnDo (FloatPixmap hdp)
		{
			SaturationStageOperationParameters pm = (SaturationStageOperationParameters)Parameters;
			
			Console.WriteLine("Basic operations: applying saturation...");
			hdp.ApplySaturation(pm.Saturation);
			if (!OnReportProgress(0.66)) throw new UserCancelException();
			
			base.OnDo (hdp);
		}
		public override Type GetParametersType ()
		{
			return typeof(SaturationStageOperationParameters);
		}

	}
}
