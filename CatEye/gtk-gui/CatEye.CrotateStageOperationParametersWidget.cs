
// This file has been generated by the GUI designer. Do not modify.
namespace CatEye
{
	public partial class CrotateStageOperationParametersWidget
	{
		private global::Gtk.Table table1;
		private global::Gtk.SpinButton angle_spinbutton;
		private global::Gtk.HBox ar_hbox;
		private global::Gtk.SpinButton aspect_spinbutton;
		private global::Gtk.ComboBox aspect_combobox;
		private global::Gtk.SpinButton c_x_spinbutton;
		private global::Gtk.SpinButton c_y_spinbutton;
		private global::Gtk.SpinButton crop_h_spinbutton;
		private global::Gtk.SpinButton crop_w_spinbutton;
		private global::Gtk.CheckButton custom_checkbutton;
		private global::Gtk.Label label10;
		private global::Gtk.Label label11;
		private global::Gtk.Label label12;
		private global::Gtk.Label label7;
		private global::Gtk.Label label8;
		private global::Gtk.Label label9;
		private global::Gtk.ToggleButton link_h_togglebutton;
		private global::Gtk.ToggleButton link_w_togglebutton;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CatEye.CrotateStageOperationParametersWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "CatEye.CrotateStageOperationParametersWidget";
			// Container child CatEye.CrotateStageOperationParametersWidget.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table (((uint)(7)), ((uint)(3)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.angle_spinbutton = new global::Gtk.SpinButton (-180, 180, 1);
			this.angle_spinbutton.CanFocus = true;
			this.angle_spinbutton.Name = "angle_spinbutton";
			this.angle_spinbutton.Adjustment.PageIncrement = 1;
			this.angle_spinbutton.ClimbRate = 0.1;
			this.angle_spinbutton.Digits = ((uint)(1));
			this.angle_spinbutton.Numeric = true;
			this.angle_spinbutton.Value = 45;
			this.table1.Add (this.angle_spinbutton);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1 [this.angle_spinbutton]));
			w1.LeftAttach = ((uint)(1));
			w1.RightAttach = ((uint)(3));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.ar_hbox = new global::Gtk.HBox ();
			this.ar_hbox.Name = "ar_hbox";
			this.ar_hbox.Spacing = 6;
			// Container child ar_hbox.Gtk.Box+BoxChild
			this.aspect_spinbutton = new global::Gtk.SpinButton (0.25, 4, 1);
			this.aspect_spinbutton.CanFocus = true;
			this.aspect_spinbutton.Name = "aspect_spinbutton";
			this.aspect_spinbutton.Adjustment.PageIncrement = 0.1;
			this.aspect_spinbutton.ClimbRate = 0.1;
			this.aspect_spinbutton.Digits = ((uint)(3));
			this.aspect_spinbutton.Numeric = true;
			this.aspect_spinbutton.Value = 0.25;
			this.ar_hbox.Add (this.aspect_spinbutton);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.ar_hbox [this.aspect_spinbutton]));
			w2.Position = 0;
			// Container child ar_hbox.Gtk.Box+BoxChild
			this.aspect_combobox = global::Gtk.ComboBox.NewText ();
			this.aspect_combobox.Name = "aspect_combobox";
			this.ar_hbox.Add (this.aspect_combobox);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.ar_hbox [this.aspect_combobox]));
			w3.Position = 1;
			this.table1.Add (this.ar_hbox);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1 [this.ar_hbox]));
			w4.TopAttach = ((uint)(6));
			w4.BottomAttach = ((uint)(7));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(3));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.c_x_spinbutton = new global::Gtk.SpinButton (-2, 3, 0.01);
			this.c_x_spinbutton.CanFocus = true;
			this.c_x_spinbutton.Name = "c_x_spinbutton";
			this.c_x_spinbutton.Adjustment.PageIncrement = 0.1;
			this.c_x_spinbutton.ClimbRate = 0.1;
			this.c_x_spinbutton.Digits = ((uint)(2));
			this.c_x_spinbutton.Numeric = true;
			this.c_x_spinbutton.Value = 0.5;
			this.table1.Add (this.c_x_spinbutton);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1 [this.c_x_spinbutton]));
			w5.TopAttach = ((uint)(1));
			w5.BottomAttach = ((uint)(2));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(3));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.c_y_spinbutton = new global::Gtk.SpinButton (-2, 3, 0.01);
			this.c_y_spinbutton.CanFocus = true;
			this.c_y_spinbutton.Name = "c_y_spinbutton";
			this.c_y_spinbutton.Adjustment.PageIncrement = 0.1;
			this.c_y_spinbutton.ClimbRate = 0.1;
			this.c_y_spinbutton.Digits = ((uint)(2));
			this.c_y_spinbutton.Numeric = true;
			this.c_y_spinbutton.Value = 0.5;
			this.table1.Add (this.c_y_spinbutton);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1 [this.c_y_spinbutton]));
			w6.TopAttach = ((uint)(2));
			w6.BottomAttach = ((uint)(3));
			w6.LeftAttach = ((uint)(1));
			w6.RightAttach = ((uint)(3));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.crop_h_spinbutton = new global::Gtk.SpinButton (0.01, 3, 0.01);
			this.crop_h_spinbutton.CanFocus = true;
			this.crop_h_spinbutton.Name = "crop_h_spinbutton";
			this.crop_h_spinbutton.Adjustment.PageIncrement = 0.1;
			this.crop_h_spinbutton.ClimbRate = 0.1;
			this.crop_h_spinbutton.Digits = ((uint)(2));
			this.crop_h_spinbutton.Numeric = true;
			this.crop_h_spinbutton.Value = 0.5;
			this.table1.Add (this.crop_h_spinbutton);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1 [this.crop_h_spinbutton]));
			w7.TopAttach = ((uint)(4));
			w7.BottomAttach = ((uint)(5));
			w7.LeftAttach = ((uint)(1));
			w7.RightAttach = ((uint)(2));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.crop_w_spinbutton = new global::Gtk.SpinButton (0.01, 3, 0.01);
			this.crop_w_spinbutton.CanFocus = true;
			this.crop_w_spinbutton.Name = "crop_w_spinbutton";
			this.crop_w_spinbutton.Adjustment.PageIncrement = 0.1;
			this.crop_w_spinbutton.ClimbRate = 0.1;
			this.crop_w_spinbutton.Digits = ((uint)(2));
			this.crop_w_spinbutton.Numeric = true;
			this.crop_w_spinbutton.Value = 0.5;
			this.table1.Add (this.crop_w_spinbutton);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1 [this.crop_w_spinbutton]));
			w8.TopAttach = ((uint)(3));
			w8.BottomAttach = ((uint)(4));
			w8.LeftAttach = ((uint)(1));
			w8.RightAttach = ((uint)(2));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.custom_checkbutton = new global::Gtk.CheckButton ();
			this.custom_checkbutton.CanFocus = true;
			this.custom_checkbutton.Name = "custom_checkbutton";
			this.custom_checkbutton.Label = global::Mono.Unix.Catalog.GetString ("Custom");
			this.custom_checkbutton.DrawIndicator = true;
			this.custom_checkbutton.UseUnderline = true;
			this.table1.Add (this.custom_checkbutton);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1 [this.custom_checkbutton]));
			w9.TopAttach = ((uint)(5));
			w9.BottomAttach = ((uint)(6));
			w9.LeftAttach = ((uint)(1));
			w9.RightAttach = ((uint)(3));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label10 = new global::Gtk.Label ();
			this.label10.Name = "label10";
			this.label10.Xalign = 1F;
			this.label10.LabelProp = global::Mono.Unix.Catalog.GetString ("Crop Width:");
			this.table1.Add (this.label10);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table1 [this.label10]));
			w10.TopAttach = ((uint)(3));
			w10.BottomAttach = ((uint)(4));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label11 = new global::Gtk.Label ();
			this.label11.Name = "label11";
			this.label11.Xalign = 1F;
			this.label11.LabelProp = global::Mono.Unix.Catalog.GetString ("Crop Height:");
			this.table1.Add (this.label11);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table1 [this.label11]));
			w11.TopAttach = ((uint)(4));
			w11.BottomAttach = ((uint)(5));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label12 = new global::Gtk.Label ();
			this.label12.Name = "label12";
			this.label12.Xalign = 1F;
			this.label12.LabelProp = global::Mono.Unix.Catalog.GetString ("Aspect ratio:");
			this.table1.Add (this.label12);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table1 [this.label12]));
			w12.TopAttach = ((uint)(5));
			w12.BottomAttach = ((uint)(6));
			w12.YPadding = ((uint)(3));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.Xalign = 1F;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("Angle:");
			this.table1.Add (this.label7);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table1 [this.label7]));
			w13.XPadding = ((uint)(2));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.label8.Xalign = 1F;
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("Center X:");
			this.table1.Add (this.label8);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table1 [this.label8]));
			w14.TopAttach = ((uint)(1));
			w14.BottomAttach = ((uint)(2));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label9 = new global::Gtk.Label ();
			this.label9.Name = "label9";
			this.label9.Xalign = 1F;
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("Center Y:");
			this.table1.Add (this.label9);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table1 [this.label9]));
			w15.TopAttach = ((uint)(2));
			w15.BottomAttach = ((uint)(3));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.link_h_togglebutton = new global::Gtk.ToggleButton ();
			this.link_h_togglebutton.CanFocus = true;
			this.link_h_togglebutton.Name = "link_h_togglebutton";
			this.link_h_togglebutton.UseUnderline = true;
			this.link_h_togglebutton.Label = global::Mono.Unix.Catalog.GetString ("Link");
			this.table1.Add (this.link_h_togglebutton);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table1 [this.link_h_togglebutton]));
			w16.TopAttach = ((uint)(4));
			w16.BottomAttach = ((uint)(5));
			w16.LeftAttach = ((uint)(2));
			w16.RightAttach = ((uint)(3));
			w16.XOptions = ((global::Gtk.AttachOptions)(0));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.link_w_togglebutton = new global::Gtk.ToggleButton ();
			this.link_w_togglebutton.CanFocus = true;
			this.link_w_togglebutton.Name = "link_w_togglebutton";
			this.link_w_togglebutton.UseUnderline = true;
			this.link_w_togglebutton.Label = global::Mono.Unix.Catalog.GetString ("Link");
			this.table1.Add (this.link_w_togglebutton);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table1 [this.link_w_togglebutton]));
			w17.TopAttach = ((uint)(3));
			w17.BottomAttach = ((uint)(4));
			w17.LeftAttach = ((uint)(2));
			w17.RightAttach = ((uint)(3));
			w17.XOptions = ((global::Gtk.AttachOptions)(0));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			this.Add (this.table1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.link_w_togglebutton.Toggled += new global::System.EventHandler (this.OnLinkWTogglebuttonToggled);
			this.link_h_togglebutton.Toggled += new global::System.EventHandler (this.OnLinkHTogglebuttonToggled);
			this.custom_checkbutton.Toggled += new global::System.EventHandler (this.OnCustomCheckbuttonToggled);
			this.crop_w_spinbutton.ValueChanged += new global::System.EventHandler (this.OnCropWSpinbuttonValueChanged);
			this.crop_h_spinbutton.ValueChanged += new global::System.EventHandler (this.OnCropHSpinbuttonValueChanged);
			this.c_y_spinbutton.ValueChanged += new global::System.EventHandler (this.OnCYSpinbuttonValueChanged);
			this.c_x_spinbutton.ValueChanged += new global::System.EventHandler (this.OnCXSpinbuttonValueChanged);
			this.aspect_spinbutton.ValueChanged += new global::System.EventHandler (this.OnAspectSpinbuttonValueChanged);
			this.aspect_combobox.Changed += new global::System.EventHandler (this.OnAspectComboboxChanged);
			this.angle_spinbutton.ValueChanged += new global::System.EventHandler (this.OnAngleSpinbuttonValueChanged);
		}
	}
}
