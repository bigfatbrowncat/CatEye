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
		private bool mAutoDarkTone = false;
		private bool mAutoLightTone = false;
		private Point mAutoDarkCenter = new Point(0, 0);
		private Point mAutoLightCenter = new Point(0, 0);
		private double mAutoDarkRadius;
		private double mAutoLightRadius;
		
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
		
		public bool AutoDarkTone
		{
			get { return mAutoDarkTone; }
			set
			{
				mAutoDarkTone = value;
				OnChanged();
			}
		}

		public bool AutoLightTone
		{
			get { return mAutoLightTone; }
			set
			{
				mAutoLightTone = value;
				OnChanged();
			}
		}
		
		public Point AutoDarkCenter
		{
			get { return mAutoDarkCenter; }
			set
			{
				mAutoDarkCenter = value;
				OnChanged();
			}
		}

		public Point AutoLightCenter
		{
			get { return mAutoLightCenter; }
			set
			{
				mAutoLightCenter = value;
				OnChanged();
			}
		}

		public double AutoDarkRadius
		{
			get { return mAutoDarkRadius; }
			set
			{
				mAutoDarkRadius = value;
				OnChanged();
			}
		}

		public double AutoLightRadius
		{
			get { return mAutoLightRadius; }
			set
			{
				mAutoLightRadius = value;
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
			
			xn.Attributes.Append(xdoc.CreateAttribute("AutoDarkTone")).Value = mAutoDarkTone.ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("AutoLightTone")).Value = mAutoLightTone.ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("AutoDarkRadius")).Value = mAutoDarkRadius.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("AutoLightRadius")).Value = mAutoLightRadius.ToString(nfi);
			xn.AppendChild(mAutoDarkCenter.SerializeToXML(xdoc)).Attributes.Append(xdoc.CreateAttribute("Name")).Value = "AutoDarkCenter";
			xn.AppendChild(mAutoLightCenter.SerializeToXML(xdoc)).Attributes.Append(xdoc.CreateAttribute("Name")).Value = "AutoLightCenter";
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
				if (xn.Name == "Point" &&
					xn.Attributes["Name"] != null &&
					xn.Attributes["Name"].Value == "AutoDarkCenter") mAutoDarkCenter.DeserializeFromXML(xn);
				if (xn.Name == "Point" &&
					xn.Attributes["Name"] != null &&
					xn.Attributes["Name"].Value == "AutoLightCenter") mAutoLightCenter.DeserializeFromXML(xn);
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
			
			res = 0;
			if (node.Attributes["AutoDarkRadius"] != null)
			{
				if (double.TryParse(node.Attributes["AutoDarkRadius"].Value, NumberStyles.Float, nfi, out res))
				{
					mAutoDarkRadius = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse AutoDarkRadius value");
			}

			res = 0;
			if (node.Attributes["AutoLightRadius"] != null)
			{
				if (double.TryParse(node.Attributes["AutoLightRadius"].Value, NumberStyles.Float, nfi, out res))
				{
					mAutoLightRadius = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse AutoLightRadius value");
			}
			
			bool bres = false;
			if (node.Attributes["AutoDarkTone"] != null)
			{
				if (bool.TryParse(node.Attributes["AutoDarkTone"].Value, out bres))
				{
					mAutoDarkTone = bres;
				}
				else
					throw new IncorrectNodeValueException("Can't parse AutoDarkTone value");
			}
			
			bres = false;
			if (node.Attributes["AutoLightTone"] != null)
			{
				if (bool.TryParse(node.Attributes["AutoLightTone"].Value, out bres))
				{
					mAutoLightTone = bres;
				}
				else
					throw new IncorrectNodeValueException("Can't parse AutoLightTone value");
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

