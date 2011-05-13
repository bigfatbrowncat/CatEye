using System;
using CatEye.Core;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public class ToneSelectorWidget : Gtk.DrawingArea
	{
		int margin = 6;
		double mAlpha = 0.5;
		bool mouse_down = false;
		
		private Tone mSelectedTone;

		public double Alpha {
			get {
				return this.mAlpha;
			}
			set {
				mAlpha = value;
				QueueDraw();
			}
		}		
		/// <summary>
		/// Occurs when a tone selected by user.
		/// </summary>
		public event EventHandler<EventArgs> ToneSelected;
		/// <summary>
		/// Occurs when SelectedTone value is changed.
		/// </summary>
		public event EventHandler<EventArgs> SelectedToneChanged;
		
		public Tone SelectedTone 
		{
			get { return mSelectedTone; }
			set
			{
				mSelectedTone = value;
				OnSelectedToneChanged();
			}
		}
		
		protected virtual void OnToneSelected()
		{
			if (ToneSelected != null)
				ToneSelected(this, EventArgs.Empty);
		}
		protected virtual void OnSelectedToneChanged()
		{
			if (SelectedToneChanged != null)
				SelectedToneChanged(this, EventArgs.Empty);
		}
		
		public ToneSelectorWidget (): base()
		{
			// Insert initialization code here.
		}

		
		/// <returns>
		/// The red channel value
		/// </returns>
		/// <param name='x'>
		/// X coord from 0 to 1
		/// </param>
		public Tone XY_to_Tone(double x, double y)
		{
			// This is a norm coefficient which is needed to
			// place the gray point into the center in any mAlpha value
			double p = Math.Pow(0.5, mAlpha) / Math.Tan(Math.PI / 4);

			double r = Math.Pow(x, mAlpha);
			double b = Math.Pow(1 - x, mAlpha);
			double g = p * Math.Pow(Math.Tan(y * (Math.PI / 2 - 0.0001)), mAlpha);  //Math.Pow(1.6 * y, 2);
			
			return new Tone(r, g, b);
		}
		
		public double Tone_to_X(Tone t)
		{
			double pown = 1.0 / (Math.Pow(t.B, 1.0 / mAlpha) + Math.Pow(t.R, 1.0 / mAlpha));
			return Math.Pow(t.R, 1.0 / mAlpha) * pown;
		}
		public double Tone_to_Y(Tone t)
		{
			// This is a norm coefficient which is needed to
			// place the gray point into the center in any mAlpha value
			double p = Math.Pow(0.5 / Math.Tan(Math.PI / 4), mAlpha);

			double pown = 1.0 / (Math.Pow(t.B, 1.0 / mAlpha) + Math.Pow(t.R, 1.0 / mAlpha));
			double n = Math.Pow(pown, mAlpha);
			return Math.Atan(Math.Pow(t.G * n / p, 1.0 / mAlpha)) / (Math.PI / 2 - 0.0001);
		}
		
		unsafe void DrawColors(Gdk.Pixbuf buf)
		{
			int h = buf.Height;
			int w = buf.Width;
			int stride = buf.Rowstride;
			
			int chan = buf.NChannels;
			
			byte *cur_row = (byte *)buf.Pixels;
			for (int j = 0; j < h; j++)
			{
				byte *cur_pixel = cur_row;
				for (int i = 0; i < w; i++)
				{
					
					Tone res = XY_to_Tone((double)i / w, (double)j / h);
					
					cur_pixel[0] = (byte)(res.R / Math.Sqrt(3) * 255);
					cur_pixel[1] = (byte)(res.G / Math.Sqrt(3) * 255);
					cur_pixel[2] = (byte)(res.B / Math.Sqrt(3) * 255);
					//cur_pixel[3] = 255;	// No alpha here
					
					cur_pixel += chan;
				}
				cur_row += stride;
			}
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose ev)
		{
			base.OnExposeEvent (ev);
			
			int W = Allocation.Width, H = Allocation.Height;
			int X = Allocation.Left, Y = Allocation.Top;
			Gdk.Pixbuf pb = null;
			Gdk.GC gc = null;
			try
			{
				// Creating PixBuf to draw a color matrix
				if (GdkWindow == null || W < 1 || H < 1) return true;
				
				pb = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 8, W - margin * 2, H - margin * 2);
				//pb = Gdk.Pixbuf.FromDrawable(Parent.GdkWindow, Gdk.Rgb.Colormap, 0, 0, 0, 0, W - margin * 2, H - margin * 2);
				
				// Drawing color matrix to back pixbuf
				DrawColors(pb);
				
				// Drawing frame to widget
				Gtk.Style.PaintBox(this.Style, GdkWindow, Gtk.StateType.Normal, Gtk.ShadowType.In, 
					new Gdk.Rectangle(0, 0, W, H),
					this, null, margin - 2, margin - 2, W - margin * 2 + 4, H - margin * 2 + 4);
	
				// Creating Graphics Context
				gc = new Gdk.GC(GdkWindow);
				
				// Drawing color matrix backbuffer
				GdkWindow.DrawPixbuf(gc, pb, 0, 0, margin, margin, pb.Width, pb.Height, Gdk.RgbDither.Normal, 0, 0);
				
				int sel_x = (int)(Tone_to_X(mSelectedTone) * pb.Width) + margin;
				int sel_y = (int)(Tone_to_Y(mSelectedTone) * pb.Height) + margin;
				//Console.WriteLine(Tone_to_X(mSelectedTone) +", " + Tone_to_Y(mSelectedTone));
				
				gc.SetLineAttributes(2, Gdk.LineStyle.Solid, Gdk.CapStyle.Round,Gdk.JoinStyle.Round);
				GdkWindow.DrawRectangle(gc, false,
					new Gdk.Rectangle(sel_x - 4, sel_y - 4, 8, 8));
			}
			finally
			{
				if (gc != null) gc.Dispose();
				if (pb != null) pb.Dispose();
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
			requisition.Height = 150;
			requisition.Width = 150;
		}
		
		private void SelectNewTone(double winX, double winY)
		{
			double x = (double)(winX - margin) / (Allocation.Width - 2*margin);
			double y = (double)(winY - margin) / (Allocation.Height - 2*margin);
		
			if (x < 0) x = 0;
			if (y < 0) y = 0;
			if (x > 1) x = 1;
			if (y > 1) y = 1;
			
			SelectedTone = XY_to_Tone(x, y);
			OnToneSelected();
				
			QueueDraw();
		}
		
		protected override bool OnMotionNotifyEvent (Gdk.EventMotion evnt)
		{
			if (mouse_down)
			{
				SelectNewTone(evnt.X, evnt.Y);
			}
			return base.OnMotionNotifyEvent (evnt);
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton ev)
		{
			mouse_down = true;
			SelectNewTone(ev.X, ev.Y);
			return base.OnButtonPressEvent (ev);
		}
		
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			mouse_down = false;
			return base.OnButtonReleaseEvent (evnt);
		}
		
	}

}

