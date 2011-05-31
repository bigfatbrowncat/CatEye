using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Gtk;
using CatEye;
using CatEye.Core;

public partial class MainWindow : Gtk.Window
{
	private static string APP_NAME = "CatEye";

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
	
	ExtendedStage stages;
	
	DateTime lastupdate;
	
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();

		// Creating stage operations and stages
		stages = new ExtendedStage(stage_vbox);
		
		// Preparing stage operation adding store
		ListStore ls = new ListStore(typeof(string), typeof(int));
		for (int i = 0; i < ExtendedStage._StageOperationTypes.Length; i++)
		{
			string desc;
			object[] descs = ExtendedStage._StageOperationTypes[i].GetCustomAttributes(
				typeof(StageOperationDescriptionAttribute), true);
			if (descs.Length == 0)
				desc = ExtendedStage._StageOperationTypes[i].Name;
			else
				desc = ((StageOperationDescriptionAttribute)descs[0]).Name;
		
			ls.AppendValues(desc, i);
		}
		
		stageOperationToAdd_combobox.Model = ls;
		Gtk.TreeIter ti;
		ls.GetIterFirst(out ti);
		stageOperationToAdd_combobox.SetActiveIter(ti);
		
		// Setting stages events
		
		stages.OperationFrozen += delegate {
			stageOperationAdding_hbox.Sensitive = false;
		};
		stages.OperationDefrozen += delegate {
			stageOperationAdding_hbox.Sensitive = true;
		};
		stages.ImageChanged += delegate {
			view_widget.HDR = stages.CurrentImage;
		};
		stages.ImageUpdated += delegate {
			view_widget.UpdatePicture();
		};
		stages.OperationAddedToStage += HandleStagesOperationAddedToStage;
		stages.OperationRemovedFromStage += HandleStagesOperationRemovedFromStage;
		stages.UIStateChanged += HandleStagesUIStateChanged;
		stages.RawLoaded += delegate {
			view_widget.CenterImagePanning();
		};
		
		// Loading default stage
		string mylocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
		string defaultstage = mylocation + System.IO.Path.DirectorySeparatorChar.ToString() + "default.cestage";
		if (System.IO.File.Exists(defaultstage))
		{
			stages.LoadStage(defaultstage);
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
		
		// Setting view widget events
		view_widget.ExposeEvent += DrawCurrentStageOperationEditor;
		view_widget.MousePositionChanged += HandleView_widgetMousePositionChanged;
		view_widget.MouseButtonStateChanged += HandleView_widgetMouseButtonStateChanged;
		
		// Setting zoom widget events
		stages.ZoomValue = zoomwidget1.Value;
		zoomwidget1.ValueChanged += delegate {
			stages.ZoomValue = zoomwidget1.Value;
		};
	}

	void HandleStagesOperationRemovedFromStage (object sender, OperationRemovedFromStageEventArgs e)
	{
		e.Operation.ReportProgress -= HandleProgress;
	}

	void HandleStagesOperationAddedToStage (object sender, OperationAddedToStageEventArgs e)
	{
		e.Operation.ReportProgress += HandleProgress;
	}

	void HandleStagesUIStateChanged (object sender, EventArgs e)
	{
		if (stages.TheUIState == UIState.Free)
		{
			loadRawAction.Sensitive = true;
			loadStageAction.Sensitive = true;
			saveStageAsAction.Sensitive = true;
			renderToAction.Sensitive = true;
			cancel_button.Visible = false;
			progressbar.Visible = false;

		}
		else if (stages.TheUIState == UIState.Loading)
		{
			loadRawAction.Sensitive = false;
			loadStageAction.Sensitive = false;
			saveStageAsAction.Sensitive = false;
			renderToAction.Sensitive = false;
			cancel_button.Visible = true;
			cancel_button.Sensitive = true;
			progressbar.Visible = true;
			progressbar.Fraction = 0;
			progressbar.Text = "";
		}
		else if (stages.TheUIState == UIState.Processing)
		{
			loadRawAction.Sensitive = true;
			loadStageAction.Sensitive = true;
			saveStageAsAction.Sensitive = true;
			renderToAction.Sensitive = true;
			cancel_button.Visible = false;
			progressbar.Visible = true;
			progressbar.Fraction = 0;
			progressbar.Text = "";
		}		
	}

	bool HandleView_widgetMouseButtonStateChanged (object sender, int x, int y, uint button_id, bool is_down)
	{
		int x_rel = x - view_widget.CurrentImagePosition.X;
		int y_rel = y - view_widget.CurrentImagePosition.Y;

		if (stages.ReportEditorMouseButton(x_rel, 
									 y_rel, 
									 view_widget.CurrentImagePosition.Width, 
									 view_widget.CurrentImagePosition.Height, 
									 button_id, is_down))
		{
			view_widget.QueueDraw();
			return true;
		}
		return false;
	}

