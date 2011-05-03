using System;
using System.Globalization;
using CatEye.Core;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("HardCutStageOperation")]
	public partial class HardCutStageOperationParametersWidget : StageOperationParametersWidget
	{
		public HardCutStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
		}

		private bool _BlackIsChanging = false;
		private bool _WhiteIsChanging = false;
		
		protected enum BlackChanger { HScale, SpinButton }
		protected enum WhiteChanger { HScale, SpinButton }
		
		protected void ChangeBlack(double new_value, BlackChanger changer)
		{
			if (!_BlackIsChanging)
			{
				_BlackIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != BlackChanger.HScale)
					black_hscale.Value = new_value;
				
				if (changer != BlackChanger.SpinButton)
					black_spinbutton.Value = new_value;
				
				((HardCutStageOperationParameters)Parameters).Black = new_value;
				
				// Black shouldn't be bigger than white
				if (((HardCutStageOperationParameters)Parameters).Black > 
					((HardCutStageOperationParameters)Parameters).White - 0.01)
				{
					((HardCutStageOperationParameters)Parameters).White = 
						((HardCutStageOperationParameters)Parameters).Black + 0.01;
					white_hscale.Value = ((HardCutStageOperationParameters)Parameters).White;
					white_spinbutton.Value = ((HardCutStageOperationParameters)Parameters).White;
				}
				
				EndChangingParameters();
				OnUserModified();
				_BlackIsChanging = false;
			}
		}
		protected void ChangeWhite(double new_value, WhiteChanger changer)
		{
			if (!_WhiteIsChanging)
			{
				_WhiteIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != WhiteChanger.HScale)
					white_hscale.Value = new_value;
				
				if (changer != WhiteChanger.SpinButton)
					white_spinbutton.Value = new_value;
				
				((HardCutStageOperationParameters)Parameters).White = new_value;

				// Black shouldn't be bigger than white
				if (((HardCutStageOperationParameters)Parameters).Black > 
					((HardCutStageOperationParameters)Parameters).White - 0.01)
				{
					((HardCutStageOperationParameters)Parameters).Black = 
						((HardCutStageOperationParameters)Parameters).White - 0.01;
					black_hscale.Value = ((HardCutStageOperationParameters)Parameters).Black;
					black_spinbutton.Value = ((HardCutStageOperationParameters)Parameters).Black;
				}

				EndChangingParameters();
				OnUserModified();
				_WhiteIsChanging = false;
			}
		}

		protected override void HandleParametersChangedNotByUI ()
		{
			_BlackIsChanging = true;
			black_hscale.Value = ((HardCutStageOperationParameters)Parameters).Black;
			black_spinbutton.Value = ((HardCutStageOperationParameters)Parameters).Black;
			_BlackIsChanging = false;
			
			_WhiteIsChanging = true;
			white_hscale.Value = ((HardCutStageOperationParameters)Parameters).White;
			white_spinbutton.Value = ((HardCutStageOperationParameters)Parameters).White;
			_WhiteIsChanging = false;
			
		}
		
		protected void OnBlackHscaleChangeValue (object o, Gtk.ChangeValueArgs args)
		{
			ChangeBlack(black_hscale.Value, BlackChanger.HScale);
		}

		protected void OnBlackSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeBlack(black_spinbutton.Value, BlackChanger.SpinButton);
		}

		protected void OnWhiteHscaleChangeValue (object o, Gtk.ChangeValueArgs args)
		{
			ChangeWhite(white_hscale.Value, WhiteChanger.HScale);
		}

		protected void OnWhiteSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeWhite(white_spinbutton.Value, WhiteChanger.SpinButton);
		}
	}
}
