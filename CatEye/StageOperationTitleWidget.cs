
using System;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class StageOperationTitleWidget : Gtk.Bin
	{
		private StageOperation _Operation;
		private string _Title;
		private bool _ViewToggledFromCode = false;
		private bool _FreezeToggledFromCode = false;

		public StageOperation Operation
		{
			get { return _Operation; }
			set 
			{ 
				_Operation = value;
			}
		}
		
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
		
		public bool View
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

		protected void OnDownIconImageExposeEvent (object o, Gtk.ExposeEventArgs args)
		{
		}
		
		unsafe void UpdateImageOn(Gtk.Button btn, string res)
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
					byte r = (byte)new_clr.Red;
					byte g = (byte)new_clr.Green;
					byte b = (byte)new_clr.Blue;
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
				Gtk.Widget wg = btn.Child;
				btn.Remove(wg);
				wg.Dispose();
			}

			Gtk.Image img = new Gtk.Image(buf);
			btn.BorderWidth = 0;
			btn.Add(img);
			img.Show();
		}
		
		protected void OnShown (object sender, System.EventArgs e)
		{
		}

		protected void OnEditTogglebuttonStateChanged (object o, Gtk.StateChangedArgs args)
		{
			UpdateImageOn(edit_togglebutton, "CatEye.res.pen.png");
		}

		protected void OnEditTogglebuttonStyleSet (object o, Gtk.StyleSetArgs args)
		{
			UpdateImageOn(edit_togglebutton, "CatEye.res.pen.png");
		}

		protected void OnFreezeTogglebuttonStateChanged (object o, Gtk.StateChangedArgs args)
		{
			UpdateImageOn(freeze_togglebutton, "CatEye.res.snowflake.png");
		}

		protected void OnFreezeTogglebuttonStyleSet (object o, Gtk.StyleSetArgs args)
		{
			UpdateImageOn(freeze_togglebutton, "CatEye.res.snowflake.png");
		}

		protected void OnUpButtonStyleSet (object o, Gtk.StyleSetArgs args)
		{
			UpdateImageOn(up_button, "CatEye.res.up.png");
		}

		protected void OnUpButtonStateChanged (object o, Gtk.StateChangedArgs args)
		{
			UpdateImageOn(up_button, "CatEye.res.up.png");
		}

		protected void OnDownButtonStyleSet (object o, Gtk.StyleSetArgs args)
		{
			UpdateImageOn(down_button, "CatEye.res.down.png");
		}

		protected void OnDownButtonStateChanged (object o, Gtk.StateChangedArgs args)
		{
			UpdateImageOn(down_button, "CatEye.res.down.png");
		}

		protected void OnRemoveButtonStyleSet (object o, Gtk.StyleSetArgs args)
		{
			UpdateImageOn(remove_button, "CatEye.res.remove.png");
		}

		protected void OnRemoveButtonStateChanged (object o, Gtk.StateChangedArgs args)
		{
			UpdateImageOn(remove_button, "CatEye.res.remove.png");
		}
	}
}
