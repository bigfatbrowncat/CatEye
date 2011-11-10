using System;
using System.Runtime.InteropServices;

namespace CatEye.Core
{
	public class RawDescriptionLoader
	{
		public enum FlipValues { None, Clockwise, Counterclockwise, UpsideDown }
		
        private static DateTime timeOrigin = new DateTime(1970, 1, 1, 00, 00, 00).ToLocalTime();
		
		private byte[] mThumbnailData;
		private bool mIsJpeg;
		private float mAperture;
		private float mShutter;
		private float mISOSpeed;
		private float mFocalLength;
		private string mArtist;
		private string mDescription;
		private string mCameraMaker;
		private string mCameraModel;
		private FlipValues mFlip;
		private DateTime mTimeStamp;
		
		protected RawDescriptionLoader ()
		{
			
		}
		
		public byte[] ThumbnailData
		{
			get 
			{
				return mThumbnailData;
			}
		}
		
		public float Aperture { get { return mAperture; } }
		public float Shutter { get { return mShutter; } }
		public float ISOSpeed { get { return mISOSpeed; } }
		public float FocalLength { get { return mFocalLength; } }
		public string Artist { get { return mArtist; } }
		public string Description { get { return mDescription; } }
		public string CameraMaker { get { return mCameraMaker; } }
		public string CameraModel { get { return mCameraModel; } }
		public FlipValues Flip { get { return mFlip; } }
		public DateTime TimeStamp { get { return mTimeStamp; } }
		
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
			ppml.mThumbnailData = new byte[eximg.data_size];
			ppml.mIsJpeg = eximg.is_jpeg;
			ppml.mAperture = eximg.aperture;
			ppml.mShutter = eximg.shutter;
			ppml.mISOSpeed = eximg.iso_speed;
			ppml.mFocalLength = eximg.focal_len;
			ppml.mArtist = Marshal.PtrToStringAnsi(eximg.artist);
			ppml.mDescription = Marshal.PtrToStringAnsi(eximg.desc);
			ppml.mTimeStamp = timeOrigin.AddSeconds(eximg.timestamp);
			ppml.mCameraMaker = Marshal.PtrToStringAnsi(eximg.camera_maker);
			ppml.mCameraModel = Marshal.PtrToStringAnsi(eximg.camera_model);
			int flip = eximg.flip;
			
			Marshal.Copy(eximg.data, ppml.mThumbnailData, 0, ppml.mThumbnailData.Length);
			
			SSRLWrapper.FreeExtractedDescription(eximg);
			
			// Rotating the image
			// flip: 0 - no rotation; 3 - 180-deg rotation; 5 - 90-deg counterclockwise, 6 - 90-deg clockwise
			if (flip == 0) ppml.mFlip = FlipValues.None;
			else if (flip == 3) ppml.mFlip = FlipValues.UpsideDown;
			else if (flip == 5) ppml.mFlip = FlipValues.Counterclockwise;
			else if (flip == 6) ppml.mFlip = FlipValues.Clockwise;
			
			return ppml;
		}
	}
}

