using System;
using System.Xml;
using System.Collections.Generic;
using CatEye.Core;

namespace CatEye.UI.Base
{
	public enum UIState { Processing, Loading, Idle };
	
	public class ExtendedStage : Stage
	{
		private UIState _TheUIState = UIState.Idle;
		private	IBitmapCore mFrozenImage = null;
		private StageOperationParameters _EditingOperation = null;
		private StageOperationParameters _FrozenAt = null;
		private double mZoomAfterPrescaleValue = 0.5;
		private string _RawFileName = null;
		private string _StageFileName = null;
		private int _PreScale = 0;
		
		private StageOperationParametersEditorFactory mSOParametersEditorFactory;
		private StageOperationHolderFactory mSOHolderFactory;

		private volatile bool mUpdatePending = false;
		private volatile bool mProcessImageDuringUpdate = false;
		private volatile bool mLoadPending = false;
		private volatile string mLoadPendingFilename;
		private volatile int mLoadPendingDownscale;
		
		protected Dictionary<StageOperationParameters, IStageOperationHolder> _Holders = 
			new Dictionary<StageOperationParameters, IStageOperationHolder>();

		public event EventHandler<EventArgs> ImageChanged;
		public event EventHandler<EventArgs> ViewNeedsUpdate;
		public event EventHandler<EventArgs> UIStateChanged;
		public event EventHandler<EventArgs> EditingOperationChanged;
		public event EventHandler<EventArgs> OperationFrozen;
		public event EventHandler<EventArgs> OperationDefrozen;
		public event EventHandler<EventArgs> UpdateQueued;
		public event EventHandler<EventArgs> RawFileNameChanged;
		public event EventHandler<EventArgs> StageFileNameChanged;
		public event EventHandler<EventArgs> PreScaleChanged;
		
		public string RawFileName
		{
			get { return _RawFileName; }
			protected set 
			{
				_RawFileName = value;
				if (RawFileNameChanged != null) RawFileNameChanged(this, EventArgs.Empty);
			}
		}

		public string StageFileName
		{
			get { return _StageFileName; }
			protected set 
			{
				_StageFileName = value;
				if (StageFileNameChanged != null) StageFileNameChanged(this, EventArgs.Empty);
			}
		}
		
		public int Prescale
		{
			get { return _PreScale; }
			protected set 
			{
				_PreScale = value;
				if (PreScaleChanged != null) PreScaleChanged(this, EventArgs.Empty);
			}
		}

		public override IBitmapCore SourceImage
		{
			get { return base.SourceImage; }
			protected set 
			{
				base.SourceImage = value;
				mFrozenImage = null;
				AskUpdate();
			}
		}
		
		//TODO: Don't update the image if it is scaled down
		public double ZoomAfterPrescaleValue
		{
			get { return mZoomAfterPrescaleValue; }
			set 
			{
				mZoomAfterPrescaleValue = value;
				mFrozenImage = null;
				AskUpdate();
			}
		}
		
		protected void SetUIState(UIState value)
		{
			if (_TheUIState != value)
			{
				_TheUIState = value;
				if (UIStateChanged != null)
					UIStateChanged(this, EventArgs.Empty);
			}
		}
		
		protected override void OnItemChanged (StageOperationParameters item)
		{
			base.OnItemChanged (item);

			// We should check if we need update in that case
			bool updating_needed = false;
			
			if (_EditingOperation != null)
			{
				if (_EditingOperation != item)
				{
					// Checking if our changed item is BEFORE the editing one
					bool editing_found = false;
					for (int i = 0; i < StageQueue.Count; i++)
					{
						if (_EditingOperation == StageQueue[i]) editing_found = true;
						if (item == StageQueue[i])
						{
							if (!editing_found)
							{
								updating_needed = true;
								break;
							}
						}
					}
				}
				else
				{
					// Re-analyzs image for change in EditingOperation
					if (_TheUIState == UIState.Idle && CurrentImage != null)
					{
						_Holders[_EditingOperation].StageOperationParametersEditor.AnalyzeImage(CurrentImage);
						Console.WriteLine("AnalyzeImage");
						AskUpdate(false);
					}
				}
			}
			else
			{
				updating_needed = true;
				//if (!item.Active)
				//{
				//	_EditingOperation = null;
				//}
			}
			if (updating_needed) AskUpdate();
		}

