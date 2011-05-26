
// This file has been generated by the GUI designer. Do not modify.
namespace CatEye
{
	public partial class UltraSharpStageOperationParametersWidget
	{
		private global::Gtk.Table table1;
		private global::Gtk.HBox hbox1;
		private global::Gtk.RadioButton sharp_radiobutton;
		private global::Gtk.RadioButton soft_radiobutton;
		private global::Gtk.Label label12;
		private global::Gtk.Label label14;
		private global::Gtk.Label label15;
		private global::Gtk.Label label7;
		private global::Gtk.HScale limitdown_hscale;
		private global::Gtk.SpinButton limitdown_spinbutton;
		private global::Gtk.HScale limitup_hscale;
		private global::Gtk.SpinButton limitup_spinbutton;
		private global::Gtk.HScale power_hscale;
		private global::Gtk.SpinButton power_spinbutton;
		private global::Gtk.HScale radius_hscale;
		private global::Gtk.SpinButton radius_spinbutton;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CatEye.UltraSharpStageOperationParametersWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "CatEye.UltraSharpStageOperationParametersWidget";
			// Container child CatEye.UltraSharpStageOperationParametersWidget.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table (((uint)(5)), ((uint)(3)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			this.table1.BorderWidth = ((uint)(2));
			// Container child table1.Gtk.Table+TableChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Homogeneous = true;
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.sharp_radiobutton = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("Sharp"));
			this.sharp_radiobutton.CanFocus = true;
			this.sharp_radiobutton.Name = "sharp_radiobutton";
			this.sharp_radiobutton.DrawIndicator = false;
			this.sharp_radiobutton.UseUnderline = true;
			this.sharp_radiobutton.Group = new global::GLib.SList (global::System.IntPtr.Zero);
			this.hbox1.Add (this.sharp_radiobutton);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.sharp_radiobutton]));
			w1.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.soft_radiobutton = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("Soft"));
			this.soft_radiobutton.CanFocus = true;
			this.soft_radiobutton.Name = "soft_radiobutton";
			this.soft_radiobutton.DrawIndicator = false;
			this.soft_radiobutton.UseUnderline = true;
			this.soft_radiobutton.Group = this.sharp_radiobutton.Group;
			this.hbox1.Add (this.soft_radiobutton);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.soft_radiobutton]));
			w2.Position = 1;
			this.table1.Add (this.hbox1);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.hbox1]));
			w3.RightAttach = ((uint)(3));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label12 = new global::Gtk.Label ();
			this.label12.Name = "label12";
			this.label12.Xalign = 1F;
			this.label12.LabelProp = global::Mono.Unix.Catalog.GetString ("Radius:");
			this.table1.Add (this.label12);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1 [this.label12]));
			w4.TopAttach = ((uint)(2));
			w4.BottomAttach = ((uint)(3));
			w4.XPadding = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label14 = new global::Gtk.Label ();
			this.label14.Name = "label14";
			this.label14.Xalign = 1F;
			this.label14.LabelProp = global::Mono.Unix.Catalog.GetString ("Upper Limit:");
			this.table1.Add (this.label14);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1 [this.label14]));
			w5.TopAttach = ((uint)(3));
			w5.BottomAttach = ((uint)(4));
			w5.XPadding = ((uint)(2));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label15 = new global::Gtk.Label ();
			this.label15.Name = "label15";
			this.label15.Xalign = 1F;
			this.label15.LabelProp = global::Mono.Unix.Catalog.GetString ("Lower Limit:");
			this.table1.Add (this.label15);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1 [this.label15]));
			w6.TopAttach = ((uint)(4));
			w6.BottomAttach = ((uint)(5));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.Xalign = 1F;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("Power:");
			this.table1.Add (this.label7);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1 [this.label7]));
			w7.TopAttach = ((uint)(1));
			w7.BottomAttach = ((uint)(2));
			w7.XPadding = ((uint)(2));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.limitdown_hscale = new global::Gtk.HScale (null);
			this.limitdown_hscale.CanFocus = true;
			this.limitdown_hscale.Name = "limitdown_hscale";
			this.limitdown_hscale.Inverted = true;
			this.limitdown_hscale.Adjustment.Upper = 1;
			this.limitdown_hscale.Adjustment.PageIncrement = 0.01;
			this.limitdown_hscale.Adjustment.StepIncrement = 0.01;
			this.limitdown_hscale.Adjustment.Value = 0.5;
			this.limitdown_hscale.DrawValue = false;
			this.limitdown_hscale.Digits = 3;
			this.limitdown_hscale.ValuePos = ((global::Gtk.PositionType)(1));
			this.table1.Add (this.limitdown_hscale);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1 [this.limitdown_hscale]));
			w8.TopAttach = ((uint)(4));
			w8.BottomAttach = ((uint)(5));
			w8.LeftAttach = ((uint)(1));
			w8.RightAttach = ((uint)(2));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.limitdown_spinbutton = new global::Gtk.SpinButton (0, 1, 0.1);
			this.limitdown_spinbutton.CanFocus = true;
			this.limitdown_spinbutton.Name = "limitdown_spinbutton";
			this.limitdown_spinbutton.Adjustment.PageIncrement = 10;
			this.limitdown_spinbutton.ClimbRate = 1;
			this.limitdown_spinbutton.Digits = ((uint)(2));
			this.limitdown_spinbutton.Numeric = true;
			this.limitdown_spinbutton.Value = 0.5;
			this.table1.Add (this.limitdown_spinbutton);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1 [this.limitdown_spinbutton]));
			w9.TopAttach = ((uint)(4));
			w9.BottomAttach = ((uint)(5));
			w9.LeftAttach = ((uint)(2));
			w9.RightAttach = ((uint)(3));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.limitup_hscale = new global::Gtk.HScale (null);
			this.limitup_hscale.CanFocus = true;
			this.limitup_hscale.Name = "limitup_hscale";
			this.limitup_hscale.Adjustment.Upper = 1;
			this.limitup_hscale.Adjustment.PageIncrement = 0.01;
			this.limitup_hscale.Adjustment.StepIncrement = 0.01;
			this.limitup_hscale.Adjustment.Value = 0.5;
			this.limitup_hscale.DrawValue = false;
			this.limitup_hscale.Digits = 3;
			this.limitup_hscale.ValuePos = ((global::Gtk.PositionType)(1));
			this.table1.Add (this.limitup_hscale);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table1 [this.limitup_hscale]));
			w10.TopAttach = ((uint)(3));
			w10.BottomAttach = ((uint)(4));
			w10.LeftAttach = ((uint)(1));
			w10.RightAttach = ((uint)(2));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.limitup_spinbutton = new global::Gtk.SpinButton (0, 1, 0.1);
			this.limitup_spinbutton.CanFocus = true;
			this.limitup_spinbutton.Name = "limitup_spinbutton";
			this.limitup_spinbutton.Adjustment.PageIncrement = 10;
			this.limitup_spinbutton.ClimbRate = 1;
			this.limitup_spinbutton.Digits = ((uint)(2));
			this.limitup_spinbutton.Numeric = true;
			this.limitup_spinbutton.Value = 0.5;
			this.table1.Add (this.limitup_spinbutton);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table1 [this.limitup_spinbutton]));
			w11.TopAttach = ((uint)(3));
			w11.BottomAttach = ((uint)(4));
			w11.LeftAttach = ((uint)(2));
			w11.RightAttach = ((uint)(3));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.power_hscale = new global::Gtk.HScale (null);
			this.power_hscale.CanFocus = true;
			this.power_hscale.Name = "power_hscale";
			this.power_hscale.Adjustment.Upper = 10;
			this.power_hscale.Adjustment.PageIncrement = 0.5;
			this.power_hscale.Adjustment.StepIncrement = 0.1;
			this.power_hscale.Adjustment.Value = 0.001;
			this.power_hscale.DrawValue = false;
			this.power_hscale.Digits = 3;
			this.power_hscale.ValuePos = ((global::Gtk.PositionType)(1));
			this.table1.Add (this.power_hscale);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table1 [this.power_hscale]));
			w12.TopAttach = ((uint)(1));
			w12.BottomAttach = ((uint)(2));
			w12.LeftAttach = ((uint)(1));
			w12.RightAttach = ((uint)(2));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.power_spinbutton = new global::Gtk.SpinButton (0, 100, 0.1);
			this.power_spinbutton.CanFocus = true;
			this.power_spinbutton.Name = "power_spinbutton";
			this.power_spinbutton.Adjustment.PageIncrement = 5;
			this.power_spinbutton.ClimbRate = 1;
			this.power_spinbutton.Digits = ((uint)(1));
			this.power_spinbutton.Numeric = true;
			this.table1.Add (this.power_spinbutton);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table1 [this.power_spinbutton]));
			w13.TopAttach = ((uint)(1));
			w13.BottomAttach = ((uint)(2));
			w13.LeftAttach = ((uint)(2));
			w13.RightAttach = ((uint)(3));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.radius_hscale = new global::Gtk.HScale (null);
			this.radius_hscale.CanFocus = true;
			this.radius_hscale.Name = "radius_hscale";
			this.radius_hscale.Adjustment.Lower = 0.001;
			this.radius_hscale.Adjustment.Upper = 1;
			this.radius_hscale.Adjustment.PageIncrement = 0.01;
			this.radius_hscale.Adjustment.StepIncrement = 0.001;
			this.radius_hscale.Adjustment.Value = 0.001;
			this.radius_hscale.DrawValue = false;
			this.radius_hscale.Digits = 3;
			this.radius_hscale.ValuePos = ((global::Gtk.PositionType)(1));
			this.table1.Add (this.radius_hscale);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table1 [this.radius_hscale]));
			w14.TopAttach = ((uint)(2));
			w14.BottomAttach = ((uint)(3));
			w14.LeftAttach = ((uint)(1));
			w14.RightAttach = ((uint)(2));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.radius_spinbutton = new global::Gtk.SpinButton (0.001, 1, 0.01);
			this.radius_spinbutton.CanFocus = true;
			this.radius_spinbutton.Name = "radius_spinbutton";
			this.radius_spinbutton.Adjustment.PageIncrement = 0.1;
			this.radius_spinbutton.ClimbRate = 1;
			this.radius_spinbutton.Digits = ((uint)(3));
			this.radius_spinbutton.Numeric = true;
			this.radius_spinbutton.Value = 0.001;
			this.table1.Add (this.radius_spinbutton);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table1 [this.radius_spinbutton]));
			w15.TopAttach = ((uint)(2));
			w15.BottomAttach = ((uint)(3));
			w15.LeftAttach = ((uint)(2));
			w15.RightAttach = ((uint)(3));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			this.Add (this.table1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.radius_spinbutton.ValueChanged += new global::System.EventHandler (this.OnRadiusSpinbuttonValueChanged);
			this.radius_hscale.ChangeValue += new global::Gtk.ChangeValueHandler (this.OnRadiusHscaleChangeValue);
			this.power_spinbutton.ValueChanged += new global::System.EventHandler (this.OnPowerSpinbuttonValueChanged);
			this.power_hscale.ChangeValue += new global::Gtk.ChangeValueHandler (this.OnPowerHscaleChangeValue);
			this.limitup_spinbutton.ValueChanged += new global::System.EventHandler (this.OnLimitupSpinbuttonValueChanged);
			this.limitup_hscale.ChangeValue += new global::Gtk.ChangeValueHandler (this.OnLimitupHscaleChangeValue);
			this.limitdown_spinbutton.ValueChanged += new global::System.EventHandler (this.OnLimitdownSpinbuttonValueChanged);
			this.limitdown_hscale.ChangeValue += new global::Gtk.ChangeValueHandler (this.OnLimitdownHscaleChangeValue);
			this.sharp_radiobutton.Toggled += new global::System.EventHandler (this.OnSharpSoftToggled);
			this.soft_radiobutton.Toggled += new global::System.EventHandler (this.OnSharpSoftToggled);
		}
	}
}
