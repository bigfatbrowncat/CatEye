using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("ScaleStageOperation")]
	public class ScaleStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		private double mNewWidth = 1920;
		private double mNewHeight = 1080;
		
		public double NewWidth
		{
			get { return mNewWidth; }
			set
			{
				mNewWidth = value;
				OnChanged();
			}
		}
		public double NewHeight
		{
			get { return mNewHeight; }
			set
			{
				mNewHeight = value;
				OnChanged();
			}
		}
		
		public ScaleStageOperationParameters ()
		{
		}
		
		public override System.Xml.XmlNode SerializeToXML (System.Xml.XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("NewWidth")).Value = mNewWidth.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("NewHeight")).Value = mNewHeight.ToString(nfi);
			return xn;
		}
		
		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			double res = 0;
			if (node.Attributes["NewWidth"] != null)
			{
				if (double.TryParse(node.Attributes["NewWidth"].Value, NumberStyles.Float, nfi, out res))
				{
					mNewWidth = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse NewWidth value");
			}
			if (node.Attributes["NewHeight"] != null)
			{
				if (double.TryParse(node.Attributes["NewHeight"].Value, NumberStyles.Float, nfi, out res))
				{
					mNewHeight = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse NewHeight value");
			}
			
			OnChanged();
		}
	}
}

