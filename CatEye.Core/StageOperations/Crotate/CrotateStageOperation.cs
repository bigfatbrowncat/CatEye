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
		
		public override void OnDo (FloatPixmap hdp)
		{
			CrotateStageOperationParameters pm = (CrotateStageOperationParameters)Parameters;
			
			Console.WriteLine("Rotating...");
			
			// Calculating new picture's real dimensions
			int trueWidth = hdp.width, trueHeight = hdp.height;
			
			double w1, h1;
			Point c_pix;

			c_pix = new Point(pm.Center.X * hdp.width, pm.Center.Y * hdp.height);
			w1 = pm.CropWidth * hdp.width;
			h1 = pm.CropHeight * hdp.height;
			
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
			
			Console.WriteLine(pm.Mode.ToString());
			
			hdp.Crotate(pm.Angle, c_pix, trueWidth, trueHeight, 2,
				delegate (double progress) {
					return OnReportProgress(progress);
				}
			);
		}
	}
}

