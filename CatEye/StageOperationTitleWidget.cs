
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
			get { return title_checkbutton.Active; }
			set { title_checkbutton.Active = value; }
		}
		
		public bool View
		{
			get { return view_togglebutton.Active; }
			set 
			{
				_ViewToggledFromCode = true;
				view_togglebutton.Active = value; 
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
				freeze_togglebutton.Sensitive = !value;
				up_button.Sensitive = !value;
				down_button.Sensitive = !value;
			}
		}
		
		public event EventHandler<EventArgs> UpButtonClicked;
		public event EventHandler<EventArgs> DownButtonClicked;
		public event EventHandler<EventArgs> TitleCheckButtonClicked;
		public event EventHandler<EventArgs> ViewButtonClicked;
		public event EventHandler<EventArgs> FreezeButtonClicked;

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
		
		
		
		
		
		
		
		
		
		
	}
}
