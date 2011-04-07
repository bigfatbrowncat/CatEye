using System;
using System.Globalization;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("SaturationStageOperation")]
	public partial class SaturationStageOperationParametersWidget : StageOperationParametersWidget
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		public SaturationStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
		}
		
		protected virtual void OnSaturationEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(saturation_entry.Text, NumberStyles.Float, nfi, out res))
			{
				try
				{
					StartChangingParameters();
					((SaturationStageOperationParameters)Parameters).Saturation = res;
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
			saturation_entry.Text = ((SaturationStageOperationParameters)Parameters).Saturation.ToString();
		}
	}
}
