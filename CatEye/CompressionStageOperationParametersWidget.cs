using System;
using System.Globalization;

namespace CatEye
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CompressionStageOperationParametersWidget : StageOperationParametersWidget
	{
		private double mPower = 0.7, mBloha = 0.01;
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		public CompressionStageOperationParametersWidget ()
		{
			this.Build ();
		}

		public double Power
		{
			get { return mPower; }
			set 
			{
				mPower = value;
				power_entry.Text = value.ToString();
			}
		}
		public double Bloha
		{
			get { return mBloha; }
			set 
			{
				mBloha = value;
				bloha_entry.Text = value.ToString();
			}
		}
		
		protected virtual void OnPowerEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(power_entry.Text, NumberStyles.Float, nfi, out res))
			{
				mPower = res;
				OnUserModified();
			}
		}
		
		protected virtual void OnBlohaEntryChanged (object sender, System.EventArgs e)
		{
			double res = 0;
			if (double.TryParse(bloha_entry.Text, NumberStyles.Float, nfi, out res))
			{
				mBloha = res;
				OnUserModified();
			}
		}
		
		
		
		
	}
}
