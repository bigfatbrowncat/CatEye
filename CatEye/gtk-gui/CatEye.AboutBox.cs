
// This file has been generated by the GUI designer. Do not modify.
namespace CatEye
{
	public partial class AboutBox
	{
		private global::Gtk.VBox vbox2;

		private global::Gtk.HBox hbox1;

		private global::Gtk.Image image20;

		private global::Gtk.Label title_label;

		private global::Gtk.Fixed fixed1;

		private global::Gtk.Label label2;

		private global::Gtk.Label label6;

		private global::Gtk.Label label4;

		private global::Gtk.Label label3;

		private global::Gtk.Label label5;

		private global::Gtk.Button close_button;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CatEye.AboutBox
			this.Name = "CatEye.AboutBox";
			this.Title = global::Mono.Unix.Catalog.GetString ("About CatEye");
			this.Icon = global::Gdk.Pixbuf.LoadFromResource ("CatEye.res.cateye-small.png");
			this.TypeHint = ((global::Gdk.WindowTypeHint)(1));
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			this.Modal = true;
			this.Resizable = false;
			this.DestroyWithParent = true;
			this.SkipPagerHint = true;
			this.SkipTaskbarHint = true;
			// Internal child CatEye.AboutBox.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			this.vbox2.BorderWidth = ((uint)(12));
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.image20 = new global::Gtk.Image ();
			this.image20.Name = "image20";
			this.image20.Pixbuf = global::Gdk.Pixbuf.LoadFromResource ("CatEye.res.cateye.png");
			this.hbox1.Add (this.image20);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.image20]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.title_label = new global::Gtk.Label ();
			this.title_label.Name = "title_label";
			this.title_label.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>CatEye 0.3</b>");
			this.title_label.UseMarkup = true;
			this.hbox1.Add (this.title_label);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.title_label]));
			w3.Position = 1;
			w3.Padding = ((uint)(13));
			// Container child hbox1.Gtk.Box+BoxChild
			this.fixed1 = new global::Gtk.Fixed ();
			this.fixed1.WidthRequest = 48;
			this.fixed1.HeightRequest = 48;
			this.fixed1.Name = "fixed1";
			this.fixed1.HasWindow = false;
			this.hbox1.Add (this.fixed1);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.fixed1]));
			w4.Position = 2;
			w4.Expand = false;
			w4.Fill = false;
			this.vbox2.Add (this.hbox1);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.hbox1]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Idea, design and development:");
			this.label2.UseMarkup = true;
			this.vbox2.Add (this.label2);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.label2]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			w6.Padding = ((uint)(6));
			// Container child vbox2.Gtk.Box+BoxChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Ilya Mizus</b>");
			this.label6.UseMarkup = true;
			this.label6.Wrap = true;
			this.vbox2.Add (this.label6);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.label6]));
			w7.Position = 2;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Special thanks to:");
			this.vbox2.Add (this.label4);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.label4]));
			w8.Position = 3;
			w8.Expand = false;
			w8.Padding = ((uint)(6));
			// Container child vbox2.Gtk.Box+BoxChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Alexey Ischenko</b> for cross-platform testing of the software.");
			this.label3.UseMarkup = true;
			this.label3.Wrap = true;
			this.vbox2.Add (this.label3);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.label3]));
			w9.Position = 4;
			w9.Expand = false;
			w9.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Dave Coffin</b> for perfect and genial <b>dcraw</b> software.");
			this.label5.UseMarkup = true;
			this.label5.Wrap = true;
			this.vbox2.Add (this.label5);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.label5]));
			w10.Position = 5;
			w10.Expand = false;
			w10.Fill = false;
			w1.Add (this.vbox2);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(w1[this.vbox2]));
			w11.Position = 0;
			w11.Expand = false;
			w11.Fill = false;
			// Internal child CatEye.AboutBox.ActionArea
			global::Gtk.HButtonBox w12 = this.ActionArea;
			w12.Name = "dialog1_ActionArea";
			w12.Spacing = 10;
			w12.BorderWidth = ((uint)(5));
			w12.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.close_button = new global::Gtk.Button ();
			this.close_button.CanDefault = true;
			this.close_button.CanFocus = true;
			this.close_button.Name = "close_button";
			this.close_button.UseStock = true;
			this.close_button.UseUnderline = true;
			this.close_button.Label = "gtk-close";
			this.AddActionWidget (this.close_button, -7);
			global::Gtk.ButtonBox.ButtonBoxChild w13 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w12[this.close_button]));
			w13.Expand = false;
			w13.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 424;
			this.DefaultHeight = 275;
			this.Show ();
		}
	}
}
