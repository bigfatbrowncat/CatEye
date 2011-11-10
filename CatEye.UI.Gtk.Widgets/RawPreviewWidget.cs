using System;
using System.Diagnostics;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class RawPreviewWidget : Bin
	{
		private string mFilename;
		private bool mFileIsGood;
		
		public RawPreviewWidget ()
		{
			this.Build ();
		}
		
		public string Filename
		{
			get { return mFilename; }
			set
			{
				mFilename = value;
				mFileIsGood = UpdatePreview();
			}
		}
		
		public bool FileIsGood
		{
			get { return mFileIsGood; }
		}
		
		protected bool UpdatePreview()
		{
			thumb_image.Clear();
			bool file_is_good = false;
			
			int size = 200, margins = 30;
	
			if (System.IO.File.Exists(mFilename))  // Selected item is a file
			{
				origsize_label.Markup = "";

				file_is_good = true;
				
				GLib.Timeout.Add(100, new GLib.TimeoutHandler(delegate {
		
					Gdk.Pixmap pm = null;
					Gdk.GC gc = null;
					Gdk.Pixbuf pb = null;
					try
					{
						pm = new Gdk.Pixmap(thumb_image.GdkWindow, size + margins, size + margins, -1);
						gc = new Gdk.GC(thumb_image.GdkWindow);
						pm.DrawRectangle(gc, true, new Gdk.Rectangle(0, 0, size + margins, size + margins));
		
						RawDescriptionLoader rdl = RawDescriptionLoader.FromFile(mFilename);
						
						string idtext = "";
						try
						{
							idtext += "Shot taken\n" +
								      "   on <b>" + rdl.TimeStamp.ToString("MMMM, d, yyyy") + "</b> at <b>" + rdl.TimeStamp.ToString("h:mm:ss") + "</b>,\n";

							idtext += "   with <b>" + rdl.CameraMaker + " " + rdl.CameraModel + "</b>\n\n";
							idtext += "ISO speed: <b>" + rdl.ISOSpeed.ToString("0") + "</b>\n";
							if (rdl.Shutter > 1)
								idtext += "Shutter: <b>" + rdl.Shutter.ToString("0.0") + "</b> sec\n";
							else
								idtext += "Shutter: <b>1/" + (1.0 / (rdl.Shutter + 0.000001)).ToString("0") + "</b> sec\n";
								
							idtext += "Aperture: <b>" + rdl.Aperture.ToString("0.0") + "</b>\n" +
								      "Focal length: <b>" + rdl.FocalLength.ToString("0") + "</b> mm\n";
							
							if (rdl.Artist != "") idtext += "Artist: <b>" + rdl.Artist + "</b>\n";
							if (rdl.Description != "") idtext += "Description: <b>" + rdl.Description + "</b>\n";
							
							// Displaying the thumbnail
							pb = new Gdk.Pixbuf(rdl.ThumbnailData);
						}
						catch (Exception ex)
						{
							Console.WriteLine("Can't load the thumbnail: " + ex.Message);
							idtext += "\n<i>Can't load the thumbnail.</i>";
						}
						identification_label.Markup = idtext;
						
						if (pb != null)
						{
							Gdk.Pixbuf pbold = pb;
							origsize_label.Markup = "Image size: <b>" + pb.Width + "</b> x <b>" + pb.Height + "</b>";
							if (pb.Width > pb.Height)
								pb = pb.ScaleSimple(size, (int)((double)pb.Height / pb.Width * size), Gdk.InterpType.Bilinear);
							else
								pb = pb.ScaleSimple((int)((double)pb.Width / pb.Height * size), size, Gdk.InterpType.Bilinear);
								
							pbold.Dispose();
							
		
							pm.DrawPixbuf(gc, pb, 0, 0, 
							              (size + margins) / 2 - pb.Width / 2, 
							              (size + margins) / 2 - pb.Height / 2, 
							              pb.Width, pb.Height, Gdk.RgbDither.Max, 0, 0);
							thumb_image.SetFromPixmap(pm, null);
							pb.Dispose();
						}
		
					}
					catch (Exception 
#if DEBUG
						ex
#endif								
						)
					{
#if DEBUG
						Console.WriteLine("Exception occured during the thumbnail loading process: " + ex.Message);
#endif								
						identification_label.Text = "Cannot decode selected file";
					}
					finally
					{
						if (gc != null) gc.Dispose();
						if (pm != null) pm.Dispose();
					}
					return false;
				}));
			}
			return file_is_good;
		}
	}
	
}

