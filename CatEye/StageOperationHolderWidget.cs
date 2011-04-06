using System;
namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class StageOperationHolderWidget : Gtk.Bin
	{
		private StageOperationParametersWidget _OperationParametersWidget;
		public StageOperationParametersWidget OperationParametersWidget 
		{
			get { return _OperationParametersWidget; } 
		}
		
		public event EventHandler<EventArgs> StageActiveButtonClicked;
		public event EventHandler<EventArgs> UpTitleButtonClicked;
		public event EventHandler<EventArgs> DownTitleButtonClicked;
		public event EventHandler<EventArgs> ViewButtonClicked;
		public event EventHandler<EventArgs> FreezeButtonClicked;
		public event EventHandler<EventArgs> RemoveButtonClicked;
		
		public bool Active
		{
			get { return _TitleWidget.Active; }
			set 
			{ 
				_TitleWidget.Active = value; 
				ActiveUpdated();
			}
		}
		
		protected void ActiveUpdated()
		{
			if (_OperationParametersWidget != null)
				_OperationParametersWidget.Sensitive = Active;
		}
		
		public string Title
		{
			get { return _TitleWidget.Title; }
			set { _TitleWidget.Title = value; }
		}
		
		public bool View
		{
			get { return _TitleWidget.View; }
			set 
			{ 
				_TitleWidget.View = value; 
			}
		}
		
		public bool Freeze
		{
			get { return _TitleWidget.Freeze; }
			set { _TitleWidget.Freeze = value; }
		}

		public bool FrozenButtonsState
		{
			get { return _TitleWidget.FrozenButtonsState; }
			set { _TitleWidget.FrozenButtonsState = value; }
		}
		
		protected virtual void OnUpTitleButtonClicked (object sender, System.EventArgs e)
		{
			if (UpTitleButtonClicked != null)
				UpTitleButtonClicked(this, EventArgs.Empty);
		}
		
		protected virtual void OnStageActiveButtonClicked (object sender, System.EventArgs e)
		{
			ActiveUpdated();
			if (StageActiveButtonClicked != null)
				StageActiveButtonClicked(this, EventArgs.Empty);
		}
		
		protected virtual void OnDownTitleButtonClicked (object sender, System.EventArgs e)
		{
			if (DownTitleButtonClicked != null)
				DownTitleButtonClicked(this, EventArgs.Empty);
		}
		
		protected virtual void OnViewButtonClicked (object sender, System.EventArgs e)
		{
			if (ViewButtonClicked != null)
				ViewButtonClicked(this, EventArgs.Empty);
		}

		protected virtual void OnFreezeButtonClicked (object sender, System.EventArgs e)
		{
			if (FreezeButtonClicked != null)
				FreezeButtonClicked(this, EventArgs.Empty);
		}

		protected virtual void OnRemoveButtonClicked (object sender, System.EventArgs e)
		{
			if (RemoveButtonClicked != null)
				RemoveButtonClicked(this, EventArgs.Empty);
		}

		public StageOperationHolderWidget (StageOperationParametersWidget operationParametersWidget)
		{
			this.Build ();
			
			_OperationParametersWidget = operationParametersWidget;
			vbox.Add(operationParametersWidget);
			((Gtk.Box.BoxChild)vbox[operationParametersWidget]).Position = 1;
			((Gtk.Box.BoxChild)vbox[operationParametersWidget]).Fill = false;
			((Gtk.Box.BoxChild)vbox[operationParametersWidget]).Expand = false;
			
			_TitleWidget.UpButtonClicked += delegate {
				OnUpTitleButtonClicked(this, EventArgs.Empty);
			};
			_TitleWidget.DownButtonClicked += delegate {
				OnDownTitleButtonClicked(this, EventArgs.Empty);
			};
			_TitleWidget.TitleCheckButtonClicked += delegate {
				OnStageActiveButtonClicked(this, EventArgs.Empty);
			};
			_TitleWidget.ViewButtonClicked += delegate {
				OnViewButtonClicked(this, EventArgs.Empty);
			};
			_TitleWidget.FreezeButtonClicked += delegate {
				OnFreezeButtonClicked(this, EventArgs.Empty);
			};
			_TitleWidget.RemoveButtonClicked += delegate {
				OnRemoveButtonClicked(this, EventArgs.Empty);
			};
			
			operationParametersWidget.Show();
		}
	}
}

