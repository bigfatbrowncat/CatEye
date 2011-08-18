using System;

namespace CatEye.UI.Gtk.Widgets
{
	public delegate bool MousePositionChangedHandler(object sender, int x, int y);
	public delegate bool MouseButtonStateChangedHandler(object sender, int x, int y, uint button_id, bool is_down);
}

