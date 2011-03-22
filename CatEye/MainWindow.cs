using System;
using System.Collections.Generic;
using Gtk;
using CatEye;

public partial class MainWindow : Gtk.Window
{
	PPMLoader ppl = null;
	DoublePixmap hdr = null;
	Stages stages;

	StageOperation downscaling_stage_op;
	StageOperation compression_stage_op;
	StageOperation ultra_sharp_stage_op;
	StageOperation basic_ops_stage_op;
	
	bool user_modified_parameter = false;
	bool cancel_pending = false;
	bool processing = false;

	private void ArrangeStageOperationBoxes()
	{
		stage_vbox.CheckResize();
	}
	
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();

		// Adding prescale list
		Gtk.ListStore ls = new Gtk.ListStore(typeof(string), typeof(int));
		ls.AppendValues("No downscaling", 1);
		ls.AppendValues("Divide by 2", 2);
		ls.AppendValues("Divide by 3", 3);
		ls.AppendValues("Divide by 4", 4);
		ls.AppendValues("Divide by 5", 5);
		ls.AppendValues("Divide by 6", 6);
		ls.AppendValues("Divide by 7", 7);
		ls.AppendValues("Divide by 8", 8);
		
		Gtk.ComboBox pres_cb = prescale_combobox;
		pres_cb.Model = ls;
		
		// Selecting "No downscale"
		TreeIter ti;
		ls.GetIterFirst(out ti);
		pres_cb.SetActiveIter(ti);
		
		// Creating stage operations and stages
		stages = new Stages(stage_vbox);
		
		downscaling_stage_op = new DownscalingStageOperation(new DownscalingStageOperationParametersWidget(), stages);
		stages.AddStageOperation(downscaling_stage_op);
		downscaling_stage_op.ReportProgress += HandleProgress;
		downscaling_stage_op.ParametersWidget.UserModified += HandleUserModifiedStageOperation;
		
		basic_ops_stage_op = new BasicOpsStageOperation(new BasicOpsStageOperationParametersWidget(), stages);
		stages.AddStageOperation(basic_ops_stage_op);
		basic_ops_stage_op.ReportProgress += HandleProgress;
		basic_ops_stage_op.ParametersWidget.UserModified += HandleUserModifiedStageOperation;

		compression_stage_op = new CompressionStageOperation(new CompressionStageOperationParametersWidget(), stages);
		stages.AddStageOperation(compression_stage_op);
		compression_stage_op.ReportProgress += HandleProgress;
		compression_stage_op.ParametersWidget.UserModified += HandleUserModifiedStageOperation;
		
		ultra_sharp_stage_op = new UltraSharpStageOperation(new UltraSharpStageOperationParametersWidget(), stages);
		stages.AddStageOperation(ultra_sharp_stage_op);
		ultra_sharp_stage_op.ReportProgress += HandleProgress;
		ultra_sharp_stage_op.ParametersWidget.UserModified += HandleUserModifiedStageOperation;

		stages.OperationActivityChanged += HandleUserModifiedStageOperation;
		stages.OperationAddedToStage += delegate {
			LaunchUpdateTimer();
			ArrangeStageOperationBoxes();
		};
		stages.OperationIndexChanged += HandleUserModifiedStageOperation;
		stages.OperationRemovedFromStage += delegate {
			LaunchUpdateTimer();
			ArrangeStageOperationBoxes();
		};
		
