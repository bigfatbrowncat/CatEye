using System;
using CatEye.Core;

namespace CatEye
{
	public class StageOperationDescriptionAttribute : Attribute
	{
		private string _Name;
		public string Name { get { return _Name; } }
		
		public StageOperationDescriptionAttribute(string name)
		{
			_Name = name;
		}

		public static string GetSOName(Type stageOperationType)
		{
			object[] attrs = stageOperationType.GetCustomAttributes(typeof(StageOperationDescriptionAttribute), true);
			if (attrs.Length == 0)
			{
				return null;
			}
			
			return ((StageOperationDescriptionAttribute)attrs[0]).Name;
		}
		
	}
	
	[StageOperationID("StageOperation")]
	public abstract class StageOperation
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
		
		public abstract double CalculateEfforts(IBitmapCore hdp);
		public abstract void OnDo(IBitmapCore hdp);
		public abstract Type GetParametersType();
	}

}
