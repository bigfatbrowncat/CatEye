using System;
using System.Collections.Generic;
using Gtk;

namespace CatEye.UI.Gtk
{
	public static class WindowsSystemColorsHelper
	{
		private static List<Widget> addedWidgets = new List<Widget>();
		public static void AssureStyleColors(Widget widget)
		{
			// Checking of the OS
			if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
				Environment.OSVersion.Platform == PlatformID.Win32S ||
				Environment.OSVersion.Platform == PlatformID.Win32Windows ||
				Environment.OSVersion.Platform == PlatformID.WinCE)
			{
				// Enumerate all children
				bool all_enum = false;
				List<Widget> children_recursive = new List<Widget>();
				List<Widget> passed = new List<Widget>();
	
	
				children_recursive.Add(widget);
				do
				{
					all_enum = true;
					Widget[] chreccur = children_recursive.ToArray();
					for (int i = 0; i < chreccur.Length; i++)
					{
						Widget chld = chreccur[i];
						if (!passed.Contains(chld))
						{
							passed.Add(chld);
							if (chld is Container) 
							{
								all_enum = false;
								foreach (Widget w in ((Container)chld).AllChildren) 
								{
										children_recursive.Add(w);
								}
							}
						}
					}
					
				} while (!all_enum);
				foreach (Widget chld in children_recursive)
				{
					if (!addedWidgets.Contains(chld))
					{
						addedWidgets.Add(chld);
						chld.Shown += HandleChldShown;
						if (chld is Container)
						{
							((Container)chld).Added += delegate(object o, AddedArgs args) {
								AssureStyleColors(args.Widget);
							};
						}
						HandleChldShown(chld, EventArgs.Empty);
					}
				}
			}
		}

		static void HandleChldShown (object sender, EventArgs e)
		{
			Gdk.Color control_color = new Gdk.Color(System.Drawing.SystemColors.Control.R,
			                                        System.Drawing.SystemColors.Control.G,
			                                        System.Drawing.SystemColors.Control.B);
			Gdk.Color button_color = new Gdk.Color(System.Drawing.SystemColors.ButtonFace.R,
			                                       System.Drawing.SystemColors.ButtonFace.G,
			                                       System.Drawing.SystemColors.ButtonFace.B);
			Gdk.Color buttonHighlight_color = new Gdk.Color(System.Drawing.SystemColors.ButtonHighlight.R,
			                                                System.Drawing.SystemColors.ButtonHighlight.G,
			                                                System.Drawing.SystemColors.ButtonHighlight.B);
			Gdk.Color controlText_color = new Gdk.Color(System.Drawing.SystemColors.ControlText.R,
			                                            System.Drawing.SystemColors.ControlText.G,
			                                            System.Drawing.SystemColors.ControlText.B);
			Gdk.Color grayText_color = new Gdk.Color(System.Drawing.SystemColors.GrayText.R,
			                                         System.Drawing.SystemColors.GrayText.G,
			                                         System.Drawing.SystemColors.GrayText.B);
			Widget me = (Widget)sender;

			Gdk.Color selected_color = me.Style.Backgrounds[(int)StateType.Selected];
			Gdk.Color selectedButton_color = new Gdk.Color((byte)(((double)selected_color.Red + 3 * button_color.Red + 65535) / 5 / 255),
			                                               (byte)(((double)selected_color.Green + 3 * button_color.Green + 65535)  / 5 / 255),
			                                               (byte)(((double)selected_color.Blue + 3 * button_color.Blue + 65535) / 5 / 255));
			
			if ((me is Button) || (me is Scale) || (me is ToolButton))
			{
				me.ModifyBg(StateType.Normal, button_color);
				me.ModifyBg(StateType.Active, button_color);
			}
			else
			{
				me.ModifyBg(StateType.Normal, control_color);
				me.ModifyBg(StateType.Active, control_color);
			}

			me.ModifyBg(StateType.Selected, selected_color);
			me.ModifyBg(StateType.Insensitive, button_color);
			me.ModifyBg(StateType.Prelight, selectedButton_color);
			
			me.ModifyCursor(controlText_color, buttonHighlight_color);

			me.ModifyFg(StateType.Selected, controlText_color);
			me.ModifyFg(StateType.Normal, controlText_color);
			me.ModifyFg(StateType.Active, controlText_color);
			me.ModifyFg(StateType.Insensitive, grayText_color);
			me.ModifyFg(StateType.Prelight, controlText_color);
			
			me.ModifyBase(StateType.Insensitive, control_color);	// Text editors back color when they are inactive
			me.ModifyBase(StateType.Selected, selected_color);	// Selection in text editors
			me.ModifyBase(StateType.Active, selected_color);	// Selection in text editors
		}

	}
}

