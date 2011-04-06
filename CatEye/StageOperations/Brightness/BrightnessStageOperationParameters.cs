using System;
using System.Xml;
using System.Globalization;

namespace CatEye
{
	[StageOperationID("BrightnessStageOperation")]
	public class BrightnessStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mBrightness = 1;
		
		public double Brightness
		{
			get { return mBrightness; }
			set 
			{
				mBrightness = value;
				OnChanged();
			}
		}

		public BrightnessStageOperationParameters ()
		{
		}
		
		public override XmlNode SerializeToXML (XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Brightness")).Value = Brightness.ToString();
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
			OnChanged();
		}
	}
}

