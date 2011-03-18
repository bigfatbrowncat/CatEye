
using System;
using System.Collections.Generic;

namespace CatEye
{
	public enum Stage { Stage2, Stage3 };
	public class OperationRemovedFromStageEventArgs : EventArgs
	{
		Stage _SourceStage;
		StageOperation _Operation;
		public Stage SourceStage { get { return _SourceStage; } }
		public OperationRemovedFromStageEventArgs(StageOperation operation, Stage source)
		{
			_SourceStage = source;
			_Operation = operation;
		}
	}
	
	public class OperationAddedToStageEventArgs : EventArgs
	{
		Stage _TargetStage;
		StageOperation _Operation;
		public Stage TargetStage { get { return _TargetStage; } }
		public StageOperation Operation { get { return _Operation; } }
		public OperationAddedToStageEventArgs(StageOperation operation, Stage target)
		{
			_TargetStage = target;
			_Operation = operation;
		}
	}
	
	public class Stages
	{
		private List<StageOperation> _Stage2, _Stage3;
		private Gtk.VBox _Stage2VBox, _Stage3VBox;
		private Dictionary<StageOperation, StageOperationHolderWidget> _Holders = new Dictionary<StageOperation, StageOperationHolderWidget>();
		
		public StageOperation[] Stage2 { get { return _Stage2.ToArray(); } }
		public StageOperation[] Stage3 { get { return _Stage3.ToArray(); } }
		
		public event EventHandler<OperationRemovedFromStageEventArgs> OperationRemovedFromStage;
		public event EventHandler<OperationAddedToStageEventArgs> OperationAddedToStage;
		public event EventHandler<EventArgs> OperationIndexChanged;

		protected virtual void OnAddedToStage(StageOperation operation, Stage target)
		{
			if (OperationAddedToStage != null) 
				OperationAddedToStage(this, new OperationAddedToStageEventArgs(operation, target));
		}
		protected virtual void OnRemovedFromStage(StageOperation operation, Stage source)
		{
			if (OperationRemovedFromStage != null) 
				OperationRemovedFromStage(this, new OperationRemovedFromStageEventArgs(operation, source));
		}
		protected virtual void OnOperationindexChanged()
		{
			if (OperationIndexChanged != null)
				OperationIndexChanged(this, EventArgs.Empty);
		}
		
		public void DoStage2(DoublePixmap hdp)
		{
			for (int j = 0; j < _Stage2.Count; j++)
			{
				if (_Holders[_Stage2[j]].Active)
					_Stage2[j].OnDo(hdp);
			}
		}
		public void DoStage3(DoublePixmap hdp)
		{
			for (int j = 0; j < _Stage3.Count; j++)
			{
				if (_Holders[_Stage3[j]].Active)
					_Stage3[j].OnDo(hdp);
			}
		}
		
		public Stage StageOf(StageOperation sop)
		{
			if (_Stage2.Contains(sop)) return Stage.Stage2;
			if (_Stage3.Contains(sop)) return Stage.Stage3;
			throw new Exception("The operation isn't contained in any stage.");
		}
		
		protected void ArrangeVBoxes()
		{
			// Arranging stage 2
			for (int i = 0; i < _Stage2.Count; i++)
			{
				StageOperationHolderWidget sohw = _Holders[_Stage2[i]];
				((Gtk.Box.BoxChild)_Stage2VBox[sohw]).Position = i;
			}
			// Arranging stage 3
			for (int i = 0; i < _Stage3.Count; i++)
			{
				StageOperationHolderWidget sohw = _Holders[_Stage3[i]];
				((Gtk.Box.BoxChild)_Stage3VBox[sohw]).Position = i;
			}
		}
		
