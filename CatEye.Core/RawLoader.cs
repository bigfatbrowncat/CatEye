using System;
using System.Runtime.InteropServices;
using System.IO;

namespace CatEye.Core
{
	public class RawLoader
	{
		private static class SSRLWrapper
		{
			/* *** That's how it looks in the SSRL library header ***
			
			struct ExtractedRawImage
			{
				int width;
				int height;
				int bitsPerChannel;
				void* data;
				libraw_processed_image_t* libraw_image;
			};
			
			typedef bool ExtractingProgressReporter(float progress);
			
			extern "C" ExtractedRawImage ExtractRawImageFromFile(char* filename, bool divide_by_2, ExtractingProgressReporter* callback);
			extern "C" void FreeExtractedRawImage(ExtractedRawImage img);
			
			*/
			
			
			[StructLayout(LayoutKind.Sequential)]
			internal struct ExtractedRawImage 
			{
				public int width;
				public int height;
				public int bitsPerChannel;
				public IntPtr data;
				IntPtr libraw_image;
			}
			
			internal delegate bool ExtractingProgressReporter(float progress);
			
			[DllImport("ssrl", CallingConvention = CallingConvention.Cdecl)]
			internal extern static ExtractedRawImage ExtractRawImageFromFile(
				[MarshalAs(UnmanagedType.LPStr)]
				string filename, 
				bool divide_by_2,
				[MarshalAs(UnmanagedType.FunctionPtr)]
				ExtractingProgressReporter callback
			);
			
			[DllImport("ssrl", CallingConvention = CallingConvention.Cdecl)]
			internal extern static void FreeExtractedRawImage(
				[MarshalAs(UnmanagedType.Struct)]
				ExtractedRawImage img
			);
		}
		
		private ushort[,] r_channel, g_channel, b_channel;
		
		public ushort[,] RChannel { get { return r_channel; } }
		public ushort[,] GChannel { get { return g_channel; } }
		public ushort[,] BChannel { get { return b_channel; } }
		
        private int bytesPerPixel;
        /// <summary>
        /// The number of bytes per pixel.
        /// </summary>
        public int BytesPerPixel
        {
            get { return bytesPerPixel; }
        }

		public int BytesPerChannel
		{
			get { return bytesPerPixel / 3; }
		}
		
		protected RawLoader(int width, int height)
		{
			r_channel = new ushort[width, height];
			g_channel = new ushort[width, height];
			b_channel = new ushort[width, height];
		}
		
        public unsafe static RawLoader FromFile(string filename, bool divide_by_2, ProgressReporter callback)
        {
			SSRLWrapper.ExtractedRawImage eximg = SSRLWrapper.ExtractRawImageFromFile(filename, divide_by_2, delegate (float progress) {
				return callback(progress);
			});
			
			RawLoader ppml = new RawLoader(eximg.width, eximg.height);
			
			if (eximg.bitsPerChannel == 16)
			{
				// Handling
				short[] rgb_data = new short[eximg.width * eximg.height * 3];
				Marshal.Copy(eximg.data, rgb_data, 0, rgb_data.Length);
				
				for (int i = 0; i < eximg.width; i++)
				for (int j = 0; j < eximg.height; j++)
				{
					 ppml.r_channel[i, j] = (ushort)rgb_data[3 * (i + eximg.width * j)];
					 ppml.g_channel[i, j] = (ushort)rgb_data[3 * (i + eximg.width * j) + 1];
					 ppml.b_channel[i, j] = (ushort)rgb_data[3 * (i + eximg.width * j) + 2];
				}
			}
			else if (eximg.bitsPerChannel == 8)
			{
				throw new Exception("Can't handle 8 bit");
			}
			else
			{
				throw new Exception("incorrect or unsupported bitsPerChannel value: " + eximg.bitsPerChannel);
			}
			
			
			SSRLWrapper.FreeExtractedRawImage(eximg);
			
			return ppml;
        }		
	
		public bool Downscale(int k, ProgressReporter callback)
		{
			int width = r_channel.GetLength(0);
			int height = r_channel.GetLength(1);
			
			ushort[,] new_r = new ushort[width / k, height / k];
			ushort[,] new_g = new ushort[width / k, height / k];
			ushort[,] new_b = new ushort[width / k, height / k];

			for (int i = 0; i < width / k; i++)
			{
				if (callback != null)
				{
					if (!callback((double)i / (width / k))) 
						return false;
				}

				for (int j = 0; j < height / k; j++)
				{
					double r = 0, g = 0, b = 0;
					for (int u = i * k; u < i * k + k; u++)
					for (int v = j * k; v < j * k + k; v++)
					{
						if (u < width && v < height)
						{
							r += r_channel[u, v];
							g += g_channel[u, v];
							b += b_channel[u, v];
						}
					}
					r /= (k * k);
					g /= (k * k);
					b /= (k * k);
					
					new_r[i, j] = (ushort)r;
					new_g[i, j] = (ushort)g;
					new_b[i, j] = (ushort)b;
				}
				
			}
			
			r_channel = new_r;
			g_channel = new_g;
			b_channel = new_b;
			
			return true;
		}
		
		
		
	}
	
}