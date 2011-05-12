using System;
namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class FrozenPanel : Gtk.Bin
	{

		//public event EventHandler<EventArgs> ViewButtonClicked;
		public event EventHandler<EventArgs> UnfreezeButtonClicked;

		/*
		public bool View
		{
			get { return view_togglebutton.Active; }
			set {
				view_togglebutton.Active = value; 
			}
		}
		*/
		
		public FrozenPanel ()
		{
			this.Build ();
		}
		
		protected virtual void OnUnfreezeButtonClicked (object sender, System.EventArgs e)
		{
			if (UnfreezeButtonClicked != null)
			{
				UnfreezeButtonClicked(this, EventArgs.Empty);
			}
		}
		
		/*
		protected virtual void OnViewTogglebuttonReleased (object sender, System.EventArgs e)
		{
			if (ViewButtonClicked != null)
			{
				ViewButtonClicked(this, EventArgs.Empty);
			}
		}
		*/		
		
	}
}

