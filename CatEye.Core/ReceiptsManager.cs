using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;

namespace CatEye.Core
{
	public static class ReceiptsManager
	{
		public const string ReceiptsExtension = ".CatEyeReceipt";
		
		
		public enum ReceiptType 
		{ 
			Default, 	// the same folder as the photo, "IMG_1234.CatEyeReceipt"
			Custom, 	// the same folder as the photo, "IMG_1234_sepia.CatEyeReceipt"
			Class, 		// the same folder as the photo, "sepia.CatEyeReceipt"
			Template,	// CatEye folder, any name
			Other 		// any other folder, any name
		}
		
		public static ReceiptType DetermineReceiptType(string receiptFileName, string rawFileName)
		{
			// extracting the paths and names
			string rawName = Path.GetFileNameWithoutExtension(Path.GetFullPath(rawFileName));
			string rawPath = Path.GetDirectoryName(Path.GetFullPath(rawFileName));
			string receiptName = Path.GetFileNameWithoutExtension(Path.GetFullPath(receiptFileName));
			string receiptPath = Path.GetDirectoryName(Path.GetFullPath(receiptFileName));
			string catEyePath = System.IO.Path.GetDirectoryName(Path.GetFullPath(System.Reflection.Assembly.GetCallingAssembly().Location));
			
			if (receiptPath == rawPath)
			{
				if (receiptName == rawName) 
					return ReceiptType.Default;
				else if (receiptName.StartsWith(rawName + "--")) 
					return ReceiptType.Custom;
				else
					return ReceiptType.Class;
			}
			else 
			{
				if (receiptPath == catEyePath)
					return ReceiptType.Template;
				else
					return ReceiptType.Other;
			}
			
		}
		
		public static string ExtractReceiptName(string receiptFileName, string rawFileName)
		{
			string rawName = Path.GetFileNameWithoutExtension(Path.GetFullPath(rawFileName));
			string rawPath = Path.GetDirectoryName(Path.GetFullPath(rawFileName));
			string receiptName = Path.GetFileNameWithoutExtension(Path.GetFullPath(receiptFileName));
			string receiptPath = Path.GetDirectoryName(Path.GetFullPath(receiptFileName));
			ReceiptType rt = DetermineReceiptType(receiptFileName, rawFileName);
			if (rt == ReceiptType.Default) return "";
			if (rt == ReceiptType.Custom)
			{
				string rcp = System.IO.Path.GetFileNameWithoutExtension(receiptName);
				rcp = rcp.Substring(rcp.IndexOf("--") + 2);
				return rcp;
			}
			return System.IO.Path.GetFileNameWithoutExtension(receiptName);
		}
		
		public static string MakeClassReceiptFilename(string path, string receiptName)
		{
			return path + Path.DirectorySeparatorChar + receiptName + ReceiptsExtension;
		}
		
		public static string MakeDefaultOrCustomReceiptFilename(string rawFileName, string receiptName)
		{
			// extracting the paths and name
			string name = Path.GetFileNameWithoutExtension(rawFileName);
			string path = Path.GetDirectoryName(rawFileName);
			
			if (receiptName != null && receiptName.Trim() != "")
				return path + Path.DirectorySeparatorChar + name + "--" + receiptName + ReceiptsExtension;
			else
				return path + Path.DirectorySeparatorChar + name + ReceiptsExtension;
				
		}
		
		private static string[] OtherRawsSameDirectory(string rawFileName)
		{
			string path = Path.GetDirectoryName(rawFileName);
			string name = Path.GetFileNameWithoutExtension(rawFileName);
			string[] alts = System.IO.Directory.GetFiles(path, "*");
			List<string> others = new List<string> ();
			for (int i = 0; i < alts.Length; i++)
			{
				if (alts[i] != rawFileName && RawLoader.IsRaw(alts[i]))
				{
					others.Add(alts[i]);
					Console.WriteLine(alts[i]);
				}
			}
			return others.ToArray();
			
		}

		public static string[] FindClassReceiptsForRaw(string rawFileName)
		{
			string path = Path.GetDirectoryName(rawFileName);
			string name = Path.GetFileNameWithoutExtension(rawFileName);
			
			List<string> receipts = new List<string>();
			string[] all = System.IO.Directory.GetFiles(path, "*" + ReceiptsExtension);
			// adding only the receipts that don't belong to other photos in the same dir
			string[] other_raws = OtherRawsSameDirectory(rawFileName);
			for (int i = 0; i < all.Length; i++)
			{
				bool belongs_to_other = false;
				for (int j = 0; j < other_raws.Length; j++)
				{
					if (DetermineReceiptType(all[i], other_raws[j]) == ReceiptType.Default ||
						DetermineReceiptType(all[i], other_raws[j]) == ReceiptType.Custom)
					{
						belongs_to_other = true;
					}
				}
				if (!belongs_to_other && 
					(!Path.GetFileNameWithoutExtension(all[i]).StartsWith(name + "--")) && 
					(!Path.GetFileNameWithoutExtension(all[i]).StartsWith(name))) receipts.Add(all[i]);
			}
			return receipts.ToArray();
		}
		
		public static string[] FindCustomReceiptsForRaw(string rawFileName)
		{
			string name = Path.GetFileNameWithoutExtension(rawFileName);
			string path = Path.GetDirectoryName(rawFileName);

			List<string> receipts = new List<string>();
			string[] alts = System.IO.Directory.GetFiles(path, name + "--*" + ReceiptsExtension);
			
			receipts.AddRange(alts);
			return receipts.ToArray();
		}
		
		public static string[] FindReceiptsForRaw(string rawFileName)
		{
			List<string> receipts = new List<string>();
			
			// extracting the paths and name
			string name = Path.GetFileNameWithoutExtension(rawFileName);
			string path = Path.GetDirectoryName(rawFileName);
			string catEyePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
			
			// 1. searching for standard receipt
			string receipt = path + Path.DirectorySeparatorChar + name + ReceiptsExtension;
			if (File.Exists(receipt)) receipts.Add(receipt);
			
			// 2. searching for custom receipts of the image
			receipts.AddRange(FindCustomReceiptsForRaw(rawFileName));
			
			// 3. searching for the class receipts in the current directory
			receipts.AddRange(FindClassReceiptsForRaw(rawFileName));
			
			// 4. adding the templates
			System.IO.TextWriter tw = new StreamWriter("out.txt");
			tw.WriteLine(catEyePath);
			tw.Close();
			string[] templates = System.IO.Directory.GetFiles(catEyePath, "*" + ReceiptsExtension);
			receipts.AddRange(templates);
			
			List<string> unique = new List<string>();
			for (int i = 0; i < receipts.Count; i++)
				if (!unique.Contains(receipts[i])) unique.Add(receipts[i]);
			
			return unique.ToArray();
		}
	}
}

