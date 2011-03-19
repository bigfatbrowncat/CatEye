using System;

namespace CatEye
{
	[StageOperationDescription("Downscaling")]
	public class DownscalingStageOperation : StageOperation
	{
		public DownscalingStageOperation (DownscalingStageOperationParametersWidget parametersWidget, Stages owner)
			: base (parametersWidget, owner)
		{
		}
		
		protected internal override void OnDo (DoublePixmap hdp)
		{
			DownscalingStageOperationParametersWidget pw = (DownscalingStageOperationParametersWidget)ParametersWidget;
			
			Console.WriteLine("Downscaling...");
			if (pw.ScaleValue != 1) hdp.Downscale(pw.ScaleValue, delegate (double progress) {
				return OnReportProgress(progress);
			});

			base.OnDo (hdp);
		}

	}
}
