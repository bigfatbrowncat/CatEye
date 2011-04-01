using System;
using Gtk;
using System.Diagnostics;

namespace CatEye
{


	public partial class RawImportDialog : Gtk.Dialog
	{
		string [] raw_extensions = {
				".arw",
				".crw",
				".cr2",
				".dng",
				".mrw",
				".nef",
				".orf",
				".pef",
				".raw",
				".raf",
				".rw2",
		};		
		
		private string _Filename = null;
		private int _PreScale = 1;
		private bool file_is_good = false;
		
		public string Filename { get { return _Filename; } }
		public int PreScale { get { return _PreScale; } }
		
		public RawImportDialog ()
		{
			this.Build ();

			// Adding prescale list
			Gtk.ListStore ls = new Gtk.ListStore(typeof(string), typeof(int));
			ls.AppendValues("No downscaling", 1);
			ls.AppendValues("Divide by 2", 2);
			ls.AppendValues("Divide by 3", 3);
			ls.AppendValues("Divide by 4", 4);
			ls.AppendValues("Divide by 5", 5);
			ls.AppendValues("Divide by 6", 6);
			ls.AppendValues("Divide by 7", 7);
			ls.AppendValues("Divide by 8", 8);
			
			Gtk.ComboBox pres_cb = prescale_combobox;
			pres_cb.Model = ls;
			
			// Selecting "No downscale"
			TreeIter ti;
			ls.GetIterFirst(out ti);
			ls.IterNext(ref ti);
			pres_cb.SetActiveIter(ti);
		}
	
		protected virtual void OnFilechooserwidgetSelectionChanged (object sender, System.EventArgs e)
		{
			thumb_image.Clear();
			open_button.Sensitive = false;
			file_is_good = false;
			
			int size = 200, margins = 30;

			string filename = filechooserwidget.Filename;
			if (System.IO.File.Exists(filename))  // Selected item is a file
			{
				origsize_label.Markup = "";
				System.Diagnostics.Process prc = DCRawConnection.CreateDCRawProcess("-i -v \"" + filename + "\"");
				if (prc.Start())
				{
					string err = prc.StandardError.ReadLine();
					if (err != null && err.StartsWith("Cannot decode file"))
					{
						identification_label.Text = "Cannot decode selected file";
					}
					else
					{
						// Reading metadata
						
						string res = prc.StandardOutput.ReadLine();
						string mu = "";
						while (res != null)
						{
							if (res.StartsWith("Camera: "))
							{
								mu += "<b>Camera: </b>" + res.Substring(8) + "\n";
							}
							if (res.StartsWith("ISO speed: "))
							{
								mu += "<b>ISO speed: </b>" + res.Substring(11) + "\n";
							}
							if (res.StartsWith("Shutter: "))
							{
								mu += "<b>Shutter: </b>" + res.Substring(9) + "\n";
							}
							if (res.StartsWith("Aperture: "))
							{
								mu += "<b>Aperture: </b>" + res.Substring(10) + "\n";
							}
							    
							res = prc.StandardOutput.ReadLine();
						}
						identification_label.Markup = mu;
						open_button.Sensitive = true;
						file_is_good = true;
						
						GLib.Timeout.Add(200, new GLib.TimeoutHandler(delegate {

							Gdk.Pixmap pm = new Gdk.Pixmap(thumb_image.GdkWindow, size + margins, size + margins, -1);
							Gdk.GC gc = new Gdk.GC(thumb_image.GdkWindow);
							pm.DrawRectangle(gc, true, new Gdk.Rectangle(0, 0, size + margins, size + margins));
							
							// Reading thumbnail
							System.IO.MemoryStream ms = null;
							Process prc2 = DCRawConnection.CreateDCRawProcess("-e -c \"" + filename + "\"");
							if (prc2.Start())
							{
								int readed = 0;
								int readed_all = 0;
								ms = new System.IO.MemoryStream();
								do
								{
									byte[] buf = new byte[1024 * 4];
									readed = prc2.StandardOutput.BaseStream.Read(buf, 0, buf.Length);
									ms.Write(buf, 0, readed);
									readed_all += readed;
								}
								while (readed > 0);
					
								while (Application.EventsPending()) Application.RunIteration();
				
								ms.Seek(0, System.IO.SeekOrigin.Begin);
							}
							
							Gdk.Pixbuf pb = null;
							try
							{
								pb = new Gdk.Pixbuf(ms.ToArray());
							}
							catch (GLib.GException)
							{
							}
							
							if (pb != null)
							{
								Gdk.Pixbuf pbold = pb;
								origsize_label.Markup = "<b>Image size: </b>" + pb.Width + " x " + pb.Height;
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

							gc.Dispose();
							pm.Dispose();
							return false;
						}));
					}
				}
				else
				{
					identification_label.Text = "Can not start DCRaw";
				}
			}

		}
		protected virtual void OnResponse (object o, Gtk.ResponseArgs args)
		{
			TreeIter ti;
			prescale_combobox.GetActiveIter(out ti);
			_PreScale = (int)(((ListStore)prescale_combobox.Model).GetValue(ti, 1));
			
			_Filename = filechooserwidget.Filename;
		}
		
		protected virtual void OnClose (object sender, System.EventArgs e)
		{
		}
		
		protected virtual void OnFilechooserwidgetFileActivated (object sender, System.EventArgs e)
		{
			if (file_is_good)
				Respond(ResponseType.Accept);
		}
		
		
		
		
	}
}
