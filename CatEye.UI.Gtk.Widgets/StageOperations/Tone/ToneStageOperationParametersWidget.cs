using System;
using System.Globalization;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("ToneStageOperation")]
	public partial class ToneStageOperationParametersWidget : StageOperationParametersWidget
	{
		public ToneStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
			toneselectorwidget1.ToneSelected += OnToneselectorwidget1ToneSelected;
			toneselectorwidget1.SelectedToneChanged += OnToneselectorwidget1SelectedToneChanged;
			toneselectorwidget1.Alpha = 0.5;
		}
		
		private bool _HlInvIsChanging = false;
		
		protected enum HlInvChanger { HScale, SpinButton }
		
		protected void ChangeHlInv(double new_value, HlInvChanger changer)
		{
			if (!_HlInvIsChanging)
			{
				_HlInvIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != HlInvChanger.HScale)
					hl_inv_hscale.Value = new_value;
				
				if (changer != HlInvChanger.SpinButton)
					hl_inv_spinbutton.Value = new_value;
				
				((ToneStageOperationParameters)Parameters).HighlightsInvariance = new_value;
				
				EndChangingParameters();
				OnUserModified();
				_HlInvIsChanging = false;
			}
		}
		
		
		protected override void HandleParametersChangedNotByUI ()
		{
			Tone tn = ((ToneStageOperationParameters)Parameters).Tone;
			toneselectorwidget1.SelectedTone = tn;

			_HlInvIsChanging = true;
			hl_inv_hscale.Value = ((ToneStageOperationParameters)Parameters).HighlightsInvariance;
			hl_inv_spinbutton.Value = ((ToneStageOperationParameters)Parameters).HighlightsInvariance;
			_HlInvIsChanging = false;
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
			ChangeHlInv(hl_inv_hscale.Value, HlInvChanger.HScale);
		}

		protected void OnHlInvSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeHlInv(hl_inv_spinbutton.Value, HlInvChanger.SpinButton);
		}
	}
}
