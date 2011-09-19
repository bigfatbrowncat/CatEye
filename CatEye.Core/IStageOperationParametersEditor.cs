using System;

namespace CatEye.Core
{
	public interface IStageOperationParametersEditor
	{
		event EventHandler<EventArgs> UserModified;
		/// <summary>
		/// Handles mouse position change. 
		/// Base method should not be called when overridden.
		/// </summary>
		/// <returns>
		/// Should return "true" if it's needed to update the picture.
		/// </returns>
		bool ReportMousePosition(IBitmapCore image, int x, int y, int width, int height);
		
		/// <summary>
		/// Handles mouse button state change i.e. the user pushed or released the button.
		/// Base method should not be called when overridden.
		/// </summary>
		/// <param name="x">
		/// X coordinate from the left top corner of the image
		/// </param>
		/// <param name="y">
		/// Y coordinate from the left top corner of the image
		/// </param>
		/// <param name="button_id">
		/// The button which state is changed.
		/// </param>
		/// <param name="is_down">
		/// <c>true</c> if the button is down now, <c>false</c> if it's up.
		/// </param>
		/// <returns>
		/// Should return "true" if it's needed to update picture and "false" otherwise.
		/// </returns>
		bool ReportMouseButton(IBitmapCore image, int x, int y, int width, int height, uint button_id, bool is_down);
		
		void AnalyzeImage(IBitmapCore image);
		
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
		void DrawEditor(IBitmapCore image, IBitmapView target); 
		
	}
}

