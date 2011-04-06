using System;
using System.Globalization;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("BasicOpsStageOperation")]
	public partial class BasicOpsStageOperationParametersWidget : StageOperationParametersWidget
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		public BasicOpsStageOperationParametersWidget (StageOperationParameters parameters) :
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
					((BasicOpsStageOperationParameters)Parameters).Brightness = res;
					EndChangingParameters();
					OnUserModified();
				}
				catch (IncorrectValueException)
				{
				}
			}
		}
		
		protected virtual void OnSaturationEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(saturation_entry.Text, NumberStyles.Float, nfi, out res))
			{
				try
				{
					StartChangingParameters();
					((BasicOpsStageOperationParameters)Parameters).Saturation = res;
					EndChangingParameters();
					OnUserModified();
				}
				catch (IncorrectValueException)
				{
				}
			}
		}
		
		protected virtual void OnRedEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(red_entry.Text, NumberStyles.Float, nfi, out res))
			{
				try
				{
					StartChangingParameters();
					((BasicOpsStageOperationParameters)Parameters).RedPart = res;
					EndChangingParameters();
					OnUserModified();
				}
				catch (IncorrectValueException)
				{
				}
			}
		}
		
		protected virtual void OnGreenEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(green_entry.Text, NumberStyles.Float, nfi, out res))
			{
				try
				{
					StartChangingParameters();
					((BasicOpsStageOperationParameters)Parameters).GreenPart = res;
					EndChangingParameters();
					OnUserModified();
				}
				catch (IncorrectValueException)
				{
				}
			}
		}
		
		protected virtual void OnBlueEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(blue_entry.Text, NumberStyles.Float, nfi, out res))
			{
				try
				{
					StartChangingParameters();
					((BasicOpsStageOperationParameters)Parameters).BluePart = res;
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
			brightness_entry.Text = ((BasicOpsStageOperationParameters)Parameters).Brightness.ToString();
			saturation_entry.Text = ((BasicOpsStageOperationParameters)Parameters).Saturation.ToString();
			red_entry.Text = ((BasicOpsStageOperationParameters)Parameters).RedPart.ToString();
			green_entry.Text = ((BasicOpsStageOperationParameters)Parameters).GreenPart.ToString();
			blue_entry.Text = ((BasicOpsStageOperationParameters)Parameters).BluePart.ToString();
		}
	}
}
