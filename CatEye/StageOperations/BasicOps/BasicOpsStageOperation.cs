using System;

namespace CatEye
{
	[StageOperationDescription("Basic operations"), StageOperationID("BasicOpsStageOperation")]
	public class BasicOpsStageOperation : StageOperation
	{
		public BasicOpsStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		protected internal override void OnDo (DoublePixmap hdp)
		{
			BasicOpsStageOperationParameters pm = (BasicOpsStageOperationParameters)Parameters;
			
			Console.WriteLine("Basic operations: scaling channels...");
			hdp.ApplyChannelsScale(pm.RedPart, pm.GreenPart, pm.BluePart);
			if (!OnReportProgress(0.33)) throw new UserCancelException();
				
			Console.WriteLine("Basic operations: applying saturation...");
			hdp.ApplySaturation(pm.Saturation);
			if (!OnReportProgress(0.66)) throw new UserCancelException();
				
			Console.WriteLine("Basic operations: scaling light...");
			hdp.ScaleLight(pm.Brightness);
			if (!OnReportProgress(0.99)) throw new UserCancelException();
			
			base.OnDo (hdp);
		}

	}
}
