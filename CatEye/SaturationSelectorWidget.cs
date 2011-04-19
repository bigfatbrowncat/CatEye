using System;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public class SaturationSelectorWidget : Gtk.HScale
	{
		private int mLeftMargin = 4, mRightMargin = 4, 
					mTopMargin = 7, mBottomMargin = 7,
					mTopSliderMargin = 4, mBottomSliderMargin = 4,
					mLeftSliderMargin = 2, mRightSliderMargin = 2;
		private bool mSliderActive = false;
		
		private int mSliderLength = 24;
		
		public SaturationSelectorWidget(Gtk.Adjustment adj) : base(adj) {}
		
		protected override bool OnExposeEvent (Gdk.EventExpose ev)
		{
			//base.OnExposeEvent (ev);
			
			
			
			int w, h, x, y;
			w = Allocation.Width;
			h = Allocation.Height;
			x = Allocation.Left;
			y = Allocation.Top;

			// Recalculating margins
			int trough_border = (int)this.StyleGetProperty("trough-border");
			mLeftMargin = trough_border;
			mRightMargin = trough_border;
			
			int trough_height = 11;
			int slider_width = (int)this.StyleGetProperty("slider-width");
			mSliderLength = (int)this.StyleGetProperty("slider-length");
			
			mTopMargin = h / 2 - trough_height / 2;
			mBottomMargin = mTopMargin;
			
			mTopSliderMargin = h / 2 - slider_width / 2;
			mBottomSliderMargin = mTopSliderMargin;
			
			// Painting focus rect
			if (this.IsFocus)
			{
				Gtk.Style.PaintFocus(this.Style, GdkWindow, Gtk.StateType.Active, 
					new Gdk.Rectangle(x, y, w, h), this, "hscale", x, y, w, h);
			}
			
			// Painting box
			Gtk.Style.PaintBox(this.Style, GdkWindow, Gtk.StateType.Insensitive, Gtk.ShadowType.In,
				new Gdk.Rectangle(x + mLeftMargin, y + mTopMargin, w - mLeftMargin - mRightMargin, h - mTopMargin - mBottomMargin), 
				this, "button", 
					    		  x + mLeftMargin, y + mTopMargin, w - mLeftMargin - mRightMargin, h - mTopMargin - mBottomMargin);
			
			// Painting gradient
			Gdk.GC gc = new Gdk.GC(GdkWindow);
			int xmin = mLeftMargin + 2;
			int xmax = w - mRightMargin - 2;
			int ymin = mTopMargin + 2;
			int ymax = h - mBottomMargin - 2;
			
			for (int i = 0; i < xmax - xmin; i+=1)
			{
				double xx = (double)i / (xmax - xmin);
				for (int j = 0; j < ymax - ymin; j+=1)
				{
					double yy = (double)j / (ymax - ymin);
					
					double r = (xx) * (255 * (1 - yy)) + (1 - xx) * 192;
					double g = (xx) * (128) +            (1 - xx) * 192;
					double b = (xx) * (255 * yy) +       (1 - xx) * 192;
					
					gc.RgbFgColor = new Gdk.Color((byte)r, (byte)g, (byte)b);
					GdkWindow.DrawPoint(gc, x + i + xmin, y + j + ymin);
				}
			}
			
			// Painting slider
			int left_slider_pos = mSliderLength / 2 + mLeftSliderMargin;
			int right_slider_pos = w - mRightSliderMargin - mSliderLength / 2;
			
			int slider_pos = (int)(left_slider_pos + (right_slider_pos - left_slider_pos) *
				(Adjustment.Value - Adjustment.Lower) / (this.Adjustment.Upper - Adjustment.Lower));
			
			Gtk.StateType slider_state = Gtk.StateType.Normal;
			
			// Testing if sensitive
			Gtk.Widget wdg = this;
			bool sens = true;
			do 
			{
				if (!wdg.Sensitive)
				{
					sens = false;
					break;
				}
				wdg = wdg.Parent;
			} while (wdg != null);
			
			if (sens)
			{
				if (mSliderActive) slider_state = Gtk.StateType.Prelight;
			}
			else
			{
				slider_state = Gtk.StateType.Insensitive;
			}
			
			Gtk.Style.PaintSlider(this.Style, GdkWindow, slider_state, Gtk.ShadowType.Out,
				new Gdk.Rectangle(x + slider_pos - mSliderLength / 2, y + mTopSliderMargin, mSliderLength, h - mTopSliderMargin - mBottomSliderMargin), this, "hscale", 
				                  x + slider_pos - mSliderLength / 2, y + mTopSliderMargin, mSliderLength, h - mTopSliderMargin - mBottomSliderMargin, 
				Gtk.Orientation.Horizontal);
	
			//gc.Dispose();
			
			return true;
		}
		
		protected override bool OnLeaveNotifyEvent (Gdk.EventCrossing evnt)
		{
			base.OnLeaveNotifyEvent(evnt);
			if (!(evnt.Mode == Gdk.CrossingMode.Grab))
				mSliderActive = false;
			QueueDraw();
			
			return true;			
		}
		
		
		
		protected override bool OnMotionNotifyEvent (Gdk.EventMotion evnt)
		{
			base.OnMotionNotifyEvent (evnt);
			
			int w, h, x, y;
			w = Allocation.Width;
			h = Allocation.Height;
			x = Allocation.Left;
			y = Allocation.Top;
			
			
			int left_slider_pos = mSliderLength / 2 + mLeftSliderMargin;
			int right_slider_pos = w - mRightSliderMargin - mSliderLength / 2;
			
			int slider_pos = (int)(left_slider_pos + (right_slider_pos - left_slider_pos) *
				(Adjustment.Value - Adjustment.Lower) / (this.Adjustment.Upper - Adjustment.Lower));
			
			if (evnt.X > slider_pos - mSliderLength / 2 && evnt.X < slider_pos + mSliderLength / 2)
			{
				mSliderActive = true;
				QueueDraw();
			}
			else
			{
				mSliderActive = false;
				QueueDraw();
			}
			return true;
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			// Insert layout code here.
		}

		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			// Calculate desired size here.
			requisition.Height = 5;//mSingleColorRowHeight * 3 + mSelectorDiameter * 2;
			requisition.Width = 30;
		}
	}
}

