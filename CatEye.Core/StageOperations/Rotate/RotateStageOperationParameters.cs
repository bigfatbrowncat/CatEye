using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("RotateStageOperation")]
	public class RotateStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		private double mAngle = 45;
		
		public double Angle
		{
			get { return mAngle; }
			set
			{
				mAngle = value;
				OnChanged();
			}
		}
		
		public RotateStageOperationParameters ()
		{
		}
		
		public override System.Xml.XmlNode SerializeToXML (System.Xml.XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Angle")).Value = mAngle.ToString(nfi);
			return xn;
		}
		
		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			double res = 0;
			if (node.Attributes["Angle"] != null)
			{
				if (double.TryParse(node.Attributes["Angle"].Value, NumberStyles.Float, nfi, out res))
				{
					mAngle = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Angle value");
			}

			OnChanged();
		}
	}
}

