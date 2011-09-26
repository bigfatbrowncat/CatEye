using System;

namespace CatEye.Core
{
	[StageOperationDescription("Saturation", "Makes the image more or less colorful. Can be used to make a bleck/white image"), 
	 StageOperationID("SaturationStageOperation")]
	public class SaturationStageOperation : StageOperation
	{
		public SaturationStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return (double)hdp.Width * hdp.Height;
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
	}
}
