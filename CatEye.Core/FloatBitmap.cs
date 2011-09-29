using System;
using System.Threading;

namespace CatEye.Core
{
	
	
	public class FloatBitmap : IBitmapCore
	{
		protected const int REPORT_EVERY_NTH_LINE = 5;
		
		protected float[,] r_chan, g_chan, b_chan, hl_chan;
		
		protected int mWidth, mHeight;
		
		public int Width { get { return mWidth; } }
		public int Height { get { return mHeight; } }

		public float[,] RedChannel
		{
			get { return r_chan; }
		}
		public float[,] GreenChannel
		{
			get { return g_chan; }
		}
		public float[,] BlueChannel
		{
			get { return b_chan; }
		}
		
		protected FloatBitmap ()
		{
			
		}
		
		protected void CopyDataFrom(FloatBitmap src)
		{
			mWidth = src.mWidth; mHeight = src.mHeight;
			r_chan = new float[mWidth, mHeight];
			g_chan = new float[mWidth, mHeight];
			b_chan = new float[mWidth, mHeight];
			hl_chan = new float[mWidth, mHeight];
			
			for (int i = 0; i < mWidth; i++)
			for (int j = 0; j < mHeight; j++)
			{
				r_chan[i, j] = src.r_chan[i, j];
				g_chan[i, j] = src.g_chan[i, j];
				b_chan[i, j] = src.b_chan[i, j];
				hl_chan[i, j] = src.hl_chan[i, j];
			}
		}
		
		public FloatBitmap (FloatBitmap src)
		{
			CopyDataFrom(src);
		}
		
		public virtual object Clone ()
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
			hl_chan = new float[mWidth, mHeight];
			
			for (int i = 0; i < mWidth; i++)
			{
				if (i % REPORT_EVERY_NTH_LINE == 0 && callback != null) 
				{
					if (!callback((double)i / mWidth / 2)) 
						return false;
				}
				lock (this)
				{
					for (int j = 0; j < mHeight; j++)
					{
						r_chan[i, j] = ppm.RChannel[i, j];
						g_chan[i, j] = ppm.GChannel[i, j];
						b_chan[i, j] = ppm.BChannel[i, j];
					}
				}
			}
			
			double maxLight = CalcMaxLight();
			
			// Normalizing to 0..1
			for (int i = 0; i < mWidth; i++)
			{
				if (i % REPORT_EVERY_NTH_LINE == 0 && callback != null) 
				{
					if (!callback(0.5 + (double)i / mWidth / 2)) 
						return false;
				}
				lock (this)
				{
					for (int j = 0; j < mHeight; j++)
					{
						r_chan[i, j] /= (float)maxLight;
						g_chan[i, j] /= (float)maxLight;
						b_chan[i, j] /= (float)maxLight;
					}
				}
			}

			return true;
		}

