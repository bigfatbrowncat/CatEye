using System;
using System.Collections.Generic;
using CatEye.Core;

namespace CatEye
{
	public class ExtendedStage : Stage
	{
		protected Type[] mStageOperationParametersWidgetTypes = new Type[]
		{
			typeof(CropStageOperationParametersWidget),
			typeof(CompressionStageOperationParametersWidget),
			typeof(BrightnessStageOperationParametersWidget),
			typeof(UltraSharpStageOperationParametersWidget),
			typeof(SaturationStageOperationParametersWidget),
			typeof(ToneStageOperationParametersWidget),
		};
		
		protected Type FindTypeForStageOperation(Type[] types, Type stageOperationType)
		{
			object[] attrs = stageOperationType.GetCustomAttributes(typeof(StageOperationIDAttribute), true);
			if (attrs.Length == 0)
			{
				return null;
			}
			string id = ((StageOperationIDAttribute)attrs[0]).ID;
			return FindTypeWithStageOperationIDEqualTo(types, id);
		}

		protected Type GetStageOperationParametersWidget(Type stageOperationType)
		{
			object[] attrs = stageOperationType.GetCustomAttributes(typeof(StageOperationIDAttribute), true);
			if (attrs.Length == 0)
			{
				return null;
			}
			string id = ((StageOperationIDAttribute)attrs[0]).ID;
			return FindTypeWithStageOperationIDEqualTo(mStageOperationParametersTypes, id);
		}
		
		private StageOperation _ViewedOperation = null;
		private StageOperation _FrozenAt = null;
		private FrozenPanel _FrozenPanel;
		private Gtk.VBox _StageVBox;

		protected Dictionary<StageOperation, StageOperationHolderWidget> _Holders = 
			new Dictionary<StageOperation, StageOperationHolderWidget>();

		public event EventHandler<EventArgs> ViewedOperationChanged;
		public event EventHandler<EventArgs> OperationFrozen;

		public ReadOnlyDictionary<StageOperation, StageOperationHolderWidget> Holders
		{
			get { return new ReadOnlyDictionary<StageOperation, StageOperationHolderWidget>(_Holders); }
		}

		public StageOperation ViewedOperation
		{
			get { return _ViewedOperation; }
			set
			{
				if (value != _ViewedOperation)
				{
					for (int i = _StageQueue.Count - 1; i >= 0; i--)
					{
						_Holders[_StageQueue[i]].View = (_StageQueue[i] == value);
					}
					_ViewedOperation = value;
					OnViewedOperationChanged();
				}
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
					_FrozenPanel.Hide();
				}
				else
				{
					bool frozenfound = false;
					bool viewedfound = false;
					for (int i = 0; i < _StageQueue.Count; i++)
					{
						_Holders[_StageQueue[i]].Sensitive = frozenfound;
						if (_ViewedOperation == _StageQueue[i]) viewedfound = true;
						
						if (_StageQueue[i] == value) 
						{
							frozenfound = true;
							if (viewedfound)
							{
								// If viewed wss before the frozen line, 
								// changing viewed to the first after frozen  
								if (_StageQueue.Count > i + 1)
									ViewedOperation = _StageQueue[i + 1];
								else
									ViewedOperation = null;
							}
							_Holders[_StageQueue[i]].Freeze = true;
							
							if (_FrozenPanel.Parent != _StageVBox)
								_StageVBox.Add(_FrozenPanel);

							((Gtk.Box.BoxChild)_StageVBox[_FrozenPanel]).Position = i + 1;
							((Gtk.Box.BoxChild)_StageVBox[_FrozenPanel]).Fill = true;
							((Gtk.Box.BoxChild)_StageVBox[_FrozenPanel]).Expand = false;
							
							/*_FrozenPanel.View = _Holders[_StageQueue[i]].View;*/
							_FrozenPanel.Show();
						}
						else
						{
							_Holders[_StageQueue[i]].Freeze = false;
							_Holders[_StageQueue[i]].FrozenButtonsState = true;
						}
					}
				}
				OnOperationFrozen();
			}
		}
		
		public void ReportImageChanged(int image_width, int image_height)
		{
			foreach (StageOperation sop in _StageQueue)
			{
				_Holders[sop].OperationParametersWidget.ReportImageChanged(image_width, image_height);
			}
		}
		
