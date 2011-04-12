using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("CropStageOperation")]
	public class CropStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private double mLeft = 0, mRight = 1, mTop = 0, mBottom = 1, mAspectRatio = 3.0/2;
		private bool mLockAspectRatio = true;
		
		public CropStageOperationParameters ()
		{
		}
		
		public double Left
		{
			get { return mLeft; }
			set 
			{
				if (value >= 0 && value <= 1 && value <= mRight)
				{
					mLeft = value;
					OnChanged();
				}
				else
					throw new IncorrectValueException();
			}
		}

		public double Right
		{
			get { return mRight; }
			set 
			{
				if (value >= 0 && value <= 1 && value >= mLeft)
				{
					mRight = value;
					OnChanged();
				}
				else
					throw new IncorrectValueException();
			}
		}

		public double Top
		{
			get { return mTop; }
			set 
			{
				if (value >= 0 && value <= 1 && value <= mBottom)
				{
					mTop = value;
					OnChanged();
				}
				else
					throw new IncorrectValueException();
			}
		}
		
		public double Bottom
		{
			get { return mBottom; }
			set 
			{
				if (value >= 0 && value <= 1 && value >= mTop)
				{
					mBottom = value;
					OnChanged();
				}
				else
					throw new IncorrectValueException();
			}
		}

		public double AspectRatio
		{
			get { return mAspectRatio; }
			set 
			{
				if (value > 0)
				{
					mAspectRatio = value;
					OnChanged();
				}
				else
					throw new IncorrectValueException();
			}
		}
		
		public bool LockAspectRatio
		{
			get { return mLockAspectRatio; }
			set
			{
				mLockAspectRatio = value;
				OnChanged();
			}
		}
		
		public void SetCrop(double left, double top, double right, double bottom)
		{
			if (left >= 0 && left <= right && right <= 1 &&
			    top >= 0 && top <= bottom && bottom <= 1)
			{
				mLeft = left; mRight = right; mTop = top; mBottom = bottom;
				OnChanged();
			}
		}

		public override XmlNode SerializeToXML (XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Left")).Value = Left.ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("Right")).Value = Right.ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("Top")).Value = Top.ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("Bottom")).Value = Bottom.ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("AspectRatio")).Value = AspectRatio.ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("LockAspectRatio")).Value = LockAspectRatio.ToString();
			return xn;
		}

		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			double res = 0;
			bool bres = false;
			if (node.Attributes["Left"] != null)
			{
				if (double.TryParse(node.Attributes["Left"].Value, NumberStyles.Float, nfi, out res))
				{
					mLeft = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Left value");
			}
			if (node.Attributes["Right"] != null)
			{
				if (double.TryParse(node.Attributes["Right"].Value, NumberStyles.Float, nfi, out res))
				{
					mRight = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Right value");
			}
			if (node.Attributes["Top"] != null)
			{
				if (double.TryParse(node.Attributes["Top"].Value, NumberStyles.Float, nfi, out res))
				{
					mTop = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Top value");
			}
			if (node.Attributes["Bottom"] != null)
			{
				if (double.TryParse(node.Attributes["Bottom"].Value, NumberStyles.Float, nfi, out res))
				{
					mBottom = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Bottom value");
			}
			if (node.Attributes["AspectRatio"] != null)
			{
				if (double.TryParse(node.Attributes["AspectRatio"].Value, NumberStyles.Float, nfi, out res))
				{
					mAspectRatio = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse AspectRatio value");
			}
			if (node.Attributes["LockAspectRatio"] != null)
			{
				if (bool.TryParse(node.Attributes["LockAspectRatio"].Value, out bres))
				{
					mLockAspectRatio = bres;
				}
				else
					throw new IncorrectNodeValueException("Can't parse LockAspectRatio value");
			}
			OnChanged();
		}
		
	}
}

