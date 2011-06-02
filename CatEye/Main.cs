using System;
using Gtk;
using CatEye.Core;

namespace CatEye
{
	class MainClass
	{
		private static Type[] mStageOperationParametersWidgetTypes = new Type[]
		{
			typeof(CompressionStageOperationParametersWidget),
			typeof(BrightnessStageOperationParametersWidget),
			typeof(UltraSharpStageOperationParametersWidget),
			typeof(SaturationStageOperationParametersWidget),
			typeof(ToneStageOperationParametersWidget),
			typeof(BlackPointStageOperationParametersWidget),
			typeof(LimitSizeStageOperationParametersWidget),
			typeof(CrotateStageOperationParametersWidget),
		};
			
		private static IStageOperationHolder StageOperationHolderWidgetFactory(IStageOperationParametersEditor editor)
		{
			return new StageOperationHolderWidget((StageOperationParametersWidget)editor);
		}
		private static IStageOperationParametersEditor StageOperationParametersWidgetFactory(StageOperation so)
		{
			Type paramType = so.GetParametersType();
			Type paramWidgetType = StageOperationIDAttribute.FindTypeByID(
					mStageOperationParametersWidgetTypes,
					StageOperationIDAttribute.GetTypeID(so.GetType())
				);
			StageOperationParametersWidget pwid = (StageOperationParametersWidget)(
				paramWidgetType.GetConstructor(new Type[] { paramType }).Invoke(new object[] { so.Parameters })
			);
			return pwid;
		}
		
		
		public static void Main (string[] args)
		{
			Application.Init ();

			ExtendedStage stage = new ExtendedStage(
				StageOperationParametersWidgetFactory, 
				StageOperationHolderWidgetFactory);
			
			DateTime lastUpdateQueuedTime = DateTime.Now;
			
			stage.UpdateQueued += delegate {
				lastUpdateQueuedTime = DateTime.Now;
			};
			
			MainWindow win = new MainWindow (stage);
			win.Show ();
			GLib.Idle.Add(delegate {
				if ((DateTime.Now - lastUpdateQueuedTime).TotalMilliseconds > 500)
				{
					stage.ProcessQueued();
				}
				return true;
			});
			
			Application.Run();
		}
	}
}
