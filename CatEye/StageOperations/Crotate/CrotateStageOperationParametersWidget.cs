using System;
using CatEye.Core;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("CrotateStageOperation")]
	public partial class CrotateStageOperationParametersWidget : StageOperationParametersWidget
	{
		Gtk.ListStore ls;
		
		public CrotateStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();

			ls = new Gtk.ListStore(typeof(string), typeof(int));
			string[] ratioNames = ((CrotateStageOperationParameters)Parameters).PresetAspectRatioNames;
			for (int i = 0; i < ratioNames.Length; i++)
			{
				ls.AppendValues(ratioNames[i], i);
			}
			aspect_combobox.Model = ls;
			
			Gtk.TreeIter ti;
			ls.GetIterFirst(out ti);
			
			aspect_combobox.SetActiveIter(ti);
		}
		
		protected void UpdateSensitive()
		{
			crop_w_spinbutton.Sensitive = !link_w_togglebutton.Active;
			crop_h_spinbutton.Sensitive = !link_h_togglebutton.Active;
			
			aspect_combobox.Sensitive = link_w_togglebutton.Active || link_h_togglebutton.Active;
			aspect_combobox.Visible = !custom_checkbutton.Active;
			
			aspect_spinbutton.Sensitive = link_w_togglebutton.Active || link_h_togglebutton.Active;
			aspect_spinbutton.Visible = custom_checkbutton.Active;
			
			custom_checkbutton.Sensitive = link_w_togglebutton.Active || link_h_togglebutton.Active;
			
			aspect_combobox.CheckResize();
		}
		
		protected void OnAngleSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).Angle = angle_spinbutton.Value;
			EndChangingParameters();
			OnUserModified();			
		}

		protected void OnCXSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).Center = new Point(c_x_spinbutton.Value, c_y_spinbutton.Value);
			EndChangingParameters();
			OnUserModified();			
		}

		protected void OnCYSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).Center = new Point(c_x_spinbutton.Value, c_y_spinbutton.Value);
			EndChangingParameters();
			OnUserModified();			
		}

		protected void OnCropWSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).CropWidth = crop_w_spinbutton.Value;
			EndChangingParameters();
			OnUserModified();
		}

		protected void OnCropHSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).CropHeight = crop_h_spinbutton.Value;
			EndChangingParameters();
			OnUserModified();			
		}

		protected void OnAspectComboboxChanged (object sender, System.EventArgs e)
		{
			Gtk.TreeIter ti;
			aspect_combobox.GetActiveIter(out ti);
			int val = (int)ls.GetValue(ti, 1);

			StartChangingParameters();
			aspect_spinbutton.Value = val;
			((CrotateStageOperationParameters)Parameters).AspectRatioPreset = val;
			EndChangingParameters();
			OnUserModified();			
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
			((CrotateStageOperationParameters)Parameters).AspectRatioCustom = custom_checkbutton.Active;
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
			custom_checkbutton.Active = ((CrotateStageOperationParameters)Parameters).AspectRatioCustom;
			
			Gtk.TreeIter ti;
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
			return base.ReportMouseButton (x, y, width, height, button_id, is_down);
		}
		
		public override bool ReportMousePosition (int x, int y, int width, int height)
		{
			return base.ReportMousePosition (x, y, width, height);
		}
		
		public override void DrawEditor (Gdk.Drawable target, Gdk.Rectangle image_position)
		{
			CrotateStageOperationParameters pm = ((CrotateStageOperationParameters)Parameters);
			
			Gdk.GC gc = new Gdk.GC(target);
			
			// Draw center square dot
			Point C = new Point(pm.Center.X, pm.Center.Y);
			
			int scr_c_x = image_position.X + (int)(image_position.Width * C.X);
			int scr_c_y = image_position.Y + (int)(image_position.Height * C.Y);

			gc.RgbFgColor = new Gdk.Color(255, 0, 0);
			target.DrawRectangle(gc, false, new Gdk.Rectangle(scr_c_x - 3, scr_c_y - 3, 6, 6));
			
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
			
			// Calculating new corners positions
			double ang = pm.Angle / 180 * Math.PI;
			
			CatEye.Core.Point lt_corner = new CatEye.Core.Point(
				 - trueWidth / 2,
				 - trueHeight / 2);
			CatEye.Core.Point lt_corner_rot = CatEye.Core.Point.Rotate(lt_corner, ang, new Point(0, 0)); 
			Gdk.Point scr_lt = new Gdk.Point(
				(int)(scr_c_x + lt_corner_rot.X),
				(int)(scr_c_y + lt_corner_rot.Y));

			
			CatEye.Core.Point rt_corner = new CatEye.Core.Point(
				 + trueWidth / 2,
				 - trueHeight / 2);
			CatEye.Core.Point rt_corner_rot = CatEye.Core.Point.Rotate(rt_corner, ang, new Point(0, 0)); 
			Gdk.Point scr_rt = new Gdk.Point(
				(int)(scr_c_x + rt_corner_rot.X),
				(int)(scr_c_y + rt_corner_rot.Y));

			
			CatEye.Core.Point rb_corner = new CatEye.Core.Point(
				 + trueWidth / 2,
				 + trueHeight / 2);
			CatEye.Core.Point rb_corner_rot = CatEye.Core.Point.Rotate(rb_corner, ang, new Point(0, 0)); 
			Gdk.Point scr_rb = new Gdk.Point(
				(int)(scr_c_x + rb_corner_rot.X),
				(int)(scr_c_y + rb_corner_rot.Y));

			
			CatEye.Core.Point lb_corner = new CatEye.Core.Point(
				 - trueWidth / 2,
				 + trueHeight / 2);
			CatEye.Core.Point lb_corner_rot = CatEye.Core.Point.Rotate(lb_corner, ang, new Point(0, 0)); 
			Gdk.Point scr_lb = new Gdk.Point(
				(int)(scr_c_x + lb_corner_rot.X),
				(int)(scr_c_y + lb_corner_rot.Y));


			// Draw frame
			target.DrawPolygon(gc, false, new Gdk.Point[] {
				scr_lt, scr_rt, scr_rb, scr_lb
			});
		}
	}
}

