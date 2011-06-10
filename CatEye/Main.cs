using System;
using Gtk;
using CatEye.Core;

namespace CatEye
{
	class MainClass
	{
		private static readonly Type[] mStageOperationTypes = new Type[]
		{
			typeof(CompressionStageOperation),
			typeof(BrightnessStageOperation),
			typeof(UltraSharpStageOperation),
			typeof(SaturationStageOperation),
			typeof(ToneStageOperation),
			typeof(BlackPointStageOperation),
			typeof(LimitSizeStageOperation),
			typeof(CrotateStageOperation)
		};

		private static readonly Type[] mStageOperationParametersTypes = new Type[]
		{
			typeof(CompressionStageOperationParameters),
			typeof(BrightnessStageOperationParameters),
			typeof(UltraSharpStageOperationParameters),
			typeof(SaturationStageOperationParameters),
			typeof(ToneStageOperationParameters),
			typeof(BlackPointStageOperationParameters),
			typeof(LimitSizeStageOperationParameters),
			typeof(CrotateStageOperationParameters)
		};
				
		private static readonly Type[] mStageOperationParametersWidgetTypes = new Type[]
		{
			typeof(CompressionStageOperationParametersWidget),
			typeof(BrightnessStageOperationParametersWidget),
			typeof(UltraSharpStageOperationParametersWidget),
			typeof(SaturationStageOperationParametersWidget),
			typeof(ToneStageOperationParametersWidget),
			typeof(BlackPointStageOperationParametersWidget),
			typeof(LimitSizeStageOperationParametersWidget),
			typeof(CrotateStageOperationParametersWidget)
		};
		
		private static StageOperation StageOperationFactory(StageOperationParameters parameters)
		{
			string id = StageOperationIDAttribute.GetTypeID(parameters.GetType());
			Type sot = StageOperationIDAttribute.FindTypeByID(mStageOperationTypes, id);

			if (sot == null)
				throw new IncorrectNodeValueException("Can't find StageOperation type for the ID (" + id + ")");

			// Creating stage operation
			StageOperation so = (StageOperation)sot.GetConstructor(
					new Type[] {typeof(StageOperationParameters)}
				).Invoke(new object[] {parameters});
			return so;			
		}
		private static StageOperationParameters StageOperationParametersFactoryFromID(string id)
		{
			Type sopt = StageOperationIDAttribute.FindTypeByID(mStageOperationParametersTypes, id);
				
			if (sopt == null)
				throw new IncorrectNodeValueException("Can't find StageOperationParameters type for the ID (" + id + ")");

			// Creating stage operation parameters
			StageOperationParameters sop = (StageOperationParameters)sopt.GetConstructor(
					new Type[] {}
				).Invoke(new object[] {});
			return sop;			
		}
		
		private static IStageOperationHolder StageOperationHolderFactory(IStageOperationParametersEditor editor)
		{
			return new StageOperationHolderWidget((StageOperationParametersWidget)editor);
		}
		private static IStageOperationParametersEditor StageOperationParametersEditorFactory(StageOperationParameters sop)
		{
			Type paramType = sop.GetType();
			Type paramWidgetType = StageOperationIDAttribute.FindTypeByID(
					mStageOperationParametersWidgetTypes,
					StageOperationIDAttribute.GetTypeID(sop.GetType())
				);
			StageOperationParametersWidget pwid = (StageOperationParametersWidget)(
				paramWidgetType.GetConstructor(new Type[] { paramType }).Invoke(new object[] { sop })
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
				StageOperationFactory, 
				StageOperationParametersFactoryFromID,
				StageOperationParametersEditorFactory, 
				StageOperationHolderFactory);
			
			DateTime lastUpdateQueuedTime = DateTime.Now;
			
			stage.UpdateQueued += delegate {
				lastUpdateQueuedTime = DateTime.Now;
			};
			
			MainWindow win = new MainWindow (stage, mStageOperationTypes);
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
