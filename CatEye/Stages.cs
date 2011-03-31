
using System;
using System.Collections.Generic;

namespace CatEye
{
	public class OperationRemovedFromStageEventArgs : EventArgs
	{
		StageOperation _Operation;
		public StageOperation Operation { get { return _Operation; } }
		public OperationRemovedFromStageEventArgs(StageOperation operation)
		{
			_Operation = operation;
		}
	}
	
	public class OperationAddedToStageEventArgs : EventArgs
	{
		StageOperation _Operation;
		public StageOperation Operation { get { return _Operation; } }
		public OperationAddedToStageEventArgs(StageOperation operation)
		{
			_Operation = operation;
		}
	}
	
	public class Stages
	{
		private FrozenPanel _FrozenPanel;
		private List<StageOperation> _StageQueue;
		private Gtk.VBox _StageVBox;
		private Dictionary<StageOperation, StageOperationHolderWidget> _Holders = 
			new Dictionary<StageOperation, StageOperationHolderWidget>();
		
		private StageOperation _ViewedOperation = null;
		private StageOperation _FrozenAt = null;
		
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
						/*if (_FrozenAt == _StageQueue[i]) 
							_FrozenPanel.View = (_StageQueue[i] == value);*/
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
								// changing viewed to frozen
								ViewedOperation = _StageQueue[i];
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
		
		public StageOperation[] StageQueue { get { return _StageQueue.ToArray(); } }
		
		public event EventHandler<OperationRemovedFromStageEventArgs> OperationRemovedFromStage;
		public event EventHandler<OperationAddedToStageEventArgs> OperationAddedToStage;
		public event EventHandler<EventArgs> OperationIndexChanged;
		public event EventHandler<EventArgs> OperationActivityChanged;
		public event EventHandler<EventArgs> ViewedOperationChanged;
		public event EventHandler<EventArgs> OperationFrozen;

		protected virtual void OnAddedToStage(StageOperation operation)
		{
			if (OperationAddedToStage != null) 
				OperationAddedToStage(this, new OperationAddedToStageEventArgs(operation));
		}
		protected virtual void OnRemovedFromStage(StageOperation operation)
		{
			if (OperationRemovedFromStage != null) 
				OperationRemovedFromStage(this, new OperationRemovedFromStageEventArgs(operation));
		}
		protected virtual void OnOperationIndexChanged()
		{
			if (OperationIndexChanged != null)
				OperationIndexChanged(this, EventArgs.Empty);
		}
		protected virtual void OnOperationActivityChanged()
		{
			if (OperationActivityChanged != null)
				OperationActivityChanged(this, EventArgs.Empty);
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
		
		public void ApplyAllOperations(DoublePixmap hdp)
		{
			for (int j = 0; j < _StageQueue.Count; j++)
			{
				if (_StageQueue[j] == ViewedOperation)
					break;
				if (_Holders[_StageQueue[j]].Active)
					_StageQueue[j].OnDo(hdp);
			}
		}

		public bool ApplyOperationsAfterFrozenLine(DoublePixmap hdp)
		{
			try
			{
				if (_FrozenAt == null)
				{
					// Do all operations
					ApplyAllOperations(hdp);
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
		
		public void AddStageOperation(StageOperation operation)
		{
			_StageQueue.Add(operation);
			
			// Creating stage operation holder
			StageOperationHolderWidget sohw = new StageOperationHolderWidget(operation);

			// Getting stage operation name from attribute
			object[] attrs = operation.GetType().GetCustomAttributes(typeof(StageOperationDescriptionAttribute), true);
			if (attrs != null && attrs.Length > 0)
				sohw.Title = (attrs[0] as StageOperationDescriptionAttribute).Name;
			
			sohw.UpTitleButtonClicked += HandleSohwUpTitleButtonClicked;
			sohw.DownTitleButtonClicked += HandleSohwDownTitleButtonClicked;
			sohw.StageActiveButtonClicked += HandleSohwStageActiveButtonClicked;
			sohw.ViewButtonClicked += HandleSohwViewButtonClicked;
			sohw.FreezeButtonClicked += HandleSohwFreezeButtonClicked;

			_StageVBox.Add(sohw);
			((Gtk.Box.BoxChild)_StageVBox[sohw]).Fill = false;
			((Gtk.Box.BoxChild)_StageVBox[sohw]).Expand = false;
			
			_Holders.Add(operation, sohw);
			sohw.Show();
			
			sohw.Active = false;	// Setting to update UI sensitiveness
			
			ArrangeVBoxes();
			OnAddedToStage(operation);
		}

		/*
		void Handle_FrozenPanelViewButtonClicked (object sender, EventArgs e)
		{
			StageOperation sop = _FrozenAt;
			
			if (_FrozenPanel.View)
			{
				ViewedOperation = sop;
			}
			else
			{
				ViewedOperation = null;
			}
		}
		*/

		void HandleSohwFreezeButtonClicked (object sender, EventArgs e)
		{
			StageOperation sop = (sender as StageOperationHolderWidget).Operation;
			FrozenAt = sop;
		}

		void HandleSohwViewButtonClicked (object sender, EventArgs e)
		{
			StageOperation sop = (sender as StageOperationHolderWidget).Operation;
			
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
			StageOperation sop = (sender as StageOperationHolderWidget).Operation;

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
			StageOperation sop = (sender as StageOperationHolderWidget).Operation;

			int index = _StageQueue.IndexOf(sop);
			if (index > 0)
			{
				_StageQueue.Remove(sop);
				_StageQueue.Insert(index - 1, sop);
				ArrangeVBoxes();
				OnOperationIndexChanged();
			}
		}

		public Stages (Gtk.VBox stage_box)
		{
			_StageQueue = new List<StageOperation>();
			_StageVBox = stage_box;
			_FrozenPanel = new FrozenPanel();
			_FrozenPanel.UnfreezeButtonClicked  += delegate {
				FrozenAt = null;
			};
			/*_FrozenPanel.ViewButtonClicked += 
				Handle_FrozenPanelViewButtonClicked;*/
		}
	}
}
