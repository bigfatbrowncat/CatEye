using System;

namespace CatEye.Core
{
	public class FloatBitmap : IBitmapCore
	{
		protected const int REPORT_EVERY_NTH_LINE = 5;
		
		protected float[,] r_chan, g_chan, b_chan;
		
		protected int mWidth, mHeight;
		
		public int Width { get { return mWidth; } }
		public int Height { get { return mHeight; } }
		
		protected FloatBitmap ()
		{
			
		}
		public FloatBitmap (FloatBitmap src)
		{
			mWidth = src.mWidth; mHeight = src.mHeight;
			r_chan = new float[mWidth, mHeight];
			g_chan = new float[mWidth, mHeight];
			b_chan = new float[mWidth, mHeight];
			
			for (int i = 0; i < mWidth; i++)
			for (int j = 0; j < mHeight; j++)
			{
				r_chan[i, j] = src.r_chan[i, j];
				g_chan[i, j] = src.g_chan[i, j];
				b_chan[i, j] = src.b_chan[i, j];
			}
			
		}
		
		public object Clone ()
		{
			return new FloatBitmap(this);
		}
		
		public static FloatBitmap FromPPM(PPMLoader ppm, ProgressReporter callback)
		{
			FloatBitmap fbg = new FloatBitmap();
			
			if (fbg.LoadDataFromPPM(ppm, callback))
				return fbg;
			else
				return null;
		}
		
		protected bool LoadDataFromPPM(PPMLoader ppm, ProgressReporter callback)
		{
			mWidth = ppm.Header.Width;
			mHeight = ppm.Header.Height;
			
			r_chan = new float[mWidth, mHeight];
			g_chan = new float[mWidth, mHeight];
			b_chan = new float[mWidth, mHeight];
			
			for (int i = 0; i < mWidth; i++)
			{
				if (i % REPORT_EVERY_NTH_LINE == 0 && callback != null) 
				{
					if (!callback((double)i / mWidth / 2)) 
						return false;
				}
				for (int j = 0; j < mHeight; j++)
				{
					r_chan[i, j] = ppm.RChannel[i, j];
					g_chan[i, j] = ppm.GChannel[i, j];
					b_chan[i, j] = ppm.BChannel[i, j];
				}
			}
			
			// Searching for maximum
			double Max = CalcMaxLight();
			
			
			// Normalizing to 0..1
			for (int i = 0; i < mWidth; i++)
			{
				if (i % REPORT_EVERY_NTH_LINE == 0 && callback != null) 
				{
					if (!callback(0.5 + (double)i / mWidth / 2)) 
						return false;
				}
				for (int j = 0; j < mHeight; j++)
				{
					r_chan[i, j] /= (float)Max;
					g_chan[i, j] /= (float)Max;
					b_chan[i, j] /= (float)Max;
				}
			}
			
			return true;
		}

