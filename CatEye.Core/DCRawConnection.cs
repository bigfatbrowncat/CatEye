using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace CatEye.Core
{
	public static class DCRawConnection
	{
/*		public static readonly string [] RAW_EXTENSIONS = {
				".arw",
				".crw",
				".cr2",
				".dng",
				".mrw",
				".nef",
				".orf",
				".pef",
				".raw",
				".raf",
				".rw2"
		};
		
		private static List<string> raw_exts = new List<string>(RAW_EXTENSIONS);
*/		
		public static bool IsRaw(string filename)
		{
			// Verifies file with dcraw
			Process dcproc = CreateDCRawProcess("-i \"" + filename.Replace("\"", "\\\"") + "\"");
			bool res = false;
			if (dcproc.Start())
			{
				dcproc.WaitForExit(-1);
				if (dcproc.ExitCode == 0) 
					res = true;
				dcproc.Close();
			}
			
			return res;
		}
		
		public static string FindDCRaw()
		{
			string mylocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
			string dcraw_path =  mylocation + System.IO.Path.DirectorySeparatorChar.ToString() + "dcraw";
			if (Environment.OSVersion.Platform == PlatformID.Win32NT || 
			    Environment.OSVersion.Platform == PlatformID.Win32Windows)
			{
				dcraw_path += ".exe";
			}
			return dcraw_path;
		}
		
		public static Process CreateDCRawProcess(string Arguments)
		{
			string dcraw_path = FindDCRaw();
			System.Diagnostics.Process prc = new System.Diagnostics.Process();
			prc.StartInfo.UseShellExecute = false;
			prc.StartInfo.FileName = dcraw_path;
			prc.StartInfo.Arguments = Arguments;
			prc.StartInfo.RedirectStandardOutput = true;
			prc.StartInfo.RedirectStandardError = true;
			prc.StartInfo.CreateNoWindow = true;
			return prc;
		}
	}
}
	
