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
		
		public override void OnDo (IBitmapCore hdp)
		{
			SaturationStageOperationParameters pm = (SaturationStageOperationParameters)Parameters;
			
			Console.WriteLine("Basic operations: applying saturation...");
			hdp.ApplySaturation(pm.Saturation);
			
			base.OnDo (hdp);
		}
		public override Type GetParametersType ()
		{
			return typeof(SaturationStageOperationParameters);
		}

	}
}
