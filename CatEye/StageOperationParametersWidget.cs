using System;

namespace CatEye
{

	//[System.ComponentModel.ToolboxItem(true)]
	public partial class StageOperationParametersWidget : Gtk.Bin
	{
		private bool mModifiedByUser = true;
		
		public event EventHandler<EventArgs> UserModified;

		public bool ModifiedByUser { get { return mModifiedByUser; } }
		public void ClearUserModified() { mModifiedByUser = false; }
		
		protected virtual void OnUserModified()
		{
			mModifiedByUser = true;
			if (UserModified != null)
				UserModified(this, EventArgs.Empty);
		}

		public StageOperationParametersWidget ()
		{
			this.Build ();
		}
	}
}
