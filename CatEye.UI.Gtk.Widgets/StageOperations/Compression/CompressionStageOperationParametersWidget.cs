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

		private bool _CurveIsChanging = false;
		
		protected enum CurveChanger { HScale, SpinButton }
		
		protected void ChangeCurve(double new_value, CurveChanger changer)
		{
			if (!_CurveIsChanging)
			{
				_CurveIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != CurveChanger.HScale)
					curve_hscale.Value = new_value;
				
				if (changer != CurveChanger.SpinButton)
					curve_spinbutton.Value = new_value;
				
				((CompressionStageOperationParameters)Parameters).Curve = new_value;
				EndChangingParameters();
				OnUserModified();
				_CurveIsChanging = false;
			}
		}

		protected override void HandleParametersChangedNotByUI ()
		{
			_CurveIsChanging = true;
			curve_hscale.Value = ((CompressionStageOperationParameters)Parameters).Curve;
			curve_spinbutton.Value = ((CompressionStageOperationParameters)Parameters).Curve;
			_CurveIsChanging = false;
		}
		
		protected void OnPowerHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeCurve(curve_hscale.Value, CurveChanger.HScale);
		}

		protected void OnPowerSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeCurve(curve_spinbutton.Value, CurveChanger.SpinButton);
		}
	}
}
