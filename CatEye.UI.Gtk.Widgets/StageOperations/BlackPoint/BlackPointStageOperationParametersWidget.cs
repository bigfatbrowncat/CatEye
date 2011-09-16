using System;
using System.Globalization;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	[StageOperationID("BlackPointStageOperation")]
	public partial class BlackPointStageOperationParametersWidget : StageOperationParametersWidget
	{
		public BlackPointStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
		}

		private bool _CutIsChanging = false;
		
		protected enum CutChanger { HScale, SpinButton }
		
		protected void ChangeCut(double new_value, CutChanger changer)
		{
			if (!_CutIsChanging)
			{
				_CutIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != CutChanger.HScale)			
					cut_hscale.Value = 2 * Math.Log(2 * new_value + 3, 5) - 1;
				
				if (changer != CutChanger.SpinButton)
					cut_spinbutton.Value = (Math.Pow(5, (new_value + 1) / 2.0) - 3) / 2.0;
				
				((BlackPointStageOperationParameters)Parameters).Cut = new_value;
				
				EndChangingParameters();
				OnUserModified();
				_CutIsChanging = false;
			}
		}


		protected override void HandleParametersChangedNotByUI ()
		{
			_CutIsChanging = true;
			cut_hscale.Value = 2 * Math.Log(2 * ((BlackPointStageOperationParameters)Parameters).Cut + 3, 5) - 1;
			cut_spinbutton.Value = ((BlackPointStageOperationParameters)Parameters).Cut;
			_CutIsChanging = false;
			
		}
		
		protected void OnCutHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeCut(cut_hscale.Value, CutChanger.HScale);
		}

		protected void OnCutSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeCut(cut_spinbutton.Value, CutChanger.SpinButton);
		}

	}
}
