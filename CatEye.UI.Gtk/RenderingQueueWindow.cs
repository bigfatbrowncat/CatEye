using System;
using Gtk;

namespace CatEye.UI.Gtk
{
	public partial class RenderingQueueWindow : Window
	{
		public RenderingQueueWindow () : 
				base(WindowType.Toplevel)
		{
			this.Build ();
		}
	}
}

