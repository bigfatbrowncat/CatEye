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
		string t = "";
		if ((mStage.StageFileName == null || mStage.StageFileName == "") && mStage.RawFileName != null) 
		{
			t = System.IO.Path.GetFileName(mStage.RawFileName) + " — " + MainClass.APP_NAME;
		} 
		else if (mStage.RawFileName != null)
		{
			string ern = ReceiptsManager.ExtractReceiptName(mStage.StageFileName, mStage.RawFileName);
			if (ern != null && ern != "")
			{
				t = System.IO.Path.GetFileName(mStage.RawFileName) + " (" + ern + ") — " + MainClass.APP_NAME;
			}
			else 
				t = System.IO.Path.GetFileName(mStage.RawFileName) + " — " + MainClass.APP_NAME;
		} 
		else
		{
			t = MainClass.APP_NAME;
		}
		Title = t;
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
		}
	}
	
	protected void SetColorsUpdatingPending()
	{
		if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
			Environment.OSVersion.Platform == PlatformID.Win32S ||
			Environment.OSVersion.Platform == PlatformID.Win32Windows ||
			Environment.OSVersion.Platform == PlatformID.WinCE)
		{
			Gdk.Color controlDark_color = new Gdk.Color(System.Drawing.SystemColors.ControlDark.R,
		                                                System.Drawing.SystemColors.ControlDark.G,
		                                                System.Drawing.SystemColors.ControlDark.B);
			stage_vbox.ModifyBg(StateType.Normal, controlDark_color);
		}
		else
		{
			Gdk.Color c = Style.Mid(StateType.Normal);
			c.Red = (ushort)(c.Red * 0.85); 
			c.Green = (ushort)(c.Green * 0.85); 
			c.Blue = (ushort)(c.Blue * 0.85); 
			
			stage_vbox.ModifyBg(StateType.Normal, c);
		}
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

		SetColorsUpdatingPending();

		Microsoft.Win32.SystemEvents.DisplaySettingsChanged += delegate {
			SetColorsUpdatingPending();
		};
		
		// Setting view widget events
		viewWidget.ExposeEvent += HandleViewWidgetExposeEvent;
		viewWidget.MousePositionChanged += HandleViewWidgetMousePositionChanged;
		viewWidget.MouseButtonStateChanged += HandleViewWidgetMouseButtonStateChanged;
		
		// Setting zoom widget events
		zoomWidget.ValueChanged += HandleZoomWidgetValueChanged;
		
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
		mStage.ImageLoadingError += HandleStageImageLoadingError;
		mStage.RawFileNameChanged += HandleStageRawFileNameChanged;
		mStage.StageFileNameChanged += HandleStageStageFileNameChanged;
		mStage.PreScaleChanged += HandleStagePrescaleChanged;
		mStage.ViewNeedsUpdate += HandleStageViewNeedsUpdate;

		mStage.ZoomAfterPrescaleValue = zoomWidget.Value * mStage.Prescale;
		
		mStageThread.Start();

		/*
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
		*/
	}

	void HandleStageImageLoadingError (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			status_label.Text = "Image loading error";
			enqueueRenderAction.Sensitive = false;
			Gtk.MessageDialog md = new Gtk.MessageDialog(this, DialogFlags.Modal,
			                                             MessageType.Warning, ButtonsType.Ok, 
			                                             "Error occured during the image loading process. " +
				                                         "Maybe the file is corrupted or you have insufficient rights to access it.");
			
			md.Title = MainClass.APP_NAME;
			md.Run();
			md.Destroy();
		});		
	}

