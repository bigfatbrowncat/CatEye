
using System;
using System.Collections.Generic;

namespace CatEye
{
	public enum Stage { Stage2, Stage3 };
	public class MovedToStageEventArgs : EventArgs
	{
		Stage _SourceStage, _DestinationStage;
		public Stage SourceStage { get { return _SourceStage; } }
		public Stage DestinationStage { get { return _DestinationStage; } }
		public MovedToStageEventArgs(Stage source, Stage destination)
		{
			_DestinationStage = destination;
			_SourceStage = source;
		}
	}
	public class IndexChangedEventArgs : EventArgs
	{
		int _OldIndex, _NewIndex;
		public int OldIndex { get { return _OldIndex; } }
		public int NewIndex { get { return _NewIndex; } }
		public IndexChangedEventArgs(int oldindex, int newindex)
		{
			_NewIndex = newindex;
			_OldIndex = oldindex;
		}
	}
	
	public class DoStageOperationEventArgs : EventArgs
	{
		DoublePixmap _hdp;
		public DoublePixmap HDP { get { return _hdp; } }
		public DoStageOperationEventArgs(DoublePixmap hdp) { _hdp = hdp; }
	}
	
	public class AddedToStageEventArgs : EventArgs
	{
		Stage _TargetStage;
		public Stage TargetStage { get { return _TargetStage; } }
		public AddedToStageEventArgs(Stage target)
		{
			_TargetStage = target;
		}
	}
	public class StageOperation : IComparable<StageOperation>
	{
		public int CompareTo (StageOperation other)
		{
			return Index - other.Index;
		}
		
		public event EventHandler<MovedToStageEventArgs> MovedToStage;
		public event EventHandler<AddedToStageEventArgs> AddedToStage;
		public event EventHandler<DoStageOperationEventArgs> Do;
		public event EventHandler<IndexChangedEventArgs> IndexChanged;
		
		private Stage _CurrentStage;
		public Stage CurrentStage { get { return _CurrentStage; } } 
		
		protected internal void SetStage(Stage s) { _CurrentStage = s; }
		
		private int _Index;
		public int Index
		{
			get { return _Index; }
			set
			{
				if (value != _Index)
				{
					int oldi = _Index;
					_Index = value;
					if (IndexChanged != null) 
						IndexChanged(this, new IndexChangedEventArgs(oldi, _Index));
				}
			}
		}
		

		public void AddToStage(Stage target)
		{
			if (MovedToStage != null) 
			{
				_CurrentStage = target;
				if (AddedToStage != null)
					AddedToStage(this, new AddedToStageEventArgs(target));
			}
		}
		public StageOperation(int index)
		{
			Index = index;
		}
		
		public virtual void OnDo(DoublePixmap hdp)
		{
			if (Do != null) 
				Do(this, new DoStageOperationEventArgs(hdp));
		}
		
		protected internal virtual void OnMovedToStage(Stage oldstage, Stage newstage)
		{
			if (MovedToStage != null) 
				MovedToStage(this, new MovedToStageEventArgs(oldstage, newstage));
		}
	}

	public class Stages
	{
		private List<StageOperation> _Stage2, _Stage3;
		public StageOperation[] Stage2 { get { return _Stage2.ToArray(); } }
		public StageOperation[] Stage3 { get { return _Stage3.ToArray(); } }

		public void ChangeStage(StageOperation sop)
		{
			Stage olds = Stage.Stage2, news = Stage.Stage2;
			
			if (sop.CurrentStage == Stage.Stage2)
			{
				olds = Stage.Stage2;
				news = Stage.Stage3;
				
				_Stage2.Remove(sop);
				_Stage3.Add(sop);
			}
			else if (sop.CurrentStage == Stage.Stage3)
			{
				olds = Stage.Stage3;
				news = Stage.Stage2;
				
				_Stage3.Remove(sop);
				_Stage2.Add(sop);
			}
			sop.SetStage(news);
			sop.OnMovedToStage(olds, news);
		}
		
		public void DoStage2(DoublePixmap hdp)
		{
			for (int i = 0; i <= _Stage2.Count + _Stage3.Count - 1; i++)
			{
				for (int j = 0; j < _Stage2.Count; j++)
				{
					if (_Stage2[j].Index == i)
					{
						_Stage2[j].OnDo(hdp);
					}
				}
			}
		}
		public void DoStage3(DoublePixmap hdp)
		{
			for (int i = 0; i <= _Stage2.Count + _Stage3.Count - 1; i++)
			{
				for (int j = 0; j < _Stage3.Count; j++)
				{
					if (_Stage3[j].Index == i)
					{
						_Stage3[j].OnDo(hdp);
					}
				}
			}
		}
		
		public Stages (StageOperation[] stage_ops)
		{
			_Stage2 = new List<StageOperation>();
			_Stage3 = new List<StageOperation>();
			for (int i = 0; i < stage_ops.Length; i++)
			{
				if (stage_ops[i].CurrentStage == Stage.Stage2)
				{
					_Stage2.Add(stage_ops[i]);
				}
				if (stage_ops[i].CurrentStage == Stage.Stage3) 
				{
					_Stage3.Add(stage_ops[i]);
				}
			}
		}
		public StageOperation[] AllOperationsSorted
		{
			get
			{
				List<StageOperation> sop = new List<StageOperation>();
				sop.AddRange(_Stage2);
				sop.AddRange(_Stage3);
				sop.Sort();
				return sop.ToArray();
			}
		}
		
		public void MoveStageOperationUp(StageOperation op)
		{
			if (op.CurrentStage == Stage.Stage2)
			{
				for (int k = 1; op.Index - k >= 0; k++)
				for (int i = 0; i < _Stage2.Count; i++)
				{
					if (_Stage2[i].Index == op.Index - k)
					{
						_Stage2[i].Index += k;
						op.Index -= k;
						return;
					}
				}
			}
			else if (op.CurrentStage == Stage.Stage3)
			{
				for (int k = 1; op.Index - k >= 0; k++)
				for (int i = 0; i < _Stage3.Count; i++)
				{
					if (_Stage3[i].Index == op.Index - k)
					{
						_Stage3[i].Index += k;
						op.Index -= k;
						return;
					}
				}
			}
		}

		public void MoveStageOperationDown(StageOperation op)
		{
			if (op.CurrentStage == Stage.Stage2)
			{
				for (int k = 1; op.Index + k < _Stage2.Count + _Stage3.Count; k++)
				for (int i = 0; i < _Stage2.Count; i++)
				{
					if (_Stage2[i].Index == op.Index + k)
					{
						_Stage2[i].Index -= k;
						op.Index += k;
						return;
					}
				}
			}
			else if (op.CurrentStage == Stage.Stage3)
			{
				for (int k = 1; op.Index + k < _Stage2.Count + _Stage3.Count; k++)
				for (int i = 0; i < _Stage3.Count; i++)
				{
					if (_Stage3[i].Index == op.Index + k)
					{
						_Stage3[i].Index -= k;
						op.Index += k;
						return;
					}
				}
			}
		}		
	}
}
