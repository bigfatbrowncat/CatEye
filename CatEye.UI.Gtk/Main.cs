using System;
using System.Collections.Generic;
using Gtk;
using CatEye.Core;
using CatEye.UI.Base;
using CatEye.UI.Gtk;
using CatEye.UI.Gtk.Widgets;

namespace CatEye
{
	class MainClass
	{
		public static string APP_NAME = "CatEye";

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
		
		public static string[] FindRawsForCEStage(string cestage_filename)
		{
			List<string> res = new List<string>();
			string cut_ext_low = System.IO.Path.GetFileNameWithoutExtension(cestage_filename).ToLower();
			string dir = System.IO.Path.GetDirectoryName(cestage_filename);
			
			// "Empty" path points to current dir
			if (dir == "") dir = System.IO.Directory.GetCurrentDirectory();
			
			string[] files = System.IO.Directory.GetFiles(dir);
			for (int i = 0; i < files.Length; i++)
			{
				if (DCRawConnection.IsRaw(files[i]) &&
					System.IO.Path.GetFileNameWithoutExtension(files[i]).ToLower() == cut_ext_low)
				{
					res.Add(files[i]);
				}
			}
#if DEBUG
			Console.WriteLine("Found " + res.Count + " RAW files");
#endif
			return res.ToArray();
		}
		
		public static void AddToQueue(Stage stage, string src, string dest, string dest_type)
		{
			Stage stg = new Stage(StageOperationFactory, 
				StageOperationParametersFactoryFromID,
				ImageLoader);
			
			for (int i = 0; i < stage.StageQueue.Length; i++)
			{
				stg.Add((StageOperationParameters)stage.StageQueue[i].Clone());
			}
			
			rq.Add(stg, src, dest, dest_type);
		}
		
		public static void Main (string[] args)
		{
			Application.Init ();
			
			// Initializing rendering queue and it's window
			rq = new RenderingQueue();
			rq.ItemRendering += HandleRqItemRendering;
			
			rqwin = new RenderingQueueWindow(rq);
			rqwin.Visible = false;

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
			
			bool just_started = true;
			
			while (!mQuitFlag)
			{
				if (just_started)
				{
					just_started = false;
					// Parsing command line arguments
					if (args.Length > 0)
					{
						if (args.Length == 1)
						{
							bool exists = System.IO.File.Exists(args[0]);
							bool is_cestage = System.IO.Path.GetExtension(args[0]).ToLower() == ".cestage";
							bool is_raw = DCRawConnection.IsRaw(args[0]);
							if (!exists)
							{
								Gtk.MessageDialog md = new Gtk.MessageDialog(win, DialogFlags.Modal,
								                                             MessageType.Error, ButtonsType.Ok, 
								                                             "Can not find file \"{0}\".", args[0]);
								md.Title = APP_NAME;
								md.Run();
								md.Destroy();
							}
							else if (is_cestage)
							{
								// Loading cestage
								stage.LoadStage(args[0]);
								
								// Trying to find a proper RAW file
								string[] raws = FindRawsForCEStage(args[0]);
								
								if (raws.Length > 0)
								{
									string raw_file = raws[0]; // TODO: support more than one found RAW file
									
									Gtk.MessageDialog ask_raw = new Gtk.MessageDialog(win, DialogFlags.Modal,
									                                             MessageType.Question, ButtonsType.YesNo, 
									                                             "The raw file \"{0}\" found in the same folder.\nWould you like to open it?", 
																				 System.IO.Path.GetFileName(raw_file));
									ask_raw.Title = APP_NAME;
									bool yes = false;
									if (ask_raw.Run() == (int)Gtk.ResponseType.Yes) yes = true;
									ask_raw.Destroy();
									
									if (yes) stage.LoadImage(raw_file, Stage.PreScale);
								}
							}
							else if (is_raw)
							{
								// TODO: handle raw file in command-line arguments
							}
							else // exists, but neither cestage nor raw
							{
								Gtk.MessageDialog md = new Gtk.MessageDialog(win, DialogFlags.Modal,
								                                             MessageType.Error, ButtonsType.Ok, 
								                                             "Incorrect file \"{0}\".", args[0]);
								md.Title = APP_NAME;
								md.Run();
								md.Destroy();
							}
						}
					}
					
				}
				
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
