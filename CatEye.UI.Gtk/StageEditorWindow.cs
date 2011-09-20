using System;
using System.IO;
using System.Xml;
using System.Threading;
using System.Collections.Generic;
using Gtk;
using CatEye;
using CatEye.Core;
using CatEye.UI.Base;
using CatEye.UI.Gtk;
using CatEye.UI.Gtk.Widgets;

public partial class StageEditorWindow : Gtk.Window
{
	private Thread mStageThread;
	private volatile ExtendedStage mStage;
	private volatile bool mStageThreadStopFlag = false;
	private volatile bool mColorsUpdatingPending = false;
	private DateTime mColorsUpdatingPendingFrom;
	private Widget[] mWidgetsWhichColorsAreChanged = new Widget[] {};
	
	private DateTime mLastUpdate;
	private FrozenPanel mFrozenPanel;
	private bool mIsDestroyed;
	
	private Type[] mStageOperationTypes;
	private StageOperationFactory mStageOperationFactory;
	private StageOperationParametersFactory mStageOperationParametersFactory;
	private StageOperationParametersEditorFactory mStageOperationParametersEditorFactory;
	private StageOperationHolderFactory mStageOperationHolderFactory;
	private BitmapCoreFactory mFloatBitmapGtkFactory;
	
	private static int mDelayBeforeUpdate = 200;
	
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
				if (mStage.Prescale > 1) t += " (1/" + mStage.Prescale + ")";
			}
			
			t += " — " + MainClass.APP_NAME;
			Title = t;
		}
		else
			Title = MainClass.APP_NAME;
	}
	
	private void StageThreadStart()
	{
		DateTime lastUpdateQueuedTime = DateTime.Now;
			
		mStage.UpdateQueued += delegate {
			lastUpdateQueuedTime = DateTime.Now;
		};
		
		// Image processor cycle
		while (!mStageThreadStopFlag)
		{
			mStage.CancelProcessing();
			if ((DateTime.Now - lastUpdateQueuedTime).TotalMilliseconds > mDelayBeforeUpdate)
			{
				lastUpdateQueuedTime = DateTime.Now;
				mStage.ProcessPending();
			}
			Thread.Sleep(30);
			if (mColorsUpdatingPending)
			{
				mColorsUpdatingPending = false;
				Application.Invoke(delegate {
					mWidgetsWhichColorsAreChanged = WindowsSystemColorsHelper.PaintIntoWindowsColors(this, mWidgetsWhichColorsAreChanged);

					Gdk.Color controlDark_color = new Gdk.Color(System.Drawing.SystemColors.ControlDark.R,
					                                            System.Drawing.SystemColors.ControlDark.G,
					                                            System.Drawing.SystemColors.ControlDark.B);
					stage_vbox.ModifyBg(StateType.Normal, controlDark_color);
				});
			}
		}
	}
	
	protected void SetColorsUpdatingPending()
	{
		mColorsUpdatingPending = true;
	}
	
	public StageEditorWindow (Type[] stageOperationTypes,
							  StageOperationFactory stageOperationFactory, 
							  StageOperationParametersFactory stageOperationParametersFactory,
							  StageOperationParametersEditorFactory stageOperationParametersEditorFactory, 
							  StageOperationHolderFactory stageOperationHolderFactory, 
							  BitmapCoreFactory floatBitmapGtkFactory) : base(Gtk.WindowType.Toplevel)
	{
		mStageOperationTypes = stageOperationTypes;
		mStageOperationFactory = stageOperationFactory;
		mStageOperationParametersFactory = stageOperationParametersFactory;
		mStageOperationParametersEditorFactory = stageOperationParametersEditorFactory;
		mStageOperationHolderFactory = stageOperationHolderFactory;
		mFloatBitmapGtkFactory = floatBitmapGtkFactory;

		// ** Preparing UI **
		Build ();
		
		mFrozenPanel = new FrozenPanel();
		mFrozenPanel.UnfreezeButtonClicked  += delegate {
			mStage.FrozenAt = null;
		};
		stage_vbox.Add(mFrozenPanel);

		//mWidgetsWhichColorsAreChanged = WindowsSystemColorsHelper.PaintIntoWindowsColors(this, mWidgetsWhichColorsAreChanged);
		SetColorsUpdatingPending();
		
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

		// Setting view widget events
		viewWidget.ExposeEvent += HandleViewWidgetExposeEvent;
		viewWidget.MousePositionChanged += HandleViewWidgetMousePositionChanged;
		viewWidget.MouseButtonStateChanged += HandleViewWidgetMouseButtonStateChanged;
		
		// Setting zoom widget events
		zoomWidget.ValueChanged += delegate {
			mStage.ZoomValue = zoomWidget.Value;
		};
		
		// ** Preparing stage and its thread **
		mStageThread = new Thread(StageThreadStart);
		mStageThread.Priority = ThreadPriority.BelowNormal;
		
		mStage = new ExtendedStage(
			mStageOperationFactory, 
			mStageOperationParametersFactory,
			mStageOperationParametersEditorFactory, 
			mStageOperationHolderFactory, 
			mFloatBitmapGtkFactory);

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

		mStage.ZoomValue = zoomWidget.Value;
		
		mStageThread.Start();
		
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
			WindowsSystemColorsHelper.PaintIntoWindowsColors(md, null);
			
			md.Title = MainClass.APP_NAME;
			md.Run();
			md.Destroy();
		}
		
	}
	
