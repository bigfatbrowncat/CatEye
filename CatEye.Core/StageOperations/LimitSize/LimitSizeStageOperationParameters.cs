using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("LimitSizeStageOperation")]
	public class LimitSizeStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		private double mNewWidth = 1920;
		private double mNewHeight = 1024;
		private bool mLimitWidth = false;
		private bool mLimitHeight = false;

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
		public bool LimitWidth
		{
			get { return mLimitWidth; }
			set
			{
				mLimitWidth = value;
				OnChanged();
			}
		}
		public bool LimitHeight
		{
			get { return mLimitHeight; }
			set
			{
				mLimitHeight = value;
				OnChanged();
			}
		}

		public LimitSizeStageOperationParameters ()
		{
		}
		
		public override System.Xml.XmlNode SerializeToXML (System.Xml.XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("NewWidth")).Value = mNewWidth.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("NewHeight")).Value = mNewHeight.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("LimitWidth")).Value = mLimitWidth.ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("LimitHeight")).Value = mLimitHeight.ToString();
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

			bool bres;
			if (node.Attributes["LimitWidth"] != null)
			{
				if (bool.TryParse(node.Attributes["LimitWidth"].Value, out bres))
				{
					mLimitWidth = bres;
				}
				else
					throw new IncorrectNodeValueException("Can't parse LimitWidth value");
			}
			if (node.Attributes["LimitHeight"] != null)
			{
				if (bool.TryParse(node.Attributes["LimitHeight"].Value, out bres))
				{
					mLimitHeight = bres;
				}
				else
					throw new IncorrectNodeValueException("Can't parse LimitHeight value");
			}
			
			OnChanged();
		}
		public override Type GetSOType ()
		{
			return typeof(LimitSizeStageOperation);
		}
		
	}
}

