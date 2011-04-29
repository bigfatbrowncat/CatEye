using System;
using System.Globalization;
using CatEye.Core;

namespace CatEye
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
		private bool _LimitUpIsChanging = false;
		private bool _LimitDownIsChanging = false;
		
		protected enum PowerChanger { HScale, SpinButton }
		protected enum RadiusChanger { HScale, SpinButton }
		protected enum LimitUpChanger { HScale, SpinButton }
		protected enum LimitDownChanger { HScale, SpinButton }
		
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
						power_hscale.Value = Math.Sqrt(new_value);
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
		
		protected void ChangeLimitUp(double new_value, LimitUpChanger changer)
		{
			if (!_LimitUpIsChanging)
			{
				_LimitUpIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != LimitUpChanger.HScale)
					limitup_hscale.Value = new_value;
				
				if (changer != LimitUpChanger.SpinButton)
					limitup_spinbutton.Value = new_value;
				
				((UltraSharpStageOperationParameters)Parameters).LimitUp = new_value;
				EndChangingParameters();
				OnUserModified();
				_LimitUpIsChanging = false;
			}
		}		

		protected void ChangeLimitDown(double new_value, LimitDownChanger changer)
		{
			if (!_LimitDownIsChanging)
			{
				_LimitDownIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != LimitDownChanger.HScale)
					limitdown_hscale.Value = new_value;
				
				if (changer != LimitDownChanger.SpinButton)
					limitdown_spinbutton.Value = new_value;
				
				((UltraSharpStageOperationParameters)Parameters).LimitDown = new_value;
				EndChangingParameters();
				OnUserModified();
				_LimitDownIsChanging = false;
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
				power_hscale.Value = Math.Sqrt(((UltraSharpStageOperationParameters)Parameters).Power);
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
			
			_LimitUpIsChanging = true;
			limitup_hscale.Value = ((UltraSharpStageOperationParameters)Parameters).LimitUp;
			limitup_spinbutton.Value = ((UltraSharpStageOperationParameters)Parameters).LimitUp;
			_LimitUpIsChanging = false;

			_LimitDownIsChanging = true;
			limitdown_hscale.Value = ((UltraSharpStageOperationParameters)Parameters).LimitDown;
			limitdown_spinbutton.Value = ((UltraSharpStageOperationParameters)Parameters).LimitDown;
			_LimitDownIsChanging = false;
		}

		protected void OnPowerHscaleChangeValue (object o, Gtk.ChangeValueArgs args)
		{
			if (((UltraSharpStageOperationParameters)Parameters).Type == UltraSharpStageOperationParameters.SharpType.Sharp)
				ChangePower(power_hscale.Value * power_hscale.Value, PowerChanger.HScale);
			else
				ChangePower(power_hscale.Value, PowerChanger.HScale);
		}

		protected void OnPowerSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangePower(power_spinbutton.Value, PowerChanger.SpinButton);
		}

		protected void OnRadiusHscaleChangeValue (object o, Gtk.ChangeValueArgs args)
		{
			ChangeRadius(radius_hscale.Value, RadiusChanger.HScale);
		}

		protected void OnRadiusSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeRadius(radius_spinbutton.Value, RadiusChanger.SpinButton);
		}

		protected void OnLimitupHscaleChangeValue (object o, Gtk.ChangeValueArgs args)
		{
			ChangeLimitUp(limitup_hscale.Value, LimitUpChanger.HScale);
		}

		protected void OnLimitupSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeLimitUp(limitup_spinbutton.Value, LimitUpChanger.SpinButton);
		}

		protected void OnLimitdownHscaleChangeValue (object o, Gtk.ChangeValueArgs args)
		{
			ChangeLimitDown(limitdown_hscale.Value, LimitDownChanger.HScale);
		}

		protected void OnLimitdownSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeLimitDown(limitdown_spinbutton.Value, LimitDownChanger.SpinButton);
		}

		protected void OnSharpSoftToggled (object sender, System.EventArgs e)
		{
			if (sharp_radiobutton.Active)
			{
				((UltraSharpStageOperationParameters)Parameters).Type = UltraSharpStageOperationParameters.SharpType.Sharp;
				power_spinbutton.Adjustment.Upper = 100;
				ChangePower(power_hscale.Value * power_hscale.Value, PowerChanger.HScale);
			}
			else
			{
				((UltraSharpStageOperationParameters)Parameters).Type = UltraSharpStageOperationParameters.SharpType.Soft;
				power_spinbutton.Adjustment.Upper = 10;
				ChangePower(power_hscale.Value, PowerChanger.HScale);
			}
		}
	}
}
