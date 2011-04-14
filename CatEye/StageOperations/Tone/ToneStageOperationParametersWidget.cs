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
		}
		
		protected virtual void OnSaturationEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(red_entry.Text, out res))
			{
				try
				{
					StartChangingParameters();
					((ToneStageOperationParameters)Parameters).RedPart = res;
					EndChangingParameters();
					OnUserModified();
				}
				catch (IncorrectValueException)
				{
				}
			}
			if (double.TryParse(green_entry.Text, out res))
			{
				try
				{
					StartChangingParameters();
					((ToneStageOperationParameters)Parameters).GreenPart = res;
					EndChangingParameters();
					OnUserModified();
				}
				catch (IncorrectValueException)
				{
				}
			}
			if (double.TryParse(blue_entry.Text, out res))
			{
				try
				{
					StartChangingParameters();
					((ToneStageOperationParameters)Parameters).BluePart = res;
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
			red_entry.Text = ((ToneStageOperationParameters)Parameters).RedPart .ToString();
			green_entry.Text = ((ToneStageOperationParameters)Parameters).GreenPart.ToString();
			blue_entry.Text = ((ToneStageOperationParameters)Parameters).BluePart.ToString();
		}
	}
}
