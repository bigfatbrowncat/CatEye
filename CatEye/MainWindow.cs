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

	// Stage boxes and stage operation boxes
	Dictionary<StageOperation, VBox> stage_op_boxes = new Dictionary<StageOperation, VBox>();
	Dictionary<StageOperation, StageOperationTitleWidget> stage_op_titles = new Dictionary<StageOperation, StageOperationTitleWidget>();
	Dictionary<Stage, Gtk.VBox> stage_vboxes = new Dictionary<Stage, Gtk.VBox>();
	
	StageOperation downscaling_stage_op;
	StageOperation compression_stage_op;
	StageOperation ultra_sharp_stage_op;
	StageOperation basic_ops_stage_op;
	
	StageOperation[] all_operations_sorted;
	
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
		
		// Arrange stage operations in stages
		int index_in_stage2 = 0, index_in_stage3 = 0;
		StageOperation[] sops = stages.AllOperationsSorted;
		for (int i = 0; i < sops.Length; i++)
		{
			if (sops[i].CurrentStage == Stage.Stage2)
			{
				((Box.BoxChild)stage2_vbox[stage_op_boxes[sops[i]]]).Position = index_in_stage2;
				index_in_stage2++;
			}
			else if (sops[i].CurrentStage == Stage.Stage3)
			{
				((Box.BoxChild)stage3_vbox[stage_op_boxes[sops[i]]]).Position = index_in_stage3;
				index_in_stage3++;
			}
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
		ls.AppendValues("Divide by 4", 4);
		ls.AppendValues("Divide by 8", 8);
		ls.AppendValues("Divide by 16", 16);
		
		Gtk.ComboBox pres_cb = prescale_combobox;
		pres_cb.Model = ls;
		
//		CellRendererText txt = new CellRendererText();
//		pres_cb.PackStart(txt, true);
//		pres_cb.AddAttribute(txt, "text", 0);
		
		// Selecting "No downscale"
		TreeIter ti;
		ls.GetIterFirst(out ti);
		pres_cb.SetActiveIter(ti);
		
		// Creating stages
		stage_vboxes.Add(Stage.Stage2, stage2_vbox);
		stage_vboxes.Add(Stage.Stage3, stage3_vbox);
		
		// Creating stage operations
		downscaling_stage_op = new DownscalingStageOperation(0, downscaling_stageoperation_parameterswidget);
		compression_stage_op = new CompressionStageOperation(1, compression_stageoperation_parameterswidget);
		ultra_sharp_stage_op = new UltraSharpStageOperation(2, ultra_sharp_stageoperation_parameterswidget);
		basic_ops_stage_op = new BasicOpsStageOperation(3, basic_ops_stageoperation_parameterswidget);
		
		// Adding moving handlers to stage operations
		downscaling_stage_op.MovedToStage += Handle_stage_op_MovedToStage;
		downscaling_stage_op.IndexChanged += HandleStageOperationIndexChanged;
		downscaling_stage_op.MovedToStage += delegate {
			ArrangeStageOperationBoxes();
		};
		
		basic_ops_stage_op.MovedToStage += Handle_stage_op_MovedToStage;
		basic_ops_stage_op.IndexChanged += HandleStageOperationIndexChanged;
		basic_ops_stage_op.MovedToStage += delegate {
			ArrangeStageOperationBoxes();
		};
		
		compression_stage_op.MovedToStage += Handle_stage_op_MovedToStage;
		compression_stage_op.IndexChanged += HandleStageOperationIndexChanged;
		compression_stage_op.MovedToStage += delegate {
			ArrangeStageOperationBoxes();
		};
		
		ultra_sharp_stage_op.MovedToStage += Handle_stage_op_MovedToStage;
		ultra_sharp_stage_op.IndexChanged += HandleStageOperationIndexChanged;
		ultra_sharp_stage_op.MovedToStage += delegate {
			ArrangeStageOperationBoxes();
		};

		// Adding stage operations to stages
		downscaling_stage_op.AddToStage(Stage.Stage3);
		basic_ops_stage_op.AddToStage(Stage.Stage3);
		compression_stage_op.AddToStage(Stage.Stage3);
		ultra_sharp_stage_op.AddToStage(Stage.Stage3);

		// Adding stage operation--boxes to dictionary
		stage_op_boxes.Add(downscaling_stage_op, downscaling_vbox);
		stage_op_boxes.Add(basic_ops_stage_op, basic_ops_vbox);
		stage_op_boxes.Add(compression_stage_op, compression_vbox);
		stage_op_boxes.Add(ultra_sharp_stage_op, ultra_sharp_vbox);
		
		// Adding stage operation--title widgets to dictionary
		stage_op_titles.Add(downscaling_stage_op, downscaling_stageoperation_titlewidget);
		stage_op_titles.Add(basic_ops_stage_op, basic_ops_stageoperation_titlewidget);
		stage_op_titles.Add(compression_stage_op, compression_stageoperationtitlewidget);
		stage_op_titles.Add(ultra_sharp_stage_op, ultra_sharp_stageoperationtitlewidget);
		
		// Creating stages
		stages = new Stages(new StageOperation[] { 
			downscaling_stage_op,  
			basic_ops_stage_op,
			compression_stage_op,
			ultra_sharp_stage_op
		});
		
		// Move all operations to stage 2
		while (stages.Stage3.Length > 0)
		{
			stages.ChangeStage(stages.Stage3[0]);
		}
		
		// Arranging boxes
		ArrangeStageOperationBoxes();
	}

	void HandleStageOperationIndexChanged (object sender, IndexChangedEventArgs e)
	{
		ArrangeStageOperationBoxes();
	}


	void Handle_stage_op_MovedToStage (object sender, MovedToStageEventArgs e)
	{
		VBox box = stage_op_boxes[(sender as StageOperation)];
		stage_vboxes[e.SourceStage].Remove(box);
		stage_vboxes[e.DestinationStage].Add(box);
		stage_op_titles[(sender as StageOperation)].CurrentStage = e.DestinationStage;
		ArrangeStageOperationBoxes();
		any_stage_modified = true;
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	
	protected virtual void OnBrightnessRangeSetButtonClicked (object sender, System.EventArgs e)
	{

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
	
	protected virtual void OnDownscalingStageoperationtitlewidgetChangeStageButtonClicked (object sender, System.EventArgs e)
	{
		stages.ChangeStage(downscaling_stage_op);
		//downscaling_stageoperationtitlewidget.CurrentStage = downscaling_stage_op.CurrentStage;
	}
	
	protected virtual void OnBasicOpsStageoperationtitlewidgetChangeStageButtonClicked (object sender, System.EventArgs e)
	{
		stages.ChangeStage(basic_ops_stage_op);
		//basic_ops_stageoperationtitlewidget.CurrentStage = basic_ops_stage_op.CurrentStage;
	}
	
	protected virtual void OnCompressionStageoperationtitlewidgetChangeStageButtonClicked (object sender, System.EventArgs e)
	{
		stages.ChangeStage(compression_stage_op);
		//compression_stageoperationtitlewidget.CurrentStage = compression_stage_op.CurrentStage;
	}
	
	protected virtual void OnSharpeningStageoperationtitlewidgetChangeStageButtonClicked (object sender, System.EventArgs e)
	{
		stages.ChangeStage(ultra_sharp_stage_op);
		//ultra_sharp_stageoperationtitlewidget.CurrentStage = ultra_sharp_stage_op.CurrentStage;
	}
	
	protected virtual void OnDownscalingStageoperationtitlewidgetUpButtonClicked (object sender, System.EventArgs e)
	{
		stages.MoveStageOperationUp(downscaling_stage_op);
	}
	
	protected virtual void OnDownscalingStageoperationtitlewidgetDownButtonClicked (object sender, System.EventArgs e)
	{
		stages.MoveStageOperationDown(downscaling_stage_op);
	}
	
	protected virtual void OnBasicOpsStageoperationtitlewidgetUpButtonClicked (object sender, System.EventArgs e)
	{
		stages.MoveStageOperationUp(basic_ops_stage_op);
	}
	
	protected virtual void OnBasicOpsStageoperationtitlewidgetDownButtonClicked (object sender, System.EventArgs e)
	{
		stages.MoveStageOperationDown(basic_ops_stage_op);
	}
	
	protected virtual void OnCompressionStageoperationtitlewidgetUpButtonClicked (object sender, System.EventArgs e)
	{
		stages.MoveStageOperationUp(compression_stage_op);
	}
	
	protected virtual void OnCompressionStageoperationtitlewidgetDownButtonClicked (object sender, System.EventArgs e)
	{
		stages.MoveStageOperationDown(compression_stage_op);
	}
	
	protected virtual void OnSharpeningStageoperationtitlewidgetUpButtonClicked (object sender, System.EventArgs e)
	{
		stages.MoveStageOperationUp(ultra_sharp_stage_op);
	}
	
	protected virtual void OnSharpeningStageoperationtitlewidgetDownButtonClicked (object sender, System.EventArgs e)
	{
		stages.MoveStageOperationDown(ultra_sharp_stage_op);
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
