using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("ToneStageOperation")]
	public class ToneStageOperationParameters : StageOperationParameters
	{
		private Tone mTone = new Tone(1, 1, 1);
		
		public Tone Tone
		{
			get { return mTone; }
			set
			{
				mTone = value;
				OnChanged();
			}
		}
		
		public ToneStageOperationParameters ()
		{
		}
		
		public override System.Xml.XmlNode SerializeToXML (System.Xml.XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.AppendChild(mTone.SerializeToXML(xdoc));
			return xn;
		}
		
		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			foreach (XmlNode xn in node.ChildNodes)
			{
				if (xn.Name == "Tone") mTone.DeserializeFromXML(xn);
			}
			
			OnChanged();
		}
	}
}

