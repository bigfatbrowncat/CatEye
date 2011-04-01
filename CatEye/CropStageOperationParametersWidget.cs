using System;
using System.Globalization;
namespace CatEye
{
	
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CropStageOperationParametersWidget : StageOperationParametersWidget
	{
		private double mLeft = 0, mRight = 1, mTop = 0, mBottom = 1;
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		public CropStageOperationParametersWidget ()
		{
			this.Build ();
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

		public void SetCrop(double left, double right, double top, double bottom)
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
				
				double left = Math.Min(x_down, x_cur);
				double top = Math.Min(y_down, y_cur);
				double right = Math.Max(x_down, x_cur);
				double bottom = Math.Max(y_down, y_cur);
				
				if (CutEdges(ref left, ref top, ref right, ref bottom))
				{


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
					
					double left = Math.Min(x_down, x_cur);
					double top = Math.Min(y_down, y_cur);
					double right = Math.Max(x_down, x_cur);
					double bottom = Math.Max(y_down, y_cur);
					
					bool update_crop = true;
					
					// Cutting edges

					if (CutEdges(ref left, ref top, ref right, ref bottom))
						SetCrop(left, right, top, bottom);
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
	}
}

