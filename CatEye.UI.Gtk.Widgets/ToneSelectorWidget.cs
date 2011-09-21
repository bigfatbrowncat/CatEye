using System;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	public enum ToneSelectorSymbol { Dot, Donut, None }
	
	[System.ComponentModel.ToolboxItem(true)]
	public class ToneSelectorWidget : DrawingArea
	{
		int margin = 9;
		double mAlpha = 0.5;
		bool mouse_down = false;
		uint mouse_button = 0;

		private Gdk.Pixbuf black_dot = null;
		private Gdk.Pixbuf white_dot = null;
		private Gdk.Pixbuf black_donut = null;
		private Gdk.Pixbuf white_donut = null;
		
		private Gdk.Pixbuf savedPalette = null;
		
		private Tone mSelectedDarkTone = new Tone(1, 1, 1);
		private Tone mSelectedLightTone = new Tone(1, 1, 1);
		
		private ToneSelectorSymbol mDarkToneSelectorSymbol = ToneSelectorSymbol.Donut;
		private ToneSelectorSymbol mLightToneSelectorSymbol = ToneSelectorSymbol.Donut;

		public double Alpha {
			get {
				return this.mAlpha;
			}
			set {
				mAlpha = value;
				
				if (savedPalette != null) DrawPalette(savedPalette);
				QueueDraw();
			}
		}		
		/// <summary>
		/// Occurs when a tone selected by user.
		/// </summary>
		public event EventHandler<EventArgs> DarkToneSelected;
		public event EventHandler<EventArgs> LightToneSelected;
		/// <summary>
		/// Occurs when SelectedTone value is changed.
		/// </summary>
		public event EventHandler<EventArgs> SelectedDarkToneChanged;
		public event EventHandler<EventArgs> SelectedLightToneChanged;
		
		public Tone SelectedDarkTone 
		{
			get { return mSelectedDarkTone; }
			set
			{
				mSelectedDarkTone = value;
				OnSelectedDarkToneChanged();
			}
		}
		public Tone SelectedLightTone 
		{
			get { return mSelectedLightTone; }
			set
			{
				mSelectedLightTone = value;
				OnSelectedLightToneChanged();
			}
		}
		
		public ToneSelectorSymbol DarkToneSelectorSymbol
		{
			get { return mDarkToneSelectorSymbol; }
			set
			{
				mDarkToneSelectorSymbol = value;
				QueueDraw();
			}
		}
		public ToneSelectorSymbol LightToneSelectorSymbol
		{
			get { return mLightToneSelectorSymbol; }
			set
			{
				mLightToneSelectorSymbol = value;
				QueueDraw();
			}
		}
		
		protected virtual void OnDarkToneSelected()
		{
			if (DarkToneSelected != null)
				DarkToneSelected(this, EventArgs.Empty);
		}
		protected virtual void OnSelectedDarkToneChanged()
		{
			if (SelectedDarkToneChanged != null)
				SelectedDarkToneChanged(this, EventArgs.Empty);
			QueueDraw();
		}
		protected virtual void OnLightToneSelected()
		{
			if (LightToneSelected != null)
				LightToneSelected(this, EventArgs.Empty);
		}
		protected virtual void OnSelectedLightToneChanged()
		{
			if (SelectedLightToneChanged != null)
				SelectedLightToneChanged(this, EventArgs.Empty);
			QueueDraw();
		}
		
		public ToneSelectorWidget (): base()
		{
			// Insert initialization code here.
			black_dot = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.dot-black.png");
			white_dot = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.dot-white.png");
			black_donut = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.donut-black.png");
			white_donut = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.donut-white.png");
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
		
		unsafe void DrawPalette(Gdk.Pixbuf buf)
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
			//base.OnExposeEvent (ev);
			int W = Allocation.Width, H = Allocation.Height;

			using (Gdk.GC gc = new Gdk.GC(GdkWindow))
			{
				// Creating PixBuf to draw a color matrix
				if (GdkWindow == null || W < 1 || H < 1) return true;
				
				//pb = Gdk.Pixbuf.FromDrawable(Parent.GdkWindow, Gdk.Rgb.Colormap, 0, 0, 0, 0, W - margin * 2, H - margin * 2);
				
				// Drawing frame to widget
				Style.PaintBox(this.Style, GdkWindow, StateType.Normal, ShadowType.In, 
					new Gdk.Rectangle(0, 0, W, H),
					this, null, margin - 2, margin - 2, W - margin * 2 + 4, H - margin * 2 + 4);
	
				if (savedPalette != null)
				{
					// Drawing color matrix backbuffer
					GdkWindow.DrawPixbuf(gc, savedPalette, 0, 0, margin, margin, savedPalette.Width, savedPalette.Height, Gdk.RgbDither.Normal, 0, 0);
				}

				// Drawing dark selected color
				int sel_x = (int)(Tone_to_X(mSelectedDarkTone) * savedPalette.Width) + margin;
				int sel_y = (int)(Tone_to_Y(mSelectedDarkTone) * savedPalette.Height) + margin;
				
				if (mDarkToneSelectorSymbol == ToneSelectorSymbol.Donut)
				{
					GdkWindow.DrawPixbuf(gc, black_donut, 
						0, 0, 
						sel_x - black_donut.Width / 2, 
						sel_y - black_donut.Height / 2, 
						black_donut.Width, black_donut.Height,
						Gdk.RgbDither.None, 0, 0);
				}
				else if (mDarkToneSelectorSymbol == ToneSelectorSymbol.Dot)
				{
					GdkWindow.DrawPixbuf(gc, black_dot, 
						0, 0, 
						sel_x - black_dot.Width / 2, 
						sel_y - black_dot.Height / 2, 
						black_dot.Width, black_dot.Height,
						Gdk.RgbDither.None, 0, 0);
				}

				// Drawing light selected color
				sel_x = (int)(Tone_to_X(mSelectedLightTone) * savedPalette.Width) + margin;
				sel_y = (int)(Tone_to_Y(mSelectedLightTone) * savedPalette.Height) + margin;
				
				if (mLightToneSelectorSymbol == ToneSelectorSymbol.Donut)
				{
					GdkWindow.DrawPixbuf(gc, white_donut, 
						0, 0, 
						sel_x - white_donut.Width / 2, 
						sel_y - white_donut.Height / 2, 
						white_donut.Width, white_donut.Height,
						Gdk.RgbDither.None, 0, 0);
				}
				else if (mLightToneSelectorSymbol == ToneSelectorSymbol.Dot)
				{
					GdkWindow.DrawPixbuf(gc, white_dot, 
						0, 0, 
						sel_x - white_dot.Width / 2, 
						sel_y - white_dot.Height / 2, 
						white_dot.Width, white_dot.Height,
						Gdk.RgbDither.None, 0, 0);
				}
			}
			return true;
		}

		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			if (savedPalette != null) savedPalette.Dispose();
			
			int W = Allocation.Width, H = Allocation.Height;

			// Drawing color matrix to back pixbuf
			savedPalette = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 8, W - margin * 2, H - margin * 2);
			DrawPalette(savedPalette);
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			// Calculate desired size here.
			requisition.Height = 150;
			requisition.Width = 150;
		}
		
		private void SelectNewDarkTone(double winX, double winY)
		{
			double x = (double)(winX - margin) / (Allocation.Width - 2*margin);
			double y = (double)(winY - margin) / (Allocation.Height - 2*margin);
		
			if (x < 0) x = 0;
			if (y < 0) y = 0;
			if (x > 1) x = 1;
			if (y > 1) y = 1;
			
			SelectedDarkTone = XY_to_Tone(x, y);
			OnDarkToneSelected();
				
			QueueDraw();
		}
		private void SelectNewLightTone(double winX, double winY)
		{
			double x = (double)(winX - margin) / (Allocation.Width - 2*margin);
			double y = (double)(winY - margin) / (Allocation.Height - 2*margin);
		
			if (x < 0) x = 0;
			if (y < 0) y = 0;
			if (x > 1) x = 1;
			if (y > 1) y = 1;
			
			SelectedLightTone = XY_to_Tone(x, y);
			OnLightToneSelected();
				
			QueueDraw();
		}
		
		protected override bool OnMotionNotifyEvent (Gdk.EventMotion evnt)
		{
			if (mouse_down && mouse_button == 1)
			{
				SelectNewDarkTone(evnt.X, evnt.Y);
			}
			else if (mouse_down && mouse_button == 3)
			{
				SelectNewLightTone(evnt.X, evnt.Y);
			}
			return base.OnMotionNotifyEvent (evnt);
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton ev)
		{
			mouse_down = true;
			mouse_button = ev.Button;	// 1 for left, 3 for right
			if (mouse_button == 1)
			{
				SelectNewDarkTone(ev.X, ev.Y);
			}
			else if (mouse_button == 3)
			{
				SelectNewLightTone(ev.X, ev.Y);
			}
			return base.OnButtonPressEvent (ev);
		}
		
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			mouse_down = false;
			return base.OnButtonReleaseEvent (evnt);
		}
		
		public override void Dispose ()
		{
			base.Dispose ();
			
			if (black_donut != null) black_donut.Dispose();
			if (white_donut != null) white_donut.Dispose();
			if (black_dot != null) black_dot.Dispose();
			if (white_dot != null) white_dot.Dispose();
			if (savedPalette != null) savedPalette.Dispose();
		}
	}

}

