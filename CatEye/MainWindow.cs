using System;
using System.Xml;
using System.Collections.Generic;
using Gtk;
using CatEye;
using CatEye.Core;

public partial class MainWindow : Gtk.Window
{
	private static string APP_NAME = "CatEye";

	protected enum UIState { Processing, Loading, Free };
	private UIState _TheUIState = UIState.Free;
	private string _FileName = null;
	private int _PreScale = 0;
	
	private void UpdateTitle()
	{
		Title = System.IO.Path.GetFileName(_FileName) + " [prescaled by " + _PreScale + "] — " + APP_NAME;
	}
	
	protected string FileName
	{
		get { return _FileName; }
		set 
		{
			_FileName = value;
			UpdateTitle();
		}
	}
	
	protected int PreScale
	{
		get { return _PreScale; }
		set 
		{
			_PreScale = value;
			UpdateTitle();
		}
	}
	
	protected UIState TheUIState 
	{ 
		get { return _TheUIState; } 
		set
		{
			if (_TheUIState != value)
			{
				_TheUIState = value;
				if (value == MainWindow.UIState.Free)
				{
					loadRawAction.Sensitive = true;
					loadStageAction.Sensitive = true;
					saveStageAsAction.Sensitive = true;
					saveImageAsAction.Sensitive = true;
					cancel_button.Visible = false;
				}
				else if (value == MainWindow.UIState.Loading)
				{
					loadRawAction.Sensitive = false;
					loadStageAction.Sensitive = false;
					saveStageAsAction.Sensitive = false;
					saveImageAsAction.Sensitive = false;
					cancel_button.Visible = true;
					cancel_button.Sensitive = true;
				}
				else if (value == MainWindow.UIState.Processing)
				{
					loadRawAction.Sensitive = true;
					loadStageAction.Sensitive = true;
					saveStageAsAction.Sensitive = true;
					saveImageAsAction.Sensitive = false;
					cancel_button.Visible = false;
				}
			}
		}
	}
	
	public delegate bool ProgressMessageReporter(double progress, string status);

	PPMLoader ppl = null;
	FloatPixmap hdr = null;
	FloatPixmap frozen = null;
	ExtendedStage stages;
	
	Type[] _StageOperationTypes = new Type[]
	{
		typeof(CropStageOperation),
		typeof(CompressionStageOperation),
		typeof(BrightnessStageOperation),
		typeof(UltraSharpStageOperation),
		typeof(SaturationStageOperation),
		typeof(ToneStageOperation),
	};
	
	bool update_timer_launched = false;
	bool cancel_pending = false;
	
	uint update_timer_delay = 500;

	DateTime lastupdate;
	
	private void ArrangeStageOperationBoxes()
	{
		left_vbox.CheckResize();
		stage_vbox.CheckResize();
		GtkScrolledWindow1.CheckResize();
	}
	
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();

		// Creating stage operations and stages
		stages = new ExtendedStage(stage_vbox);

		// Preparing stage operation adding store
		ListStore ls = new ListStore(typeof(string), typeof(int));
		for (int i = 0; i < _StageOperationTypes.Length; i++)
		{
			string desc;
			object[] descs = _StageOperationTypes[i].GetCustomAttributes(
				typeof(StageOperationDescriptionAttribute), true);
			if (descs.Length == 0)
				desc = _StageOperationTypes[i].Name;
			else
				desc = ((StageOperationDescriptionAttribute)descs[0]).Name;
		
			ls.AppendValues(desc, i);
		}
		
		stageOperationToAdd_combobox.Model = ls;
		Gtk.TreeIter ti;
		ls.GetIterFirst(out ti);
		stageOperationToAdd_combobox.SetActiveIter(ti);
		
		// Loading default stage
		string mylocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
		string defaultstage = mylocation + System.IO.Path.DirectorySeparatorChar.ToString() + "default.cestage";
		if (System.IO.File.Exists(defaultstage))
		{
			LoadStage(defaultstage);
		}
		else
		{
			Gtk.MessageDialog md = new Gtk.MessageDialog(this, DialogFlags.Modal,
			                                             MessageType.Warning, ButtonsType.Ok, 
			                                             "Can not find default.cestage");
			md.Title = "Warning";
			md.Run();
			md.Destroy();
		}
		
