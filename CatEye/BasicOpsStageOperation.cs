using System;

namespace CatEye
{
	public class BasicOpsStageOperation : StageOperation
	{
		public BasicOpsStageOperation (int index, BasicOpsStageOperationParametersWidget parametersWidget)
			: base (index, parametersWidget)
		{
		}
		
		public override void OnDo (DoublePixmap hdp)
		{
			BasicOpsStageOperationParametersWidget pw = (BasicOpsStageOperationParametersWidget)mParametersWidget;
			
			Console.WriteLine("Basic operations: scaling channels...");
			hdp.ApplyChannelsScale(pw.RedPart, pw.GreenPart, pw.BluePart);
				
			Console.WriteLine("Basic operations: applying saturation...");
			hdp.ApplySaturation(pw.Saturation);
				
			Console.WriteLine("Basic operations: scaling light...");
			hdp.ScaleLight(pw.Brightness);
			
			base.OnDo (hdp);
		}

	}
}