		public void ScaleFast(double k, ProgressReporter callback)
		{
			float[,] new_r = new float[(int)(mWidth * k), (int)(mHeight * k)];
			float[,] new_g = new float[(int)(mWidth * k), (int)(mHeight * k)];
			float[,] new_b = new float[(int)(mWidth * k), (int)(mHeight * k)];
			int[,] sum = new int[(int)(mWidth * k), (int)(mHeight * k)];

			for (int i = 0; i < (int)(mWidth * k); i++)
			{
				if (i % REPORT_EVERY_NTH_LINE == 0 && callback != null)
				{
					if (!callback((double)i / (mWidth * k)))
					{
						throw new UserCancelException();
					}
				}
				for (int j = 0; j < (int)(mHeight * k); j++)
				{
					double r = 0, g = 0, b = 0; int s = 0;
					for (int u = (int)((double)i / k); u < (int)(((double)i + 1) / k); u++)
					for (int v = (int)((double)j / k); v < (int)(((double)j + 1) / k); v++)
					{
						if (u < mWidth && v < mHeight)
						{
							r += r_chan[u, v];
							g += g_chan[u, v];
							b += b_chan[u, v];
							s ++;
						}
					}
					new_r[i, j] = (float)r;
					new_g[i, j] = (float)g;
					new_b[i, j] = (float)b;
					sum[i, j] = s;
				}
				
			}
			
			for (int i = 0; i < (int)(mWidth * k); i++)
				for (int j = 0; j < (int)(mHeight * k); j++)
			{
				new_r[i, j] /= sum[i, j];
				new_g[i, j] /= sum[i, j];
				new_b[i, j] /= sum[i, j];
			}

			r_chan = new_r;
			g_chan = new_g;
			b_chan = new_b;
			mWidth = (int)(mWidth * k);
			mHeight = (int)(mHeight * k);
		}
		
/*		/// <summary>
		/// 
		/// </summary>
		/// <param name="left">
		/// Left from 0 to 1
		/// </param>
		/// <param name="top">
		/// Top from 0 to 1
		/// </param>
		/// <param name="right">
		/// Right from 0 to 1 must be greater than left
		/// </param>
		/// <param name="bottom">
		/// Bottom from 0 to 1 must be greater than top
		/// </param>
		/// <param name="callback">
		/// A <see cref="ProgressReporter"/>, which will be used to report the progress.
		/// </param>
		public void Crop(double left, double top, double right, double bottom, ProgressReporter callback)
		{
			int i1 = Math.Max(0, (int)(width * left)), 
				i2 = Math.Min(width - 1, (int)(width * right)), 
				j1 = Math.Max(0, (int)(height * top)), 
				j2 = Math.Min(height - 1, (int)(height * bottom));
			
			float[,] newr = new float[i2 - i1 + 1, j2 - j1 + 1];
			float[,] newg = new float[i2 - i1 + 1, j2 - j1 + 1];
			float[,] newb = new float[i2 - i1 + 1, j2 - j1 + 1];
			
			for (int i = i1; i <= i2; i++)
			{
				if (i % REPORT_EVERY_NTH_LINE == 0 && callback != null) 
					callback((double)(i - i1) / (i2 - i1 + 1));
				for (int j = j1; j <= j2; j++)
				{
					newr[i - i1, j - j1] = r_chan[i, j];
					newg[i - i1, j - j1] = g_chan[i, j];
					newb[i - i1, j - j1] = b_chan[i, j];
				}
			}
			
			r_chan = newr; g_chan = newg; b_chan = newb;
			width = i2 - i1 + 1;
			height = j2 - j1 + 1;
		}
*/
		
		public void AmplitudeMultiply(double Amplitude, ProgressReporter callback)
		{
			for (int i = 0; i < mWidth; i++)
			{
				if (i % REPORT_EVERY_NTH_LINE == 0 && callback != null)
				{
					if (!callback((double)i / mWidth))
					{
						throw new UserCancelException();
					}
				}
				
				for (int j = 0; j < mHeight; j++)
				{
					r_chan[i, j] *= (float)Amplitude;
					g_chan[i, j] *= (float)Amplitude;
					b_chan[i, j] *= (float)Amplitude;
				}
			}
		}

		public double AmplitudeFindMedian()
		{
			double local_mid = 0;
			double[,] light = new double[mWidth, mHeight];
			
			// Searching median
			for (int i = 0; i < mWidth; i++)
			for (int j = 0; j < mHeight; j++)
			{
				light[i, j] = Math.Sqrt(r_chan[i, j] * r_chan[i, j] +
				                               g_chan[i, j] * g_chan[i, j] +
				                               b_chan[i, j] * b_chan[i, j]);
				
				local_mid += light[i, j];
			}
			local_mid /= (double)mWidth * mHeight;
			
			return local_mid;
		}

		public double AmplitudeFindBlackPoint()
		{
			double[,] light = new double[mWidth, mHeight];
			
			// Searching minimum
			double local_min = Math.Sqrt(r_chan[0, 0] * r_chan[0, 0] +
				                         g_chan[0, 0] * g_chan[0, 0] +
				                         b_chan[0, 0] * b_chan[0, 0]) / Math.Sqrt(3);
			for (int i = 0; i < mWidth; i++)
			for (int j = 0; j < mHeight; j++)
			{
				light[i, j] = Math.Sqrt(r_chan[i, j] * r_chan[i, j] +
				                               g_chan[i, j] * g_chan[i, j] +
				                               b_chan[i, j] * b_chan[i, j]) / Math.Sqrt(3);
				
				if (local_min > light[i, j]) local_min = light[i, j];
			}

			return local_min;
		}
		