		public bool ApplyOperationsBeforeFrozenLine(DoublePixmap hdp)
		{
			if (_FrozenAt == null) return true;	// If not frozen, do nothing
			try
			{
				// Do all operations
				for (int j = 0; j < _StageQueue.Count; j++)
				{
					if (_Holders[_StageQueue[j]].Active)
						_StageQueue[j].OnDo(hdp);
					
					if (_StageQueue[j] == _FrozenAt)
					{
						// If frozen line is here, succeed.
						return true;
					}
				}
			}
			catch (UserCancelException)
			{
				return false;
			}
			return true;
		}
		
		public bool ApplyOperationsAfterFrozenLine(DoublePixmap hdp)
		{
			try
			{
				if (_FrozenAt == null)
				{
					// Do all operations
					for (int j = 0; j < _StageQueue.Count; j++)
					{
						if (_StageQueue[j] == ViewedOperation)
							break;
						if (_Holders[_StageQueue[j]].Active)
							_StageQueue[j].OnDo(hdp);
					}
				}
				else
				{
					bool frozen_line_found = false;
					for (int j = 0; j < _StageQueue.Count; j++)
					{
						if (_StageQueue[j] == ViewedOperation)
							break;
						if (frozen_line_found && _Holders[_StageQueue[j]].Active)
							_StageQueue[j].OnDo(hdp);
						if (_StageQueue[j] == _FrozenAt) frozen_line_found = true;
					}
				}
			}
			catch (UserCancelException)
			{
				return false;
			}
			return true;
		}
		
		protected void ArrangeVBoxes()
		{
			// Arranging stage 2
			for (int i = 0; i < _StageQueue.Count; i++)
			{
				StageOperationHolderWidget sohw = _Holders[_StageQueue[i]];
				((Gtk.Box.BoxChild)_StageVBox[sohw]).Position = i;
			}
		}

		public StageOperation CreateAndAddNewStageOperation(Type sot)
		{
			// Constructing so-sop-sopw structure
			Type sopt = FindTypeForStageOperation(mStageOperationParametersTypes, sot);
			
			StageOperationParameters sop = (StageOperationParameters)sopt.GetConstructor(
					new Type[] { }).Invoke(new object[] { });
			
			StageOperation so = (StageOperation)sot.GetConstructor(
					new Type[] { typeof(StageOperationParameters) }
				).Invoke(new object[] { sop });
			
			AddStageOperation(so);
			return so;
		}
		
		public ExtendedStage (Gtk.VBox stage_box)
		{
			_StageVBox = stage_box;
			_FrozenPanel = new FrozenPanel();
			_FrozenPanel.UnfreezeButtonClicked  += delegate {
				FrozenAt = null;
			};
		}

		protected virtual void OnViewedOperationChanged()
		{
			if (ViewedOperationChanged != null)
				ViewedOperationChanged(this, EventArgs.Empty);
		}
		protected virtual void OnOperationFrozen()
		{
			if (OperationFrozen != null)
				OperationFrozen(this, EventArgs.Empty);
		}
		
		protected override void OnAddedToStage (StageOperation operation)
		{
			Console.WriteLine("OnAddedToStage");
/*			
			StageOperationParameters pars = (StageOperationParameters)(
				paramType.GetConstructor(new Type[] { }).Invoke(new object[] { })
			);
*/
			Type paramType = FindTypeForStageOperation(mStageOperationParametersTypes, operation.GetType());
			Type paramWidgetType = FindTypeForStageOperation(mStageOperationParametersWidgetTypes, operation.GetType());
			StageOperationParametersWidget pwid = (StageOperationParametersWidget)(
				paramWidgetType.GetConstructor(new Type[] { paramType }).Invoke(new object[] { operation.Parameters })
			);
			
			// Creating stage operation holder
			StageOperationHolderWidget sohw = new StageOperationHolderWidget(pwid);
			// Getting stage operation name from attribute
			object[] attrs = operation.GetType().GetCustomAttributes(typeof(StageOperationDescriptionAttribute), true);
			if (attrs != null && attrs.Length > 0)
				sohw.Title = (attrs[0] as StageOperationDescriptionAttribute).Name;
			
			// Setting events
			sohw.UpTitleButtonClicked += HandleSohwUpTitleButtonClicked;
			sohw.DownTitleButtonClicked += HandleSohwDownTitleButtonClicked;
			sohw.StageActiveButtonClicked += HandleSohwStageActiveButtonClicked;
			sohw.ViewButtonClicked += HandleSohwViewButtonClicked;
			sohw.FreezeButtonClicked += HandleSohwFreezeButtonClicked;
			sohw.RemoveButtonClicked += HandleSohwRemoveButtonClicked;

			_Holders.Add(operation, sohw);
			sohw.Show();
			
			sohw.Active = false;	// Setting to update UI sensitiveness
			
			_StageVBox.Add(sohw);
			((Gtk.Box.BoxChild)_StageVBox[sohw]).Fill = false;
			((Gtk.Box.BoxChild)_StageVBox[sohw]).Expand = false;
			ArrangeVBoxes();
			
			base.OnAddedToStage (operation);
		}
		