		// Setting stages events
		
		stages.OperationActivityChanged += delegate {
			LaunchUpdateTimer();
		};
		stages.OperationIndexChanged += delegate {
			LaunchUpdateTimer();
		};
		stages.OperationAddedToStage += delegate {
			LaunchUpdateTimer();
			ArrangeStageOperationBoxes();
		};
		stages.OperationRemovedFromStage += delegate {
			LaunchUpdateTimer();
			ArrangeStageOperationBoxes();
		};
		stages.ViewedOperationChanged += delegate {
			LaunchUpdateTimer();
		};
		stages.OperationFrozen += delegate {
			LaunchUpdateTimer();
			stageOperationAdding_hbox.Sensitive = false;
		};
		stages.OperationDefrozen += delegate {
			stageOperationAdding_hbox.Sensitive = true;
		};
		
		// Adding events for stage operation view redrawing
		ppmviewwidget1.Events |= 
								Gdk.EventMask.PointerMotionMask |
								Gdk.EventMask.ButtonMotionMask |
								Gdk.EventMask.ButtonPressMask |
								Gdk.EventMask.ButtonReleaseMask |
								Gdk.EventMask.Button1MotionMask |
								Gdk.EventMask.ExposureMask;
		
		ppmviewwidget1.ExposeEvent += DrawCurrentlyViewedStageOperationAdditions;
		ppmviewwidget1.ButtonPressEvent += ImageMouseButtonPressed;
		ppmviewwidget1.ButtonReleaseEvent += ImageMouseButtonReleased;
		ppmviewwidget1.MotionNotifyEvent += HandleImageMouseMotion;

