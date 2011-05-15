using System;
using CatEye.Core;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("RotateStageOperation")]
	public partial class RotateStageOperationParametersWidget : StageOperationParametersWidget
	{
		public RotateStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
		}

		protected void OnAngleSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((RotateStageOperationParameters)Parameters).Angle = angle_spinbutton.Value;
			EndChangingParameters();
			OnUserModified();			
		}

		protected override void HandleParametersChangedNotByUI ()
		{
			angle_spinbutton.Value = ((RotateStageOperationParameters)Parameters).Angle;
		}
	}
}

