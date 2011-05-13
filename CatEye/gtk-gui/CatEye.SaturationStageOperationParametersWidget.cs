
// This file has been generated by the GUI designer. Do not modify.
namespace CatEye
{
	public partial class SaturationStageOperationParametersWidget
	{
		private global::Gtk.Table table1;

		private global::CatEye.SaturationSelectorWidget saturation_hscale;

		private global::Gtk.SpinButton saturation_spinbutton;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CatEye.SaturationStageOperationParametersWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "CatEye.SaturationStageOperationParametersWidget";
			// Container child CatEye.SaturationStageOperationParametersWidget.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table (((uint)(1)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(2));
			this.table1.ColumnSpacing = ((uint)(6));
			this.table1.BorderWidth = ((uint)(4));
			// Container child table1.Gtk.Table+TableChild
			this.saturation_hscale = new global::CatEye.SaturationSelectorWidget (null);
			this.saturation_hscale.Name = "saturation_hscale";
			this.saturation_hscale.Adjustment.Upper = 2;
			this.saturation_hscale.Adjustment.PageIncrement = 0.1;
			this.saturation_hscale.Adjustment.StepIncrement = 0.01;
			this.saturation_hscale.Adjustment.Value = 1;
			this.saturation_hscale.DrawValue = false;
			this.saturation_hscale.Digits = 0;
			this.saturation_hscale.ValuePos = ((global::Gtk.PositionType)(2));
			this.table1.Add (this.saturation_hscale);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1[this.saturation_hscale]));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.saturation_spinbutton = new global::Gtk.SpinButton (0, 10, 0.1);
			this.saturation_spinbutton.CanFocus = true;
			this.saturation_spinbutton.Name = "saturation_spinbutton";
			this.saturation_spinbutton.Adjustment.PageIncrement = 1;
			this.saturation_spinbutton.ClimbRate = 0.1;
			this.saturation_spinbutton.Digits = ((uint)(2));
			this.saturation_spinbutton.Numeric = true;
			this.saturation_spinbutton.Value = 0.05;
			this.table1.Add (this.saturation_spinbutton);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1[this.saturation_spinbutton]));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			this.Add (this.table1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.saturation_spinbutton.ValueChanged += new global::System.EventHandler (this.OnSaturationSpinbuttonValueChanged);
			this.saturation_hscale.ValueChanged += new global::System.EventHandler (this.OnSaturationHscaleValueChanged);
		}
	}
}
