using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("HardCutStageOperation")]
	public class HardCutStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mBlack = 0.05;
		private double mWhite = 0.95;
		
		public double Black
		{
			get { return mBlack; }
			set 
			{
				mBlack = value;
				OnChanged();
			}
		}

		public double White
		{
			get { return mWhite; }
			set 
			{
				mWhite = value;
				OnChanged();
			}
		}
		
		public HardCutStageOperationParameters ()
		{
		}
		
		public override XmlNode SerializeToXML (XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Black")).Value = mBlack.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("White")).Value = mWhite.ToString(nfi);
			return xn;
		}

		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			double res = 0;
			if (node.Attributes["Black"] != null)
			{
				if (double.TryParse(node.Attributes["Black"].Value, NumberStyles.Float, nfi, out res))
				{
					mBlack = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Black value");
			}
			if (node.Attributes["White"] != null)
			{
				if (double.TryParse(node.Attributes["White"].Value, NumberStyles.Float, nfi, out res))
				{
					mWhite = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse White value");
			}

			OnChanged();
		}
	}
}