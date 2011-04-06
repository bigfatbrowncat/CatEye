using System;

namespace CatEye
{
	public class StageOperationParameters
	{
		public event EventHandler<EventArgs> Changed;
		
		protected virtual void OnChanged()
		{
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}
		
		public StageOperationParameters ()
		{
		}
	}
}

