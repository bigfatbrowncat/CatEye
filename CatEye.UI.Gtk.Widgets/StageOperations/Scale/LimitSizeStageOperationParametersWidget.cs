using System;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("LimitSizeStageOperation")]
	public partial class LimitSizeStageOperationParametersWidget : StageOperationParametersWidget
	{
		public LimitSizeStageOperationParametersWidget (StageOperationParameters parameters) : 
			base(parameters)
		{
			this.Build ();
		}

		protected void OnWidthSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((LimitSizeStageOperationParameters)Parameters).NewWidth = width_spinbutton.Value;
			EndChangingParameters();
			OnUserModified();
		}

		protected void OnHeightSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((LimitSizeStageOperationParameters)Parameters).NewHeight = height_spinbutton.Value;
			EndChangingParameters();
			OnUserModified();
		}
		
		protected override void HandleParametersChangedNotByUI ()
		{
			width_spinbutton.Value = ((LimitSizeStageOperationParameters)Parameters).NewWidth;
			height_spinbutton.Value = ((LimitSizeStageOperationParameters)Parameters).NewHeight;
			limit_w_togglebutton.Active = ((LimitSizeStageOperationParameters)Parameters).LimitWidth;
			limit_h_togglebutton.Active = ((LimitSizeStageOperationParameters)Parameters).LimitHeight;

			width_spinbutton.Sensitive = limit_w_togglebutton.Active;
			height_spinbutton.Sensitive = limit_h_togglebutton.Active;
		}

		protected void OnLimitWTogglebuttonToggled (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((LimitSizeStageOperationParameters)Parameters).LimitWidth = limit_w_togglebutton.Active;
			EndChangingParameters();
			OnUserModified();
			width_spinbutton.Sensitive = limit_w_togglebutton.Active;
		}

		protected void OnLimitHTogglebuttonToggled (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((LimitSizeStageOperationParameters)Parameters).LimitHeight = limit_h_togglebutton.Active;
			EndChangingParameters();
			OnUserModified();
			height_spinbutton.Sensitive = limit_h_togglebutton.Active;
		}
	}
}

