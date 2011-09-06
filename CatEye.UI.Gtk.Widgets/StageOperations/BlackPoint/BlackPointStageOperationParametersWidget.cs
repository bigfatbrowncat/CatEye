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
					cut_hscale.Value = new_value;
				
				if (changer != CutChanger.SpinButton)
					cut_spinbutton.Value = new_value;
				
				((BlackPointStageOperationParameters)Parameters).Cut = new_value;
				
				EndChangingParameters();
				OnUserModified();
				_CutIsChanging = false;
			}
		}


		protected override void HandleParametersChangedNotByUI ()
		{
			_CutIsChanging = true;
			cut_hscale.Value = ((BlackPointStageOperationParameters)Parameters).Cut;
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
