using System;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	[StageOperationID("CrotateStageOperation")]
	[StageOperationParametersEditModeSupported(true)]
	public partial class CrotateStageOperationParametersWidget : StageOperationParametersWidget
	{
		ListStore ls;
		private int mDotRadius = 5;
		
		private enum DragState { None, Center, Round, Corner };
		private DragState mDragState = DragState.None;
		private int mDragStartX, mDragStartY;
		private int mCenterDotStartX, mCenterDotStartY;
		private double mCropWidthStart, mCropHeightStart;
		
		private bool mAspectComboboxSelfModifying = false;
		
		public CrotateStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();

			// Link to height (chain)
			ForeColoredSymbol link_w_symbol = new ForeColoredSymbol();
			using (Gdk.Pixbuf buf = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.chain.png"))
			{
				link_w_symbol.Symbol = buf;
			}
			link_w_symbol.Show();
			link_w_togglebutton.Image = link_w_symbol;

			// Link to width (chain)
			ForeColoredSymbol link_h_symbol = new ForeColoredSymbol();
			using (Gdk.Pixbuf buf = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.chain.png"))
			{
				link_h_symbol.Symbol = buf;
			}
			link_h_symbol.Show();
			link_h_togglebutton.Image = link_h_symbol;
			
			// Custom (line)
			ForeColoredSymbol custom_symbol = new ForeColoredSymbol();
			using (Gdk.Pixbuf buf = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.line.png"))
			{
				custom_symbol.Symbol = buf;
			}
			custom_symbol.Show();
			custom_togglebutton.Image = custom_symbol;
			custom_togglebutton.Label = null;
			
			ls = new ListStore(typeof(string), typeof(int));
			string[] ratioNames = ((CrotateStageOperationParameters)Parameters).PresetAspectRatioNames;
			for (int i = 0; i < ratioNames.Length; i++)
			{
				ls.AppendValues(ratioNames[i], i);
			}
			aspect_combobox.Model = ls;
			
			TreeIter ti;
			ls.GetIterFirst(out ti);
			
			mAspectComboboxSelfModifying = true;
			aspect_combobox.SetActiveIter(ti);
			mAspectComboboxSelfModifying = false;
		}
		
		protected void UpdateSensitive()
		{
			if (mDragState == DragState.None)
			{
				crop_w_spinbutton.Visible = !link_w_togglebutton.Active;
				((HBox.BoxChild)w_hbox[link_w_togglebutton]).Expand = !crop_w_spinbutton.Visible;
				((HBox.BoxChild)w_hbox[link_w_togglebutton]).Fill = !crop_w_spinbutton.Visible;
				link_w_togglebutton.Label = link_w_togglebutton.Active ? "Linked to height" : null;
					
				crop_h_spinbutton.Visible = !link_h_togglebutton.Active;
				((HBox.BoxChild)h_hbox[link_h_togglebutton]).Expand = !crop_h_spinbutton.Visible;
				((HBox.BoxChild)h_hbox[link_h_togglebutton]).Fill = !crop_h_spinbutton.Visible;
				link_h_togglebutton.Label = link_h_togglebutton.Active ? "Linked to width" : null;
				
				aspect_combobox.Sensitive = link_w_togglebutton.Active || link_h_togglebutton.Active;
				aspect_combobox.Visible = !custom_togglebutton.Active;
				
				aspect_spinbutton.Sensitive = link_w_togglebutton.Active || link_h_togglebutton.Active;
				aspect_spinbutton.Visible = custom_togglebutton.Active;
				
				custom_togglebutton.Sensitive = link_w_togglebutton.Active || link_h_togglebutton.Active;
				
				aspect_combobox.CheckResize();
			}
		}
		
		protected void OnAngleSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			if (mDragState != DragState.Round)
			{
				StartChangingParameters();
				((CrotateStageOperationParameters)Parameters).Angle = angle_spinbutton.Value;
				EndChangingParameters();
				OnUserModified();
			}
		}

		protected void OnCXSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			if (mDragState != DragState.Center)
			{
				StartChangingParameters();
				((CrotateStageOperationParameters)Parameters).Center = 
					new Point( c_x_spinbutton.Value, ((CrotateStageOperationParameters)Parameters).Center.Y);
				EndChangingParameters();
				OnUserModified();
			}
		}

		protected void OnCYSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			if (mDragState != DragState.Center)
			{
				StartChangingParameters();
				((CrotateStageOperationParameters)Parameters).Center = 
					new Point(((CrotateStageOperationParameters)Parameters).Center.X, c_y_spinbutton.Value);
				EndChangingParameters();
				OnUserModified();
			}
		}

		protected void OnCropWSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			if (mDragState != DragState.Corner)
			{
				StartChangingParameters();
				((CrotateStageOperationParameters)Parameters).CropWidth = crop_w_spinbutton.Value;
				EndChangingParameters();
				OnUserModified();
			}
		}

		protected void OnCropHSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			if (mDragState != DragState.Corner)
			{
				StartChangingParameters();
				((CrotateStageOperationParameters)Parameters).CropHeight = crop_h_spinbutton.Value;
				EndChangingParameters();
				OnUserModified();
			}
		}

		protected void OnAspectComboboxChanged (object sender, System.EventArgs e)
		{
			if (!mAspectComboboxSelfModifying)
			{
				TreeIter ti;
				aspect_combobox.GetActiveIter(out ti);
				int val = (int)ls.GetValue(ti, 1);
	
				StartChangingParameters();
				aspect_spinbutton.Value = val;
				((CrotateStageOperationParameters)Parameters).AspectRatioPreset = val;
				EndChangingParameters();
				OnUserModified();
			}
		}

		protected void OnAspectSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).AspectRatio = aspect_spinbutton.Value;
			EndChangingParameters();
			OnUserModified();			
		}

		protected void OnLinkWTogglebuttonToggled (object sender, System.EventArgs e)
		{
			if (link_w_togglebutton.Active)
			{
				link_h_togglebutton.Active = false;
				StartChangingParameters();
				((CrotateStageOperationParameters)Parameters).Mode = CrotateStageOperation.Mode.ProportionalHeightFixed;
				EndChangingParameters();
				OnUserModified();			
			}
			else
			{
				StartChangingParameters();
				((CrotateStageOperationParameters)Parameters).Mode = CrotateStageOperation.Mode.Disproportional;
				EndChangingParameters();
				OnUserModified();			
			}
			UpdateSensitive();
		}

		protected void OnLinkHTogglebuttonToggled (object sender, System.EventArgs e)
		{
			if (link_h_togglebutton.Active)
			{
				link_w_togglebutton.Active = false;
				
				StartChangingParameters();
				((CrotateStageOperationParameters)Parameters).Mode = CrotateStageOperation.Mode.ProportionalWidthFixed;
				EndChangingParameters();
				OnUserModified();			
			}
			else
			{
				StartChangingParameters();
				((CrotateStageOperationParameters)Parameters).Mode = CrotateStageOperation.Mode.Disproportional;
				EndChangingParameters();
				OnUserModified();			
			}
			UpdateSensitive();
		}

		protected void OnCustomCheckbuttonToggled (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).AspectRatioCustom = custom_togglebutton.Active;
			EndChangingParameters();
			OnUserModified();			
			UpdateSensitive();
		}

		protected override void HandleParametersChangedNotByUI ()
		{
			angle_spinbutton.Value = ((CrotateStageOperationParameters)Parameters).Angle;
			
			c_x_spinbutton.Value = ((CrotateStageOperationParameters)Parameters).Center.X;
			c_y_spinbutton.Value = ((CrotateStageOperationParameters)Parameters).Center.Y;
			crop_w_spinbutton.Value = ((CrotateStageOperationParameters)Parameters).CropWidth;
			crop_h_spinbutton.Value = ((CrotateStageOperationParameters)Parameters).CropHeight;
			aspect_spinbutton.Value = ((CrotateStageOperationParameters)Parameters).AspectRatio;
			custom_togglebutton.Active = ((CrotateStageOperationParameters)Parameters).AspectRatioCustom;
			
			TreeIter ti;
			if (ls.GetIterFirst(out ti))
			{
				do
				{
					int n = (int)ls.GetValue(ti, 1);
					if (n == ((CrotateStageOperationParameters)Parameters).AspectRatioPreset)
					{
						aspect_combobox.SetActiveIter(ti);
						break;
					}						
				} 
				while (ls.IterNext(ref ti));
			}
			
			link_w_togglebutton.Active = ((CrotateStageOperationParameters)Parameters).Mode == CrotateStageOperation.Mode.ProportionalHeightFixed;
			link_h_togglebutton.Active = ((CrotateStageOperationParameters)Parameters).Mode == CrotateStageOperation.Mode.ProportionalWidthFixed;
			UpdateSensitive();
		}
		
		public override bool ReportMouseButton (int x, int y, int width, int height, uint button_id, bool is_down)
		{
			if (width == 0 || height == 0) return false;
			
			CrotateStageOperationParameters pm = ((CrotateStageOperationParameters)Parameters);

			if (is_down)
			{
				// Checking if the user clicked the center dot
				Point C = new Point(pm.Center.X, pm.Center.Y);
				int scr_c_x = (int)(width * C.X);
				int scr_c_y = (int)(height * C.Y);
				
				if (new Gdk.Rectangle(
					scr_c_x - mDotRadius, 
					scr_c_y - mDotRadius, 
					2 * mDotRadius, 
					2 * mDotRadius).Contains(new Gdk.Point(x, y)))
				{
					mDragState = DragState.Center;
					mDragStartX = x;
					mDragStartY = y;
					mCenterDotStartX = scr_c_x;
					mCenterDotStartY = scr_c_y;
					return true;
				}
				
				// Checking if the user clicked the "round" dot
				Gdk.Point scr_rnd = new Gdk.Point(
					(int)(scr_c_x + (rt_corner_rot.X + rb_corner_rot.X) / 2),
					(int)(scr_c_y + (rt_corner_rot.Y + rb_corner_rot.Y) / 2));
				if (new Gdk.Rectangle(
					scr_rnd.X - mDotRadius, 
					scr_rnd.Y - mDotRadius, 
					2 * mDotRadius, 
					2 * mDotRadius).Contains(new Gdk.Point(x, y)))
				{
					mDragState = DragState.Round;
					mDragStartX = x;
					mDragStartY = y;
					return true;
				}
				
				// Checking if the user clicked the rb corner "square" dot
				Gdk.Point scr_rb = new Gdk.Point(
					(int)(scr_c_x + rb_corner_rot.X),
					(int)(scr_c_y + rb_corner_rot.Y));
				if (new Gdk.Rectangle(
					scr_rb.X - mDotRadius, 
					scr_rb.Y - mDotRadius, 
					2 * mDotRadius, 
					2 * mDotRadius).Contains(new Gdk.Point(x, y)))
				{
					mDragState = DragState.Corner;
					mDragStartX = x;
					mDragStartY = y;
					mCropWidthStart = pm.CropWidth;
					mCropHeightStart = pm.CropHeight;
					return true;
				}
				
			}
			else
			{
				if (mDragState != DragState.None)
				{
					mDragState = DragState.None;
					//OnUserModified();
				}
			}
			return false;
		}
		
		public override bool ReportMousePosition (int x, int y, int width, int height)
		{
			if (width == 0 || height == 0) return false; 
			
			CrotateStageOperationParameters pm = ((CrotateStageOperationParameters)Parameters);
			
			bool res = false;
			
			if (mDragState == DragState.Center)
			{
				int new_scr_x = mCenterDotStartX - mDragStartX + x;
				int new_scr_y = mCenterDotStartY - mDragStartY + y;
				
				pm.Center = new Point((double)new_scr_x / width, (double)new_scr_y / height);
				
				res = true;
			}
			else if (mDragState == DragState.Round)
			{
				Point scr_C = new Point(width * pm.Center.X, height * pm.Center.Y);
				Point cur = new Point(x, y);
				Vector cur_dir = new Vector(scr_C, cur);
				
				pm.Angle = 180.0 / Math.PI * Math.Atan2(cur_dir.Y, cur_dir.X);
				res = true;
			}
			else if (mDragState == DragState.Corner)
			{
				Point scr_C = new Point(width * pm.Center.X, height * pm.Center.Y);
				Point scr_cur = new Point(x, y);
				Point scr_rnd = new Point(
					(scr_C.X + (rt_corner_rot.X + rb_corner_rot.X) / 2),
					(scr_C.Y + (rt_corner_rot.Y + rb_corner_rot.Y) / 2));
				Point scr_start = new Point(mDragStartX, mDragStartY);
				
				// Rotated orts
				Vector v_w = new Vector(scr_C, scr_rnd);
				v_w /= v_w.Length;
				Vector v_h = new Vector(v_w.Y, -v_w.X);
				
				// Current diameter
				Vector D = new Vector(scr_C, scr_cur);
				double Dw = D * v_w;
				double Dh = D * v_h;
				
				// Starting diameter
				Vector D0 = new Vector(scr_C, scr_start);
				double D0w = D0 * v_w;
				double D0h = D0 * v_h;
				
				switch (pm.Mode)
				{
				case CatEye.Core.CrotateStageOperation.Mode.Disproportional:
					pm.CropWidth = mCropWidthStart * Dw / D0w;
					pm.CropHeight = mCropHeightStart * Dh / D0h;
					break;
				case CatEye.Core.CrotateStageOperation.Mode.ProportionalWidthFixed:
					pm.CropWidth = mCropWidthStart * Dw / D0w;
					break;
				case CatEye.Core.CrotateStageOperation.Mode.ProportionalHeightFixed:
					pm.CropHeight = mCropHeightStart * Dh / D0h;
					break;
				}
				res = true;
			}
			
			return res;
		}
		
		CatEye.Core.Point lt_corner_rot, rt_corner_rot, lb_corner_rot, rb_corner_rot;
		
		public override void DrawEditor (IBitmapView view)
		{
			if (view.Image == null || view.Image.Width == 0 || view.Image.Height == 0) return;

			Gdk.Drawable target = ((FloatPixmapViewWidget)view).GdkWindow;
			Gdk.Rectangle image_position = ((FloatPixmapViewWidget)view).CurrentImagePosition;
			
			CrotateStageOperationParameters pm = ((CrotateStageOperationParameters)Parameters);
			
			Gdk.GC gc = new Gdk.GC(target);
			
			// Draw center square dot
			Point C = new Point(pm.Center.X, pm.Center.Y);
			
			int scr_c_x = image_position.X + (int)(image_position.Width * C.X);
			int scr_c_y = image_position.Y + (int)(image_position.Height * C.Y);
			
			
			// Calculating new picture's real dimensions
			int trueWidth = image_position.Width, trueHeight = image_position.Height;
			double w1, h1;

			w1 = pm.CropWidth * image_position.Width;
			h1 = pm.CropHeight * image_position.Height;
			
			double asp_rat;

			if (pm.AspectRatioCustom)
				asp_rat = pm.AspectRatio;
			else
				asp_rat = pm.PresetAspectRatioValues[pm.AspectRatioPreset];
			
			switch (pm.Mode)
			{
			case CatEye.Core.CrotateStageOperation.Mode.Disproportional:
				trueWidth = (int)w1;
				trueHeight = (int)h1;
				break;
			case CatEye.Core.CrotateStageOperation.Mode.ProportionalWidthFixed:
				trueWidth = (int)w1;
				trueHeight = (int)(w1 / asp_rat);
				break;
			case CatEye.Core.CrotateStageOperation.Mode.ProportionalHeightFixed:
				trueWidth = (int)(h1 * asp_rat);
				trueHeight = (int)h1;
				break;
			}
			
			// Calculating new corners positions and "round" dot position
			double ang = pm.Angle / 180 * Math.PI;
			
			CatEye.Core.Point lt_corner = new CatEye.Core.Point(
				 - trueWidth / 2,
				 - trueHeight / 2);
			lt_corner_rot = CatEye.Core.Point.Rotate(lt_corner, ang, new Point(0, 0)); 
			Gdk.Point scr_lt = new Gdk.Point(
				(int)(scr_c_x + lt_corner_rot.X),
				(int)(scr_c_y + lt_corner_rot.Y));

			
			CatEye.Core.Point rt_corner = new CatEye.Core.Point(
				 + trueWidth / 2,
				 - trueHeight / 2);
			rt_corner_rot = CatEye.Core.Point.Rotate(rt_corner, ang, new Point(0, 0)); 
			Gdk.Point scr_rt = new Gdk.Point(
				(int)(scr_c_x + rt_corner_rot.X),
				(int)(scr_c_y + rt_corner_rot.Y));

			
			CatEye.Core.Point rb_corner = new CatEye.Core.Point(
				 + trueWidth / 2,
				 + trueHeight / 2);
			rb_corner_rot = CatEye.Core.Point.Rotate(rb_corner, ang, new Point(0, 0)); 
			Gdk.Point scr_rb = new Gdk.Point(
				(int)(scr_c_x + rb_corner_rot.X),
				(int)(scr_c_y + rb_corner_rot.Y));

			
			CatEye.Core.Point lb_corner = new CatEye.Core.Point(
				 - trueWidth / 2,
				 + trueHeight / 2);
			lb_corner_rot = CatEye.Core.Point.Rotate(lb_corner, ang, new Point(0, 0)); 
			Gdk.Point scr_lb = new Gdk.Point(
				(int)(scr_c_x + lb_corner_rot.X),
				(int)(scr_c_y + lb_corner_rot.Y));

			Gdk.Point scr_rnd = new Gdk.Point(
				(int)(scr_c_x + (rt_corner_rot.X + rb_corner_rot.X) / 2),
				(int)(scr_c_y + (rt_corner_rot.Y + rb_corner_rot.Y) / 2));

			
			// Drawing frame
			
			using (Cairo.Context cc = Gdk.CairoHelper.Create(target))
			{
				cc.LineCap = Cairo.LineCap.Round;
				cc.LineJoin = Cairo.LineJoin.Round;

				cc.Color = new Cairo.Color(0, 0, 0, 0.5);
				cc.LineWidth = 3;
				cc.MoveTo(scr_lt.X, scr_lt.Y);
				cc.LineTo(scr_lb.X, scr_lb.Y);
				cc.LineTo(scr_rb.X, scr_rb.Y);
				cc.LineTo(scr_rt.X, scr_rt.Y);
				cc.LineTo(scr_lt.X, scr_lt.Y);
				cc.ClosePath();
				cc.Stroke();
				
				cc.Color = new Cairo.Color(1, 1, 1, 1);
				cc.LineWidth = 1;
				cc.SetDash(new double[] {3, 3}, 0);
				cc.MoveTo(scr_lt.X, scr_lt.Y);
				cc.LineTo(scr_lb.X, scr_lb.Y);
				cc.LineTo(scr_rb.X, scr_rb.Y);
				cc.LineTo(scr_rt.X, scr_rt.Y);
				cc.LineTo(scr_lt.X, scr_lt.Y);
				cc.ClosePath();
				cc.Stroke();
				
			}
		
			
			// Drawing center "triangle" dot.
			using (Gdk.Pixbuf buf = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.triangle_dot.png"))
			{
				target.DrawPixbuf(gc, buf, 
					0, 0, (int)(scr_c_x - buf.Width / 2), (int)(scr_c_y - buf.Height / 2), 
					buf.Width, buf.Height, Gdk.RgbDither.None, 0, 0);
			}		
			
			// Drawing side "round" dot.
			using (Gdk.Pixbuf buf = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.round_dot.png"))
			{
				target.DrawPixbuf(gc, buf, 
					0, 0, 
					(int)(scr_rnd.X - buf.Width / 2), 
					(int)(scr_rnd.Y - buf.Height / 2), 
					buf.Width, buf.Height, Gdk.RgbDither.None, 0, 0);
			}

			// Drawing corner "square" dot.
			using (Gdk.Pixbuf buf = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.Widgets.res.square_dot.png"))
			{			
				target.DrawPixbuf(gc, buf, 
					0, 0, 
					(int)(scr_rb.X - buf.Width / 2), 
					(int)(scr_rb.Y - buf.Height / 2), 
					buf.Width, buf.Height, Gdk.RgbDither.None, 0, 0);
			}	
			
		}
	}
}

