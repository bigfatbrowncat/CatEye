using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("CompressionStageOperation")]
	public class CompressionStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mPower = 0.7, mDarkPreserving = 0.01;

		public double Power
		{
			get { return mPower; }
			set 
			{
				mPower = value;
				OnChanged();
			}
		}
		public double DarkPreserving
		{
			get { return mDarkPreserving; }
			set 
			{
				mDarkPreserving = value;
				OnChanged();
			}
		}
		
		public override XmlNode SerializeToXML (XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Power")).Value = mPower.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("DarkPreserving")).Value = mDarkPreserving.ToString(nfi);
			return xn;
		}

		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			double res = 0;
			if (node.Attributes["Power"] != null)
			{
				if (double.TryParse(node.Attributes["Power"].Value, NumberStyles.Float, nfi, out res))
				{
					mPower = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Brightness value");
			}
			if (node.Attributes["DarkPreserving"] != null)
			{
				if (double.TryParse(node.Attributes["DarkPreserving"].Value, NumberStyles.Float, nfi, out res))
				{
					mDarkPreserving = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse DarkPreserving value");
			}
			OnChanged();
		}
		
		public CompressionStageOperationParameters ()
		{
		}

		public override Type GetSOType ()
		{
			return typeof(CompressionStageOperation);
		}
		
	}
}

