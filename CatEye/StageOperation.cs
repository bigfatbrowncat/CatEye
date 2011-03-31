
using System;

namespace CatEye
{
	public class DoStageOperationEventArgs : EventArgs
	{
		DoublePixmap _hdp;
		public DoublePixmap HDP { get { return _hdp; } }
		public DoStageOperationEventArgs(DoublePixmap hdp) { _hdp = hdp; }
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
	
	public class StageOperation
	{
		private StageOperationParametersWidget mParametersWidget;

		public event EventHandler<DoStageOperationEventArgs> Do;
		public event EventHandler<ReportStageOperationProgressEventArgs> ReportProgress;
		
		public StageOperationParametersWidget ParametersWidget { get { return mParametersWidget; } }
		
		protected StageOperation(StageOperationParametersWidget parametersWidget)
		{
			mParametersWidget = parametersWidget;
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
		
		protected internal virtual void OnDo(DoublePixmap hdp)
		{
			if (Do != null)
				Do(this, new DoStageOperationEventArgs(hdp));
			if (!OnReportProgress(1)) throw new UserCancelException();
		}
	}

}
