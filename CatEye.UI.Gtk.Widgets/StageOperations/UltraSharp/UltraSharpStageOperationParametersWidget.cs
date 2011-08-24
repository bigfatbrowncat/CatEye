using System;
using System.Globalization;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("UltraSharpStageOperation")]
	public partial class UltraSharpStageOperationParametersWidget : StageOperationParametersWidget
	{
		public UltraSharpStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
		}
		
		private bool _PressureIsChanging = false;
		private bool _ContrastIsChanging = false;
		private bool _RadiusIsChanging = false;

		protected enum PressureChanger { HScale, SpinButton }
		protected enum ContrastChanger { HScale, SpinButton }
		protected enum RadiusChanger { HScale, SpinButton }

		protected void ChangePressure(double new_value, PressureChanger changer)
		{
			if (!_PressureIsChanging)
			{
				_PressureIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != PressureChanger.HScale)
				{
					if (((UltraSharpStageOperationParameters)Parameters).Type == UltraSharpStageOperationParameters.SharpType.Sharp)
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
				
				((UltraSharpStageOperationParameters)Parameters).Pressure = new_value;
				EndChangingParameters();
				OnUserModified();
				_PressureIsChanging = false;
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
				
				((UltraSharpStageOperationParameters)Parameters).Contrast = new_value;
				EndChangingParameters();
				OnUserModified();
				_ContrastIsChanging = false;
			}
		}

		protected void ChangeRadius(double new_value, RadiusChanger changer)
		{
			if (!_RadiusIsChanging)
			{
				_RadiusIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != RadiusChanger.HScale)
					radius_hscale.Value = new_value;
				
				if (changer != RadiusChanger.SpinButton)
					radius_spinbutton.Value = new_value;
				
				((UltraSharpStageOperationParameters)Parameters).Radius = new_value;
				EndChangingParameters();
				OnUserModified();
				_RadiusIsChanging = false;
			}
		}
		
		protected override void HandleParametersChangedNotByUI ()
		{
			if (((UltraSharpStageOperationParameters)Parameters).Type == UltraSharpStageOperationParameters.SharpType.Sharp)
				sharp_radiobutton.Active = true;
			else
				soft_radiobutton.Active = true;
			
			_PressureIsChanging = true;
			if (((UltraSharpStageOperationParameters)Parameters).Type == UltraSharpStageOperationParameters.SharpType.Sharp)
			{				
				pressure_hscale.Value = ((UltraSharpStageOperationParameters)Parameters).Pressure;
			}
			else
			{				
				pressure_hscale.Value = ((UltraSharpStageOperationParameters)Parameters).Pressure;
			}
				
			pressure_spinbutton.Value = ((UltraSharpStageOperationParameters)Parameters).Pressure;
			_PressureIsChanging = false;
			
			_ContrastIsChanging = true;
			contrast_hscale.Value = ((UltraSharpStageOperationParameters)Parameters).Contrast;
			contrast_spinbutton.Value = ((UltraSharpStageOperationParameters)Parameters).Contrast;
			_ContrastIsChanging = false;
			
			_RadiusIsChanging = true;
			radius_hscale.Value = ((UltraSharpStageOperationParameters)Parameters).Radius;
			radius_spinbutton.Value = ((UltraSharpStageOperationParameters)Parameters).Radius;
			_RadiusIsChanging = false;
			
		}

		protected void OnPressureHscaleChangeValue (object o, ChangeValueArgs args)
		{
			if (((UltraSharpStageOperationParameters)Parameters).Type == UltraSharpStageOperationParameters.SharpType.Sharp)
				ChangePressure(pressure_hscale.Value, PressureChanger.HScale);
			else
				ChangePressure(pressure_hscale.Value, PressureChanger.HScale);
		}

		protected void OnPressureSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangePressure(pressure_spinbutton.Value, PressureChanger.SpinButton);
		}

		protected void OnRadiusHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeRadius(radius_hscale.Value, RadiusChanger.HScale);
		}

		protected void OnRadiusSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeRadius(radius_spinbutton.Value, RadiusChanger.SpinButton);
		}

		protected void OnSharpSoftToggled (object sender, System.EventArgs e)
		{
			if (sharp_radiobutton.Active)
			{
				((UltraSharpStageOperationParameters)Parameters).Type = UltraSharpStageOperationParameters.SharpType.Sharp;
				pressure_spinbutton.Adjustment.Upper = 100;
				pressure_hscale.Adjustment.Upper = 100;
				ChangePressure(pressure_hscale.Value, PressureChanger.HScale);
			}
			else
			{
				((UltraSharpStageOperationParameters)Parameters).Type = UltraSharpStageOperationParameters.SharpType.Soft;
				pressure_spinbutton.Adjustment.Upper = 20;
				pressure_hscale.Adjustment.Upper = 20;
				ChangePressure(pressure_hscale.Value, PressureChanger.HScale);
			}
		}

		protected void OnContrastHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeContrast(contrast_hscale.Value, ContrastChanger.HScale);
		}

		protected void OnContrastSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeContrast(contrast_spinbutton.Value, ContrastChanger.SpinButton);
		}
	}
}
