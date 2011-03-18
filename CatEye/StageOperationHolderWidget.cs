using System;
namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class StageOperationHolderWidget : Gtk.Bin
	{
		private StageOperation _Operation;
		public StageOperation Operation { get { return _Operation; } }
		//public StageOperationTitleWidget TitleWidget { get { return _TitleWidget; } }
		
		public event EventHandler<EventArgs> ChangeStageTitleButtonClicked;
		public event EventHandler<EventArgs> UpTitleButtonClicked;
		public event EventHandler<EventArgs> DownTitleButtonClicked;
		
		public bool Active
		{
			get { return _TitleWidget.Active; }
			set { _TitleWidget.Active = value; }
		}
		
		public Stage CurrentStage
		{
			get { return _TitleWidget.CurrentStage; }
			set { _TitleWidget.CurrentStage = value; }
		}
		
		public string Title
		{
			get { return _TitleWidget.Title; }
			set { _TitleWidget.Title = value; }
		}
		
		protected virtual void OnUpTitleButtonClicked (object sender, System.EventArgs e)
		{
			if (UpTitleButtonClicked != null)
				UpTitleButtonClicked(this, EventArgs.Empty);
		}
		
		protected virtual void OnChangeStageTitleButtonClicked (object sender, System.EventArgs e)
		{
			if (ChangeStageTitleButtonClicked != null)
				ChangeStageTitleButtonClicked(this, EventArgs.Empty);
		}
		
		protected virtual void OnDownTitleButtonClicked (object sender, System.EventArgs e)
		{
			if (DownTitleButtonClicked != null)
				DownTitleButtonClicked(this, EventArgs.Empty);
		}
		
		public StageOperationHolderWidget (StageOperation operation)
		{
			this.Build ();
			
			_Operation = operation;
			vbox.Add(operation.ParametersWidget);
			((Gtk.Box.BoxChild)vbox[operation.ParametersWidget]).Fill = false;
			((Gtk.Box.BoxChild)vbox[operation.ParametersWidget]).Expand = false;
			
			_TitleWidget.ChangeStageButtonClicked += delegate {
				OnChangeStageTitleButtonClicked(this, EventArgs.Empty);
			};
			_TitleWidget.UpButtonClicked += delegate {
				OnUpTitleButtonClicked(this, EventArgs.Empty);
			};
			_TitleWidget.DownButtonClicked += delegate {
				OnDownTitleButtonClicked(this, EventArgs.Empty);
			};
			
			operation.ParametersWidget.Show();
		}
	}
}

