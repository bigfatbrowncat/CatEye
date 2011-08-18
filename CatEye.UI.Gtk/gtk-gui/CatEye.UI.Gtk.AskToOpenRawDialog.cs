
// This file has been generated by the GUI designer. Do not modify.
namespace CatEye.UI.Gtk
{
	public partial class AskToOpenRawDialog
	{
		private global::Gtk.HBox hbox1;
		private global::Gtk.VBox vbox2;
		private global::Gtk.Label found_label;
		private global::Gtk.Label identification_label;
		private global::Gtk.Image thumb_image;
		private global::Gtk.Label origsize_label;
		private global::Gtk.Label label1;
		private global::Gtk.HScale prescale_hscale;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOpen;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CatEye.UI.Gtk.AskToOpenRawDialog
			this.Name = "CatEye.UI.Gtk.AskToOpenRawDialog";
			this.Title = global::Mono.Unix.Catalog.GetString ("Question");
			this.Icon = global::Gdk.Pixbuf.LoadFromResource ("CatEye.UI.Gtk.res.cateye-small.png");
			this.TypeHint = ((global::Gdk.WindowTypeHint)(1));
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			this.Modal = true;
			this.Resizable = false;
			// Internal child CatEye.UI.Gtk.AskToOpenRawDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 8;
			this.hbox1.BorderWidth = ((uint)(7));
			// Container child hbox1.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.found_label = new global::Gtk.Label ();
			this.found_label.Name = "found_label";
			this.found_label.Xalign = 0F;
			this.found_label.LabelProp = global::Mono.Unix.Catalog.GetString ("Found raw image");
			this.found_label.UseMarkup = true;
			this.vbox2.Add (this.found_label);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.found_label]));
			w2.Position = 0;
			w2.Padding = ((uint)(5));
			// Container child vbox2.Gtk.Box+BoxChild
			this.identification_label = new global::Gtk.Label ();
			this.identification_label.Name = "identification_label";
			this.identification_label.Xalign = 0F;
			this.identification_label.LabelProp = global::Mono.Unix.Catalog.GetString ("<i>Select a file...</i>");
			this.identification_label.UseMarkup = true;
			this.vbox2.Add (this.identification_label);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.identification_label]));
			w3.Position = 1;
			// Container child vbox2.Gtk.Box+BoxChild
			this.thumb_image = new global::Gtk.Image ();
			this.thumb_image.WidthRequest = 200;
			this.thumb_image.HeightRequest = 200;
			this.thumb_image.Name = "thumb_image";
			this.vbox2.Add (this.thumb_image);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.thumb_image]));
			w4.Position = 2;
			w4.Padding = ((uint)(5));
			// Container child vbox2.Gtk.Box+BoxChild
			this.origsize_label = new global::Gtk.Label ();
			this.origsize_label.Name = "origsize_label";
			this.origsize_label.Xalign = 0F;
			this.origsize_label.LabelProp = global::Mono.Unix.Catalog.GetString ("Original size:");
			this.origsize_label.UseMarkup = true;
			this.vbox2.Add (this.origsize_label);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.origsize_label]));
			w5.Position = 3;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Downscale by:");
			this.vbox2.Add (this.label1);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.label1]));
			w6.Position = 4;
			w6.Expand = false;
			w6.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.prescale_hscale = new global::Gtk.HScale (null);
			this.prescale_hscale.CanFocus = true;
			this.prescale_hscale.Name = "prescale_hscale";
			this.prescale_hscale.Adjustment.Lower = 1;
			this.prescale_hscale.Adjustment.Upper = 11;
			this.prescale_hscale.Adjustment.PageIncrement = 1;
			this.prescale_hscale.Adjustment.PageSize = 1;
			this.prescale_hscale.Adjustment.StepIncrement = 1;
			this.prescale_hscale.Adjustment.Value = 2;
			this.prescale_hscale.DrawValue = true;
			this.prescale_hscale.Digits = 0;
			this.prescale_hscale.ValuePos = ((global::Gtk.PositionType)(1));
			this.vbox2.Add (this.prescale_hscale);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.prescale_hscale]));
			w7.Position = 5;
			w7.Expand = false;
			w7.Fill = false;
			this.hbox1.Add (this.vbox2);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vbox2]));
			w8.Position = 0;
			w8.Padding = ((uint)(5));
			w1.Add (this.hbox1);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(w1 [this.hbox1]));
			w9.Position = 0;
			w9.Expand = false;
			w9.Fill = false;
			// Internal child CatEye.UI.Gtk.AskToOpenRawDialog.ActionArea
			global::Gtk.HButtonBox w10 = this.ActionArea;
			w10.Name = "dialog1_ActionArea";
			w10.Spacing = 10;
			w10.BorderWidth = ((uint)(12));
			w10.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(2));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseUnderline = true;
			// Container child buttonCancel.Gtk.Container+ContainerChild
			global::Gtk.Alignment w11 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			global::Gtk.HBox w12 = new global::Gtk.HBox ();
			w12.Spacing = 2;
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Image w13 = new global::Gtk.Image ();
			w13.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-no", global::Gtk.IconSize.Menu);
			w12.Add (w13);
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Label w15 = new global::Gtk.Label ();
			w15.LabelProp = global::Mono.Unix.Catalog.GetString ("_Don't open");
			w15.UseUnderline = true;
			w12.Add (w15);
			w11.Add (w12);
			this.buttonCancel.Add (w11);
			this.AddActionWidget (this.buttonCancel, -9);
			global::Gtk.ButtonBox.ButtonBoxChild w19 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w10 [this.buttonCancel]));
			w19.Expand = false;
			w19.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOpen = new global::Gtk.Button ();
			this.buttonOpen.CanDefault = true;
			this.buttonOpen.CanFocus = true;
			this.buttonOpen.Name = "buttonOpen";
			this.buttonOpen.UseStock = true;
			this.buttonOpen.UseUnderline = true;
			this.buttonOpen.Label = "gtk-open";
			this.AddActionWidget (this.buttonOpen, -8);
			global::Gtk.ButtonBox.ButtonBoxChild w20 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w10 [this.buttonOpen]));
			w20.Position = 1;
			w20.Expand = false;
			w20.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 324;
			this.DefaultHeight = 437;
			this.Hide ();
		}
	}
}
