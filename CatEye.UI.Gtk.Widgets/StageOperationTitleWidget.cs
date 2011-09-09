using System;
using Gtk;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class StageOperationTitleWidget : Bin
	{
		private string _Title;
		private bool _ViewToggledFromCode = false;
		private bool _FreezeToggledFromCode = false;

		public string Title
		{
			get { return _Title; }
			set
			{ 
				_Title = value;
				title_label.Markup = "<b>" + _Title + "</b>"; 
			}
		}
		
		public bool Active
		{
			get { 
				return title_checkbutton.Active; 
			}
			set { 
				title_checkbutton.Active = value; 
				UpdateButtonsSensitivity();
			}
		}
		
		public bool Edit
		{
			get { return edit_togglebutton.Active; }
			set 
			{
				_ViewToggledFromCode = true;
				edit_togglebutton.Active = value; 
				_ViewToggledFromCode = false;
			}
		}
		
		public bool Freeze
		{
			get { return freeze_togglebutton.Active; }
			set 
			{
				_FreezeToggledFromCode = true;
				freeze_togglebutton.Active = value; 
				_FreezeToggledFromCode = false;
			}
		}
		
		bool _FrozenButtonsState = false;
		
		public bool FrozenButtonsState
		{
			get { return _FrozenButtonsState; }
			set
			{
				_FrozenButtonsState = value;
				UpdateButtonsSensitivity();
			}
		}
		
		protected virtual void UpdateButtonsSensitivity()
		{
			freeze_togglebutton.Sensitive = !_FrozenButtonsState && title_checkbutton.Active;
			up_button.Sensitive = !_FrozenButtonsState;
			down_button.Sensitive = !_FrozenButtonsState;
			edit_togglebutton.Sensitive = title_checkbutton.Active;
		}
		
		public event EventHandler<EventArgs> UpButtonClicked;
		public event EventHandler<EventArgs> DownButtonClicked;
		public event EventHandler<EventArgs> TitleCheckButtonClicked;
		public event EventHandler<EventArgs> ViewButtonClicked;
		public event EventHandler<EventArgs> FreezeButtonClicked;
		public event EventHandler<EventArgs> RemoveButtonClicked;

		public StageOperationTitleWidget ()
		{
			this.Build ();
			
			// Edit
			ForeColoredSymbol edit_symbol = new ForeColoredSymbol();
			using (Gdk.Pixbuf buf = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.pen.png"))
			{
				edit_symbol.Symbol = buf;
			}
			edit_symbol.Show();
			edit_togglebutton.Image = edit_symbol;
			
			// Freeze
			ForeColoredSymbol freeze_symbol = new ForeColoredSymbol();
			using (Gdk.Pixbuf buf = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.snowflake.png"))
			{
				freeze_symbol.Symbol = buf;
			}
			freeze_symbol.Show();
			freeze_togglebutton.Image = freeze_symbol;

			// Up
			ForeColoredSymbol up_symbol = new ForeColoredSymbol();
			using (Gdk.Pixbuf buf = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.up.png"))
			{
				up_symbol.Symbol = buf;
			}
			up_symbol.Show();
			up_button.Image = up_symbol;
			
			// Down
			ForeColoredSymbol down_symbol = new ForeColoredSymbol();
			using (Gdk.Pixbuf buf = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.down.png"))
			{
				down_symbol.Symbol = buf;
			}
			down_symbol.Show();
			down_button.Image = down_symbol;

			// Remove
			ForeColoredSymbol remove_symbol = new ForeColoredSymbol();
			using (Gdk.Pixbuf buf = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.remove.png"))
			{
				remove_symbol.Symbol = buf;
			}
			remove_symbol.Show();
			remove_button.Image = remove_symbol;


		}
		
		public void SetEditModeSupported(bool supported)
		{
			edit_togglebutton.Visible = supported;
		}
		
		protected virtual void OnUpButtonClicked (object sender, System.EventArgs e)
		{
			if (UpButtonClicked != null)
				UpButtonClicked(this, EventArgs.Empty);
		}
		
		protected virtual void OnDownButtonClicked (object sender, System.EventArgs e)
		{
			if (DownButtonClicked != null)
				DownButtonClicked(this, EventArgs.Empty);
		}
		
		protected virtual void OnTitleCheckbuttonToggled (object sender, System.EventArgs e)
		{
			if (TitleCheckButtonClicked != null)
				TitleCheckButtonClicked(this, EventArgs.Empty);
		}

		protected virtual void OnViewTogglebuttonToggled (object sender, System.EventArgs e)
		{
			if (!_ViewToggledFromCode)
			{
				if (ViewButtonClicked != null)
					ViewButtonClicked(this, EventArgs.Empty);
			}
		}
		
		protected virtual void OnFreezeTogglebuttonToggled (object sender, System.EventArgs e)
		{
			if (!_FreezeToggledFromCode)
			{
				if (FreezeButtonClicked != null)
					FreezeButtonClicked(this, EventArgs.Empty);
			}
		}
		
		protected void OnRemoveButtonClicked (object sender, System.EventArgs e)
		{
			if (RemoveButtonClicked != null)
				RemoveButtonClicked(sender, EventArgs.Empty);
		}

		protected void OnDownIconImageExposeEvent (object o, ExposeEventArgs args)
		{
		}
		/*
		unsafe void UpdateImageOn(Button btn, string res)
		{
			Gdk.Color new_clr = btn.Style.Foreground(btn.State);
			
			Gdk.Pixbuf buf = Gdk.Pixbuf.LoadFromResource(res);
			int h = buf.Height;
			int w = buf.Width;
			int stride = buf.Rowstride;
			
			int chan = buf.NChannels;
			
			byte *cur_row = (byte *)buf.Pixels;
			for (int j = 0; j < h; j++)
			{
				byte *cur_pixel = cur_row;
				for (int i = 0; i < w; i++)
				{
					byte r = (byte)(new_clr.Red / 256);
					byte g = (byte)(new_clr.Green / 256);
					byte b = (byte)(new_clr.Blue / 256);
					byte a = cur_pixel[3];
					
					cur_pixel[0] = r;
					cur_pixel[1] = g;
					cur_pixel[2] = b;
					cur_pixel[3] = a;
					
					cur_pixel += chan;
				}
				cur_row += stride;
			}
			
			if (btn.Child != null)
			{
				Widget wg = btn.Child;
				btn.Remove(wg);
				wg.Dispose();
			}

			Image img = new Image(buf);
			btn.BorderWidth = 0;
			btn.Add(img);
			img.Show();
		}
		*/
		protected void OnShown (object sender, System.EventArgs e)
		{
		}

		protected void OnEditTogglebuttonStateChanged (object o, StateChangedArgs args)
		{
			//UpdateImageOn(edit_togglebutton, "CatEye.UI.Gtk.Widgets.res.pen.png");
		}

		protected void OnEditTogglebuttonStyleSet (object o, StyleSetArgs args)
		{
			//UpdateImageOn(edit_togglebutton, "CatEye.UI.Gtk.Widgets.res.pen.png");
		}

		protected void OnFreezeTogglebuttonStateChanged (object o, StateChangedArgs args)
		{
			//UpdateImageOn(freeze_togglebutton, "");
		}

		protected void OnFreezeTogglebuttonStyleSet (object o, StyleSetArgs args)
		{
			//UpdateImageOn(freeze_togglebutton, "CatEye.UI.Gtk.Widgets.res.snowflake.png");
		}

		protected void OnUpButtonStyleSet (object o, StyleSetArgs args)
		{
			//UpdateImageOn(up_button, "");
		}

		protected void OnUpButtonStateChanged (object o, StateChangedArgs args)
		{
			//UpdateImageOn(up_button, "CatEye.UI.Gtk.Widgets.res.up.png");
		}

		protected void OnDownButtonStyleSet (object o, StyleSetArgs args)
		{
			//UpdateImageOn(down_button, "CatEye.UI.Gtk.Widgets.res.down.png");
		}

		protected void OnDownButtonStateChanged (object o, StateChangedArgs args)
		{
			//UpdateImageOn(down_button, "CatEye.UI.Gtk.Widgets.res.down.png");
		}

		protected void OnRemoveButtonStyleSet (object o, StyleSetArgs args)
		{
			//UpdateImageOn(remove_button, "CatEye.UI.Gtk.Widgets.res.remove.png");
		}

		protected void OnRemoveButtonStateChanged (object o, StateChangedArgs args)
		{
			//UpdateImageOn(remove_button, "CatEye.UI.Gtk.Widgets.res.remove.png");
		}
	}
}
