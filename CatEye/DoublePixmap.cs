
using System;

namespace CatEye
{
	public delegate bool ProgressReporter(double progress);
	public class UserCancelException : Exception
	{
	}
	
	public class DoublePixmap
	{
		double[,] r_chan, g_chan, b_chan;
		
		public int width, height;
		
		private DoublePixmap ()
		{
			
		}
		public DoublePixmap (DoublePixmap src)
		{
			width = src.width; height = src.height;
			r_chan = new double[width, height];
			g_chan = new double[width, height];
			b_chan = new double[width, height];
			
			for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				r_chan[i, j] = src.r_chan[i, j];
				g_chan[i, j] = src.g_chan[i, j];
				b_chan[i, j] = src.b_chan[i, j];
			}
			
		}
		
		public static DoublePixmap FromPPM(PPMLoader ppm)
		{
			// Applying inverse hdr function: x = -N * ln[ (N - y) / N ]
			DoublePixmap res = new DoublePixmap();
			
			res.width = ppm.Header.Width;
			res.height = ppm.Header.Height;
			
			res.r_chan = new double[res.width, res.height];
			res.g_chan = new double[res.width, res.height];
			res.b_chan = new double[res.width, res.height];

			for (int i = 0; i < res.width; i++)
			for (int j = 0; j < res.height; j++)
			{
				res.r_chan[i, j] = ppm.RChannel[i, j]; //- N * Math.Log(((double)N - (double)ppm.RChannel[i, j]) / N);
				res.g_chan[i, j] = ppm.GChannel[i, j]; //- N * Math.Log(((double)N - (double)ppm.GChannel[i, j]) / N);
				res.b_chan[i, j] = ppm.BChannel[i, j]; //- N * Math.Log(((double)N - (double)ppm.BChannel[i, j]) / N);
			}
			
			// Scaling res to 0..1
			double Max = res.CalcMaxLight();
			for (int i = 0; i < res.width; i++)
			for (int j = 0; j < res.height; j++)
			{
				res.r_chan[i, j] /= Max;
				res.g_chan[i, j] /= Max;
				res.b_chan[i, j] /= Max;
			}
			
			return res;
		}

		private byte cut(double val)
		{
			if (val > 255) return 255;
			if (val < 0) return 0;
			return (byte)val;
		}
		
		public void Downscale(int k, ProgressReporter callback)
		{
			double[,] new_r = new double[width / k, height / k];
			double[,] new_g = new double[width / k, height / k];
			double[,] new_b = new double[width / k, height / k];

			for (int i = 0; i < width / k; i++)
			{
				if (callback != null)
				{
					if (!callback((double)i / (width / k)))
					{
						throw new UserCancelException();
					}
				}
				for (int j = 0; j < height / k; j++)
				{
					double r = 0, g = 0, b = 0;
					for (int u = i * k; u < i * k + k; u++)
					for (int v = j * k; v < j * k + k; v++)
					{
						if (u < width && v < height)
						{
							r += r_chan[u, v];
							g += g_chan[u, v];
							b += b_chan[u, v];
						}
					}
					r /= (k * k);
					g /= (k * k);
					b /= (k * k);
					
					new_r[i, j] = r;
					new_g[i, j] = g;
					new_b[i, j] = b;
				}
				
			}
			r_chan = new_r;
			g_chan = new_g;
			b_chan = new_b;
			width /= k;
			height /= k;
		}
				
		
		public void ScaleLight(double Amplitude)
		{
			double local_mid = 0;
			double[,] light = new double[width, height];
			
			// Searching minimum
			double local_min = Math.Sqrt(r_chan[0, 0] * r_chan[0, 0] +
				                         g_chan[0, 0] * g_chan[0, 0] +
				                         b_chan[0, 0] * b_chan[0, 0]);
			for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				light[i, j] = Math.Sqrt(r_chan[i, j] * r_chan[i, j] +
				                               g_chan[i, j] * g_chan[i, j] +
				                               b_chan[i, j] * b_chan[i, j]);
				
				if (local_min > light[i, j]) local_min = light[i, j];
			}

			// Scaling black
			DeltaAmplitude(-local_min);

			// Searching median
			for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				light[i, j] = Math.Sqrt(r_chan[i, j] * r_chan[i, j] +
				                               g_chan[i, j] * g_chan[i, j] +
				                               b_chan[i, j] * b_chan[i, j]);
				
