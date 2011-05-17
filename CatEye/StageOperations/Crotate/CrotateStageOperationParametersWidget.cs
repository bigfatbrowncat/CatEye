using System;
using CatEye.Core;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("CrotateStageOperation")]
	public partial class CrotateStageOperationParametersWidget : StageOperationParametersWidget
	{
		public CrotateStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
		}

		protected void OnAngleSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).Angle = angle_spinbutton.Value;
			EndChangingParameters();
			OnUserModified();			
		}

		protected override void HandleParametersChangedNotByUI ()
		{
			angle_spinbutton.Value = ((CrotateStageOperationParameters)Parameters).Angle;
		}
	}
}

