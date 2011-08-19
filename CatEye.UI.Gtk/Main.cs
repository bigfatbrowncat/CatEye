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

		public static StageEditorWindow win;

		private static bool mQuitFlag = false;
		private static int mDelayBeforeUpdate = 100;

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
		public static StageOperationParameters StageOperationParametersFactory(string id)
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
		
		public static IBitmapCore FloatBitmapGtkFactory(PPMLoader ppl, ProgressReporter callback)
		{
			return FloatBitmapGtk.FromPPM(ppl, callback);
		}
		
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
		
		public static bool FindRawsForCestageAndAskToOpen(string cestage_filename, out string raw_filename, out int prescale)
		{
			raw_filename = ""; prescale = 2;
			// Trying to find a proper RAW file
			string[] raws = FindRawsForCEStage(cestage_filename);
			
			if (raws.Length > 0)
			{
				raw_filename = raws[0]; // TODO: support more than one found RAW file
				
				AskToOpenRawDialog ask_raw = new AskToOpenRawDialog();
				ask_raw.Filename = raw_filename;
				
				/*Gtk.MessageDialog ask_raw = new Gtk.MessageDialog(win, DialogFlags.Modal,
				                                             MessageType.Question, ButtonsType.YesNo, 
				                                             "The raw file \"{0}\" found in the same folder.\nWould you like to open it?", 
															 System.IO.Path.GetFileName(raw_file));*/
				//ask_raw.Title = APP_NAME;
				bool yes = false;
				if (ask_raw.Run() == (int)Gtk.ResponseType.Yes) yes = true;
				prescale = ask_raw.Prescale;
				ask_raw.Destroy();
				return yes;
			}
			return false;
		}
		
		public static void Main(string[] args)
		{
			Application.Init ();
			
		}
		
/* OLD MAIN 		
		public static void Main (string[] args)
		{
			Application.Init ();
			
			List<string> argslist = new List<string>(args);
			if (argslist.Count > 0 && (argslist[0] == "--queue" || argslist[0] == "-q"))
			{
				bool queue_server_created = RemotingObject.RunQueueServiceOrConnectToIt();
				
				if (argslist.Count > 1)
				{
					// Trying to load something
					
					// Options
					string cestage_filename = null, raw_filename = null, output_filename = null, type = "jpeg";
					int prescale = 2;
					
					int inx = argslist.IndexOf("--cestage");
					if (inx > -1)
					{
						try
						{
							cestage_filename = argslist[inx + 1];
						}
						catch (Exception)
						{
							Console.WriteLine("Incorrect cestage argument. It should be \"--cestage <file_name>\"");
							return;
						}
					}
					inx = argslist.IndexOf("--raw");
					if (inx > -1)
					{
						try
						{
							raw_filename = argslist[inx + 1];
						}
						catch (Exception)
						{
							Console.WriteLine("Incorrect raw argument. It should be \"--raw <file_name>\"");
							return;
						}
					}
					inx = argslist.IndexOf("--output");
					if (inx > -1)
					{
						try
						{
							output_filename = argslist[inx + 1];
						}
						catch (Exception)
						{
							Console.WriteLine("Incorrect output argument. It should be \"--output <file_name>\"");
							return;
						}
					}
					inx = argslist.IndexOf("--type");
					if (inx > -1)
					{
						try
						{
							type = argslist[inx + 1];
						}
						catch (Exception)
						{
							Console.WriteLine("Incorrect type argument. It should be \"--type (jpeg, png, bmp)\"");
							return;
						}
					}
					inx = argslist.IndexOf("--prescale");
					if (inx > -1)
					{
						try
						{
							prescale = int.Parse(argslist[inx + 1]);
							if (prescale < 1 || prescale > 10) throw new Exception();
						}
						catch (Exception)
						{
							Console.WriteLine("Incorrect prescale argument. It should be a positive integer lower or equal than 10");
							return;
						}
					}
					
					// Guessing missed arguments
					if (cestage_filename == null && raw_filename != null)
					{
						cestage_filename = System.IO.Path.ChangeExtension(raw_filename, ".cestage");
					}
					if (raw_filename == null && cestage_filename != null)
					{
						try
						{
							raw_filename = FindRawsForCEStage(cestage_filename)[0];
						}
						catch (Exception)
						{
							Console.WriteLine("No RAW files found for " + cestage_filename);
							return;
						}
					}
					if (output_filename == null && cestage_filename != null)
					{
						output_filename = System.IO.Path.ChangeExtension(cestage_filename, ".jpeg");
					}
					
					if (cestage_filename == null || raw_filename == null || output_filename == null)
					{
						Console.WriteLine("Incorrect parameters");
						return;
					}

					// Initializing rendering queue and it's window
					
					Stage stage = new Stage(StageOperationFactory, StageOperationParametersFactoryFromID, FloatBitmapGtkFactory);
					stage.LoadStage(cestage_filename);
					RemotingObject.rob.rq.Add(stage, raw_filename, prescale, output_filename, type);
				}
				if (queue_server_created)
				{
					
					while (!mQuitFlag)
					{
						Gtk.Application.RunIteration();
					}
				}
			}
			else
			{	
				// ** Starting main editor window **
				
				// Initializing stage and main window
				ExtendedStage stage = new ExtendedStage(
					StageOperationFactory, 
					StageOperationParametersFactoryFromID,
					StageOperationParametersEditorFactory, 
					StageOperationHolderFactory, 
					FloatBitmapGtkFactory);
				
			
				DateTime lastUpdateQueuedTime = DateTime.Now;
				
				stage.UpdateQueued += delegate {
					lastUpdateQueuedTime = DateTime.Now;
				};
				
				win = new StageEditorWindow (stage, mStageOperationTypes);
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
									
									string raw_filename; int prescale;
									if (FindRawsForCestageAndAskToOpen(args[0], out raw_filename, out prescale))
									{
										stage.LoadImage(raw_filename, prescale);
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
		}
*/

	}
}
