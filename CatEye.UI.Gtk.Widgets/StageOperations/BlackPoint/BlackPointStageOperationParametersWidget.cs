using System;
using System.Globalization;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("BlackPointStageOperation")]
	public partial class BlackPointStageOperationParametersWidget : StageOperationParametersWidget
	{
		public BlackPointStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
		}

		private bool _BlackIsChanging = false;
		
		protected enum BlackChanger { HScale, SpinButton }
		
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
				
				((BlackPointStageOperationParameters)Parameters).Black = new_value;
				
				EndChangingParameters();
				OnUserModified();
				_BlackIsChanging = false;
			}
		}


		protected override void HandleParametersChangedNotByUI ()
		{
			_BlackIsChanging = true;
			black_hscale.Value = ((BlackPointStageOperationParameters)Parameters).Black;
			black_spinbutton.Value = ((BlackPointStageOperationParameters)Parameters).Black;
			_BlackIsChanging = false;
			
		}
		
		protected void OnBlackHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeBlack(black_hscale.Value, BlackChanger.HScale);
		}

		protected void OnBlackSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeBlack(black_spinbutton.Value, BlackChanger.SpinButton);
		}

	}
}
