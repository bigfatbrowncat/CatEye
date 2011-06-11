using System;
using Gtk;

namespace CatEye
{
	public partial class RenderingProgressWindow : Gtk.Dialog
	{
		private bool cancel_pending = false;
		
		public string ImageName
		{
			set 
			{ 
				description_label.Text = "Processing image " + value + "..."; 
			}
		}
		
		public RenderingProgressWindow ()
		{
			this.Build ();
		}

		public bool SetStatusAndProgress(double progress, string status)
		{
			progressbar.Fraction = progress;
			progressbar.Text = status;
			while (Application.EventsPending()) Application.RunIteration();
	
			if (cancel_pending) this.Destroy();
			return (!cancel_pending);
		}
		
		protected void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			this.cancel_pending = true;
		}

		protected void OnDeleteEvent (object o, Gtk.DeleteEventArgs args)
		{
			cancel_pending = true;
		}
	}
}

