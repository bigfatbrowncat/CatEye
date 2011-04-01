using System;

namespace CatEye
{
	public class IncorrectValueException : Exception
	{
		public IncorrectValueException() : base("Incorrect value") {}
	}

	//[System.ComponentModel.ToolboxItem(true)]
	public class StageOperationParametersWidget : Gtk.Bin
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
		}
		
		/// <summary>
		/// Handles mouse position changed. 
		/// Base method should not be called when overridden.
		/// </summary>
		/// <returns>
		/// Should return "true" if it's needed to update picture.
		/// </returns>
		public virtual bool ReportMousePosition(double x, double y)	
		{ 
			return false;
		}
		
		/// <summary>
		/// Handles mouse button state changed.
		/// Base method should not be called when overridden.
		/// </summary>
		/// <returns>
		/// Should return "true" if it's needed to update picture.
		/// </returns>
		public virtual bool ReportMouseButton(double x, double y, uint button_id, bool is_down) 
		{
			return false;
		}
		
		/// <summary>
		/// Adds modifications to picture.
		/// </summary>
		/// <param name="gc">
		/// A <see cref="Gdk.GC"/>
		/// </param>
		/// <param name="target">
		/// A <see cref="Gdk.Drawable"/>
		/// </param>
		public virtual void DrawToDrawable(Gdk.Drawable target, Gdk.Rectangle image_position) { }
	}
}
