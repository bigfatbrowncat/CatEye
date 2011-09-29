using System;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	[StageOperationID("PreprocessStageOperation")]
	public partial class PreprocessStageOperationParametersWidget : StageOperationParametersWidget
	{
		public PreprocessStageOperationParametersWidget (StageOperationParameters parameters) 
			: base (parameters)
		{
			this.Build ();
		}

		private bool _HighlightsCutIsChanging = false;
		private bool _SoftnessIsChanging = false;

		protected enum HighlightsCutChanger { HScale, SpinButton }
		protected enum SoftnessChanger { HScale, SpinButton }
		
		protected void ChangeHighlightsCut(double new_value, HighlightsCutChanger changer)
		{
			if (!_HighlightsCutIsChanging)
			{
				_HighlightsCutIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != HighlightsCutChanger.HScale)
					cut_hscale.Value = new_value;
				
				if (changer != HighlightsCutChanger.SpinButton)
					cut_spinbutton.Value = new_value;
				
				((PreprocessStageOperationParameters)Parameters).HighlightsCut = new_value;
				EndChangingParameters();
				OnUserModified();
				_HighlightsCutIsChanging = false;
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
				
				((PreprocessStageOperationParameters)Parameters).Softness = new_value;
				EndChangingParameters();
				OnUserModified();
				_SoftnessIsChanging = false;
			}
		}
		
		protected override void HandleParametersChangedNotByUI ()
		{			
			_HighlightsCutIsChanging = true;
			cut_hscale.Value = ((PreprocessStageOperationParameters)Parameters).HighlightsCut;
			cut_spinbutton.Value = ((PreprocessStageOperationParameters)Parameters).HighlightsCut;
			_HighlightsCutIsChanging = false;
			
			_SoftnessIsChanging = true;
			softness_hscale.Value = ((PreprocessStageOperationParameters)Parameters).Softness;
			softness_spinbutton.Value = ((PreprocessStageOperationParameters)Parameters).Softness;
			_SoftnessIsChanging = false;
			
		}

		protected void OnCutHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeHighlightsCut(cut_hscale.Value, HighlightsCutChanger.HScale);
		}

		protected void OnCutSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeHighlightsCut(cut_spinbutton.Value, HighlightsCutChanger.SpinButton);
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