				local_mid += light[i, j];
			}
			local_mid /= (double)width * height;
			
			for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				r_chan[i, j] *= Amplitude / local_mid;
				g_chan[i, j] *= Amplitude / local_mid;
				b_chan[i, j] *= Amplitude / local_mid;
			}
		}
		
		public void DeltaAmplitude(double delta)
		{
			for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{				
				double amp = Math.Sqrt(r_chan[i, j] * r_chan[i, j] + 
				                       g_chan[i, j] * g_chan[i, j] + 
				                       b_chan[i, j] * b_chan[i, j]);
	
				// Adding "power" to color amplitude
				if (amp + delta > 0)
				{
					r_chan[i, j] *= (amp + delta) / amp;
					g_chan[i, j] *= (amp + delta) / amp;
					b_chan[i, j] *= (amp + delta) / amp;
				}
				else
				{
					r_chan[i, j] = 0;
					g_chan[i, j] = 0;
					b_chan[i, j] = 0;
				}
			}
		}
		
		public double CalcMaxLight()
		{
			double Max = 0;
			for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				double light = Math.Sqrt(r_chan[i, j] * r_chan[i, j] + 
							  			g_chan[i, j] * g_chan[i, j] + 
						      			b_chan[i, j] * b_chan[i, j]);
				
				if (Max < light) Max = light;
			}
			return Max;
		}
		
		public void CompressLight(double power, double bloha)
		{
	
			for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				double light = Math.Sqrt(r_chan[i, j] * r_chan[i, j] +
				                         g_chan[i, j] * g_chan[i, j] +
				                         b_chan[i, j] * b_chan[i, j]);
				
				r_chan[i, j] = r_chan[i, j] * Math.Pow(1.0 / (light + bloha), power);
				g_chan[i, j] = g_chan[i, j] * Math.Pow(1.0 / (light + bloha), power);
				b_chan[i, j] = b_chan[i, j] * Math.Pow(1.0 / (light + bloha), power);

			}
			
		}
		
		public delegate void PointProcessingDelegate(int u, int v);
		
		public interface ISharpeningSamplingMethod
		{
			void DoSampling(PointProcessingDelegate ppd, int radius);
		}
		
		public class StraightSharpeningSamplingMethod : ISharpeningSamplingMethod
		{
			public void DoSampling (DoublePixmap.PointProcessingDelegate ppd, int radius)
			{
				// Go thru all the square
				for (int u = - radius; u <= radius; u++)
				for (int v = - radius; v <= radius; v++)
				{
					ppd(u, v);
				}
			}

		}
		
		public class MonteCarloSharpeningSamplingMethod : ISharpeningSamplingMethod
		{
			public int SamplesCount;
			public Random rnd;
			public MonteCarloSharpeningSamplingMethod(int samplesCount, Random rnd)
			{
				SamplesCount = samplesCount;
				this.rnd = rnd;
			}
			public void DoSampling (DoublePixmap.PointProcessingDelegate ppd, int radius)
			{
				for (int k = 0; k < SamplesCount; k++)
				{
					int u = (int)(rnd.NextDouble() * 2 * radius - radius);
					int v = (int)(rnd.NextDouble() * 2 * radius - radius);
					
					ppd(u, v);
				}
			}			
		}
		
		public void SharpenLight(double radius_part, double power, double delta_0, ISharpeningSamplingMethod ssm, ProgressReporter callback)
		{
			double[,] light = new double[width, height];
			double Max = CalcMaxLight();
			unsafe {
	
				// Сalculating light
				for (int i = 0; i < width; i++)
				for (int j = 0; j < height; j++)
				{
					light[i, j] = Math.Sqrt(r_chan[i, j] * r_chan[i, j] + 
								  			g_chan[i, j] * g_chan[i, j] + 
							      			b_chan[i, j] * b_chan[i, j]);
				}
			}
			
			double[,] scale_matrix = new double[width, height];
			int[,] scale_matrix_adds = new int[width, height];
			
			int radius = (int)((width + height) / 2 * radius_part + 1);
			
			Console.WriteLine("Calculating scale factors...");
			unsafe {
				for (int i = 0; i < width + radius; i++)	// "radius" added to process all "i_back" values
				{
					int i_back = i - radius;
					if (callback != null)
					{
						if (!callback((double)i / (width + radius)))
							throw new UserCancelException();
					}
	
					for (int j = 0; j < height; j++)
					{
						if (i < width)
						{
							ssm.DoSampling(delegate (int u, int v) {
								u += i; v += j;
								
								double rad = Math.Sqrt((double)((u - i) * (u - i) + (v - j) * (v - j)));
								if (rad > radius) return;
								
								double falloff = Math.Exp(-3 * (rad / radius)) - Math.Exp(-3) + 0.0001;
								
								if (u >= 0 && u < width && v >= 0 && v < height)
								{
									double delta = (light[u, v] - light[i, j]);
									
									double f = Math.Log(Math.Abs(delta / delta_0) + 1) * Math.Sign(delta);
									
									double scale = f * falloff;
									//double limited_scale = Math.Abs(scale) < delta_limit ? scale : delta_limit * Math.Sign(scale);
									
									scale_matrix[u, v] += scale;//limited_scale;
									
									scale_matrix_adds[u, v] ++;
								}
							}, radius);
						}
							
						if (i_back >= 0)
						{
							// Scaling amplitudes
							double kcomp;
							if (scale_matrix_adds[i_back, j] == 0)
								kcomp = 1;
							else
								kcomp = Math.Pow(scale_matrix[i_back, j] / scale_matrix_adds[i_back, j] + 1, power);
							
							r_chan[i_back, j] = r_chan[i_back, j] * kcomp;
							g_chan[i_back, j] = g_chan[i_back, j] * kcomp;
							b_chan[i_back, j] = b_chan[i_back, j] * kcomp;
			
							//if (r_chan[i_back, j] > 0.99999) r_chan[i_back, j] = 0.99999;
							//if (g_chan[i_back, j] > 0.99999) g_chan[i_back, j] = 0.99999;
							//if (b_chan[i_back, j] > 0.99999) b_chan[i_back, j] = 0.99999;
						}
					}
				}
			}
			
		}

		public void ApplyChannelsScale(double r_scale, double g_scale, double b_scale)
		{
			for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				r_chan[i, j] *= r_scale;
				g_chan[i, j] *= g_scale;
				b_chan[i, j] *= b_scale;
			}
				
		}
		
		public void ApplyGreenAngle(double RBtoG_angle)
		{
			for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				double mu = Math.Sqrt(r_chan[i, j] * r_chan[i, j] + b_chan[i, j] * b_chan[i, j]);
				double mu_new = mu * Math.Cos(RBtoG_angle) - g_chan[i, j] * Math.Sin(RBtoG_angle);
				double g_new = g_chan[i, j] * Math.Cos(RBtoG_angle) + mu * Math.Sin(RBtoG_angle);
				
				r_chan[i, j] *= mu_new / mu;
				b_chan[i, j] *= mu_new / mu;
				g_chan[i, j] = g_new;
			}
		}
		
		public void ApplyBtoRAngle(double BtoR_angle)
		{
			for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				double r_new = b_chan[i, j] * Math.Sin(BtoR_angle) + r_chan[i, j] * Math.Cos(BtoR_angle);
				double b_new = b_chan[i, j] * Math.Cos(BtoR_angle) - r_chan[i, j] * Math.Sin(BtoR_angle);
				r_chan[i, j] = r_new;
				b_chan[i, j] = b_new;
			}
		}
			
		public void ApplySaturation(double satur_factor)
		{
			for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				double light_sqr = r_chan[i, j] * r_chan[i, j] + 
							  			 g_chan[i, j] * g_chan[i, j] + 
				                         b_chan[i, j] * b_chan[i, j];
				double val = Math.Sqrt(light_sqr / 3);
				
				// Normalizing image
				r_chan[i, j] = r_chan[i, j] * satur_factor + val * (1 - satur_factor);
				g_chan[i, j] = g_chan[i, j] * satur_factor + val * (1 - satur_factor);
				b_chan[i, j] = b_chan[i, j] * satur_factor + val * (1 - satur_factor);
			}
		}
		
		public unsafe void DrawToPixbuf(Gdk.Pixbuf buf)
		{
			double N = 1;	// This is the norm. It should be equal to the
							// value which means the lightest point of 
							// the screen
			
			int chan = buf.NChannels;
			int w = buf.Width, h = buf.Height, stride = buf.Rowstride;
			
			// counting the maximum channel value
			double max = 0;
			for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				double r = N * (1 - Math.Exp(-r_chan[i, j] / N));
				double g = N * (1 - Math.Exp(-g_chan[i, j] / N));
				double b = N * (1 - Math.Exp(-b_chan[i, j] / N));
				if (r > max) max = r;
				if (g > max) max = g;
				if (b > max) max = b;
			}
			
			byte *cur_row = (byte *)buf.Pixels;
			for (int j = 0; j < h; j++)
			{
				if (j >= this.height)
				{
					break;
				}
				byte *cur_pixel = cur_row;
				for (int i = 0; i < w; i++)
				{
					if (i >= this.width)
					{
						break;
					}
					
					double r = N * (1 - Math.Exp(-r_chan[i, j] / N));
					double g = N * (1 - Math.Exp(-g_chan[i, j] / N));
					double b = N * (1 - Math.Exp(-b_chan[i, j] / N));
					
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
