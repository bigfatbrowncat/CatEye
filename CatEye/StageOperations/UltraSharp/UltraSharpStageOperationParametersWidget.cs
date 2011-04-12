using System;
using System.Globalization;
using CatEye.Core;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true), StageOperationID("UltraSharpStageOperation")]
	public partial class UltraSharpStageOperationParametersWidget : StageOperationParametersWidget
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		public UltraSharpStageOperationParametersWidget (StageOperationParameters parameters) :
			base(parameters)
		{
			this.Build ();
		}
		
		protected virtual void OnPowerEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(power_entry.Text, NumberStyles.Float, nfi, out res))
			{
				try
				{
					StartChangingParameters();
					((UltraSharpStageOperationParameters)Parameters).Power = res;
					EndChangingParameters();
					OnUserModified();
				}
				catch (IncorrectValueException)
				{
				}
			}
		}
		
		protected virtual void OnPointsEntryChanged (object sender, System.EventArgs e)
		{
			int res = 0;
			if (int.TryParse(points_entry.Text, NumberStyles.Float, nfi, out res))
			{
				try
				{
					StartChangingParameters();
					((UltraSharpStageOperationParameters)Parameters).Points = res;
					EndChangingParameters();
					OnUserModified();
				}
				catch (IncorrectValueException)
				{
				}
			}

		}
		
		protected virtual void OnRadiusHscaleChangeValue (object o, Gtk.ChangeValueArgs args)
		{
			try
			{
				StartChangingParameters();
				((UltraSharpStageOperationParameters)Parameters).Power = radius_hscale.Value;
				EndChangingParameters();
				OnUserModified();
			}
			catch (IncorrectValueException)
			{
			}
		}
		
		protected void OnDelta0EntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(delta_0_entry.Text, NumberStyles.Float, nfi, out res))
			{
				try
				{
					StartChangingParameters();
					((UltraSharpStageOperationParameters)Parameters).Delta0 = res;
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
			power_entry.Text = ((UltraSharpStageOperationParameters)Parameters).Power.ToString();
			points_entry.Text = ((UltraSharpStageOperationParameters)Parameters).Points.ToString();
			delta_0_entry.Text = ((UltraSharpStageOperationParameters)Parameters).Delta0.ToString();
			radius_hscale.Value = ((UltraSharpStageOperationParameters)Parameters).Radius;
		}
	}
}
