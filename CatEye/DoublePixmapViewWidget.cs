using System;
using Gdk;
using Gtk;
using CatEye.Core;

namespace CatEye
{

	[System.ComponentModel.ToolboxItem(true)]
	public class DoublePixmapViewWidget : DrawingArea
	{
		public enum ScaleType { None, Divide, Multiply };

		private bool _InstantUpdate = false;
		private FloatPixmap _HDR;
		private Pixbuf _RenderedPicture = null;
		private TimeSpan _UpdateTimeSpan = new TimeSpan(0, 0, 1);	// Initial set to 1 second to avoid possible division by 0
		
		private Rectangle _CurrentImagePosition = new Rectangle(0, 0, 1, 1);
		public Rectangle CurrentImagePosition
		{
			get { return _CurrentImagePosition; }
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
		
		public DoublePixmapViewWidget()
		{
		}

		
		public void UpdatePicture()
		{
			DateTime update_start = DateTime.Now;
			if (_HDR != null)
			{
				if (_RenderedPicture != null)
					_RenderedPicture.Dispose();
					
				_RenderedPicture = Gdk.Pixbuf.FromDrawable(GdkWindow, Gdk.Rgb.Colormap, 0, 0, 0, 0, _HDR.width, _HDR.height);
		
				_HDR.DrawToPixbuf(_RenderedPicture, delegate {
					while (Application.EventsPending()) Application.RunIteration();
					return true;
				});
			}
			_UpdateTimeSpan = DateTime.Now - update_start;
			QueueResizeNoRedraw();
			QueueDraw();
		}
		
		protected override bool OnExposeEvent (EventExpose evnt)
		{
			GdkWindow.Background = new Color(0, 0, 0);
			GdkWindow.Clear();
			
			if (_HDR != null && _RenderedPicture != null)
			{
				int w, h, x = 0, y = 0, d, dstx = 0, dsty = 0;
				GdkWindow.GetGeometry(out x, out y, out w, out h, out d);
				
				if (w > _RenderedPicture.Width)
					dstx = (w - _RenderedPicture.Width) / 2;
				if (h > _RenderedPicture.Height)
					dsty = (h - _RenderedPicture.Height) / 2;
				
				_CurrentImagePosition = new Rectangle(dstx, dsty, _RenderedPicture.Width, _RenderedPicture.Height);
				
				GdkWindow.DrawPixbuf(new Gdk.GC(GdkWindow), _RenderedPicture, 
				                     0, 0, dstx, dsty, 
				                     _RenderedPicture.Width, 
				                     _RenderedPicture.Height, 
				                     RgbDither.None, 0, 0);
				this.SetSizeRequest(_RenderedPicture.Width, _RenderedPicture.Height);
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

		
		protected override bool OnButtonReleaseEvent (EventButton evnt)
		{
			return base.OnButtonReleaseEvent(evnt);
		}
	}
}
