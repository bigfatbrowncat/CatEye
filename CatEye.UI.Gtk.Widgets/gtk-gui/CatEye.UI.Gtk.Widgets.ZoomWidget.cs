
// This file has been generated by the GUI designer. Do not modify.
namespace CatEye.UI.Gtk.Widgets
{
	public partial class ZoomWidget
	{
		private global::Gtk.HBox hbox1;
		private global::Gtk.Button zoomOut_button;
		private global::Gtk.Label zoom_label;
		private global::Gtk.HScale zoom_hscale;
		private global::Gtk.Button zoomIn_button;
		private global::Gtk.Button zoom100_button;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CatEye.UI.Gtk.Widgets.ZoomWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "CatEye.UI.Gtk.Widgets.ZoomWidget";
			// Container child CatEye.UI.Gtk.Widgets.ZoomWidget.Gtk.Container+ContainerChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			// Container child hbox1.Gtk.Box+BoxChild
			this.zoomOut_button = new global::Gtk.Button ();
			this.zoomOut_button.CanFocus = true;
			this.zoomOut_button.Name = "zoomOut_button";
			this.zoomOut_button.UseUnderline = true;
			this.zoomOut_button.Relief = ((global::Gtk.ReliefStyle)(2));
			// Container child zoomOut_button.Gtk.Container+ContainerChild
			global::Gtk.Alignment w1 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			global::Gtk.HBox w2 = new global::Gtk.HBox ();
			w2.Spacing = 2;
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Image w3 = new global::Gtk.Image ();
			w3.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-zoom-out", global::Gtk.IconSize.Menu);
			w2.Add (w3);
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Label w5 = new global::Gtk.Label ();
			w2.Add (w5);
			w1.Add (w2);
			this.zoomOut_button.Add (w1);
			this.hbox1.Add (this.zoomOut_button);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.zoomOut_button]));
			w9.Position = 0;
			w9.Expand = false;
			w9.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.zoom_label = new global::Gtk.Label ();
			this.zoom_label.Name = "zoom_label";
			this.zoom_label.LabelProp = global::Mono.Unix.Catalog.GetString ("100%");
			this.zoom_label.WidthChars = 5;
			this.hbox1.Add (this.zoom_label);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.zoom_label]));
			w10.Position = 1;
			w10.Expand = false;
			w10.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.zoom_hscale = new global::Gtk.HScale (null);
			this.zoom_hscale.WidthRequest = 70;
			this.zoom_hscale.CanFocus = true;
			this.zoom_hscale.Name = "zoom_hscale";
			this.zoom_hscale.Adjustment.Upper = 1;
			this.zoom_hscale.Adjustment.PageIncrement = 1;
			this.zoom_hscale.Adjustment.StepIncrement = 1;
			this.zoom_hscale.Adjustment.Value = 1;
			this.zoom_hscale.DrawValue = false;
			this.zoom_hscale.Digits = 0;
			this.zoom_hscale.ValuePos = ((global::Gtk.PositionType)(0));
			this.hbox1.Add (this.zoom_hscale);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.zoom_hscale]));
			w11.Position = 2;
			// Container child hbox1.Gtk.Box+BoxChild
			this.zoomIn_button = new global::Gtk.Button ();
			this.zoomIn_button.CanFocus = true;
			this.zoomIn_button.Name = "zoomIn_button";
			this.zoomIn_button.UseUnderline = true;
			this.zoomIn_button.Relief = ((global::Gtk.ReliefStyle)(2));
			// Container child zoomIn_button.Gtk.Container+ContainerChild
			global::Gtk.Alignment w12 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			global::Gtk.HBox w13 = new global::Gtk.HBox ();
			w13.Spacing = 2;
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Image w14 = new global::Gtk.Image ();
			w14.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-zoom-in", global::Gtk.IconSize.Menu);
			w13.Add (w14);
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Label w16 = new global::Gtk.Label ();
			w13.Add (w16);
			w12.Add (w13);
			this.zoomIn_button.Add (w12);
			this.hbox1.Add (this.zoomIn_button);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.zoomIn_button]));
			w20.Position = 3;
			w20.Expand = false;
			w20.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.zoom100_button = new global::Gtk.Button ();
			this.zoom100_button.CanFocus = true;
			this.zoom100_button.Name = "zoom100_button";
			this.zoom100_button.UseUnderline = true;
			this.zoom100_button.Relief = ((global::Gtk.ReliefStyle)(2));
			// Container child zoom100_button.Gtk.Container+ContainerChild
			global::Gtk.Alignment w21 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			global::Gtk.HBox w22 = new global::Gtk.HBox ();
			w22.Spacing = 2;
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Image w23 = new global::Gtk.Image ();
			w23.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-zoom-100", global::Gtk.IconSize.Menu);
			w22.Add (w23);
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Label w25 = new global::Gtk.Label ();
			w22.Add (w25);
			w21.Add (w22);
			this.zoom100_button.Add (w21);
			this.hbox1.Add (this.zoom100_button);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.zoom100_button]));
			w29.Position = 4;
			w29.Expand = false;
			w29.Fill = false;
			this.Add (this.hbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.zoomOut_button.Clicked += new global::System.EventHandler (this.OnZoomOutButtonClicked);
			this.zoom_hscale.ValueChanged += new global::System.EventHandler (this.OnZoomHscaleValueChanged);
			this.zoomIn_button.Clicked += new global::System.EventHandler (this.OnZoomInButtonClicked);
			this.zoom100_button.Clicked += new global::System.EventHandler (this.OnZoom100ButtonClicked);
		}
	}
}