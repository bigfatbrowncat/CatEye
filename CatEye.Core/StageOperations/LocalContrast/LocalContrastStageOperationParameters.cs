using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("LocalContrastStageOperation")]
	public class LocalContrastStageOperationParameters : StageOperationParameters
	{
		public enum SharpType { Sharp, Soft }
		
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mPressure = 10, mCurve = 0.5, /*mCompression = 0.5,*/ mContrast = 0.1;
		private SharpType mType = SharpType.Sharp;

/*		public double Compression
		{
			get { return mCompression; }
			set 
			{
				mCompression = value;
				OnChanged();
			}
		}*/

		public double Curve
		{
			get { return mCurve; }
			set 
			{
				mCurve = value;
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

		public double Contrast
		{
			get { return mContrast; }
			set 
			{
				mContrast = value;
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
//			xn.Attributes.Append(xdoc.CreateAttribute("Compression")).Value = mCompression.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("Curve")).Value = mCurve.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("Pressure")).Value = mPressure.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("Contrast")).Value = mContrast.ToString(nfi);
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
/*			if (node.Attributes["Compression"] != null)
			{
				if (double.TryParse(node.Attributes["Compression"].Value, NumberStyles.Float, nfi, out res))
				{
					mCompression = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Compression value");
			}*/
			if (node.Attributes["Curve"] != null)
			{
				if (double.TryParse(node.Attributes["Curve"].Value, NumberStyles.Float, nfi, out res))
				{
					mCurve = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Curve value");
			}
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
					throw new IncorrectNodeValueException("Can't parse SharpType value");
			}
			OnChanged();
		}
		
		public LocalContrastStageOperationParameters ()
		{
		}

		public override Type GetSOType ()
		{
			return typeof(LocalContrastStageOperation);
		}

		public override void CopyDataTo (StageOperationParameters target)
		{
			base.CopyDataTo (target);
			LocalContrastStageOperationParameters t = (LocalContrastStageOperationParameters)target;
//			t.mCompression = mCompression;
			t.mCurve = mCurve;
			t.mPressure = mPressure;
			t.mContrast = mContrast;
			t.mType = mType;
			t.OnChanged();
		}
		
		public override object Clone ()
		{
			LocalContrastStageOperationParameters target = new LocalContrastStageOperationParameters();
			CopyDataTo(target);
			return target;
		}
	}
}

