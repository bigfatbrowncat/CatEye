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
	

	bool cancel_pending = false;

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
		
		compression_stage_op = new CompressionStageOperation(new CompressionStageOperationParametersWidget(), stages);
		stages.AddStageOperation(compression_stage_op);
		compression_stage_op.ReportProgress += HandleProgress;
		
		ultra_sharp_stage_op = new UltraSharpStageOperation(new UltraSharpStageOperationParametersWidget(), stages);
		stages.AddStageOperation(ultra_sharp_stage_op);
		ultra_sharp_stage_op.ReportProgress += HandleProgress;

		basic_ops_stage_op = new BasicOpsStageOperation(new BasicOpsStageOperationParametersWidget(), stages);
		stages.AddStageOperation(basic_ops_stage_op);
		basic_ops_stage_op.ReportProgress += HandleProgress;
		
		stages.OperationAddedToStage += delegate {
			ArrangeStageOperationBoxes();
		};
		
		// Arranging boxes
		ArrangeStageOperationBoxes();
	}

	void HandleProgress (object sender, ReportStageOperationProgressEventArgs e)
	{
		progressbar.Fraction = e.Progress;
		object[] attrs = sender.GetType().GetCustomAttributes(typeof(StageOperationDescriptionAttribute), false);
		if (attrs.Length > 0) 
			progressbar.Text = (attrs[0] as StageOperationDescriptionAttribute).Name + ": ";
		progressbar.Text += (e.Progress * 100).ToString("0") + "%";
		
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
	
	private void LoadFile(string FileName)
	{
		ClearHDR();
		ppl = PPMLoader.FromFile(FileName);

		TreeIter ti;
		prescale_combobox.GetActiveIter(out ti);
		int downscale_by = (int)(((ListStore)prescale_combobox.Model).GetValue(ti, 1));
		
		if (downscale_by != 1)
			ppl.Downscale(downscale_by);
		
		//hdr = DoublePixmap.FromPPM(ppl);
	}

	private void LoadStream(System.IO.Stream stream)
	{
		ClearHDR();
		ppl = PPMLoader.FromStream(stream);

		TreeIter ti;
		prescale_combobox.GetActiveIter(out ti);
		int downscale_by = (int)(((ListStore)prescale_combobox.Model).GetValue(ti, 1));
		
		if (downscale_by != 1)
			ppl.Downscale(downscale_by);
		
		//hdr_after_stage2 = DoublePixmap.FromPPM(ppl);
	}
	
	protected virtual void OnOpenActionActivated (object sender, System.EventArgs e)
	{
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
			LoadFile(filename);
		}
	}
	

	protected virtual void OnApplyStage2ButtonClicked (object sender, System.EventArgs e)
	{
		UpdateStage();
	}
	
	void UpdateStage()
	{
		cancel_button.Sensitive = true;
		if (ppl != null)
		{
			hdr = DoublePixmap.FromPPM(ppl);

			if (stages.ApplyOperations(hdr))
			{
				Console.WriteLine("Stage updated.");
				ppmviewwidget1.HDR = hdr;
			}
			else
			{
				progressbar.Text = "User cancelled the operation!";
				cancel_pending = false;//  .Active = false;
			}
		}
		cancel_button.Sensitive = false;
	}
	
	private System.IO.Stream ImportRaw(string filename)
	{
		string mylocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
		string dcraw_path =  mylocation + System.IO.Path.DirectorySeparatorChar.ToString() + "dcraw";
		if (Environment.OSVersion.Platform == PlatformID.Win32NT || 
		    Environment.OSVersion.Platform == PlatformID.Win32Windows)
		{
			dcraw_path += ".exe";
		}
		Console.WriteLine(dcraw_path);
		
		if (System.IO.File.Exists(dcraw_path))
		{
			System.Diagnostics.Process prc = new System.Diagnostics.Process();
			prc.StartInfo.UseShellExecute = false;
			prc.StartInfo.FileName = dcraw_path;
			prc.StartInfo.Arguments = "-4 -w -c \"" + filename + "\"";
			prc.StartInfo.RedirectStandardOutput = true;
			if (prc.Start())
			{
				int readed = 0;
				System.IO.MemoryStream ms= new System.IO.MemoryStream();
				do
				{
					byte[] buf = new byte[1024 * 4];
					readed = prc.StandardOutput.BaseStream.Read(buf, 0, buf.Length);
					ms.Write(buf, 0, readed);
				}
				while (readed > 0);
				Console.WriteLine("PPM data readed");

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
			LoadStream(ImportRaw(filename));
		}

	}
	
	protected virtual void OnCancelButtonClicked (object sender, System.EventArgs e)
	{
		cancel_pending = true;
	}
	
	
}
