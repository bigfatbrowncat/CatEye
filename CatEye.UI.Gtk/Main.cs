using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		
		public static string[] FindCEStagesForRaw(string raw_filename)
		{
			List<string> res = new List<string>();
			string cut_ext_low = System.IO.Path.GetFileNameWithoutExtension(raw_filename).ToLower();
			string dir = System.IO.Path.GetDirectoryName(raw_filename);
			
			// "Empty" path points to current dir
			if (dir == "") dir = System.IO.Directory.GetCurrentDirectory();
			
			string[] files = System.IO.Directory.GetFiles(dir);
			for (int i = 0; i < files.Length; i++)
			{
				if (IsCEStageFile(files[i]) &&
					System.IO.Path.GetFileNameWithoutExtension(files[i]).ToLower() == cut_ext_low)
				{
					res.Add(files[i]);
				}
			}
#if DEBUG
			Console.WriteLine("Found " + res.Count + " cestage files");
#endif
			return res.ToArray();
		}
		
		public static bool IsCEStageFile(string filename)
		{
			return System.IO.Path.GetExtension(filename).ToLower() == "cestage";
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
		
		public static bool FindRawsForCestageAndAskToOpen(string cestage_filename, out string raw_filename, ref int prescale)
		{
			raw_filename = "";
			// Trying to find a proper RAW file
			string[] raws = FindRawsForCEStage(cestage_filename);
			
			if (raws.Length > 0)
			{
				raw_filename = raws[0]; // TODO: support more than one found RAW file
				
				AskToOpenRawDialog ask_raw = new AskToOpenRawDialog();
				ask_raw.Filename = raw_filename;
				ask_raw.PreScale = prescale;
				
				bool yes = false;
				if (ask_raw.Run() == (int)Gtk.ResponseType.Accept) yes = true;
				prescale = ask_raw.PreScale;
				ask_raw.Destroy();
				return yes;
			}
			return false;
		}
		
		private static List<StageEditorWindow> mStageEditorWindows = new List<StageEditorWindow>();
		private static RenderingQueue mRenderingQueue;
		private static RenderingQueueWindow mRenderingQueueWindow;
		private static RemoteControlService mRemoteControlService;
		
		public static List<StageEditorWindow> StageEditorWindows
		{
			get { return mStageEditorWindows; }
		}

		public static RemoteControlService RemoteControlService
		{
			get { return mRemoteControlService; }
		}
		
		public static RenderingQueueWindow RenderingQueueWindow
		{
			get { return mRenderingQueueWindow; }
		}
		public static RenderingQueue RenderingQueue
		{
			get { return mRenderingQueue; }
		}
		
		static string CheckIfFileExistsAndAddIndex(string filename)
		{
			if (System.IO.File.Exists(filename))
			{
				string base_name = System.IO.Path.GetFileNameWithoutExtension(filename);
				string ext = System.IO.Path.GetExtension(filename);
				int i;
				for (i = 1; System.IO.File.Exists(base_name + "[" + i.ToString() + "]." + ext); i++) {}
				return base_name + "[" + i.ToString() + "]." + ext;
			}
			else
				return filename;
				
		}
		
		static void HandleRemoteControlServiceRemoteCommandReceived (object sender, RemoteCommandEventArgs e)
		{
			if (e.Command == "StageEditor")
			{
				// No arguments
				Application.Invoke(delegate
				{
					
					StageEditorWindow sew = new StageEditorWindow(
						mStageOperationTypes,
						StageOperationFactory, 
						StageOperationParametersFactory,
						StageOperationParametersEditorFactory, 
						StageOperationHolderFactory, 
						FloatBitmapGtkFactory);
					
					mStageEditorWindows.Add(sew);
					sew.Show();
				});
			}
			if (e.Command == "AddToQueue_StageData")
			{
				string cestageData = e.Arguments[0];		// XML-serialized .cestage
				string rawFileName = e.Arguments[1];		// Raw filename
				string targetFileName = e.Arguments[2];		// target file name
				string targetType = e.Arguments[3];			// target file type (jpeg, png, bmp)
				int Prescale = int.Parse(e.Arguments[4]);	// prescale value
				Application.Invoke(delegate
				{
					Stage stage = new Stage(StageOperationFactory, 
						StageOperationParametersFactory,
						FloatBitmapGtkFactory);
					
					stage.LoadStageFromString(cestageData);
					
					mRenderingQueue.Add(stage, rawFileName, Prescale, targetFileName, targetType);
					mRenderingQueueWindow.Show();
				});
			}
		}
		
		private static bool mLoadedSomethingAlready = false;
		public static void Main(string[] args)
		{
			// Initializing remote control
			mRemoteControlService = new RemoteControlService("localhost", 21985);
			mRemoteControlService.RemoteCommandReceived += HandleRemoteControlServiceRemoteCommandReceived;
			
			// Parsing command line. Formulating command and it's arguments
			List<string> argslist = new List<string>(args);
			
			List<string> command = new List<string>();
			List<List<string>> commands_arguments = new List<List<string>>();

			if (argslist.Count == 0)
			{
				// No command line arguments passed. 
				// Sending the "StageEditor" command with no arguments
				command.Add("StageEditor");
				commands_arguments.Add(new List<string>());
			}
			else if (argslist.Contains("-q") || argslist.Contains("--queue"))
			{
				// Queue launch mode
				
				// Searching for "--default" option
				int d_inx = -1;
				d_inx = argslist.IndexOf("-d") >= 0 ? argslist.IndexOf("-d") : d_inx;
				d_inx = argslist.IndexOf("--default") >= 0 ? argslist.IndexOf("--default") : d_inx;
				string d_cestage_name = "";
				
				if (d_inx >= 0)
				{
					if (d_inx < argslist.Count - 1)
					{
						d_cestage_name = argslist[d_inx + 1];
						// Removing readed "-d"
						argslist.RemoveRange(d_inx, 2);
						
						if (!IsCEStageFile(d_cestage_name))
						{
							Console.WriteLine("Incorrect argument: " + d_cestage_name + " is not a .cestage file.");
							d_cestage_name = "";
						}
						
					}
					if (d_inx == argslist.Count - 1)
					{
						Console.WriteLine("Incorrect argument: .cestage file name should be provided after --default or -d");
						argslist.RemoveAt(d_inx);
					}
				}
				
				// Searching for "--target-type"
				int tt_inx = -1;
				tt_inx = argslist.IndexOf("-t") >= 0 ? argslist.IndexOf("-t") : tt_inx;
				tt_inx = argslist.IndexOf("--target-type") >= 0 ? argslist.IndexOf("--target-type") : tt_inx;
				string target_type = "";
				
				if (tt_inx >= 0)
				{
					if (tt_inx < argslist.Count - 1)
					{
						target_type = argslist[d_inx + 1];
						// Removing readed "-t"
						argslist.RemoveRange(d_inx, 2);
						
						if (target_type != "jpeg" && target_type != "png" && target_type != "bmp")
						{
							Console.WriteLine("Incorrect target type specified: " + target_type + ". It should be \"jpeg\", \"png\" or \"bmp\".");
							target_type = "";
						}
						
					}
					if (d_inx == argslist.Count - 1)
					{
						Console.WriteLine("Incorrect argument: target type should be provided after --target-type or -t");
						argslist.RemoveAt(d_inx);
					}
				}
				
				// Searching for "--prescale"
				int p_inx = -1;
				p_inx = argslist.IndexOf("-p") >= 0 ? argslist.IndexOf("-p") : p_inx;
				p_inx = argslist.IndexOf("--prescale") >= 0 ? argslist.IndexOf("--prescale") : p_inx;
				int prescale = 2;
				
				if (p_inx >= 0)
				{
					if (p_inx < argslist.Count - 1)
					{
						// Removing readed "-t"
						argslist.RemoveRange(d_inx, 2);
						
						if (!int.TryParse(argslist[d_inx + 1], out prescale) || prescale < 1 || prescale > 8)
						{
							Console.WriteLine("Incorrect prescale value specified: " + argslist[d_inx + 1] + ". It should be an integer value from 1 to 8.");
						}
						
					}
					if (d_inx == argslist.Count - 1)
					{
						Console.WriteLine("Incorrect argument: prescale should be provided after --prescale or -p");
						argslist.RemoveAt(d_inx);
					}
				}
				
				// Now when all the additional parameters are parsed and removed, 
				// we're analysing, what's left. The options:
				if (argslist.Count == 2 && (IsCEStageFile(argslist[0]) && DCRawConnection.IsRaw(argslist[1])) ||
										   (IsCEStageFile(argslist[1]) && DCRawConnection.IsRaw(argslist[0])))
				{
					// Two file names: one cestage and one raw

					string cestage_filename = "";
					string raw_filename = "";
					if (IsCEStageFile(argslist[0]) && DCRawConnection.IsRaw(argslist[1]))
					{
						cestage_filename = argslist[0];
						raw_filename = argslist[1];
					}
					else if (IsCEStageFile(argslist[1]) && DCRawConnection.IsRaw(argslist[0]))
					{
						cestage_filename = argslist[1];
						raw_filename = argslist[0];
					}
					
					// Guessing target filename
					if (target_type == "") target_type = "jpeg";
					string target_name = System.IO.Path.ChangeExtension(argslist[1], target_type);
					target_name = CheckIfFileExistsAndAddIndex(target_name);
					
					// Launching StageEditor with the cestage file and the raw file
					command.Add("AddToQueue");
					commands_arguments.Add(new List<string>(new string[] 
					{
						argslist[0], 
						argslist[1], 
						target_name, 
						target_type, 
						prescale.ToString()
					}));
				}
				else
				{
					// Searching for cestage for each raw and for raws for each cestage
					for (int i = 0; i < argslist.Count; i++)
					{
						if (DCRawConnection.IsRaw(argslist[i]))
						{
							// The current file is a raw

							// Guessing target filename
							if (target_type == "") target_type = "jpeg";
							string target_name = System.IO.Path.ChangeExtension(argslist[i], target_type);
							target_name = CheckIfFileExistsAndAddIndex(target_name);
							
							string[] cestages = FindCEStagesForRaw(argslist[i]);
							if (cestages.Length > 0)
							{
								// At least one found

								// Launching StageEditor with the cestage file and the raw file
								command.Add("AddToQueue");
								commands_arguments.Add(new List<string>(new string[] 
								{
									cestages[0], 
									argslist[i], 
									target_name, 
									target_type, 
									prescale.ToString()
								}));
							}
							else if (d_cestage_name != "")
							{
								// Nothing found, but default name is present
								command.Add("AddToQueue");
								commands_arguments.Add(new List<string>(new string[] 
								{
									d_cestage_name,
									argslist[i], 
									target_name, 
									target_type, 
									prescale.ToString()
								}));
							}
							else
							{
								Console.WriteLine("Can't open " + argslist[i] + ": can't find it's .cestage file");
							}
						} 
						else if (IsCEStageFile(argslist[i]))
						{
							// The current file is a cestage

							// Guessing target filename
							if (target_type == "") target_type = "jpeg";
							string target_name = System.IO.Path.ChangeExtension(argslist[i], target_type);
							target_name = CheckIfFileExistsAndAddIndex(target_name);
							
							string[] raws = FindRawsForCEStage(argslist[i]);
							if (raws.Length > 0)
							{
								// At least one found

								command.Add("AddToQueue");
								commands_arguments.Add(new List<string>(new string[] 
								{
									argslist[i],
									raws[0], 
									target_name, 
									target_type, 
									prescale.ToString()
								}));

							}
							else
							{
								Console.WriteLine("Can't open " + argslist[i] + ": can't find it's raw file");
							}
						}

					}
					
				}
				
			}
			else
			{
				// Not a queue launch mode
				if (argslist.Count == 2 && IsCEStageFile(argslist[0]) && DCRawConnection.IsRaw(argslist[1]))
				{
					// Launching StageEditor with the cestage file and the raw file
					command.Add("StageEditor_CEStage_RAW");
					commands_arguments.Add(new List<string>(new string[] 
					{ 
						argslist[0], 
						argslist[1]
					}));
				}
				else
				if (argslist.Count == 2 && IsCEStageFile(argslist[1]) && DCRawConnection.IsRaw(argslist[0]))
				{
					// Launching StageEditor with the cestage file and the raw file
					command.Add("StageEditor_CEStage_RAW");
					commands_arguments.Add(new List<string>(new string[] 
					{ 
						argslist[1], 
						argslist[0]
					}));
				}
				else
				{
					// If we don't have 1 cestage and 1 raw, let's open as many windows as possible.
					// But, at first, trying to find "--default" option
					int d_inx = -1;
					d_inx = argslist.IndexOf("-d") >= 0 ? argslist.IndexOf("-d") : d_inx;
					d_inx = argslist.IndexOf("--default") >= 0 ? argslist.IndexOf("--default") : d_inx;
					string d_cestage_name = "";
					
					if (d_inx >= 0)
					{
						if (d_inx < argslist.Count - 1)
						{
							d_cestage_name = argslist[d_inx + 1];
							// Removing readed "-d"
							argslist.RemoveRange(d_inx, 2);
							
							if (!IsCEStageFile(d_cestage_name))
							{
								Console.WriteLine("Incorrect argument: " + d_cestage_name + " is not a .cestage file.");
								d_cestage_name = "";
							}
							
						}
						if (d_inx == argslist.Count - 1)
						{
							Console.WriteLine("Incorrect argument: .cestage file name should be provided after --default or -d");
							argslist.RemoveAt(d_inx);
						}
					}
					
					// Searching for cestage for each raw and for raws for each cestage
					for (int i = 0; i < argslist.Count; i++)
					{
						if (DCRawConnection.IsRaw(argslist[i]))
						{
							// The current file is a raw
							
							string[] cestages = FindCEStagesForRaw(argslist[i]);
							if (cestages.Length > 0)
							{
								// At least one found
								// Launching StageEditor with the cestage file and the raw file
								command.Add("StageEditor_CEStage_RAW");
								commands_arguments.Add(new List<string>(new string[] 
								{ 								
									cestages[0],
									argslist[i]
								}));
							}
							else if (d_cestage_name != "")
							{
								// Nothing found, but default name is present
								command.Add("StageEditor_CEStage_RAW");
								commands_arguments.Add(new List<string>(new string[] 
								{
									d_cestage_name,
									argslist[i]
								}));
							}
							else
							{
								Console.WriteLine("Can't open " + argslist[i] + ": can't find it's .cestage file");
							}
						} 
						else if (IsCEStageFile(argslist[i]))
						{
							// The current file is a cestage
							
							string[] raws = FindRawsForCEStage(argslist[i]);
							if (raws.Length > 0)
							{
								// At least one found
								// Launching StageEditor with the cestage file and the raw file
								command.Add("StageEditor_CEStage_RAW");
								commands_arguments.Add(new List<string>(new string[] 
								{
									argslist[i],
									raws[0]
								}));
							}
							else
							{
								Console.WriteLine("Can't open " + argslist[i] + ": can't find it's raw file");
							}
						}

					}
				}
			}
				
			
			
			if (mRemoteControlService.Start())
			{
				Application.Init ();
				
				// Creating queue and it's window
				mRenderingQueue = new RenderingQueue();
				mRenderingQueueWindow = new RenderingQueueWindow(mRenderingQueue);
				mRenderingQueue.StartThread();
				
				GLib.Idle.Add(delegate {
					// Checking if something is already started
					if (RenderingQueue.IsProcessingItem || StageEditorWindows.Count > 0) 
						mLoadedSomethingAlready = true;
					
					// Removing all destroyed
					StageEditorWindows.RemoveAll(delegate (StageEditorWindow wnd)
					{
						return wnd.IsDestroyed;
					});
					
					// Checking is there any activity or no
					if (mLoadedSomethingAlready && 
						StageEditorWindows.Count == 0 &&
						!RenderingQueue.IsProcessingItem) 
					{
						RenderingQueue.StopThread();
						Application.Quit();
					}
					return true;
				});
				
				Application.Run();
			}
			mRemoteControlService.Stop();
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