		//TODO: Check if the item is interchanged with an inactive stage operation
		protected override void OnItemIndexChanged(StageOperationParameters item)
		{
			base.OnItemIndexChanged (item);
			if (item.Active) AskUpdate();
		}
		
		public ReadOnlyDictionary<StageOperationParameters, IStageOperationHolder> Holders
		{
			get { return new ReadOnlyDictionary<StageOperationParameters, IStageOperationHolder>(_Holders); }
		}
		
		public UIState TheUIState 
		{ 
			get { return _TheUIState; } 
		}
		
		//TODO: To add "_Holders[StageQueue[i]].Edit = false;" if the stage operation is inactive
		public StageOperationParameters EditingOperation
		{
			get { return _EditingOperation; }
			set
			{
				if (value != _EditingOperation)
				{
					for (int i = StageQueue.Count - 1; i >= 0; i--)
					{
						_Holders[StageQueue[i]].Edit = (StageQueue[i] == value);
					}
					_EditingOperation = value;
					OnEditingOperationChanged();
				}
			}
		}

		public override void LoadStage(string filename)
		{
			LoadStage(filename, true);
		}
		
		public override void SaveStage(string filename)
		{
			base.SaveStage(filename);
			StageFileName = filename;
		}
		
		public void LoadStage(string filename, bool setStageFilename)
		{
			try
			{
				SetUIState(UIState.Loading);
				FrozenAt = null;
				
				base.LoadStage(filename);
				if (setStageFilename) StageFileName = filename;
			}
			finally
			{
				SetUIState(UIState.Idle);
			}
		}
		
		protected override void DoProcess()
		{
			mUpdatePending = false;
			if (SourceImage != null)
			{
				try
				{
					SetUIState(UIState.Processing);

					CancelProcessingPending = false;
					
					if (mProcessImageDuringUpdate)
					{
						// Checking if the stage is frozen or not and is there a frozen image.
						if (FrozenAt == null || mFrozenImage == null)
						{
							CurrentImage = (IBitmapCore)SourceImage.Clone();
	
							if (mZoomAfterPrescaleValue < 0.999 || mZoomAfterPrescaleValue > 1.001)
							{
								CurrentImage.ScaleFast(mZoomAfterPrescaleValue, delegate (double progress) {
									OnProgressMessageReport(true, progress, "Applying zoom (downscaling)...", false);
									return !CancelProcessingPending;			
								});
							}
							if (ImageChanged != null) ImageChanged(this, EventArgs.Empty);
						}
						else
						{
							CurrentImage = (IBitmapCore)mFrozenImage.Clone();
							if (ImageChanged != null) ImageChanged(this, EventArgs.Empty);
	
						}
						
						// Making the list of stage operations to apply
						List<StageOperation> operationsToApply = new List<StageOperation>();
						List<double> efforts = new List<double>();
						double full_efforts = 0;
	
						int start_index = 0;
						if (FrozenAt != null && mFrozenImage != null)
							start_index = StageQueue.IndexOf(FrozenAt) + 1;
						
						for (int i = start_index; i < StageQueue.Count; i++)
						{
							if (StageQueue[i] != _EditingOperation)
							{
								// Don't add inactives
								if (StageQueue[i].Active == false) continue;
								
								StageOperation newOperation = CallStageOperationFactory(StageQueue[i]);
								operationsToApply.Add(newOperation);
								efforts.Add(newOperation.CalculateEfforts(CurrentImage));
								full_efforts += efforts[efforts.Count - 1];
								
								newOperation.ReportProgress += delegate(object sender, ReportStageOperationProgressEventArgs e) {
									double cur_eff = 0;
									int j = 0;
									while (operationsToApply[j] != (StageOperation)sender)
									{
										cur_eff += efforts[j];
										j++;
									}
									cur_eff += e.Progress * efforts[j];
									string desc = StageOperationDescriptionAttribute.GetName(sender.GetType());
									
									OnProgressMessageReport(true,
										cur_eff / full_efforts, 
										"" + (j + 1) + " of " + efforts.Count +  ": " + desc + "...", true);
									
									if (CancelProcessingPending)
										e.Cancel = true;
								};
							}
							else
								break;
						}
						
						// Executing
						for (int k = 0; k < operationsToApply.Count; k++)
						{
							Console.WriteLine("AnalyzeImage Calling for " + operationsToApply[k].GetType().Name);
							_Holders[operationsToApply[k].Parameters].StageOperationParametersEditor.AnalyzeImage(CurrentImage);
							operationsToApply[k].OnDo(CurrentImage);
							if (operationsToApply[k].Parameters == FrozenAt)
							{
								// After the frozen line is reached,
								// setting the current frozen image
								mFrozenImage = (IBitmapCore)CurrentImage.Clone();
							}
						}
					}
					
					if (_EditingOperation != null)
					{
						Console.WriteLine("AnalyzeImage Calling for " + _EditingOperation.GetType().Name);
						_Holders[_EditingOperation].StageOperationParametersEditor.AnalyzeImage(CurrentImage);
					}
					
					OnImageUpdatingCompleted();
					SetUIState(UIState.Idle);
				}
				catch (UserCancelException)
				{
					// The user cancelled processing.
					// Setting to idle state
					SetUIState(UIState.Idle);
					// Unset cancelling flag.
					AskUpdate();
				}
			}
		}
		
