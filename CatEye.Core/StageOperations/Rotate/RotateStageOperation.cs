using System;

namespace CatEye.Core
{
	[StageOperationDescription("Rotate"), StageOperationID("RotateStageOperation")]
	public class RotateStageOperation : StageOperation
	{
		public enum Mode 
		{ 
			Disproportional = 0, 
			ProportionalWidthFixed = 1, 
			ProportionalHeightFixed = 2 
		}
		public enum Measure
		{
			Pixels = 0,
			Percents = 1
		}

		public RotateStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override void OnDo (FloatPixmap hdp)
		{
			RotateStageOperationParameters pm = (RotateStageOperationParameters)Parameters;
			
			Console.WriteLine("Rotating...");
			
			// Calculating new picture's real dimensions
			int trueWidth = hdp.width, trueHeight = hdp.height;
			
			double w1, h1;
			Point c_pix;
			if (pm.Measure == Measure.Pixels)
			{
				c_pix = pm.Center;
				w1 = pm.CropWidth;
				h1 = pm.CropHeight;
			}	
			else //if (pm.Measure == Measure.Percents)
			{
				c_pix = new Point(pm.Center.X * hdp.width, pm.Center.Y * hdp.height);
				w1 = pm.CropWidth * hdp.width;
				h1 = pm.CropHeight * hdp.height;
			}
			
			switch (pm.Mode)
			{
			case Mode.Disproportional:
				trueWidth = (int)(w1);
				trueHeight = (int)(h1);
				break;
			case Mode.ProportionalWidthFixed:
				trueWidth = (int)(w1);
				trueHeight = (int)(w1 / pm.AspectRatio);
				break;
			case Mode.ProportionalHeightFixed:
				trueWidth = (int)(h1 * pm.AspectRatio);
				trueHeight = (int)(h1);
				break;
			}
			
			
			hdp.Crotate(pm.Angle, c_pix, trueWidth, trueHeight, 2,
				delegate (double progress) {
					return OnReportProgress(progress);
				});
		}
	}
}

