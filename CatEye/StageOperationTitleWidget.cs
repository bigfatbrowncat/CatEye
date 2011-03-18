
using System;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class StageOperationTitleWidget : Gtk.Bin
	{
		private Stage _CurrentStage;
		private StageOperation _Operation;
		private string _Title;

		public Stage CurrentStage 
		{ 
			get 
			{ 
				return _CurrentStage; 
			} 
			set
			{
				_CurrentStage = value;
				if (_CurrentStage == Stage.Stage2)
					change_stage_arrow.ArrowType = Gtk.ArrowType.Right;
				if (_CurrentStage == Stage.Stage3)
					change_stage_arrow.ArrowType = Gtk.ArrowType.Left;
			}
		}
		
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
		
		public event EventHandler<EventArgs> ChangeStageButtonClicked;
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
		
		protected virtual void OnChangeStageButtonClicked (object sender, System.EventArgs e)
		{
			if (ChangeStageButtonClicked != null)
				ChangeStageButtonClicked(this, EventArgs.Empty);
		}
		
		protected virtual void OnDownButtonClicked (object sender, System.EventArgs e)
		{
			if (DownButtonClicked != null)
				DownButtonClicked(this, EventArgs.Empty);
		}
		
		
		
	}
}
