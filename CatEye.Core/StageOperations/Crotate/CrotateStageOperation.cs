using System;

namespace CatEye.Core
{
	[StageOperationDescription("Crop and Rotate"), StageOperationID("CrotateStageOperation")]
	public class CrotateStageOperation : StageOperation
	{
		public enum Mode 
		{ 
			Disproportional = 0, 
			ProportionalWidthFixed = 1, 
			ProportionalHeightFixed = 2 
		}

		public CrotateStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			CrotateStageOperationParameters pm = (CrotateStageOperationParameters)Parameters;
			
			Console.WriteLine("Rotating...");
			
			// Calculating new picture's real dimensions
			int trueWidth = hdp.Width, trueHeight = hdp.Height;
			
			double w1, h1;
			Point c_pix;

			c_pix = new Point(pm.Center.X * hdp.Width, pm.Center.Y * hdp.Height);
			w1 = pm.CropWidth * hdp.Width;
			h1 = pm.CropHeight * hdp.Height;
			
			double asp_rat;

			if (pm.AspectRatioCustom)
				asp_rat = pm.AspectRatio;
			else
				asp_rat = pm.PresetAspectRatioValues[pm.AspectRatioPreset];
			
			switch (pm.Mode)
			{
			case Mode.Disproportional:
				trueWidth = (int)w1;
				trueHeight = (int)h1;
				break;
			case Mode.ProportionalWidthFixed:
				trueWidth = (int)w1;
				trueHeight = (int)(w1 / asp_rat);
				break;
			case Mode.ProportionalHeightFixed:
				trueWidth = (int)(h1 * asp_rat);
				trueHeight = (int)h1;
				break;
			}
			
			hdp.Crotate(pm.Angle, c_pix, trueWidth, trueHeight, 3,
				delegate (double progress) {
					return OnReportProgress(progress);
				}
			);
		}
		public override Type GetParametersType ()
		{
			return typeof(CrotateStageOperationParameters);
		}
	}
}

