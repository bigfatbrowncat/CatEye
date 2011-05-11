using System;
using CatEye.Core;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("ScaleStageOperation")]
	public partial class ScaleStageOperationParametersWidget : StageOperationParametersWidget
	{
		public ScaleStageOperationParametersWidget (StageOperationParameters parameters) : 
			base(parameters)
		{
			this.Build ();
		}

		protected void OnWidthSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((ScaleStageOperationParameters)Parameters).NewWidth = width_spinbutton.Value;
			EndChangingParameters();
			OnUserModified();
		}

		protected void OnHeightSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((ScaleStageOperationParameters)Parameters).NewHeight = height_spinbutton.Value;
			EndChangingParameters();
			OnUserModified();
		}
		
		protected override void HandleParametersChangedNotByUI ()
		{
			width_spinbutton.Value = ((ScaleStageOperationParameters)Parameters).NewWidth;
			height_spinbutton.Value = ((ScaleStageOperationParameters)Parameters).NewHeight;
		}
	}
}

