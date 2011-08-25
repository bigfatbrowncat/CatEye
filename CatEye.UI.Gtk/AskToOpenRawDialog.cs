using System;
using Gtk;
using System.Diagnostics;
using CatEye.Core;

namespace CatEye.UI.Gtk
{
	public partial class AskToOpenRawDialog : Dialog
	{
		public string Filename 
		{ 
			get { return rawpreviewwidget.Filename; } 
			set 
			{ 
				rawpreviewwidget.Filename = value; 
				found_label.Markup = "Image <b>" + System.IO.Path.GetFileName(value) + "</b> has been found\nfor the cestage file you had selected.";
			}
		}

		public int PreScale 
		{ 
			get 
			{
				return prescaleselectorwidget.Value;
			}
			set
			{
				prescaleselectorwidget.Value = value;
			}
		}
				
		protected override void OnShown ()
		{
			//Title = MainClass.APP_NAME;
			base.OnShown ();
		}
		
		public AskToOpenRawDialog ()
		{
			this.Build ();
			ActionArea.BorderWidth = 10;
			ActionArea.Layout = ButtonBoxStyle.Spread;
		}
	}
}

