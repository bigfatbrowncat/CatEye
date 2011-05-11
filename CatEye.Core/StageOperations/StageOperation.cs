using System;
using CatEye.Core;

namespace CatEye
{
	public class DoStageOperationEventArgs : EventArgs
	{
		FloatPixmap _hdp;
		public FloatPixmap HDP { get { return _hdp; } }
		public DoStageOperationEventArgs(FloatPixmap hdp) { _hdp = hdp; }
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
	
	public class StageOperationDescriptionAttribute : Attribute
	{
		private string _Name;
		public string Name { get { return _Name; } }
		
		public StageOperationDescriptionAttribute(string name)
		{
			_Name = name;
		}
	}
	
	[StageOperationID("StageOperation")]
	public class StageOperation
	{
		private StageOperationParameters mParameters;

		public event EventHandler<ReportStageOperationProgressEventArgs> ReportProgress;
		
		public StageOperationParameters Parameters 
		{ 
			get { return mParameters; } 
		}
		
		protected StageOperation(StageOperationParameters parameters)
		{
			mParameters = parameters;
			mParameters.Changed += delegate {
				OnParametersChanged();
			};
		}
		
		protected virtual void OnParametersChanged()
		{
		}
		
		protected virtual bool OnReportProgress(double progress)
		{
			if (ReportProgress != null)
			{
				ReportStageOperationProgressEventArgs e = new ReportStageOperationProgressEventArgs(progress);
				ReportProgress(this, e);
				return !e.Cancel;
			}
			else
				return true;
		}
		
		public virtual void OnDo(FloatPixmap hdp)
		{
		}
	}

}
