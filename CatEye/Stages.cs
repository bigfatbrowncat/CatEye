
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
		private List<StageOperation> _StageQueue;
		private Gtk.VBox _StageVBox;
		private Dictionary<StageOperation, StageOperationHolderWidget> _Holders = 
			new Dictionary<StageOperation, StageOperationHolderWidget>();
		
		public StageOperation[] StageQueue { get { return _StageQueue.ToArray(); } }
		
		public event EventHandler<OperationRemovedFromStageEventArgs> OperationRemovedFromStage;
		public event EventHandler<OperationAddedToStageEventArgs> OperationAddedToStage;
		public event EventHandler<EventArgs> OperationIndexChanged;
		public event EventHandler<EventArgs> OperationActivityChanged;

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
		
		public bool ApplyOperations(DoublePixmap hdp)
		{
			try
			{
				for (int j = 0; j < _StageQueue.Count; j++)
				{
					if (_Holders[_StageQueue[j]].Active)
						_StageQueue[j].OnDo(hdp);
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
			
			_StageVBox.Add(sohw);
			((Gtk.Box.BoxChild)_StageVBox[sohw]).Fill = false;
			((Gtk.Box.BoxChild)_StageVBox[sohw]).Expand = false;
			
			_Holders.Add(operation, sohw);
			sohw.Show();
			ArrangeVBoxes();
			OnAddedToStage(operation);
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
		}
	}
}
