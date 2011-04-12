using System;
using System.Globalization;
using CatEye.Core;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("CompressionStageOperation")]
	public partial class CompressionStageOperationParametersWidget : StageOperationParametersWidget
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		public CompressionStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
		}

		protected virtual void OnPowerEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(power_entry.Text, NumberStyles.Float, nfi, out res))
			{
				try
				{
					StartChangingParameters();
					((CompressionStageOperationParameters)Parameters).Power = res;
					EndChangingParameters();
					OnUserModified();
				}
				catch (IncorrectValueException)
				{
				}
			}
		}
		
		protected virtual void OnBlohaEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(bloha_entry.Text, NumberStyles.Float, nfi, out res))
			{
				try
				{
					StartChangingParameters();
					((CompressionStageOperationParameters)Parameters).Bloha = res;
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
			power_entry.Text = ((CompressionStageOperationParameters)Parameters).Power.ToString();
			bloha_entry.Text = ((CompressionStageOperationParameters)Parameters).Bloha.ToString();
		}
		
	}
}
