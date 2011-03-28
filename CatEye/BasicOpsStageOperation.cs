using System;

namespace CatEye
{
	[StageOperationDescription("Basic operations")]
	public class BasicOpsStageOperation : StageOperation
	{
		public BasicOpsStageOperation (BasicOpsStageOperationParametersWidget parametersWidget)
			: base (parametersWidget)
		{
		}
		
		protected internal override void OnDo (DoublePixmap hdp)
		{
			BasicOpsStageOperationParametersWidget pw = (BasicOpsStageOperationParametersWidget)ParametersWidget;
			
			Console.WriteLine("Basic operations: scaling channels...");
			hdp.ApplyChannelsScale(pw.RedPart, pw.GreenPart, pw.BluePart);
			if (!OnReportProgress(0.33)) throw new UserCancelException();
				
			Console.WriteLine("Basic operations: applying saturation...");
			hdp.ApplySaturation(pw.Saturation);
			if (!OnReportProgress(0.66)) throw new UserCancelException();
				
			Console.WriteLine("Basic operations: scaling light...");
			hdp.ScaleLight(pw.Brightness);
			if (!OnReportProgress(0.99)) throw new UserCancelException();
			
			base.OnDo (hdp);
		}

	}
}
