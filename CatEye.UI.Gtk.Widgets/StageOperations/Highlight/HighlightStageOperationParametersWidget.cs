using System;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	[StageOperationID("HighlightStageOperation")]
	public partial class HighlightStageOperationParametersWidget : StageOperationParametersWidget
	{
		public HighlightStageOperationParametersWidget (StageOperationParameters parameters) 
			: base (parameters)
		{
			this.Build ();
		}

		private bool _CutIsChanging = false;
		private bool _SoftnessIsChanging = false;

		protected enum CutChanger { HScale, SpinButton }
		protected enum SoftnessChanger { HScale, SpinButton }
		
		protected void ChangeCut(double new_value, CutChanger changer)
		{
			if (!_CutIsChanging)
			{
				_CutIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != CutChanger.HScale)
					cut_hscale.Value = new_value;
				
				if (changer != CutChanger.SpinButton)
					cut_spinbutton.Value = new_value;
				
				((HighlightStageOperationParameters)Parameters).Cut = new_value;
				EndChangingParameters();
				OnUserModified();
				_CutIsChanging = false;
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
				
				((HighlightStageOperationParameters)Parameters).Softness = new_value;
				EndChangingParameters();
				OnUserModified();
				_SoftnessIsChanging = false;
			}
		}
		
		protected override void HandleParametersChangedNotByUI ()
		{			
			_CutIsChanging = true;
			cut_hscale.Value = ((HighlightStageOperationParameters)Parameters).Cut;
			cut_spinbutton.Value = ((HighlightStageOperationParameters)Parameters).Cut;
			_CutIsChanging = false;
			
			_SoftnessIsChanging = true;
			softness_hscale.Value = ((HighlightStageOperationParameters)Parameters).Softness;
			softness_spinbutton.Value = ((HighlightStageOperationParameters)Parameters).Softness;
			_SoftnessIsChanging = false;
			
		}

		protected void OnCutHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeCut(cut_hscale.Value, CutChanger.HScale);
		}

		protected void OnCutSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeCut(cut_spinbutton.Value, CutChanger.SpinButton);
		}

		protected void OnSoftnessHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeSoftness(softness_hscale.Value, SoftnessChanger.HScale);
		}

		protected void OnSoftnessSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeSoftness(softness_spinbutton.Value, SoftnessChanger.SpinButton);
		}
	}
}

