using System;

namespace CatEye
{
	public class DownscalingStageOperation : StageOperation
	{
		public DownscalingStageOperation (int index, DownscalingStageOperationParametersWidget parametersWidget)
			: base (index, parametersWidget)
		{
		}
		
		public override void OnDo (DoublePixmap hdp)
		{
			DownscalingStageOperationParametersWidget pw = (DownscalingStageOperationParametersWidget)mParametersWidget;
			
			Console.WriteLine("Downscaling...");
			if (pw.ScaleValue != 1) hdp.Downscale(pw.ScaleValue);

			base.OnDo (hdp);
		}

	}
}