		public void AmplitudeAdd(double delta)
		{
			for (int i = 0; i < mWidth; i++)
			for (int j = 0; j < mHeight; j++)
			{				
				double amp = Math.Sqrt(r_chan[i, j] * r_chan[i, j] + 
				                       g_chan[i, j] * g_chan[i, j] + 
				                       b_chan[i, j] * b_chan[i, j]);
	
				// Adding "power" to color amplitude
				if (amp + delta > 0)
				{
					r_chan[i, j] *= (float)((amp + delta) / amp);
					g_chan[i, j] *= (float)((amp + delta) / amp);
					b_chan[i, j] *= (float)((amp + delta) / amp);
				}
				else
				{
					r_chan[i, j] = 0;
					g_chan[i, j] = 0;
					b_chan[i, j] = 0;
				}
			}
		}
		
		protected double CalcMaxLight()
		{
			double Max = 0;
			for (int i = 0; i < mWidth; i++)
			for (int j = 0; j < mHeight; j++)
			{
				double light = Math.Sqrt(r_chan[i, j] * r_chan[i, j] + 
							  			g_chan[i, j] * g_chan[i, j] + 
						      			b_chan[i, j] * b_chan[i, j]) / Math.Sqrt(3);
				
				if (Max < light) Max = light;
			}
			return Max;
		}
		
		public void CompressLight(double power, double dark_preserving, ProgressReporter callback)
		{
			for (int i = 0; i < mWidth; i++)
			{
				if (i % REPORT_EVERY_NTH_LINE == 0 && callback != null)
				{
					if (!callback((double)i / mWidth))
					{
						throw new UserCancelException();
					}
				}
				for (int j = 0; j < mHeight; j++)
				{
					double light = Math.Sqrt(r_chan[i, j] * r_chan[i, j] +
					                         g_chan[i, j] * g_chan[i, j] +
					                         b_chan[i, j] * b_chan[i, j]) / Math.Sqrt(3);
					
					r_chan[i, j] = r_chan[i, j] * (float)(Math.Pow(1.0 / (light + dark_preserving), power));
					g_chan[i, j] = g_chan[i, j] * (float)(Math.Pow(1.0 / (light + dark_preserving), power));
					b_chan[i, j] = b_chan[i, j] * (float)(Math.Pow(1.0 / (light + dark_preserving), power));
		
				}
			}
		}
		
