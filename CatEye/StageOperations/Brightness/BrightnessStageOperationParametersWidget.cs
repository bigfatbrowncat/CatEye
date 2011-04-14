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
			brightness_entry.Text = ((BrightnessStageOperationParameters)Parameters).Brightness.ToString();
		}
	}
}

