using System;
using System.Globalization;
using System.Xml;

namespace CatEye.Core
{
	public struct Color
	{
		private double mR, mG, mB;
		
		public double R	{ get { return mR; } }
		public double G	{ get { return mG; } }
		public double B	{ get { return mB; } }

		public Color (double r, double g, double b)
		{
			mR = r; mG = g; mB = b;
		}
		
		public double CalcBrightness()
		{
			return Math.Sqrt(mR * mR + mG * mG + mB * mB) / Math.Sqrt(3);
		}
		
		public Color ChangeBrightness(double newBrightness)
		{
			double oldBr = CalcBrightness() + 0.0001;
			return new Color(
				mR * newBrightness / oldBr,
				mG * newBrightness / oldBr,
				mB * newBrightness / oldBr);
		}
		
		public Color ApplyDualToning(Tone dark_tone, Tone light_tone, 
		                             double softness, double edge, double maxBrightness)
		{
			// Calculating relative brightness
			double brightness_before = CalcBrightness() + 0.0001;
			double rel_bright = brightness_before / maxBrightness;
			
			// Calculating new color
			
			double K = Math.Atan2(softness * rel_bright, edge * edge - rel_bright * rel_bright) / Math.Atan2(softness, edge * edge - 1);
	
			double R1 = dark_tone.R * mR;
			double G1 = dark_tone.G * mG;
			double B1 = dark_tone.B * mB;
			
			double R2 = light_tone.R * mR;
			double G2 = light_tone.G * mG;
			double B2 = light_tone.B * mB;

			return new Color((R2 - R1) * K + R1,
			                 (G2 - G1) * K + G1,
			                 (B2 - B1) * K + B1).ChangeBrightness(brightness_before);
		}		
	}
	
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
		
		public static double Distance(Tone t1, Tone t2)
		{
			return Math.Sqrt(
				(t1.mR - t2.mR) * (t1.mR - t2.mR) +
				(t1.mG - t2.mG) * (t1.mG - t2.mG) +
				(t1.mB - t2.mB) * (t1.mB - t2.mB));
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