	bool HandleView_widgetMousePositionChanged (object sender, int x, int y)
	{
		int x_rel = x - view_widget.CurrentImagePosition.X;
		int y_rel = y - view_widget.CurrentImagePosition.Y;

		if (stages.ReportEditorMousePosition(x_rel, 
									 y_rel, 
									 view_widget.CurrentImagePosition.Width, 
									 view_widget.CurrentImagePosition.Height))
		{
			view_widget.QueueDraw();
			return true;
		}
		return false;
	}


	void DrawCurrentStageOperationEditor (object o, ExposeEventArgs args)
	{
		stages.DrawEditor(view_widget.GdkWindow, view_widget.CurrentImagePosition);
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
			if ((DateTime.Now - lastupdate).TotalMilliseconds / view_widget.UpdateTimeSpan.TotalMilliseconds > 10)
			{
				if (view_widget.HDR != stages.CurrentImage)
					view_widget.HDR = stages.CurrentImage;
				else
					view_widget.UpdatePicture();

				lastupdate = DateTime.Now;
			}
		}
		
		while (Gtk.Application.EventsPending())
			Gtk.Application.RunIteration();
		
		if (stages.CancelPending) e.Cancel = true;
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		if (stages.TheUIState != UIState.Free)
		{
			stages.SetCancelPending();
		}
		Application.Quit ();
		a.RetVal = true;
	}
	
	bool ImportRawAndLoadingReporter(double progress, string status)
	{
		progressbar.Fraction = progress;
		progressbar.Text = status;
		while (Application.EventsPending()) Application.RunIteration();

		return (!stages.CancelPending);
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
		if (stages.TheUIState == UIState.Loading)
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
				MemoryStream ms = ImportRaw(FileName, ImportRawAndLoadingReporter);
				stages.LoadRaw(ms, PreScale, ImportRawAndLoadingReporter);
				
				ms.Close();
				ms.Dispose();
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
		stages.SetCancelPending();
	}
	
	protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
	{
		stages.CancelAll();
		Application.Quit();
	}
	
	protected virtual void OnAboutActionActivated (object sender, System.EventArgs e)
	{
		AboutBox abb = new AboutBox();
		abb.Run();
		abb.Destroy();
	}
	
	protected virtual void OnUpdateDuringProcessingActionToggled (object sender, System.EventArgs e)
	{
		view_widget.InstantUpdate = this.UpdateDuringProcessingAction.Active;
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
			stages.LaunchStageLoading(fcd.Filename);
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
			StageOperation so = stages.CreateAndAddNewStageOperation(ExtendedStage._StageOperationTypes[index]);
			
			stage_vbox.CheckResize();
		}
	}

	[GLib.ConnectBefore()]
	protected void OnStageVboxExposeEvent (object o, Gtk.ExposeEventArgs args)
	{
		int l, t, w, h;
		w = stage_vbox.Allocation.Width; h = stage_vbox.Allocation.Height;
		l = stage_vbox.Allocation.Left; t = stage_vbox.Allocation.Top;


		Gtk.Style.PaintBox(stage_vbox.Style, stage_vbox.GdkWindow, Gtk.StateType.Active, 
			Gtk.ShadowType.In, new Gdk.Rectangle(l, t, w, h), this, null,
		l + 1, t + 1, w - 2, h - 2);
	}

	protected void OnStageVboxSizeAllocated (object o, Gtk.SizeAllocatedArgs args)
	{
		QueueDraw();
	}

	protected void OnRenderToActionActivated (object sender, System.EventArgs e)
	{
		Gtk.FileChooserDialog fcd = new Gtk.FileChooserDialog("Render image to", 
		                                                      this, 
		                                                      FileChooserAction.Save,
		                                                      "Cancel", ResponseType.Cancel,
		                                                      "Render", ResponseType.Accept);
		
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
		
		string type = "", filename = "";
		bool accept = false;
		
		if (fcd.Run() == (int)Gtk.ResponseType.Accept)
		{
			if (fcd.Filter == ffs[0])
				type = "jpeg";
			if (fcd.Filter == ffs[1])
				type = "png";
			if (fcd.Filter == ffs[2])
				type = "bmp";
			accept = true;
			filename = fcd.Filename;
		}
		fcd.Destroy();

		if (accept)
		{
			/*
			// Rendering
			RenderingProgressWindow rpw = new RenderingProgressWindow();
			rpw.ImageName = this.FileName;
			rpw.Show();
			
			FloatPixmap renderDest = new FloatPixmap(src_img);
			
			// Rendering			
			stages.ApplyAllOperations(renderDest);
			
			// Drawing to pixbuf and saving to file
			Gdk.Pixbuf rp = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 8, renderDest.width, renderDest.height);

			renderDest.DrawToPixbuf(rp, 
				delegate (double progress) {
					rpw.SetStatusAndProgress(progress, "Saving image...");
					while (Application.EventsPending()) Application.RunIteration();
					return true;
				}
			);
		
			// TODO Can't be used currently cause of buggy Gtk#
			//rp.Savev(filename, type, new string[] { "quality" }, new string[] { "95" });
	
			rp.Save(filename, type);
				
			rpw.Hide();
			rpw.Dispose();
			
			if (rp != null)
				rp.Dispose();
			*/
		}
	}

}