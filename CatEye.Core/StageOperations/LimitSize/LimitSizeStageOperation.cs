using System;

namespace CatEye.Core
{
	[StageOperationDescription("Limit size"), StageOperationID("LimitSizeStageOperation")]
	public class LimitSizeStageOperation : StageOperation
	{
		public enum Mode 
		{ 
			Disproportional = 0, 
			ProportionalWidthFixed = 1, 
			ProportionalHeightFixed = 2 
		}
		
		public LimitSizeStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			LimitSizeStageOperationParameters pm = (LimitSizeStageOperationParameters)Parameters;
			
			Console.WriteLine("Limiting size...");
			
			double w = hdp.Width, h = hdp.Height;

			if (pm.LimitWidth && w > pm.NewWidth) 
			{
				h *= pm.NewWidth / w;
				w = pm.NewWidth;
			}
			
			if (pm.LimitHeight && h > pm.NewHeight)
			{
				w *= pm.NewHeight / h;
				h = pm.NewHeight;
			}

			if (pm.LimitWidth || pm.LimitHeight)
			{
				hdp.Resize((int)w, (int)h, 3, 
					delegate (double progress) {
						return OnReportProgress(progress);
					});
			}
		}
		public override Type GetParametersType ()
		{
			return typeof(LimitSizeStageOperationParameters);
		}

	}

}