		public void SharpenLight(double radius_part, double power, double limit_up, double limit_down, int points, ProgressReporter callback)
		{
			double[,] light = new double[mWidth, mHeight];
			double maxlight = 0;
			unsafe {
	
				// Сalculating light
				for (int i = 0; i < mWidth; i++)
				for (int j = 0; j < mHeight; j++)
				{
					light[i, j] = Math.Sqrt(r_chan[i, j] * r_chan[i, j] + 
								  			g_chan[i, j] * g_chan[i, j] + 
							      			b_chan[i, j] * b_chan[i, j]) / Math.Sqrt(3);
					if (light[i,j] > maxlight) maxlight = light[i,j];
				}
			}
			
			double[,] scale_matrix = new double[mWidth, mHeight];
			int[,] scale_matrix_adds = new int[mWidth, mHeight];
			
			int radius = (int)((mWidth + mHeight) / 2 * radius_part + 1);
			
			Random rnd = new Random();
			
			Console.WriteLine("Calculating scale factors...");
			unsafe {
				for (int i = 0; i < mWidth + radius; i++)	// "radius" added to process all "i_back" values
				{
					int i_back = i - radius;
					if (callback != null)
					{
						if (!callback((double)i / (mWidth + radius)))
							throw new UserCancelException();
					}
	
					for (int j = 0; j < mHeight; j++)
					{
						if (i < mWidth)
						{
							for (int k = 0; k < points; k++)
							{
								double phi = rnd.NextDouble() * 2 * Math.PI;
								//double alpha = 3;
								double rad = radius * rnd.NextDouble(); //-radius / alpha * Math.Log(rnd.NextDouble() + Math.Exp(-alpha));
							
								int u = i + (int)(rad * Math.Cos(phi));
								int v = j + (int)(rad * Math.Sin(phi));
								
								if (u >= 0 && u < mWidth && v >= 0 && v < mHeight)
								{
									double delta = (light[u, v] - light[i, j]);
									
									double f;
									if (delta > 0)
									{
										f = Math.Log(Math.Abs(delta) + 1);
										// Limiting f
										f = limit_up * (1 - Math.Exp(-f / limit_up));
									}
									else
									{										
										f = Math.Log(Math.Abs(delta) + 1);
										// Limiting f
										f = -limit_down * (1 - Math.Exp(-f / limit_down));
									}
									
									double scale = f / 5;
									
									scale_matrix[u, v] += scale;
									scale_matrix_adds[u, v] ++;
								}
							}
						}
							
						if (i_back >= 0)
						{
							// Scaling amplitudes
							double kcomp;
							if (scale_matrix_adds[i_back, j] == 0)
								kcomp = 1;
							else
								kcomp = Math.Pow(scale_matrix[i_back, j] / scale_matrix_adds[i_back, j] + 1, power);

							r_chan[i_back, j] = r_chan[i_back, j] * (float)kcomp;
							g_chan[i_back, j] = g_chan[i_back, j] * (float)kcomp;
							b_chan[i_back, j] = b_chan[i_back, j] * (float)kcomp;
			
							if (r_chan[i_back, j] > maxlight) r_chan[i_back, j] = (float)maxlight;
							if (g_chan[i_back, j] > maxlight) g_chan[i_back, j] = (float)maxlight;
							if (b_chan[i_back, j] > maxlight) b_chan[i_back, j] = (float)maxlight;
							
						}
					}
				}
			}
		}
		
		public void ApplyTone(Tone tone, double HighlightsInvariance, ProgressReporter callback)
		{
			double maxlight = CalcMaxLight();
			
			for (int i = 0; i < mWidth; i++)
			{
				if (i % REPORT_EVERY_NTH_LINE == 0 && callback != null)
				{
					if (!callback((double)i / mWidth))
					{
						throw new UserCancelException();
					}
				}
				
				for (int j = 0; j < mHeight; j++)
				{
					// calculating current norm
					double light_before = Math.Sqrt(
								  r_chan[i, j] * r_chan[i, j] + 
								  g_chan[i, j] * g_chan[i, j] + 
								  b_chan[i, j] * b_chan[i, j]) / Math.Sqrt(3);
					
					// Calculating color coefficients
					// R2, G2, B2 values depend on light value.
					// For highlights it should exponentially approach 1.
					double kappa = Math.Pow(10, HighlightsInvariance);
					
					double R2 = (1 - tone.R) * Math.Exp(-kappa * (maxlight - light_before)) + tone.R;
					double G2 = (1 - tone.G) * Math.Exp(-kappa * (maxlight - light_before)) + tone.G;
					double B2 = (1 - tone.B) * Math.Exp(-kappa * (maxlight - light_before)) + tone.B;
					
					// Applying toning
					r_chan[i, j] *= (float)(R2);
					g_chan[i, j] *= (float)(G2);
					b_chan[i, j] *= (float)(B2);
					
					// calculating norm after
					double light_after = Math.Sqrt(
								  r_chan[i, j] * r_chan[i, j] + 
								  g_chan[i, j] * g_chan[i, j] + 
								  b_chan[i, j] * b_chan[i, j]) / Math.Sqrt(3) + 0.00001;
					
					// Normalizing
					r_chan[i, j] *= (float)(light_before / light_after);
					g_chan[i, j] *= (float)(light_before / light_after);
					b_chan[i, j] *= (float)(light_before / light_after);
					
				}
			}
		}
			
