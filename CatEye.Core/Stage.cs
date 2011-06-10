using System;
using System.Xml;
using System.Collections.Generic;

namespace CatEye.Core
{
	public class StageOperationParametersEventArgs : EventArgs
	{
		StageOperationParameters _Target;
		public StageOperationParameters Target { get { return _Target; } }
		public StageOperationParametersEventArgs(StageOperationParameters target)
		{
			_Target = target;
		}
	}
	
	public class Stage
	{
		protected StageOperationFactory _StageOperationFactory;
		protected StageOperationParametersFactoryFromID _StageOperationParametersFactoryFromID;
		protected List<StageOperationParameters> _StageQueue;
		
		public StageOperationParameters[] StageQueue { get { return _StageQueue.ToArray(); } }
		
		public event EventHandler<StageOperationParametersEventArgs> ItemRemoved;
		public event EventHandler<StageOperationParametersEventArgs> ItemAdded;
		public event EventHandler<StageOperationParametersEventArgs> ItemIndexChanged;
		public event EventHandler<StageOperationParametersEventArgs> ItemChanged;
		
		void HandleItemChanged(object sender, EventArgs e)
		{
			OnItemChanged((StageOperationParameters)sender);			
		}

		public Stage(StageOperationFactory stageOperationFactory, 
			StageOperationParametersFactoryFromID stageOperationParametersFactoryFromID)
		{
			_StageQueue = new List<StageOperationParameters>();
			_StageOperationFactory = stageOperationFactory;
			_StageOperationParametersFactoryFromID = stageOperationParametersFactoryFromID;
		}
		
		protected virtual void OnItemAdded(StageOperationParameters item)
		{
			if (ItemAdded != null) 
				ItemAdded(this, new StageOperationParametersEventArgs(item));
		}
		protected virtual void OnItemRemoved(StageOperationParameters item)
		{
			if (ItemRemoved != null) 
				ItemRemoved(this, new StageOperationParametersEventArgs(item));
		}
		protected virtual void OnItemIndexChanged(StageOperationParameters item)
		{
			if (ItemIndexChanged != null)
				ItemIndexChanged(this, new StageOperationParametersEventArgs(item));
		}
		protected virtual void OnItemChanged(StageOperationParameters item)
		{
			if (ItemChanged != null)
				ItemChanged(this, new StageOperationParametersEventArgs(item));
		}
		
		public void ApplyAllOperations(IBitmapCore hdp)
		{
			for (int j = 0; j < _StageQueue.Count; j++)
			{
				if (_StageQueue[j].Active)
				{
					StageOperation so = _StageOperationFactory(_StageQueue[j]);
					so.OnDo(hdp);
				}
			}
		}
		
		public double CalculateAllEfforts(IBitmapCore hdp)
		{
			double res = 0;
			for (int j = 0; j < _StageQueue.Count; j++)
			{
				if (_StageQueue[j].Active)
				{
					StageOperation so = _StageOperationFactory(_StageQueue[j]);
					res += so.CalculateEfforts(hdp);
				}
			}
			return res;
		}

		public void Add(StageOperationParameters newItem)
		{
			_StageQueue.Add(newItem);
			newItem.Changed += HandleItemChanged;
			OnItemAdded(newItem);
		}

		public void Remove(StageOperationParameters item)
		{
			_StageQueue.Remove(item);
			item.Changed -= HandleItemChanged;
			OnItemRemoved(item);
		}

		public void StepDown(StageOperationParameters item)
		{
			int index = _StageQueue.IndexOf(item);
			if (index < _StageQueue.Count - 1)
			{
				_StageQueue.Remove(item);
				_StageQueue.Insert(index + 1, item);
				OnItemIndexChanged(item);
			}
		}

		public void StepUp(StageOperationParameters item)
		{
			int index = _StageQueue.IndexOf(item);
			if (index > 0)
			{
				_StageQueue.Remove(item);
				_StageQueue.Insert(index - 1, item);
				OnItemIndexChanged(item);
			}
		}
		
		public void Clear()
		{
			while (_StageQueue.Count > 0)
				Remove(_StageQueue[0]);
		}
		
		public XmlNode SerializeToXML(XmlDocument xdoc)
		{
			XmlNode xn = xdoc.CreateElement("Stages");
			for (int i = 0; i < _StageQueue.Count; i++)
			{
				XmlNode ch = _StageQueue[i].SerializeToXML(xdoc);
				xn.AppendChild(ch);
			}
			return xn;
		}

		/*
		protected Type FindTypeWithStageOperationIDEqualTo(Type[] types, string id)
		{
			for (int i = 0; i < types.Length; i++)
			{
				object[] attrs = types[i].GetCustomAttributes(typeof(StageOperationIDAttribute), true);
				if (attrs.Length == 0)
				{
					continue;
				}
				if (((StageOperationIDAttribute)attrs[0]).ID == id)
					return types[i];
			}
			return null;
		}
		*/
		
		protected virtual void OnItemDeserialized(StageOperationParameters sop)
		{
		}
		
		public void DeserializeFromXML(XmlNode xn)
		{
			if (xn.Name != "Stages")
				throw new IncorrectNodeException("Node isn't a Stages node");
			
			List<StageOperationParameters> sops = new List<StageOperationParameters>();
			
			for (int i = 0; i < xn.ChildNodes.Count; i++)
			{
				XmlNode ch = xn.ChildNodes[i];
				if (ch.Name == "StageOperationParameters")
				{
					if (ch.Attributes["ID"] == null)
						throw new IncorrectNodeException("StageOperationParameters node doesn't contain ID");
					
					StageOperationParameters sop = _StageOperationParametersFactoryFromID(ch.Attributes["ID"].Value);
					 
					// Deserializing stage operation parameters
					sop.DeserializeFromXML(ch);
					sops.Add(sop);
					
					OnItemDeserialized(sop);
				}
			}
			
			Clear();
			
			for (int i = 0; i < sops.Count; i++)
			{
				Add(sops[i]);
			}
			
		}
	}
}
