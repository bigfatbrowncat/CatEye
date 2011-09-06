using System;
using System.Globalization;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	[StageOperationID("SaturationStageOperation")]
	public partial class SaturationStageOperationParametersWidget : StageOperationParametersWidget
	{
		private bool mChangingValue = false;
		private double mMaxValue;
		
		public double MaxValue 
		{
			get { return mMaxValue; }
			set 
			{
				mMaxValue = value;
				saturation_hscale.Adjustment.Upper = value;
				saturation_spinbutton.Adjustment.Upper = value;
				saturation_spinbutton.Value = ((SaturationStageOperationParameters)Parameters).Saturation;
			}
		}
		
		public SaturationStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
			//HandleParametersChangedNotByUI();
			MaxValue = 3;
			
		}
		
		protected override void HandleParametersChangedNotByUI ()
		{
			if (!mChangingValue)
			{
				mChangingValue = true;
				saturation_spinbutton.Value = ((SaturationStageOperationParameters)Parameters).Saturation;
				saturation_hscale.Value = ((SaturationStageOperationParameters)Parameters).Saturation;
				mChangingValue = false;
			}
		}

		protected void OnSaturationHscaleValueChanged (object sender, System.EventArgs e)
		{
			if (!mChangingValue)
			{
				mChangingValue = true;
				
				StartChangingParameters();
				((SaturationStageOperationParameters)Parameters).Saturation = saturation_hscale.Value;
				saturation_spinbutton.Value = saturation_hscale.Value;
				EndChangingParameters();
				
				OnUserModified();
				mChangingValue = false;
			}
		}

		protected void OnSaturationSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			if (!mChangingValue)
			{
				StartChangingParameters();
				
				((SaturationStageOperationParameters)Parameters).Saturation = saturation_spinbutton.Value;
				saturation_hscale.Value = saturation_spinbutton.Value;
	
				EndChangingParameters();
				OnUserModified();
			}
		}
	}
}
