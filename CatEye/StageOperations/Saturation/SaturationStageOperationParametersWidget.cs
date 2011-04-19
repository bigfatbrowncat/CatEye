using System;
using System.Globalization;
using CatEye.Core;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("SaturationStageOperation")]
	public partial class SaturationStageOperationParametersWidget : StageOperationParametersWidget
	{
		private bool mChangingValue = false;
		private double mMaxValue = 2;
		
		public double MaxValue 
		{
			get { return mMaxValue; }
			set 
			{
				mMaxValue = value;
				saturation_hscale.Adjustment.Upper = value;
				saturation_entry.Text = ((SaturationStageOperationParameters)Parameters).Saturation.ToString("0.00");
			}
		}
		
		public SaturationStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
			HandleParametersChangedNotByUI();
			MaxValue = 3;
			
		}
		
		protected virtual void OnSaturationEntryChanged (object sender, System.EventArgs e)
		{
			if (!mChangingValue)
			{
				mChangingValue = true;
				double res = 0;
				if (double.TryParse(saturation_entry.Text, out res))
				{
					try
					{
						StartChangingParameters();
						if (res > mMaxValue) res = mMaxValue;
						((SaturationStageOperationParameters)Parameters).Saturation = res;
						saturation_hscale.Value = res;
						EndChangingParameters();
						OnUserModified();
					}
					catch (IncorrectValueException)
					{
					}
				}
				mChangingValue = false;
			}
			
		}
		
		protected override void HandleParametersChangedNotByUI ()
		{
			saturation_entry.Text = ((SaturationStageOperationParameters)Parameters).Saturation.ToString("0.00");
			saturation_hscale.Value = ((SaturationStageOperationParameters)Parameters).Saturation;
		}

		protected void OnSaturationHscaleValueChanged (object sender, System.EventArgs e)
		{
			if (!mChangingValue)
			{
				mChangingValue = true;
				StartChangingParameters();
				((SaturationStageOperationParameters)Parameters).Saturation = saturation_hscale.Value;
				saturation_entry.Text =  saturation_hscale.Value.ToString("0.00");
				EndChangingParameters();
				OnUserModified();
				mChangingValue = false;
			}
		}

		protected void OnSaturationHscaleMoveSlider (object o, Gtk.MoveSliderArgs args)
		{

		}

		protected void OnSaturationEntryEditingDone (object sender, System.EventArgs e)
		{
		}

		protected void OnSaturationEntryFocusOutEvent (object o, Gtk.FocusOutEventArgs args)
		{
			saturation_entry.Text = ((SaturationStageOperationParameters)Parameters).Saturation.ToString("0.00");
		}
	}
}
