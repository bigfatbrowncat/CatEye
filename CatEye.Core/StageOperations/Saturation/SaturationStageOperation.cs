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
		
		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return hdp.Width * hdp.Height;
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			SaturationStageOperationParameters pm = (SaturationStageOperationParameters)Parameters;
			
			Console.WriteLine("Basic operations: applying saturation...");
			hdp.ApplySaturation(pm.Saturation, 
				delegate (double progress) {
					return OnReportProgress(progress);
				}
			);
		}
		public override Type GetParametersType ()
		{
			return typeof(SaturationStageOperationParameters);
		}

	}
}
