using System;
using System.Globalization;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class BrightnessStageOperationParametersWidget : StageOperationParametersWidget
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		
		public BrightnessStageOperationParametersWidget (BrightnessStageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
		}

		protected virtual void OnBrightnessEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(brightness_entry.Text, NumberStyles.Float, nfi, out res))
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