#region Handlers called from other thread. 
	// Each handler here should contain Application.Invoke
	void HandleStageViewNeedsUpdate (object sender, EventArgs e)
	{
		Application.Invoke(delegate {
			viewWidget.QueueDraw();
		});
	}
	
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
			if (mStage.Prescale == 0)
				zoomWidget.Sensitive = false;
			else
			{
				zoomWidget.Sensitive = true;
				zoomWidget.MaxValue = (1.0 / mStage.Prescale);
				mStage.ZoomAfterPrescaleValue = zoomWidget.Value * mStage.Prescale;	
			}
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
				openAction.Sensitive = true;
				saveReceiptAction.Sensitive = true;
				saveReceiptAsAction.Sensitive = true;
				enqueueRenderAction.Sensitive = File.Exists(mStage.RawFileName);
				cancel_button.Visible = false;
				progressbar.Visible = false;
	
			}
			else if (mStage.TheUIState == UIState.Loading)
			{
				openAction.Sensitive = false;
				saveReceiptAction.Sensitive = false;
				saveReceiptAsAction.Sensitive = false;
				enqueueRenderAction.Sensitive = false;
				cancel_button.Visible = true;
				cancel_button.Sensitive = true;
				progressbar.Visible = true;
				progressbar.Fraction = 0;
				progressbar.Text = "";
			}
			else if (mStage.TheUIState == UIState.Processing)
			{
				openAction.Sensitive = true;
				saveReceiptAction.Sensitive = true;
				saveReceiptAsAction.Sensitive = true;
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
			catch (Exception 
#if DEBUG
				ex
#endif
				)
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
			
		//	if (sohw.Requisition.Width > maxWidth) maxWidth = sohw.Requisition.Width;
		}
		//if (maxWidth > 0)
		//	stage_vbox.SetSizeRequest(maxWidth, -1);
		stage_vbox.CheckResize();
	}
	
	void HandleZoomWidgetValueChanged (object sender, EventArgs e)
	{
		mStage.ZoomAfterPrescaleValue = zoomWidget.Value * mStage.Prescale;	
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
	/*
	public void LoadCEStage(string filename, bool findRawForCEStage)
	{
		try
		{
			mStage.LoadStage(filename);

			string raw_filename; int prescale = 2;
			if (mStage.Prescale != 0) prescale = mStage.Prescale;
			
			if (findRawForCEStage && MainClass.FindRawsForCestageAndAskToOpen(filename, out raw_filename, ref prescale))
			{
				mStage.AskLoadImage(raw_filename, prescale);
			}
		}
		catch (StageDeserializationException sdex)
		{
			Gtk.MessageDialog md = new Gtk.MessageDialog(
				this, DialogFlags.Modal,
				MessageType.Error, ButtonsType.Ok, 
				false, "Can't load stage from the file \"{0}\".\n{1}", filename, sdex.Message);
			
			md.Title = MainClass.APP_NAME;
			
			md.Run();
			md.Destroy();
		}
	}
	*/
	public bool ShowLoadImageDialog()
	{
		if (mStage.TheUIState == UIState.Loading)
		{
			Gtk.MessageDialog md = new Gtk.MessageDialog(this, DialogFlags.Modal,
			                                             MessageType.Error, ButtonsType.Ok, 
			                                             "Loading is in progress");
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

			// Filter for RAWs
			FileFilter ff = new FileFilter();
			
			ff.AddCustom(FileFilterFlags.Filename, delegate (Gtk.FileFilterInfo ffi) {
				return RawLoader.IsRaw(ffi.Filename);
			});
			ff.Name = "RAW image";
	
			fcd.AddFilter(ff);
			
			// Setting current folder to stage file folder or to RAW file folder
			if (mStage.StageFileName != null && mStage.StageFileName != "" 
					&& !fcd.SetCurrentFolder(System.IO.Path.GetDirectoryName(mStage.StageFileName)))
				fcd.SetCurrentFolder(System.IO.Path.GetDirectoryName(mStage.RawFileName));
	
			
			
			// Adding preview widget
			
			ReceiptSelectorWidget rsw = new ReceiptSelectorWidget();
			RawPreviewWidget rpw = new RawPreviewWidget();
			
			// Adding options widget
			
			PreScaleSelectorWidget pssw = new PreScaleSelectorWidget();
			pssw.Value = 2;
			if (mStage.Prescale != 0) pssw.Value = mStage.Prescale;
			
			VBox preview_box = new VBox(false, 4);
			preview_box.PackStart(rpw, true, true, 0);
			preview_box.ShowAll();
			fcd.PreviewWidget = preview_box;
			
			HBox options_box = new HBox(false, 20);
			options_box.PackStart(rsw, true, true, 0);
			options_box.PackEnd(pssw, false, true, 0);
			fcd.ExtraWidget = options_box;
			options_box.ShowAll();
			
			// Adding events
			
			fcd.UpdatePreview += delegate {
				rpw.Filename = fcd.Filename;
			};
			fcd.SelectionChanged += delegate {
				rpw.Filename = fcd.Filename;
				rsw.RawFileName = fcd.Filename;
			};
			rpw.FileIsGoodChanged += delegate {
				options_box.Sensitive = rpw.FileIsGood;
				fcd.PreviewWidgetActive = rpw.FileIsGood;
			};
			

			if (mStage.RawFileName != null)
			{
				fcd.SetFilename(mStage.RawFileName);
				rpw.Filename = mStage.RawFileName;
				rsw.RawFileName = mStage.RawFileName;
				fcd.PreviewWidgetActive = rpw.FileIsGood;
				options_box.Sensitive = rpw.FileIsGood;
			}			
			
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
				if (mStage.RawFileName != filename)
				{
					mStage.AskLoadImage(filename, prescale);
				}
				mStage.LoadStage(rsw.SelectedReceiptFileName);
				return true;
			}
		}
		return false;
	}

	protected virtual void OnCancelButtonClicked (object sender, System.EventArgs e)
	{
		cancel_button.Sensitive = false;
		mStage.CancelLoading();
	}
	
	public void CloseStageEditor()
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
		
		if (fcd.Run() == (int)Gtk.ResponseType.Accept)
		{
			string fn = fcd.Filename;
			if (System.IO.Path.GetExtension(fn).ToLower() != ".cestage")
				fn += ".cestage";
			
			mStage.SaveStage(fn);
		}
		fcd.Destroy();
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

	}

	protected void OnTogglebuttonToggled (object sender, System.EventArgs e)
	{
		if (addNewOperation_togglebutton.Active)
		{
			Menu menu = new Menu();
			int w, h;
			menu.GetSizeRequest(out w, out h);
			int menu_width = left_vbox.Allocation.Width;
			
			menu.SetSizeRequest(menu_width, h);
	
			Dictionary<MenuItem, Type> stage_operation_types = new Dictionary<MenuItem, Type>();
			
			for (int i = 0; i < mStageOperationTypes.Length; i++)
			{
				string name = StageOperationDescriptionAttribute.GetName(mStageOperationTypes[i]);
				if (name == null) name = mStageOperationTypes[i].Name;
				string description = StageOperationDescriptionAttribute.GetDescription(mStageOperationTypes[i]);
			
				MenuItem item = new MenuItem();
				
				VBox item_vbox = new VBox();
				item_vbox.BorderWidth = 4;
				item_vbox.Show();
				
				
				Label lbl_name = new Label();
				lbl_name.Text = name;
				lbl_name.Justify = Justification.Left;
				lbl_name.Xalign = 0;

				// Setting the name font
				double name_size_k = 1.1;
				Pango.FontDescription name_fd = FontHelpers.ScaleFontSize(lbl_name, name_size_k);
				name_fd.Weight = Pango.Weight.Bold;
				lbl_name.ModifyFont(name_fd);
				
				item_vbox.Add(lbl_name);
				lbl_name.Show();

				if (description != null && description != "")
				{
					Label lbl_desc = new Label(description);
					lbl_desc.LineWrapMode = Pango.WrapMode.Word;
					lbl_desc.LineWrap = true;
					lbl_desc.Wrap = true;
					
					// Setting the description font
					double desc_size_k = 0.9;
					Pango.FontDescription desc_fd = FontHelpers.ScaleFontSize(lbl_desc, desc_size_k);
					lbl_desc.ModifyFont(desc_fd);
						
					item_vbox.Add(lbl_desc);
					lbl_desc.Show();
					item_vbox.SizeAllocated += delegate(object o, SizeAllocatedArgs args) {
						lbl_desc.WidthRequest = args.Allocation.Width - 10;
					};
				}
				
				item.Child = item_vbox;
				stage_operation_types.Add(item, mStageOperationTypes[i]);
				
				item.Activated += delegate(object s, EventArgs ea) {
					mStage.CreateAndAddNewItem(stage_operation_types[(MenuItem)s]).Active = true;
					GtkScrolledWindow.HscrollbarPolicy = PolicyType.Never;
					GtkScrolledWindow.Vadjustment.Value = GtkScrolledWindow.Vadjustment.Upper;
					ArrangeVBoxes();
					
				};
				
				menu.Append(item);
				item_vbox.CheckResize();
				//lbl_desc.WidthRequest = ww;
			}
			menu.Deactivated += delegate {
				addNewOperation_togglebutton.Active = false;
			};
			
			menu.ShowAll();
			menu.Popup(null, null, delegate (Menu m, out int x, out int y, out bool push_in) {
				int x1, y1, x0, y0;
				GdkWindow.GetOrigin(out x0, out y0);
				left_vbox.TranslateCoordinates(this, 0, 0, out x1, out y1);
				x = x0 + x1;
				y = y0 + y1;
				push_in = false;
			}, 0, 0);
		}
	}

	protected void OnGtkScrolledWindowExposeEvent (object o, Gtk.ExposeEventArgs args)
	{
	}

	protected void OnStageVboxSizeAllocated (object o, Gtk.SizeAllocatedArgs args)
	{
		if (stage_vbox.Children.Length == 0)
			stage_vbox.QueueDraw();
		else
		{
			int frame = 7;
			// Search for stages outer rect
			int l = stage_vbox.Children[0].Allocation.Left + frame;
			int t = stage_vbox.Children[0].Allocation.Top + frame;
			int r = stage_vbox.Children[0].Allocation.Right - frame;
			int b = stage_vbox.Children[0].Allocation.Bottom - frame;
			
			for (int i = 1; i < stage_vbox.Children.Length; i++)
			{
				if (stage_vbox.Children[i].Allocation.Left + frame < l)
					l = stage_vbox.Children[i].Allocation.Left + frame;
				if (stage_vbox.Children[i].Allocation.Top + frame < t)
					t = stage_vbox.Children[i].Allocation.Top + frame;
				if (stage_vbox.Children[i].Allocation.Right - frame > r)
					r = stage_vbox.Children[i].Allocation.Right - frame;
				if (stage_vbox.Children[i].Allocation.Bottom - frame > b)
					b = stage_vbox.Children[i].Allocation.Bottom - frame;
			}
		
			stage_vbox.QueueDrawArea(0, 0, stage_vbox.Allocation.Width, t);
			stage_vbox.QueueDrawArea(0, 0, l, stage_vbox.Allocation.Height);
			stage_vbox.QueueDrawArea(r, 0, stage_vbox.Allocation.Width, stage_vbox.Allocation.Height);
			stage_vbox.QueueDrawArea(0, b, stage_vbox.Allocation.Width, stage_vbox.Allocation.Height);
		}
	}

	protected void OnExposeEvent (object o, Gtk.ExposeEventArgs args)
	{
		MainClass.windowsGtkStyle.UpdateStyle(this.GdkWindow, main_menubar);

	}

	protected void OnSaveReceiptActionActivated (object sender, System.EventArgs e)
	{
		if (ReceiptsManager.DetermineReceiptType(mStage.StageFileName, mStage.RawFileName) == ReceiptsManager.ReceiptType.Default ||
			ReceiptsManager.DetermineReceiptType(mStage.StageFileName, mStage.RawFileName) == ReceiptsManager.ReceiptType.Custom)
		{
			mStage.SaveStage();
		}
		else
		{
			mStage.SaveStage(ReceiptsManager.MakeDefaultOrCustomReceiptFilename(mStage.RawFileName, null));
		}
		//mStage.StageFileName
		
	}

	protected void OnSaveReceiptAsActionActivated (object sender, System.EventArgs e)
	{
		ReceiptSaveDialog rsd = new ReceiptSaveDialog(this, mStage.RawFileName);

		if (ReceiptsManager.DetermineReceiptType(mStage.StageFileName, mStage.RawFileName) == ReceiptsManager.ReceiptType.Custom) 
		{
			rsd.SelectedType = ReceiptSaveDialog.ReceiptType.Custom;
		}
		else if (ReceiptsManager.DetermineReceiptType(mStage.StageFileName, mStage.RawFileName) == ReceiptsManager.ReceiptType.Class)
		{
			rsd.SelectedType = ReceiptSaveDialog.ReceiptType.Class;
		}
		else
		{
			rsd.SelectedType = ReceiptSaveDialog.ReceiptType.Default;
		}
		
		rsd.SelectedName = ReceiptsManager.ExtractReceiptName(mStage.StageFileName, mStage.RawFileName);
		
		if (rsd.Run() == (int)Gtk.ResponseType.Accept)
		{
			if (rsd.SelectedType == ReceiptSaveDialog.ReceiptType.Default)
				mStage.SaveStage(ReceiptsManager.MakeDefaultOrCustomReceiptFilename(mStage.RawFileName, null));
			else if (rsd.SelectedType == ReceiptSaveDialog.ReceiptType.Custom)
				mStage.SaveStage(ReceiptsManager.MakeDefaultOrCustomReceiptFilename(mStage.RawFileName, rsd.SelectedName));
			else if (rsd.SelectedType == ReceiptSaveDialog.ReceiptType.Class)
				mStage.SaveStage(ReceiptsManager.MakeClassReceiptFilename(System.IO.Path.GetDirectoryName(mStage.RawFileName), rsd.SelectedName));
			else
				throw new Exception("Invalid rsd.SelectedType value");
		}
		rsd.Destroy();
	}

	protected void OnDestroyEvent (object o, Gtk.DestroyEventArgs args)
	{
	}

	protected void OnOpenActionActivated (object sender, System.EventArgs e)
	{
		// Sending remote command to open new image window
		string command = "StageEditor";
		string[] arguments = new string[] {};
		MainClass.RemoteControlService.SendCommand(RemoteControlService.PackCommand(command, arguments));
	}
}