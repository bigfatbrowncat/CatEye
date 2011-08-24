using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("UltraSharpStageOperation")]
	public class UltraSharpStageOperationParameters : StageOperationParameters
	{
		public enum SharpType { Sharp, Soft }
		
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mPressure = 10, mContrast = 0.5, mRadius = 0.1;
		private SharpType mType = SharpType.Sharp;

		public double Contrast
		{
			get { return mContrast; }
			set 
			{
				mContrast = value;
				OnChanged();
			}
		}
		
		public double Pressure
		{
			get { return mPressure; }
			set 
			{
				mPressure = value;
				OnChanged();
			}
		}

		public double Radius
		{
			get { return mRadius; }
			set 
			{
				mRadius = value;
				OnChanged();
			}
		}
		
		public SharpType Type
		{
			get { return mType; }
			set 
			{
				mType = value;
				OnChanged();
			}
		}
		
		public override XmlNode SerializeToXML (XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Pressure")).Value = mPressure.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("Contrast")).Value = mContrast.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("Radius")).Value = mRadius.ToString(nfi);
			string st = "";
			if (mType == SharpType.Sharp)
				st = "sharp";
			else
				st = "soft";
			
			xn.Attributes.Append(xdoc.CreateAttribute("SharpType")).Value = st;
			
			return xn;
		}

		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			double res = 0;
			if (node.Attributes["Pressure"] != null)
			{
				if (double.TryParse(node.Attributes["Pressure"].Value, NumberStyles.Float, nfi, out res))
				{
					mPressure = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Pressure value");
			}
			if (node.Attributes["Contrast"] != null)
			{
				if (double.TryParse(node.Attributes["Contrast"].Value, NumberStyles.Float, nfi, out res))
				{
					mContrast = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Contrast value");
			}
			if (node.Attributes["Radius"] != null)
			{
				if (double.TryParse(node.Attributes["Radius"].Value, NumberStyles.Float, nfi, out res))
				{
					mRadius = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Radius value");
			}
			if (node.Attributes["SharpType"] != null)
			{
				
				if (node.Attributes["SharpType"].Value == "sharp")
				{
					mType = SharpType.Sharp;
				}
				else if (node.Attributes["SharpType"].Value == "soft")
				{
					mType = SharpType.Soft;
				}
				else
					throw new IncorrectNodeValueException("Can't parse LimitDown value");
			}
			OnChanged();
		}
		
		public UltraSharpStageOperationParameters ()
		{
		}

		public override Type GetSOType ()
		{
			return typeof(UltraSharpStageOperation);
		}

		public override void CopyDataTo (StageOperationParameters target)
		{
			base.CopyDataTo (target);
			UltraSharpStageOperationParameters t = (UltraSharpStageOperationParameters)target;
			t.mPressure = mPressure;
			t.mContrast = mContrast;
			t.mRadius = mRadius;
			t.mType = mType;
			t.OnChanged();
		}
		
		public override object Clone ()
		{
			UltraSharpStageOperationParameters target = new UltraSharpStageOperationParameters();
			CopyDataTo(target);
			return target;
		}
	}
}

