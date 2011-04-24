using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("ToneStageOperation")]
	public class ToneStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mRedPart = 1, mGreenPart = 1, mBluePart = 1;
		
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
		
		public ToneStageOperationParameters ()
		{
		}
		
		public override System.Xml.XmlNode SerializeToXML (System.Xml.XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("RedPart")).Value = mRedPart.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("GreenPart")).Value = mGreenPart.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("BluePart")).Value = mBluePart.ToString(nfi);
			return xn;
		}
		
		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			double res = 0;
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
					Console.Write("blue=" + res);
					mBluePart = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse BluePart value");
			}
			OnChanged();
		}
	}
}