		public void AddStageOperation(StageOperation operation, Stage stage)
		{
			if (stage == Stage.Stage2)
				_Stage2.Add(operation);
			else
				_Stage3.Add(operation);
				
			// Creating stage operation holder
			StageOperationHolderWidget sohw = new StageOperationHolderWidget(operation);
			sohw.CurrentStage = stage;

			// Getting stage operation name from attribute
			object[] attrs = operation.GetType().GetCustomAttributes(typeof(StageOperationDescriptionAttribute), true);
			if (attrs != null && attrs.Length > 0)
				sohw.Title = (attrs[0] as StageOperationDescriptionAttribute).Name;
			
			sohw.ChangeStageTitleButtonClicked += HandleHolderTitleWidgetChangeStageButtonClicked;
			sohw.UpTitleButtonClicked += HandleSohwUpTitleButtonClicked;
			sohw.DownTitleButtonClicked += HandleSohwDownTitleButtonClicked;
			
			if (stage == Stage.Stage2)
			{
				_Stage2VBox.Add(sohw);
				((Gtk.Box.BoxChild)_Stage2VBox[sohw]).Fill = false;
				((Gtk.Box.BoxChild)_Stage2VBox[sohw]).Expand = false;
			}
			else
			{
				_Stage3VBox.Add(sohw);
				((Gtk.Box.BoxChild)_Stage3VBox[sohw]).Fill = false;
				((Gtk.Box.BoxChild)_Stage3VBox[sohw]).Expand = false;
			}
			
			_Holders.Add(operation, sohw);
			sohw.Show();
			ArrangeVBoxes();
			OnAddedToStage(operation, stage);
		}

		void HandleSohwDownTitleButtonClicked (object sender, EventArgs e)
		{
			StageOperation sop = (sender as StageOperationHolderWidget).Operation;
			Stage stg = StageOf(sop);
			if (stg == Stage.Stage2)
			{
				int index = _Stage2.IndexOf(sop);
				if (index < _Stage2.Count - 1)
				{
					_Stage2.Remove(sop);
					_Stage2.Insert(index + 1, sop);
					ArrangeVBoxes();
					OnOperationindexChanged();
				}
			}
			else if (stg == Stage.Stage3)
			{
				int index = _Stage3.IndexOf(sop);
				if (index < _Stage3.Count - 1)
				{
					_Stage3.Remove(sop);
					_Stage3.Insert(index + 1, sop);
					ArrangeVBoxes();
					OnOperationindexChanged();
				}
			}
		}

		void HandleSohwUpTitleButtonClicked (object sender, EventArgs e)
		{
			StageOperation sop = (sender as StageOperationHolderWidget).Operation;
			Stage stg = StageOf(sop);
			if (stg == Stage.Stage2)
			{
				int index = _Stage2.IndexOf(sop);
				if (index > 0)
				{
					_Stage2.Remove(sop);
					_Stage2.Insert(index - 1, sop);
					ArrangeVBoxes();
					OnOperationindexChanged();
				}
			}
			else if (stg == Stage.Stage3)
			{
				int index = _Stage3.IndexOf(sop);
				if (index > 0)
				{
					_Stage3.Remove(sop);
					_Stage3.Insert(index - 1, sop);
					ArrangeVBoxes();
					OnOperationindexChanged();
				}
			}
		}

		void HandleHolderTitleWidgetChangeStageButtonClicked (object sender, EventArgs e)
		{
			StageOperation sop = (sender as StageOperationHolderWidget).Operation;
			ToggleStage(sop);
			(sender as StageOperationHolderWidget).CurrentStage = StageOf(sop);
		}
		
		public void ToggleStage(StageOperation operation)
		{
			Stage curStage = StageOf(operation);
				
			
			StageOperationHolderWidget sohw = _Holders[operation];
			if (curStage == Stage.Stage2)
			{
				_Stage2VBox.Remove(sohw);
				sohw.CurrentStage = Stage.Stage3;
				_Stage3VBox.Add(sohw);

				((Gtk.Box.BoxChild)_Stage3VBox[sohw]).Fill = false;
				((Gtk.Box.BoxChild)_Stage3VBox[sohw]).Expand = false;
				
				_Stage2.Remove(operation);
				_Stage3.Add(operation);
				
				ArrangeVBoxes();
				OnRemovedFromStage(operation, Stage.Stage2);
				OnAddedToStage(operation, Stage.Stage3);
			}
			else
			{
				_Stage3VBox.Remove(sohw);
				sohw.CurrentStage = Stage.Stage2;
				_Stage2VBox.Add(sohw);

				((Gtk.Box.BoxChild)_Stage2VBox[sohw]).Fill = false;
				((Gtk.Box.BoxChild)_Stage2VBox[sohw]).Expand = false;

				_Stage3.Remove(operation);
				_Stage2.Add(operation);

				OnRemovedFromStage(operation, Stage.Stage3);
				OnAddedToStage(operation, Stage.Stage2);
				ArrangeVBoxes();
			}
		}
		
		public Stages (Gtk.VBox stage2_box, Gtk.VBox stage3_box)
		{
			_Stage2 = new List<StageOperation>();
			_Stage3 = new List<StageOperation>();
			
			_Stage2VBox = stage2_box;
			_Stage3VBox = stage3_box;
			
		}
	}
}
