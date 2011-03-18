using System;

namespace CatEye
{
	[StageOperationDescription("Basic operations")]
	public class BasicOpsStageOperation : StageOperation
	{
		public BasicOpsStageOperation (BasicOpsStageOperationParametersWidget parametersWidget, Stages owner)
			: base (parametersWidget, owner)
		{
		}
		
		protected internal override void OnDo (DoublePixmap hdp)
		{
			BasicOpsStageOperationParametersWidget pw = (BasicOpsStageOperationParametersWidget)ParametersWidget;
			
			Console.WriteLine("Basic operations: scaling channels...");
			hdp.ApplyChannelsScale(pw.RedPart, pw.GreenPart, pw.BluePart);
			OnReportProgress(0.33);
				
			Console.WriteLine("Basic operations: applying saturation...");
			hdp.ApplySaturation(pw.Saturation);
			OnReportProgress(0.66);
				
			Console.WriteLine("Basic operations: scaling light...");
			hdp.ScaleLight(pw.Brightness);
			OnReportProgress(0.99);
			
			base.OnDo (hdp);
		}

	}
}
