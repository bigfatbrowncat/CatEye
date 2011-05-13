
// This file has been generated by the GUI designer. Do not modify.
namespace CatEye
{
	public partial class BlackPointStageOperationParametersWidget
	{
		private global::Gtk.Table table1;
		private global::Gtk.HScale black_hscale;
		private global::Gtk.SpinButton black_spinbutton;
		private global::Gtk.Label label7;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CatEye.BlackPointStageOperationParametersWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "CatEye.BlackPointStageOperationParametersWidget";
			// Container child CatEye.BlackPointStageOperationParametersWidget.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table (((uint)(1)), ((uint)(3)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			this.table1.BorderWidth = ((uint)(4));
			// Container child table1.Gtk.Table+TableChild
			this.black_hscale = new global::Gtk.HScale (null);
			this.black_hscale.CanFocus = true;
			this.black_hscale.Name = "black_hscale";
			this.black_hscale.Adjustment.Upper = 0.999;
			this.black_hscale.Adjustment.PageIncrement = 0.005;
			this.black_hscale.Adjustment.StepIncrement = 0.01;
			this.black_hscale.Adjustment.Value = 0.001;
			this.black_hscale.DrawValue = false;
			this.black_hscale.Digits = 3;
			this.black_hscale.ValuePos = ((global::Gtk.PositionType)(1));
			this.table1.Add (this.black_hscale);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1 [this.black_hscale]));
			w1.LeftAttach = ((uint)(1));
			w1.RightAttach = ((uint)(2));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.black_spinbutton = new global::Gtk.SpinButton (0, 1, 0.01);
			this.black_spinbutton.CanFocus = true;
			this.black_spinbutton.Name = "black_spinbutton";
			this.black_spinbutton.Adjustment.PageIncrement = 0.05;
			this.black_spinbutton.ClimbRate = 0.001;
			this.black_spinbutton.Digits = ((uint)(3));
			this.black_spinbutton.Numeric = true;
			this.table1.Add (this.black_spinbutton);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this.black_spinbutton]));
			w2.LeftAttach = ((uint)(2));
			w2.RightAttach = ((uint)(3));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.Xalign = 1F;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("Black:");
			this.table1.Add (this.label7);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.label7]));
			w3.XPadding = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			this.Add (this.table1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.black_spinbutton.ValueChanged += new global::System.EventHandler (this.OnBlackSpinbuttonValueChanged);
			this.black_hscale.ChangeValue += new global::Gtk.ChangeValueHandler (this.OnBlackHscaleChangeValue);
		}
	}
}