using System;
using System.Collections.Generic;
using Gtk;

namespace CatEye.UI.Gtk
{
	public static class WindowsSystemColorsHelper
	{
		public static Gdk.Color SelectionColor = new Gdk.Color(92, 192, 64);
		
		public static Widget[] PaintIntoWindowsColors(Widget widget, Widget[] descendantsToExclude)
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
			Gdk.Color selectedButton_color = new Gdk.Color((byte)(((double)SelectionColor.Red + 3 * button_color.Red + 65535) / 5 / 255),
			                                               (byte)(((double)SelectionColor.Green + 3 * button_color.Green + 65535)  / 5 / 255),
			                                               (byte)(((double)SelectionColor.Blue + 3 * button_color.Blue + 65535) / 5 / 255));
	
			// Enumerate all children
			bool all_enum = false;
			List<Widget> children_recursive = new List<Widget>();
			List<Widget> passed = new List<Widget>();

			if (descendantsToExclude == null) descendantsToExclude = new Widget[] {};
			List<Widget> to_exclude = new List<Widget>(descendantsToExclude);
			List<Widget> result = new List<Widget>(descendantsToExclude);

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
								if (!to_exclude.Contains(w))
									children_recursive.Add(w);
							}
						}
						result.Add(chld);
					}
				}
				
			} while (!all_enum);
			
			foreach (Widget chld in children_recursive)
			{
				chld.ModifyBg(StateType.Selected, SelectionColor);
				chld.ModifyBg(StateType.Normal, button_color);
				chld.ModifyBg(StateType.Active, button_color);
				chld.ModifyBg(StateType.Insensitive, button_color);
				chld.ModifyBg(StateType.Prelight, selectedButton_color);
				
				chld.ModifyCursor(controlText_color, buttonHighlight_color);
	
				chld.ModifyFg(StateType.Selected, controlText_color);
				chld.ModifyFg(StateType.Normal, controlText_color);
				chld.ModifyFg(StateType.Active, controlText_color);
				chld.ModifyFg(StateType.Insensitive, grayText_color);
				chld.ModifyFg(StateType.Prelight, controlText_color);
				
				chld.ModifyBase(StateType.Insensitive, button_color);	// Text editors back color when they are inactive
				chld.ModifyBase(StateType.Selected, SelectionColor);	// Selection in text editors
				chld.ModifyBase(StateType.Active, SelectionColor);	// Selection in text editors
			}
			
			return result.ToArray();
		}

	}
}

