using System;

namespace CatEye.UI.Gtk
{
	public class RemoteCommandEventArgs : EventArgs
	{
		private string mCommand;
		private string[] mArguments;
		public string Command { get { return mCommand; } }
		public string[] Arguments { get { return mArguments; } }
		public RemoteCommandEventArgs(string command, string[] arguments)
		{
			mArguments = arguments;
			mCommand = command;
		}
	}
	
}

