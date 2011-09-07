using System;
using Gtk;
namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class FrozenPanel : Bin
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
			
			ForeColoredSymbol freeze_symbol = new ForeColoredSymbol();
			int pic_width;
			using (Gdk.Pixbuf buf = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.snowflake.png"))
			{
				freeze_symbol.Symbol = buf;
				pic_width = buf.Width;
			}
			
			hbox.Add (freeze_symbol);
			((HBox.BoxChild)(hbox[freeze_symbol])).Position = 0;
			((HBox.BoxChild)(hbox[freeze_symbol])).Expand = false;
			freeze_symbol.SetSizeRequest(pic_width + 4, 0);
			freeze_symbol.Show();
			
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