#region Handlers called from other thread. 
	// Each handler here should contain Application.Invoke
	
	void HandleStageItemIndexChanged (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			ArrangeVBoxes();
		});
	}
	void HandleStageRawFileNameChanged (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			UpdateTitle();
		});
	}
	void HandleStageStageFileNameChanged (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			UpdateTitle();
		});
	}
	void HandleStagePrescaleChanged (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			UpdateTitle();
		});
	}
	void HandleStageImageChanged (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			viewWidget.Image = (FloatBitmapGtk)mStage.CurrentImage;
		});
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
			enqueueRenderAction.Sensitive = File.Exists(mStage.RawFileName);
		});
	}

	void HandleStageImageUpdatingCompleted (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			status_label.Text = "Image is updated successfully";
			enqueueRenderAction.Sensitive = File.Exists(mStage.RawFileName);
			viewWidget.UpdatePicture();
		});
	}

	void HandleStageImageLoadingCompleted (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			viewWidget.CenterImagePanning();
			status_label.Text = "Image is loaded successfully";
		});
	}

	void HandleStageOperationAddedToStage (object sender, StageOperationParametersEventArgs e)
	{
		StageOperationHolderWidget sohw = (StageOperationHolderWidget)mStage.Holders[e.Target];
		Application.Invoke(delegate {

			stage_vbox.Add(sohw);
			((Gtk.Box.BoxChild)stage_vbox[sohw]).Fill = false;
			((Gtk.Box.BoxChild)stage_vbox[sohw]).Expand = false;

			WindowsSystemColorsHelper.PaintIntoWindowsColors(sohw, new Widget[] {});

			sohw.Show();
			ArrangeVBoxes();
			
		});	
	}

	void HandleStageOperationRemovedFromStage (object sender, StageOperationParametersEventArgs e)
	{
		StageOperationHolderWidget sohw = (StageOperationHolderWidget)mStage.Holders[e.Target];
		Application.Invoke(delegate {
			
			stage_vbox.Remove(sohw);
			sohw.Hide();
			sohw.Dispose();
			
			ArrangeVBoxes();
		});
	}
	void HandleStageUIStateChanged (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			if (mStage.TheUIState == UIState.Idle)
			{
				loadRawAction.Sensitive = true;
				loadStageAction.Sensitive = true;
				saveStageAsAction.Sensitive = true;
				enqueueRenderAction.Sensitive = File.Exists(mStage.RawFileName);
				cancel_button.Visible = false;
				progressbar.Visible = false;
	
			}
			else if (mStage.TheUIState == UIState.Loading)
			{
				loadRawAction.Sensitive = false;
				loadStageAction.Sensitive = false;
				saveStageAsAction.Sensitive = false;
				enqueueRenderAction.Sensitive = false;
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
				enqueueRenderAction.Sensitive = File.Exists(mStage.RawFileName);
				cancel_button.Visible = false;
				progressbar.Visible = true;
				progressbar.Fraction = 0;
				progressbar.Text = "";
			}
			
		});
	}

	void HandleProgress(object sender, ReportStageProgressMessageEventArgs ea)
	{
		Application.Invoke(delegate {
			try
			{
				progressbar.Visible = ea.ShowProgressBar;
				progressbar.Fraction = ea.Progress;
				progressbar.Text = (ea.Progress * 100).ToString("0") + "%";
				status_label.Text = ea.Status;
				
				if (ea.Update && UpdateDuringProcessingAction.Active)
				{
					if ((DateTime.Now - mLastUpdate).TotalMilliseconds / viewWidget.UpdateTimeSpan.TotalMilliseconds > 5)
					{
						if (viewWidget.Image != mStage.CurrentImage)
							viewWidget.Image = (FloatBitmapGtk)mStage.CurrentImage;
						else
							viewWidget.UpdatePicture();
		
						mLastUpdate = DateTime.Now;
					}
				}
			}
			catch (Exception ex)
			{
				// Any exception is harmless here
#if DEBUG
				Console.WriteLine("Exception during HandleProgress: " + ex.Message + "\n" + ex.StackTrace);
#endif
			}
		});
	}	
