using System;
using System.Xml;
using System.Collections.Generic;
using CatEye.Core;

namespace CatEye
{
	public enum UIState { Processing, Loading, Free };
	public delegate bool ProgressMessageReporter(double progress, string status);
	
	public class ExtendedStage : Stage
	{
		private UIState _TheUIState = UIState.Free;
		private bool update_timer_launched = false;
		private bool mCancelPending = false;
		private uint update_timer_delay = 500;
		private IBitmapCore mSourceImage = null;
		private IBitmapCore mCurrentImage = null;
		private IBitmapCore frozen = null;
		private StageOperation _EditingOperation = null;
		private StageOperation _FrozenAt = null;
		private double mZoomValue = 0.5;
		private StageOperationParametersEditorFactory mSOParametersEditorFactory;
		private StageOperationHolderFactory mSOHolderFactory;
		private bool _UpdatePending = false;
		
		protected Dictionary<StageOperation, IStageOperationHolder> _Holders = 
			new Dictionary<StageOperation, IStageOperationHolder>();

		public event ProgressMessageReporter ProgressMessagesReporter;
		public event EventHandler<EventArgs> UIStateChanged;
		public event EventHandler<EventArgs> EditingOperationChanged;
		public event EventHandler<EventArgs> OperationFrozen;
		public event EventHandler<EventArgs> OperationDefrozen;
		public event EventHandler<EventArgs> ImageChanged;
		public event EventHandler<EventArgs> ImageUpdated;
		
		public static readonly Type[] _StageOperationTypes = new Type[]
		{
			typeof(CompressionStageOperation),
			typeof(BrightnessStageOperation),
			typeof(UltraSharpStageOperation),
			typeof(SaturationStageOperation),
			typeof(ToneStageOperation),
			typeof(BlackPointStageOperation),
			typeof(LimitSizeStageOperation),
			typeof(CrotateStageOperation),
		};
		
		public IBitmapCore SourceImage
		{
			get { return mSourceImage; }
			set 
			{ 
				mCurrentImage = null;
				frozen = null;
				mSourceImage = value;
				
				if (ImageChanged != null) ImageChanged(this, EventArgs.Empty);
				LaunchUpdateTimer();
			}
		}
		