		public void ScaleFast(double k, ProgressReporter callback)
		{
			float[,] new_r = new float[(int)(mWidth * k), (int)(mHeight * k)];
			float[,] new_g = new float[(int)(mWidth * k), (int)(mHeight * k)];
			float[,] new_b = new float[(int)(mWidth * k), (int)(mHeight * k)];
			float[,] new_hl = new float[(int)(mWidth * k), (int)(mHeight * k)];
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
					double r = 0, g = 0, b = 0, hl = 0; int s = 0;
					for (int u = (int)((double)i / k); u < (int)(((double)i + 1) / k); u++)
					for (int v = (int)((double)j / k); v < (int)(((double)j + 1) / k); v++)
					{
						if (u < mWidth && v < mHeight)
						{
							r += r_chan[u, v];
							g += g_chan[u, v];
							b += b_chan[u, v];
							hl += hl_chan[u, v];
							s ++;
						}
					}
					new_r[i, j] = (float)r;
					new_g[i, j] = (float)g;
					new_b[i, j] = (float)b;
					new_hl[i, j] = (float)hl;
					sum[i, j] = s;
				}
			}
			
			for (int i = 0; i < (int)(mWidth * k); i++)
				for (int j = 0; j < (int)(mHeight * k); j++)
			{
				new_r[i, j] /= sum[i, j];
				new_g[i, j] /= sum[i, j];
				new_b[i, j] /= sum[i, j];
				new_hl[i, j] /= sum[i, j];
			}
			
			lock (this)
			{
				r_chan = new_r;
				g_chan = new_g;
				b_chan = new_b;
				hl_chan = new_hl;
				mWidth = (int)(mWidth * k);
				mHeight = (int)(mHeight * k);
			}
		}
		
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
				lock (this)
				{
					for (int j = 0; j < mHeight; j++)
					{
						r_chan[i, j] *= (float)Amplitude;
						g_chan[i, j] *= (float)Amplitude;
						b_chan[i, j] *= (float)Amplitude;
					}
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
				                               b_chan[i, j] * b_chan[i, j]) / Math.Sqrt(3);
				
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
			{
				lock (this)
				{
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
		
		public void CompressLight(double curve, ProgressReporter callback)
		{
			double a, b;

			if (curve > 0)
			{
				a = Math.Log(2);
				b = Math.Log(1.0 + Math.Pow(100, Math.Abs(curve * 1.5)));
			}
			else
			{
				b = Math.Log(2);
				a = Math.Log(1.0 + Math.Pow(100, Math.Abs(curve * 1.5)));
			}
			
			double p = Math.Pow(100, curve * 1.5);

			for (int i = 0; i < mWidth; i++)
			{
				if (i % REPORT_EVERY_NTH_LINE == 0 && callback != null)
				{
					if (!callback((double)i / mWidth))
					{
						throw new UserCancelException();
					}
				}
				lock (this)
				{
					for (int j = 0; j < mHeight; j++)
					{
						double light = Math.Sqrt(r_chan[i, j] * r_chan[i, j] +
						                         g_chan[i, j] * g_chan[i, j] +
						                         b_chan[i, j] * b_chan[i, j]) / Math.Sqrt(3);
						
						double light_new = Math.Log(p * (Math.Exp(a * light) - 1.0) + 1.0) / b;
							
						double bloha = 0.001;
						r_chan[i, j] *= (float)(light_new / (light + bloha)); 
						g_chan[i, j] *= (float)(light_new / (light + bloha)); 
						b_chan[i, j] *= (float)(light_new / (light + bloha)); 
					}
				}
			}
		}
		
		class thread_data 
		{
			public int i1, i2;
		}
		
		public unsafe void SharpenLight(double radius_part, double pressure, double contrast, int points, ProgressReporter callback)
		{
			float[,] light = new float[mWidth, mHeight];
			double maxlight = 0;
	
			// Сalculating light
			for (int i = 0; i < mWidth; i++)
			for (int j = 0; j < mHeight; j++)
			{
				light[i, j] = (float)(Math.Sqrt(r_chan[i, j] * r_chan[i, j] + 
				                                g_chan[i, j] * g_chan[i, j] + 
				                                b_chan[i, j] * b_chan[i, j]) / Math.Sqrt(3));
				if (light[i, j] > maxlight) maxlight = light[i, j];
				
			}
						
			float[,] scale_matrix = new float[mWidth, mHeight];
			float[,] dispersion_matrix = new float[mWidth, mHeight];
				
			int radius = (int)((mWidth + mHeight) / 2 * radius_part + 1);
			
			// Full progress
			int full_i = 0;
			object full_i_lock = new object();
			
			// Initializing threads
			int threads_num = 6;
			Thread[] threads = new Thread[threads_num];
			
			bool user_cancel = false;
			
			for (int q = 0; q < threads_num; q++)
			{
				threads[q] = new Thread(delegate (object obj)
				{
					try
					{
						Random rnd = new Random();
			
						int i1 = ((thread_data)obj).i1;
						int i2 = ((thread_data)obj).i2;
									
						for (int i = i1; i < i2; i++)
						{
							if (callback != null)
							{
								if (!callback((double)full_i / (mWidth)))
									throw new UserCancelException();
							}
			
							for (int j = 0; j < mHeight; j++)
							{
								if (i < mWidth)
								{
									// Dispersion
									int avg = 0;
									for (int k = 0; k < points; k++)
									{
										double phi = rnd.NextDouble() * 2 * Math.PI;
										//double alpha = 3;
										double rad = radius * rnd.NextDouble(); //-radius / alpha * Math.Log(rnd.NextDouble() + Math.Exp(-alpha));
									
										int u = i + (int)(rad * Math.Cos(phi));
										int v = j + (int)(rad * Math.Sin(phi));
										
										if (u >= 0 && u < mWidth && v >= 0 && v < mHeight)
										{
											double delta = (light[i, j] - light[u, v]);
											dispersion_matrix[i, j] += (float)(delta * delta);
											avg ++;
										}
									}
									dispersion_matrix[i, j] = (float)(Math.Sqrt(dispersion_matrix[i, j] / (avg + 1)));  // (avg + 1) to avoid div by zero
			
									// Average
									avg = 0;
									for (int k = 0; k < points; k++)
									{
										double phi = rnd.NextDouble() * 2 * Math.PI;
										//double alpha = 3;
										double rad = radius * rnd.NextDouble(); //-radius / alpha * Math.Log(rnd.NextDouble() + Math.Exp(-alpha));
									
										int u = i + (int)(rad * Math.Cos(phi));
										int v = j + (int)(rad * Math.Sin(phi));
										
										if (u >= 0 && u < mWidth && v >= 0 && v < mHeight)
										{
											double delta = (light[i, j] - light[u, v]);
											double f = Math.Log(Math.Abs(delta) + 1);

											// Limiting f to remove white and dark "crowns" near contrast objects
											//double d = 2, K = 0.5; -- good
											double K = 2.5 * contrast;
											//double denoise_delta = 0.05, denoise_min = 0.2;
											//double denoiser = (1 - Math.Exp(-dispersion_matrix[i, j] / denoise_delta)) * (1 - denoise_min) + denoise_min;
											double limit = /*denoiser **/ 0.01 * Math.Exp(-dispersion_matrix[i, j] * dispersion_matrix[i, j] / (contrast * contrast)) * 
												(K / (Math.Sqrt(dispersion_matrix[i, j] + K*K) + 0.0001)) + 0.0001;
											
											f = limit * (1 - Math.Exp(-f / limit)) * Math.Sign(delta);
			
											double scale = f;	// It was f / 5
											
											scale_matrix[i, j] += (float)scale;
											avg ++;
										}
									}
									scale_matrix[i, j] /= avg + 1;	// (avg + 1) to avoid div by zero
								}
								
								// Scaling amplitudes
								double kcomp;
								kcomp = Math.Pow(scale_matrix[i, j] + 1, pressure);
			
								lock (this)
								{
									r_chan[i, j] = r_chan[i, j] * (float)kcomp;
									g_chan[i, j] = g_chan[i, j] * (float)kcomp;
									b_chan[i, j] = b_chan[i, j] * (float)kcomp;
								}
								
							}
							lock (full_i_lock) full_i ++;						
						}
					}
					catch (UserCancelException)
					{
						user_cancel = true;
					}						
				});
			}
			
			// Starting threads
			for (int q = 0; q < threads_num; q++)
			{
				thread_data td = new thread_data();
				td.i1 = (mWidth / threads_num) * q;
				td.i2 = (mWidth / threads_num) * (q + 1);
				
				threads[q].Start(td);
			}
			
			// Waiting for threads
			for (int q = 0; q < threads_num; q++)
			{
				threads[q].Join();
			}
			
			if (user_cancel) throw new UserCancelException();
			
		}
		
		public Tone FindLightTone(Tone dark_tone, double edge, double softness, Point light_center, double light_radius, int points)
		{
			int i = (int)(light_center.X * Width);
			int j = (int)(light_center.Y * Height);
			
			// Selecting points for analysis
			
			Random rnd = new Random();
			int n = 0;
			
			Color[] sel = new Color[points];
			
			for (int p = 0; p < points; p++)
			{
				double phi = rnd.NextDouble() * 2 * Math.PI;
				double rad = light_radius * rnd.NextDouble();
			
				int u = i + (int)(rad * Math.Cos(phi));
				int v = j + (int)(rad * Math.Sin(phi));
				
				if (u >= 0 && u < mWidth && v >= 0 && v < mHeight)
				{
					sel[n] = new Color(r_chan[u, v], g_chan[u, v], b_chan[u, v]);
					n++;
				}
			}
			
			// Searching for minimum tone distance to (1, 1, 1)
			int steps_count = 1000;
			double dc = 0.001;
			Tone cur_light_tone = new Tone(1, 1, 1);
			
			for (int step = 0; step < steps_count; step++)
			{
				// Calculationg distance from current point to "gray" Tone(1, 1, 1)
				double r0 = 0, g0 = 0, b0 = 0;
				for (int p = 0; p < n; p++)
				{
					Color changed = sel[p].ApplyDualToning(dark_tone, cur_light_tone, softness, edge);
					
					r0 += changed.R;
					g0 += changed.G;
					b0 += changed.B;
				}
				double dist0 = Tone.Distance(new Tone(r0, g0, b0), new Tone(1, 1, 1));
				
				// Calculationg gradient
				// Red shift
				double Pdist_dR;
				//if (cur_light_tone.R + dc < 1) 
				{
					Tone cur_light_tone_dR = new Tone(cur_light_tone.R + dc, cur_light_tone.G, cur_light_tone.B);
				
					double r_dR = 0, g_dR = 0, b_dR = 0;
					for (int p = 0; p < n; p++)
					{
						Color changed = sel[p].ApplyDualToning(dark_tone, cur_light_tone_dR, softness, edge);
						
						r_dR += changed.R;
						g_dR += changed.G;
						b_dR += changed.B;
					}
					double dist_dR = Tone.Distance(new Tone(r_dR, g_dR, b_dR), new Tone(1, 1, 1));
					Pdist_dR = (dist_dR - dist0) / dc;
				}
				//else
				//	Pdist_dR = 0;
	
				// Green shift
				double Pdist_dG;
				//if (cur_light_tone.G + dc < 1) 
				{
					Tone cur_light_tone_dG = new Tone(cur_light_tone.R, cur_light_tone.G + dc, cur_light_tone.B);
				
					double r_dG = 0, g_dG = 0, b_dG = 0;
					for (int p = 0; p < n; p++)
					{
						Color changed = sel[p].ApplyDualToning(dark_tone, cur_light_tone_dG, softness, edge);
						
						r_dG += changed.R;
						g_dG += changed.G;
						b_dG += changed.B;
					}
					double dist_dG = Tone.Distance(new Tone(r_dG, g_dG, b_dG), new Tone(1, 1, 1));
					Pdist_dG = (dist_dG - dist0) / dc;
				}
				//else
				//	Pdist_dG = 0;
	
				// Blue shift
				double Pdist_dB;
				//if (cur_light_tone.B + dc < 1) 
				{
					Tone cur_light_tone_dB = new Tone(cur_light_tone.R, cur_light_tone.G, cur_light_tone.B + dc);
				
					double r_dB = 0, g_dB = 0, b_dB = 0;
					for (int p = 0; p < n; p++)
					{
						Color changed = sel[p].ApplyDualToning(dark_tone, cur_light_tone_dB, softness, edge);
						
						r_dB += changed.R;
						g_dB += changed.G;
						b_dB += changed.B;
					}
					double dist_dB = Tone.Distance(new Tone(r_dB, g_dB, b_dB), new Tone(1, 1, 1));
					Pdist_dB = (dist_dB - dist0) / dc;
				}
				//else
				//	Pdist_dB = 0;
				
				// Moving up the gradient
				double newR = cur_light_tone.R - Pdist_dR * dc;
				double newG = cur_light_tone.G - Pdist_dG * dc;
				double newB = cur_light_tone.B - Pdist_dB * dc;
				
				newR = Math.Max(newR, 0);
				newG = Math.Max(newG, 0);
				newB = Math.Max(newB, 0);
				
				cur_light_tone = new Tone(newR, newG, newB);
			}
			
			return cur_light_tone;
		}
		
		public Tone FindDarkTone(Tone light_tone, double edge, double softness, Point dark_center, double dark_radius, int points)
		{
			int i = (int)(dark_center.X * Width);
			int j = (int)(dark_center.Y * Height);
			
			// Selecting points for analysis
			
			Random rnd = new Random();
			int n = 0;
			
			Color[] sel = new Color[points];
			
			for (int p = 0; p < points; p++)
			{
				double phi = rnd.NextDouble() * 2 * Math.PI;
				double rad = dark_radius * rnd.NextDouble();
			
				int u = i + (int)(rad * Math.Cos(phi));
				int v = j + (int)(rad * Math.Sin(phi));
				
				if (u >= 0 && u < mWidth && v >= 0 && v < mHeight)
				{
					sel[n] = new Color(r_chan[u, v], g_chan[u, v], b_chan[u, v]);
					n++;
				}
			}
			
			// Searching for minimum tone distance to (1, 1, 1)
			int steps_count = 1000;
			double dc = 0.001;
			Tone cur_dark_tone = new Tone(1, 1, 1);
			
			for (int step = 0; step < steps_count; step++)
			{
				// Calculationg distance from current point to "gray" Tone(1, 1, 1)
				double r0 = 0, g0 = 0, b0 = 0;
				for (int p = 0; p < n; p++)
				{
					Color changed = sel[p].ApplyDualToning(cur_dark_tone, light_tone, softness, edge);
					
					r0 += changed.R;
					g0 += changed.G;
					b0 += changed.B;
				}
				double dist0 = Tone.Distance(new Tone(r0, g0, b0), new Tone(1, 1, 1));
				
				// Calculationg gradient
				// Red shift
				double Pdist_dR;
				//if (cur_dark_tone.R + dc < 1) 
				{
					Tone cur_dark_tone_dR = new Tone(cur_dark_tone.R + dc, cur_dark_tone.G, cur_dark_tone.B);
				
					double r_dR = 0, g_dR = 0, b_dR = 0;
					for (int p = 0; p < n; p++)
					{
						Color changed = sel[p].ApplyDualToning(cur_dark_tone_dR, light_tone, softness, edge);
						
						r_dR += changed.R;
						g_dR += changed.G;
						b_dR += changed.B;
					}
					double dist_dR = Tone.Distance(new Tone(r_dR, g_dR, b_dR), new Tone(1, 1, 1));
					Pdist_dR = (dist_dR - dist0) / dc;
				}
				//else
				//	Pdist_dR = 0;
	
				// Green shift
				double Pdist_dG;
				//if (cur_dark_tone.G + dc < 1) 
				{
					Tone cur_dark_tone_dG = new Tone(cur_dark_tone.R, cur_dark_tone.G + dc, cur_dark_tone.B);
				
					double r_dG = 0, g_dG = 0, b_dG = 0;
					for (int p = 0; p < n; p++)
					{
						Color changed = sel[p].ApplyDualToning(cur_dark_tone_dG, light_tone, softness, edge);
						
						r_dG += changed.R;
						g_dG += changed.G;
						b_dG += changed.B;
					}
					double dist_dG = Tone.Distance(new Tone(r_dG, g_dG, b_dG), new Tone(1, 1, 1));
					Pdist_dG = (dist_dG - dist0) / dc;
				}
				//else
				//	Pdist_dG = 0;
	
				// Blue shift
				double Pdist_dB;
				//if (cur_dark_tone.B + dc < 1) 
				{
					Tone cur_dark_tone_dB = new Tone(cur_dark_tone.R, cur_dark_tone.G, cur_dark_tone.B + dc);
				
					double r_dB = 0, g_dB = 0, b_dB = 0;
					for (int p = 0; p < n; p++)
					{
						Color changed = sel[p].ApplyDualToning(cur_dark_tone_dB, light_tone, softness, edge);
						
						r_dB += changed.R;
						g_dB += changed.G;
						b_dB += changed.B;
					}
					double dist_dB = Tone.Distance(new Tone(r_dB, g_dB, b_dB), new Tone(1, 1, 1));
					Pdist_dB = (dist_dB - dist0) / dc;
				}
				//else
				//	Pdist_dB = 0;
				
				// Moving up the gradient
				double newR = cur_dark_tone.R - Pdist_dR * dc;
				double newG = cur_dark_tone.G - Pdist_dG * dc;
				double newB = cur_dark_tone.B - Pdist_dB * dc;
				
				newR = Math.Max(newR, 0);
				newG = Math.Max(newG, 0);
				newB = Math.Max(newB, 0);
				
				cur_dark_tone = new Tone(newR, newG, newB);
			}
			
			return cur_dark_tone;
		}

		public void ApplyTone(Tone dark_tone, Tone light_tone, double edge, double softness, ProgressReporter callback)
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
					Color res_ij = new Color(r_chan[i, j], g_chan[i, j], b_chan[i, j]).ApplyDualToning(dark_tone, light_tone, softness, edge);
				
					lock (this)
					{
						r_chan[i, j] = (float)(res_ij.R);
						g_chan[i, j] = (float)(res_ij.G);
						b_chan[i, j] = (float)(res_ij.B);
					}
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
				
				lock (this)
				{
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
		}
		
		public void CutBlackPoint(double cut, ProgressReporter callback)
		{
			double max_light = CalcMaxLight();
			double min_light = AmplitudeFindBlackPoint();
			
			if (cut < 0.00001)
			{
				cut = (cut + 1) * min_light;	
			}
			else 
			{
				cut = min_light + (max_light - min_light) * cut;
			}
			
			
			lock (this)
			{
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
						
						double newlight = light - cut;
						if (newlight < 0) newlight = 0;
						
						r_chan[i, j] = (float)(curtone.R * newlight);
						g_chan[i, j] = (float)(curtone.G * newlight);
						b_chan[i, j] = (float)(curtone.B * newlight);
					}
				}
			}
		}
				
		public void Crotate(double beta, Point c, int crop_w, int crop_h, int quality, ProgressReporter callback)
		{
			beta *= Math.PI / 180.0;
			
			float[,] oldr = r_chan; 
			float[,] oldg = g_chan; 
			float[,] oldb = b_chan;
			float[,] oldhl = hl_chan;
			int oldW = mWidth, oldH = mHeight;
			
			// Creating new image
			lock (this)
			{
				r_chan = new float[crop_w, crop_h];
				g_chan = new float[crop_w, crop_h];
				b_chan = new float[crop_w, crop_h];
				hl_chan = new float[crop_w, crop_h];
				mWidth = crop_w; mHeight = crop_h;
			}
			
			// Full progress
			int full_n = 0;
			object full_n_lock = new object();
			
			// Initializing threads
			int threads_num = 6;
			Thread[] threads = new Thread[threads_num];
			
			bool user_cancel = false;
			
			for (int q = 0; q < threads_num; q++)
			{
				threads[q] = new Thread(delegate (object obj)
				{
					try
					{
						int n1 = ((thread_data)obj).i1;
						int n2 = ((thread_data)obj).i2;
									
						for (int n = n1; n < n2; n++)
						{
			
							if (n % REPORT_EVERY_NTH_LINE == 0 && callback != null)
							{
								if (!callback((double)full_n / oldH)) throw new UserCancelException();
							}
							
							for (int m = 0; m < oldW; m++)
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
								
								for (int j = ymin; j < ymax; j++)
								{
									for (int i = xmin; i < xmax; i++)
									{
										double part = cp_src_tr.CalcProjectionToPixel(i, j, quality);
										
										// Adding colors part
										lock (this)
										{
											r_chan[i, j] += (float)(oldr[m, n] * part);
											g_chan[i, j] += (float)(oldg[m, n] * part);
											b_chan[i, j] += (float)(oldb[m, n] * part);
											hl_chan[i, j] += (float)(oldhl[m, n] * part);
										}
									}
								}
							}
							lock (full_n_lock) full_n ++;						
						}
					}
					catch (UserCancelException)
					{
						user_cancel = true;
					}						
				});
			}
			
			// Starting threads
			for (int q = 0; q < threads_num; q++)
			{
				thread_data td = new thread_data();
				td.i1 = (oldH / threads_num) * q;
				td.i2 = (oldH / threads_num) * (q + 1);
				
				threads[q].Start(td);
			}
			
			// Waiting for threads
			for (int q = 0; q < threads_num; q++)
			{
				threads[q].Join();
			}
			
			if (user_cancel) throw new UserCancelException();
						
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
			float[,] newr = new float[targetWidth, targetHeight];
			float[,] newg = new float[targetWidth, targetHeight];
			float[,] newb = new float[targetWidth, targetHeight];
			float[,] newhl = new float[targetWidth, targetHeight];
			
			
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
							double part = cp_src_tr.CalcProjectionToPixel(i, j, quality);
							
							// Adding colors part
							newr[i, j] += (float)(r_chan[m, n] * part);
							newg[i, j] += (float)(g_chan[m, n] * part);
							newb[i, j] += (float)(b_chan[m, n] * part);
							newhl[i, j] += (float)(hl_chan[m, n] * part);
						}
					}
				}
			}
			
			lock (this)
			{
				r_chan = newr;
				g_chan = newg;
				b_chan = newb;
				hl_chan = newhl;
				mWidth = targetWidth; mHeight = targetHeight;
			}
			return true;
		}
		
		public void CutHighlights(double cut, double softness, int lines, double tailValueAtLeast, ProgressReporter callback)
		{
			double maxlight = CalcMaxLight();
			HistogramCollector sc = new HistogramCollector(maxlight, lines);
			sc.CollectData(this);
			double red_tail = sc.LineToScale(sc.FindHighTailRed(tailValueAtLeast));
			double green_tail = sc.LineToScale(sc.FindHighTailGreen(tailValueAtLeast));
			double blue_tail = sc.LineToScale(sc.FindHighTailBlue(tailValueAtLeast));
			double min_tail = Math.Min(red_tail, Math.Min(green_tail, blue_tail));
			double max_tail = Math.Max(red_tail, Math.Max(green_tail, blue_tail));
			
			Console.WriteLine("min: " + min_tail + "max: " + max_tail);
			
			double max_hl = 0;
			// Building highlights matrix
			double delta = softness;	// Highlight distance
			double alpha = Math.Log(2) / delta;
			double q = min_tail + cut * (max_tail - min_tail);
			for (int j = 0; j < mHeight; j++)
			{
				if (j % REPORT_EVERY_NTH_LINE == 0 && callback != null)
				{
					if (!callback((double)j / mHeight)) throw new UserCancelException();
				}
				
				for (int i = 0; i < mWidth; i++)
				{
					if (r_chan[i, j] > q) r_chan[i, j] = (float)q;
					if (g_chan[i, j] > q) g_chan[i, j] = (float)q;
					if (b_chan[i, j] > q) b_chan[i, j] = (float)q;
					
					double x = Math.Sqrt(r_chan[i, j] * r_chan[i, j] +
					                     g_chan[i, j] * g_chan[i, j] +
					                     b_chan[i, j] * b_chan[i, j]) / Math.Sqrt(3) / q;
					double beta = Math.Log(q) - alpha;
					hl_chan[i, j] = (float)((Math.Exp(alpha * x + beta) - Math.Exp(beta)) * (1 + Math.Exp(beta)));
					if (hl_chan[i, j] > max_hl) max_hl = hl_chan[i, j];
				}
			}	
			Console.WriteLine(max_hl);
		}
	}
}

