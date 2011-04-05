using System;
using System.Globalization;
namespace CatEye
{
	
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CropStageOperationParametersWidget : StageOperationParametersWidget
	{
		private double mLeft = 0, mRight = 1, mTop = 0, mBottom = 1, mAspectRatio = 3.0/2;
		private double sel_left, sel_top, sel_right, sel_bottom;
		private bool mLockAspectRatio = true;
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private int image_width, image_height;

		public CropStageOperationParametersWidget ()
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

		
		
		public double Left
		{
			get { return mLeft; }
			set 
			{
				if (value >= 0 && value <= 1 && value <= mRight)
				{
					mLeft = value;
					left_entry.Text = value.ToString();
				}
				else
					throw new IncorrectValueException();
			}
		}

		public double Right
		{
			get { return mRight; }
			set 
			{
				if (value >= 0 && value <= 1 && value >= mLeft)
				{
					mRight = value;
					right_entry.Text = value.ToString();
				}
				else
					throw new IncorrectValueException();
			}
		}

		public double Top
		{
			get { return mTop; }
			set 
			{
				if (value >= 0 && value <= 1 && value <= mBottom)
				{
					mTop = value;
					top_entry.Text = value.ToString();
				}
				else
					throw new IncorrectValueException();
			}
		}
		
		public double Bottom
		{
			get { return mBottom; }
			set 
			{
				if (value >= 0 && value <= 1 && value >= mTop)
				{
					mBottom = value;
					bottom_entry.Text = value.ToString();
				}
				else
					throw new IncorrectValueException();
			}
		}

		public void SetCrop(double left, double top, double right, double bottom)
		{
			if (left >= 0 && left <= right && right <= 1 &&
			    top >= 0 && top <= bottom && bottom <= 1)
			{
				mLeft = left; mRight = right; mTop = top; mBottom = bottom;
				
				left_entry.Text = mLeft.ToString();
				right_entry.Text = mRight.ToString();
				top_entry.Text = mTop.ToString();
				bottom_entry.Text = mBottom.ToString();
			}
		}
		
		protected virtual void OnLeftEntryChanged (object sender, System.EventArgs e)
		{
			double res;
			if (double.TryParse(left_entry.Text, NumberStyles.Float, nfi, out res))
			{
				if (res >= 0 && res <= 1 && mRight >= res)
				{
					mLeft = res;
					OnUserModified();
				}
			}
		}
		
		protected virtual void OnRightEntryChanged (object sender, System.EventArgs e)
		{
			double res;
			if (double.TryParse(right_entry.Text, NumberStyles.Float, nfi, out res))
			{
				if (res >= 0 && res <= 1 && mLeft <= res)
				{
					mRight = res;
					OnUserModified();
				}
			}
		}
		
		protected virtual void OnTopEntryChanged (object sender, System.EventArgs e)
		{
			double res;
			if (double.TryParse(top_entry.Text, NumberStyles.Float, nfi, out res))
			{
				if (res >= 0 && res <= 1 && mBottom >= res)
				{
					mTop = res;
					OnUserModified();
				}
			}
		}
		
		protected virtual void OnBottomEntryChanged (object sender, System.EventArgs e)
		{
			double res;
			if (double.TryParse(bottom_entry.Text, NumberStyles.Float, nfi, out res))
			{
				if (res >= 0 && res <= 1 && mTop <= res)
				{
					mBottom = res;
					OnUserModified();
				}
			}
		}
		private void CheckAspect(ref double left, ref double top, ref double right, ref double bottom, double real_aspect)
		{

			if (mLockAspectRatio)
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
			int i1 = (int)(image_position.Left + mLeft * image_position.Width);
			int i2 = (int)(image_position.Left + mRight * image_position.Width);
			int j1 = (int)(image_position.Top + mTop * image_position.Height);
			int j2 = (int)(image_position.Top + mBottom * image_position.Height);
			
			gc.Function = Gdk.Function.Invert;
			gc.SetLineAttributes(1, Gdk.LineStyle.Solid, Gdk.CapStyle.Butt, Gdk.JoinStyle.Bevel);
			
			Gdk.Rectangle rct = new Gdk.Rectangle(i1, j1, i2 - i1, j2 - j1);
			target.DrawRectangle(gc, false, rct);
			
			if (_mouse_is_down)
			{
				
				double x_cur2 = x_cur, y_cur2 = y_cur;
/*				
				if (mLockAspectRatio)
				{
					// Applying aspect ratio
					if (mAspectRatio > 1)
					{
						y_cur2 = y_down + Math.Sign(y_cur - y_down + 1e-10) * Math.Abs(x_cur - x_down) / mAspectRatio;
					}
					else
					{
						x_cur2 = x_down + Math.Sign(x_cur - x_down + 1e-10) * Math.Abs(y_cur - y_down) * mAspectRatio;
					}
				}
*/
				
				double left = Math.Min(x_down, x_cur2);
				double top = Math.Min(y_down, y_cur2);
				double right = Math.Max(x_down, x_cur2);
				double bottom = Math.Max(y_down, y_cur2);
	
				double real_aspect = mAspectRatio * image_height / image_width;
				
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

			double real_aspect = mAspectRatio * image_height / image_width;
			CheckAspect(ref mLeft, ref mTop, ref mRight, ref mBottom, real_aspect);
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
					
					SetCrop(sel_left, sel_top, sel_right, sel_bottom);
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
				mAspectRatio = res;
				mLockAspectRatio = true;
			}
			else
				mLockAspectRatio = false;
			
			double real_aspect = mAspectRatio * image_height / image_width;
			CheckAspect(ref mLeft, ref mTop, ref mRight, ref mBottom, real_aspect);
		}
	}
}

