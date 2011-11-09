using System;
using System.Runtime.InteropServices;

namespace CatEye.Core
{
	public class RawDescriptionLoader
	{
		private byte[] mRgbData;
		private bool mIsJpeg;
		
		protected RawDescriptionLoader ()
		{
			
		}
		
		public byte[] RgbData
		{
			get 
			{
				return mRgbData;
			}
		}
		
		public bool IsJpeg { get { return mIsJpeg; } }
		
        public unsafe static RawDescriptionLoader FromFile(string filename)
		{
			IntPtr eximg_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SSRLWrapper.ExtractedDescription)));
			int err = SSRLWrapper.ExtractDescriptionFromFile(filename, eximg_ptr);
#if DEBUG
			Console.WriteLine("ExtractDescriptionFromFile returned " + err);
#endif
			if (err != 0)
			{
				throw new Exception();	// TODO: Design a correct exception type
			}

			SSRLWrapper.ExtractedDescription eximg = (SSRLWrapper.ExtractedDescription)Marshal.PtrToStructure(eximg_ptr, typeof(SSRLWrapper.ExtractedDescription));
			
			RawDescriptionLoader ppml = new RawDescriptionLoader();
			
			// Handling
			ppml.mRgbData = new byte[eximg.data_size];
			ppml.mIsJpeg = eximg.is_jpeg;
			Marshal.Copy(eximg.data, ppml.mRgbData, 0, ppml.mRgbData.Length);
			
			/*System.IO.BinaryWriter bw = new System.IO.BinaryWriter(new System.IO.FileStream("test2.jpeg", System.IO.FileMode.Create));
			bw.Write(ppml.mRgbData);
			bw.Close();*/
			
			//SSRLWrapper.FreeExtractedDescription(eximg);
			
			return ppml;
		}
	}
}