		public void ApplySaturation(double satur_factor, ProgressReporter callback)
		{
			for (int i = 0; i < mWidth; i++)
			{
				if (i % REPORT_EVERY_NTH_LINE == 0 && callback != null)
				{
					if (!callback((double)i / mWidth))
					{
						throw new UserCancelException();
					}
				}
				
				for (int j = 0; j < mHeight; j++)
				{
					double light_sqr = r_chan[i, j] * r_chan[i, j] + 
								  			 g_chan[i, j] * g_chan[i, j] + 
					                         b_chan[i, j] * b_chan[i, j];
					double val = Math.Sqrt(light_sqr / 3);
					
					// Normalizing image
					r_chan[i, j] = (float)(r_chan[i, j] * satur_factor + val * (1 - satur_factor));
					g_chan[i, j] = (float)(g_chan[i, j] * satur_factor + val * (1 - satur_factor));
					b_chan[i, j] = (float)(b_chan[i, j] * satur_factor + val * (1 - satur_factor));
				}
			}
		}
		
		public void CutBlackPoint(double black, ProgressReporter callback)
		{
			double max_light = CalcMaxLight();
			
			black *= max_light;
			
			for (int j = 0; j < mHeight; j++)
			{
				if (j % REPORT_EVERY_NTH_LINE == 0 && callback != null)
				{
					if (!callback((double)j / this.mHeight)) return;
				}
				
				for (int i = 0; i < mWidth; i++)
				{
					double light = Math.Sqrt(
									r_chan[i, j] * r_chan[i, j] + 
								  	g_chan[i, j] * g_chan[i, j] + 
					                b_chan[i, j] * b_chan[i, j]) / Math.Sqrt(3);
					
					Tone curtone = new Tone(r_chan[i, j], g_chan[i, j], b_chan[i, j]);
					
					double newlight = light - black;
					if (newlight < 0) newlight = 0;
					
					r_chan[i, j] = (float)(curtone.R * newlight);
					g_chan[i, j] = (float)(curtone.G * newlight);
					b_chan[i, j] = (float)(curtone.B * newlight);
				}
			}
		}
				
		public bool Crotate(double beta, Point c, int crop_w, int crop_h, int quality, ProgressReporter callback)
		{
			beta *= Math.PI / 180.0;
			
			// Creating new image
			float[,] newr = new float[crop_w, crop_h];
			float[,] newg = new float[crop_w, crop_h];
			float[,] newb = new float[crop_w, crop_h];
			
			
			// Going thru new pixels. Calculating influence from source pixel
			// colors to new pixel colors
			
			for (int n = 0; n < mHeight; n++)
			{
				if (n % REPORT_EVERY_NTH_LINE == 0 && callback != null)
				{
					if (!callback((double)n / mHeight)) return false;
				}
				
				for (int m = 0; m < mWidth; m++)
				{
					// Rotated source matrix squares
					CatEye.Core.Point[] src_tr_pts = new CatEye.Core.Point[]
					{
						Point.Rotate(new CatEye.Core.Point(m,       n      ), -beta, c),
						Point.Rotate(new CatEye.Core.Point((m + 1), n      ), -beta, c),
						Point.Rotate(new CatEye.Core.Point((m + 1), (n + 1)), -beta, c),
						Point.Rotate(new CatEye.Core.Point(m,       (n + 1)), -beta, c)
					};
					
					// Rotated and translated source matrix squares
					CatEye.Core.Point[] src_tr_pts2 = new CatEye.Core.Point[]
					{
						new Point(src_tr_pts[0].X - c.X + (double)crop_w / 2, src_tr_pts[0].Y - c.Y + (double)crop_h / 2),
						new Point(src_tr_pts[1].X - c.X + (double)crop_w / 2, src_tr_pts[1].Y - c.Y + (double)crop_h / 2),
						new Point(src_tr_pts[2].X - c.X + (double)crop_w / 2, src_tr_pts[2].Y - c.Y + (double)crop_h / 2),
						new Point(src_tr_pts[3].X - c.X + (double)crop_w / 2, src_tr_pts[3].Y - c.Y + (double)crop_h / 2),
					};
					
					ConvexPolygon cp_src_tr = new ConvexPolygon(src_tr_pts2);
					
					int xmin = Math.Max((int)cp_src_tr.XMin, 0);
					int ymin = Math.Max((int)cp_src_tr.YMin, 0);
					int xmax = Math.Min((int)cp_src_tr.XMax + 1, crop_w);
					int ymax = Math.Min((int)cp_src_tr.YMax + 1, crop_h);
					
					double bloha = 0.00001;
					
					for (int j = ymin; j < ymax; j++)
					{
						for (int i = xmin; i < xmax; i++)
						{
							double part = 0;
							for (int qx = 0; qx < quality; qx++)
								for (int qy = 0; qy < quality; qy++)
							{
								double px = i + bloha + (double)qx / quality;
								double py = j + bloha + (double)qy / quality;
								if (cp_src_tr.Contains(new Point(px, py)))
								{
									part += 1.0;
								}
							}
							part /= quality * quality;
							
							// Adding colors part
							newr[i, j] += (float)(r_chan[m, n] * part);
							newg[i, j] += (float)(g_chan[m, n] * part);
							newb[i, j] += (float)(b_chan[m, n] * part);
							
						}
					}
				}
			}
			
			r_chan = newr;
			g_chan = newg;
			b_chan = newb;
			mWidth = crop_w; mHeight = crop_h;
			return true;
		}
		
