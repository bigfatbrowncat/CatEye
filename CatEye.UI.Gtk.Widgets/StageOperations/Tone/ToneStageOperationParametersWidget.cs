using System;
using System.Globalization;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	[StageOperationID("ToneStageOperation")]
	[StageOperationParametersEditModeSupported(true)]
	public partial class ToneStageOperationParametersWidget : StageOperationParametersWidget
	{
		private uint mButtonDown;
		// Points showing where is the center of area "would-be-white" and where is radius pointing from the center of this area
		private Point mCenter = null, mSide = null;
		private bool mIsMouseDown = false;
		
		public ToneStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
			
			softness_hscale.Adjustment.Lower = 0.001;
			softness_spinbutton.Adjustment.Lower = 0.001;
			
			toneselectorwidget1.DarkToneSelected += HandleToneSelectorWidgetDarkToneSelected;
			toneselectorwidget1.LightToneSelected += HandleToneSelectorWidgetLightToneSelected;

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
			Tone tn = ((ToneStageOperationParameters)Parameters).DarkTone;
			toneselectorwidget1.SelectedDarkTone = tn;

			_EdgeIsChanging = true;
			edge_hscale.Value = ((ToneStageOperationParameters)Parameters).Edge;
			edge_spinbutton.Value = ((ToneStageOperationParameters)Parameters).Edge;
			_EdgeIsChanging = false;

			_SoftnessIsChanging = true;
			softness_hscale.Value = ((ToneStageOperationParameters)Parameters).Softness;
			softness_spinbutton.Value = ((ToneStageOperationParameters)Parameters).Softness;
			_SoftnessIsChanging = false;
		}

		protected void HandleToneSelectorWidgetDarkToneSelected (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((ToneStageOperationParameters)Parameters).DarkTone = toneselectorwidget1.SelectedDarkTone;
			EndChangingParameters();
			OnUserModified();
		}
		void HandleToneSelectorWidgetLightToneSelected (object sender, EventArgs e)
		{
			StartChangingParameters();
			((ToneStageOperationParameters)Parameters).LightTone = toneselectorwidget1.SelectedLightTone;
			EndChangingParameters();
			OnUserModified();
		}
		
		protected void OnAlphaVscaleChangeValue (object o, ChangeValueArgs args)
		{
			toneselectorwidget1.Alpha = alpha_vscale.Value;
		}

		protected void OnHlInvSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeEdge(edge_spinbutton.Value, EdgeChanger.SpinButton);
		}

		public override bool ReportMouseButton (IBitmapCore image, int x, int y, int width, int height, uint button_id, bool is_down)
		{
			if (width == 0 || height == 0) return false;

			if (is_down) 
			{
				mIsMouseDown = true;
				mButtonDown = button_id;
				mCenter = new Point((double)x / width, (double)y / height);
				mSide = new Point((double)(x + 1) / width, (double)(y + 1) / height);
			}
			else
			{
				int points = 100;
				
				if (mButtonDown == 1 /* left */)
				{
					ToneStageOperationParameters pms = (ToneStageOperationParameters)Parameters;
						
					pms.DarkTone = image.FindDarkTone(
						pms.LightTone, 
						pms.Edge, 
						pms.Softness, 
						mCenter, 
						Point.Distance(mCenter, mSide), points);
				}
				else if (mButtonDown == 3 /* right */)
				{
					ToneStageOperationParameters pms = (ToneStageOperationParameters)Parameters;
						
					pms.LightTone = image.FindLightTone(
						pms.DarkTone, 
						pms.Edge, 
						pms.Softness, 
						mCenter, 
						Point.Distance(mCenter, mSide), points);
				}
				
				mIsMouseDown = false;
				mCenter = null; mSide = null;
			}
			return true;
		}
		
		public override bool ReportMousePosition (IBitmapCore image, int x, int y, int width, int height)
		{
			if (width == 0 || height == 0) return false;

			if (mIsMouseDown)
			{
				mSide = new Point((double)x / width, (double)y / height);
				if (Point.Distance(mCenter, mSide) < Math.Sqrt(2) / Math.Sqrt((double)width * height)) 
				{
					mSide = new Point((double)(x + 1) / width, (double)(y + 1) / height);
				}
				
				return true;
			}
			return false;
		}

		public override void DrawEditor (IBitmapCore image, IBitmapView view)
		{
			if (mCenter != null && mSide != null)
			{
				Gdk.Drawable target = ((FloatPixmapViewWidget)view).GdkWindow;
				Gdk.Rectangle image_position = ((FloatPixmapViewWidget)view).CurrentImagePosition;
				
				
				Gdk.GC gc = new Gdk.GC(target);
				gc.Function = Gdk.Function.Copy;
				
				Point scrCenter = new Point(image_position.X + (int)(mCenter.X * view.Image.Width),	
				                            image_position.Y + (int)(mCenter.Y * view.Image.Height));
				Point scrSide = new Point(image_position.X + (int)(mSide.X * view.Image.Width),	
				                          image_position.Y + (int)(mSide.Y * view.Image.Height));
							
				
				if (mIsMouseDown)
				{
					using (Cairo.Context cc = Gdk.CairoHelper.Create(target))
					{
						cc.LineCap = Cairo.LineCap.Round;
						cc.LineJoin = Cairo.LineJoin.Round;
		
						if (mButtonDown == 1 /* left */)
							cc.Color = new Cairo.Color(0, 0, 0, 0.5);
						else if (mButtonDown == 3 /* right */)
							cc.Color = new Cairo.Color(1, 1, 1, 0.5);
								
						cc.LineWidth = 3;
						cc.Arc(scrCenter.X, scrCenter.Y, Point.Distance(scrCenter, scrSide), 0, 2 * Math.PI);
						cc.ClosePath();
						cc.Stroke();
						
						if (mButtonDown == 1 /* left */)
							cc.Color = new Cairo.Color(1, 1, 1, 1);
						else if (mButtonDown == 3 /* right */)
							cc.Color = new Cairo.Color(0, 0, 0, 0);

						cc.LineWidth = 1;
						cc.SetDash(new double[] {3, 3}, 0);
						cc.Arc(scrCenter.X, scrCenter.Y, Point.Distance(scrCenter, scrSide), 0, 2 * Math.PI);
						cc.ClosePath();
						cc.Stroke();
						
					}					
				}
			}
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