		public void ProcessPending()
		{
			if (mLoadPending)
			{
				DoLoading();
			}
			
			if (mUpdatePending)
			{
				DoProcess();
			}
		}
		
		public void AskUpdate(bool processImage)
		{
			CancelProcessing();
			mProcessImageDuringUpdate = processImage;
			mUpdatePending = true;
			if (UpdateQueued != null) UpdateQueued(this, EventArgs.Empty);
		}
		
		public void AskUpdate()
		{
			AskUpdate(true);
		}
		
		public StageOperationParameters FrozenAt
		{
			get { return _FrozenAt; }
			set
			{
				_FrozenAt = value;
				if (value == null)
				{
					for (int i = 0; i < StageQueue.Count; i++)
					{
						_Holders[StageQueue[i]].Sensitive = true;
						_Holders[StageQueue[i]].Freeze = false;
						_Holders[StageQueue[i]].FrozenButtonsState = false;
					}
					OnOperationDefrozen();
				}
				else
				{
					bool frozenfound = false;
					bool viewedfound = false;
					for (int i = 0; i < StageQueue.Count; i++)
					{
						_Holders[StageQueue[i]].Sensitive = frozenfound;
						if (_EditingOperation == StageQueue[i]) viewedfound = true;
						
						if (StageQueue[i] == value) 
						{
							frozenfound = true;
							if (viewedfound)
							{
								EditingOperation = null;
							}
							_Holders[StageQueue[i]].Freeze = true;
							
							OnOperationFrozen();
						}
						else
						{
							_Holders[StageQueue[i]].Freeze = false;
							_Holders[StageQueue[i]].FrozenButtonsState = true;
						}
					}
					if (!frozenfound)
						throw new Exception("Frozen not found!");
				}
			}
		}
		
		/*
		public void ReportImageChanged(int image_width, int image_height)
		{
			foreach (StageOperationParameters sop in StageQueue)
			{
				_Holders[sop].StageOperationParametersEditor.ReportImageChanged(image_width, image_height);
			}
		}
		*/

