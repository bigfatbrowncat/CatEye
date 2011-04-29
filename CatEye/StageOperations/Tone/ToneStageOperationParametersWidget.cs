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
			toneselectorwidget1.Alpha = 0.5;
		}
		
		protected override void HandleParametersChangedNotByUI ()
		{
			
			Tone tn = new Tone(((ToneStageOperationParameters)Parameters).RedPart,
							   ((ToneStageOperationParameters)Parameters).GreenPart,
							   ((ToneStageOperationParameters)Parameters).BluePart);
			toneselectorwidget1.SelectedTone = tn;
		}

		protected void OnToneselectorwidget1SelectedToneChanged (object sender, System.EventArgs e)
		{
			r_label.Markup = "R: <b>" + toneselectorwidget1.SelectedTone.R.ToString("0.00") + "</b>";
			g_label.Markup = "G: <b>" + toneselectorwidget1.SelectedTone.G.ToString("0.00") + "</b>";
			b_label.Markup = "B: <b>" + toneselectorwidget1.SelectedTone.B.ToString("0.00") + "</b>";
		}

		protected void OnToneselectorwidget1ToneSelected (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((ToneStageOperationParameters)Parameters).RedPart = toneselectorwidget1.SelectedTone.R;
			((ToneStageOperationParameters)Parameters).GreenPart = toneselectorwidget1.SelectedTone.G;
			((ToneStageOperationParameters)Parameters).BluePart = toneselectorwidget1.SelectedTone.B;
			EndChangingParameters();
			OnUserModified();
		}

		protected void OnAlphaVscaleChangeValue (object o, Gtk.ChangeValueArgs args)
		{
			toneselectorwidget1.Alpha = alpha_vscale.Value;
		}
	}
}
