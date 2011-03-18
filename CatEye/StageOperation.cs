
using System;

namespace CatEye
{
	public class DoStageOperationEventArgs : EventArgs
	{
		DoublePixmap _hdp;
		public DoublePixmap HDP { get { return _hdp; } }
		public DoStageOperationEventArgs(DoublePixmap hdp) { _hdp = hdp; }
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
		private Stages mOwner;

		public event EventHandler<DoStageOperationEventArgs> Do;
		
		public StageOperationParametersWidget ParametersWidget { get { return mParametersWidget; } }
		
		protected StageOperation(StageOperationParametersWidget parametersWidget, Stages owner)
		{
			mParametersWidget = parametersWidget;
			mOwner = owner;
		}
		
		protected internal virtual void OnDo(DoublePixmap hdp)
		{
			if (Do != null) 
				Do(this, new DoStageOperationEventArgs(hdp));
		}
	}

}