		// Arranging boxes
		ArrangeStageOperationBoxes();
		
	}

	void ImageMouseButtonPressed (object o, ButtonPressEventArgs args)
	{
		if (stages.EditingOperation != null)
		{
			double dx = (args.Event.X - ppmviewwidget1.CurrentImagePosition.X) / ppmviewwidget1.CurrentImagePosition.Width;
			double dy = (args.Event.Y - ppmviewwidget1.CurrentImagePosition.Y) / ppmviewwidget1.CurrentImagePosition.Height;

			if (stages.Holders[stages.EditingOperation].OperationParametersWidget.ReportMouseButton(dx, dy, args.Event.Button, true))
			{
				ppmviewwidget1.QueueDraw();
			}
		}
	}

	[GLib.ConnectBefore]
	void ImageMouseButtonReleased (object o, ButtonReleaseEventArgs args)
	{
		if (stages.EditingOperation != null)
		{
			double dx = (args.Event.X - ppmviewwidget1.CurrentImagePosition.X) / ppmviewwidget1.CurrentImagePosition.Width;
			double dy = (args.Event.Y - ppmviewwidget1.CurrentImagePosition.Y) / ppmviewwidget1.CurrentImagePosition.Height;

			if (stages.Holders[stages.EditingOperation].OperationParametersWidget.ReportMouseButton(dx, dy, args.Event.Button, false))
			{
				ppmviewwidget1.QueueDraw();
			}
		}
	}
	
	void HandleImageMouseMotion (object o, MotionNotifyEventArgs args)
	{
		if (stages.EditingOperation != null)
		{
			double dx = (args.Event.X - ppmviewwidget1.CurrentImagePosition.X) / ppmviewwidget1.CurrentImagePosition.Width;
			double dy = (args.Event.Y - ppmviewwidget1.CurrentImagePosition.Y) / ppmviewwidget1.CurrentImagePosition.Height;

			if (stages.Holders[stages.EditingOperation].OperationParametersWidget.ReportMousePosition(dx, dy))
			{
				ppmviewwidget1.QueueDraw();
			}
		}
	}

	void DrawCurrentlyViewedStageOperationAdditions (object o, ExposeEventArgs args)
	{
		if (stages.EditingOperation != null)
		{
			stages.Holders[stages.EditingOperation].OperationParametersWidget.DrawToDrawable(ppmviewwidget1.GdkWindow, 
			                                                       ppmviewwidget1.CurrentImagePosition);
		}
	}

	protected void LaunchUpdateTimer()
	{
		if (!update_timer_launched)
		{
			update_timer_launched = true;
			GLib.Timeout.Add(update_timer_delay, new GLib.TimeoutHandler(delegate {
				update_timer_launched = false;
				switch (TheUIState)
				{
				case UIState.Processing:
					// Already processing. Image processing needs to be interrupted, after 
					// that we have to hit the update timer again
					cancel_pending = true;
					return true;
				
				case UIState.Loading:
					// The image is loading. No refresh allowed. Just ignoring the command
					return false;
					
				case UIState.Free:
					// Updating and stopping the timer
					if (stages.FrozenAt == null)
					{
						frozen = null;
						UpdateStageAfterFrozen();
					}
					else
					{
						if (frozen == null)
						{
							if (UpdateFrozen())
							{
								UpdateStageAfterFrozen();
							}
							else
							{
								Console.WriteLine("Frozen image isn't updated.");
							}
						}
						else
						{
							UpdateStageAfterFrozen();
						}
						
					}
					
					return false;
				default:
					throw new Exception("Unhandled case occured: " + TheUIState);
				}
			}));
		}
	}


	void HandleProgress (object sender, ReportStageOperationProgressEventArgs e)
	{
		progressbar.Fraction = e.Progress;
		object[] attrs = sender.GetType().GetCustomAttributes(typeof(StageOperationDescriptionAttribute), false);
		if (attrs.Length > 0) 
			progressbar.Text = (attrs[0] as StageOperationDescriptionAttribute).Name + ": ";
		progressbar.Text += (e.Progress * 100).ToString("0") + "%";
		
		if (UpdateDuringProcessingAction.Active)
		{
			if ((DateTime.Now - lastupdate).TotalMilliseconds / ppmviewwidget1.UpdateTimeSpan.TotalMilliseconds > 5)
			{
				ppmviewwidget1.UpdatePicture();
				ppmviewwidget1.QueueDraw();
				lastupdate = DateTime.Now;
			}
		}
		
		while (Gtk.Application.EventsPending())
			Gtk.Application.RunIteration();
		
		if (cancel_pending) e.Cancel = true;
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		if (TheUIState != MainWindow.UIState.Free)
			cancel_pending = true;
		Application.Quit ();
		a.RetVal = true;
	}
	
	protected virtual void OnSaveAsActionActivated (object sender, System.EventArgs e)
	{
		Gtk.FileChooserDialog fcd = new Gtk.FileChooserDialog("Save photo", 
		                                                      this, 
		                                                      FileChooserAction.Save,
		                                                      "Cancel", ResponseType.Cancel,
		                                                      "Save", ResponseType.Accept);
		
		FileFilter[] ffs = new FileFilter[3];
		ffs[0] = new FileFilter();
		ffs[0].AddPattern("*.jpg");
		ffs[0].Name = "JPEG image";
		ffs[1] = new FileFilter();
		ffs[1].AddPattern("*.png");
		ffs[1].Name = "PNG image";
		ffs[2] = new FileFilter();
		ffs[2].AddPattern("*.bmp");
		ffs[2].Name = "Plain 24 bpp bitmap (BMP)";

		fcd.AddFilter(ffs[0]);
		fcd.AddFilter(ffs[1]);
		fcd.AddFilter(ffs[2]);
		
		if (fcd.Run() == (int)Gtk.ResponseType.Accept)
		{
			if (fcd.Filter == ffs[0])
				ppmviewwidget1.SavePicture(fcd.Filename, "jpeg");
			if (fcd.Filter == ffs[1])
				ppmviewwidget1.SavePicture(fcd.Filename, "png");
			if (fcd.Filter == ffs[2])
				ppmviewwidget1.SavePicture(fcd.Filename, "bmp");
		}
		fcd.Destroy();
	}
	
	private void ClearHDR()
	{
		ppmviewwidget1.HDR = null;
		ppmviewwidget1.UpdatePicture();
		hdr = null;
		frozen = null;
		GC.Collect();		// For freeing memory from unused hdr_src
	}
	
	private void LoadStream(System.IO.Stream stream, ProgressMessageReporter callback, int downscale_by)
	{
		ClearHDR();
		
		ppl = PPMLoader.FromStream(stream, delegate (double progress) {
			if (callback != null) 
			{
				return callback(progress, "Parsing image...");
			}
			else 
				return true; // If callback is not assigned, just continue
		});
		
		if (ppl == null)
		{
			if (callback != null)
			{
				cancel_pending = false;
				callback(0, "Loading cancelled");
			}
		}
		else
		{
			if (downscale_by != 1)		
			{
				bool dsres = ppl.Downscale(downscale_by, delegate (double progress) {
					if (callback != null) 
					{
						return callback(progress, "Downscaling...");
					}
					else 
						return true; // If callback is not assigned, just continue
				});
				
				if (dsres == false)
				{
					ppl = null;
					if (callback != null)
					{
						cancel_pending = false;
						callback(0, "Loading cancelled");
					}
				}
			}
		}
		TheUIState = MainWindow.UIState.Free;
		stages.ReportImageChanged(ppl.Header.Width, ppl.Header.Height);
		LaunchUpdateTimer();
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns>
	/// True if complete, false if user hit Cancel
	/// </returns>
	bool UpdateFrozen()
	{
		bool res;
		if (TheUIState != MainWindow.UIState.Processing)
		{
			UIState curstate = TheUIState;
			TheUIState = MainWindow.UIState.Processing;
			
			if (ppl != null)
			{
				FloatPixmap frozen_tmp = FloatPixmap.FromPPM(ppl, delegate (double progress) {
					return ImportRawAndLoadingReporter(progress, "Loading source image...");
					
				});
				if (frozen_tmp != null)
				{
					if (stages.ApplyOperationsBeforeFrozenLine(frozen_tmp))
					{
						frozen = frozen_tmp;
						progressbar.Text = "Operation completed";
						progressbar.Fraction = 0;
						res = true;
					}
					else
					{
						progressbar.Text = "Operation cancelled";
						progressbar.Fraction = 0;
						cancel_pending = false;
						res = false;
					}
				}
				else
				{
					progressbar.Text = "Operation cancelled";
					progressbar.Fraction = 0;
					cancel_pending = false;
					res = false;
				}
			}
			else
			{
				res = false;
			}
			
			TheUIState = curstate;
		}
		else
		{
			res = false;
		}
		return res;
	}
	
	/// <summary>
	/// Assumes that frozen image is prepared already
	/// </summary>
	void UpdateStageAfterFrozen()
	{
		if (TheUIState != MainWindow.UIState.Processing)
		{
			UIState curstate = TheUIState;
			TheUIState = MainWindow.UIState.Processing;
			
			if (ppl != null)
			{
				if (frozen == null)
				{
					hdr = FloatPixmap.FromPPM(ppl, delegate (double progress) {
						return ImportRawAndLoadingReporter(progress, "Loading source image...");
					});
				}
				else
					hdr = new FloatPixmap(frozen);
				
				ppmviewwidget1.HDR = hdr;
				
				if (hdr != null)
				{
					if (stages.ApplyOperationsAfterFrozenLine(hdr))
					{
						progressbar.Text = "Operation completed";
						progressbar.Fraction = 0;
						ppmviewwidget1.UpdatePicture();
						ppmviewwidget1.QueueDraw();
					}
					else
					{
						progressbar.Text = "Operation cancelled";
						progressbar.Fraction = 0;
						cancel_pending = false;
					}
				}
				else
				{
					progressbar.Text = "Operation cancelled";
					progressbar.Fraction = 0;
					cancel_pending = false;
				}
			}
			TheUIState = curstate;
		}
	}
	
	bool ImportRawAndLoadingReporter(double progress, string status)
	{
		progressbar.Fraction = progress;
		progressbar.Text = status;
		while (Application.EventsPending()) Application.RunIteration();

		return (!cancel_pending);
	}
	
	/// <summary>
	/// Launching dcraw to process the raw file, loads the result into memory stream
	/// </summary>
	/// <returns>
	/// A stream to read the decoded PPM data from. 
	/// Should be closed by user.
	/// </returns>
	private System.IO.MemoryStream ImportRaw(string filename, ProgressMessageReporter callback)
	{
		if (callback != null)
			if (!callback(0, "Waiting for dcraw...")) return null;
		
		System.Diagnostics.Process prc = DCRawConnection.CreateDCRawProcess("-4 -c \"" + filename + "\"");
		
		int cnt = 0;
		if (prc != null)
		{
			if (prc.Start())
			{
				int readed = 0;
				int readed_all = 0;
				System.IO.MemoryStream ms = new System.IO.MemoryStream();
				byte[] buf = new byte[1024 * 4];
				do
				{
					cnt++;
					if (cnt % 100 == 0)
					{
						if (callback != null)
						{
							if (!callback(0, "Reading data: " + (readed_all / (1024 * 1024)) + " M")) return null;
						}
					}
					readed = prc.StandardOutput.BaseStream.Read(buf, 0, buf.Length);
					ms.Write(buf, 0, readed);
					readed_all += readed;
				}
				while (readed > 0);
	
				if (callback != null)
				{
					if (!callback(0, "Data reading complete.")) return null;
				}
				while (Application.EventsPending()) Application.RunIteration();
				
				prc.StandardOutput.Dispose();
				prc.WaitForExit(-1);	// R.I.P.
				prc.Close();
				
				ms.Seek(0, System.IO.SeekOrigin.Begin);
				return ms;
			}
			else
			{
				Gtk.MessageDialog md = new Gtk.MessageDialog(this, DialogFlags.Modal,
				                                             MessageType.Error, ButtonsType.Ok, 
				                                             "Can not start DCRaw process");
				md.Title = "Error";
				md.Run();
				md.Destroy();
			}
		}
		else
		{
			Gtk.MessageDialog md = new Gtk.MessageDialog(this, DialogFlags.Modal,
			                                             MessageType.Error, ButtonsType.Ok, 
			                                             "DCRaw executable not found");
			md.Title = "Error";
			md.Run();
			md.Destroy();
		}
		return null;
	}
	
	
	
	protected void LoadRawImageActionPicked()
	{
		if (TheUIState == MainWindow.UIState.Loading)
		{
			Gtk.MessageDialog md = new Gtk.MessageDialog(this, DialogFlags.Modal,
			                                             MessageType.Error, ButtonsType.Ok, 
			                                             "Can not start DCRaw process");
			md.Title = "Error";
			md.Run();
			md.Destroy();
		}
		else
		{
			RawImportDialog rid = new RawImportDialog();
			
			if (FileName != null) rid.Filename = FileName;
			if (PreScale != 0) rid.PreScale = PreScale;
			
			bool ok = false;
			
			if (rid.Run() == (int)Gtk.ResponseType.Accept)
			{
				ok = true;
				FileName = rid.Filename;
				PreScale = rid.PreScale;
			}
			rid.Destroy();
			
			if (ok)
			{
				UIState curstate = TheUIState;
				TheUIState = MainWindow.UIState.Loading;
				System.IO.MemoryStream strm = ImportRaw(FileName, ImportRawAndLoadingReporter);
				if (strm == null)
				{
					cancel_pending = false;
					progressbar.Fraction = 0;
					progressbar.Text = "Importing cancelled";
				}
				else
				{
					LoadStream(strm, ImportRawAndLoadingReporter, PreScale);
					strm.Close();
					strm.Dispose();
				}
				TheUIState = curstate;
			}
		}
	}
	
	protected virtual void OnImportFromDCRawActionActivated (object sender, System.EventArgs e)
	{
		GLib.Timeout.Add(1, delegate {
			LoadRawImageActionPicked();
			return false;
		});
	}
	
	protected virtual void OnCancelButtonClicked (object sender, System.EventArgs e)
	{
		cancel_pending = true;
	}
	
	protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
	{
		if (TheUIState != MainWindow.UIState.Free)
			cancel_pending = true;
		Application.Quit();
	}
	
	protected virtual void OnAboutActionActivated (object sender, System.EventArgs e)
	{
		AboutBox abb = new AboutBox();
		abb.Run();
		abb.Destroy();
	}
	
	protected virtual void OnUpdateDuringProcessingActionChanged (object o, Gtk.ChangedArgs args)
	{
	}
	
	protected virtual void OnUpdateDuringProcessingActionToggled (object sender, System.EventArgs e)
	{
		ppmviewwidget1.InstantUpdate = this.UpdateDuringProcessingAction.Active;
	}
	
	protected void OnSaveStageAsActionActivated (object sender, System.EventArgs e)
	{
		Gtk.FileChooserDialog fcd = new Gtk.FileChooserDialog("Save stage description", 
		                                                      this, 
		                                                      FileChooserAction.Save,
		                                                      "Cancel", ResponseType.Cancel,
		                                                      "Save", ResponseType.Accept);
		
		FileFilter[] ffs = new FileFilter[1];
		ffs[0] = new FileFilter();
		ffs[0].AddPattern("*.cestage");
		ffs[0].Name = "CatEye Stage file";

		fcd.AddFilter(ffs[0]);
		
		if (fcd.Run() == (int)Gtk.ResponseType.Accept)
		{
			string fn = fcd.Filename;
			if (System.IO.Path.GetExtension(fn).ToLower() != ".cestage")
				fn += ".cestage";
			
			XmlDocument xdoc = new XmlDocument();
			xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", null, null));
			xdoc.AppendChild(stages.SerializeToXML(xdoc));
			xdoc.Save(fn);
		}
		fcd.Destroy();
	}
	
	public void LoadStage(string filename)
	{
		XmlDocument xdoc = new XmlDocument();
		xdoc.Load(filename);
		try
		{
			stages.FrozenAt = null;
			stages.DeserializeFromXML(xdoc.ChildNodes[1], _StageOperationTypes);
			
			// Assigning our handlers
			for (int i = 0; i < stages.StageQueue.Length; i++)
			{
				stages.Holders[stages.StageQueue[i]].OperationParametersWidget.UserModified += delegate {
					LaunchUpdateTimer();
				};
				stages.StageQueue[i].ReportProgress += HandleProgress;
			}
		}
		catch (Exception ex)
		{
			Gtk.MessageDialog md = new Gtk.MessageDialog(this, DialogFlags.Modal,
			                                             MessageType.Error, ButtonsType.Ok, 
			                                             ex.Message);
			md.Title = "Stage file loading error";
			md.Run();
			md.Destroy();
#if DEBUG
			throw ex;
#endif
		}
	}		
	
	protected void OnLoadStageActionActivated (object sender, System.EventArgs e)
	{
		Gtk.FileChooserDialog fcd = new Gtk.FileChooserDialog("Load stage description", 
		                                                      this, 
		                                                      FileChooserAction.Open,
		                                                      "Cancel", ResponseType.Cancel,
		                                                      "Open", ResponseType.Accept);
		
		FileFilter[] ffs = new FileFilter[1];
		ffs[0] = new FileFilter();
		ffs[0].AddCustom(FileFilterFlags.Filename, delegate (Gtk.FileFilterInfo ffi) {
			return System.IO.Path.GetExtension(ffi.Filename).ToLower() == ".cestage";
		});
		ffs[0].Name = "CatEye Stage file";

		fcd.AddFilter(ffs[0]);
		
		if (fcd.Run() == (int)Gtk.ResponseType.Accept)
		{
			string fn = fcd.Filename;
			GLib.Timeout.Add(100, delegate {
				if (TheUIState == MainWindow.UIState.Processing)
				{
					cancel_pending = true;
					return true;
				}
				else
				{
					fcd.Hide();
					LoadStage(fn);
					return false;
				}
			});
			
		}
		fcd.Destroy();
	}
	protected void OnAddStageOperationButtonClicked (object sender, System.EventArgs e)
	{
		if (stages.FrozenAt == null)
		{
			Gtk.TreeIter ti;
			stageOperationToAdd_combobox.GetActiveIter(out ti);
			int index = (int)stageOperationToAdd_combobox.Model.GetValue(ti, 1);
			StageOperation so = stages.CreateAndAddNewStageOperation(_StageOperationTypes[index]);
			
			stages.Holders[so].OperationParametersWidget.UserModified += delegate {
				LaunchUpdateTimer();
			};
			so.ReportProgress += HandleProgress;
		}
	}
}