using System;
using Gtk;
using CatEye.Core;
namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class StageOperationHolderWidget : Bin, IStageOperationHolder
	{
		private StageOperationParametersWidget _OperationParametersWidget;
		private StageOperationTitleWidget _TitleWidget;
		
		public IStageOperationParametersEditor StageOperationParametersEditor 
		{
			get { return _OperationParametersWidget; } 
		}
		
		public event EventHandler<EventArgs> StageActiveButtonClicked;
		public event EventHandler<EventArgs> UpTitleButtonClicked;
		public event EventHandler<EventArgs> DownTitleButtonClicked;
		public event EventHandler<EventArgs> EditButtonClicked;
		public event EventHandler<EventArgs> FreezeButtonClicked;
		public event EventHandler<EventArgs> RemoveButtonClicked;
		
		public bool Edit
		{
			get { return _TitleWidget.Edit; }
			set 
			{ 
				_TitleWidget.Edit = value; 
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
			
			_TitleWidget = new StageOperationTitleWidget();
			vbox.Add(_TitleWidget);
			_TitleWidget.Show();
			
			_OperationParametersWidget = operationParametersWidget;
			
			_TitleWidget.Title = StageOperationDescriptionAttribute.GetSOName(_OperationParametersWidget.Parameters.GetSOType());
			
			vbox.Add(operationParametersWidget);
			((Box.BoxChild)vbox[operationParametersWidget]).Position = 1;
			((Box.BoxChild)vbox[operationParametersWidget]).Fill = false;
			((Box.BoxChild)vbox[operationParametersWidget]).Expand = false;
			
			_TitleWidget.Active = _OperationParametersWidget.Parameters.Active;
			
			// Active
			_OperationParametersWidget.Parameters.Changed += delegate {
				_TitleWidget.Active = _OperationParametersWidget.Parameters.Active;
			};


			_TitleWidget.UpButtonClicked += delegate {
				OnUpTitleButtonClicked(this, EventArgs.Empty);
			};
			_TitleWidget.DownButtonClicked += delegate {
				OnDownTitleButtonClicked(this, EventArgs.Empty);
			};
			_TitleWidget.TitleCheckButtonClicked += delegate {
				_OperationParametersWidget.Parameters.Active = _TitleWidget.Active;
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
			// Here we should draw the shadow
			
			
			int l, t, w, h;
			w = Allocation.Width + 4; h = Allocation.Height + 4;
			l = Allocation.Left - 1; t = Allocation.Top - 1;
			
			int mx, my;
			using (Gdk.GC gc = new Gdk.GC(GdkWindow))
			{
				using (Gdk.Pixbuf buf = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.shadow.png"))
				{
					mx = buf.Width / 3;
					my = buf.Height / 3;
					
					// Corners
					GdkWindow.DrawPixbuf(gc, buf, 0, 0, l, t, mx, my, Gdk.RgbDither.None, 0, 0);
					GdkWindow.DrawPixbuf(gc, buf, 2 * mx, 0, l + w - mx, t, mx, my, Gdk.RgbDither.None, 0, 0);
					GdkWindow.DrawPixbuf(gc, buf, 2 * mx, 2 * my, l + w - mx, t + h - my, mx, my, Gdk.RgbDither.None, 0, 0);
					GdkWindow.DrawPixbuf(gc, buf, 0, 2 * my, l, t + h - my, mx, my, Gdk.RgbDither.None, 0, 0);
					
					// Sides
					for (int i = 0; i < (w - 2 * mx); i++)
					{
						GdkWindow.DrawPixbuf(gc, buf, mx, 0, l + mx + i, t, 1, my, Gdk.RgbDither.None, 0, 0);
						GdkWindow.DrawPixbuf(gc, buf, mx, 2 * my, l + mx + i, t + h - my, 1, my, Gdk.RgbDither.None, 0, 0);
					}
					for (int j = 0; j < (h - 2 * my); j++)
					{
						GdkWindow.DrawPixbuf(gc, buf, 0, my, l, t + my + j, mx, 1, Gdk.RgbDither.None, 0, 0);
						GdkWindow.DrawPixbuf(gc, buf, 2 * mx, my, l + w - mx, t + my + j, mx, 1, Gdk.RgbDither.None, 0, 0);
					}

					gc.RgbFgColor = this.Style.Background(StateType.Normal);
					GdkWindow.DrawRectangle(gc, true, new Gdk.Rectangle(l - 1 + mx, t - 1 + my, w - 2 * mx, h - 2 * my));
				}
				
				/*
				Gdk.Color mid = Style.Mid(StateType.Normal);
				Gdk.Color dark = new Gdk.Color(0,0,0);
				
				Gdk.Color[] shadow_colors = new Gdk.Color[4];
				for (int i = 0; i < shadow_colors.Length; i++)
				{
					double a = (double)i * i / (shadow_colors.Length * shadow_colors.Length);
					shadow_colors[i] = new Gdk.Color(
						(byte)(((dark.Red / 256) * a + (mid.Red / 256) * (1 - a))),
						(byte)(((dark.Green / 256) * a + (mid.Green / 256) * (1 - a))),
						(byte)(((dark.Blue / 256) * a + (mid.Blue / 256) * (1 - a)))
					);
					gc.RgbFgColor = shadow_colors[i]; 
					GdkWindow.DrawRectangle(gc, false, new Gdk.Rectangle(l + i, t + i, w - 2*i, h - 2*i));
				}
				
				*/
			}
			return base.OnExposeEvent(evnt);
		}
	}
}

