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
		private bool _BaseIsChanging = false;
		
		protected enum PowerChanger { HScale, SpinButton }
		protected enum RadiusChanger { HScale, SpinButton }
		protected enum BaseChanger { HScale, SpinButton }
		
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
		
		protected void ChangeBase(double new_value, BaseChanger changer)
		{
			if (!_BaseIsChanging)
			{
				_BaseIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != BaseChanger.HScale)
					base_hscale.Value = new_value;
				
				if (changer != BaseChanger.SpinButton)
					base_spinbutton.Value = new_value;
				
				((UltraSharpStageOperationParameters)Parameters).Base = new_value;
				EndChangingParameters();
				OnUserModified();
				_BaseIsChanging = false;
			}
		}		
		
		protected override void HandleParametersChangedNotByUI ()
		{
			_PowerIsChanging = true;
			power_hscale.Value = ((UltraSharpStageOperationParameters)Parameters).Power;
			power_spinbutton.Value = ((UltraSharpStageOperationParameters)Parameters).Power;
			_PowerIsChanging = false;

			_RadiusIsChanging = true;
			radius_hscale.Value = ((UltraSharpStageOperationParameters)Parameters).Radius;
			radius_spinbutton.Value = ((UltraSharpStageOperationParameters)Parameters).Radius;
			_RadiusIsChanging = false;
			
			_BaseIsChanging = true;
			base_hscale.Value = ((UltraSharpStageOperationParameters)Parameters).Base;
			base_spinbutton.Value = ((UltraSharpStageOperationParameters)Parameters).Base;
			_BaseIsChanging = false;
		}

		protected void OnPowerHscaleChangeValue (object o, Gtk.ChangeValueArgs args)
		{
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

		protected void OnBaseHscaleChangeValue (object o, Gtk.ChangeValueArgs args)
		{
			ChangeBase(base_hscale.Value, BaseChanger.HScale);
		}

		protected void OnBaseSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeBase(base_spinbutton.Value, BaseChanger.SpinButton);
		}
	}
}
