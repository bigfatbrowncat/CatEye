using System;

namespace CatEye.UI.Base
{
	public class RenderingTaskEventArgs : EventArgs
	{
		RenderingTask _Target;
		public RenderingTask Target { get { return _Target; } }
		public RenderingTaskEventArgs(RenderingTask target)
		{
			_Target = target;
		}
	}
}

