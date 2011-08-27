using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("CompressionStageOperation")]
	public class CompressionStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mCurve = 0.5;

		public double Curve
		{
			get { return mCurve; }
			set 
			{
				mCurve = value;
				OnChanged();
			}
		}
		
		public override XmlNode SerializeToXML (XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Curve")).Value = mCurve.ToString(nfi);
			return xn;
		}

		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			double res = 0;
			if (node.Attributes["Curve"] != null)
			{
				if (double.TryParse(node.Attributes["Curve"].Value, NumberStyles.Float, nfi, out res))
				{
					mCurve = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Curve value");
			}
			OnChanged();
		}
		
		public CompressionStageOperationParameters ()
		{
		}

		public override Type GetSOType ()
		{
			return typeof(CompressionStageOperation);
		}
		
		public override void CopyDataTo (StageOperationParameters target)
		{
			base.CopyDataTo (target);
			CompressionStageOperationParameters t = (CompressionStageOperationParameters)target;
			t.mCurve = mCurve;
			t.OnChanged();
		}
		
		public override object Clone ()
		{
			CompressionStageOperationParameters target = new CompressionStageOperationParameters();
			CopyDataTo(target);
			return target;
		}		
	}
}

