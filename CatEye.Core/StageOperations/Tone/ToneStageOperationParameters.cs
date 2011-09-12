using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("ToneStageOperation")]
	public class ToneStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		private Tone mDarkTone = new Tone(1, 1, 1);
		private Tone mLightTone = new Tone(1, 1, 1);
		private double mEdge = 0.95;
		private double mSoftness = 0.1;
		
		public Tone DarkTone
		{
			get { return mDarkTone; }
			set
			{
				mDarkTone = value;
				OnChanged();
			}
		}
		public Tone LightTone
		{
			get { return mLightTone; }
			set
			{
				mLightTone = value;
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
			xn.AppendChild(mDarkTone.SerializeToXML(xdoc)).Attributes.Append(xdoc.CreateAttribute("Name")).Value = "DarkTone";
			xn.AppendChild(mLightTone.SerializeToXML(xdoc)).Attributes.Append(xdoc.CreateAttribute("Name")).Value = "LightTone";
			return xn;
		}
		
		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			foreach (XmlNode xn in node.ChildNodes)
			{
				if (xn.Name == "Tone" && 
					xn.Attributes["Name"] != null &&
					xn.Attributes["Name"].Value == "DarkTone") mDarkTone.DeserializeFromXML(xn);
				if (xn.Name == "Tone" && 
					xn.Attributes["Name"] != null &&
					xn.Attributes["Name"].Value == "LightTone") mLightTone.DeserializeFromXML(xn);
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
			t.mDarkTone = (Tone)mDarkTone.Clone();
			t.mLightTone = (Tone)mLightTone.Clone();
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