		protected override void OnRemovedFromStage (StageOperation operation)
		{
			
			StageOperationHolderWidget sohw = _Holders[operation];

			_StageVBox.Remove(sohw);
			sohw.UpTitleButtonClicked -= HandleSohwUpTitleButtonClicked;
			sohw.DownTitleButtonClicked -= HandleSohwDownTitleButtonClicked;
			sohw.StageActiveButtonClicked -= HandleSohwStageActiveButtonClicked;
			sohw.ViewButtonClicked -= HandleSohwViewButtonClicked;
			sohw.FreezeButtonClicked -= HandleSohwFreezeButtonClicked;
			sohw.RemoveButtonClicked -= HandleSohwRemoveButtonClicked;
			sohw.Dispose();
			_Holders.Remove(operation);
			
			ArrangeVBoxes();

			base.OnRemovedFromStage (operation);
		}
		
		void HandleSohwRemoveButtonClicked (object sender, EventArgs e)
		{
			StageOperation sop = StageOperationByHolder(sender as StageOperationHolderWidget);
			RemoveStageOperation(sop);
		}

		void HandleSohwFreezeButtonClicked (object sender, EventArgs e)
		{
			StageOperation sop = StageOperationByHolder(sender as StageOperationHolderWidget);
			FrozenAt = sop;
		}

		void HandleSohwViewButtonClicked (object sender, EventArgs e)
		{
			StageOperation sop = StageOperationByHolder(sender as StageOperationHolderWidget);
			
			if (_Holders[sop].View)
			{
				ViewedOperation = sop;
			}
			else
			{
				ViewedOperation = null;
			}
		}
		
		void HandleSohwStageActiveButtonClicked (object sender, EventArgs e)
		{
			OnOperationActivityChanged();
		}

		void HandleSohwDownTitleButtonClicked (object sender, EventArgs e)
		{
			StageOperation sop = StageOperationByHolder(sender as StageOperationHolderWidget);

			int index = _StageQueue.IndexOf(sop);
			if (index < _StageQueue.Count - 1)
			{
				_StageQueue.Remove(sop);
				_StageQueue.Insert(index + 1, sop);
				ArrangeVBoxes();
				OnOperationIndexChanged();
			}
		}

		void HandleSohwUpTitleButtonClicked (object sender, EventArgs e)
		{
			StageOperation sop = StageOperationByHolder(sender as StageOperationHolderWidget);

			int index = _StageQueue.IndexOf(sop);
			if (index > 0)
			{
				_StageQueue.Remove(sop);
				_StageQueue.Insert(index - 1, sop);
				ArrangeVBoxes();
				OnOperationIndexChanged();
			}
		}
		
		private StageOperation StageOperationByHolder(StageOperationHolderWidget hw)
		{
			foreach (StageOperation sop in _Holders.Keys)
			{
				if (_Holders[sop] == hw) return sop;
			}
			return null;
		}
		
		protected override void OnStageOperationDeserialized(StageOperation so, StageOperationParameters sop)
		{
			Console.WriteLine("OnStageOperationDeserialized");
			
			Type sopwt = FindTypeForStageOperation(mStageOperationParametersWidgetTypes, so.GetType());
			if (sopwt == null)
				throw new IncorrectNodeValueException("Can't find StageOperationParametersWidgetsType type for type " + so.GetType().Name);
			StageOperationParametersWidget sopw = (StageOperationParametersWidget)sopwt.GetConstructor(
					new Type[] { typeof(StageOperationParameters) }
				).Invoke(new object[] { sop });
		}		
	}
}

