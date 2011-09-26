using System;

namespace CatEye.Core
{
	[StageOperationID("HistogramStageOperation")]
	public class HistogramStageOperationParameters : StageOperationParameters
	{
		public HistogramStageOperationParameters ()
		{
		}
		
		public override Type GetSOType ()
		{
			return typeof(HistogramStageOperation);
		}		
	}
}

