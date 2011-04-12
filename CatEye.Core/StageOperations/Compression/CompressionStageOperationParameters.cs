using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("CompressionStageOperation")]
	public class CompressionStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mPower = 0.7, mBloha = 0.01;

		public double Power
		{
			get { return mPower; }
			set 
			{
				mPower = value;
				OnChanged();
			}
		}
		public double Bloha
		{
			get { return mBloha; }
			set 
			{
				mBloha = value;
				OnChanged();
			}
		}
		
		public override XmlNode SerializeToXML (XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Power")).Value = mPower.ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("Bloha")).Value = mBloha.ToString();
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
			if (node.Attributes["Bloha"] != null)
			{
				if (double.TryParse(node.Attributes["Bloha"].Value, NumberStyles.Float, nfi, out res))
				{
					mBloha = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Brightness value");
			}
			OnChanged();
		}
		
		public CompressionStageOperationParameters ()
		{
		}
	}
}

