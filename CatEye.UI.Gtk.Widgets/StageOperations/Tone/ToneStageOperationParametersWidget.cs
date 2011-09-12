using System;
using System.Globalization;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	[StageOperationID("ToneStageOperation")]
	public partial class ToneStageOperationParametersWidget : StageOperationParametersWidget
	{
		private int mDotRadius = 6;
		// Points showing where is the center of area "would-be-white" and where is radius pointing from the center of this area
		private Point mCenterDot = new Point(0.5, 0.5), mSideDot = new Point(0.6, 0.5);
		private enum ControlState { None, Center, Side };
		private ControlState mControlState = ControlState.None, mHightlightState = ControlState.None;
		private int mDragStartX, mDragStartY;
		
		public ToneStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
			
			softness_hscale.Adjustment.Lower = 0.001;
			softness_spinbutton.Adjustment.Lower = 0.001;
			
			toneselectorwidget1.ToneSelected += OnToneselectorwidget1ToneSelected;
			toneselectorwidget1.SelectedToneChanged += OnToneselectorwidget1SelectedToneChanged;
			toneselectorwidget1.Alpha = 0.5;
		}
		
		private bool _EdgeIsChanging = false;
		private bool _SoftnessIsChanging = false;
		
		protected enum EdgeChanger { HScale, SpinButton }
		protected enum SoftnessChanger { HScale, SpinButton }
		
		protected void ChangeEdge(double new_value, EdgeChanger changer)
		{
			if (!_EdgeIsChanging)
			{
				_EdgeIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != EdgeChanger.HScale)
					edge_hscale.Value = new_value;
				
				if (changer != EdgeChanger.SpinButton)
					edge_spinbutton.Value = new_value;
				
				((ToneStageOperationParameters)Parameters).Edge = new_value;
				
				EndChangingParameters();
				OnUserModified();
				_EdgeIsChanging = false;
			}
		}
		protected void ChangeSoftness(double new_value, SoftnessChanger changer)
		{
			if (!_SoftnessIsChanging)
			{
				_SoftnessIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != SoftnessChanger.HScale)
					softness_hscale.Value = new_value;
				
				if (changer != SoftnessChanger.SpinButton)
					softness_spinbutton.Value = new_value;
				
				((ToneStageOperationParameters)Parameters).Softness = new_value;
				
				EndChangingParameters();
				OnUserModified();
				_SoftnessIsChanging = false;
			}
		}		
		
		protected override void HandleParametersChangedNotByUI ()
		{
			Tone tn = ((ToneStageOperationParameters)Parameters).Tone;
			toneselectorwidget1.SelectedTone = tn;

			_EdgeIsChanging = true;
			edge_hscale.Value = ((ToneStageOperationParameters)Parameters).Edge;
			edge_spinbutton.Value = ((ToneStageOperationParameters)Parameters).Edge;
			_EdgeIsChanging = false;

			_SoftnessIsChanging = true;
			softness_hscale.Value = ((ToneStageOperationParameters)Parameters).Softness;
			softness_spinbutton.Value = ((ToneStageOperationParameters)Parameters).Softness;
			_SoftnessIsChanging = false;
		}

		protected void OnToneselectorwidget1SelectedToneChanged (object sender, System.EventArgs e)
		{
			if (toneselectorwidget1.SelectedTone != null)
			{
				r_label.Markup = "R: <b>" + toneselectorwidget1.SelectedTone.R.ToString("0.00") + "</b>";
				g_label.Markup = "G: <b>" + toneselectorwidget1.SelectedTone.G.ToString("0.00") + "</b>";
				b_label.Markup = "B: <b>" + toneselectorwidget1.SelectedTone.B.ToString("0.00") + "</b>";
			}
		}

		protected void OnToneselectorwidget1ToneSelected (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((ToneStageOperationParameters)Parameters).Tone = toneselectorwidget1.SelectedTone;
			EndChangingParameters();
			OnUserModified();
		}

		protected void OnAlphaVscaleChangeValue (object o, ChangeValueArgs args)
		{
			toneselectorwidget1.Alpha = alpha_vscale.Value;
		}

		protected void OnHlInvHscaleChangeValue (object o, ChangeValueArgs args)
		{
		}

		protected void OnHlInvSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeEdge(edge_spinbutton.Value, EdgeChanger.SpinButton);
		}

		public override bool ReportMouseButton (int x, int y, int width, int height, uint button_id, bool is_down)
		{
			//Console.WriteLine("[Tone] {0}:{1} <{2}:{3}> {4} [{5}]", x.ToString(), y.ToString(), width.ToString(), height.ToString(), button_id.ToString(), is_down?"D":"U");
			
			//ToneStageOperationParameters pm = ((ToneStageOperationParameters)Parameters);
			// getting screen coordinates of center manipulation dot
			int scrCenterX = (int)(width * mCenterDot.X);
			int scrCenterY = (int)(height * mCenterDot.Y);
			// getting screen coordinates of side manipulation dot
			int scrSideX = (int)(width * mSideDot.X);
			int scrSideY = (int)(height * mSideDot.Y);
			
			bool onCenterDot = new Gdk.Rectangle(
					scrCenterX - mDotRadius, 
					scrCenterY - mDotRadius, 
					2 * mDotRadius, 
					2 * mDotRadius).Contains(new Gdk.Point(x, y));
			bool onSideDot = new Gdk.Rectangle(
					scrSideX - mDotRadius, 
					scrSideY - mDotRadius, 
					2 * mDotRadius, 
					2 * mDotRadius).Contains(new Gdk.Point(x, y));

			if (is_down) {
				
				if (onCenterDot)
				{
					// user clicked on center manipulation point
					mControlState = ControlState.Center;
					mDragStartX = x;
					mDragStartY = y;
					return true;
				} 

				if (onSideDot)
				{
					// user clicked on side manipulation point
					mControlState = ControlState.Side;
					mDragStartX = x;
					mDragStartY = y;
					return true;
				} 
				
				// user clicked somewhere else, no need to do anything
				mControlState = ControlState.None;
				return false;
			}
			else
			{
				if (mControlState != ControlState.None)
				{
					// user changed something, we need to recalculate the saturation and all
					// not done yet
					mControlState = ControlState.None;
					if (onCenterDot)
					{
						mHightlightState = ControlState.Center;
					}
					else
					if (onSideDot)
					{
						mHightlightState = ControlState.Side;
					} 
					else
					{
						mHightlightState = ControlState.None;
					}
					return true;
				}
			}
			return false;
		}
		
		public override bool ReportMousePosition (int x, int y, int width, int height)
		{
			if (mControlState == ControlState.Center)
			{
				double size = mSideDot.X - mCenterDot.X;
				mCenterDot = new Point((double)x/width, (double)y/height);
				mSideDot = new Point(mCenterDot.X + size, mCenterDot.Y);
				mHightlightState = ControlState.Center;
				return true;
			}
			
			if (mControlState == ControlState.Side)
			{
				mSideDot = new Point((double)x/width, mCenterDot.Y);
				mHightlightState = ControlState.Side;
				return true;
			}
			
			// getting screen coordinates of center manipulation dot
			int scrCenterX = (int)(width * mCenterDot.X);
			int scrCenterY = (int)(height * mCenterDot.Y);
			// getting screen coordinates of side manipulation dot
			int scrSideX = (int)(width * mSideDot.X);
			int scrSideY = (int)(height * mSideDot.Y);
			
			if (new Gdk.Rectangle(
				scrCenterX - mDotRadius, 
				scrCenterY - mDotRadius, 
				2 * mDotRadius, 
				2 * mDotRadius).Contains(new Gdk.Point(x, y)))
			{
				// mouse pointer is over center manipulation point
				mHightlightState = ControlState.Center;
				return true;
			} 

			if (new Gdk.Rectangle(
				scrSideX - mDotRadius, 
				scrSideY - mDotRadius, 
				2 * mDotRadius, 
				2 * mDotRadius).Contains(new Gdk.Point(x, y)))
			{
				// mouse pointer is over side manipulation point
				mHightlightState = ControlState.Side;
				return true;
			} 
			else
			{
				bool result = mHightlightState != ControlState.None;
				mHightlightState = ControlState.None;
				return result;
			}

			return false;
		}
				
		public override void DrawEditor (IBitmapView view)
		{
			Gdk.Drawable target = ((FloatPixmapViewWidget)view).GdkWindow;
			Gdk.Rectangle image_position = ((FloatPixmapViewWidget)view).CurrentImagePosition;
			
			ToneStageOperationParameters pm = ((ToneStageOperationParameters)Parameters);
			
			Gdk.GC gc = new Gdk.GC(target);
			gc.Function = Gdk.Function.Copy;
			
			int scrCenterX = image_position.X + (int)(image_position.Width * mCenterDot.X);
			int scrCenterY = image_position.Y + (int)(image_position.Height * mCenterDot.Y);
			int scrSideX = image_position.X + (int)(image_position.Width * mSideDot.X);
			int scrSideY = image_position.Y + (int)(image_position.Height * mSideDot.Y);
			
			// Draw the circle
			int scrRadius = Math.Abs(scrSideX - scrCenterX);
			gc.RgbFgColor = new Gdk.Color(255, 255, 255);
			target.DrawArc(gc, false, 
				scrCenterX - scrRadius - 1, 
				scrCenterY - scrRadius - 1, 
				2 * scrRadius + 2, 
				2 * scrRadius + 2,
				0, 64*360);
			target.DrawArc(gc, false, 
				scrCenterX - scrRadius + 1, 
				scrCenterY - scrRadius + 1, 
				2 * scrRadius - 2, 
				2 * scrRadius - 2,
				0, 64*360);
			gc.RgbFgColor = new Gdk.Color(0, 0, 0);
			target.DrawArc(gc, false, 
				scrCenterX - scrRadius, 
				scrCenterY - scrRadius, 
				2 * scrRadius, 
				2 * scrRadius,
				0, 64*360);

			// Draw center square dot
			int dotRadius = (mHightlightState == ControlState.Center)?mDotRadius:mDotRadius/2;
			gc.RgbFgColor = new Gdk.Color(0, 0, 0);
			target.DrawRectangle(gc, true, new Gdk.Rectangle(
				scrCenterX - dotRadius, 
				scrCenterY - dotRadius, 
				2 * dotRadius, 
				2 * dotRadius));
			gc.RgbFgColor = new Gdk.Color(255, 255, 255);
			target.DrawRectangle(gc, false, new Gdk.Rectangle(
				scrCenterX - dotRadius, 
				scrCenterY - dotRadius, 
				2 * dotRadius, 
				2 * dotRadius));

			// Draw side round dot
			dotRadius = (mHightlightState == ControlState.Side)?mDotRadius:mDotRadius/2;
			gc.RgbFgColor = new Gdk.Color(0, 0, 0);
			target.DrawArc(gc, true, 
				scrSideX - dotRadius, 
				scrSideY - dotRadius, 
				2 * dotRadius, 
				2 * dotRadius,
				0, 64*360);
			gc.RgbFgColor = new Gdk.Color(255, 255, 255);
			target.DrawArc(gc, false, 
				scrSideX - dotRadius, 
				scrSideY - dotRadius, 
				2 * dotRadius, 
				2 * dotRadius,
				0, 64*360);
		}

		protected void OnEdgeHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeEdge(edge_hscale.Value, EdgeChanger.HScale);
		}

		protected void OnSoftnessHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeSoftness(softness_hscale.Value, SoftnessChanger.HScale);
		}

		protected void OnEdgeSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeEdge(edge_hscale.Value, EdgeChanger.SpinButton);
		}

		protected void OnSoftnessSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeSoftness(softness_hscale.Value, SoftnessChanger.SpinButton);
		}
	}
}
