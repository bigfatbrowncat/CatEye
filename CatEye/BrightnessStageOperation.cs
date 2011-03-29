using System;
namespace CatEye
{
	[StageOperationDescription("Brightness")]
	public class BrightnessStageOperation : StageOperation
	{
		public BrightnessStageOperation (BrightnessStageOperationParametersWidget parametersWidget)
			: base (parametersWidget)
		{
		}

		protected internal override void OnDo (DoublePixmap hdp)
		{
			BrightnessStageOperationParametersWidget pw = (BrightnessStageOperationParametersWidget)ParametersWidget;
			
			Console.WriteLine("Setting brightness...");
			hdp.ScaleLight(pw.Brightness);
			
			base.OnDo (hdp);
		}
		
	}
}

