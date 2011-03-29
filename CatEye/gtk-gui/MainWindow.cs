
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;

	private global::Gtk.RadioAction ScaledRangeAction;

	private global::Gtk.Action FileAction;

	private global::Gtk.Action CloseAction;

	private global::Gtk.Action quitAction;

	private global::Gtk.Action saveAsAction;

	private global::Gtk.Action loadAction;

	private global::Gtk.Action HelpAction;

	private global::Gtk.Action aboutAction;

	private global::Gtk.Action StageAction;

	private global::Gtk.VBox vbox2;

	private global::Gtk.MenuBar main_menubar;

	private global::Gtk.VBox vbox1;

	private global::Gtk.HBox main_hbox;

	private global::Gtk.VBox left_vbox;

	private global::Gtk.ScrolledWindow GtkScrolledWindow1;

	private global::Gtk.VBox stage_vbox;

	private global::Gtk.HBox status_bar_hbox;

	private global::Gtk.ProgressBar progressbar;

	private global::Gtk.Button cancel_button;

	private global::Gtk.VBox vbox3;

	private global::Gtk.ScrolledWindow scrolledwindow3;

	private global::CatEye.DoublePixmapViewWidget ppmviewwidget1;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.UIManager = new global::Gtk.UIManager ();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
		this.ScaledRangeAction = new global::Gtk.RadioAction ("ScaledRangeAction", global::Mono.Unix.Catalog.GetString ("Scaled Range"), null, null, 1);
		this.ScaledRangeAction.Group = new global::GLib.SList (global::System.IntPtr.Zero);
		this.ScaledRangeAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Scaled Range");
		w1.Add (this.ScaledRangeAction, null);
		this.FileAction = new global::Gtk.Action ("FileAction", global::Mono.Unix.Catalog.GetString ("File"), null, null);
		this.FileAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
		w1.Add (this.FileAction, null);
		this.CloseAction = new global::Gtk.Action ("CloseAction", global::Mono.Unix.Catalog.GetString ("Close"), null, null);
		this.CloseAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Close");
		w1.Add (this.CloseAction, null);
		this.quitAction = new global::Gtk.Action ("quitAction", global::Mono.Unix.Catalog.GetString ("Quit"), null, "gtk-quit");
		this.quitAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Quit");
		w1.Add (this.quitAction, "<Control>q");
		this.saveAsAction = new global::Gtk.Action ("saveAsAction", global::Mono.Unix.Catalog.GetString ("Save image as..."), null, "gtk-save-as");
		this.saveAsAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Save as...");
		w1.Add (this.saveAsAction, "<Control>s");
		this.loadAction = new global::Gtk.Action ("loadAction", global::Mono.Unix.Catalog.GetString ("Load raw image..."), null, "gtk-open");
		this.loadAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Import from DCRaw...");
		w1.Add (this.loadAction, "<Control>o");
		this.HelpAction = new global::Gtk.Action ("HelpAction", global::Mono.Unix.Catalog.GetString ("Help"), null, null);
		this.HelpAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Help");
		w1.Add (this.HelpAction, null);
		this.aboutAction = new global::Gtk.Action ("aboutAction", global::Mono.Unix.Catalog.GetString ("About CatEye..."), null, "gtk-about");
		this.aboutAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("About CatEye...");
		w1.Add (this.aboutAction, null);
		this.StageAction = new global::Gtk.Action ("StageAction", global::Mono.Unix.Catalog.GetString ("Stage"), null, null);
		this.StageAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Stage");
		w1.Add (this.StageAction, null);
		this.UIManager.InsertActionGroup (w1, 0);
		this.AddAccelGroup (this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("CatEye 0.3");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox2 = new global::Gtk.VBox ();
		this.vbox2.Name = "vbox2";
		// Container child vbox2.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString ("<ui><menubar name='main_menubar'><menu name='FileAction' action='FileAction'><menuitem name='loadAction' action='loadAction'/><menuitem name='saveAsAction' action='saveAsAction'/><separator/><menuitem name='quitAction' action='quitAction'/></menu><menu name='HelpAction' action='HelpAction'><menuitem name='aboutAction' action='aboutAction'/></menu></menubar></ui>");
		this.main_menubar = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/main_menubar")));
		this.main_menubar.Name = "main_menubar";
		this.vbox2.Add (this.main_menubar);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.main_menubar]));
		w2.Position = 0;
		w2.Expand = false;
		w2.Fill = false;
		// Container child vbox2.Gtk.Box+BoxChild
		this.vbox1 = new global::Gtk.VBox ();
		this.vbox1.Name = "vbox1";
		this.vbox1.Spacing = 4;
		// Container child vbox1.Gtk.Box+BoxChild
		this.main_hbox = new global::Gtk.HBox ();
		this.main_hbox.Name = "main_hbox";
		this.main_hbox.Spacing = 1;
		// Container child main_hbox.Gtk.Box+BoxChild
		this.left_vbox = new global::Gtk.VBox ();
		this.left_vbox.Name = "left_vbox";
		this.left_vbox.Spacing = 5;
		this.left_vbox.BorderWidth = ((uint)(3));
		// Container child left_vbox.Gtk.Box+BoxChild
		this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
		this.GtkScrolledWindow1.HscrollbarPolicy = ((global::Gtk.PolicyType)(2));
		// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
		global::Gtk.Viewport w3 = new global::Gtk.Viewport ();
		w3.ShadowType = ((global::Gtk.ShadowType)(0));
		// Container child GtkViewport2.Gtk.Container+ContainerChild
		this.stage_vbox = new global::Gtk.VBox ();
		this.stage_vbox.Name = "stage_vbox";
		this.stage_vbox.Spacing = 6;
		this.stage_vbox.BorderWidth = ((uint)(3));
		w3.Add (this.stage_vbox);
		this.GtkScrolledWindow1.Add (w3);
		this.left_vbox.Add (this.GtkScrolledWindow1);
		global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.left_vbox[this.GtkScrolledWindow1]));
		w6.Position = 0;
		// Container child left_vbox.Gtk.Box+BoxChild
		this.status_bar_hbox = new global::Gtk.HBox ();
		this.status_bar_hbox.Name = "status_bar_hbox";
		this.status_bar_hbox.Spacing = 6;
		// Container child status_bar_hbox.Gtk.Box+BoxChild
		this.progressbar = new global::Gtk.ProgressBar ();
		this.progressbar.Name = "progressbar";
		this.status_bar_hbox.Add (this.progressbar);
		global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.status_bar_hbox[this.progressbar]));
		w7.Position = 0;
		// Container child status_bar_hbox.Gtk.Box+BoxChild
		this.cancel_button = new global::Gtk.Button ();
		this.cancel_button.Sensitive = false;
		this.cancel_button.CanFocus = true;
		this.cancel_button.Name = "cancel_button";
		this.cancel_button.UseUnderline = true;
		// Container child cancel_button.Gtk.Container+ContainerChild
		global::Gtk.Alignment w8 = new global::Gtk.Alignment (0.5f, 0.5f, 0f, 0f);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w9 = new global::Gtk.HBox ();
		w9.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w10 = new global::Gtk.Image ();
		w10.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-cancel", global::Gtk.IconSize.SmallToolbar);
		w9.Add (w10);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w12 = new global::Gtk.Label ();
		w12.LabelProp = global::Mono.Unix.Catalog.GetString ("_Cancel");
		w12.UseUnderline = true;
		w9.Add (w12);
		w8.Add (w9);
		this.cancel_button.Add (w8);
		this.status_bar_hbox.Add (this.cancel_button);
		global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.status_bar_hbox[this.cancel_button]));
		w16.Position = 1;
		w16.Expand = false;
		w16.Fill = false;
		this.left_vbox.Add (this.status_bar_hbox);
		global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.left_vbox[this.status_bar_hbox]));
		w17.Position = 1;
		w17.Expand = false;
		w17.Fill = false;
		this.main_hbox.Add (this.left_vbox);
		global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.main_hbox[this.left_vbox]));
		w18.Position = 0;
		w18.Expand = false;
		// Container child main_hbox.Gtk.Box+BoxChild
		this.vbox3 = new global::Gtk.VBox ();
		this.vbox3.Name = "vbox3";
		this.vbox3.Spacing = 6;
		// Container child vbox3.Gtk.Box+BoxChild
		this.scrolledwindow3 = new global::Gtk.ScrolledWindow ();
		this.scrolledwindow3.CanFocus = true;
		this.scrolledwindow3.Name = "scrolledwindow3";
		this.scrolledwindow3.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child scrolledwindow3.Gtk.Container+ContainerChild
		global::Gtk.Viewport w19 = new global::Gtk.Viewport ();
		w19.ShadowType = ((global::Gtk.ShadowType)(0));
		// Container child GtkViewport.Gtk.Container+ContainerChild
		this.ppmviewwidget1 = new global::CatEye.DoublePixmapViewWidget ();
		this.ppmviewwidget1.Events = ((global::Gdk.EventMask)(256));
		this.ppmviewwidget1.Name = "ppmviewwidget1";
		w19.Add (this.ppmviewwidget1);
		this.scrolledwindow3.Add (w19);
		this.vbox3.Add (this.scrolledwindow3);
		global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.vbox3[this.scrolledwindow3]));
		w22.Position = 0;
		this.main_hbox.Add (this.vbox3);
		global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.main_hbox[this.vbox3]));
		w23.Position = 1;
		this.vbox1.Add (this.main_hbox);
		global::Gtk.Box.BoxChild w24 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.main_hbox]));
		w24.Position = 0;
		this.vbox2.Add (this.vbox1);
		global::Gtk.Box.BoxChild w25 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.vbox1]));
		w25.Position = 1;
		this.Add (this.vbox2);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 879;
		this.DefaultHeight = 500;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.quitAction.Activated += new global::System.EventHandler (this.OnQuitActionActivated);
		this.saveAsAction.Activated += new global::System.EventHandler (this.OnSaveAsActionActivated);
		this.loadAction.Activated += new global::System.EventHandler (this.OnImportFromDCRawActionActivated);
		this.aboutAction.Activated += new global::System.EventHandler (this.OnAboutActionActivated);
		this.cancel_button.Clicked += new global::System.EventHandler (this.OnCancelButtonClicked);
	}
}
