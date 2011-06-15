using System;
using Gtk;
using CatEye.Core;
using CatEye.UI.Base;
using CatEye.UI.Gtk.Widgets;

namespace CatEye.UI.Gtk
{
	public partial class RenderingQueueWindow : Window
	{
		private RenderingQueue mRenderingQueue;
		
		public RenderingQueueWindow (RenderingQueue renderingQueue) : 
				base(WindowType.Toplevel)
		{
			mRenderingQueue = renderingQueue;
			mRenderingQueue.ImageToFileSaver = ImageToFileSaver;
			mRenderingQueue.QueueProgressMessageReport += HandleRenderingQueueProgressMessageReport;
			
			this.Build ();
		}

		private void HandleRenderingQueueProgressMessageReport (string source, string destination, double progress, string status)
		{
			source_label.Text = source;
			destination_label.Text = destination;
			processing_progressbar.Fraction = progress;
			processing_progressbar.Text = status;
		}

		private void ImageToFileSaver(IBitmapCore image, string filename, string type)
		{
			FloatBitmapGtk fbg = (FloatBitmapGtk)image;
			
			// Drawing to pixbuf and saving to file
			using (Gdk.Pixbuf rp = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 8, fbg.Width, fbg.Height))
			{
				fbg.DrawToPixbuf(rp, 
					delegate (double progress) {
						processing_progressbar.Fraction = progress;
						processing_progressbar.Text = "Saving image...";
						return true;
					}
				);
			
				// TODO Can't be used currently cause of buggy Gtk#
				//rp.Savev(filename, type, new string[] { "quality" }, new string[] { "95" });
		
				rp.Save(filename, type);
			}
		}
	}
}

