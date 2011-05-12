using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("BrightnessStageOperation")]
	public class BrightnessStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mBrightness = 1;
		private bool mNormalize = false;
		private double mMedian = 0;			// Don't save it.
		
		public double Brightness
		{
			get { return mBrightness; }
			set 
			{
				mBrightness = value;
				OnChanged();
			}
		}

		public double Median
		{
			get { return mMedian; }
			internal set
			{
				mMedian = value;
				OnChanged();
			}
		}
		
		public bool Normalize
		{
			get { return mNormalize; }
			set 
			{ 
				mNormalize = value; 
				OnChanged();
			}
		}
		
		public BrightnessStageOperationParameters ()
		{
		}
		
		public override XmlNode SerializeToXML (XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Brightness")).Value = Brightness.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("Normalize")).Value = Normalize.ToString();
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
			bool bres;
			if (node.Attributes["Normalize"] != null)
			{
				if (bool.TryParse(node.Attributes["Normalize"].Value, out bres))
				{
					mNormalize = bres;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Normalize value");
			}
			OnChanged();
		}
	}
}

