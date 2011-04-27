using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("UltraSharpStageOperation")]
	public class UltraSharpStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mPower = 0.3, mRadius = 0.1, mBase = 2;
		private int mPoints = 100;
		
		public double Power
		{
			get { return mPower; }
			set 
			{
				mPower = value;
				OnChanged();
			}
		}

		public double Radius
		{
			get { return mRadius; }
			set 
			{
				mRadius = value;
				OnChanged();
			}
		}
		public double Base
		{
			get { return mBase; }
			set 
			{
				mBase = value;
				OnChanged();
			}
		}
		public int Points
		{
			get { return mPoints; }
			set 
			{
				mPoints = value;
				OnChanged();
			}
		}
		
		public override XmlNode SerializeToXML (XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Power")).Value = mPower.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("Radius")).Value = mRadius.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("Base")).Value = mBase.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("Points")).Value = mPoints.ToString(nfi);
			return xn;
		}

		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			double res = 0;
			int ires = 0;
			if (node.Attributes["Power"] != null)
			{
				if (double.TryParse(node.Attributes["Power"].Value, NumberStyles.Float, nfi, out res))
				{
					mPower = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Power value");
			}
			if (node.Attributes["Radius"] != null)
			{
				if (double.TryParse(node.Attributes["Radius"].Value, NumberStyles.Float, nfi, out res))
				{
					mRadius = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Radius value");
			}
			if (node.Attributes["Base"] != null)
			{
				if (double.TryParse(node.Attributes["Base"].Value, NumberStyles.Float, nfi, out res))
				{
					mBase = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Delta0 value");
			}
			if (node.Attributes["Points"] != null)
			{
				if (int.TryParse(node.Attributes["Points"].Value, out ires))
				{
					mPoints = ires;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Points value");
			}
			OnChanged();
		}
		
		public UltraSharpStageOperationParameters ()
		{
		}
	}
}

