using System;
using Gtk;

namespace CatEye
{


	[System.ComponentModel.ToolboxItem(true)]
	public partial class DownscalingStageOperationParametersWidget : StageOperationParametersWidget
	{
		private ListStore ls;
		
		private int mScaleValue = 1;
		
		protected virtual void OnScaleComboboxChanged (object sender, System.EventArgs e)
		{
			TreeIter ti;
			if (scale_combobox.GetActiveIter(out ti))
			{
				mScaleValue = (int)ls.GetValue(ti, 1);
				OnUserModified();
			}
		}
		
		public int ScaleValue 
		{ 
			get { return mScaleValue; } 
			set
			{
				if ((value >= 1) && (value <= 8))
				{
					mScaleValue = value;
					// searching for iter
					TreeIter ti;
					if (ls.GetIterFirst(out ti))
					{
						do 
						{
							int val = (int)ls.GetValue(ti, 1);
							if (val == value) break;
						} while (ls.IterNext(ref ti));
					}
					
					
				}
			}
		}
		
		public DownscalingStageOperationParametersWidget ()
		{
			this.Build ();
			
			ls = new Gtk.ListStore(typeof(string), typeof(int));
			ls.AppendValues("No downscaling", 1);
			for (int i = 2; i <= 8; i++)
				ls.AppendValues("Divide by " + i, i);
			
			scale_combobox.Model = ls;
			
			// Selecting "No downscale"
			TreeIter ti;
			ls.GetIterFirst(out ti);
			scale_combobox.SetActiveIter(ti);
			
		}
	}
}
