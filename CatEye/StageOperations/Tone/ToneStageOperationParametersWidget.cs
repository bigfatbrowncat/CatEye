using System;
using System.Globalization;
using CatEye.Core;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("ToneStageOperation")]
	public partial class ToneStageOperationParametersWidget : StageOperationParametersWidget
	{
		public ToneStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
			toneselectorwidget1.ToneSelected += OnToneselectorwidget1ToneSelected;
			toneselectorwidget1.SelectedToneChanged += OnToneselectorwidget1SelectedToneChanged;
		}
		
		protected override void HandleParametersChangedNotByUI ()
		{
			
			Tone tn = new Tone(((ToneStageOperationParameters)Parameters).RedPart,
							   ((ToneStageOperationParameters)Parameters).GreenPart,
							   ((ToneStageOperationParameters)Parameters).BluePart);
			toneselectorwidget1.SelectedTone = new Tone(1, 1, 1);
		}

		protected void OnToneselectorwidget1SelectedToneChanged (object sender, System.EventArgs e)
		{
			r_label.Text = toneselectorwidget1.SelectedTone.R.ToString("0.00");
			g_label.Text = toneselectorwidget1.SelectedTone.G.ToString("0.00");
			b_label.Text = toneselectorwidget1.SelectedTone.B.ToString("0.00");
		}

		protected void OnToneselectorwidget1ToneSelected (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((ToneStageOperationParameters)Parameters).RedPart = toneselectorwidget1.SelectedTone.R;
			((ToneStageOperationParameters)Parameters).GreenPart = toneselectorwidget1.SelectedTone.G;
			((ToneStageOperationParameters)Parameters).BluePart = toneselectorwidget1.SelectedTone.B;
			EndChangingParameters();
		}
	}
}
