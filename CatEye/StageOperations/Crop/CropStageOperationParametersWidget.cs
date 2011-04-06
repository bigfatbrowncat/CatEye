using System;
using System.Globalization;
namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CropStageOperationParametersWidget : StageOperationParametersWidget
	{
		private double sel_left, sel_top, sel_right, sel_bottom;
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private int image_width, image_height;
		
		public CropStageOperationParametersWidget (CropStageOperationParameters parameters):
			base(parameters)
		{
			this.Build ();
			
			Gtk.ListStore ls = new Gtk.ListStore(typeof(string), typeof(double));
			ls.AppendValues("1.5 - Photo album (3:2)", 3.0/2);
			ls.AppendValues("0.667 - Photo portrait (2:3)", 2.0/3);
			ls.AppendValues("1.333 - Screen album (4:3)", 4.0/3);
			ls.AppendValues("0.75 - Screen portrait (3:4)", 3.0/4);
			ls.AppendValues("Don't preserve", 0);
			aspect_comboboxentry.Model = ls;
			
			Gtk.TreeIter ti;
			ls.GetIterFirst(out ti);
			
			aspect_comboboxentry.SetActiveIter(ti);
		}

		protected virtual void OnLeftEntryChanged (object sender, System.EventArgs e)
		{
			double res;
			if (double.TryParse(left_entry.Text, NumberStyles.Float, nfi, out res))
			{
				try
				{
					StartChangingParameters();
					((CropStageOperationParameters)Parameters).Left = res;
					EndChangingParameters();
					OnUserModified();
				}
				catch (IncorrectValueException)
				{
				}
			}
		}
		
		protected virtual void OnRightEntryChanged (object sender, System.EventArgs e)
		{
			double res;
			if (double.TryParse(right_entry.Text, NumberStyles.Float, nfi, out res))
			{
				try
				{
					StartChangingParameters();
					((CropStageOperationParameters)Parameters).Right = res;
					EndChangingParameters();
					OnUserModified();
				}
				catch (IncorrectValueException)
				{
				}
			}
		}
		
		protected virtual void OnTopEntryChanged (object sender, System.EventArgs e)
		{
			double res;
			if (double.TryParse(top_entry.Text, NumberStyles.Float, nfi, out res))
			{
				try
				{
					StartChangingParameters();
					((CropStageOperationParameters)Parameters).Top = res;
					EndChangingParameters();
					OnUserModified();
				}
				catch (IncorrectValueException)
				{
				}
			}
		}
		
		protected virtual void OnBottomEntryChanged (object sender, System.EventArgs e)
		{
			double res;
			if (double.TryParse(bottom_entry.Text, NumberStyles.Float, nfi, out res))
			{
				try
				{
					((CropStageOperationParameters)Parameters).Bottom = res;
					OnUserModified();
				}
				catch (IncorrectValueException)
				{
				}
			}
		}
		
		protected override void HandleParametersChangedNotByUI ()
		{
			left_entry.Text = ((CropStageOperationParameters)Parameters).Left.ToString("0.000");
			top_entry.Text = ((CropStageOperationParameters)Parameters).Top.ToString("0.000");
			right_entry.Text = ((CropStageOperationParameters)Parameters).Right.ToString("0.000");
			bottom_entry.Text = ((CropStageOperationParameters)Parameters).Bottom.ToString("0.000");

			double ar = ((CropStageOperationParameters)Parameters).AspectRatio;
			if (((CropStageOperationParameters)Parameters).LockAspectRatio)
			{
				Gtk.ListStore ls = (Gtk.ListStore)aspect_comboboxentry.Model;
				Gtk.TreeIter ti;
				ls.GetIterFirst(out ti);
				while (ls.IterIsValid(ti) && 
					Math.Abs((double)ls.GetValue(ti, 1) - ar) > 0.0011)
				{
					ls.IterNext(ref ti);
				}
				
				if (ls.IterIsValid(ti))
					aspect_comboboxentry.SetActiveIter(ti);
				else
				{
					aspect_comboboxentry.Entry.Text = ((CropStageOperationParameters)Parameters).AspectRatio.ToString("0.000");
				}
			}
			else
			{
				Gtk.ListStore ls = (Gtk.ListStore)aspect_comboboxentry.Model;
				Gtk.TreeIter ti;
				ls.GetIterFirst(out ti);
				while (ls.IterIsValid(ti) && (double)ls.GetValue(ti, 1) > 0)
				{
					ls.IterNext(ref ti);
				}
				if (ls.IterIsValid(ti))
				{
					
				}
			}
		}
		
		private void CheckAspect(ref double left, ref double top, ref double right, ref double bottom, double real_aspect)
		{

			if (((CropStageOperationParameters)Parameters).LockAspectRatio)
			{
				if (y_cur >= y_down && x_cur >= x_down)
				{
					bottom = top + (right - left) / real_aspect;
					if (bottom > 1)
					{
						bottom = 1;
						right = left + (bottom - top) * real_aspect;
					}
				} 
				else if (y_cur < y_down && x_cur >= x_down)
				{
					top = bottom - (right - left) / real_aspect;
					if (top < 0)
					{
						top = 0;
						right = left + (bottom - top) * real_aspect;
					}
				}
				else if (y_cur >= y_down && x_cur < x_down)
				{
					bottom = top + (right - left) / real_aspect;
					if (bottom > 1)
					{
						bottom = 1;
						left = right - (bottom - top) * real_aspect;
					}
				} 
				else if (y_cur < y_down && x_cur < x_down)
				{
					top = bottom - (right - left) / real_aspect;
					if (top < 0)
					{
						top = 0;
						left = right - (bottom - top) * real_aspect;
					}
				}
			}
		}
		
		public override void DrawToDrawable (Gdk.Drawable target, Gdk.Rectangle image_position)
		{
			base.DrawToDrawable (target, image_position);
			
			// Drawing currently selected
			Gdk.GC gc = new Gdk.GC(target);
			int i1 = (int)(image_position.Left + ((CropStageOperationParameters)Parameters).Left * image_position.Width);
			int i2 = (int)(image_position.Left + ((CropStageOperationParameters)Parameters).Right * image_position.Width);
			int j1 = (int)(image_position.Top + ((CropStageOperationParameters)Parameters).Top * image_position.Height);
			int j2 = (int)(image_position.Top + ((CropStageOperationParameters)Parameters).Bottom * image_position.Height);
			
			gc.Function = Gdk.Function.Invert;
			gc.SetLineAttributes(1, Gdk.LineStyle.Solid, Gdk.CapStyle.Butt, Gdk.JoinStyle.Bevel);
			
			Gdk.Rectangle rct = new Gdk.Rectangle(i1, j1, i2 - i1, j2 - j1);
			target.DrawRectangle(gc, false, rct);
			
			if (_mouse_is_down)
			{
				double left = Math.Min(x_down, x_cur);
				double top = Math.Min(y_down, y_cur);
				double right = Math.Max(x_down, x_cur);
				double bottom = Math.Max(y_down, y_cur);
	
				double real_aspect = ((CropStageOperationParameters)Parameters).AspectRatio * image_height / image_width;
				
				if (CutEdges(ref left, ref top, ref right, ref bottom))
				{
					CheckAspect(ref left, ref top, ref right, ref bottom, real_aspect);
					
					sel_left = left; sel_top = top; sel_right = right; sel_bottom = bottom;
					
					int ti1 = (int)(image_position.Left + left * image_position.Width);
					int ti2 = (int)(image_position.Left + right * image_position.Width);
					int tj1 = (int)(image_position.Top + top * image_position.Height);
					int tj2 = (int)(image_position.Top + bottom * image_position.Height);
					
					rct = new Gdk.Rectangle(ti1, tj1, ti2 - ti1, tj2 - tj1);
					gc.SetLineAttributes(1, Gdk.LineStyle.OnOffDash, Gdk.CapStyle.Butt, Gdk.JoinStyle.Bevel);
					gc.SetDashes(0, new sbyte[] { 2, 2 }, 2);
					target.DrawRectangle(gc, false, rct);
				}
			}
			
		}
		
		private double x_down, y_down, x_cur, y_cur;
		private bool _mouse_is_down = false;
		
		private static bool CutEdges(ref double left, ref double top, ref double right, ref double bottom)
		{
			bool update_crop = true;
			if (left < 0)
			{
				if (right > 0) left = 0;
				else update_crop = false;
			}
			if (top < 0)
			{
				if (bottom > 0) top = 0;
				else update_crop = false;
			}
			if (right > 1)
			{
				if (left < 1) right = 1;
				else update_crop = false;
			}
			if (bottom > 1)
			{
				if (top < 1) bottom = 1;
				else update_crop = false;
			}
			return update_crop;
		}
		
		public override void ReportImageChanged (int image_width, int image_height)
		{
			this.image_width = image_width;
			this.image_height = image_height;

			CheckAspect();
		}
		
		private void CheckAspect()
		{
			double real_aspect = ((CropStageOperationParameters)Parameters).AspectRatio * image_height / image_width;

			double l = ((CropStageOperationParameters)Parameters).Left, 
				   t = ((CropStageOperationParameters)Parameters).Top, 
				   r = ((CropStageOperationParameters)Parameters).Right,
				   b = ((CropStageOperationParameters)Parameters).Bottom;
			CheckAspect(ref l, ref t, ref r, ref b, real_aspect);
			((CropStageOperationParameters)Parameters).SetCrop(l, t, r, b);
		}
		
		public override bool ReportMouseButton (double x, double y, uint button_id, bool is_down)
		{
			if (button_id == 1)	// Left button
			{
				if (is_down)
				{
					_mouse_is_down = true;
					x_down = x; y_down = y;
					x_cur = x; y_cur = y;
					return true;
				}
				else
				{
					_mouse_is_down = false;
					x_cur = x; y_cur = y;
					((CropStageOperationParameters)Parameters).SetCrop(sel_left, sel_top, sel_right, sel_bottom);
				}
				return true;
			}
			return false;
		}
		
		public override bool ReportMousePosition (double x, double y)
		{
			if (_mouse_is_down)
			{
				x_cur = x; y_cur = y;
				return true;
			}
			return false;
		}

		protected void OnAspectComboboxentryChanged (object sender, System.EventArgs e)
		{
			Gtk.TreeIter ti;
			Gtk.ListStore ls = (Gtk.ListStore)aspect_comboboxentry.Model;
			aspect_comboboxentry.GetActiveIter(out ti);
			
			object val = ls.GetValue(ti, 1);
			double res = 0;
			if (val == null)
			{
				if (double.TryParse(aspect_comboboxentry.ActiveText, NumberStyles.Float, nfi, out res))
				{
					
				}
				
			}
			else
			{
				res = (double)val;
			}
			
			if (res > 0)
			{
				StartChangingParameters();
				((CropStageOperationParameters)Parameters).AspectRatio = res;
				((CropStageOperationParameters)Parameters).LockAspectRatio = true;
				EndChangingParameters();
			}
			else
			{
				StartChangingParameters();
				((CropStageOperationParameters)Parameters).LockAspectRatio = false;
				EndChangingParameters();
			}
			
			CheckAspect();
		}
	}
}

