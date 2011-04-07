using System;

namespace CatEye
{
	[StageOperationDescription("Saturation"), StageOperationID("SaturationStageOperation")]
	public class SaturationStageOperation : StageOperation
	{
		public SaturationStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		protected internal override void OnDo (DoublePixmap hdp)
		{
			SaturationStageOperationParameters pm = (SaturationStageOperationParameters)Parameters;
			
			Console.WriteLine("Basic operations: applying saturation...");
			hdp.ApplySaturation(pm.Saturation);
			if (!OnReportProgress(0.66)) throw new UserCancelException();
			
			base.OnDo (hdp);
		}

	}
}
