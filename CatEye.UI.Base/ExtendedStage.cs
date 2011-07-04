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
		private double mZoomValue = 0.5;
		private string _FileName = null;
		private int _PreScale = 0;
		
		private StageOperationParametersEditorFactory mSOParametersEditorFactory;
		private StageOperationHolderFactory mSOHolderFactory;
		private bool _UpdateQueued = false;
		
		protected Dictionary<StageOperationParameters, IStageOperationHolder> _Holders = 
			new Dictionary<StageOperationParameters, IStageOperationHolder>();

		public event EventHandler<EventArgs> ImageChanged;
		public event EventHandler<EventArgs> UIStateChanged;
		public event EventHandler<EventArgs> EditingOperationChanged;
		public event EventHandler<EventArgs> OperationFrozen;
		public event EventHandler<EventArgs> OperationDefrozen;
		public event EventHandler<EventArgs> UpdateQueued;
		public event EventHandler<EventArgs> FileNameChanged;
		public event EventHandler<EventArgs> PreScaleChanged;
		
		public string FileName
		{
			get { return _FileName; }
			protected set 
			{
				_FileName = value;
				if (FileNameChanged != null) FileNameChanged(this, EventArgs.Empty);
			}
		}
		
		public int PreScale
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
				QueueUpdate();
			}
		}
		
		public double ZoomValue
		{
			get { return mZoomValue; }
			set 
			{
				mZoomValue = value;
				mFrozenImage = null;
				QueueUpdate();
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
			QueueUpdate();
		}
		
		protected override void OnItemIndexChanged(StageOperationParameters item)
		{
			base.OnItemIndexChanged (item);
			QueueUpdate();
		}
		
		public ReadOnlyDictionary<StageOperationParameters, IStageOperationHolder> Holders
		{
			get { return new ReadOnlyDictionary<StageOperationParameters, IStageOperationHolder>(_Holders); }
		}
		
		public UIState TheUIState 
		{ 
			get { return _TheUIState; } 
		}
		
		public StageOperationParameters EditingOperation
		{
			get { return _EditingOperation; }
			set
			{
				if (value != _EditingOperation)
				{
					for (int i = _StageQueue.Count - 1; i >= 0; i--)
					{
						_Holders[_StageQueue[i]].Edit = (_StageQueue[i] == value);
					}
					_EditingOperation = value;
					OnEditingOperationChanged();
				}
			}
		}

		public override void LoadStage(string filename)
		{
			try
			{
				SetUIState(UIState.Loading);
				FrozenAt = null;
				
				base.LoadStage(filename);
			}
			finally
			{
				SetUIState(UIState.Idle);
			}
		}
		
		protected override void DoProcess()
		{
			if (SourceImage != null)
			{
				try
				{
					SetUIState(UIState.Processing);

					CancelProcessingPending = false;
					// Removing updating queue flag
					_UpdateQueued = false;
					
					// Checking if the stage is frozen or not and is there a frozen image.
					if (FrozenAt == null || mFrozenImage == null)
					{
						CurrentImage = (IBitmapCore)SourceImage.Clone();

						if (mZoomValue < 0.999 || mZoomValue > 1.001)
						{
							CurrentImage.ScaleFast(mZoomValue, delegate (double progress) {
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
						start_index = _StageQueue.IndexOf(FrozenAt) + 1;
					
					for (int i = start_index; i < _StageQueue.Count; i++)
					{
						if (_StageQueue[i] != _EditingOperation) 
						{
							// Don't add inactives
							if (_StageQueue[i].Active == false) continue;
							
							StageOperation newOperation = _StageOperationFactory(_StageQueue[i]);
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
								string desc = StageOperationDescriptionAttribute.GetSOName(sender.GetType());
								
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
						operationsToApply[k].OnDo(CurrentImage);
						if (operationsToApply[k].Parameters == FrozenAt)
						{
							// After the frozen line is reached,
							// setting the current frozen image
							mFrozenImage = (IBitmapCore)CurrentImage.Clone();
						}
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
					QueueUpdate();
				}
			}
		}
		
		public void ProcessQueued()
		{
			if (_UpdateQueued)
			{
				DoProcess();
			}
		}
		
		public void QueueUpdate()
		{
			CancelProcessing();
			if (UpdateQueued != null) UpdateQueued(this, EventArgs.Empty);
			_UpdateQueued = true;
		}
		
		public StageOperationParameters FrozenAt
		{
			get { return _FrozenAt; }
			set
			{
				_FrozenAt = value;
				if (value == null)
				{
					for (int i = 0; i < _StageQueue.Count; i++)
					{
						_Holders[_StageQueue[i]].Sensitive = true;
						_Holders[_StageQueue[i]].Freeze = false;
						_Holders[_StageQueue[i]].FrozenButtonsState = false;
					}
					OnOperationDefrozen();
				}
				else
				{
					bool frozenfound = false;
					bool viewedfound = false;
					for (int i = 0; i < _StageQueue.Count; i++)
					{
						_Holders[_StageQueue[i]].Sensitive = frozenfound;
						if (_EditingOperation == _StageQueue[i]) viewedfound = true;
						
						if (_StageQueue[i] == value) 
						{
							frozenfound = true;
							if (viewedfound)
							{
								EditingOperation = null;
							}
							_Holders[_StageQueue[i]].Freeze = true;
							
							OnOperationFrozen();
						}
						else
						{
							_Holders[_StageQueue[i]].Freeze = false;
							_Holders[_StageQueue[i]].FrozenButtonsState = true;
						}
					}
					if (!frozenfound)
						throw new Exception("Frozen not found!");
				}
			}
		}
		
		public void ReportImageChanged(int image_width, int image_height)
		{
			foreach (StageOperationParameters sop in _StageQueue)
			{
				_Holders[sop].StageOperationParametersEditor.ReportImageChanged(image_width, image_height);
			}
		}

		public StageOperationParameters CreateAndAddNewItem(Type sot)
		{
			// Constructing so-sop-sopw structure
			string id = StageOperationIDAttribute.GetTypeID(sot);
			StageOperationParameters sop = _StageOperationParametersFactoryFromID(id);
			Add(sop);
			return sop;
		}
		
		
		public ExtendedStage (StageOperationFactory stageOperationFactory, 
			StageOperationParametersFactoryFromID stageOperationParametersFactoryFromID,
			StageOperationParametersEditorFactory SOParametersEditorFactory,
			StageOperationHolderFactory SOHolderFactory,
			ImageLoader imageLoader) : base(stageOperationFactory, stageOperationParametersFactoryFromID, imageLoader)
		{
			mSOParametersEditorFactory = SOParametersEditorFactory;
			mSOHolderFactory = SOHolderFactory;
		}

		protected virtual void OnEditingOperationChanged()
		{
			if (EditingOperationChanged != null)
				EditingOperationChanged(this, EventArgs.Empty);
			QueueUpdate();
		}
		protected virtual void OnOperationFrozen()
		{
			if (OperationFrozen != null)
				OperationFrozen(this, EventArgs.Empty);
			QueueUpdate();
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
			QueueUpdate();
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
			QueueUpdate();
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

			QueueUpdate();
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

			QueueUpdate();
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

		public override bool LoadImage(string filename, int downscale_by)
		{
			SetUIState(UIState.Loading);
			
			bool res = base.LoadImage(filename, downscale_by);
			if (res)
			{
				FileName = filename;
				PreScale = downscale_by;
			}
			
			SetUIState(UIState.Idle);
			
			return res;
		}
	}
}

