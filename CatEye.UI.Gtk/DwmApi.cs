using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CatEye.UI.Gtk
{
	internal class DwmApi
	{
	    [DllImport("dwmapi", PreserveSig = false)]
	    public static extern void DwmEnableBlurBehindWindow(
	        IntPtr hWnd, DWM_BLURBEHIND pBlurBehind);
	
	    [DllImport("dwmapi", PreserveSig = false)]
	    public static extern void DwmExtendFrameIntoClientArea(
	        IntPtr hWnd, MARGINS pMargins);
	
	    [DllImport("dwmapi", PreserveSig = false)]
	    public static extern bool DwmIsCompositionEnabled();
	
	    [DllImport("dwmapi", PreserveSig = false)]
	    public static extern void DwmEnableComposition(bool bEnable);
	
	    [DllImport("dwmapi", PreserveSig = false)]
	    public static extern void DwmGetColorizationColor(
	        out int pcrColorization, 
	        [MarshalAs(UnmanagedType.Bool)]out bool pfOpaqueBlend);

	    [DllImport("dwmapi", PreserveSig = false)]
	    public static extern IntPtr DwmRegisterThumbnail(
	        IntPtr dest, IntPtr source);
	
	    [DllImport("dwmapi", PreserveSig = false)]
	    public static extern void DwmUnregisterThumbnail(IntPtr hThumbnail);
	
	    [DllImport("dwmapi", PreserveSig = false)]
	    public static extern void DwmUpdateThumbnailProperties(
	        IntPtr hThumbnail, DWM_THUMBNAIL_PROPERTIES props);
	
	    [DllImport("dwmapi", PreserveSig = false)]
	    public static extern void DwmQueryThumbnailSourceSize(
	        IntPtr hThumbnail, out System.Drawing.Size size);
	
	    [StructLayout(LayoutKind.Sequential)]
	    public class DWM_THUMBNAIL_PROPERTIES
	    {
	        public uint dwFlags;
	        public RECT rcDestination;
	        public RECT rcSource;
	        public byte opacity;
	        [MarshalAs(UnmanagedType.Bool)]
	        public bool fVisible;
	        [MarshalAs(UnmanagedType.Bool)]
	        public bool fSourceClientAreaOnly;
	        public const uint DWM_TNP_RECTDESTINATION = 0x00000001;
	        public const uint DWM_TNP_RECTSOURCE = 0x00000002;
	        public const uint DWM_TNP_OPACITY = 0x00000004;
	        public const uint DWM_TNP_VISIBLE = 0x00000008;
	        public const uint DWM_TNP_SOURCECLIENTAREAONLY = 0x00000010;
	    }
	
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect,
		   int nBottomRect);
		
	    [StructLayout(LayoutKind.Sequential)]
	    public class MARGINS
	    {
	        public int cxLeftWidth, cxRightWidth, 
	                   cyTopHeight, cyBottomHeight;
	
	        public MARGINS(int left, int top, int right, int bottom)
	        {
	            cxLeftWidth = left; cyTopHeight = top; 
	            cxRightWidth = right; cyBottomHeight = bottom;
	        }
	    }
	
	    [StructLayout(LayoutKind.Sequential)]
	    public class DWM_BLURBEHIND
	    {
	        public uint dwFlags;
	        [MarshalAs(UnmanagedType.Bool)]
	        public bool fEnable;
	        public IntPtr hRegionBlur;
	        [MarshalAs(UnmanagedType.Bool)]
	        public bool fTransitionOnMaximized;
	
	        public const uint DWM_BB_ENABLE = 0x00000001;
	        public const uint DWM_BB_BLURREGION = 0x00000002;
	        public const uint DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004;
	    }
	
	    [StructLayout(LayoutKind.Sequential)]
	    public struct RECT
	    {
	        public int left, top, right, bottom;
	
	        public RECT(int left, int top, int right, int bottom)
	        {
	            this.left = left; this.top = top; 
	            this.right = right; this.bottom = bottom;
	        }
	    }
	}
}

