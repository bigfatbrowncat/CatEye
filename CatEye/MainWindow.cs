using System;
using System.Collections.Generic;
using Gtk;
using CatEye;

public partial class MainWindow : Gtk.Window
{
	PPMLoader ppl = null;
	DoublePixmap hdr_after_stage2 = null;
	DoublePixmap hdr_after_stage3 = null;
	Stages stages;

	StageOperation downscaling_stage_op;
	StageOperation compression_stage_op;
	StageOperation ultra_sharp_stage_op;
	StageOperation basic_ops_stage_op;
	

	bool any_stage_modified = true;
	
	private void ArrangeStageOperationBoxes()
	{
		// Check if we are using stage 3
		if (stages.Stage3.Length > 0)
		{
			// Show right panel with stage 3
			right_vbox.Visible = true;
			// Show "Show" button near "Update view" button
			show_stage2_radiobutton.Visible = true;
		}
		else
		{
			// Hide right panel with stage 3
			right_vbox.Visible = false;
			// Hide "Show" button near "Update view" button
			show_stage2_radiobutton.Visible = false;
		}
		
		stage2_vbox.CheckResize();
		stage3_vbox.CheckResize();
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
		stages = new Stages(stage2_vbox, stage3_vbox);
		
		downscaling_stage_op = new DownscalingStageOperation(new DownscalingStageOperationParametersWidget(), stages);
		stages.AddStageOperation(downscaling_stage_op, Stage.Stage2);
		
		compression_stage_op = new CompressionStageOperation(new CompressionStageOperationParametersWidget(), stages);
		stages.AddStageOperation(compression_stage_op, Stage.Stage2);
		
		ultra_sharp_stage_op = new UltraSharpStageOperation(new UltraSharpStageOperationParametersWidget(), stages);
		stages.AddStageOperation(ultra_sharp_stage_op, Stage.Stage2);

		basic_ops_stage_op = new BasicOpsStageOperation(new BasicOpsStageOperationParametersWidget(), stages);
		stages.AddStageOperation(basic_ops_stage_op, Stage.Stage2);
		
		stages.OperationAddedToStage += delegate {
			ArrangeStageOperationBoxes();
		};
		
		// Arranging boxes
		ArrangeStageOperationBoxes();
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
	
	private void LoadFile(string FileName)
	{
		ppmviewwidget1.HDR = null;
		hdr_after_stage2 = null;
		hdr_after_stage3 = null;
		GC.Collect();		// For freeng memory from unused hdr_src
		
		ppl = PPMLoader.FromFile(FileName);

		TreeIter ti;
		prescale_combobox.GetActiveIter(out ti);
		int downscale_by = (int)(((ListStore)prescale_combobox.Model).GetValue(ti, 1));
		
		if (downscale_by != 1)
			ppl.Downscale(downscale_by);
		
		hdr_after_stage2 = DoublePixmap.FromPPM(ppl);
	}

	private void LoadStream(System.IO.Stream stream)
	{
		ppmviewwidget1.HDR = null;
		hdr_after_stage2 = null;
		hdr_after_stage3 = null;
		GC.Collect();		// For freeng memory from unused hdr_src
		
		ppl = PPMLoader.FromStream(stream);

		TreeIter ti;
		prescale_combobox.GetActiveIter(out ti);
		int downscale_by = (int)(((ListStore)prescale_combobox.Model).GetValue(ti, 1));
		
		if (downscale_by != 1)
			ppl.Downscale(downscale_by);
		
		hdr_after_stage2 = DoublePixmap.FromPPM(ppl);
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
		UpdateStage2(true);
	}
	
	void UpdateStage2(bool update_stage_3_and_view)
	{
		if (ppl != null)
		{
			hdr_after_stage2 = DoublePixmap.FromPPM(ppl);

			stages.DoStage2(hdr_after_stage2);
			
			Console.WriteLine("Stage 2 completed.");
			if (update_stage_3_and_view)
			{
				UpdateStage3();
				UpdateHDRView();
			}
		}
	}
	
	void UpdateStage3()
	{
		if (hdr_after_stage2 == null || any_stage_modified)
		{
			UpdateStage2(false);
		}
		
		if (hdr_after_stage2 != null)
		{
			hdr_after_stage3 = new DoublePixmap(hdr_after_stage2);

			stages.DoStage3(hdr_after_stage3);
			
			UpdateHDRView();
			any_stage_modified = false;
			Console.WriteLine("Stage 3 completed.");
		}
	}
	
	void UpdateHDRView()
	{
		if (show_stage3_radiobutton.Active)
		{
			ppmviewwidget1.HDR = hdr_after_stage3;
		}	
		if (show_stage2_radiobutton.Active)
		{
			ppmviewwidget1.HDR = hdr_after_stage2;
		}		
	}
	
	protected virtual void OnApplyStage3ButtonClicked (object sender, System.EventArgs e)
	{
		UpdateStage3();
	}
	
	protected virtual void OnShowStage3RadiobuttonToggled (object sender, System.EventArgs e)
	{
		UpdateHDRView();
	}
	
	protected virtual void OnShowStage2RadiobuttonToggled (object sender, System.EventArgs e)
	{
		UpdateHDRView();
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
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
}
