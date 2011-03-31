using System;
using System.Collections.Generic;
using Gtk;
using CatEye;

public partial class MainWindow : Gtk.Window
{
	protected enum UIState { Processing, Loading, Free };
	private UIState _TheUIState = UIState.Free;
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
					loadAction.Sensitive = true;
					saveAsAction.Sensitive = true;
					cancel_button.Sensitive = false;
				}
				else if (value == MainWindow.UIState.Loading)
				{
					loadAction.Sensitive = false;
					saveAsAction.Sensitive = false;
					cancel_button.Sensitive = true;
				}
				else if (value == MainWindow.UIState.Processing)
				{
					loadAction.Sensitive = true;
					saveAsAction.Sensitive = true;
					cancel_button.Sensitive = true;
				}
			}
		}
	}
	
	public delegate bool ProgressMessageReporter(double progress, string status);

	PPMLoader ppl = null;
	DoublePixmap hdr = null;
	DoublePixmap frozen = null;
	Stages stages;

	StageOperation crop_stage_op;
	StageOperation compression_stage_op;
	StageOperation ultra_sharp_stage_op;
	StageOperation basic_ops_stage_op;
	StageOperation brightness_stage_op;
	
	bool update_timer_launched = false;
	bool cancel_pending = false;
	
	uint update_timer_delay = 500;

	DateTime lastupdate;
	
	private void ArrangeStageOperationBoxes()
	{
		left_vbox.CheckResize();
		stage_vbox.CheckResize();
	}
	
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();

		// Creating stage operations and stages
		stages = new Stages(stage_vbox);

		crop_stage_op = new CropStageOperation(new CropStageOperationParametersWidget());
		stages.AddStageOperation(crop_stage_op);
		crop_stage_op.ReportProgress += HandleProgress;
		crop_stage_op.ParametersWidget.UserModified += delegate {
			LaunchUpdateTimer();
		};
		
		brightness_stage_op = new BrightnessStageOperation(new BrightnessStageOperationParametersWidget());
		stages.AddStageOperation(brightness_stage_op);
		brightness_stage_op.ReportProgress += HandleProgress;
		brightness_stage_op.ParametersWidget.UserModified += delegate {
			LaunchUpdateTimer();
		};
		
		compression_stage_op = new CompressionStageOperation(new CompressionStageOperationParametersWidget());
		stages.AddStageOperation(compression_stage_op);
		compression_stage_op.ReportProgress += HandleProgress;
		compression_stage_op.ParametersWidget.UserModified += delegate {
			LaunchUpdateTimer();
		};
		
		ultra_sharp_stage_op = new UltraSharpStageOperation(new UltraSharpStageOperationParametersWidget());
		stages.AddStageOperation(ultra_sharp_stage_op);
		ultra_sharp_stage_op.ReportProgress += HandleProgress;
		ultra_sharp_stage_op.ParametersWidget.UserModified += delegate {
			LaunchUpdateTimer();
		};

		basic_ops_stage_op = new BasicOpsStageOperation(new BasicOpsStageOperationParametersWidget());
		stages.AddStageOperation(basic_ops_stage_op);
		basic_ops_stage_op.ReportProgress += HandleProgress;
		basic_ops_stage_op.ParametersWidget.UserModified += delegate {
			LaunchUpdateTimer();
		};
		
		
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
		};
		
		// Arranging boxes
		ArrangeStageOperationBoxes();
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
				DoublePixmap frozen_tmp = DoublePixmap.FromPPM(ppl, delegate (double progress) {
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
					hdr = DoublePixmap.FromPPM(ppl, delegate (double progress) {
						return ImportRawAndLoadingReporter(progress, "Loading source image...");
					});
				}
				else
					hdr = new DoublePixmap(frozen);
				
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
	private System.IO.Stream ImportRaw(string filename, ProgressMessageReporter callback)
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
					byte[] buf = new byte[1024 * 4];
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
			string filename = null;
	
			int downscale_by = 1;
			RawImportDialog rid = new RawImportDialog();
			
			if (rid.Run() == (int)Gtk.ResponseType.Accept)
			{
				filename = rid.Filename;
				downscale_by = rid.PreScale;
			}
			rid.Destroy();
			
			if (filename != null)
			{
				UIState curstate = TheUIState;
				TheUIState = MainWindow.UIState.Loading;
				System.IO.Stream strm = ImportRaw(filename, ImportRawAndLoadingReporter);
				if (strm == null)
				{
					cancel_pending = false;
					progressbar.Fraction = 0;
					progressbar.Text = "Importing cancelled";
				}
				else
				{
					LoadStream(strm, ImportRawAndLoadingReporter, downscale_by);
					strm.Close();
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
	
	
	
	
	
	
}
