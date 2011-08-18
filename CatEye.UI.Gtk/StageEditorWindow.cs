using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Gtk;
using CatEye;
using CatEye.Core;
using CatEye.UI.Base;
using CatEye.UI.Gtk;
using CatEye.UI.Gtk.Widgets;

public partial class StageEditorWindow : Gtk.Window
{
	private ExtendedStage mStage;
	private DateTime mLastUpdate;
	private FrozenPanel mFrozenPanel;
	private Type[] mStageOperationTypes;
	
	private void UpdateTitle()
	{
		if (mStage.RawFileName != null || mStage.StageFileName != null)
		{
			string t = "";
			if (mStage.StageFileName != null) t += System.IO.Path.GetFileName(mStage.StageFileName);
			if (mStage.StageFileName != null && mStage.RawFileName != null) t += " / ";
			if (mStage.RawFileName != null) 
			{
				t += System.IO.Path.GetFileName(mStage.RawFileName);
				if (mStage.Prescale > 1) t += " [%" + mStage.Prescale + "]";
			}
			
			t += " — " + MainClass.APP_NAME;
			Title = t;
		}
		else
			Title = MainClass.APP_NAME;
	}
	

	public StageEditorWindow (ExtendedStage stage, Type[] stageOperationTypes) : base(Gtk.WindowType.Toplevel)
	{
		Build ();

		// Creating stage operations and stages
		mStage = stage;
		
		mFrozenPanel = new FrozenPanel();
		mFrozenPanel.UnfreezeButtonClicked  += delegate {
			mStage.FrozenAt = null;
		};
		stage_vbox.Add(mFrozenPanel);
		
		mStageOperationTypes = stageOperationTypes;
		
		// Preparing stage operation adding store
		ListStore ls = new ListStore(typeof(string), typeof(int));
		for (int i = 0; i < mStageOperationTypes.Length; i++)
		{
			string desc = StageOperationDescriptionAttribute.GetSOName(mStageOperationTypes[i]);
			if (desc == null) desc = mStageOperationTypes[i].Name;
		
			ls.AppendValues(desc, i);
		}
		stageOperationToAdd_combobox.Model = ls;
		Gtk.TreeIter ti;
		ls.GetIterFirst(out ti);
		stageOperationToAdd_combobox.SetActiveIter(ti);
		
		// Setting stage events
		mStage.OperationFrozen += HandleStageOperationFrozen;
		mStage.OperationDefrozen += HandleStageOperationDefrozen;
		mStage.ImageChanged += HandleStageImageChanged;
		mStage.ItemAdded += HandleStageOperationAddedToStage;
		mStage.ItemRemoved += HandleStageOperationRemovedFromStage;
		mStage.ItemIndexChanged += HandleStageItemIndexChanged;
		
		mStage.UIStateChanged += HandleStageUIStateChanged;
		mStage.ProgressMessageReport += HandleProgress;

		mStage.ImageLoadingCompleted += HandleStageImageLoadingCompleted;
		mStage.ImageUpdatingCompleted += HandleStageImageUpdatingCompleted;
		mStage.ImageLoadingCancelled += HandleStageImageLoadingCancelled;
		
		mStage.RawFileNameChanged += HandleStageRawFileNameChanged;
		mStage.StageFileNameChanged += HandleStageStageFileNameChanged;
		mStage.PreScaleChanged += HandleStagePrescaleChanged;

		// Loading default stage
		string mylocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
		string defaultstage = mylocation + System.IO.Path.DirectorySeparatorChar.ToString() + "default.cestage";
		if (System.IO.File.Exists(defaultstage))
		{
			mStage.LoadStage(defaultstage, false);
		}
		else
		{
			Gtk.MessageDialog md = new Gtk.MessageDialog(this, DialogFlags.Modal,
			                                             MessageType.Warning, ButtonsType.Ok, 
			                                             "Can not find default.cestage");
			md.Title = MainClass.APP_NAME;
			md.Run();
			md.Destroy();
		}
		
		// Setting view widget events
		view_widget.ExposeEvent += DrawCurrentStageOperationEditor;
		view_widget.MousePositionChanged += HandleView_widgetMousePositionChanged;
		view_widget.MouseButtonStateChanged += HandleView_widgetMouseButtonStateChanged;
		
		// Setting zoom widget events
		mStage.ZoomValue = zoomWidget.Value;
		zoomWidget.ValueChanged += delegate {
			mStage.ZoomValue = zoomWidget.Value;
		};
	}
	void HandleStageItemIndexChanged (object sender, EventArgs e)
	{
		ArrangeVBoxes();
	}
	void HandleStageRawFileNameChanged (object sender, EventArgs e)
	{
		UpdateTitle();
	}
	void HandleStageStageFileNameChanged (object sender, EventArgs e)
	{
		UpdateTitle();
	}
	void HandleStagePrescaleChanged (object sender, EventArgs e)
	{
		UpdateTitle();
	}
	void HandleStageImageChanged (object sender, EventArgs e)
	{
		view_widget.HDR = (FloatBitmapGtk)mStage.CurrentImage;
	}
	void HandleStageOperationFrozen (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			int index = mStage.IndexOf(mStage.FrozenAt);
			((Gtk.Box.BoxChild)stage_vbox[mFrozenPanel]).Position = index + 1;
			((Gtk.Box.BoxChild)stage_vbox[mFrozenPanel]).Fill = true;
			((Gtk.Box.BoxChild)stage_vbox[mFrozenPanel]).Expand = false;
			mFrozenPanel.Show();

			stageOperationAdding_hbox.Sensitive = false;
		});
	}
	void HandleStageOperationDefrozen (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			mFrozenPanel.Hide();
			stageOperationAdding_hbox.Sensitive = true;
		});
	}

	void HandleStageImageLoadingCancelled (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			status_label.Text = "Image loading is cancelled";
		});
	}

	void HandleStageImageUpdatingCompleted (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			status_label.Text = "Image is updated successfully";
			view_widget.UpdatePicture();
		});
	}

	void HandleStageImageLoadingCompleted (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			view_widget.CenterImagePanning();
			status_label.Text = "Image is loaded successfully";
		});
	}
	
	protected void ArrangeVBoxes()
	{
		// Arranging stage 2
		for (int i = 0; i < mStage.StageQueue.Count; i++)
		{
			StageOperationHolderWidget sohw = (StageOperationHolderWidget)mStage.Holders[mStage.StageQueue[i]];
			((Gtk.Box.BoxChild)stage_vbox[sohw]).Position = i;
		}
	}
	
	void HandleStageOperationAddedToStage (object sender, StageOperationParametersEventArgs e)
	{
		StageOperationHolderWidget sohw = (StageOperationHolderWidget)mStage.Holders[e.Target];

		stage_vbox.Add(sohw);
		((Gtk.Box.BoxChild)stage_vbox[sohw]).Fill = false;
		((Gtk.Box.BoxChild)stage_vbox[sohw]).Expand = false;

		sohw.Show();
		ArrangeVBoxes();		
		
	}

	void HandleStageOperationRemovedFromStage (object sender, StageOperationParametersEventArgs e)
	{
		StageOperationHolderWidget sohw = (StageOperationHolderWidget)mStage.Holders[e.Target];
		
		stage_vbox.Remove(sohw);
		sohw.Hide();
		sohw.Dispose();
		
		ArrangeVBoxes();

	}

	void HandleStageUIStateChanged (object sender, EventArgs e)
	{
		if (mStage.TheUIState == UIState.Idle)
		{
			loadRawAction.Sensitive = true;
			loadStageAction.Sensitive = true;
			saveStageAsAction.Sensitive = true;
			renderToAction.Sensitive = true;
			cancel_button.Visible = false;
			progressbar.Visible = false;

		}
		else if (mStage.TheUIState == UIState.Loading)
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
		else if (mStage.TheUIState == UIState.Processing)
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

		if (mStage.ReportEditorMouseButton(x_rel, 
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

		if (mStage.ReportEditorMousePosition(x_rel, 
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
		mStage.DrawEditor(view_widget);
	}

	void HandleProgress(object sender, ReportStageProgressMessageEventArgs ea)
	{
		progressbar.Visible = ea.ShowProgressBar;
		progressbar.Fraction = ea.Progress;
		progressbar.Text = (ea.Progress * 100).ToString("0") + "%";
		status_label.Text = ea.Status;
		
		if (ea.Update && UpdateDuringProcessingAction.Active)
		{
			if ((DateTime.Now - mLastUpdate).TotalMilliseconds / view_widget.UpdateTimeSpan.TotalMilliseconds > 10)
			{
				if (view_widget.HDR != mStage.CurrentImage)
					view_widget.HDR = (FloatBitmapGtk)mStage.CurrentImage;
				else
					view_widget.UpdatePicture();

				mLastUpdate = DateTime.Now;
			}
		}
		
		while (Gtk.Application.EventsPending())
			Gtk.Application.RunIteration();
		
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		if (mStage.TheUIState != UIState.Idle)
		{
			mStage.CancelAll();
			//MainClass.rob.rq.CancelAll();
		}
		//Main.Quit();
		//Application.Quit ();
		MainClass.Quit();
		a.RetVal = true;
	}
/*	
	bool ImportRawAndLoadingReporter(double progress, string status)
	{
		progressbar.Visible = true;
		progressbar.Fraction = progress;
		progressbar.Text = status;
		while (Application.EventsPending()) Application.RunIteration();
		
		if (stages.CancelProcessingPending || stages.CancelLoadingPending)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
*/	

/*	
	private IBitmapCore ImageLoader(string filename, int downscale_by, ProgressMessageReporter callback)
	{
		MemoryStream ms = ImportRaw(filename, callback);
		if (ms != null)
		{
			IBitmapCore ibc = LoadRaw(ms, downscale_by, callback);
				
			ms.Close();
			ms.Dispose();
			return ibc;
		}
		else
			return null;
	}
*/
	
	protected void LoadRawImageActionPicked()
	{
		if (mStage.TheUIState == UIState.Loading)
		{
			Gtk.MessageDialog md = new Gtk.MessageDialog(this, DialogFlags.Modal,
			                                             MessageType.Error, ButtonsType.Ok, 
			                                             "Can not start DCRaw process");
			md.Title = MainClass.APP_NAME;
			md.Run();
			md.Destroy();
		}
		else
		{
			RawImportDialog rid = new RawImportDialog();
			
			if (mStage.RawFileName != null) rid.Filename = mStage.RawFileName;
			if (mStage.Prescale != 0) rid.Prescale = mStage.Prescale;
			
			bool ok = false;
			string fn = ""; int ps = 1;
			
			if (rid.Run() == (int)Gtk.ResponseType.Accept)
			{
				ok = true;
				fn = rid.Filename;
				ps = rid.Prescale;
			}
			rid.Destroy();
			
			if (ok)
			{
				mStage.LoadImage(fn, ps);
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
		mStage.CancelLoading();
	}
	
	protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
	{
		mStage.CancelAll();
		//MainClass.rq.CancelAll();
		Destroy();
		MainClass.Quit();
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
		
		fcd.CurrentName = System.IO.Path.GetFileNameWithoutExtension(mStage.RawFileName);
		fcd.SetCurrentFolder(System.IO.Path.GetDirectoryName(mStage.RawFileName));
		
		if (fcd.Run() == (int)Gtk.ResponseType.Accept)
		{
			string fn = fcd.Filename;
			if (System.IO.Path.GetExtension(fn).ToLower() != ".cestage")
				fn += ".cestage";
			
			mStage.SaveStage(fn);
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
		fcd.SetCurrentFolder(System.IO.Path.GetDirectoryName(mStage.RawFileName));
		
		string fn = "";
		bool ok = false;
		if (fcd.Run() == (int)Gtk.ResponseType.Accept)
		{
			ok = true;
			fn = fcd.Filename;
		}
		fcd.Destroy();
		if (ok)
		{
			try
			{
				mStage.LoadStage(fn);
			}
			catch (StageDeserializationException sdex)
			{
				Gtk.MessageDialog md = new Gtk.MessageDialog(
					this, DialogFlags.Modal,
					MessageType.Error, ButtonsType.Ok, 
					false, "Can't load stage from the file \"{0}\".\n{1}", fn, sdex.Message);
				md.Title = MainClass.APP_NAME;
				
				md.Run();
				md.Destroy();
			}
			string raw_filename; int prescale;
			if (MainClass.FindRawsForCestageAndAskToOpen(fn, out raw_filename, out prescale))
			{
				mStage.LoadImage(raw_filename, prescale);
			}
		}
	}
	
	protected void OnAddStageOperationButtonClicked (object sender, System.EventArgs e)
	{
		if (mStage.FrozenAt == null)
		{
			Gtk.TreeIter ti;
			stageOperationToAdd_combobox.GetActiveIter(out ti);
			int index = (int)stageOperationToAdd_combobox.Model.GetValue(ti, 1);
			mStage.CreateAndAddNewItem(mStageOperationTypes[index]);
				
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
		ffs[0].Name = "JPEG image";
		ffs[0].AddCustom(FileFilterFlags.Filename, delegate (Gtk.FileFilterInfo ffi) {
			return (System.IO.Path.GetExtension(ffi.Filename).ToLower() == ".jpg") ||
				   (System.IO.Path.GetExtension(ffi.Filename).ToLower() == ".jpeg");
		});
		
		ffs[1] = new FileFilter();
		ffs[1].Name = "PNG image";
		ffs[1].AddCustom(FileFilterFlags.Filename, delegate (Gtk.FileFilterInfo ffi) {
			return System.IO.Path.GetExtension(ffi.Filename).ToLower() == ".png";
		});

		ffs[2] = new FileFilter();
		ffs[2].Name = "Plain 24 bpp bitmap (BMP)";
		ffs[2].AddCustom(FileFilterFlags.Filename, delegate (Gtk.FileFilterInfo ffi) {
			return System.IO.Path.GetExtension(ffi.Filename).ToLower() == ".bmp";
		});

		fcd.AddFilter(ffs[0]);
		fcd.AddFilter(ffs[1]);
		fcd.AddFilter(ffs[2]);
		
		string dest_type = "", fn = "";
		bool accept = false;

		fcd.CurrentName = System.IO.Path.GetFileNameWithoutExtension(mStage.RawFileName);
		fcd.SetCurrentFolder(System.IO.Path.GetDirectoryName(mStage.RawFileName));
		
		if (fcd.Run() == (int)Gtk.ResponseType.Accept)
		{
			fn = fcd.Filename;

			if (fcd.Filter == ffs[0])
			{
				if (System.IO.Path.GetExtension(fn).ToLower() != ".jpeg" &&
					System.IO.Path.GetExtension(fn).ToLower() != ".jpg")
					fn += ".jpeg";
				
				dest_type = "jpeg";
			}
			if (fcd.Filter == ffs[1])
			{
				if (System.IO.Path.GetExtension(fn).ToLower() != ".png")
					fn += ".png";

				dest_type = "png";
			}
			if (fcd.Filter == ffs[2])
			{
				if (System.IO.Path.GetExtension(fn).ToLower() != ".bmp")
					fn += ".bmp";

				dest_type = "bmp";
			}
			accept = true;
		}
		fcd.Destroy();

		if (accept)
		{
			/*
			Stage stg = new Stage(MainClass.StageOperationFactory, 
				MainClass.StageOperationParametersFactoryFromID,
				MainClass.ImageLoader);
			
			for (int i = 0; i < stages.StageQueue.Length; i++)
			{
				stg.Add((StageOperationParameters)stages.StageQueue[i].Clone());
			}
			
			MainClass.rq.Add(stg, stages.RawFileName, fn, dest_type);
			*/
			if (RemotingObject.rob == null)
			{
				
			}
			
			//RemotingObject.AssureQueueServiceIsRunning();
			RemotingObject.RunQueueServiceOrConnectToIt();
			RemotingObject.rob.AddToQueue(mStage.SaveStageToString(), mStage.RawFileName, mStage.Prescale, fn, dest_type);
			
			//MainClass.rqwin.Show();
			
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
	
	
	protected void OnRenderingQueueActionToggled (object sender, System.EventArgs e)
	{
		//RemotingObject.AssureQueueServiceIsRunning();
		RemotingObject.RunQueueServiceOrConnectToIt();
		RemotingObject.rob.rqwin.Visible = RenderingQueueAction.Active;
	}

	protected void OnViewActionActivated (object sender, System.EventArgs e)
	{
		if (RemotingObject.rob != null && RemotingObject.rob.rq.IsProcessing)
		{
			RenderingQueueAction.Visible = true;
			RenderingQueueAction.Active = RemotingObject.rob.rqwin.Visible;
		}
		else
		{
			RenderingQueueAction.Visible = false;
		}
			
	}
}