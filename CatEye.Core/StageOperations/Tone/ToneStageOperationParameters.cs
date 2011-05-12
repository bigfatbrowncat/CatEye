using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("ToneStageOperation")]
	public class ToneStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		private Tone mTone = new Tone(1, 1, 1);
		private double mHighlightsInvariance = 0;
		
		public Tone Tone
		{
			get { return mTone; }
			set
			{
				mTone = value;
				OnChanged();
			}
		}
		
		public double HighlightsInvariance
		{
			get { return mHighlightsInvariance; }
			set
			{
				mHighlightsInvariance = value;
				OnChanged();
			}
		}
		
		public ToneStageOperationParameters ()
		{
		}
		
		public override System.Xml.XmlNode SerializeToXML (System.Xml.XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("HighlightsInvariance")).Value = mHighlightsInvariance.ToString(nfi);
			xn.AppendChild(mTone.SerializeToXML(xdoc));
			return xn;
		}
		
		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			foreach (XmlNode xn in node.ChildNodes)
			{
				if (xn.Name == "Tone") mTone.DeserializeFromXML(xn);
			}

			double res = 0;
			if (node.Attributes["HighlightsInvariance"] != null)
			{
				if (double.TryParse(node.Attributes["HighlightsInvariance"].Value, NumberStyles.Float, nfi, out res))
				{
					mHighlightsInvariance = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse HighlightsInvariance value");
			}
			
			OnChanged();
		}
	}
}

