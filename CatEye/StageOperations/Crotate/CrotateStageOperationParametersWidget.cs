using System;
using CatEye.Core;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("CrotateStageOperation")]
	public partial class CrotateStageOperationParametersWidget : StageOperationParametersWidget
	{
		Gtk.ListStore ls;
		
		public CrotateStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();

			ls = new Gtk.ListStore(typeof(string), typeof(int));
			string[] ratioNames = ((CrotateStageOperationParameters)Parameters).PresetAspectRatioNames;
			for (int i = 0; i < ratioNames.Length; i++)
			{
				ls.AppendValues(ratioNames[i], i);
			}
			aspect_combobox.Model = ls;
			
			Gtk.TreeIter ti;
			ls.GetIterFirst(out ti);
			
			aspect_combobox.SetActiveIter(ti);
		}
		
		protected void UpdateSensitive()
		{
			crop_w_spinbutton.Sensitive = !link_w_togglebutton.Active;
			crop_h_spinbutton.Sensitive = !link_h_togglebutton.Active;
			
			aspect_combobox.Sensitive = link_w_togglebutton.Active || link_h_togglebutton.Active;
			aspect_combobox.Visible = !custom_checkbutton.Active;
			
			aspect_spinbutton.Sensitive = link_w_togglebutton.Active || link_h_togglebutton.Active;
			aspect_spinbutton.Visible = custom_checkbutton.Active;
			
			custom_checkbutton.Sensitive = link_w_togglebutton.Active || link_h_togglebutton.Active;
			
			aspect_combobox.CheckResize();
		}
		
		protected void OnAngleSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).Angle = angle_spinbutton.Value;
			EndChangingParameters();
			OnUserModified();			
		}

		protected void OnCXSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).Center = new Point(c_x_spinbutton.Value, c_y_spinbutton.Value);
			EndChangingParameters();
			OnUserModified();			
		}

		protected void OnCYSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).Center = new Point(c_x_spinbutton.Value, c_y_spinbutton.Value);
			EndChangingParameters();
			OnUserModified();			
		}

		protected void OnCropWSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).CropWidth = crop_w_spinbutton.Value;
			EndChangingParameters();
			OnUserModified();
		}

		protected void OnCropHSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).CropHeight = crop_h_spinbutton.Value;
			EndChangingParameters();
			OnUserModified();			
		}

		protected void OnAspectComboboxChanged (object sender, System.EventArgs e)
		{
			Gtk.TreeIter ti;
			aspect_combobox.GetActiveIter(out ti);
			int val = (int)ls.GetValue(ti, 1);

			StartChangingParameters();
			aspect_spinbutton.Value = val;
			((CrotateStageOperationParameters)Parameters).AspectRatioPreset = val;
			EndChangingParameters();
			OnUserModified();			
		}

		protected void OnAspectSpinbuttonValueChanged (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).AspectRatio = aspect_spinbutton.Value;
			EndChangingParameters();
			OnUserModified();			
		}

		protected void OnLinkWTogglebuttonToggled (object sender, System.EventArgs e)
		{
			if (link_w_togglebutton.Active)
			{
				link_h_togglebutton.Active = false;
				StartChangingParameters();
				((CrotateStageOperationParameters)Parameters).Mode = CrotateStageOperation.Mode.ProportionalHeightFixed;
				EndChangingParameters();
				OnUserModified();			
			}
			else
			{
				StartChangingParameters();
				((CrotateStageOperationParameters)Parameters).Mode = CrotateStageOperation.Mode.Disproportional;
				EndChangingParameters();
				OnUserModified();			
			}
			UpdateSensitive();
		}

		protected void OnLinkHTogglebuttonToggled (object sender, System.EventArgs e)
		{
			if (link_h_togglebutton.Active)
			{
				link_w_togglebutton.Active = false;
				StartChangingParameters();
				((CrotateStageOperationParameters)Parameters).Mode = CrotateStageOperation.Mode.ProportionalWidthFixed;
				EndChangingParameters();
				OnUserModified();			
			}
			else
			{
				StartChangingParameters();
				((CrotateStageOperationParameters)Parameters).Mode = CrotateStageOperation.Mode.Disproportional;
				EndChangingParameters();
				OnUserModified();			
			}
			UpdateSensitive();
		}

		protected void OnCustomCheckbuttonToggled (object sender, System.EventArgs e)
		{
			StartChangingParameters();
			((CrotateStageOperationParameters)Parameters).AspectRatioCustom = custom_checkbutton.Active;
			EndChangingParameters();
			OnUserModified();			
			UpdateSensitive();
		}

		protected override void HandleParametersChangedNotByUI ()
		{
			angle_spinbutton.Value = ((CrotateStageOperationParameters)Parameters).Angle;
			c_x_spinbutton.Value = ((CrotateStageOperationParameters)Parameters).Center.X;
			c_y_spinbutton.Value = ((CrotateStageOperationParameters)Parameters).Center.Y;
			crop_w_spinbutton.Value = ((CrotateStageOperationParameters)Parameters).CropWidth;
			crop_h_spinbutton.Value = ((CrotateStageOperationParameters)Parameters).CropHeight;
			aspect_spinbutton.Value = ((CrotateStageOperationParameters)Parameters).AspectRatio;
			custom_checkbutton.Active = ((CrotateStageOperationParameters)Parameters).AspectRatioCustom;
			
			Gtk.TreeIter ti;
			if (ls.GetIterFirst(out ti))
			{
				do
				{
					int n = (int)ls.GetValue(ti, 1);
					if (n == ((CrotateStageOperationParameters)Parameters).AspectRatioPreset)
					{
						aspect_combobox.SetActiveIter(ti);
						break;
					}						
				} 
				while (ls.IterNext(ref ti));
			}
			
			link_w_togglebutton.Active = ((CrotateStageOperationParameters)Parameters).Mode == CrotateStageOperation.Mode.ProportionalHeightFixed;
			link_h_togglebutton.Active = ((CrotateStageOperationParameters)Parameters).Mode == CrotateStageOperation.Mode.ProportionalWidthFixed;
			UpdateSensitive();
		}
	}
}

