using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("BlackPointStageOperation")]
	public class BlackPointStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mBlack = 0.05;
		
		public double Black
		{
			get { return mBlack; }
			set 
			{
				mBlack = value;
				OnChanged();
			}
		}
		
		public BlackPointStageOperationParameters ()
		{
		}
		
		public override XmlNode SerializeToXML (XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Black")).Value = mBlack.ToString(nfi);
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
			t.mBlack = mBlack;
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