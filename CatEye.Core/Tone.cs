using System;
using System.Globalization;
using System.Xml;

namespace CatEye.Core
{
	public class Tone : ICloneable
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		
		private double mR, mG, mB;
		
		public double R	{ get { return mR; } }
		public double G	{ get { return mG; } }
		public double B	{ get { return mB; } }
		
		public Tone (double r, double g, double b)
		{
			double norm = Math.Sqrt((r*r + g*g + b*b) / 3);
			mR = r / norm; 
			mG = g / norm; 
			mB = b / norm;
		}

		public System.Xml.XmlNode SerializeToXML (System.Xml.XmlDocument xdoc)
		{
			XmlNode xn = xdoc.CreateElement("Tone");
			xn.Attributes.Append(xdoc.CreateAttribute("R")).Value = mR.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("G")).Value = mG.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("B")).Value = mB.ToString(nfi);
			return xn;
		}
		
		public void DeserializeFromXML (XmlNode node)
		{
			if (node.Name != "Tone")
				throw new IncorrectNodeException("Node should have name \"Tone\"");
			
			double res = 0;
			if (node.Attributes["R"] != null)
			{
				if (double.TryParse(node.Attributes["R"].Value, NumberStyles.Float, nfi, out res))
				{
					mR = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse R value");
			}
			if (node.Attributes["G"] != null)
			{
				if (double.TryParse(node.Attributes["G"].Value, NumberStyles.Float, nfi, out res))
				{
					mG = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse G value");
			}
			if (node.Attributes["B"] != null)
			{
				if (double.TryParse(node.Attributes["B"].Value, NumberStyles.Float, nfi, out res))
				{
					mB = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse B value");
			}
		}
		
		public object Clone ()
		{
			return new Tone(mR, mG, mB);
		}
	}
}

