using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("SaturationStageOperation")]
	public class SaturationStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mSaturation = 1;
		
		public double Saturation
		{
			get { return mSaturation; }
			set
			{
				mSaturation = value;
				OnChanged();
			}
		}
		
		public SaturationStageOperationParameters ()
		{
		}
		
		public override System.Xml.XmlNode SerializeToXML (System.Xml.XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Saturation")).Value = mSaturation.ToString(nfi);
			return xn;
		}
		
		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			double res = 0;
			if (node.Attributes["Saturation"] != null)
			{
				if (double.TryParse(node.Attributes["Saturation"].Value, NumberStyles.Float, nfi, out res))
				{
					mSaturation = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Saturation value");
			}
			OnChanged();
		}

		public override Type GetSOType ()
		{
			return typeof(SaturationStageOperation);
		}
		
	}
}

