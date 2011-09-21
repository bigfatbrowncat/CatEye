using System;

namespace CatEye.Core
{
	[StageOperationDescription("Tone")]
	[StageOperationID("ToneStageOperation")]
	public class ToneStageOperation : StageOperation
	{
		public ToneStageOperation (StageOperationParameters parameters)
			: base (parameters)
		{
		}
		
		public override double CalculateEfforts (IBitmapCore hdp)
		{
			return (double)hdp.Width * hdp.Height;
		}
		
		public override void OnDo (IBitmapCore hdp)
		{
			int points = 500;	// TODO: Make option
			
			ToneStageOperationParameters pm = (ToneStageOperationParameters)Parameters;
			
			Tone darkTone = pm.DarkTone;
			Tone lightTone = pm.LightTone;
			
			if (pm.AutoDarkTone)
			{
				darkTone = hdp.FindDarkTone(
					pm.LightTone, 
					pm.Edge, 
					pm.Softness, 
					pm.AutoDarkCenter,
					pm.AutoDarkRadius, 
					points);
			}
			if (pm.AutoLightTone)
			{
				lightTone = hdp.FindLightTone(
					pm.DarkTone, 
					pm.Edge, 
					pm.Softness, 
					pm.AutoLightCenter,
					pm.AutoLightRadius, 
					points);
			}
			
			Console.WriteLine("Basic operations: toning... ");
			hdp.ApplyTone(darkTone, lightTone, pm.Edge, pm.Softness,
				delegate (double progress) {
					return OnReportProgress(progress);
				}
			);
		}
		public override Type GetParametersType ()
		{
			return typeof(ToneStageOperationParameters);
		}

	}
}
