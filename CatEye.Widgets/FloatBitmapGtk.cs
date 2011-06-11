using System;

namespace CatEye.Core
{
	public class FloatBitmapGtk : FloatBitmap
	{
		public static new FloatBitmapGtk FromPPM(PPMLoader ppm, ProgressReporter callback)
		{
			FloatBitmapGtk fbg = new FloatBitmapGtk();
			
			if (fbg.LoadDataFromPPM(ppm, callback))
				return fbg;
			else
				return null;
		}
		
		private byte cut(double val)
		{
			if (val > 255) return 255;
			if (val < 0) return 0;
			return (byte)val;
		}
		
		/// <summary>
		/// Draws image into selected pixbuf
		/// </summary>
		/// <param name="callback">
		/// A <see cref="ProgressReporter"/> to report drawing progress.
		/// </param>
		public unsafe void DrawToPixbuf(Gdk.Pixbuf buf, ProgressReporter callback)
		{
			double N = 1;	// This is the norm. It should be equal to the
							// value which means the lightest point of 
							// the screen
			
			int chan = buf.NChannels;
			int w = buf.Width, h = buf.Height, stride = buf.Rowstride;
			
			// counting the maximum light value
			double max = 0;
			for (int i = 0; i < mWidth; i++)
			for (int j = 0; j < mHeight; j++)
			{
				double r = N * (1.0 - Math.Exp(-(double)r_chan[i, j] / N));
				double g = N * (1.0 - Math.Exp(-(double)g_chan[i, j] / N));
				double b = N * (1.0 - Math.Exp(-(double)b_chan[i, j] / N));

				double light = Math.Sqrt(r*r + g*g + b*b) / Math.Sqrt(3);
				if (light > max) max = light;
			}
			if (max > 1) max = 1;
			
			byte *cur_row = (byte *)buf.Pixels;
			for (int j = 0; j < h; j++)
			{
				if (j >= this.mHeight)
				{
					break;
				}
				
				if (j % REPORT_EVERY_NTH_LINE == 0 && callback != null)
				{
					if (!callback((double)j / this.mHeight)) return;
				}
				
				byte *cur_pixel = cur_row;
				for (int i = 0; i < w; i++)
				{
					if (i >= this.mWidth)
					{
						break;
					}
					
					double r = N * (1.0 - Math.Exp(-(double)r_chan[i, j] / N));
					double g = N * (1.0 - Math.Exp(-(double)g_chan[i, j] / N));
					double b = N * (1.0 - Math.Exp(-(double)b_chan[i, j] / N));
					
					cur_pixel[0] = cut(r / max * 255);      // Red
					cur_pixel[1] = cut(g / max * 255);      // Green
					cur_pixel[2] = cut(b / max * 255);      // Blue
					cur_pixel += chan;
				}
				cur_row += stride;
			}
		}
		
	}
}
