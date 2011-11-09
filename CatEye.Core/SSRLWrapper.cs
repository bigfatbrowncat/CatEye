using System;
using System.Runtime.InteropServices;

namespace CatEye.Core
{
	static class SSRLWrapper
	{
		/* *** That's how it looks in the SSRL library header ***
		
		struct ExtractedRawImage {
			int width;
			int height;
			int bitsPerChannel;
			void* data;
			libraw_processed_image_t* libraw_image;
		};
		
		struct ExtractedDescription
		{
			void* data;
			int data_size;	// in bytes
			bool is_jpeg;
		
			float       iso_speed;
		    float       shutter;
		    float       aperture;
		    float       focal_len;
		    time_t      timestamp;
		    unsigned    shot_order;
		    unsigned    gpsdata[32];
		    char        desc[512];
		    char        artist[64];
		
			libraw_processed_image_t* libraw_image;
		};
		
		typedef bool ExtractingProgressReporter(float progress);
		
		extern "C"
		{
			DllDef ExtractedRawImage ExtractRawImageFromFile(char* filename, bool divide_by_2, ExtractingProgressReporter* callback);
			DllDef ExtractedDescription ExtractDescriptionFromFile(char* filename);
			DllDef void FreeExtractedRawImage(ExtractedRawImage img);
			DllDef void FreeExtractedDescription(ExtractedDescription img);
		}

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
		
		[StructLayout(LayoutKind.Sequential)]
		internal struct ExtractedDescription 
		{
			public IntPtr data;
			public int data_size;
			public bool is_jpeg;
			
			public float iso_speed;
			public float shutter;
		    public float aperture;
		    public float focal_len;
		    public uint timestamp;
		    public uint shot_order;
		    public IntPtr gpsdata;
		    public IntPtr desc;
		    public IntPtr artist;
			
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
		internal extern static int ExtractDescriptionFromFile(
			[MarshalAs(UnmanagedType.LPStr)]
			string filename,
			IntPtr res
		);
		
		[DllImport("ssrl", CallingConvention = CallingConvention.Cdecl)]
		internal extern static void FreeExtractedRawImage(
			[MarshalAs(UnmanagedType.Struct)]
			ExtractedRawImage img
		);
		
		[DllImport("ssrl", CallingConvention = CallingConvention.Cdecl)]
		internal extern static void FreeExtractedDescription(
			[MarshalAs(UnmanagedType.Struct)]
			ExtractedDescription img
		);
	}

}

