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
				using (System.Diagnostics.Process prc = DCRawConnection.CreateDCRawProcess("-i -v \"" + mFilename.Replace("\"", "\\\"") + "\""))
				{
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
							string mu = "";
							try
							{
								string res = prc.StandardOutput.ReadLine();
								while (res != null)
								{
									if (res.StartsWith("Camera: "))
									{
										mu += "Camera: <b>" + res.Substring(8) + "</b>\n";
									}
									else if (res.StartsWith("ISO speed: "))
									{
										mu += "ISO speed: <b>" + res.Substring(11) + "</b>\n";
									}
									else if (res.StartsWith("Shutter: "))
									{
										mu += "Shutter: <b>" + res.Substring(9) + "</b>\n";
									}
									else if (res.StartsWith("Aperture: "))
									{
										mu += "Aperture: <b>" + res.Substring(10) + "</b>\n";
									}
									else if (res.StartsWith("Focal length: "))
									{
										mu += "Focal length: <b>" + res.Substring(14) + "</b>\n";
									}
#if DEBUG							
									else 
										Console.WriteLine("metadata> " + res);
#endif
									res = prc.StandardOutput.ReadLine();
								}
								prc.WaitForExit(-1);	// R.I.P.
								prc.Close();
							}
							catch (Exception 
#if DEBUG
								ex
#endif								
								)
							{
#if DEBUG
								Console.WriteLine("Exception occured during the image metadata loading: " + ex.Message);
#endif								
								mu = "Can't load metadata";
							}

							identification_label.Markup = mu;
							file_is_good = true;
							
							GLib.Timeout.Add(100, new GLib.TimeoutHandler(delegate {
		
								Gdk.Pixmap pm = null;
								Gdk.GC gc = null;
								Gdk.Pixbuf pb = null;
								Process prc2 = null;
								System.IO.MemoryStream ms = null;
								try
								{
									pm = new Gdk.Pixmap(thumb_image.GdkWindow, size + margins, size + margins, -1);
									gc = new Gdk.GC(thumb_image.GdkWindow);
									pm.DrawRectangle(gc, true, new Gdk.Rectangle(0, 0, size + margins, size + margins));
									
									// Reading thumbnail
									prc2 = DCRawConnection.CreateDCRawProcess("-e -c \"" + mFilename.Replace("\"", "\\\"") + "\"");
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
			
										prc2.WaitForExit(-1);	// R.I.P.
										prc2.Close();
										ms.Seek(0, System.IO.SeekOrigin.Begin);
									}
									
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
									if (prc2 != null) prc2.Dispose();
									if (ms != null) ms.Close();
									if (gc != null) gc.Dispose();
									if (pm != null) pm.Dispose();
								}
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
			return file_is_good;
		}
	}
	
}

