using System;
using System.Globalization;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	[StageOperationID("BrightnessStageOperation")]
	public partial class BrightnessStageOperationParametersWidget : StageOperationParametersWidget
	{
		public BrightnessStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
		}

		protected override void HandleParametersChangedNotByUI ()
		{
			double brt = ((BrightnessStageOperationParameters)Parameters).Brightness;
			brightness_spinbutton.Value = brt;
			logbrightness_hscale.Value = Math.Log(brt + 0.95, 10.95);
			
			normalize_togglebutton.Active = ((BrightnessStageOperationParameters)Parameters).Normalize;
			
		}
		
		public override void AnalyzeImage (IBitmapCore image)
		{
			double median = image.AmplitudeFindMedian();
			Application.Invoke(delegate {
				median_label.Text = median.ToString("0.00");
			});			
		}
		
		protected void OnLogbrightnessHscaleChangeValue (object o, ChangeValueArgs args)
		{
			StartChangingParameters();
			brightness_spinbutton.Value = (Math.Pow(10.95, logbrightness_hscale.Value) - 0.95);

			EndChangingParameters();
			OnUserModified();
		}

		protected void OnNormalizeTogglebuttonClicked (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((BrightnessStageOperationParameters)Parameters).Normalize = normalize_togglebutton.Active;
			EndChangingParameters();
			OnUserModified();
		}

		protected void OnBrightnessSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			
			((BrightnessStageOperationParameters)Parameters).Brightness = brightness_spinbutton.Value;
			
			logbrightness_hscale.Value = Math.Log(brightness_spinbutton.Value + 0.95, 10.95);

			EndChangingParameters();
			OnUserModified();
		}
	}
}

