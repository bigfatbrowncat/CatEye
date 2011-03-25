
using System;

namespace CatEye
{
	public static class DCRawConnection
	{
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
		
		public static System.Diagnostics.Process CreateDCRawProcess(string Arguments)
		{
			string dcraw_path = FindDCRaw();
			if (System.IO.File.Exists(dcraw_path))
			{
				System.Diagnostics.Process prc = new System.Diagnostics.Process();
				prc.StartInfo.UseShellExecute = false;
				prc.StartInfo.FileName = dcraw_path;
				prc.StartInfo.Arguments = Arguments;
				prc.StartInfo.RedirectStandardOutput = true;
				prc.StartInfo.RedirectStandardError = true;
				prc.StartInfo.CreateNoWindow = true;
				return prc;
			}
			else
				return null;
		}
	}
}
	
