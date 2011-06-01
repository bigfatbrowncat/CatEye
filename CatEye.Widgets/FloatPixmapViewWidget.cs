using System;
using Gdk;
using Gtk;
using CatEye.Core;

namespace CatEye
{
	public delegate bool MousePositionChangedHandler(object sender, int x, int y);
	public delegate bool MouseButtonStateChangedHandler(object sender, int x, int y, uint button_id, bool is_down);

	
	[System.ComponentModel.ToolboxItem(true)]
	public class FloatPixmapViewWidget : DrawingArea, IBitmapView
	{
		public enum ScaleType { None, Divide, Multiply };
		
		public event MousePositionChangedHandler MousePositionChanged;
		public event MouseButtonStateChangedHandler MouseButtonStateChanged;
		
		private bool _InstantUpdate = false;
		private FloatPixmap _HDR;
		private Pixbuf _RenderedPicture = null;
		private TimeSpan _UpdateTimeSpan = new TimeSpan(0, 0, 1);	// Initial set to 1 second to avoid possible division by 0

		private int mImageCenterX, mImageCenterY;
		private bool mAutoPan = true;
		
		// Panning internals
		private bool mPanInProgress = false;
		private int mPanStartX, mPanStartY, mImgCenterStartX, mImgCenterStartY;
			
		public Rectangle CurrentImagePosition
		{
			get 
			{
				if (_RenderedPicture != null)
					return new Rectangle(
						mImageCenterX - _RenderedPicture.Width / 2, 
						mImageCenterY - _RenderedPicture.Height / 2, 
						_RenderedPicture.Width, _RenderedPicture.Height);
				else
					return new Gdk.Rectangle(0, 0, 0, 0);
			}
		}
		
		public FloatPixmap HDR
		{
			get { return _HDR; }
			set 
			{
				_HDR = value; 
				if (_InstantUpdate) UpdatePicture();
			}
		}
		
		public bool InstantUpdate
		{
			get {  return _InstantUpdate; }
			set { _InstantUpdate = value; }
		}
		
		public TimeSpan UpdateTimeSpan { get { return _UpdateTimeSpan; } }
		
		public FloatPixmapViewWidget()
		{
			this.Events |= 
							Gdk.EventMask.PointerMotionMask |
							Gdk.EventMask.ButtonMotionMask |
							Gdk.EventMask.ButtonPressMask |
							Gdk.EventMask.ButtonReleaseMask |
							Gdk.EventMask.Button1MotionMask |
							Gdk.EventMask.ExposureMask;
		}
		
		public void CenterImagePanning()
		{
			mImageCenterX = Allocation.Width  / 2;
			mImageCenterY = Allocation.Height / 2;
		}

		public void UpdatePicture()
		{
			GdkWindow.Cursor = new Cursor(Gdk.CursorType.Watch);
			
			DateTime update_start = DateTime.Now;
			if (_HDR != null)
			{
				Gdk.Pixbuf newRenderedPicture = Gdk.Pixbuf.FromDrawable(GdkWindow, Gdk.Rgb.Colormap, 0, 0, 0, 0, _HDR.Width, _HDR.Height);
	
				_HDR.DrawToPixbuf(newRenderedPicture, delegate {
					while (Application.EventsPending()) Application.RunIteration();
					return true;
				});
				
				if (_RenderedPicture != null)
					_RenderedPicture.Dispose();

				_RenderedPicture = newRenderedPicture;
					
			}
			if (!mPanInProgress)
				GdkWindow.Cursor = new Cursor(Gdk.CursorType.Arrow);
			else
				GdkWindow.Cursor = new Cursor(Gdk.CursorType.Hand1);
				
			
			_UpdateTimeSpan = DateTime.Now - update_start;
			//QueueResizeNoRedraw();
			QueueDraw();
		}
		