		public bool Resize(int targetWidth, int targetHeight, int quality, 
			ProgressReporter callback)
		{
			// Calculating new picture's real dimensions
			double kx = 1, ky = 1;
			
			// Scaling coefficients:
			kx = (double)targetWidth / mWidth;
			ky = (double)targetHeight / mHeight;

			// Creating new image
			float[,] newr = new float[targetWidth, targetWidth];
			float[,] newg = new float[targetWidth, targetWidth];
			float[,] newb = new float[targetWidth, targetWidth];
			
			
			// Going thru new pixels. Calculating influence from source pixel
			// colors to new pixel colors
			for (int n = 0; n < mHeight; n++)
			{
				if (n % REPORT_EVERY_NTH_LINE == 0 && callback != null)
				{
					if (!callback((double)n / mHeight)) return false;
				}
				
				for (int m = 0; m < mWidth; m++)
				{
					// Transformed source matrix squares
					CatEye.Core.Point[] src_tr_pts = new CatEye.Core.Point[]
					{
						new CatEye.Core.Point(kx * m,       ky * n),
						new CatEye.Core.Point(kx * (m + 1), ky * n),
						new CatEye.Core.Point(kx * (m + 1), ky * (n + 1)),
						new CatEye.Core.Point(kx * m,       ky * (n + 1))
					};
					
					ConvexPolygon cp_src_tr = new ConvexPolygon(src_tr_pts);
					
					int xmin = Math.Max((int)cp_src_tr.XMin, 0);
					int ymin = Math.Max((int)cp_src_tr.YMin, 0);
					int xmax = Math.Min((int)cp_src_tr.XMax + 1, targetWidth);
					int ymax = Math.Min((int)cp_src_tr.YMax + 1, targetHeight);
					
					for (int j = ymin; j < ymax; j++)
					{
						for (int i = xmin; i < xmax; i++)
						{
							double part = 0;
							for (int qx = 0; qx < quality; qx++)
								for (int qy = 0; qy < quality; qy++)
							{
								double px = i + qx / quality;
								double py = j + qy / quality;
								if (cp_src_tr.Contains(new Point(px, py)))
								{
									part += 1.0 / quality / quality;
								}
							}
							
							// Adding colors part
							newr[i, j] += (float)(r_chan[m, n] * part);
							newg[i, j] += (float)(g_chan[m, n] * part);
							newb[i, j] += (float)(b_chan[m, n] * part);
						}
					}
				}
			}
			
			r_chan = newr;
			g_chan = newg;
			b_chan = newb;
			mWidth = targetWidth; mHeight = targetHeight;
			return true;
		}
	}
}

