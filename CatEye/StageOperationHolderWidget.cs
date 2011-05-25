using System;
namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class StageOperationHolderWidget : Gtk.Bin
	{
		private StageOperationParametersWidget _OperationParametersWidget;
		private Gtk.StateType _AdditionalState = Gtk.StateType.Normal;
		
		public StageOperationParametersWidget OperationParametersWidget 
		{
			get { return _OperationParametersWidget; } 
		}
		
		public event EventHandler<EventArgs> StageActiveButtonClicked;
		public event EventHandler<EventArgs> UpTitleButtonClicked;
		public event EventHandler<EventArgs> DownTitleButtonClicked;
		public event EventHandler<EventArgs> EditButtonClicked;
		public event EventHandler<EventArgs> FreezeButtonClicked;
		public event EventHandler<EventArgs> RemoveButtonClicked;
		
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
			if (EditButtonClicked != null)
				EditButtonClicked(this, EventArgs.Empty);
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
			
			_TitleWidget.Active = _OperationParametersWidget.Parameters.Active;
			
			// Active
			_OperationParametersWidget.Parameters.Changed += delegate {
				_TitleWidget.Active = _OperationParametersWidget.Parameters.Active;
			};
			
			_TitleWidget.TitleCheckButtonClicked += delegate {
				_OperationParametersWidget.Parameters.Active = _TitleWidget.Active;
			};
			

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

		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			int l, t, w, h;
			w = Allocation.Width; h = Allocation.Height;
			l = Allocation.Left; t = Allocation.Top;
			
			Gdk.GC gc = new Gdk.GC(GdkWindow);
			// outer box
			/*Gtk.Style.PaintBox(this.Style, GdkWindow, Gtk.StateType.Normal, 
				Gtk.ShadowType.In, new Gdk.Rectangle(l + 1, t + 1, w - 2, h - 2), this, null,
				l + 1, t + 1, w - 2, h - 2);
			*/
			
			Gdk.Color mid = Style.Mid(Gtk.StateType.Normal);
			Gdk.Color dark = new Gdk.Color((byte)(mid.Red / 256 / 1.5), 
										   (byte)(mid.Green / 256 / 1.5), 
										   (byte)(mid.Blue / 256 / 1.5));
			
			Gdk.Color[] shadow_colors = new Gdk.Color[2];
			for (int i = 0; i < shadow_colors.Length; i++)
			{
				
				shadow_colors[i] = new Gdk.Color(
					(byte)(((dark.Red / 256) * i + (mid.Red / 256) * (shadow_colors.Length - i)) / shadow_colors.Length),
					(byte)(((dark.Green / 256) * i + (mid.Green / 256) * (shadow_colors.Length - i)) / shadow_colors.Length),
					(byte)(((dark.Blue / 256) * i + (mid.Blue / 256) * (shadow_colors.Length - i)) / shadow_colors.Length)
				);
				gc.RgbFgColor = shadow_colors[i]; 
				GdkWindow.DrawRectangle(gc, false, new Gdk.Rectangle(l + i, t + i, w - 2*i, h - 2*i));
			}
			
			int m = shadow_colors.Length;
			gc.RgbFgColor = this.Style.Background(Gtk.StateType.Normal);
			GdkWindow.DrawRectangle(gc, true, new Gdk.Rectangle(l + m, t + m, w - 2*m, h - 2*m));
/*			
			Gtk.Style.PaintShadowGap(this.Style, GdkWindow, Gtk.StateType.Normal, 
				Gtk.ShadowType.In, new Gdk.Rectangle(l + 4, t + 4, w - 8, h - 8), this, null,
				l + 4, t + 4, w - 8, h - 8, Gtk.PositionType.Bottom, 2, 4);
*/			
			// inner box
			//Gtk.Style.PaintBox(this.Style, GdkWindow, Gtk.StateType.Normal, 
			//	Gtk.ShadowType.None, new Gdk.Rectangle(l + 3, t + 3, w - 6, h - 6), this, null,
			//	l + 3, t + 3, w - 6, h - 6);
			
			return base.OnExposeEvent(evnt);
		}
	}
}

