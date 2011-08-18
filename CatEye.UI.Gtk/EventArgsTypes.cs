using System;

namespace CatEye.UI.Gtk
{
	public class RemoteCommandEventArgs : EventArgs
	{
		private string[] mArguments;
		public string[] Arguments { get { return mArguments; } }
		public RemoteCommandEventArgs(string[] arguments)
		{
			mArguments = arguments;
		}
	}
	
}

