using System;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public class SaturationSelectorWidget : Gtk.DrawingArea
	{
		private int mLeftMargin = 5, mRightMargin = 5, mTopMarginPart = 1, mBottomMarginPart = 1;
		private int mSingleColorRowHeight = 3;
		private int mSelectorDiameter = 3;
		
		private double mMaxValue = 2;
		
		public double MaxValue 
		{
			get { return mMaxValue; }
			set { mMaxValue = value; QueueDraw(); }
		}
		
		public SaturationSelectorWidget ()
		{
			// Insert initialization code here.
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton ev)
		{
			// Insert button press handling code here.
			return base.OnButtonPressEvent (ev);
		}

		protected override bool OnExposeEvent (Gdk.EventExpose ev)
		{
			base.OnExposeEvent (ev);

			int w, h, x, y, d;
			GdkWindow.GetGeometry(out x, out y, out w, out h, out d);
			
			Gdk.GC gc = new Gdk.GC(GdkWindow);
			int xmin = mLeftMargin;
			int xmax = w - mRightMargin;
			int y0 = (int)(h * ((double)mTopMarginPart / (mTopMarginPart + mBottomMarginPart)) - mSingleColorRowHeight * 1.5);
			int y1 = y0 + mSingleColorRowHeight;
			int y2 = y1 + mSingleColorRowHeight;
			int y3 = y2 + mSingleColorRowHeight;
			
			
			for (int i = 0; i < xmax - xmin; i++)
			{
				double s = (double)i / (xmax - xmin);
				
				byte p, q;
				bool sens = true;
				Gtk.Widget wd = this;
				do
				{
					if (!wd.Sensitive) { sens = false; break; }
					wd = wd.Parent;
				}
				while (wd.Parent != null);
				
				if (sens)
				{
					q = (byte)(s * 255 + (1 - s) * 128);
					p = (byte)((1 - s) * 128);
				}	
				else
				{
					q = (byte)(s * 192 + (1 - s) * 128);
					p = (byte)(s * 192 + (1 - s) * 128);
				}
				gc.RgbFgColor = new Gdk.Color(q, p, p);
				GdkWindow.DrawLine(gc, i + xmin, y0, i + xmin, y1);
				gc.RgbFgColor = new Gdk.Color(p, q, p);
				GdkWindow.DrawLine(gc, i + xmin, y1, i + xmin, y2);
				gc.RgbFgColor = new Gdk.Color(p, p, q);
				GdkWindow.DrawLine(gc, i + xmin, y2, i + xmin, y3);
			}
		
			int valx = (int)(xmin + (1.0 / mMaxValue) * (xmax - xmin));
			
			gc.RgbFgColor = new Gdk.Color(0, 0, 0);
			gc.SetDashes(0, new sbyte[] { 1, 2 }, 2);
			gc.SetLineAttributes(1, Gdk.LineStyle.OnOffDash, Gdk.CapStyle.Round, Gdk.JoinStyle.Round);
			GdkWindow.DrawLine(gc, 
				valx, 
				y0 - mSelectorDiameter, 
				valx, 
				y3 + mSelectorDiameter
			);
			/*
			gc.RgbFgColor = new Gdk.Color(0, 0, 0);
			gc.SetLineAttributes(2, Gdk.LineStyle.Solid, Gdk.CapStyle.Round, Gdk.JoinStyle.Round);
			GdkWindow.DrawRectangle(gc, false, new Gdk.Rectangle(
				valx - mSelectorDiameter - 1, 
				y0 - mSelectorDiameter, 
				2 * mSelectorDiameter + 1, 
				y3 - y0 + 2 * mSelectorDiameter)
			);
			*/
			gc.Dispose();
			
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
			requisition.Height = mSingleColorRowHeight * 3 + mSelectorDiameter * 2;
			requisition.Width = 10;
		}
	}
}