		protected override bool OnExposeEvent (EventExpose evnt)
		{
			GdkWindow.Background = new Color(0, 0, 0);
			GdkWindow.Clear();
			
			if (_HDR != null && _RenderedPicture != null)
			{
				Rectangle r = CurrentImagePosition;
				
				GdkWindow.DrawPixbuf(new Gdk.GC(GdkWindow), _RenderedPicture, 
				                     0, 0, 
									 r.X, r.Y, 
				                     r.Width, 
				                     r.Height, 
				                     RgbDither.None, 0, 0);
				
				// If panning, draw panning scheme
				if (mPanInProgress)
				{
					int size_divider = 10;
					
					Rectangle screen_rect = new Rectangle(
						Allocation.Width / 2 - Allocation.Width / size_divider / 2,
						Allocation.Height / 2 - Allocation.Height / size_divider / 2,
						Allocation.Width / size_divider,
						Allocation.Height / size_divider);
					
					Rectangle picture_rect = new Rectangle(
						Allocation.Width / 2 - Allocation.Width / size_divider / 2 + 
						mImageCenterX / size_divider - _RenderedPicture.Width / size_divider / 2,
						
						Allocation.Height / 2 - Allocation.Height / size_divider / 2 + 
						mImageCenterY / size_divider - _RenderedPicture.Height / size_divider / 2,
						
						_RenderedPicture.Width / size_divider,
						_RenderedPicture.Height / size_divider
						);
					
					using (Gdk.GC gc = new Gdk.GC(GdkWindow))
					{
						gc.Function = Gdk.Function.Xor;
						gc.RgbFgColor = new Color(255, 255, 0);
						GdkWindow.DrawRectangle(gc, false, screen_rect);
						gc.RgbFgColor = new Color(255, 255, 255);
						GdkWindow.DrawRectangle(gc, false, picture_rect);
						gc.Function = Gdk.Function.Clear;
					}
				}
				
				//this.SetSizeRequest(_RenderedPicture.Width, _RenderedPicture.Height);
			}
			return base.OnExposeEvent (evnt);
		}

		
		public void SavePicture(string FileName, string Type)
		{
			if (_HDR != null)
			{
				if (_RenderedPicture != null)
					_RenderedPicture.Save(FileName, Type);
			}
		}
		
		public override void Dispose ()
		{
			if (_RenderedPicture != null) _RenderedPicture.Dispose();
			base.Dispose ();
		}
		
		protected override bool OnMotionNotifyEvent (EventMotion evnt)
		{
			if (MousePositionChanged != null && MousePositionChanged(this, (int)evnt.X, (int)evnt.Y))
			{
				return true;
			}
			else
			{
				if (mPanInProgress)
				{
					if (!mAutoPan)
					{
						mPanInProgress = false;
						GdkWindow.Cursor = new Cursor(Gdk.CursorType.Arrow);
					}
					else
					{
						mImageCenterX = mImgCenterStartX + (int)(evnt.X -  mPanStartX);
						mImageCenterY = mImgCenterStartY + (int)(evnt.Y -  mPanStartY);
						
						// Applying framing
						
						if (_RenderedPicture != null)
						{
							if (mImageCenterX > Allocation.Width + _RenderedPicture.Width / 2)
								mImageCenterX = Allocation.Width + _RenderedPicture.Width / 2;
							if (mImageCenterY > Allocation.Height + _RenderedPicture.Height / 2)
								mImageCenterY = Allocation.Height + _RenderedPicture.Height / 2;

							if (mImageCenterX < -_RenderedPicture.Width / 2)
								mImageCenterX = -_RenderedPicture.Width / 2;
							if (mImageCenterY < -_RenderedPicture.Height / 2)
								mImageCenterY = -_RenderedPicture.Height / 2;
						}
						
						QueueDraw();
					}
					return true;
				}
			}
			return false;
		}
		
		protected override bool OnButtonPressEvent (EventButton evnt)
		{
			if (MouseButtonStateChanged != null && 
				MouseButtonStateChanged(this, (int)evnt.X, (int)evnt.Y, evnt.Button, true))
			{
				return true;
			}
			else
			{
				if (mAutoPan)
				{
					mPanInProgress = true;
					GdkWindow.Cursor = new Cursor(Gdk.CursorType.Hand1);
					mImgCenterStartX = mImageCenterX;
					mImgCenterStartY = mImageCenterY;
					mPanStartX = (int)evnt.X;
					mPanStartY = (int)evnt.Y;
					return true;
				}
			}
			return false;
		}
		
		protected override bool OnButtonReleaseEvent (EventButton evnt)
		{
			if (MouseButtonStateChanged != null && 
				MouseButtonStateChanged(this, (int)evnt.X, (int)evnt.Y, evnt.Button, false))
			{
				return true;
			}
			else
			{
				if (mPanInProgress)
				{
					GdkWindow.Cursor = new Cursor(Gdk.CursorType.Arrow);
					mPanInProgress = false;
					QueueDraw();
					return true;
				}
			}
			return false;
		}
	}
}
