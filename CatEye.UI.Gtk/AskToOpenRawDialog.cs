using System;
using Gtk;
using System.Diagnostics;
using CatEye.Core;

namespace CatEye.UI.Gtk
{
	public partial class AskToOpenRawDialog : global::Gtk.Dialog
	{
		private string mFilename;
		
		public string Filename 
		{ 
			get { return mFilename; } 
			set 
			{ 
				mFilename = value; 
				if (Visible) UpdateContent();
			}
		}

		public int Prescale 
		{ 
			get 
			{
				return (int)prescale_hscale.Value;
			}
			set
			{
				prescale_hscale.Value = value;
			}
		}
				
		protected override void OnShown ()
		{
			Title = MainClass.APP_NAME;
			base.OnShown ();
			UpdateContent();
		}
		
		void UpdateContent()
		{
			thumb_image.Clear();
			
			int size = 200, margins = 30;
			thumb_image.SetSizeRequest(size + margins * 2, size + margins);
			
			found_label.Markup = "Image <b>" + System.IO.Path.GetFileName(mFilename) + "</b> found\nfor the cestage file you selected.";
			
			origsize_label.Markup = "";
			System.Diagnostics.Process prc = DCRawConnection.CreateDCRawProcess("-i -v \"" + mFilename.Replace("\"", "\\\"") + "\"");
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
							mu += "<b>Aperture: </b>" + res.Substring(10) + "";
						}
						    
						res = prc.StandardOutput.ReadLine();
					}
					prc.WaitForExit(-1);	// R.I.P.
					prc.Close();
					
					identification_label.Markup = mu;
					
					GLib.Timeout.Add(100, new GLib.TimeoutHandler(delegate {

						Gdk.Pixmap pm = new Gdk.Pixmap(thumb_image.GdkWindow, size + margins, size + margins, -1);
						Gdk.GC gc = new Gdk.GC(thumb_image.GdkWindow);
						pm.DrawRectangle(gc, true, new Gdk.Rectangle(0, 0, size + margins, size + margins));
						
						// Reading thumbnail
						System.IO.MemoryStream ms = null;
						Process prc2 = DCRawConnection.CreateDCRawProcess("-e -c \"" + mFilename.Replace("\"", "\\\"") + "\"");
						if (prc2.Start())
						{
							int readed = 0;
							int readed_all = 0;
							ms = new System.IO.MemoryStream();
							byte[] buf = new byte[1024 * 4];
							do
							{
								readed = prc2.StandardOutput.BaseStream.Read(buf, 0, buf.Length);
								ms.Write(buf, 0, readed);
								readed_all += readed;
							}
							while (readed > 0);
				
							while (Application.EventsPending()) Application.RunIteration();

							prc2.WaitForExit(-1);	// R.I.P.
							prc2.Close();
							ms.Seek(0, System.IO.SeekOrigin.Begin);
						}
						prc2.Dispose();
						
						Gdk.Pixbuf pb = null;
						try
						{
							pb = new Gdk.Pixbuf(ms.ToArray());
						}
						catch (GLib.GException)
						{
						}
						
						ms.Close();
						
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
			prc.Dispose();
			
		}
		
		public AskToOpenRawDialog ()
		{
			this.Build ();
			
		}
	}
}

