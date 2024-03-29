using System;
using System.Xml;

namespace CatEye
{
/*	public class IncorrectValueException : Exception
	{
		public IncorrectValueException() : base("Incorrect value") {}
	}*/
	public class MissingStageOperationIDException : Exception
	{
		public MissingStageOperationIDException(string message) : base(message) {}
	}
	
	public class StageDeserializationException : Exception
	{
		public StageDeserializationException(string message) : base(message) {}
	}
	
	public class IncorrectNodeException : StageDeserializationException
	{
		public IncorrectNodeException(string message) : base(message) {}
	}
	public class IncorrectNodeValueException : StageDeserializationException
	{
		public IncorrectNodeValueException(string message) : base(message) {}
	}
	
	[StageOperationID("StageOperation")]
	public class StageOperationParameters : ICloneable
	{
		private bool mActive;
		
		public event EventHandler<EventArgs> Changed;
		
		public bool Active
		{
			get { return mActive; }
			set
			{
				mActive = value;
				OnChanged();
			}
		}
		
		protected string GetStageOperationID()
		{
			object[] attrs = GetType().GetCustomAttributes(typeof(StageOperationIDAttribute), true);
			if (attrs.Length == 0)
			{
				return null;
			}
			
			return ((StageOperationIDAttribute)attrs[0]).ID;
		}
		
		/// <summary>
		/// Serializes object to XML node.
		/// 
		/// Should be overridden in children. Base function should be called in
		/// every child and the child data should be appended to it's result.
		/// </summary>
		/// <returns>
		/// XML node
		/// </returns>
		/// <param name='xdoc'>
		/// Target XML document.
		/// </param>
		public virtual XmlNode SerializeToXML(XmlDocument xdoc)
		{
			XmlNode res = xdoc.CreateElement("StageOperationParameters");
			string ID = GetStageOperationID();
			if (ID == null)
			{
				throw new MissingStageOperationIDException("Can't serialize the object. It's class (" + GetType().Name + ") has no StageOperationID attribute");
			}
			
			res.Attributes.Append(xdoc.CreateAttribute("ID")).Value = ID;
			res.Attributes.Append(xdoc.CreateAttribute("Active")).Value = mActive.ToString();
			
			return res;
		}
		
		public virtual void DeserializeFromXML(XmlNode node)
		{
			if (node.Name != "StageOperationParameters")
				throw new IncorrectNodeException("Node isn't a StageOperationParameters node");
			
			string ID = GetStageOperationID();
			if (ID == null)
			{
				throw new MissingStageOperationIDException("Can't deserialize the object. It's class (" + GetType().Name + ") has no StageOperationID attribute");
			}

			if (node.Attributes["ID"] == null) 
				throw new IncorrectNodeException("Node doesn't contain ID attribute");
				
			if (node.Attributes["ID"].Value != ID)
				
			{
				throw new IncorrectNodeException("Node ID (" + node.Attributes["ID"].Value +
					") is inequal to the ID of the object being loaded (" + ID + ")");
			}

			if (node.Attributes["Active"] != null)
			{
				bool bres;
				if (bool.TryParse(node.Attributes["Active"].Value, out bres))
				{
					Active = bres;
				}
				else
				{
					throw new IncorrectNodeException("Incorrect Active value");
				}
			}
			OnChanged();
		}
		
		protected virtual void OnChanged()
		{
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}
		
		public StageOperationParameters ()
		{
		}
		
		public virtual Type GetSOType ()
		{
			return typeof(StageOperation);
		}
		
		public virtual void CopyDataTo(StageOperationParameters target)
		{
			target.mActive = mActive;
		}
		
		public virtual object Clone ()
		{
			StageOperationParameters target = new StageOperationParameters();
			CopyDataTo(target);
			return target;
		}
	}
}

