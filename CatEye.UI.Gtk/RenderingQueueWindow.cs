using System;
using Gtk;
using CatEye.Core;
using CatEye.UI.Base;

namespace CatEye.UI.Gtk
{
	public partial class RenderingQueueWindow : Window
	{
		private RenderingQueue mRenderingQueue;
		
		public RenderingQueueWindow (RenderingQueue renderingQueue) : 
				base(WindowType.Toplevel)
		{
			mRenderingQueue = renderingQueue;
			
			this.Build ();
		}
	}
}

