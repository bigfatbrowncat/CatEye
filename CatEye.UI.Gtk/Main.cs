using System;
using Gtk;
using CatEye.Core;
using CatEye.UI.Base;
using CatEye.UI.Gtk;
using CatEye.UI.Gtk.Widgets;

namespace CatEye
{
	class MainClass
	{
		public static ExtendedStage stage;
		public static RenderingQueue rq;

		public static MainWindow win;
		public static RenderingQueueWindow rqwin;
			
		public static readonly Type[] mStageOperationTypes = new Type[]
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

		public static readonly Type[] mStageOperationParametersTypes = new Type[]
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
				
		public static readonly Type[] mStageOperationParametersWidgetTypes = new Type[]
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
		
		public static StageOperation StageOperationFactory(StageOperationParameters parameters)
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
		public static StageOperationParameters StageOperationParametersFactoryFromID(string id)
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
		
		public static IStageOperationHolder StageOperationHolderFactory(IStageOperationParametersEditor editor)
		{
			return new StageOperationHolderWidget((StageOperationParametersWidget)editor);
		}
		public static IStageOperationParametersEditor StageOperationParametersEditorFactory(StageOperationParameters sop)
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
		
		public static IBitmapCore ImageLoader(PPMLoader ppl, ProgressReporter callback)
		{
			return FloatBitmapGtk.FromPPM(ppl, callback);
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
			
			// Initializing rendering queue and it's window
			rq = new RenderingQueue();
			rq.ItemRendering += HandleRqItemRendering;
			
			rqwin = new RenderingQueueWindow(rq);
			//rqwin.Visible = false;

			// Initializing stage and main window
			stage = new ExtendedStage(
				StageOperationFactory, 
				StageOperationParametersFactoryFromID,
				StageOperationParametersEditorFactory, 
				StageOperationHolderFactory, 
				ImageLoader);
			
			DateTime lastUpdateQueuedTime = DateTime.Now;
			
			stage.UpdateQueued += delegate {
				lastUpdateQueuedTime = DateTime.Now;
			};
			
			win = new MainWindow (stage, mStageOperationTypes);
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

		static void HandleRqItemRendering (object sender, RenderingTaskEventArgs e)
		{
			FloatBitmapGtk renderDest = (FloatBitmapGtk)e.Target.Stage.CurrentImage;
			
			// Drawing to pixbuf and saving to file
			using (Gdk.Pixbuf rp = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 8, renderDest.Width, renderDest.Height))
			{
	
				renderDest.DrawToPixbuf(rp, 
					delegate (double progress) {
					
						// TODO: report progress via RenderingTask (needed to create an event)
						//rpw.SetStatusAndProgress(progress, "Saving image...");
						while (Application.EventsPending()) Application.RunIteration();
						return true;
					}
				);
			
				// TODO Can't be used currently cause of buggy Gtk#
				//rp.Savev(filename, type, new string[] { "quality" }, new string[] { "95" });
		
				rp.Save(e.Target.Destination, e.Target.FileType);
			}
		}
	}
}
