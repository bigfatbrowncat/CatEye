using System;
using System.Globalization;
namespace CatEye
{
	
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CropStageOperationParametersWidget : StageOperationParametersWidget
	{
		private double mLeft = 0, mRight = 1, mTop = 0, mBottom = 1;
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		public CropStageOperationParametersWidget ()
		{
			this.Build ();
		}

		public double Left
		{
			get { return mLeft; }
			set 
			{
				if (value >= 0 && value <= 1 && value < mRight)
				{
					mLeft = value;
					left_entry.Text = value.ToString();
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
				if (value >= 0 && value <= 1 && value > mLeft)
				{
					mRight = value;
					right_entry.Text = value.ToString();
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
				if (value >= 0 && value <= 1 && value < mBottom)
				{
					mTop = value;
					top_entry.Text = value.ToString();
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
				if (value >= 0 && value <= 1 && value > mTop)
				{
					mBottom = value;
					bottom_entry.Text = value.ToString();
				}
				else
					throw new IncorrectValueException();
			}
		}

		protected virtual void OnLeftEntryChanged (object sender, System.EventArgs e)
		{
			double res;
			if (double.TryParse(left_entry.Text, NumberStyles.Float, nfi, out res))
			{
				if (res >= 0 && res <= 1 && mRight > res)
				{
					mLeft = res;
					OnUserModified();
				}
			}
		}
		
		protected virtual void OnRightEntryChanged (object sender, System.EventArgs e)
		{
			double res;
			if (double.TryParse(right_entry.Text, NumberStyles.Float, nfi, out res))
			{
				if (res >= 0 && res <= 1 && mLeft < res)
				{
					mRight = res;
					OnUserModified();
				}
			}
		}
		
		protected virtual void OnTopEntryChanged (object sender, System.EventArgs e)
		{
			double res;
			if (double.TryParse(top_entry.Text, NumberStyles.Float, nfi, out res))
			{
				if (res >= 0 && res <= 1 && mBottom > res)
				{
					mTop = res;
					OnUserModified();
				}
			}
		}
		
		protected virtual void OnBottomEntryChanged (object sender, System.EventArgs e)
		{
			double res;
			if (double.TryParse(bottom_entry.Text, NumberStyles.Float, nfi, out res))
			{
				if (res >= 0 && res <= 1 && mTop < res)
				{
					mBottom = res;
					OnUserModified();
				}
			}
		}
		
		
		
	}
}

