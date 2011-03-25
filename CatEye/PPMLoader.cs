using System;
using System.IO;

namespace CatEye
{
	public class PPMLoader
	{
        private PixelMapHeader header;
        /// <summary>
        /// The header portion of the PixelMap.
        /// </summary>
        public PixelMapHeader Header
        {
            get { return header; }
            set { header = value; }
        }

		private ushort[,] r_channel, g_channel, b_channel;
		
		public ushort[,] RChannel { get { return r_channel; } }
		public ushort[,] GChannel { get { return g_channel; } }
		public ushort[,] BChannel { get { return b_channel; } }
		
		private ushort min_chan_value, max_chan_value;
	
		public double GetRDoubleValue(int x, int y)
		{
			return ((double)(r_channel[x, y] - min_chan_value)) / 
				(max_chan_value - min_chan_value);
		}
		public double GetGDoubleValue(int x, int y)
		{
			return ((double)(g_channel[x, y] - min_chan_value)) / 
				(max_chan_value - min_chan_value);
		}
		public double GetBDoubleValue(int x, int y)
		{
			return ((double)(b_channel[x, y] - min_chan_value)) / 
				(max_chan_value - min_chan_value);
		}

		public void SetRDoubleValue(int x, int y, double r)
		{
			r_channel[x, y] = (ushort)(min_chan_value + r * (max_chan_value - min_chan_value));
		}
		public void SetGDoubleValue(int x, int y, double g)
		{
			g_channel[x, y] = (ushort)(min_chan_value + g * (max_chan_value - min_chan_value));
		}
		public void SetBDoubleValue(int x, int y, double b)
		{
			b_channel[x, y] = (ushort)(min_chan_value + b * (max_chan_value - min_chan_value));
		}
		
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
		
		public enum ChannelIndex { Red = 0, Green = 1, Blue = 2 };
		
		/// <summary>
		/// Returns double value (0..1) corresponding to selected (chind) channel 
		/// of selected (x, y) pixel 
		/// </summary>
		private static ushort GetChannelValue(byte[] imageData, int x, int y, ChannelIndex chind, int BytesPerChannel, int stride)
		{
			int BytesPerPixel = BytesPerChannel * 3;
			byte[] theValue = new byte[BytesPerChannel];
			for (int i = 0; i < BytesPerChannel; i++)
			{
				theValue[i] = imageData[stride * y + 
				                        BytesPerPixel * x + 
				                        (int)chind * BytesPerChannel + 
				                        i];	
			}
			
			// Constructing long channel value from bytes
			int res = 0;
			for (int i = 0; i < BytesPerChannel; i++)
			{
				res *= 256;
				res += theValue[i];
			}
			
			return (ushort) res;
		}
		
        private int stride;
        /// <summary>
        /// The stride of the scan across the image.  Typically this is width * bytesPerPixel, and is a multiple of 4.
        /// </summary>
        public int Stride
        {
            get { return stride; }
            set { stride = value; }
        }

		private PPMLoader (PixelMapHeader header, 
		                   ushort[,] r_channel, 
		                   ushort[,] g_channel, 
		                   ushort[,] b_channel, 
		                   ushort minchan, ushort maxchan,
		                   int bytesPerPixel, int stride)
		{
			this.header = header;

			this.r_channel = r_channel;
			this.g_channel = g_channel;
			this.b_channel = b_channel;
			
			this.min_chan_value = minchan;
			this.max_chan_value = maxchan;

			this.bytesPerPixel = bytesPerPixel;
			this.stride = stride;
		}

        public static PPMLoader FromFile(string filename, ProgressReporter callback)
        {
			PPMLoader ppml;
            if (File.Exists(filename))
            {
                FileStream stream = new FileStream(filename, FileMode.Open);
                ppml = FromStream(stream, callback);
                stream.Close();
            }
            else
            {
                throw new FileNotFoundException("The file \"" + filename + "\" does not exist", filename);
            }
			return ppml;
        }		
        private static int ReadValue(BinaryReader binReader)
        {
            string value = string.Empty;
            while (!Char.IsWhiteSpace((char)binReader.PeekChar()))
            {
                value += binReader.ReadChar().ToString();
            }
            binReader.ReadByte();   // get rid of the whitespace.
            return int.Parse(value);
        }
        		
