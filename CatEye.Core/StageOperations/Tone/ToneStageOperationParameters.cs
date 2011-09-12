using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("ToneStageOperation")]
	public class ToneStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		private Tone mTone = new Tone(1, 1, 1);
		private double mEdge = 0.95;
		private double mSoftness = 0.1;
		
		public Tone Tone
		{
			get { return mTone; }
			set
			{
				mTone = value;
				OnChanged();
			}
		}
		
		public double Edge
		{
			get { return mEdge; }
			set
			{
				mEdge = value;
				OnChanged();
			}
		}
		
		public double Softness
		{
			get { return mSoftness; }
			set
			{
				mSoftness = value;
				OnChanged();
			}
		}
		
		public ToneStageOperationParameters ()
		{
		}
		
		public override System.Xml.XmlNode SerializeToXML (System.Xml.XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Edge")).Value = mEdge.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("Softness")).Value = mSoftness.ToString(nfi);
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

			double res = 0;
			if (node.Attributes["Edge"] != null)
			{
				if (double.TryParse(node.Attributes["Edge"].Value, NumberStyles.Float, nfi, out res))
				{
					mEdge = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Edge value");
			}

			res = 0;
			if (node.Attributes["Softness"] != null)
			{
				if (double.TryParse(node.Attributes["Softness"].Value, NumberStyles.Float, nfi, out res))
				{
					mSoftness = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Softness value");
			}
			
			OnChanged();
		}
		
		public override Type GetSOType ()
		{
			return typeof(ToneStageOperation);
		}
		
		public override void CopyDataTo (StageOperationParameters target)
		{
			base.CopyDataTo (target);
			ToneStageOperationParameters t = (ToneStageOperationParameters)target;
			t.mTone = (Tone)mTone.Clone();
			t.mEdge = mEdge;
			t.mSoftness = mSoftness;
			t.OnChanged();
		}
		
		public override object Clone ()
		{
			ToneStageOperationParameters target = new ToneStageOperationParameters();
			CopyDataTo(target);
			return target;
		}
		
	}
}

