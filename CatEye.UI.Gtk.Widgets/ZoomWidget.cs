using System;
using Gtk;

namespace CatEye.UI.Gtk.Widgets
{
	class WarningTooltip : Window
	{
		private Label mLabel;
		private uint mTimerID = 0;
		private Widget mBaseWidget;
		public WarningTooltip(Widget baseWidget) : base(WindowType.Popup)
		{
			mBaseWidget = baseWidget;
			
			mLabel = new Label();
			mLabel.UseMarkup = true;
			Add(mLabel);
			BorderWidth = 4;
			mLabel.Show();
			
			SizeAllocated += HandleSizeAllocated;

			using (Style stl = Rc.GetStyleByPaths(this.Settings, "gtk-tooltip*", "gtk-tooltip*", GLib.GType.None))
			{
				this.Style = stl;
				mLabel.Style = stl;
			}
		}

		void HandleSizeAllocated (object o, SizeAllocatedArgs args)
		{
			if (mBaseWidget != null && mBaseWidget.GdkWindow != null)
			{
				int x0, y0, x1, y1;
				mBaseWidget.GdkWindow.GetOrigin(out x0, out y0);
				mBaseWidget.TranslateCoordinates(mBaseWidget.Toplevel, 0, 0, out x1, out y1);
				Move(x0 + x1, y0 + y1 - args.Allocation.Height - 10);
			}
		}

		public void ShowWarning(double maxValue, uint delay)
		{
			mLabel.Markup = 
				String.Format("<b>Sorry!</b>\nYou can't zoom the image more than at <b>{0}%</b> of its original size cause it has been downscaled", 
				(maxValue * 100).ToString("0"));
			mLabel.LineWrap = true;
			mLabel.WidthRequest = mBaseWidget.Allocation.Width - 10;
			
			SetSizeRequest(mBaseWidget.Allocation.Width, -1);
			
			if (mTimerID != 0)
			{
				GLib.Source.Remove(mTimerID);
			}
			mTimerID = GLib.Timeout.Add(delay, delegate {
				Hide();
				return false;
			});
			
			Show();
		}
	}
	
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ZoomWidget : Bin
	{
		private double[] mGoodValuesBase = new double[] { 0.125 / 2, 0.125, 0.25, 1.0 / 3, 0.5, 0.75, 1 };
		private double[] mGoodValues = new double[] { 0.125 / 2, 0.125, 0.25, 1.0 / 3, 0.5, 0.75, 1 };
		
		private double mValue = 1;
		private double mMaxValue = 1;
		private double mMinValue = 0.05;
		private bool setting_divider = false;
		private WarningTooltip mWarningTooltip;
		private bool mChangingSelf = false;
		
		public event EventHandler<EventArgs> ValueChanged;
		
		private bool ValueIsNear(double val)
		{
			return (Value > val - 0.01) && (Value < val + 0.01);
		}
			

		private void UpdateDividerView()
		{
			if (!setting_divider)
			{
				setting_divider = true;
				if (mValue >= 0.99 || Math.Abs(mValue * 100 - Math.Round(mValue * 100)) < 0.01)
				{
					zoom_label.Text = (100.0 * mValue).ToString("0") + "%";
				}
				else
				{
					zoom_label.Text = (100.0 * mValue).ToString("0.0") + "%";
				}
				mChangingSelf = true;
				zoom_hscale.Value = Math.Log(1 + (1 + Math.Sqrt(5)) * (double)mValue, 2 + Math.Sqrt(5));
				mChangingSelf = false;
				setting_divider = false;
			}
		}
		
		public double Value
		{
			get { return mValue; }
			set
			{
				double oldValue = mValue;
				mValue = value;

				if (value > mMaxValue) 
					mValue = mMaxValue;
				
				if (value < mMinValue)
					mValue = mMinValue;
				
				
				if (mValue < mGoodValues[0]) mValue = mGoodValues[0];
				if (mValue > mGoodValues[mGoodValues.Length - 1]) 
					mValue = mGoodValues[mGoodValues.Length - 1];
				
				if (mValue != oldValue)
				{
					if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
				}
				
				UpdateDividerView();
			}
		}
		
		public double MaxValue
		{
			get { return mMaxValue; }
			set { 
				mMaxValue = value;
				for (int i = 0; i < mGoodValues.Length; i++)
					mGoodValues[i] = mGoodValuesBase[i] * mMaxValue;
				
				if (Value > mMaxValue) Value = mMaxValue;
			}
		}

		public ZoomWidget ()
		{
			this.Build ();
			mWarningTooltip = new WarningTooltip(this);
		}
		
		protected void OnZoomHscaleValueChanged (object sender, System.EventArgs e)
		{
			double newValue = (Math.Pow(2 + Math.Sqrt(5), zoom_hscale.Value) - 1) / (1 + Math.Sqrt(5));
			if (!mChangingSelf && newValue > mMaxValue)
			{
				mWarningTooltip.ShowWarning(mMaxValue, 5000);
			}
			
			Value = newValue;
			
			UpdateDividerView();
		}

		protected void OnZoom100ButtonClicked (object sender, System.EventArgs e)
		{
			Value = mMaxValue;
		}

		protected void OnZoomInButtonClicked (object sender, System.EventArgs e)
		{
			for (int i = 0; i < mGoodValues.Length - 1; i++)
			{
				if ((ValueIsNear(mGoodValues[i]) || Value > mGoodValues[i]) && Value < mGoodValues[i + 1])
				{
					Value = mGoodValues[i + 1];
					return;
				}
			}
			if (ValueIsNear(mMaxValue))
			{
				mWarningTooltip.ShowWarning(mMaxValue, 5000);
			}
		}

		protected void OnZoomOutButtonClicked (object sender, System.EventArgs e)
		{
			for (int i = 0; i < mGoodValues.Length - 1; i++)
			{
				if (Value > mGoodValues[i] && (ValueIsNear(mGoodValues[i + 1]) || Value < mGoodValues[i + 1]))
				{
					Value = mGoodValues[i];
					break;
				}
			}
		}
	}
}