		public StageOperationParameters CreateAndAddNewItem(Type sot)
		{
			// Constructing so-sop-sopw structure
			string id = StageOperationIDAttribute.GetTypeID(sot);
			StageOperationParameters sop = CallStageOperationParametersFactory(id);
			Add(sop);
			return sop;
		}
		
		
		public ExtendedStage (StageOperationFactory stageOperationFactory, 
			StageOperationParametersFactory stageOperationParametersFactoryFromID,
			StageOperationParametersEditorFactory SOParametersEditorFactory,
			StageOperationHolderFactory SOHolderFactory,
			BitmapCoreFactory imageLoader) : base(stageOperationFactory, stageOperationParametersFactoryFromID, imageLoader)
		{
			mSOParametersEditorFactory = SOParametersEditorFactory;
			mSOHolderFactory = SOHolderFactory;
		}
		
		protected virtual void OnViewNeedsUpdate()
		{
			if (ViewNeedsUpdate != null)
				ViewNeedsUpdate(this, EventArgs.Empty);
		}
		
		protected virtual void OnEditingOperationChanged()
		{
			if (EditingOperationChanged != null)
				EditingOperationChanged(this, EventArgs.Empty);
			AskUpdate();
		}
		protected virtual void OnOperationFrozen()
		{
			if (OperationFrozen != null)
				OperationFrozen(this, EventArgs.Empty);
			//AskUpdate();
		}

		protected virtual void OnOperationDefrozen()
		{
			mFrozenImage = null;
			if (OperationDefrozen != null)
				OperationDefrozen(this, EventArgs.Empty);
		}

		void HandleOperationReportProgress (object sender, ReportStageOperationProgressEventArgs e)
		{
			e.Cancel = CancelProcessingPending;
		}

		void HandleSohwOperationParametersEditorUserModified (object sender, EventArgs e)
		{
			// If the user modified some property in editing ("pen") mode,
			if (_EditingOperation != null && (sender as IStageOperationHolder) == _Holders[_EditingOperation])
			{
				// refresh the picture
				OnViewNeedsUpdate();
			}
			/*IStageOperationParametersEditor editor = (IStageOperationParametersEditor)sender;
			if (_EditingOperation != null && _Holders[_EditingOperation].StageOperationParametersEditor == editor)
				OnImageUpdatingCompleted();*/
		}
		
		void HandleSohwRemoveButtonClicked (object sender, EventArgs e)
		{
			StageOperationParameters sop = StageOperationByHolder(sender as IStageOperationHolder);
			Remove(sop);
			
		}
	
		void HandleSohwFreezeButtonClicked (object sender, EventArgs e)
		{
			StageOperationParameters sop = StageOperationByHolder(sender as IStageOperationHolder);
			FrozenAt = sop;
		}
	
		void HandleSohwEditButtonClicked (object sender, EventArgs e)
		{
			StageOperationParameters sop = StageOperationByHolder(sender as IStageOperationHolder);
			
			if (_Holders[sop].Edit)
			{
				EditingOperation = sop;
			}
			else
			{
				EditingOperation = null;
			}
		}
		
		void HandleSohwUpTitleButtonClicked (object sender, EventArgs e)
		{
			StageOperationParameters sop = StageOperationByHolder(sender as IStageOperationHolder);
			StepUp(sop);
		}
	
		void HandleSohwDownTitleButtonClicked (object sender, EventArgs e)
		{
			StageOperationParameters sop = StageOperationByHolder(sender as IStageOperationHolder);
			StepDown(sop);
		}
		
		void HandleSohwStageActiveButtonClicked (object sender, EventArgs e)
		{
			AskUpdate();
		}
		
		protected override void OnItemAdded (StageOperationParameters item)
		{
			IStageOperationParametersEditor edtr = mSOParametersEditorFactory(item);
			IStageOperationHolder sohw = mSOHolderFactory(edtr);
			
			// Setting events
			sohw.FreezeButtonClicked += HandleSohwFreezeButtonClicked;
			sohw.RemoveButtonClicked += HandleSohwRemoveButtonClicked;
			sohw.EditButtonClicked += HandleSohwEditButtonClicked;
			sohw.StageActiveButtonClicked += HandleSohwStageActiveButtonClicked;
			sohw.UpTitleButtonClicked += HandleSohwUpTitleButtonClicked;
			sohw.DownTitleButtonClicked += HandleSohwDownTitleButtonClicked;
			
			_Holders.Add(item, sohw);
			_Holders[item].StageOperationParametersEditor.UserModified += HandleSohwOperationParametersEditorUserModified;

			base.OnItemAdded(item);

			AskUpdate();
		}
		
