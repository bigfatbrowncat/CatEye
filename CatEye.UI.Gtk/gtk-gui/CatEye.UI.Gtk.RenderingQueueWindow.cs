
// This file has been generated by the GUI designer. Do not modify.
namespace CatEye.UI.Gtk
{
	public partial class RenderingQueueWindow
	{
		private global::Gtk.VBox vbox2;
		private global::Gtk.HBox hbox2;
		private global::Gtk.Frame frame1;
		private global::Gtk.Alignment GtkAlignment4;
		private global::Gtk.VBox vbox3;
		private global::Gtk.Table table1;
		private global::Gtk.Label destination_label;
		private global::Gtk.Label label2;
		private global::Gtk.Label label3;
		private global::Gtk.Label source_label;
		private global::Gtk.HBox hbox3;
		private global::Gtk.ProgressBar processing_progressbar;
		private global::Gtk.Button cancel_button;
		private global::Gtk.Label titleGtkLabel;
		private global::Gtk.Expander expander1;
		private global::Gtk.HBox hbox1;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.NodeView queue_nodeview;
		private global::Gtk.VBox vbox4;
		private global::Gtk.Button up_button;
		private global::Gtk.Button down_button;
		private global::Gtk.Button remove_button;
		private global::Gtk.Button cancelAll_button;
		private global::Gtk.Label queue_GtkLabel;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CatEye.UI.Gtk.RenderingQueueWindow
			this.Name = "CatEye.UI.Gtk.RenderingQueueWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("Rendering queue");
			this.Icon = global::Gdk.Pixbuf.LoadFromResource ("CatEye.UI.Gtk.res.cateye-small.png");
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			this.AllowGrow = false;
			// Container child CatEye.UI.Gtk.RenderingQueueWindow.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 2;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame ();
			this.frame1.Name = "frame1";
			this.frame1.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment4 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment4.Name = "GtkAlignment4";
			// Container child GtkAlignment4.Gtk.Container+ContainerChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			this.vbox3.BorderWidth = ((uint)(4));
			// Container child vbox3.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table (((uint)(2)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.destination_label = new global::Gtk.Label ();
			this.destination_label.Name = "destination_label";
			this.destination_label.Xalign = 0F;
			this.destination_label.Ellipsize = ((global::Pango.EllipsizeMode)(2));
			this.destination_label.MaxWidthChars = 60;
			this.destination_label.SingleLineMode = true;
			this.table1.Add (this.destination_label);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1 [this.destination_label]));
			w1.TopAttach = ((uint)(1));
			w1.BottomAttach = ((uint)(2));
			w1.LeftAttach = ((uint)(1));
			w1.RightAttach = ((uint)(2));
			w1.XOptions = ((global::Gtk.AttachOptions)(4));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 1F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Source:</b>");
			this.label2.UseMarkup = true;
			this.table1.Add (this.label2);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this.label2]));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Destination:</b>");
			this.label3.UseMarkup = true;
			this.table1.Add (this.label3);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.label3]));
			w3.TopAttach = ((uint)(1));
			w3.BottomAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.source_label = new global::Gtk.Label ();
			this.source_label.Name = "source_label";
			this.source_label.Xalign = 0F;
			this.source_label.Ellipsize = ((global::Pango.EllipsizeMode)(2));
			this.source_label.MaxWidthChars = 60;
			this.source_label.SingleLineMode = true;
			this.table1.Add (this.source_label);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1 [this.source_label]));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox3.Add (this.table1);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.table1]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			w5.Padding = ((uint)(6));
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.processing_progressbar = new global::Gtk.ProgressBar ();
			this.processing_progressbar.Name = "processing_progressbar";
			this.hbox3.Add (this.processing_progressbar);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.processing_progressbar]));
			w6.Position = 0;
			// Container child hbox3.Gtk.Box+BoxChild
			this.cancel_button = new global::Gtk.Button ();
			this.cancel_button.CanFocus = true;
			this.cancel_button.Name = "cancel_button";
			this.cancel_button.UseStock = true;
			this.cancel_button.UseUnderline = true;
			this.cancel_button.Label = "gtk-cancel";
			this.hbox3.Add (this.cancel_button);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.cancel_button]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			this.vbox3.Add (this.hbox3);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox3]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			this.GtkAlignment4.Add (this.vbox3);
			this.frame1.Add (this.GtkAlignment4);
			this.titleGtkLabel = new global::Gtk.Label ();
			this.titleGtkLabel.HeightRequest = 28;
			this.titleGtkLabel.Name = "titleGtkLabel";
			this.titleGtkLabel.Xalign = 0F;
			this.titleGtkLabel.Yalign = 1F;
			this.titleGtkLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Currently processing</b>");
			this.titleGtkLabel.UseMarkup = true;
			this.frame1.LabelWidget = this.titleGtkLabel;
			this.hbox2.Add (this.frame1);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.frame1]));
			w11.Position = 0;
			w11.Padding = ((uint)(6));
			this.vbox2.Add (this.hbox2);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox2]));
			w12.Position = 0;
			w12.Expand = false;
			w12.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.expander1 = new global::Gtk.Expander (null);
			this.expander1.CanFocus = true;
			this.expander1.Name = "expander1";
			this.expander1.Expanded = true;
			this.expander1.BorderWidth = ((uint)(9));
			// Container child expander1.Gtk.Container+ContainerChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			this.hbox1.BorderWidth = ((uint)(3));
			// Container child hbox1.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.queue_nodeview = new global::Gtk.NodeView ();
			this.queue_nodeview.CanFocus = true;
			this.queue_nodeview.Name = "queue_nodeview";
			this.GtkScrolledWindow.Add (this.queue_nodeview);
			this.hbox1.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.GtkScrolledWindow]));
			w14.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.up_button = new global::Gtk.Button ();
			this.up_button.Sensitive = false;
			this.up_button.CanFocus = true;
			this.up_button.Name = "up_button";
			this.up_button.UseStock = true;
			this.up_button.UseUnderline = true;
			this.up_button.Label = "gtk-go-up";
			this.vbox4.Add (this.up_button);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.up_button]));
			w15.Position = 0;
			w15.Expand = false;
			w15.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.down_button = new global::Gtk.Button ();
			this.down_button.Sensitive = false;
			this.down_button.CanFocus = true;
			this.down_button.Name = "down_button";
			this.down_button.UseStock = true;
			this.down_button.UseUnderline = true;
			this.down_button.Label = "gtk-go-down";
			this.vbox4.Add (this.down_button);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.down_button]));
			w16.Position = 1;
			w16.Expand = false;
			w16.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.remove_button = new global::Gtk.Button ();
			this.remove_button.CanFocus = true;
			this.remove_button.Name = "remove_button";
			this.remove_button.UseStock = true;
			this.remove_button.UseUnderline = true;
			this.remove_button.Label = "gtk-remove";
			this.vbox4.Add (this.remove_button);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.remove_button]));
			w17.Position = 2;
			w17.Expand = false;
			w17.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.cancelAll_button = new global::Gtk.Button ();
			this.cancelAll_button.CanFocus = true;
			this.cancelAll_button.Name = "cancelAll_button";
			this.cancelAll_button.UseUnderline = true;
			// Container child cancelAll_button.Gtk.Container+ContainerChild
			global::Gtk.Alignment w18 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			global::Gtk.HBox w19 = new global::Gtk.HBox ();
			w19.Spacing = 2;
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Image w20 = new global::Gtk.Image ();
			w20.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-cancel", global::Gtk.IconSize.Button);
			w19.Add (w20);
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Label w22 = new global::Gtk.Label ();
			w22.LabelProp = global::Mono.Unix.Catalog.GetString ("_Cancel all");
			w22.UseUnderline = true;
			w19.Add (w22);
			w18.Add (w19);
			this.cancelAll_button.Add (w18);
			this.vbox4.Add (this.cancelAll_button);
			global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.cancelAll_button]));
			w26.Position = 3;
			w26.Expand = false;
			w26.Fill = false;
			w26.Padding = ((uint)(8));
			this.hbox1.Add (this.vbox4);
			global::Gtk.Box.BoxChild w27 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vbox4]));
			w27.Position = 1;
			w27.Expand = false;
			w27.Fill = false;
			this.expander1.Add (this.hbox1);
			this.queue_GtkLabel = new global::Gtk.Label ();
			this.queue_GtkLabel.Name = "queue_GtkLabel";
			this.queue_GtkLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Tasks queue</b>");
			this.queue_GtkLabel.UseMarkup = true;
			this.queue_GtkLabel.UseUnderline = true;
			this.expander1.LabelWidget = this.queue_GtkLabel;
			this.vbox2.Add (this.expander1);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.expander1]));
			w29.Position = 1;
			this.Add (this.vbox2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 432;
			this.DefaultHeight = 344;
			this.Hide ();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
			this.Shown += new global::System.EventHandler (this.OnShown);
			this.cancel_button.Clicked += new global::System.EventHandler (this.OnCancelButtonClicked);
			this.expander1.Activated += new global::System.EventHandler (this.OnExpander1Activated);
			this.queue_nodeview.CursorChanged += new global::System.EventHandler (this.OnQueueNodeviewCursorChanged);
			this.up_button.Clicked += new global::System.EventHandler (this.OnUpButtonClicked);
			this.down_button.Clicked += new global::System.EventHandler (this.OnDownButtonClicked);
			this.remove_button.Clicked += new global::System.EventHandler (this.OnRemoveButtonClicked);
			this.cancelAll_button.Clicked += new global::System.EventHandler (this.OnCancelAllButtonClicked);
		}
	}
}