		public double ZoomValue
		{
			get { return mZoomValue; }
			set 
			{
				mZoomValue = value;
				LaunchUpdateTimer();
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
		
		protected override void OnOperationActivityChanged ()
		{
			base.OnOperationActivityChanged ();
			LaunchUpdateTimer();
		}
		
		protected override void OnOperationIndexChanged ()
		{
			base.OnOperationIndexChanged ();
			LaunchUpdateTimer();
		}
		
		public ReadOnlyDictionary<StageOperation, IStageOperationHolder> Holders
		{
			get { return new ReadOnlyDictionary<StageOperation, IStageOperationHolder>(_Holders); }
		}

		public int IndexOf(StageOperation so)
		{
			return _StageQueue.IndexOf(so);
		}
		
		public void SetCancelPending()
		{
			mCancelPending = true;
		}
		
		public UIState TheUIState 
		{ 
			get { return _TheUIState; } 
		}
		
		public IBitmapCore CurrentImage
		{
			get { return mCurrentImage; }
		}
		
		public bool CancelPending
		{
			get { return mCancelPending; }
		}
		
		public StageOperation EditingOperation
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
		
		public void CancelAll()
		{
			if (TheUIState != UIState.Free)
				SetCancelPending();
		}
		
		/*
		public void LaunchStageLoading(string filename)
		{
			GLib.Timeout.Add(update_timer_delay, delegate {
				if (TheUIState == UIState.Processing)
				{
					mCancelPending = true;
					return true;
				}
				else
				{
					LoadStage(filename);
					return false;
				}
			});
		}
		*/
		
		public void LoadStage(string filename)
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.Load(filename);

			FrozenAt = null;
			DeserializeFromXML(xdoc.ChildNodes[1], _StageOperationTypes);
			
			// Assigning our handlers
			for (int i = 0; i < StageQueue.Length; i++)
			{
				Holders[StageQueue[i]].StageOperationParametersEditor.UserModified += delegate {
					LaunchUpdateTimer();
				};
			}
		}
		
		
		
		public void LaunchUpdateTimer()
		{
			if (!update_timer_launched)
			{
				update_timer_launched = true;
				_UpdatePending = false;
/*				GLib.Timeout.Add(update_timer_delay, new GLib.TimeoutHandler(delegate {
					update_timer_launched = false;*/
					switch (TheUIState)
					{
					case UIState.Processing:
						// Already processing. Image processing needs to be interrupted, after 
						// that we have to hit the update timer again
						mCancelPending = true;
						_UpdatePending = true;
						return;
					
					case UIState.Loading:
						// The image is loading. No refresh allowed. Just ignoring the command
						return;
						
					case UIState.Free:
						// Updating and stopping the timer
						if (FrozenAt == null)
						{
							frozen = null;
							UpdateStageAfterFrozen();
						}
						else
						{
							if (frozen == null)
							{
								if (UpdateFrozen())
								{
									UpdateStageAfterFrozen();
								}
								else
								{
									Console.WriteLine("Frozen image isn't updated.");
								}
							}
							else
							{
								UpdateStageAfterFrozen();
							}
							
						}
						
						return;
					default:
						throw new Exception("Unhandled case occured: " + TheUIState);
					}
//				}));
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// True if complete, false if user hit Cancel
		/// </returns>
		bool UpdateFrozen()
		{
			bool res = false;
			try
			{
				if (TheUIState != UIState.Processing)
				{
					UIState curstate = TheUIState;
					SetUIState(UIState.Processing);
					
					IBitmapCore frozen_tmp = (IBitmapCore)mSourceImage.Clone();
						
					if (frozen_tmp != null)
					{
						if (mZoomValue < 0.999 || mZoomValue > 1.001)
						{
							frozen_tmp.ScaleFast(mZoomValue, delegate (double progress) {
								if (ProgressMessagesReporter != null) 
											return ProgressMessagesReporter(progress, "Zooming...");
										else 
											return true; // If callback is not assigned, just continue
							});
						}
	
						ApplyOperationsBeforeFrozenLine(frozen_tmp);
						frozen = frozen_tmp;
						res = true;
					}
					
					SetUIState(curstate);
				}
			}
			catch (UserCancelException)
			{
				mCancelPending = false;
			}
			
			return res;
		}
		
		/// <summary>
		/// Assumes that frozen image is prepared already
		/// </summary>
		void UpdateStageAfterFrozen()
		{
			if (mSourceImage == null) return;
			if (TheUIState != UIState.Processing)
			{
				UIState curstate = TheUIState;
				SetUIState(UIState.Processing);
	
				try
				{
					// 1. Creating the new CurrentImage out of source or frozen image
					
					if (frozen == null)
					{
						mCurrentImage = (IBitmapCore)mSourceImage.Clone();
						
						if (mZoomValue < 0.999 || mZoomValue > 1.001)
						{
							mCurrentImage.ScaleFast(mZoomValue, delegate (double progress) {
								if (ProgressMessagesReporter != null) 
									return ProgressMessagesReporter(progress, "Downscaling...");
								else 
									return true; // If callback is not assigned, just continue
							});
						}

						if (ImageChanged != null) ImageChanged(this, EventArgs.Empty);
						if (ImageUpdated != null) ImageUpdated(this, EventArgs.Empty);
					}
					else
						mCurrentImage = (IBitmapCore)frozen.Clone();
					
					// 2. Processing the stage operations to newly created image

					if (mCurrentImage != null)
					{
						ApplyOperationsAfterFrozenLine(mCurrentImage);
						if (ImageUpdated != null) ImageUpdated(this, EventArgs.Empty);
					}
				}
				catch (UserCancelException)
				{
					mCancelPending = false;
				}
				SetUIState(curstate);
			}
		}

		public StageOperation FrozenAt
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
								// If viewed wss before the frozen line, 
								// changing viewed to the first after frozen  
								if (_StageQueue.Count > i + 1)
									EditingOperation = _StageQueue[i + 1];
								else
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
			foreach (StageOperation sop in _StageQueue)
			{
				_Holders[sop].StageOperationParametersEditor.ReportImageChanged(image_width, image_height);
			}
		}
		
		public void ApplyOperationsBeforeFrozenLine(IBitmapCore hdp)
		{
			if (_FrozenAt == null) return;	// If not frozen, do nothing

			// Do operations
			for (int j = 0; j < _StageQueue.Count; j++)
			{
				if (_StageQueue[j].Parameters.Active)
					_StageQueue[j].OnDo(hdp);
				
				if (_StageQueue[j] == _FrozenAt)
				{
					// If frozen line is here, succeed.
					return;
				}
			}
		}
		
		public void ApplyOperationsAfterFrozenLine(IBitmapCore hdp)
		{
			if (_FrozenAt == null)
			{
				// Do all operations
				for (int j = 0; j < _StageQueue.Count; j++)
				{
					if (_StageQueue[j] == EditingOperation)
						break;
					if (_StageQueue[j].Parameters.Active)
						_StageQueue[j].OnDo(hdp);
				}
			}
			else
			{
				bool frozen_line_found = false;
				for (int j = 0; j < _StageQueue.Count; j++)
				{
					if (_StageQueue[j] == EditingOperation)
						break;
					if (frozen_line_found && _StageQueue[j].Parameters.Active)
						_StageQueue[j].OnDo(hdp);
					if (_StageQueue[j] == _FrozenAt) frozen_line_found = true;
				}
			}
		}
		
		
		public StageOperation CreateAndAddNewStageOperation(Type sot)
		{
			// Constructing so-sop-sopw structure
			string id = StageOperationIDAttribute.GetTypeID(sot);
			Type sopt = StageOperationIDAttribute.FindTypeByID(mStageOperationParametersTypes, id);
			
			StageOperationParameters sop = (StageOperationParameters)sopt.GetConstructor(
					new Type[] { }).Invoke(new object[] { });
			
			StageOperation so = (StageOperation)sot.GetConstructor(
					new Type[] { typeof(StageOperationParameters) }
				).Invoke(new object[] { sop });
			
			AddStageOperation(so);
			return so;
		}
		
		
		public ExtendedStage (StageOperationParametersEditorFactory SOParametersEditorFactory,
							  StageOperationHolderFactory SOHolderFactory)
		{
			mSOParametersEditorFactory = SOParametersEditorFactory;
			mSOHolderFactory = SOHolderFactory;
		}

		protected virtual void OnEditingOperationChanged()
		{
			if (EditingOperationChanged != null)
				EditingOperationChanged(this, EventArgs.Empty);
			LaunchUpdateTimer();
		}
		protected virtual void OnOperationFrozen()
		{
			if (OperationFrozen != null)
				OperationFrozen(this, EventArgs.Empty);
			LaunchUpdateTimer();
		}

		protected virtual void OnOperationDefrozen()
		{
			if (OperationDefrozen != null)
				OperationDefrozen(this, EventArgs.Empty);
		}
		
		protected override void OnAddedToStage (StageOperation operation)
		{
			/*
			Type paramType = FindTypeForStageOperation(mStageOperationParametersTypes, operation.GetType());
			Type paramWidgetType = FindTypeForStageOperation(mStageOperationParametersWidgetTypes, operation.GetType());
			Console.WriteLine("Creating widget for " + paramWidgetType.Name);
			StageOperationParametersWidget pwid = (StageOperationParametersWidget)(
				paramWidgetType.GetConstructor(new Type[] { paramType }).Invoke(new object[] { operation.Parameters })
			);
			
			// Creating stage operation holder
			StageOperationHolderWidget sohw = new StageOperationHolderWidget(pwid);
			// Getting stage operation name from attribute
			object[] attrs = operation.GetType().GetCustomAttributes(typeof(StageOperationDescriptionAttribute), true);
			if (attrs != null && attrs.Length > 0)
				sohw.Title = (attrs[0] as StageOperationDescriptionAttribute).Name;
			
			sohw.StageOperationParametersEditor.UserModified += HandleSohwOperationParametersWidgetUserModified;
			
			// Setting events
			sohw.UpTitleButtonClicked += HandleSohwUpTitleButtonClicked;
			sohw.DownTitleButtonClicked += HandleSohwDownTitleButtonClicked;
			sohw.EditButtonClicked += HandleSohwEditButtonClicked;
			sohw.FreezeButtonClicked += HandleSohwFreezeButtonClicked;
			sohw.RemoveButtonClicked += HandleSohwRemoveButtonClicked;

			_Holders.Add(operation, sohw);
			sohw.Show();
			
			//sohw.Active = false;	// Setting to update UI sensitiveness
			
			_StageVBox.Add(sohw);
			((Gtk.Box.BoxChild)_StageVBox[sohw]).Fill = false;
			((Gtk.Box.BoxChild)_StageVBox[sohw]).Expand = false;
			ArrangeVBoxes();
			*/
			
			IStageOperationParametersEditor edtr = mSOParametersEditorFactory(operation);
			IStageOperationHolder sohw = mSOHolderFactory(edtr);
			_Holders.Add(operation, sohw);
			_Holders[operation].StageOperationParametersEditor.UserModified += HandleSohwOperationParametersWidgetUserModified;

			base.OnAddedToStage (operation);

			LaunchUpdateTimer();
		}

		void HandleSohwOperationParametersWidgetUserModified (object sender, EventArgs e)
		{
			LaunchUpdateTimer();
		}
		
		protected override void OnRemovedFromStage (StageOperation operation)
		{
			if (_EditingOperation == operation) _EditingOperation = null;

			base.OnRemovedFromStage (operation);
			
			_Holders[operation].StageOperationParametersEditor.UserModified -= HandleSohwOperationParametersWidgetUserModified;
			_Holders.Remove(operation);
			LaunchUpdateTimer();
		}
		
		public void StepDown(StageOperation sop)
		{
			int index = _StageQueue.IndexOf(sop);
			if (index < _StageQueue.Count - 1)
			{
				_StageQueue.Remove(sop);
				_StageQueue.Insert(index + 1, sop);
				OnOperationIndexChanged();
			}
		}

		public void StepUp(StageOperation sop)
		{
			int index = _StageQueue.IndexOf(sop);
			if (index > 0)
			{
				_StageQueue.Remove(sop);
				_StageQueue.Insert(index - 1, sop);
				OnOperationIndexChanged();
			}
		}
		
		public StageOperation StageOperationByHolder(IStageOperationHolder hw)
		{
			foreach (StageOperation sop in _Holders.Keys)
			{
				if (_Holders[sop] == hw) return sop;
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
	}
}