        public static PPMLoader FromStream(Stream stream, ProgressReporter callback)
        {
            PixelMapHeader header = new PixelMapHeader();
            BinaryReader binReader = new BinaryReader(stream);

			int headerItemCount = 0;
            try
            {
                //1. Read the Header.
                while (headerItemCount < 4)
                {
                    char nextChar = (char)binReader.PeekChar();
                    if (nextChar == '#')    // comment
                    {
                        while (binReader.ReadChar() != '\n') ;  // ignore the rest of the line.
                    }
                    else if (Char.IsWhiteSpace(nextChar))   // whitespace
                    {
                        binReader.ReadChar();   // ignore whitespace
                    }
                    else
                    {
                        switch (headerItemCount)
                        {
                            case 0: // next item is Magic Number
                                // Read the first 2 characters and determine the type of pixelmap.
                                char[] chars = binReader.ReadChars(2);
                                header.MagicNumber = chars[0].ToString() + chars[1].ToString();
                                headerItemCount++;
                                break;
                            case 1: // next item is the width.
                                header.Width = ReadValue(binReader);
                                headerItemCount++;
                                break;
                            case 2: // next item is the height.
                                header.Height = ReadValue(binReader);
                                headerItemCount++;
                                break;
                            case 3: // next item is the depth.
                                if (header.MagicNumber == "P1" | header.MagicNumber == "P4")
                                {
                                    // no depth value for PBM type.
                                    headerItemCount++;
                                }
                                else
                                {
                                    header.Depth = ReadValue(binReader);
                                    headerItemCount++;
                                }
                                break;
                            default:
                                throw new Exception("Error parsing the file header.");
                        }
                    }
                }

				int bytesPerPixel;
				byte[] imageData;
				int stride;
				
                // 2. Read the image data.
                // 2.1 Size the imageData array to hold the image bytes.
                switch (header.MagicNumber)
                {
                    case "P6":  // 3 bytes per pixel
						if (header.Depth < 256) 
							bytesPerPixel = 3;		// 1 byte per channel
						else
							bytesPerPixel = 6;		// 2 bytes per channel
					
                        break;
                    default:
                        throw new Exception("Unsupported Magic Number: " + header.MagicNumber);
                }
                imageData = new byte[header.Width * header.Height * bytesPerPixel];
                stride = header.Width * bytesPerPixel;

                int bytesLeft = (int)(binReader.BaseStream.Length - binReader.BaseStream.Position);
                imageData = binReader.ReadBytes(bytesLeft);
				
				// Extracting channel values and calculating minimal and maximal
				
				ushort[,] r_chan = new ushort[header.Width, header.Height];
				ushort[,] g_chan = new ushort[header.Width, header.Height];
				ushort[,] b_chan = new ushort[header.Width, header.Height];
				
				ushort minchan = r_chan[0, 0], maxchan = r_chan[0, 0];
				
				for (int i = 0; i < header.Width; i++)
				{
					if (callback != null)
					{
						if (!callback((double)i / header.Width)) 
							return null;
					}
					for (int j = 0; j < header.Height; j++)
					{
						r_chan[i, j] = GetChannelValue(imageData, i, j, PPMLoader.ChannelIndex.Red, bytesPerPixel / 3, stride);
						g_chan[i, j] = GetChannelValue(imageData, i, j, PPMLoader.ChannelIndex.Green, bytesPerPixel / 3, stride);
						b_chan[i, j] = GetChannelValue(imageData, i, j, PPMLoader.ChannelIndex.Blue, bytesPerPixel / 3, stride);
						
						if (minchan > r_chan[i, j]) minchan = r_chan[i, j];
						if (minchan > g_chan[i, j]) minchan = g_chan[i, j];
						if (minchan > b_chan[i, j]) minchan = b_chan[i, j];

						if (maxchan < r_chan[i, j]) maxchan = r_chan[i, j];
						if (maxchan < g_chan[i, j]) maxchan = g_chan[i, j];
						if (maxchan < b_chan[i, j]) maxchan = b_chan[i, j];
					}
				}
				return new PPMLoader(header, r_chan, g_chan, b_chan, minchan, maxchan, bytesPerPixel, stride);
            }

            // If the end of the stream is reached before reading all of the expected values raise an exception.
            catch (EndOfStreamException e)
            {
                throw new Exception("Error reading the stream! ", e);
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading the stream! ", ex);
            }
            finally
            {
               binReader.Close();
            }
        }

        /// <summary>
        /// This struct contains the objects that are found in the header of .pbm, .pgm, and .ppm files.
        /// </summary>
        [Serializable]
        public struct PixelMapHeader
        {
            private string magicNumber;
            /// <summary>
            /// The "Magic Number" that identifies the type of Pixelmap. P1 = PBM (ASCII); P2 = PGM (ASCII); P3 = PPM (ASCII); P4 is not used;
            /// P5 = PGM (Binary); P6 = PPM (Binary).
            /// </summary>
            public string MagicNumber
            {
                get { return magicNumber; }
                set { magicNumber = value; }
            }

            private int width;
            /// <summary>
            /// The width of the image.
            /// </summary>
            public int Width
            {
                get { return width; }
                set { width = value; }
            }

            private int height;
            /// <summary>
            /// The height of the image.
            /// </summary>
            public int Height
            {
                get { return height; }
                set { height = value; }
            }

            private int depth;
            /// <summary>
            /// The depth (maximum color value in each channel) of the image.  This allows the format to represent 
            /// more than a single byte (0-255) for each color channel.
            /// </summary>
            public int Depth
            {
                get { return depth; }
                set { depth = value; }
            }
        }
		
		public bool Downscale(int k, ProgressReporter callback)
		{
			ushort[,] new_r = new ushort[header.Width / k, header.Height / k];
			ushort[,] new_g = new ushort[header.Width / k, header.Height / k];
			ushort[,] new_b = new ushort[header.Width / k, header.Height / k];

			for (int i = 0; i < header.Width / k; i++)
			{
				if (callback != null)
				{
					if (!callback((double)i / (header.Width / k))) 
						return false;
				}

				for (int j = 0; j < header.Height / k; j++)
				{
					double r = 0, g = 0, b = 0;
					for (int u = i * k; u < i * k + k; u++)
					for (int v = j * k; v < j * k + k; v++)
					{
						if (u < Header.Width && v < Header.Height)
						{
							r += GetRDoubleValue(u, v);
							g += GetGDoubleValue(u, v);
							b += GetBDoubleValue(u, v);
						}
					}
					r /= (k * k);
					g /= (k * k);
					b /= (k * k);
					
					new_r[i, j] = (ushort)(min_chan_value + r * (max_chan_value - min_chan_value));
					new_g[i, j] = (ushort)(min_chan_value + g * (max_chan_value - min_chan_value));
					new_b[i, j] = (ushort)(min_chan_value + b * (max_chan_value - min_chan_value));
				}
				
			}
			
			r_channel = new_r;
			g_channel = new_g;
			b_channel = new_b;
			header.Width /= k;
			header.Height /= k;
			
			return true;
		}
		
		
		
	}
	
}
