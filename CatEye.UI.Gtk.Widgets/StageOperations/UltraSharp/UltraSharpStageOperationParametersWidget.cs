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
		
		private bool _PowerIsChanging = false;
		private bool _RadiusIsChanging = false;

		protected enum PowerChanger { HScale, SpinButton }
		protected enum RadiusChanger { HScale, SpinButton }

		protected void ChangePower(double new_value, PowerChanger changer)
		{
			if (!_PowerIsChanging)
			{
				_PowerIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != PowerChanger.HScale)
				{
					if (((UltraSharpStageOperationParameters)Parameters).Type == UltraSharpStageOperationParameters.SharpType.Sharp)
					{
						power_hscale.Value = new_value;
					}
					else
					{
						power_hscale.Value = new_value;
					}
				}
				
				if (changer != PowerChanger.SpinButton)
					power_spinbutton.Value = new_value;
				
				((UltraSharpStageOperationParameters)Parameters).Power = new_value;
				EndChangingParameters();
				OnUserModified();
				_PowerIsChanging = false;
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
			
			_PowerIsChanging = true;
			if (((UltraSharpStageOperationParameters)Parameters).Type == UltraSharpStageOperationParameters.SharpType.Sharp)
			{				
				power_hscale.Value = ((UltraSharpStageOperationParameters)Parameters).Power;
			}
			else
			{				
				power_hscale.Value = ((UltraSharpStageOperationParameters)Parameters).Power;
			}
				
			power_spinbutton.Value = ((UltraSharpStageOperationParameters)Parameters).Power;
			_PowerIsChanging = false;

			_RadiusIsChanging = true;
			radius_hscale.Value = ((UltraSharpStageOperationParameters)Parameters).Radius;
			radius_spinbutton.Value = ((UltraSharpStageOperationParameters)Parameters).Radius;
			_RadiusIsChanging = false;
			
		}

		protected void OnPowerHscaleChangeValue (object o, ChangeValueArgs args)
		{
			if (((UltraSharpStageOperationParameters)Parameters).Type == UltraSharpStageOperationParameters.SharpType.Sharp)
				ChangePower(power_hscale.Value, PowerChanger.HScale);
			else
				ChangePower(power_hscale.Value, PowerChanger.HScale);
		}

		protected void OnPowerSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangePower(power_spinbutton.Value, PowerChanger.SpinButton);
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
				power_spinbutton.Adjustment.Upper = 100;
				power_hscale.Adjustment.Upper = 100;
				ChangePower(power_hscale.Value, PowerChanger.HScale);
			}
			else
			{
				((UltraSharpStageOperationParameters)Parameters).Type = UltraSharpStageOperationParameters.SharpType.Soft;
				power_spinbutton.Adjustment.Upper = 20;
				power_hscale.Adjustment.Upper = 20;
				ChangePower(power_hscale.Value, PowerChanger.HScale);
			}
		}
	}
}
