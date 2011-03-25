
using System;
using Gdk;

namespace CatEye
{

	[System.ComponentModel.ToolboxItem(true)]
	public class DoublePixmapViewWidget : Gtk.Bin
	{
		public enum ScaleType { None, Divide, Multiply };

		private DoublePixmap _HDR;
		private Pixbuf _RenderedPicture = null;
		private TimeSpan _UpdateTimeSpan = new TimeSpan(0, 0, 1);	// Initial set to 1 second to avoid possible division by 0
		
		public DoublePixmap HDR
		{
			get { return _HDR; }
			set 
			{ 
				_HDR = value; 
				UpdatePicture();
			}
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
		
				_HDR.DrawToPixbuf(_RenderedPicture);
			}
			_UpdateTimeSpan = DateTime.Now - update_start;
			Console.WriteLine(_UpdateTimeSpan);
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

	}
}
