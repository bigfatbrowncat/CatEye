using System;
using System.Collections.Generic;
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
		
		private static float[,] Upsample2(float[,] Q, int new_w, int new_h)
		{
			int w = Q.GetLength(0), h = Q.GetLength(1);
			
			float[,] Q2 = new float[new_w, new_h];
			for (int i = 0; i < new_w; i++)
			for (int j = 0; j < new_h; j++)
			{
				int i2 = i;
				if (i2 / 2 >= w) i2 = 2 * w - 1;
				int j2 = j;
				if (j2 / 2 >= h) j2 = 2 * h - 1;
				
				Q2[i, j] = Q[i2 / 2, j2 / 2];
			}
			
			// Blurring
			float[,] Q22 = new float[new_w, new_h];
			for (int i = 0; i < new_w - 1; i++)
			for (int j = 0; j < new_h - 1; j++)
			{
				Q22[i, j] = 0;
				float diver = 0;
				
				float mix_k = 0.5f;
				
				diver += 0.125f * mix_k;
				if (i > 0 && j > 0)
				{
					Q22[i, j] += 0.125f * mix_k * Q2[i - 1, j - 1];
				}
				else
				{
					Q22[i, j] += 0.125f * mix_k * Q2[i, j];
				}
					
				diver += 0.375f * mix_k;
				if (i > 0)
				{
				    Q22[i, j] += 0.25f * mix_k * Q2[i - 1, j] +
				                0.125f * mix_k * Q2[i - 1, j + 1];
				}
				else
				{
				    Q22[i, j] += 0.25f * mix_k * Q2[i, j] +
				                0.125f * mix_k * Q2[i, j + 1];
				}
				
				diver += 0.375f;
				if (j > 0)
				{
				    Q22[i, j] += 0.25f * mix_k * Q2[i, j - 1] +
					            0.125f * mix_k * Q2[i + 1, j - 1];
				}
				else
				{
				    Q22[i, j] += 0.25f * mix_k * Q2[i, j] +
					            0.125f * mix_k * Q2[i + 1, j];
				}
				
				diver += 0.125f * mix_k + 1;
				Q22[i, j] += Q2[i, j] + 
				     0.25f * mix_k * Q2[i + 1, j] +
				     0.25f * mix_k * Q2[i, j + 1] +
				    0.125f * mix_k * Q2[i + 1, j + 1];
				
				Q22[i, j] /= diver;
			}
			return Q22;
		}
		
		private float[,] BuildPhi(float[,] H, double alpha, double beta, double noise_gate)
		{
			int Hw = H.GetLength(0);
			int Hh = H.GetLength(1);
			
			int divides = 0, wt = Hw, ht = Hh;
			while (wt > 64 && ht > 64)
			{
				wt /= 2; ht /= 2;
				divides ++;
			}
			
			// Building H0
			float[,] H_cur = new float[Hw, Hh];
			for (int i = 0; i < Hw; i++)
			for (int j = 0; j < Hh; j++)
			{
				H_cur[i, j] = H[i, j];
			}
			
			// Building phi_k			
			List<float[,]> phi = new List<float[,]>();

			for (int k = 0; k <= divides; k++)	      // k is the index of H_cur
			{
				float avg_grad = 0;

				int w = (int)(Hw / Math.Pow(2, k));
				int h = (int)(Hh / Math.Pow(2, k));
	
				if (k > 0)
				{
					// Calculating the new H_cur
					float[,] H_cur_new = new float[w, h];
					for (int i = 0; i < w; i++)
					for (int j = 0; j < h; j++)
					{
						H_cur_new[i, j] = (float)(0.25 * (H_cur[2 * i, 2 * j]     + H_cur[2 * i + 1, 2 * j] +
						                                  H_cur[2 * i, 2 * j + 1] + H_cur[2 * i + 1, 2 * j + 1]));
					}
					H_cur = H_cur_new;
				}
				
				// Calculating grad_H_cur
				float[,] grad_H_cur_x = new float[w, h];
				float[,] grad_H_cur_y = new float[w, h];
				for (int i = 0; i < w; i++)
				for (int j = 0; j < h; j++)
				{
					if (w == 1 || h == 1)
					{
						grad_H_cur_x[i, j] = 0; 
						grad_H_cur_y[i, j] = 0;
					}
					else
					{
						if (i < w - 1)
							grad_H_cur_x[i, j] = (float)((H_cur[i + 1, j] - H_cur[i, j]) / Math.Pow(2, k));
						else
							grad_H_cur_x[i, j] = (float)((H_cur[i, j] - H_cur[i - 1, j]) / Math.Pow(2, k));
						
						if (j < h - 1)
							grad_H_cur_y[i, j] = (float)((H_cur[i, j + 1] - H_cur[i, j]) / Math.Pow(2, k));
						else
							grad_H_cur_y[i, j] = (float)((H_cur[i, j] - H_cur[i, j - 1]) / Math.Pow(2, k));
						
						avg_grad += (float)Math.Sqrt(grad_H_cur_x[i, j] * grad_H_cur_x[i, j] +
						                             grad_H_cur_y[i, j] * grad_H_cur_y[i, j]);
					}
				}	

				// Calculating phi_k
				avg_grad /= w * h;
				Console.WriteLine("avg_grad = " + avg_grad);
				float[,] phi_k = new float[w, h];
				for (int i = 0; i < w; i++)
				for (int j = 0; j < h; j++)
				{
					double abs_grad_H_cur = Math.Sqrt(grad_H_cur_x[i, j] * grad_H_cur_x[i, j] +
					                                  grad_H_cur_y[i, j] * grad_H_cur_y[i, j]);
					abs_grad_H_cur += 0.001;	// Avoiding zero
					
					phi_k[i, j] = (float)(Math.Pow(abs_grad_H_cur / alpha, beta - 1));
					
					// Noise gate
					//float k0 = 0.01f, nf_edge = 0.005f;
					//float nf_softness = k0 * nf_edge * nf_edge;
					//phi_k[i, j] *= (float)(1.0 / Math.PI * Math.Atan2(nf_softness * abs_grad_H_cur, nf_edge * nf_edge - abs_grad_H_cur * abs_grad_H_cur));
					float nf_edge = (float)(noise_gate * noise_gate);
					phi_k[i, j] *= (float)(1 - Math.Exp(-abs_grad_H_cur * abs_grad_H_cur / nf_edge / nf_edge));
				}
				phi.Add(phi_k);
			}
			
			
			// Building Phi from phi_k
			float[,] Phi = new float[phi[phi.Count - 1].GetLength(0), phi[phi.Count - 1].GetLength(1)];
			for (int i = 0; i < Phi.GetLength(0); i++)
			for (int j = 0; j < Phi.GetLength(1); j++)
				Phi[i, j] = 1;
			
			for (int k = divides; k >= 0; k--)
			{
				int w = phi[k].GetLength(0);
				int h = phi[k].GetLength(1);
				// Multiplying
				for (int i = 0; i < w; i++)
				for (int j = 0; j < h; j++)
				{
					Phi[i, j] *= phi[k][i, j];
				}
				
				if (k > 0)
				{
					Phi = Upsample2(Phi, 2 * w + 1, 2 * h + 1);
				}
			}
			
			// Extracting the correct size
			float[,] Phi_cut = new float[Hw, Hh];
			for (int i = 0; i < Hw; i++)
			for (int j = 0; j < Hh; j++)
			{
				Phi_cut[i, j] = Phi[i, j];
			}
			
			return Phi_cut;
		}
		
		/// <summary>
		/// Solution reporter.
		/// </summary>
		/// <returns>
		/// Should return "true" when searching is completed and "false" to continue the process
		/// </returns>
		private delegate void SolutionReporter(float progress, float[,] solution);
		
		private static float[,] SolvePoissonNeimanMultiLattice(float[,] rho, int steps_max, float stop_dpd, SolutionReporter callback)
		{
			// Making lower resolutions
			int W = rho.GetLength(0);
			int H = rho.GetLength(1);
			
			List<int> ww = new List<int>();
			List<int> hh = new List<int>();
			
			int divides = 0, wt = W, ht = H;
			ww.Add(wt); hh.Add(ht);
			while (wt > 64 && ht > 64)
			{
				wt /= 2; ht /= 2;
				ww.Add(wt); hh.Add(ht);
				divides ++;
			}
			
			List<float[,]> Rho = new List<float[,]>();
			Rho.Add(rho);
			
			for (int p = 1; p <= divides; p++)
			{
				int w = ww[p - 1];
				int h = hh[p - 1];
				
				float[,] rho_new = new float[ww[p], hh[p]];
				for (int i = 0; i < w; i++)
				for (int j = 0; j < h; j++)
				{
					if (i / 2 < ww[p] && j / 2 < hh[p])
						rho_new[i / 2, j / 2] += 0.25f * Rho[p - 1][i, j];
				}
				
				Rho.Add(rho_new);
			}
		
			float[,] I = new float[Rho[divides].GetLength(0), Rho[divides].GetLength(1)];
			float old_progress = 0;
			for (int p = divides; p >= 0; p--)
			{
				SolvePoissonNeiman(I, Rho[p], steps_max, stop_dpd, delegate (float progress, float[,] solution)
				{
					if (callback != null)
					{
						float complete_effort = 0;
						for (int q = divides; q > p; q--)
							complete_effort += (float)ww[q] * hh[q];
						
						float full_effort = complete_effort;
						for (int q = p; q >= 0; q--)
							full_effort += (float)ww[q] * hh[q];
						
						float new_progress = (complete_effort + ww[p] * hh[p] * progress) / full_effort;
						if (new_progress > old_progress) old_progress = new_progress;
						
						callback(old_progress, solution);
					}
					
				});
				
				if (p > 0)
				{
					I = Upsample2(I, ww[p - 1], hh[p - 1]);
				}
			}
			
			return I;
		}
		
		private static bool SolvePoissonNeiman(float[,] I0, float[,] rho, int steps_max, float stop_dpd, SolutionReporter callback)
		{
			int w = rho.GetLength(0), h = rho.GetLength(1);
			float[,] I = new float[w + 2, h + 2];
			float[,] Inew = new float[w + 2, h + 2];
			
			// Setting initial values
			for (int i = 0; i < w + 2; i++)
			for (int j = 0; j < h + 2; j++)
			{
				int i1 = i;
				if (i == 0) i1 = 1;
				if (i == w + 1) i1 = w;
				int j1 = j;
				if (j == 0) j1 = 1;
				if (j == h + 1) j1 = h;
				
				I[i, j] = I0[i1 - 1, j1 - 1];
				Inew[i, j] = I0[i1 - 1, j1 - 1];
			}						
			
			
			float delta = 0; float delta_prev = 10000;
			object delta_lock = new object();
			for (int step = 0; step < steps_max; step ++)
			{
				// *** Horizontal iterations ***
				int threads_num = 4;
				Thread[] threads = new Thread[threads_num];
				
				for (int q = 0; q < threads_num; q++)
				{
					threads[q] = new Thread(delegate (object obj)
					{
						int i1 = ((thread_data)obj).i1;
						int i2 = ((thread_data)obj).i2;
									
						float my_delta = 0;
							
						for (int i = i1; i < i2; i++)
						{
							// Run, Thomas, run!
							float[] alpha = new float[h + 3];
							float[] beta = new float[h + 3];
							
							alpha[1] = 0.25f; beta[1] = 0.25f * (I[i + 1, 0] + Inew[i - 1, 0]);
							for (int j = 1; j < h + 2; j++)
							{
								alpha[j + 1] = 1.0f / (4 - alpha[j]);
								float Fj;
								if (j < h + 1)
									Fj = I[i + 1, j] + Inew[i - 1, j] - 2 * rho[i - 1, j - 1];
								else
									Fj = I[i + 1, j] + Inew[i - 1, j];
								
								beta[j + 1] = (Fj + beta[j]) / (4f - alpha[j]);
							}
							
							Inew[i, h + 1] = beta[h + 2];
							
							for (int j = h; j >= 0; j--)
							{
								double iold = I[i, j];
								Inew[i, j] = alpha[j + 1] * Inew[i, j + 1] + beta[j + 1];
								my_delta += (float)Math.Abs(Inew[i, j] - iold);
							}
						}
						

						lock (delta_lock)
						{
							delta += my_delta;
						}
						
					});
				}
				
				// Starting horizontal threads
				for (int q = 0; q < threads_num; q++)
				{
					thread_data td = new thread_data();
					td.i1 = (w / threads_num) * q + 1;
					if (q < threads_num - 1)
					{
						td.i2 = (w / threads_num) * (q + 1) + 1;
					}
					else
					{
						td.i2 = w + 1;
					}
					
					threads[q].Start(td);
				}
				
				// Waiting for horizontal threads
				for (int q = 0; q < threads_num; q++)
				{
					threads[q].Join();
				}
				
				// Restoring Neiman boundary conditions after horizontal iterations
				for (int i = 0; i < w + 2; i++)
				{
					Inew[i, 0] = Inew[i, 1];
					Inew[i, h + 1] = Inew[i, h];
				}
				for (int j = 0; j < h + 2; j++)
				{
					Inew[0, j] = Inew[1, j];
					Inew[w + 1, j] = Inew[w, j];
				}
				
				// Controlling the constant after horizontal iterations
				float m = 0;
				for (int i = 0; i < w + 2; i++)
				for (int j = 0; j < h + 2; j++)
				{
					m += Inew[i, j];
				}
				m /= (w+2) * (h+2);

				for (int i = 0; i < w + 2; i++)
				for (int j = 0; j < h + 2; j++)
				{
					I[i, j] = Inew[i, j] - m;
				}				
			
				// *** Vertical iterations ***
				threads = new Thread[threads_num];
				
				for (int q = 0; q < threads_num; q++)
				{
					threads[q] = new Thread(delegate (object obj)
					{
						int j1 = ((thread_data)obj).i1;
						int j2 = ((thread_data)obj).i2;
									
						float my_delta = 0;
							
						for (int j = j1; j < j2; j++)
						{
							// Run, Thomas, run!
							float[] alpha = new float[w + 3];
							float[] beta = new float[w + 3];
							
							alpha[1] = 0.25f; beta[1] = 0.25f * (I[0, j + 1] + Inew[0, j - 1]);
							for (int i = 1; i < w + 2; i++)
							{
								alpha[i + 1] = 1.0f / (4 - alpha[i]);
								float Fi;
								if (i < w + 1)
									Fi = I[i, j + 1] + Inew[i, j - 1] - 2 * rho[i - 1, j - 1];
								else
									Fi = I[i, j + 1] + Inew[i, j - 1];
								
								beta[i + 1] = (Fi + beta[i]) / (4f - alpha[i]);
							}
							
							Inew[w + 1, j] = beta[w + 2];
							
							for (int i = w; i >= 0; i--)
							{
								double iold = I[i, j];
								Inew[i, j] = alpha[i + 1] * Inew[i + 1, j] + beta[i + 1];
								my_delta += (float)Math.Abs(Inew[i, j] - iold);
							}
						}
						

						lock (delta_lock)
						{
							delta += my_delta;
						}
						
					});
				}
				
				// Starting vertical threads
				for (int q = 0; q < threads_num; q++)
				{
					thread_data td = new thread_data();
					td.i1 = (h / threads_num) * q + 1;
					if (q < threads_num - 1)
					{
						td.i2 = (h / threads_num) * (q + 1) + 1;
					}
					else
					{
						td.i2 = h + 1;
					}
					
					threads[q].Start(td);
				}
				
				// Waiting for vertical threads
				for (int q = 0; q < threads_num; q++)
				{
					threads[q].Join();
				}				
				
				
				// Restoring Neiman boundary conditions after vertical iterations
				for (int i = 0; i < w + 2; i++)
				{
					Inew[i, 0] = Inew[i, 1];
					Inew[i, h + 1] = Inew[i, h];
				}
				for (int j = 0; j < h + 2; j++)
				{
					Inew[0, j] = Inew[1, j];
					Inew[w + 1, j] = Inew[w, j];
				}
				
				// Controlling the constant after vertical iterations
				m = 0;
				for (int i = 0; i < w + 2; i++)
				for (int j = 0; j < h + 2; j++)
				{
					m += Inew[i, j];
				}
				m /= (w+2) * (h+2);

				for (int i = 0; i < w + 2; i++)
				for (int j = 0; j < h + 2; j++)
				{
					I[i, j] = Inew[i, j] - m;
				}
				
				for (int i = 1; i < w + 1; i++)
				for (int j = 1; j < h + 1; j++)
				{
					I0[i - 1, j - 1] = I[i, j];
				}
				
				delta /= (float)Math.Sqrt(w * h);
				float dpd = Math.Abs(delta - delta_prev) / delta;
				float progress = (float)Math.Min(Math.Pow(stop_dpd / (dpd + 0.000001), 0.78), 0.999);

				if (callback != null)
				{
					callback(progress, I0);
				}
				
				if (Math.Abs(delta - delta_prev) / delta < stop_dpd)
				{
					return true;
				}
				delta_prev = delta;
				delta = 0;
			}
			
			return false;
		}
		
		public unsafe void SharpenLight(double curve, double noise_gate, double pressure, ProgressReporter callback)
		{
			
			float[,] oldr = r_chan; 
			float[,] oldg = g_chan; 
			float[,] oldb = b_chan;
			
			r_chan = new float[mWidth, mHeight];
			g_chan = new float[mWidth, mHeight];
			b_chan = new float[mWidth, mHeight];
			
			for (int i = 0; i < mWidth; i++)
			for (int j = 0; j < mHeight; j++)
			{
				r_chan[i, j] = oldr[i, j];
				g_chan[i, j] = oldg[i, j];
				b_chan[i, j] = oldb[i, j];
			}
			
			int Hw = mWidth;
			int Hh = mHeight;
			float[,] H = new float[Hw, Hh];
	
			// Сalculating logarithmic luminosity
			for (int i = 0; i < Hw; i++)
			for (int j = 0; j < Hh; j++)
			{
				double light = Math.Sqrt((r_chan[i, j] * r_chan[i, j] + 
				                          g_chan[i, j] * g_chan[i, j] + 
				                          b_chan[i, j] * b_chan[i, j]) / 3);
				
				H[i, j] = (float)(Math.Log(light + 0.00001));
			}
			
			float[,] grad_H_x = new float[Hw, Hh];
			float[,] grad_H_y = new float[Hw, Hh];
			
			// Calculating gradient of H
			for (int i = 1; i < Hw - 1; i++)
			for (int j = 1; j < Hh - 1; j++)
			{
				grad_H_x[i, j] = (float)(H[i + 1, j] - H[i, j]);
				grad_H_y[i, j] = (float)(H[i, j + 1] - H[i, j]);
			}
			
			// Calculating Phi
			float[,] Phi = BuildPhi(H, 0.01 * pressure * pressure, 0.8, noise_gate);
			
			
			// Calculating G and div_G
			float[,] div_G = new float[Hw, Hh];
			for (int i = 0; i < Hw - 1; i++)
			for (int j = 0; j < Hh - 1; j++)
			{
				float G_x_ij = grad_H_x[i, j] * Phi[i, j];
				float G_y_ij = grad_H_y[i, j] * Phi[i, j];
				
				float G_x_ip1j = grad_H_x[i + 1, j] * Phi[i + 1, j];
				float G_y_ijp1 = grad_H_y[i, j + 1] * Phi[i, j + 1];
				
				div_G[i, j] = - (G_x_ij - G_x_ip1j + G_y_ij - G_y_ijp1);
			}

			// Preparing the compressor
			
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

			
			// Solving Poisson equation Delta I = div G
			float[,] I = null;
			int step = 0;
			
//			double epsilon = 5e-8f;	// TODO: Should be configured somehow...
//			double epsilon = 0.1f;	// TODO: Should be configured somehow...
			float epsilon = 0.002f;	// TODO: Should be configured somehow...
			
			float delta_prev = 0;
			SolutionReporter srep = delegate (float progress, float[,] solution)
			{
				if (step % 30 == 0 || progress > 0.999)
				{
					double kw = H.GetLength(0) / solution.GetLength(0);
					double kh = H.GetLength(1) / solution.GetLength(1);
					
					// Draw it
					for (int i = 0; i < mWidth; i++)
					for (int j = 0; j < mHeight; j++)
					{
	
						double Lold = Math.Exp(H[i, j]);
						
						double Lcomp = Math.Log(p * (Math.Exp(a * Lold) - 1.0) + 1.0) / b;
						
						int i1 = (int)(Math.Min (i / kw, solution.GetLength(0) - 1));
						int j1 = (int)(Math.Min (j / kh, solution.GetLength(1) - 1));
						
						double L = Math.Exp(solution[i1, j1]) * Lcomp;
						
						r_chan[i, j] = (float)(oldr[i, j] * L / (Lold + 0.00001));
						g_chan[i, j] = (float)(oldg[i, j] * L / (Lold + 0.00001));
						b_chan[i, j] = (float)(oldb[i, j] * L / (Lold + 0.00001));
					}
				}
				step ++;
				
				if (callback != null)
				{
					// This formula is found experimentally
					if (!callback(progress))
						throw new UserCancelException();
				}
			
			};
			
			I = SolvePoissonNeimanMultiLattice(div_G, 20000, epsilon, srep);
			//SolvePoissonNeiman(H, div_G, 20000, epsilon, srep);

			srep(1, I);
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
				double rad = light_radius * (Width + Height) / 2 * rnd.NextDouble();
			
				int u = i + (int)(rad * Math.Cos(phi));
				int v = j + (int)(rad * Math.Sin(phi));
				
				if (u >= 0 && u < mWidth && v >= 0 && v < mHeight)
				{
					sel[n] = new Color(r_chan[u, v], g_chan[u, v], b_chan[u, v]);
					n++;
				}
			}
			
			// Searching for minimum tone distance to (1, 1, 1)
			int steps_count = 100;
			double dc = 0.01;
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
				double delta = 0.01;
				double newR = cur_light_tone.R - Pdist_dR * delta;
				double newG = cur_light_tone.G - Pdist_dG * delta;
				double newB = cur_light_tone.B - Pdist_dB * delta;
				
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
				double rad = dark_radius * (Width + Height) / 2 * rnd.NextDouble();
			
				int u = i + (int)(rad * Math.Cos(phi));
				int v = j + (int)(rad * Math.Sin(phi));
				
				if (u >= 0 && u < mWidth && v >= 0 && v < mHeight)
				{
					sel[n] = new Color(r_chan[u, v], g_chan[u, v], b_chan[u, v]);
					n++;
				}
			}
			
			// Searching for minimum tone distance to (1, 1, 1)
			int steps_count = 100;
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
				double delta = 0.01;
				double newR = cur_dark_tone.R - Pdist_dR * delta;
				double newG = cur_dark_tone.G - Pdist_dG * delta;
				double newB = cur_dark_tone.B - Pdist_dB * delta;
				
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
				if (q < threads_num - 1)
				{
					td.i2 = (oldH / threads_num) * (q + 1);
				}
				else
				{
					td.i2 = oldH;
				}
				
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
					hl_chan[i, j] = (float)((Math.Exp(alpha * x + beta) - Math.Exp(beta)) / (Math.Exp(alpha + beta) - Math.Exp(beta)));
				}
			}
		}
	}
}

