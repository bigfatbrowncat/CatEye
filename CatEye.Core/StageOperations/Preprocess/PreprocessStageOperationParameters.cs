using System;
using System.Globalization;
using System.Xml;

namespace CatEye.Core
{
	[StageOperationID("PreprocessStageOperation")]
	public class PreprocessStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mHighlightsCut = 0, mSoftness = 0.1;
		
		public double HighlightsCut
		{
			get { return mHighlightsCut; }
			set 
			{
				mHighlightsCut = value;
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
			xn.Attributes.Append(xdoc.CreateAttribute("HighlightsCut")).Value = mHighlightsCut.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("Softness")).Value = mSoftness.ToString(nfi);
			return xn;
		}

		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			double res = 0;
			if (node.Attributes["HighlightsCut"] != null)
			{
				if (double.TryParse(node.Attributes["HighlightsCut"].Value, NumberStyles.Float, nfi, out res))
				{
					mHighlightsCut = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse HighlightsCut value");
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
				
		
		public PreprocessStageOperationParameters ()
		{
		}
		
		public override Type GetSOType ()
		{
			return typeof(PreprocessStageOperation);
		}
	}
}

