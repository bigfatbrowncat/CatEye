using System;
using System.Globalization;
using CatEye.Core;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("BrightnessStageOperation")]
	public partial class BrightnessStageOperationParametersWidget : StageOperationParametersWidget
	{
		public BrightnessStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
			HandleParametersChangedNotByUI();
		}

		protected virtual void OnBrightnessEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(brightness_entry.Text, out res))
			{
				try
				{
					StartChangingParameters();
					((BrightnessStageOperationParameters)Parameters).Brightness = res;
					
					logbrightness_hscale.Value = Math.Log(res + 0.95, 5.95);

					EndChangingParameters();
					OnUserModified();
				}
				catch (IncorrectValueException)
				{
				}
			}
		}
		
		protected override void HandleParametersChangedNotByUI ()
		{
			double brt = ((BrightnessStageOperationParameters)Parameters).Brightness;
			brightness_entry.Text = brt.ToString();
			logbrightness_hscale.Value = Math.Log(brt + 0.95, 5.95);
			
			normalize_togglebutton.Active = ((BrightnessStageOperationParameters)Parameters).Normalize;
			
			double med = ((BrightnessStageOperationParameters)Parameters).Median;
			median_label.Text = med.ToString("0.00");
		}

		protected void OnLogbrightnessHscaleChangeValue (object o, Gtk.ChangeValueArgs args)
		{
			StartChangingParameters();
			brightness_entry.Text = (Math.Pow(5.95, logbrightness_hscale.Value) - 0.95).ToString("0.00");

			double brt = ((BrightnessStageOperationParameters)Parameters).Brightness;
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
	}
}

