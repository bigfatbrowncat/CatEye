using System;
using System.Globalization;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("CompressionStageOperation")]
	public partial class CompressionStageOperationParametersWidget : StageOperationParametersWidget
	{
		public CompressionStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
		}

		private bool _PowerIsChanging = false;
		private bool _DarkPreservingIsChanging = false;
		
		protected enum PowerChanger { HScale, SpinButton }
		protected enum DarkPreservingChanger { HScale, SpinButton }
		
		protected void ChangePower(double new_value, PowerChanger changer)
		{
			if (!_PowerIsChanging)
			{
				_PowerIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != PowerChanger.HScale)
					power_hscale.Value = new_value;
				
				if (changer != PowerChanger.SpinButton)
					power_spinbutton.Value = new_value;
				
				((CompressionStageOperationParameters)Parameters).Power = new_value;
				EndChangingParameters();
				OnUserModified();
				_PowerIsChanging = false;
			}
		}
		protected void ChangeDarkPreserving(double new_value, DarkPreservingChanger changer)
		{
			if (!_DarkPreservingIsChanging)
			{
				_DarkPreservingIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != DarkPreservingChanger.HScale)
					dark_preserving_hscale.Value = new_value;
				
				if (changer != DarkPreservingChanger.SpinButton)
					dark_preserving_spinbutton.Value = new_value;
				
				((CompressionStageOperationParameters)Parameters).DarkPreserving = new_value;
				EndChangingParameters();
				OnUserModified();
				_DarkPreservingIsChanging = false;
			}
		}

		protected override void HandleParametersChangedNotByUI ()
		{
			_PowerIsChanging = true;
			power_hscale.Value = ((CompressionStageOperationParameters)Parameters).Power;
			power_spinbutton.Value = ((CompressionStageOperationParameters)Parameters).Power;
			_PowerIsChanging = false;
			
			_DarkPreservingIsChanging = true;
			dark_preserving_hscale.Value = ((CompressionStageOperationParameters)Parameters).DarkPreserving;
			dark_preserving_spinbutton.Value = ((CompressionStageOperationParameters)Parameters).DarkPreserving;
			_DarkPreservingIsChanging = false;
			
//			power_entry.Text = ((CompressionStageOperationParameters)Parameters).Power.ToString();
//			bloha_entry.Text = ((CompressionStageOperationParameters)Parameters).DarkPreserving.ToString();
			
		}
		
		protected void OnPowerHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangePower(power_hscale.Value, PowerChanger.HScale);
		}

		protected void OnPowerSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangePower(power_spinbutton.Value, PowerChanger.SpinButton);
		}

		protected void OnDarkPreservingHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeDarkPreserving(dark_preserving_hscale.Value, DarkPreservingChanger.HScale);
		}
		
		protected void OnDarkPreservingSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeDarkPreserving(dark_preserving_spinbutton.Value, DarkPreservingChanger.SpinButton);
		}
	}
}
