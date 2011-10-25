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
//		private bool _BlurDarkIsChanging = false;
		
		protected enum CutChanger { HScale, SpinButton }
//		protected enum BlurDarkChanger { HScale, SpinButton }
		
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

/*		protected void ChangeBlurDark(double new_value, BlurDarkChanger changer)
		{
			if (!_BlurDarkIsChanging)
			{
				_BlurDarkIsChanging = true;
				StartChangingParameters();
				
				// Setting all editors to the value
				if (changer != BlurDarkChanger.HScale)			
					blurDark_hscale.Value = new_value;
				
				if (changer != BlurDarkChanger.SpinButton)
					blurDark_spinbutton.Value = new_value;
				
				((BlackPointStageOperationParameters)Parameters).BlurDarkLevel = new_value;
				
				EndChangingParameters();
				OnUserModified();
				_BlurDarkIsChanging = false;
			}
		}
*/
		protected override void HandleParametersChangedNotByUI ()
		{
			_CutIsChanging = true;
			cut_hscale.Value = ((BlackPointStageOperationParameters)Parameters).Cut;
			cut_spinbutton.Value = ((BlackPointStageOperationParameters)Parameters).Cut;
			_CutIsChanging = false;

/*			_BlurDarkIsChanging = true;
			blurDark_hscale.Value = ((BlackPointStageOperationParameters)Parameters).BlurDarkLevel;
			blurDark_spinbutton.Value = ((BlackPointStageOperationParameters)Parameters).BlurDarkLevel;
			_BlurDarkIsChanging = false;*/
		}
		
		protected void OnCutHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeCut(cut_hscale.Value, CutChanger.HScale);
		}

		protected void OnCutSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeCut(cut_spinbutton.Value, CutChanger.SpinButton);
		}

/*		protected void OnBlurDarkHscaleChangeValue (object o, ChangeValueArgs args)
		{
			ChangeBlurDark(blurDark_hscale.Value, BlurDarkChanger.HScale);
		}

		protected void OnBlurDarkSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			ChangeBlurDark(blurDark_spinbutton.Value, BlurDarkChanger.HScale);
		}
*/
	}
}
