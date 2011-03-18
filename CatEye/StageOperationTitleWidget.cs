
using System;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class StageOperationTitleWidget : Gtk.Bin
	{
		private StageOperation _Operation;
		private string _Title;

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
			get { return togglebutton.Active; }
			set { togglebutton.Active = value; }
		}
		
		public event EventHandler<EventArgs> UpButtonClicked;
		public event EventHandler<EventArgs> DownButtonClicked;

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
		
		
		
	}
}
