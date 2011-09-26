using System;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	[StageOperationID("HistogramStageOperation")]
	public partial class HistogramStageOperationParametersWidget : StageOperationParametersWidget
	{
		public HistogramStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
			
		{
			this.Build ();
		}
	}
}