		// Arranging boxes
		ArrangeStageOperationBoxes();
	}

	void HandleUserModifiedStageOperation (object sender, EventArgs e)
	{
		LaunchUpdateTimer();
	}
	
	protected void LaunchUpdateTimer()
	{
		user_modified_parameter = true;
		if (processing) cancel_pending = true;
		
		GLib.Timeout.Add(1000, new GLib.TimeoutHandler(delegate {
			if (processing) return true;
			if (user_modified_parameter)
			{
				user_modified_parameter = false;
				UpdateStage();
			}
			return false;
		}));
	}

	double old_frac = 0;
	void HandleProgress (object sender, ReportStageOperationProgressEventArgs e)
	{
		progressbar.Fraction = e.Progress;
		object[] attrs = sender.GetType().GetCustomAttributes(typeof(StageOperationDescriptionAttribute), false);
		if (attrs.Length > 0) 
			progressbar.Text = (attrs[0] as StageOperationDescriptionAttribute).Name + ": ";
		progressbar.Text += (e.Progress * 100).ToString("0") + "%";
		
		if (Math.Abs(progressbar.Fraction - old_frac) > 0.1)
		{
			ppmviewwidget1.UpdatePicture();
			ppmviewwidget1.QueueDraw();
			old_frac = progressbar.Fraction;
		}
		
		while (Gtk.Application.EventsPending())
			Gtk.Application.RunIteration();
		
		if (cancel_pending) e.Cancel = true;
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
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
		GC.Collect();		// For freeng memory from unused hdr_src
	}
	
	
	
	private void LoadFile(string FileName, ProgressMessageReporter callback)
	{
		ppl = PPMLoader.FromFile(FileName, delegate (double progress) {
			if (callback != null) 
			{
				return callback(progress, "Loading...");
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
			ClearHDR();
			
			TreeIter ti;
			prescale_combobox.GetActiveIter(out ti);
			int downscale_by = (int)(((ListStore)prescale_combobox.Model).GetValue(ti, 1));
			
			if (downscale_by != 1)
				ppl.Downscale(downscale_by);
		}
	}

	private void LoadStream(System.IO.Stream stream, ProgressMessageReporter callback)
	{
		ClearHDR();
		
		ppl = PPMLoader.FromStream(stream, delegate (double progress) {
			if (callback != null) 
			{
				return callback(progress, "Loading...");
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
			TreeIter ti;
			prescale_combobox.GetActiveIter(out ti);
			int downscale_by = (int)(((ListStore)prescale_combobox.Model).GetValue(ti, 1));
			
			if (downscale_by != 1)
				ppl.Downscale(downscale_by);
			
			UpdateStage();
		}
	}
	
	protected virtual void OnOpenActionActivated (object sender, System.EventArgs e)
	{
		cancel_button.Sensitive = true;
		
		Gtk.FileChooserDialog fcd = new Gtk.FileChooserDialog("Open photo", this, 
		                                                      FileChooserAction.Open,
		                                                      "Cancel", ResponseType.Cancel,
		                                                      "Open", ResponseType.Accept);
		
		FileFilter[] ffs = new FileFilter[1];
		ffs[0] = new FileFilter();
		ffs[0].AddPattern("*.ppm");
		ffs[0].Name = "PPM image";
		
		fcd.AddFilter(ffs[0]);

		string filename = null;
		if (fcd.Run() == (int)Gtk.ResponseType.Accept)
		{
			if (fcd.Filter == ffs[0])
			{
				filename = fcd.Filename;
			}
		}
		fcd.Destroy();
		
		if (filename != null)
		{
			LoadFile(filename, ImportRawAndLoadingReporter);
		}
		cancel_button.Sensitive = false;
	}
	
	void UpdateStage()
	{
		processing = true;
		cancel_button.Sensitive = true;
		
		if (ppl != null)
		{
			hdr = DoublePixmap.FromPPM(ppl);
			ppmviewwidget1.HDR = hdr;

			if (stages.ApplyOperations(hdr))
			{
				progressbar.Text = "Operation completed";
				progressbar.Fraction = 0;
				ppmviewwidget1.UpdatePicture();
			}
			else
			{
				progressbar.Text = "Operation cancelled";
				progressbar.Fraction = 0;
				cancel_pending = false;
			}
		}
		cancel_button.Sensitive = false;
		processing = false;
	}
	
	public delegate bool ProgressMessageReporter(double progress, string status);
	
	bool ImportRawAndLoadingReporter(double progress, string status)
	{
		progressbar.Fraction = progress;
		progressbar.Text = status;
		while (Application.EventsPending()) Application.RunIteration();

		return (!cancel_pending);
	}
	
	private System.IO.Stream ImportRaw(string filename, ProgressMessageReporter callback)
	{
		if (callback != null)
			if (!callback(0, "Waiting for dcraw to complete...")) return null;
		
		string mylocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
		string dcraw_path =  mylocation + System.IO.Path.DirectorySeparatorChar.ToString() + "dcraw";
		if (Environment.OSVersion.Platform == PlatformID.Win32NT || 
		    Environment.OSVersion.Platform == PlatformID.Win32Windows)
		{
			dcraw_path += ".exe";
		}
		
		if (System.IO.File.Exists(dcraw_path))
		{
			System.Diagnostics.Process prc = new System.Diagnostics.Process();
			prc.StartInfo.UseShellExecute = false;
			prc.StartInfo.FileName = dcraw_path;
			prc.StartInfo.Arguments = "-4 -w -c \"" + filename + "\"";
			prc.StartInfo.RedirectStandardOutput = true;
			prc.StartInfo.CreateNoWindow = true;
			int cnt = 0;
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

				ms.Seek(0, System.IO.SeekOrigin.Begin);
				return ms;
			}
		}
		else
		{
			Gtk.MessageDialog md = new Gtk.MessageDialog(this, DialogFlags.Modal,
			                                             MessageType.Error, ButtonsType.Ok, 
			                                             "dcraw not found at location \"" + mylocation + "\"");
			md.Title = "Error";
			if (md.Run() == (int)Gtk.ResponseType.Ok)
			{
				md.Destroy();
			}
		}
		return null;
	}
	
	protected virtual void OnImportFromDCRawActionActivated (object sender, System.EventArgs e)
	{
		cancel_button.Sensitive = true;
		Gtk.FileChooserDialog fcd = new Gtk.FileChooserDialog("Import raw", this, 
		                                                      FileChooserAction.Open,
		                                                      "Cancel", ResponseType.Cancel,
		                                                      "Open", ResponseType.Accept);
		
		FileFilter[] ffs = new FileFilter[1];
		ffs[0] = new FileFilter();
		ffs[0].AddPattern("*");
		ffs[0].Name = "RAW images accepted by dcraw";
		
		fcd.AddFilter(ffs[0]);

		string filename = null;
		if (fcd.Run() == (int)Gtk.ResponseType.Accept)
		{
			if (fcd.Filter == ffs[0])
			{
				filename = fcd.Filename;
			}
		}
		fcd.Destroy();
		
		if (filename != null)
		{
			System.IO.Stream strm = ImportRaw(filename, ImportRawAndLoadingReporter);
			if (strm == null)
			{
				cancel_pending = false;
				progressbar.Fraction = 0;
				progressbar.Text = "Importing cancelled";
				cancel_button.Sensitive = false;
			}
			else
				LoadStream(strm, ImportRawAndLoadingReporter);
		}
		cancel_button.Sensitive = false;
	}
	
	protected virtual void OnCancelButtonClicked (object sender, System.EventArgs e)
	{
		cancel_pending = true;
	}
	
	protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
	{
		Application.Quit();
	}
	
	
	
}
