using System;
namespace CatEye
{
	[StageOperationDescription("Crop")]
	public class CropStageOperation : StageOperation
	{
		public CropStageOperation (CropStageOperationParametersWidget parametersWidget)
			: base (parametersWidget)
		{
		}
		
		protected internal override void OnDo (DoublePixmap hdp)
		{
			CropStageOperationParametersWidget pw = (CropStageOperationParametersWidget)ParametersWidget;
			
			hdp.Crop(pw.Left, pw.Top, pw.Right, pw.Bottom, 
			         delegate (double progress) {
				return OnReportProgress(progress);
			});
			
			base.OnDo (hdp);
		}
	}
}