		protected override void OnItemRemoved (StageOperationParameters item)
		{
			base.OnItemRemoved (item);
				
			if (_EditingOperation == item) _EditingOperation = null;
			IStageOperationHolder sohw = _Holders[item];
			
			// Clearing events
			sohw.FreezeButtonClicked -= HandleSohwFreezeButtonClicked;
			sohw.RemoveButtonClicked -= HandleSohwRemoveButtonClicked;
			sohw.EditButtonClicked -= HandleSohwEditButtonClicked;
			sohw.StageActiveButtonClicked -= HandleSohwStageActiveButtonClicked;
			sohw.UpTitleButtonClicked -= HandleSohwUpTitleButtonClicked;
			sohw.DownTitleButtonClicked -= HandleSohwDownTitleButtonClicked;

			_Holders[item].StageOperationParametersEditor.UserModified -= HandleSohwOperationParametersEditorUserModified;
			_Holders.Remove(item);

			AskUpdate();
		}
		
		protected StageOperationParameters StageOperationByHolder(IStageOperationHolder h)
		{
			foreach (StageOperationParameters item in _Holders.Keys)
			{
				if (_Holders[item] == h) return item;
			}
			return null;
		}
		
		/// <summary>
		/// Handles mouse position change. 
		/// Base method should not be called when overridden.
		/// </summary>
		/// <returns>
		/// Should return "true" if something is changed.
		/// </returns>
		public bool ReportEditorMousePosition(int x, int y, int width, int height)
		{
			if (EditingOperation != null &&
				Holders[EditingOperation].StageOperationParametersEditor != null)
			{
				return Holders[EditingOperation].StageOperationParametersEditor.ReportMousePosition(x, y, width, height);
			}
			else
				return false;
		}
		
		/// <summary>
		/// Handles mouse button state change i.e. the user pushed or released the button.
		/// Base method should not be called when overridden.
		/// </summary>
		/// <param name="x">
		/// X coordinate from the left top corner of the image
		/// </param>
		/// <param name="y">
		/// Y coordinate from the left top corner of the image
		/// </param>
		/// <param name="button_id">
		/// The button which state is changed.
		/// </param>
		/// <param name="is_down">
		/// <c>true</c> if the button is down now, <c>false</c> if it's up.
		/// </param>
		/// <returns>
		/// Should return "true" if something is changed and "false" otherwise.
		/// </returns>
		public bool ReportEditorMouseButton(int x, int y, int width, int height, uint button_id, bool is_down)
		{
			if (EditingOperation != null &&
				Holders[EditingOperation].StageOperationParametersEditor != null)
			{
				return Holders[EditingOperation].StageOperationParametersEditor.ReportMouseButton(x, y, width, height, button_id, is_down);
			}
			else
				return false;
		}
		
		public void DrawEditor(IBitmapView target)
		{
			if (EditingOperation != null &&
				Holders[EditingOperation].StageOperationParametersEditor != null)
			{
				Holders[EditingOperation].StageOperationParametersEditor.DrawEditor(target);
			}
		}
		
		private void DoLoading()
		{
			mLoadPending = false;
			SetUIState(UIState.Loading);
			
			base.LoadImage(mLoadPendingFilename, mLoadPendingDownscale);
			if (SourceImage != null)
			{
				RawFileName = mLoadPendingFilename;
				Prescale = mLoadPendingDownscale;
			}
			
			SetUIState(UIState.Idle);
		}
		
		public void AskLoadImage(string filename, int downscale_by)
		{
			CancelProcessing();
			mLoadPendingFilename = filename;
			mLoadPendingDownscale = downscale_by;
			mLoadPending = true;
		}
	}
}

