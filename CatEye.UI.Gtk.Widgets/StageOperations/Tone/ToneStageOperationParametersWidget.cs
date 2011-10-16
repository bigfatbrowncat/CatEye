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

		private Point mScrCenter = null, mScrSide = null;
		private bool mIsMouseDown = false;
		private Tone mAnalyzedDarkTone, mAnalyzedLightTone;
		
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
					edge_hscale.Value = Math.Log(new_value + 1, 11);
				
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
			if (((ToneStageOperationParameters)Parameters).AutoDarkTone)
			{
				toneselectorwidget1.DarkToneSelectorSymbol = ToneSelectorSymbol.None;
			}
			else
			{
				toneselectorwidget1.DarkToneSelectorSymbol = ToneSelectorSymbol.Donut;
				toneselectorwidget1.SelectedDarkTone = ((ToneStageOperationParameters)Parameters).DarkTone;
			}

			if (((ToneStageOperationParameters)Parameters).AutoLightTone)
			{
				toneselectorwidget1.LightToneSelectorSymbol = ToneSelectorSymbol.None;
			}
			else
			{
				toneselectorwidget1.LightToneSelectorSymbol = ToneSelectorSymbol.Donut;
				toneselectorwidget1.SelectedLightTone = ((ToneStageOperationParameters)Parameters).LightTone;
			}
			

			_EdgeIsChanging = true;
			edge_hscale.Value = Math.Log(((ToneStageOperationParameters)Parameters).Edge + 1, 11);
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
			((ToneStageOperationParameters)Parameters).AutoDarkTone = false;
			((ToneStageOperationParameters)Parameters).DarkTone = toneselectorwidget1.SelectedDarkTone;
			toneselectorwidget1.DarkToneSelectorSymbol = ToneSelectorSymbol.Donut;
			EndChangingParameters();
			OnUserModified();
		}
		void HandleToneSelectorWidgetLightToneSelected (object sender, EventArgs e)
		{
			StartChangingParameters();
			((ToneStageOperationParameters)Parameters).AutoLightTone = false;
			((ToneStageOperationParameters)Parameters).LightTone = toneselectorwidget1.SelectedLightTone;
			toneselectorwidget1.LightToneSelectorSymbol = ToneSelectorSymbol.Donut;
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

		public override bool ReportMouseButton (int x, int y, int width, int height, uint button_id, bool is_down)
		{
			if (width == 0 || height == 0) return false;

			if (is_down) 
			{
				mIsMouseDown = true;
				mButtonDown = button_id;
				mScrCenter = new Point(x, y);
				mScrSide = new Point(x + 1, y + 1);
			}
			else
			{
				// Setting the Parameters
				if (mButtonDown == 1 /* left */)
				{
					ToneStageOperationParameters pms = (ToneStageOperationParameters)Parameters;
					
					pms.AutoDarkCenter = new Point(mScrCenter.X / width, mScrCenter.Y / height);
					pms.AutoDarkRadius = Point.Distance(mScrCenter, mScrSide) * 2 / (width + height);
					pms.AutoDarkTone = true;
					toneselectorwidget1.DarkToneSelectorSymbol = ToneSelectorSymbol.None;
				}
				else if (mButtonDown == 3 /* right */)
				{
					ToneStageOperationParameters pms = (ToneStageOperationParameters)Parameters;
						
					pms.AutoLightCenter = new Point(mScrCenter.X / width, mScrCenter.Y / height);
					pms.AutoLightRadius = Point.Distance(mScrCenter, mScrSide) * 2 / (width + height);
					pms.AutoLightTone = true;
					toneselectorwidget1.LightToneSelectorSymbol = ToneSelectorSymbol.None;
				}
				
				mIsMouseDown = false;
				mScrCenter = null; mScrSide = null;
			}
			return true;
		}
		
		public override bool ReportMousePosition (int x, int y, int width, int height)
		{
			if (width == 0 || height == 0) return false;

			if (mIsMouseDown)
			{
				mScrSide = new Point(x, y);
				if (Point.Distance(mScrCenter, mScrSide) < Math.Sqrt(2)) 
				{
					mScrSide = new Point(x + 1, y + 1);
				}
				
				// Setting the Parameters
				if (mButtonDown == 1 /* left */)
				{
					ToneStageOperationParameters pms = (ToneStageOperationParameters)Parameters;
					
					pms.AutoDarkCenter = new Point(mScrCenter.X / width, mScrCenter.Y / height);
					pms.AutoDarkRadius = Point.Distance(mScrCenter, mScrSide) * 2 / (width + height);
					pms.AutoDarkTone = true;
					toneselectorwidget1.DarkToneSelectorSymbol = ToneSelectorSymbol.None;
				}
				else if (mButtonDown == 3 /* right */)
				{
					ToneStageOperationParameters pms = (ToneStageOperationParameters)Parameters;
						
					pms.AutoLightCenter = new Point(mScrCenter.X / width, mScrCenter.Y / height);
					pms.AutoLightRadius = Point.Distance(mScrCenter, mScrSide) * 2 / (width + height);
					pms.AutoLightTone = true;
					toneselectorwidget1.LightToneSelectorSymbol = ToneSelectorSymbol.None;
				}
				
				return true;
			}
			return false;
		}

		public override void DrawEditor (IBitmapView view)
		{
			Gdk.Drawable target = ((FloatPixmapViewWidget)view).GdkWindow;
			Gdk.Rectangle image_position = ((FloatPixmapViewWidget)view).CurrentImagePosition;
			ToneStageOperationParameters pms = (ToneStageOperationParameters)Parameters;
			
			
			Gdk.GC gc = new Gdk.GC(target);
			
			gc.Function = Gdk.Function.Copy;
			
			if (pms.AutoDarkTone)
			{
				// Drawing dark selection
				Point scrDarkCenter = new Point(image_position.X + (int)(pms.AutoDarkCenter.X * view.Image.Width),
				                                image_position.Y + (int)(pms.AutoDarkCenter.Y * view.Image.Height));
				double scrDarkRadius = pms.AutoDarkRadius * (view.Image.Width + view.Image.Height) / 2;
				
				using (Cairo.Context cc = Gdk.CairoHelper.Create(target))
				{
					cc.LineCap = Cairo.LineCap.Round;
					cc.LineJoin = Cairo.LineJoin.Round;
	
					cc.Color = new Cairo.Color(1, 1, 1, 0.3);
							
					cc.LineWidth = 3;
					cc.Arc(scrDarkCenter.X, scrDarkCenter.Y, scrDarkRadius, 0, 2 * Math.PI);
					cc.ClosePath();
					cc.Stroke();
					
					cc.Color = new Cairo.Color(0, 0, 0, 1);
	
					cc.LineWidth = 1;
					cc.SetDash(new double[] {3, 3}, 0);
					cc.Arc(scrDarkCenter.X, scrDarkCenter.Y, scrDarkRadius, 0, 2 * Math.PI);
					cc.ClosePath();
					cc.Stroke();
				}
			}
			
			if (pms.AutoLightTone)
			{
				// Drawing light selection
				Point scrLightCenter = new Point(image_position.X + (int)(pms.AutoLightCenter.X * view.Image.Width),
				                                 image_position.Y + (int)(pms.AutoLightCenter.Y * view.Image.Height));
				double scrLightRadius = pms.AutoLightRadius * (view.Image.Width + view.Image.Height) / 2;
				
				using (Cairo.Context cc = Gdk.CairoHelper.Create(target))
				{
					cc.LineCap = Cairo.LineCap.Round;
					cc.LineJoin = Cairo.LineJoin.Round;
	
					cc.Color = new Cairo.Color(0, 0, 0, 0.3);
							
					cc.LineWidth = 3;
					cc.Arc(scrLightCenter.X, scrLightCenter.Y, scrLightRadius, 0, 2 * Math.PI);
					cc.ClosePath();
					cc.Stroke();
					
					cc.Color = new Cairo.Color(1, 1, 1, 1);
	
					cc.LineWidth = 1;
					cc.SetDash(new double[] {3, 3}, 0);
					cc.Arc(scrLightCenter.X, scrLightCenter.Y, scrLightRadius, 0, 2 * Math.PI);
					cc.ClosePath();
					cc.Stroke();
				}
			}
		}
		
		public override void AnalyzeImage (IBitmapCore image)
		{
			int points = 200;	// TODO: Make option
			
			ToneStageOperationParameters pm = (ToneStageOperationParameters)Parameters;
			if (pm.AutoDarkTone)
			{
				mAnalyzedDarkTone = image.FindDarkTone(
					pm.LightTone, 
					pm.Edge, 
					pm.Softness, 
					pm.AutoDarkCenter,
					pm.AutoDarkRadius, 
					points);
				toneselectorwidget1.SelectedDarkTone = mAnalyzedDarkTone;
				toneselectorwidget1.DarkToneSelectorSymbol = ToneSelectorSymbol.Dot;
			}
			if (pm.AutoLightTone)
			{
				mAnalyzedLightTone = image.FindLightTone(
					pm.DarkTone, 
					pm.Edge, 
					pm.Softness, 
					pm.AutoLightCenter,
					pm.AutoLightRadius, 
					points);
				toneselectorwidget1.SelectedLightTone = mAnalyzedLightTone;
				toneselectorwidget1.LightToneSelectorSymbol = ToneSelectorSymbol.Dot;
			}
		}
		
		protected void OnEdgeHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeEdge(Math.Pow(11, edge_hscale.Value) - 1, EdgeChanger.HScale);
		}

		protected void OnSoftnessHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeSoftness(softness_hscale.Value, SoftnessChanger.HScale);
		}

		protected void OnEdgeSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeEdge(edge_spinbutton.Value, EdgeChanger.SpinButton);
		}

		protected void OnSoftnessSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeSoftness(softness_hscale.Value, SoftnessChanger.SpinButton);
		}
	}
}
