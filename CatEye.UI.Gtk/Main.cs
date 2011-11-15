using System;
using System.IO;
using System.Text;
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
			typeof(HighlightStageOperation),
			typeof(CompressSharpStageOperation),
			typeof(BrightnessStageOperation),
			typeof(SaturationStageOperation),
			typeof(ToneStageOperation),
			typeof(BlackPointStageOperation),		// Incomplete
			//typeof(LimitSizeStageOperation),		// Incomplete
			typeof(CrotateStageOperation),
			//typeof(HistogramStageOperation),		// Incomplete
		};

		public static readonly Type[] mStageOperationParametersTypes = new Type[]
		{
			typeof(HighlightStageOperationParameters),
			typeof(CompressSharpStageOperationParameters),
			typeof(BrightnessStageOperationParameters),
			typeof(SaturationStageOperationParameters),
			typeof(ToneStageOperationParameters),
			typeof(BlackPointStageOperationParameters),		// Incomplete
			//typeof(LimitSizeStageOperationParameters),		// Incomplete
			typeof(CrotateStageOperationParameters),
			//typeof(HistogramStageOperationParameters),		// Incomplete
		};
				
		public static readonly Type[] mStageOperationParametersWidgetTypes = new Type[]
		{
			typeof(HighlightStageOperationParametersWidget),
			typeof(BrightnessStageOperationParametersWidget),
			typeof(CompressSharpStageOperationParametersWidget),
			typeof(SaturationStageOperationParametersWidget),
			typeof(ToneStageOperationParametersWidget),
			typeof(BlackPointStageOperationParametersWidget),	// Incomplete
			//typeof(LimitSizeStageOperationParametersWidget),	// Incomplete
			typeof(CrotateStageOperationParametersWidget),
			//typeof(HistogramStageOperationParametersWidget),	// Incomplete
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
		
		public static IBitmapCore FloatBitmapGtkFactory(RawLoader ppl, ProgressReporter callback)
		{
			return FloatBitmapGtk.FromPPM(ppl, callback);
		}
		
		internal static WindowsGtkStyle windowsGtkStyle;
		
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
				string path = System.IO.Path.GetDirectoryName(filename);
				string base_name = System.IO.Path.GetFileNameWithoutExtension(filename);
				string ext = System.IO.Path.GetExtension(filename);
				int i;
				for (i = 1; System.IO.File.Exists(path + System.IO.Path.DirectorySeparatorChar + base_name + "[" + i.ToString() + "]" + ext); i++) {}
				return path + System.IO.Path.DirectorySeparatorChar + base_name + "[" + i.ToString() + "]" + ext;
			}
			else
				return filename;
				
		}
		
		static void HandleRemoteControlServiceRemoteCommandReceived (object sender, RemoteCommandEventArgs e)
		{
#if DEBUG
			Console.WriteLine("[S] Remote command arrived. Handling it:");
#endif
			if (e.Command == "StageEditor")
			{
#if DEBUG
				Console.WriteLine("Command: StageEditor ( no arguments )");
#endif
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
					
					if (sew.ShowLoadImageDialog())
						sew.Show();
					else
						sew.CloseStageEditor();
				});
			} 
			else if (e.Command == "StageEditor_CEStage_RAW")
			{
				string cestageFileName = e.Arguments[0];		// .cestage filename
				string rawFileName = e.Arguments[1];			// Raw filename
				int prescale = int.Parse(e.Arguments[2]);		// Prescale

#if DEBUG
				Console.WriteLine("Command: StageEditor_CEStage_RAW ( " + 
					"cestageFileName = " + cestageFileName + ", " +
					"rawFileName = " + rawFileName + ", " + 
					"prescale = " + prescale.ToString() + " )");
#endif
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
					sew.LoadRaw(rawFileName, prescale);
					//sew.LoadCEStage(cestageFileName, false);
				});
			}
			else if (e.Command == "StageEditor_CEStage")
			{
				string cestageFileName = e.Arguments[0];		// .cestage filename

#if DEBUG
				Console.WriteLine("Command: StageEditor_CEStage ( " + 
					"cestageFileName = " + cestageFileName + " )");
#endif
				
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
					//sew.LoadCEStage(cestageFileName, true);
				});
			}
			else if (e.Command == "StageEditor_RAW")
			{
				string rawFileName = e.Arguments[0];			// Raw filename
				int prescale = int.Parse(e.Arguments[1]);		// Prescale

#if DEBUG
				Console.WriteLine("Command: StageEditor_RAW ( " + 
					"rawFileName = " + rawFileName + ", " + 
					"prescale = " + prescale.ToString() + " )");
#endif
				
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
					sew.LoadRaw(rawFileName, prescale);
				});
			}
			else if (e.Command == "AddToQueue_StageData")
			{
				string cestageData = e.Arguments[0];		// XML-serialized .cestage
				string rawFileName = e.Arguments[1];		// Raw filename
				string targetFileName = e.Arguments[2];		// target file name
				string targetType = e.Arguments[3];			// target file type (jpeg, png, bmp)
				int prescale = int.Parse(e.Arguments[4]);	// prescale value

#if DEBUG
				Console.WriteLine("Command: AddToQueue_StageData ( " + 
					"cestageData = " + cestageData + ", " +
					"rawFileName = " + rawFileName + ", " + 
					"targetFileName = " + targetFileName + ", " + 
					"targetType = " + targetType + ", " + 
					"prescale = " + prescale.ToString() + " )");
#endif
				
				Application.Invoke(delegate
				{
					Stage stage = new Stage(StageOperationFactory, 
						StageOperationParametersFactory,
						FloatBitmapGtkFactory);
					
					stage.LoadStageFromString(cestageData);
					
					mRenderingQueue.Add(stage, rawFileName, prescale, targetFileName, targetType);
					mRenderingQueueWindow.Show();
				});
			}
			else if (e.Command == "AddToQueue")
			{
				string cestageFileName = e.Arguments[0];		// .cestage filename
				string rawFileName = e.Arguments[1];		// Raw filename
				string targetFileName = e.Arguments[2];		// target file name
				string targetType = e.Arguments[3];			// target file type (jpeg, png, bmp)
				int prescale = int.Parse(e.Arguments[4]);	// prescale value

#if DEBUG
				Console.WriteLine("Command: AddToQueue ( " + 
					"cestageFileName = " + cestageFileName + ", " +
					"rawFileName = " + rawFileName + ", " + 
					"targetFileName = " + targetFileName + ", " + 
					"targetType = " + targetType + ", " + 
					"prescale = " + prescale.ToString() + " )");
#endif
				Application.Invoke(delegate
				{
					Stage stage = new Stage(StageOperationFactory, 
						StageOperationParametersFactory,
						FloatBitmapGtkFactory);
					
					stage.LoadStage(cestageFileName);
					
					mRenderingQueue.Add(stage, rawFileName, prescale, targetFileName, targetType);
					mRenderingQueueWindow.Show();
				});
			}
			else
			{
#if DEBUG
				Console.Write("Command: " + e.Command + " ( ");
				for (int i = 0; i < e.Arguments.Length - 1; i++)
				{
					Console.Write("\"" + e.Arguments[i] + "\", ");
				}
				if (e.Arguments.Length > 0) 
					Console.Write("\"" + e.Arguments[e.Arguments.Length - 1] + "\"");
				Console.WriteLine(" )");
#endif
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
			
			List<string> commands = new List<string>();
			List<List<string>> commands_arguments = new List<List<string>>();

			if (argslist.Contains("-q") || argslist.Contains("--queue"))
			{
				argslist.Remove("-q"); argslist.Remove("--queue");
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
						
						if (!ReceiptsManager.IsReceipt(d_cestage_name))
						{
							Console.WriteLine("Incorrect argument: " + d_cestage_name + " is not a .cestage file.");
							d_cestage_name = "";
						}
						
					} else if (d_inx == argslist.Count - 1)
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
						target_type = argslist[tt_inx + 1];
						// Removing readed "-t"
						argslist.RemoveRange(tt_inx, 2);
						
						if (target_type != "jpeg" && target_type != "png" && target_type != "bmp")
						{
							Console.WriteLine("Incorrect target type specified: " + target_type + ". It should be \"jpeg\", \"png\" or \"bmp\".");
							target_type = "";
						}
						
					} else if (tt_inx == argslist.Count - 1)
					{
						Console.WriteLine("Incorrect argument: target type should be provided after --target-type or -t");
						argslist.RemoveAt(tt_inx);
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
						if (!int.TryParse(argslist[p_inx + 1], out prescale) || prescale < 1 || prescale > 8)
						{
							Console.WriteLine("Incorrect prescale value specified: " + argslist[p_inx + 1] + ". It should be an integer value from 1 to 8.");
						}

						// Removing readed "-t"
						argslist.RemoveRange(p_inx, 2);
						
					} else if (p_inx == argslist.Count - 1)
					{
						Console.WriteLine("Incorrect argument: prescale should be provided after --prescale or -p");
						argslist.RemoveAt(p_inx);
					}
				}
				
				// Now when all the additional parameters are parsed and removed, 
				// we're analysing, what's left. The options:
				if (argslist.Count == 2 && ((ReceiptsManager.IsReceipt(argslist[0]) && RawLoader.IsRaw(argslist[1])) ||
										   (ReceiptsManager.IsReceipt(argslist[1]) && RawLoader.IsRaw(argslist[0]))))
				{
					// Two file names: one cestage and one raw

					string cestage_filename;
					string raw_filename;
					if (ReceiptsManager.IsReceipt(argslist[0]) && RawLoader.IsRaw(argslist[1]))
					{
						cestage_filename = argslist[0];
						raw_filename = argslist[1];
					}
					else // if (IsCEStageFile(argslist[1]) && DCRawConnection.IsRaw(argslist[0]))
					{
						cestage_filename = argslist[1];
						raw_filename = argslist[0];
					}
					
					// Guessing target filename
					if (target_type == "") target_type = "jpeg";
					string target_name = System.IO.Path.ChangeExtension(raw_filename, target_type);
					target_name = CheckIfFileExistsAndAddIndex(target_name);
					
					// Launching StageEditor with the cestage file and the raw file
					commands.Add("AddToQueue");
					commands_arguments.Add(new List<string>(new string[] 
					{
						cestage_filename, 
						raw_filename, 
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
						if (RawLoader.IsRaw(argslist[i]))
						{
							// The current file is a raw

							// Guessing target filename
							if (target_type == "") target_type = "jpeg";
							string target_name = System.IO.Path.ChangeExtension(argslist[i], target_type);
							target_name = CheckIfFileExistsAndAddIndex(target_name);
							
							string[] cestages = ReceiptsManager.FindReceiptsForRaw(argslist[i]);
							if (cestages.Length > 0)
							{
								// At least one found

								// Launching StageEditor with the cestage file and the raw file
								commands.Add("AddToQueue");
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
								commands.Add("AddToQueue");
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
						else if (ReceiptsManager.IsReceipt(argslist[i]))
						{
							// The current file is a cestage

							// Guessing target filename
							if (target_type == "") target_type = "jpeg";
							string target_name = System.IO.Path.ChangeExtension(argslist[i], target_type);
							target_name = CheckIfFileExistsAndAddIndex(target_name);
							
							string[] raws = new string[] {};
							try
							{
								raws = ReceiptsManager.FindRawsForReceipt(argslist[i]);
							}
							catch (System.IO.DirectoryNotFoundException dnfe)
							{
								Console.WriteLine("Error: " + dnfe.Message);			
							}
							if (raws.Length > 0)
							{
								// At least one found

								commands.Add("AddToQueue");
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
						
						if (!ReceiptsManager.IsReceipt(d_cestage_name))
						{
							Console.WriteLine("Incorrect argument: " + d_cestage_name + " is not a .cestage file.");
							d_cestage_name = "";
						}
						
					} else if (d_inx == argslist.Count - 1)
					{
						Console.WriteLine("Incorrect argument: .cestage file name should be provided after --default or -d");
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
						if (!int.TryParse(argslist[p_inx + 1], out prescale) || prescale < 1 || prescale > 8)
						{
							Console.WriteLine("Incorrect prescale value specified: " + argslist[p_inx + 1] + ". It should be an integer value from 1 to 8.");
						}

						// Removing readed "-p"
						argslist.RemoveRange(p_inx, 2);
						
					} else if (p_inx == argslist.Count - 1)
					{
						Console.WriteLine("Incorrect argument: prescale should be provided after --prescale or -p");
						argslist.RemoveAt(p_inx);
					}
				}
						
				if (argslist.Count == 2 && ReceiptsManager.IsReceipt(argslist[0]) && RawLoader.IsRaw(argslist[1]))
				{
					// Launching StageEditor with the cestage file and the raw file
					commands.Add("StageEditor_CEStage_RAW");
					commands_arguments.Add(new List<string>(new string[] 
					{ 
						argslist[0],
						argslist[1],
						prescale.ToString()
					}));
				}
				else
				if (argslist.Count == 2 && ReceiptsManager.IsReceipt(argslist[1]) && RawLoader.IsRaw(argslist[0]))
				{
					// Launching StageEditor with the cestage file and the raw file
					commands.Add("StageEditor_CEStage_RAW");
					commands_arguments.Add(new List<string>(new string[] 
					{
						argslist[1],
						argslist[0],
						prescale.ToString()
					}));
				}
				else
				{
					// Searching for cestage for each raw and for raws for each cestage
					for (int i = 0; i < argslist.Count; i++)
					{
						if (RawLoader.IsRaw(argslist[i]))
						{
							// The current file is a raw
							
							if (d_cestage_name != "")
							{
								// Nothing found, but default name is present
								commands.Add("StageEditor_CEStage_RAW");
								commands_arguments.Add(new List<string>(new string[] 
								{
									d_cestage_name,
									argslist[i],
									prescale.ToString()
								}));
							}
							else
							{
								commands.Add("StageEditor_RAW");
								commands_arguments.Add(new List<string>(new string[] 
								{
									argslist[i],
									prescale.ToString()
								}));
							}
						} 
						else if (ReceiptsManager.IsReceipt(argslist[i]))
						{
							// The current file is a cestage
							
							string[] raws = ReceiptsManager.FindRawsForReceipt(argslist[i]);
							if (raws.Length > 0)
							{
								// At least one found
								// Launching StageEditor with the cestage file and the raw file
								commands.Add("StageEditor_CEStage_RAW");
								commands_arguments.Add(new List<string>(new string[] 
								{
									argslist[i],
									raws[0],
									prescale.ToString()
								}));
							}
							else if (raws.Length == 0)
							{
								Gtk.MessageDialog md = new Gtk.MessageDialog(
									null, DialogFlags.Modal,
									MessageType.Error, ButtonsType.Ok, 
									false, "Can't find raw file for {0}", argslist[i]);
								
								md.Title = MainClass.APP_NAME;
								
								md.Run();
								md.Destroy();
							} 
							else // raws.Length > 1
							{
								Gtk.MessageDialog md = new Gtk.MessageDialog(
									null, DialogFlags.Modal,
									MessageType.Error, ButtonsType.Ok, 
									false, "More than one raw file found for {0}", argslist[i]);
								
								md.Title = MainClass.APP_NAME;
								
								md.Run();
								md.Destroy();
							}
						}

					}
				}
			}
				
			bool ownServerStarted = mRemoteControlService.Start();

			if (ownServerStarted)
			{
				string mylocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
				windowsGtkStyle = new WindowsGtkStyle(mylocation + 
				                                      Path.DirectorySeparatorChar + 
				                                      "res" + 
				                                      Path.DirectorySeparatorChar + 
				                                      "win-gtkrc");
				Application.Init ();
				
				// Creating render queue and its window
				mRenderingQueue = new RenderingQueue();
				mRenderingQueueWindow = new RenderingQueueWindow(mRenderingQueue);
				mRenderingQueue.StartThread();
			}

			// No files asked
			if (commands.Count == 0)
			{
				// No command line arguments passed. 
				// Sending the "StageEditor" command with no arguments
				commands.Add("StageEditor");
				commands_arguments.Add(new List<string>());
			}
						
			// Sending the commands
			List<string> packed_commands = new List<string>();
			for (int i = 0; i < commands.Count; i++)
			{
				packed_commands.Add(RemoteControlService.PackCommand(commands[i], commands_arguments[i].ToArray()));
			}
			mRemoteControlService.SendCommands(packed_commands.ToArray());
			
			
			if (ownServerStarted)
			{
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
	}
}
