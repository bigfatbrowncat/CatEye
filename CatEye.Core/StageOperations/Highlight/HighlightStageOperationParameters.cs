using System;
using System.Globalization;
using System.Xml;

namespace CatEye.Core
{
	[StageOperationID("HighlightStageOperation")]
	public class HighlightStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mCut = 0, mSoftness = 0.1;
		
		public double Cut
		{
			get { return mCut; }
			set 
			{
				mCut = value;
				OnChanged();
			}
		}
		public double Softness
		{
			get { return mSoftness; }
			set 
			{
				mSoftness = value;
				OnChanged();
			}
		}
		
		public override XmlNode SerializeToXML (XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Cut")).Value = mCut.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("Softness")).Value = mSoftness.ToString(nfi);
			return xn;
		}

		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			double res = 0;
			if (node.Attributes["Cut"] != null)
			{
				if (double.TryParse(node.Attributes["Cut"].Value, NumberStyles.Float, nfi, out res))
				{
					mCut = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Cut value");
			}
			if (node.Attributes["Softness"] != null)
			{
				if (double.TryParse(node.Attributes["Softness"].Value, NumberStyles.Float, nfi, out res))
				{
					mSoftness = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Softness value");
			}
			OnChanged();
		}
				
		
		public HighlightStageOperationParameters ()
		{
		}
		
		public override Type GetSOType ()
		{
			return typeof(HighlightStageOperation);
		}
	}
}

