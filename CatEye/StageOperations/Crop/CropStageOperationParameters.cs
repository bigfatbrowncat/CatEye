using System;

namespace CatEye
{
	public class CropStageOperationParameters : StageOperationParameters
	{
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
	}
}

