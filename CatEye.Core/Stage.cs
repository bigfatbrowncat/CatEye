using System;
using System.Xml;
using System.Collections.Generic;

namespace CatEye.Core
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
	
	public class Stage
	{
		protected List<StageOperation> _StageQueue;
		
		public StageOperation[] StageQueue { get { return _StageQueue.ToArray(); } }
		
		public event EventHandler<OperationRemovedFromStageEventArgs> OperationRemovedFromStage;
		public event EventHandler<OperationAddedToStageEventArgs> OperationAddedToStage;
		public event EventHandler<EventArgs> OperationIndexChanged;
		public event EventHandler<EventArgs> OperationActivityChanged;
		
		protected Type[] mStageOperationParametersTypes = new Type[]
		{
			typeof(CropStageOperationParameters),
			typeof(CompressionStageOperationParameters),
			typeof(BrightnessStageOperationParameters),
			typeof(UltraSharpStageOperationParameters),
			typeof(SaturationStageOperationParameters),
			typeof(ToneStageOperationParameters),
			typeof(HardCutStageOperationParameters),
		};
		
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
		
		public void ApplyAllOperations(FloatPixmap hdp)
		{
			for (int j = 0; j < _StageQueue.Count; j++)
			{
				_StageQueue[j].OnDo(hdp);
			}
		}

		public void AddStageOperation(StageOperation operation)
		{
			_StageQueue.Add(operation);
			
			OnAddedToStage(operation);
		}

		public void RemoveStageOperation(StageOperation operation)
		{
			_StageQueue.Remove(operation);
			OnRemovedFromStage(operation);
		}
		
		public void ClearStage()
		{
			while (_StageQueue.Count > 0) RemoveStageOperation(_StageQueue[0]);
		}
		
		public Stage ()
		{
			_StageQueue = new List<StageOperation>();
		}
		
		public XmlNode SerializeToXML(XmlDocument xdoc)
		{
			XmlNode xn = xdoc.CreateElement("Stages");
			for (int i = 0; i < _StageQueue.Count; i++)
			{
				XmlNode ch = _StageQueue[i].Parameters.SerializeToXML(xdoc);
				xn.AppendChild(ch);
			}
			return xn;
		}
		
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
		
		protected virtual void OnStageOperationDeserialized(StageOperation so, StageOperationParameters sop)
		{
		}
		
		public void DeserializeFromXML(XmlNode xn, Type[] stageOperationTypes)
		{
			if (xn.Name != "Stages")
				throw new IncorrectNodeException("Node isn't a Stages node");
			
			List<StageOperation> sos = new List<StageOperation>();
			Dictionary<StageOperation, bool> actives = new Dictionary<StageOperation, bool>();
			
			for (int i = 0; i < xn.ChildNodes.Count; i++)
			{
				XmlNode ch = xn.ChildNodes[i];
				if (ch.Name == "StageOperationParameters")
				{
					if (ch.Attributes["ID"] == null)
						throw new IncorrectNodeException("StageOperationParameters node doesn't contain ID");
					
					Type sot = FindTypeWithStageOperationIDEqualTo(stageOperationTypes, ch.Attributes["ID"].Value);
					if (sot == null)
						throw new IncorrectNodeValueException("Can't find StageOperation type for the ID (" + ch.Attributes["ID"].Value + ")");
					Type sopt = FindTypeWithStageOperationIDEqualTo(mStageOperationParametersTypes, ch.Attributes["ID"].Value);
					if (sopt == null)
						throw new IncorrectNodeValueException("Can't find StageOperationParameters type for the ID (" + ch.Attributes["ID"].Value + ")");

					// Constructing so-sop structure
					StageOperationParameters sop = (StageOperationParameters)sopt.GetConstructor(
							new Type[] {}
						).Invoke(new object[] {});
					
					StageOperation so = (StageOperation)sot.GetConstructor(
							new Type[] { typeof(StageOperationParameters) }
						).Invoke(new object[] { sop });

					// Deserializing stage operation parameters
					sop.DeserializeFromXML(ch);

					sos.Add(so);
					
					OnStageOperationDeserialized(so, sop);
					
					// Checking "Active"
					if (ch.Attributes["Active"] != null) 
					{
						bool bres;
						if (bool.TryParse(ch.Attributes["Active"].Value, out bres))
						{
							actives.Add(so, bres);
						}
						else
							throw new IncorrectNodeValueException("Can not parse Active value");
					}
				}
			}
			
			ClearStage();
			
			
			for (int i = 0; i < sos.Count; i++)
			{
				AddStageOperation(sos[i]);
			}
			
		}
	}
}
