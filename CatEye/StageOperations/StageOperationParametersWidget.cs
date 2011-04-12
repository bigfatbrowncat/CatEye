using System;

namespace CatEye
{
	public class IncorrectValueException : Exception
	{
		public IncorrectValueException() : base("Incorrect value") {}
	}
	
	[StageOperationID("StageOperation")]
	public class StageOperationParametersWidget : Gtk.Bin
	{
		private StageOperationParameters mParameters;
		private bool mParametersAreChangingByMe = false;
		
		public event EventHandler<EventArgs> UserModified;

		public StageOperationParameters Parameters
		{
			get { return mParameters; }
		}
		
		/// <summary>
		/// Raises the <c>UserModified</c> event.
		/// Should be called every time when the user changes something in the
		/// widget's UI.
		/// </summary>
		protected virtual void OnUserModified()
		{
			if (UserModified != null)
				UserModified(this, EventArgs.Empty);
		}

		public StageOperationParametersWidget (StageOperationParameters parameters)
		{
			mParameters = parameters;
			mParameters.Changed += delegate {
				if (!mParametersAreChangingByMe) 
					HandleParametersChangedNotByUI();
			};
			
		}
		
		/// <summary>
		/// Should be called before changing <c>Parameters</c> properties from Widget UI.
		/// </summary>
		/// <remarks>
		/// Used to prevent calling <c>HandleParametersChangedBySomeoneElse()</c> handler. 
		/// Be careful! Don't forget to call <c>EndChangingParameters()</c> after changing.
		/// </remarks>
		protected void StartChangingParameters()
		{
			mParametersAreChangingByMe = true;
		}
		
		/// <summary>
		/// Should be called after changing <c>Parameters</c> properties from Widget UI.
		/// </summary>
		/// <remarks>
		/// Used to prevent calling <c>HandleParametersChangedBySomeoneElse()</c> handler.
		/// </remarks>
		protected void EndChangingParameters()
		{
			mParametersAreChangingByMe = false;
		}
		
		/// <summary>
		/// Used to update data in the widget when someone changed <c>Parameters</c>
		/// properties. 
		/// 
		/// Should be overridden in child classes to handle
		/// when some parameters are changed by some other class.
		/// 
		/// <remarks>
		/// To avoid calling of this handler when the class changes
		/// the parameter by itself, you should enclose changing 
		/// operators with
		/// <see cref="StartChangingParameters()"/> and 
		/// <see cref="EndChangingParameters()"/> calls.
		/// <example>
		///    StartChangingParameters();
		///    ((SomeChildClassOfParameters)Parameters).SomeProperty = some_value;
		///    EndChangingParameters();
		/// </example>
		/// </remarks>
		protected virtual void HandleParametersChangedNotByUI ()
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
		/// Handles mouse button state changed i.e. the user pushed or released the button.
		/// Base method should not be called when overridden.
		/// </summary>
		/// <param name="x">
		/// X coordinate from 0 to 1.
		/// </param>
		/// <param name="y">
		/// Y coordinate from 0 to 1.
		/// </param>
		/// <param name="button_id">
		/// The button which stage is changed.
		/// </param>
		/// <param name="is_down">
		/// <c>true</c> if the button is down now, <c>false</c> if it's up.
		/// </param>
		/// <returns>
		/// Should return "true" if it's needed to update picture.
		/// </returns>
		public virtual bool ReportMouseButton(double x, double y, uint button_id, bool is_down) 
		{
			return false;
		}
		
		/// <summary>
		/// Handles that the image is changed. Should be called from the outside.
		/// Override it to do some recalculations.
		/// </summary>
		/// <param name='image_width'>
		/// New image width.
		/// </param>
		/// <param name='image_height'>
		/// New image height.
		/// </param>
		public virtual void ReportImageChanged(int image_width, int image_height)
		{
		}
		
		/// <summary>
		/// Adds editor modifications to picture. For example, some handlers to
		/// change parameters graphically, the crop frame and so on.
		/// </summary>
		/// <param name="target">
		/// A <see cref="Gdk.Drawable"/> to paint to.
		/// </param>
		/// <param name="image_position">
		/// The rectangle on the <c>target</c> which is occupied by the image.
		/// </param>
		public virtual void DrawToDrawable(Gdk.Drawable target, Gdk.Rectangle image_position) 
		{
		}
	}
}
