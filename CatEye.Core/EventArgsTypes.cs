using System;

namespace CatEye.Core
{
	public class StageOperationParametersEventArgs : EventArgs
	{
		StageOperationParameters _Target;
		public StageOperationParameters Target { get { return _Target; } }
		public StageOperationParametersEventArgs(StageOperationParameters target)
		{
			_Target = target;
		}
	}
	public class ReportStageProgressMessageEventArgs : EventArgs
	{
		private bool mShowProgressBar;
		private double mProgress;
		private string mStatus;
		private bool mUpdate;
		
		public bool ShowProgressBar { get { return mShowProgressBar; } }
		public double Progress { get { return mProgress; } }
		public string Status { get { return mStatus; } }
		public bool Update { get { return mUpdate; } }		
		
		public ReportStageProgressMessageEventArgs(bool showProgressBar, double progress, string status, bool update)
		{
			mShowProgressBar = showProgressBar;
			mProgress = progress;
			mStatus = status;
			mUpdate = update;
		}
	}
	public class DoStageOperationEventArgs : EventArgs
	{
		IBitmapCore _hdp;
		public IBitmapCore HDP { get { return _hdp; } }
		public DoStageOperationEventArgs(IBitmapCore hdp) { _hdp = hdp; }
	}
	
	public class ReportStageOperationProgressEventArgs : EventArgs
	{
		private double _Progress;
		private bool _Cancel = false;
		public double Progress { get { return _Progress; } }
		public bool Cancel 
		{ 
			get { return _Cancel; }
			set { _Cancel = value; }
		}
		public ReportStageOperationProgressEventArgs(double progress)
		{
			_Progress = progress;
		}
	}
}