using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("BlackPointStageOperation")]
	public class BlackPointStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mCut = 0.5, mBlurDarkLevel = 0.2;
		
		public double Cut
		{
			get { return mCut; }
			set 
			{
				mCut = value;
				OnChanged();
			}
		}
		public double BlurDarkLevel
		{
			get { return mBlurDarkLevel; }
			set 
			{
				mBlurDarkLevel = value;
				OnChanged();
			}
		}
		
		public BlackPointStageOperationParameters ()
		{
		}
		
		public override XmlNode SerializeToXML (XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Cut")).Value = mCut.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("BlurDarkLevel")).Value = mBlurDarkLevel.ToString(nfi);
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
			if (node.Attributes["BlurDarkLevel"] != null)
			{
				if (double.TryParse(node.Attributes["BlurDarkLevel"].Value, NumberStyles.Float, nfi, out res))
				{
					mBlurDarkLevel = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse BlurDarkLevel value");
			}
			OnChanged();
		}
		
		public override Type GetSOType ()
		{
			return typeof(BlackPointStageOperation);
		}

		public override void CopyDataTo (StageOperationParameters target)
		{
			base.CopyDataTo (target);
			BlackPointStageOperationParameters t = (BlackPointStageOperationParameters)target;
			t.mCut = mCut;
			t.mBlurDarkLevel = mBlurDarkLevel;
			t.OnChanged();
		}
		
		public override object Clone ()
		{
			BlackPointStageOperationParameters target = new BlackPointStageOperationParameters();
			CopyDataTo(target);
			return target;
		}	
	}
}