#endregion	
	protected void ArrangeVBoxes()
	{
		// Arranging stage 2
		for (int i = 0; i < mStage.StageQueue.Count; i++)
		{
			StageOperationHolderWidget sohw = (StageOperationHolderWidget)mStage.Holders[mStage.StageQueue[i]];
			((Gtk.Box.BoxChild)stage_vbox[sohw]).Position = i;
		}
	}

	bool HandleViewWidgetMouseButtonStateChanged (object sender, int x, int y, uint button_id, bool is_down)
	{
		int x_rel = x - viewWidget.CurrentImagePosition.X;
		int y_rel = y - viewWidget.CurrentImagePosition.Y;

		if (mStage.ReportEditorMouseButton(x_rel, 
									 y_rel, 
									 viewWidget.CurrentImagePosition.Width, 
									 viewWidget.CurrentImagePosition.Height, 
									 button_id, is_down))
		{
			viewWidget.QueueDraw();
			return true;
		}
		return false;
	}

	bool HandleViewWidgetMousePositionChanged (object sender, int x, int y)
	{
		int x_rel = x - viewWidget.CurrentImagePosition.X;
		int y_rel = y - viewWidget.CurrentImagePosition.Y;

		if (mStage.ReportEditorMousePosition(x_rel, 
									 y_rel, 
									 viewWidget.CurrentImagePosition.Width, 
									 viewWidget.CurrentImagePosition.Height))
		{
			viewWidget.QueueDraw();
			return true;
		}
		return false;
	}

	public bool IsDestroyed { get { return mIsDestroyed; } }
	
	void HandleViewWidgetExposeEvent (object o, ExposeEventArgs args)
	{
		mStage.DrawEditor(viewWidget);
	}
	
	public void LoadRaw(string filename, int prescale)
	{
		mStage.AskLoadImage(filename, prescale);
	}
	
	public void LoadCEStage(string filename)
	{
		mStage.LoadStage(filename);
	}
	
	protected void LoadRawImageActionPicked()
	{
		if (mStage.TheUIState == UIState.Loading)
		{
			Gtk.MessageDialog md = new Gtk.MessageDialog(this, DialogFlags.Modal,
			                                             MessageType.Error, ButtonsType.Ok, 
			                                             "Can not start DCRaw process");
			WindowsSystemColorsHelper.PaintIntoWindowsColors(md, null);
			md.Title = MainClass.APP_NAME;
			md.Run();
			md.Destroy();
		}
		else
		{
			Gtk.FileChooserDialog fcd = new Gtk.FileChooserDialog("Open RAW image", 
			                                                      this, 
			                                                      FileChooserAction.Open,
			                                                      "Cancel", ResponseType.Cancel,
			                                                      "Open", ResponseType.Accept);
			WindowsSystemColorsHelper.PaintIntoWindowsColors(fcd, null);

			// Filter for RAWs
			FileFilter ff = new FileFilter();
			
			ff.AddCustom(FileFilterFlags.Filename, delegate (Gtk.FileFilterInfo ffi) {
				return DCRawConnection.IsRaw(ffi.Filename);
			});
			ff.Name = "RAW image";
	
			fcd.AddFilter(ff);
			
			// Setting current folder to stage file folder or to RAW file folder
			if (mStage.StageFileName != null && mStage.StageFileName != "" 
					&& !fcd.SetCurrentFolder(System.IO.Path.GetDirectoryName(mStage.StageFileName)))
				fcd.SetCurrentFolder(System.IO.Path.GetDirectoryName(mStage.RawFileName));
	
			
			
			// Adding preview widget
			RawPreviewWidget rpw = new RawPreviewWidget();
			fcd.PreviewWidget = rpw;
			fcd.UpdatePreview += delegate {
				rpw.Filename = fcd.Filename;
				fcd.PreviewWidgetActive = rpw.FileIsGood;
			};
			fcd.SelectionChanged += delegate {
				rpw.Filename = fcd.Filename;
				fcd.PreviewWidgetActive = rpw.FileIsGood;
			};

			if (mStage.RawFileName != null)
			{
				fcd.SetFilename(mStage.RawFileName);
				rpw.Filename = mStage.RawFileName;
				fcd.PreviewWidgetActive = rpw.FileIsGood;
			}
			
			// Adding prescale widget
			PreScaleSelectorWidget pssw = new PreScaleSelectorWidget();
			pssw.Value = 2;
			if (mStage.Prescale != 0) pssw.Value = mStage.Prescale;
			fcd.ExtraWidget = pssw;
			
			string filename = ""; int prescale = 2;
			bool ok = false;
			if (fcd.Run() == (int)Gtk.ResponseType.Accept)
			{
				ok = true;
				filename = fcd.Filename;
				prescale = pssw.Value;
			}
			fcd.Destroy();
			if (ok)
			{
				mStage.AskLoadImage(filename, prescale);
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
		cancel_button.Sensitive = false;
		mStage.CancelLoading();
	}
	
	private void CloseStageEditor()
	{
		// Stopping stage thread
		mStageThreadStopFlag = true;
		mStage.CancelAll();
		
		Destroy();

		mIsDestroyed = true;
	}
	
	protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
	{
		CloseStageEditor();
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		CloseStageEditor();
		a.RetVal = true;
	}
	
	protected virtual void OnAboutActionActivated (object sender, System.EventArgs e)
	{
		AboutBox abb = new AboutBox();
		WindowsSystemColorsHelper.PaintIntoWindowsColors(abb, null);
		abb.Run();
		abb.Destroy();
	}
	
	protected virtual void OnUpdateDuringProcessingActionToggled (object sender, System.EventArgs e)
	{
		viewWidget.InstantUpdate = this.UpdateDuringProcessingAction.Active;
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
		
		WindowsSystemColorsHelper.PaintIntoWindowsColors(fcd, null);
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
		WindowsSystemColorsHelper.PaintIntoWindowsColors(fcd, null);
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

				string raw_filename; int prescale = 2;
				if (mStage.Prescale != 0) prescale = mStage.Prescale;
				
				if (MainClass.FindRawsForCestageAndAskToOpen(fn, out raw_filename, ref prescale))
				{
					mStage.AskLoadImage(raw_filename, prescale);
				}
			}
			catch (StageDeserializationException sdex)
			{
				Gtk.MessageDialog md = new Gtk.MessageDialog(
					this, DialogFlags.Modal,
					MessageType.Error, ButtonsType.Ok, 
					false, "Can't load stage from the file \"{0}\".\n{1}", fn, sdex.Message);
				WindowsSystemColorsHelper.PaintIntoWindowsColors(md, null);
				
				md.Title = MainClass.APP_NAME;
				
				md.Run();
				md.Destroy();
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


		Gtk.Style.PaintBox(stage_vbox.Style, stage_vbox.GdkWindow, Gtk.StateType.Normal, 
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
		// Adding image filters
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
		
		// Adding prescaling widget
		PreScaleSelectorWidget pssw = new PreScaleSelectorWidget();
		pssw.Value = mStage.Prescale;
		fcd.ExtraWidget = pssw;
		
		int prescale = 2;
		string dest_type = "", fn = "";
		bool accept = false;

		fcd.CurrentName = System.IO.Path.GetFileNameWithoutExtension(mStage.RawFileName);
		fcd.SetCurrentFolder(System.IO.Path.GetDirectoryName(mStage.RawFileName));
		
		WindowsSystemColorsHelper.PaintIntoWindowsColors(fcd, null);
		if (fcd.Run() == (int)Gtk.ResponseType.Accept)
		{
			fn = fcd.Filename;
			prescale = pssw.Value;
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
			
			// Sending remote command to add stage to queue
			string command = "AddToQueue_StageData";
			string[] arguments = new string[] 
			{
				mStage.SaveStageToString(), 
				mStage.RawFileName, 
				fn, 
				dest_type, 
				prescale.ToString()
			};
			MainClass.RemoteControlService.SendCommand(RemoteControlService.PackCommand(command, arguments));
			
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
		MainClass.RenderingQueueWindow.Visible = RenderingQueueAction.Active;
	}

	protected void OnViewActionActivated (object sender, System.EventArgs e)
	{
		RenderingQueueAction.Visible = MainClass.RenderingQueue.IsProcessingItem;
		RenderingQueueAction.Active = MainClass.RenderingQueueWindow.Visible;
	}

	protected void OnShown (object sender, System.EventArgs e)
	{
		// Colors in Windows OS
		// TODO: Add Windows check
		SetColorsUpdatingPending();
		Microsoft.Win32.SystemEvents.DisplaySettingsChanged += delegate {
			SetColorsUpdatingPending();
		};
	}
}