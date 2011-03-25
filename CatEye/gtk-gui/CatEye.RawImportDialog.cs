
// This file has been generated by the GUI designer. Do not modify.
namespace CatEye
{
	public partial class RawImportDialog
	{
		private global::Gtk.HBox hbox1;

		private global::Gtk.FileChooserWidget filechooserwidget;

		private global::Gtk.VBox vbox2;

		private global::Gtk.Label identification_label;

		private global::Gtk.Image thumb_image;

		private global::Gtk.Label origsize_label;

		private global::Gtk.Label label1;

		private global::Gtk.ComboBox prescale_combobox;

		private global::Gtk.Button cancel_button;

		private global::Gtk.Button open_button;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CatEye.RawImportDialog
			this.Name = "CatEye.RawImportDialog";
			this.Title = global::Mono.Unix.Catalog.GetString ("Import raw images");
			this.TypeHint = ((global::Gdk.WindowTypeHint)(1));
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			this.Modal = true;
			this.DestroyWithParent = true;
			this.SkipTaskbarHint = true;
			// Internal child CatEye.RawImportDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 8;
			this.hbox1.BorderWidth = ((uint)(7));
			// Container child hbox1.Gtk.Box+BoxChild
			this.filechooserwidget = new global::Gtk.FileChooserWidget (((global::Gtk.FileChooserAction)(0)));
			this.filechooserwidget.Name = "filechooserwidget";
			this.hbox1.Add (this.filechooserwidget);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.filechooserwidget]));
			w2.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.identification_label = new global::Gtk.Label ();
			this.identification_label.Name = "identification_label";
			this.identification_label.Xalign = 0f;
			this.identification_label.LabelProp = global::Mono.Unix.Catalog.GetString ("<i>Select a file...</i>");
			this.identification_label.UseMarkup = true;
			this.vbox2.Add (this.identification_label);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.identification_label]));
			w3.Position = 0;
			// Container child vbox2.Gtk.Box+BoxChild
			this.thumb_image = new global::Gtk.Image ();
			this.thumb_image.Name = "thumb_image";
			this.vbox2.Add (this.thumb_image);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.thumb_image]));
			w4.Position = 1;
			// Container child vbox2.Gtk.Box+BoxChild
			this.origsize_label = new global::Gtk.Label ();
			this.origsize_label.Name = "origsize_label";
			this.origsize_label.Xalign = 0f;
			this.origsize_label.LabelProp = global::Mono.Unix.Catalog.GetString ("Original size:");
			this.origsize_label.UseMarkup = true;
			this.vbox2.Add (this.origsize_label);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.origsize_label]));
			w5.Position = 2;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xalign = 0f;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Downscale on loading:");
			this.vbox2.Add (this.label1);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.label1]));
			w6.Position = 3;
			w6.Expand = false;
			w6.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.prescale_combobox = global::Gtk.ComboBox.NewText ();
			this.prescale_combobox.Name = "prescale_combobox";
			this.vbox2.Add (this.prescale_combobox);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.prescale_combobox]));
			w7.Position = 4;
			w7.Expand = false;
			w7.Fill = false;
			this.hbox1.Add (this.vbox2);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.vbox2]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			w1.Add (this.hbox1);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(w1[this.hbox1]));
			w9.Position = 0;
			// Internal child CatEye.RawImportDialog.ActionArea
			global::Gtk.HButtonBox w10 = this.ActionArea;
			w10.Name = "dialog1_ActionArea";
			w10.Spacing = 10;
			w10.BorderWidth = ((uint)(5));
			w10.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.cancel_button = new global::Gtk.Button ();
			this.cancel_button.CanDefault = true;
			this.cancel_button.CanFocus = true;
			this.cancel_button.Name = "cancel_button";
			this.cancel_button.UseStock = true;
			this.cancel_button.UseUnderline = true;
			this.cancel_button.Label = "gtk-cancel";
			this.AddActionWidget (this.cancel_button, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w11 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w10[this.cancel_button]));
			w11.Expand = false;
			w11.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.open_button = new global::Gtk.Button ();
			this.open_button.CanDefault = true;
			this.open_button.CanFocus = true;
			this.open_button.Name = "open_button";
			this.open_button.UseStock = true;
			this.open_button.UseUnderline = true;
			this.open_button.Label = "gtk-open";
			this.AddActionWidget (this.open_button, -3);
			global::Gtk.ButtonBox.ButtonBoxChild w12 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w10[this.open_button]));
			w12.Position = 1;
			w12.Expand = false;
			w12.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 718;
			this.DefaultHeight = 421;
			this.Show ();
			this.Response += new global::Gtk.ResponseHandler (this.OnResponse);
			this.Close += new global::System.EventHandler (this.OnClose);
			this.filechooserwidget.SelectionChanged += new global::System.EventHandler (this.OnFilechooserwidgetSelectionChanged);
			this.filechooserwidget.FileActivated += new global::System.EventHandler (this.OnFilechooserwidgetFileActivated);
		}
	}
}
