
// This file has been generated by the GUI designer. Do not modify.
namespace CatEye
{
	public partial class CompressionStageOperationParametersWidget
	{
		private global::Gtk.Table table2;
		private global::Gtk.Entry bloha_entry;
		private global::Gtk.Label label3;
		private global::Gtk.Label label6;
		private global::Gtk.Entry power_entry;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CatEye.CompressionStageOperationParametersWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "CatEye.CompressionStageOperationParametersWidget";
			// Container child CatEye.CompressionStageOperationParametersWidget.Gtk.Container+ContainerChild
			this.table2 = new global::Gtk.Table (((uint)(2)), ((uint)(2)), true);
			this.table2.Name = "table2";
			this.table2.RowSpacing = ((uint)(6));
			this.table2.ColumnSpacing = ((uint)(6));
			// Container child table2.Gtk.Table+TableChild
			this.bloha_entry = new global::Gtk.Entry ();
			this.bloha_entry.CanFocus = true;
			this.bloha_entry.Name = "bloha_entry";
			this.bloha_entry.Text = global::Mono.Unix.Catalog.GetString ("0");
			this.bloha_entry.IsEditable = true;
			this.bloha_entry.WidthChars = 6;
			this.bloha_entry.InvisibleChar = '●';
			this.table2.Add (this.bloha_entry);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table2 [this.bloha_entry]));
			w1.TopAttach = ((uint)(1));
			w1.BottomAttach = ((uint)(2));
			w1.LeftAttach = ((uint)(1));
			w1.RightAttach = ((uint)(2));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 1F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Power:");
			this.table2.Add (this.label3);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table2 [this.label3]));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.Xalign = 1F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Bloha:");
			this.table2.Add (this.label6);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table2 [this.label6]));
			w3.TopAttach = ((uint)(1));
			w3.BottomAttach = ((uint)(2));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.power_entry = new global::Gtk.Entry ();
			this.power_entry.CanFocus = true;
			this.power_entry.Name = "power_entry";
			this.power_entry.Text = global::Mono.Unix.Catalog.GetString ("0");
			this.power_entry.IsEditable = true;
			this.power_entry.WidthChars = 6;
			this.power_entry.InvisibleChar = '●';
			this.table2.Add (this.power_entry);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table2 [this.power_entry]));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			this.Add (this.table2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.power_entry.Changed += new global::System.EventHandler (this.OnPowerEntryChanged);
			this.bloha_entry.Changed += new global::System.EventHandler (this.OnBlohaEntryChanged);
		}
	}
}
