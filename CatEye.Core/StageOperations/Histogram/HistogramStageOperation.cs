using System;

namespace CatEye.Core
{
	[StageOperationDescription("Histogram", "Shows the image's histogram")]
	[StageOperationID("HistogramStageOperation")]
	public class HistogramStageOperation : StageOperation
	{
		public HistogramStageOperation (StageOperationParameters parameters)
			: base(parameters)
		{
		}
		
		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return 0;
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			// Do nothing
		}
	}
}

