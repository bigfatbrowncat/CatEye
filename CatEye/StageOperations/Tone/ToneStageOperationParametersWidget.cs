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
			//HandleParametersChangedNotByUI();
		}
		
		protected override void HandleParametersChangedNotByUI ()
		{
			red_entry.Text = ((ToneStageOperationParameters)Parameters).RedPart.ToString();
			green_entry.Text = ((ToneStageOperationParameters)Parameters).GreenPart.ToString();
			blue_entry.Text = ((ToneStageOperationParameters)Parameters).BluePart.ToString();
		}

		protected void OnRedEntryChanged (object sender, System.EventArgs e)
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
		}

		protected void OnGreenEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
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
		}

		protected void OnBlueEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
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
	}
}
