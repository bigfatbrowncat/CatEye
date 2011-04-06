using System;
using System.Xml;
using System.Globalization;

namespace CatEye
{
	[StageOperationID("BasicOpsStageOperation")]
	public class BasicOpsStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mBrightness = 1, mSaturation = 1, mRedPart = 1, mGreenPart = 1, mBluePart = 1;
		
		public double Brightness
		{
			get { return mBrightness; }
			set 
			{
				mBrightness = value;
				OnChanged();
			}
		}
		public double Saturation
		{
			get { return mSaturation; }
			set
			{
				mSaturation = value;
				OnChanged();
			}
		}
		public double RedPart
		{
			get { return mRedPart; }
			set
			{
				mRedPart = value;
				OnChanged();
			}
		}
		public double GreenPart
		{
			get { return mGreenPart; }
			set
			{
				mGreenPart = value;
				OnChanged();
			}
		}
		public double BluePart
		{
			get { return mBluePart; }
			set
			{
				mBluePart = value;
				OnChanged();
			}
		}
		
		public BasicOpsStageOperationParameters ()
		{
		}
		
		public override System.Xml.XmlNode SerializeToXML (System.Xml.XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Brightness")).Value = mBrightness.ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("Saturation")).Value = mSaturation.ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("RedPart")).Value = mRedPart.ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("GreenPart")).Value = mGreenPart.ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("BluePart")).Value = mBluePart.ToString();
			return xn;
		}
		
		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			double res = 0;
			if (node.Attributes["Brightness"] != null)
			{
				if (double.TryParse(node.Attributes["Brightness"].Value, NumberStyles.Float, nfi, out res))
				{
					mBrightness = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Brightness value");
			}
			if (node.Attributes["Saturation"] != null)
			{
				if (double.TryParse(node.Attributes["Saturation"].Value, NumberStyles.Float, nfi, out res))
				{
					mSaturation = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Saturation value");
			}
			if (node.Attributes["RedPart"] != null)
			{
				if (double.TryParse(node.Attributes["RedPart"].Value, NumberStyles.Float, nfi, out res))
				{
					mRedPart = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse RedPart value");
			}
			if (node.Attributes["GreenPart"] != null)
			{
				if (double.TryParse(node.Attributes["GreenPart"].Value, NumberStyles.Float, nfi, out res))
				{
					mGreenPart = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse GreenPart value");
			}
			if (node.Attributes["BluePart"] != null)
			{
				if (double.TryParse(node.Attributes["BluePart"].Value, NumberStyles.Float, nfi, out res))
				{
					mBluePart = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse BluePart value");
			}
			OnChanged();
		}
	}
}

