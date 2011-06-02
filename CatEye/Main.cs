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
		
		private static bool mQuitFlag = false;
		private static int mDelayBeforeUpdate = 100;
		public static void Quit()
		{
			mQuitFlag = true;
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
			
			
			while (!mQuitFlag)
			{
				Gtk.Application.RunIteration();
				if ((DateTime.Now - lastUpdateQueuedTime).TotalMilliseconds > mDelayBeforeUpdate)
				{
					stage.ProcessQueued();
				}
			}
		}
	}
}
