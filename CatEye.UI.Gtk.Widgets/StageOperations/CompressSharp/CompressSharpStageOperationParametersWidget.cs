using System;
using System.Globalization;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	[StageOperationID("CompressSharpStageOperation")]
	public partial class CompressSharpStageOperationParametersWidget : StageOperationParametersWidget
	{
		public CompressSharpStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
		}
		
		private bool _CompressionIsChanging = false;
		private bool _CurveIsChanging = false;
		private bool _PressureIsChanging = false;
		private bool _ContrastIsChanging = false;

		protected enum CompressionChanger { HScale, SpinButton }
		protected enum PressureChanger { HScale, SpinButton }
		protected enum CurveChanger { HScale, SpinButton }
		protected enum ContrastChanger { HScale, SpinButton }
		
/*		protected void ChangeCompression(double new_value, CompressionChanger changer)
		{
			if (!_CompressionIsChanging)
			{
				_CompressionIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != CompressionChanger.HScale)
					compression_hscale.Value = new_value;
				
				if (changer != CompressionChanger.SpinButton)
					compression_spinbutton.Value = new_value;
				
				((LocalContrastStageOperationParameters)Parameters).Compression = new_value;
				EndChangingParameters();
				OnUserModified();
				_CompressionIsChanging = false;
			}
		}*/		
		protected void ChangePressure(double new_value, PressureChanger changer)
		{
			if (!_PressureIsChanging)
			{
				_PressureIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != PressureChanger.HScale)
				{
					if (((CompressSharpStageOperationParameters)Parameters).Type == CompressSharpStageOperationParameters.SharpType.Sharp)
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
				
				((CompressSharpStageOperationParameters)Parameters).Pressure = new_value;
				EndChangingParameters();
				OnUserModified();
				_PressureIsChanging = false;
			}
		}

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
				
				((CompressSharpStageOperationParameters)Parameters).Curve = new_value;
				EndChangingParameters();
				OnUserModified();
				_CurveIsChanging = false;
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
				
				((CompressSharpStageOperationParameters)Parameters).Contrast = new_value;
				EndChangingParameters();
				OnUserModified();
				_ContrastIsChanging = false;
			}
		}
		
		protected override void HandleParametersChangedNotByUI ()
		{
			if (((CompressSharpStageOperationParameters)Parameters).Type == CompressSharpStageOperationParameters.SharpType.Sharp)
				sharp_radiobutton.Active = true;
			else
				soft_radiobutton.Active = true;
			
			_PressureIsChanging = true;
			if (((CompressSharpStageOperationParameters)Parameters).Type == CompressSharpStageOperationParameters.SharpType.Sharp)
			{				
				pressure_hscale.Value = ((CompressSharpStageOperationParameters)Parameters).Pressure;
			}
			else
			{				
				pressure_hscale.Value = ((CompressSharpStageOperationParameters)Parameters).Pressure;
			}
				
			pressure_spinbutton.Value = ((CompressSharpStageOperationParameters)Parameters).Pressure;
			_PressureIsChanging = false;

/*			_CompressionIsChanging = true;
			compression_hscale.Value = ((LocalContrastStageOperationParameters)Parameters).Compression;
			compression_spinbutton.Value = ((LocalContrastStageOperationParameters)Parameters).Compression;
			_CompressionIsChanging = false;*/
			
			_CurveIsChanging = true;
			curve_hscale.Value = ((CompressSharpStageOperationParameters)Parameters).Curve;
			curve_spinbutton.Value = ((CompressSharpStageOperationParameters)Parameters).Curve;
			_CurveIsChanging = false;
			
			_ContrastIsChanging = true;
			contrast_hscale.Value = ((CompressSharpStageOperationParameters)Parameters).Contrast;
			contrast_spinbutton.Value = ((CompressSharpStageOperationParameters)Parameters).Contrast;
			_ContrastIsChanging = false;
			
		}

		protected void OnPressureHscaleChangeValue (object o, ChangeValueArgs args)
		{
			if (((CompressSharpStageOperationParameters)Parameters).Type == CompressSharpStageOperationParameters.SharpType.Sharp)
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
				((CompressSharpStageOperationParameters)Parameters).Type = CompressSharpStageOperationParameters.SharpType.Sharp;
				pressure_spinbutton.Adjustment.Upper = 100;
				pressure_hscale.Adjustment.Upper = 100;
				ChangePressure(pressure_hscale.Value, PressureChanger.HScale);
			}
			else
			{
				((CompressSharpStageOperationParameters)Parameters).Type = CompressSharpStageOperationParameters.SharpType.Soft;
				pressure_spinbutton.Adjustment.Upper = 20;
				pressure_hscale.Adjustment.Upper = 20;
				ChangePressure(pressure_hscale.Value, PressureChanger.HScale);
			}
		}

		protected void OnCurveHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeCurve(curve_hscale.Value, CurveChanger.HScale);
		}

		protected void OnCurveSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeCurve(curve_spinbutton.Value, CurveChanger.SpinButton);
		}

		protected void OnCompressionSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
//			ChangeCompression(compression_spinbutton.Value, CompressionChanger.SpinButton);
		}

		protected void OnCompressionHscaleChangeValue (object o, ChangeValueArgs args)
		{
//			ChangeCompression(compression_hscale.Value, CompressionChanger.HScale);
		}
	}
}
