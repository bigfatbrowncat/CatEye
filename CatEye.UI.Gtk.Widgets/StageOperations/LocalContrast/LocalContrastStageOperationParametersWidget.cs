using System;
using System.Globalization;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	[StageOperationID("LocalContrastStageOperation")]
	public partial class LocalContrastStageOperationParametersWidget : StageOperationParametersWidget
	{
		public LocalContrastStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
		}
		
		private bool _PressureIsChanging = false;
		private bool _AnticrownIsChanging = false;
		private bool _ContrastIsChanging = false;

		protected enum PressureChanger { HScale, SpinButton }
		protected enum AnticrownChanger { HScale, SpinButton }
		protected enum ContrastChanger { HScale, SpinButton }

		protected void ChangePressure(double new_value, PressureChanger changer)
		{
			if (!_PressureIsChanging)
			{
				_PressureIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != PressureChanger.HScale)
				{
					if (((LocalContrastStageOperationParameters)Parameters).Type == LocalContrastStageOperationParameters.SharpType.Sharp)
					{
						pressure_hscale.Value = new_value;
					}
					else
					{
						pressure_hscale.Value = new_value;
					}
				}
				
				if (changer != PressureChanger.SpinButton)
					pressure_spinbutton.Value = new_value;
				
				((LocalContrastStageOperationParameters)Parameters).Pressure = new_value;
				EndChangingParameters();
				OnUserModified();
				_PressureIsChanging = false;
			}
		}

		protected void ChangeAnticrown(double new_value, AnticrownChanger changer)
		{
			if (!_AnticrownIsChanging)
			{
				_AnticrownIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != AnticrownChanger.HScale)
					anticrown_hscale.Value = new_value;
				
				if (changer != AnticrownChanger.SpinButton)
					anticrown_spinbutton.Value = new_value;
				
				((LocalContrastStageOperationParameters)Parameters).Anticrown = new_value;
				EndChangingParameters();
				OnUserModified();
				_AnticrownIsChanging = false;
			}
		}

		protected void ChangeContrast(double new_value, ContrastChanger changer)
		{
			if (!_ContrastIsChanging)
			{
				_ContrastIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != ContrastChanger.HScale)
					contrast_hscale.Value = new_value;
				
				if (changer != ContrastChanger.SpinButton)
					contrast_spinbutton.Value = new_value;
				
				((LocalContrastStageOperationParameters)Parameters).Contrast = new_value;
				EndChangingParameters();
				OnUserModified();
				_ContrastIsChanging = false;
			}
		}
		
		protected override void HandleParametersChangedNotByUI ()
		{
			if (((LocalContrastStageOperationParameters)Parameters).Type == LocalContrastStageOperationParameters.SharpType.Sharp)
				sharp_radiobutton.Active = true;
			else
				soft_radiobutton.Active = true;
			
			_PressureIsChanging = true;
			if (((LocalContrastStageOperationParameters)Parameters).Type == LocalContrastStageOperationParameters.SharpType.Sharp)
			{				
				pressure_hscale.Value = ((LocalContrastStageOperationParameters)Parameters).Pressure;
			}
			else
			{				
				pressure_hscale.Value = ((LocalContrastStageOperationParameters)Parameters).Pressure;
			}
				
			pressure_spinbutton.Value = ((LocalContrastStageOperationParameters)Parameters).Pressure;
			_PressureIsChanging = false;
			
			_AnticrownIsChanging = true;
			anticrown_hscale.Value = ((LocalContrastStageOperationParameters)Parameters).Anticrown;
			anticrown_spinbutton.Value = ((LocalContrastStageOperationParameters)Parameters).Anticrown;
			_AnticrownIsChanging = false;
			
			_ContrastIsChanging = true;
			contrast_hscale.Value = ((LocalContrastStageOperationParameters)Parameters).Contrast;
			contrast_spinbutton.Value = ((LocalContrastStageOperationParameters)Parameters).Contrast;
			_ContrastIsChanging = false;
			
		}

		protected void OnPressureHscaleChangeValue (object o, ChangeValueArgs args)
		{
			if (((LocalContrastStageOperationParameters)Parameters).Type == LocalContrastStageOperationParameters.SharpType.Sharp)
				ChangePressure(pressure_hscale.Value, PressureChanger.HScale);
			else
				ChangePressure(pressure_hscale.Value, PressureChanger.HScale);
		}

		protected void OnPressureSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangePressure(pressure_spinbutton.Value, PressureChanger.SpinButton);
		}

		protected void OnContrastHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeContrast(contrast_hscale.Value, ContrastChanger.HScale);
		}

		protected void OnContrastSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeContrast(contrast_spinbutton.Value, ContrastChanger.SpinButton);
		}

		protected void OnSharpSoftToggled (object sender, System.EventArgs e)
		{
			if (sharp_radiobutton.Active)
			{
				((LocalContrastStageOperationParameters)Parameters).Type = LocalContrastStageOperationParameters.SharpType.Sharp;
				pressure_spinbutton.Adjustment.Upper = 100;
				pressure_hscale.Adjustment.Upper = 100;
				ChangePressure(pressure_hscale.Value, PressureChanger.HScale);
			}
			else
			{
				((LocalContrastStageOperationParameters)Parameters).Type = LocalContrastStageOperationParameters.SharpType.Soft;
				pressure_spinbutton.Adjustment.Upper = 20;
				pressure_hscale.Adjustment.Upper = 20;
				ChangePressure(pressure_hscale.Value, PressureChanger.HScale);
			}
		}

		protected void OnAnticrownHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeAnticrown(anticrown_hscale.Value, AnticrownChanger.HScale);
		}

		protected void OnAnticrownSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeAnticrown(anticrown_spinbutton.Value, AnticrownChanger.SpinButton);
		}
	}
}
