
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;
	private global::Gtk.RadioAction ScaledRangeAction;
	private global::Gtk.Action FileAction;
	private global::Gtk.Action CloseAction;
	private global::Gtk.Action quitAction;
	private global::Gtk.Action renderToAction;
	private global::Gtk.Action loadRawAction;
	private global::Gtk.Action HelpAction;
	private global::Gtk.Action aboutAction;
	private global::Gtk.Action StageAction;
	private global::Gtk.Action ViewAction;
	private global::Gtk.ToggleAction UpdateDuringProcessingAction;
	private global::Gtk.Action loadStageAction;
	private global::Gtk.Action saveStageAction;
	private global::Gtk.Action saveStageAsAction;
	private global::Gtk.Action qweAction;
	private global::Gtk.Action zoomInAction;
	private global::Gtk.Action zoomOutAction;
	private global::Gtk.Action zoom100Action;
	private global::Gtk.ToggleAction RenderingQueueAction;
	private global::Gtk.VBox vbox2;
	private global::Gtk.MenuBar main_menubar;
	private global::Gtk.VBox vbox1;
	private global::Gtk.HPaned hpaned1;
	private global::Gtk.VBox left_vbox;
	private global::Gtk.HBox stageOperationAdding_hbox;
	private global::Gtk.ComboBox stageOperationToAdd_combobox;
	private global::Gtk.Button addStageOperation_button;
	private global::Gtk.ScrolledWindow GtkScrolledWindow1;
	private global::Gtk.VBox stage_vbox;
	private global::Gtk.VBox vbox3;
	private global::CatEye.UI.Gtk.Widgets.FloatPixmapViewWidget view_widget;
	private global::Gtk.HBox hbox1;
	private global::Gtk.Label status_label;
	private global::Gtk.ProgressBar progressbar;
	private global::Gtk.Button cancel_button;
	private global::CatEye.UI.Gtk.Widgets.ZoomWidget zoomwidget1;

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
		this.renderToAction = new global::Gtk.Action ("renderToAction", global::Mono.Unix.Catalog.GetString ("Render to..."), null, "gtk-execute");
		this.renderToAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Render to...");
		w1.Add (this.renderToAction, "<Control>r");
		this.loadRawAction = new global::Gtk.Action ("loadRawAction", global::Mono.Unix.Catalog.GetString ("Load raw image..."), null, "gtk-open");
		this.loadRawAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Import from DCRaw...");
		w1.Add (this.loadRawAction, "<Control>o");
		this.HelpAction = new global::Gtk.Action ("HelpAction", global::Mono.Unix.Catalog.GetString ("Help"), null, null);
		this.HelpAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Help");
		w1.Add (this.HelpAction, null);
		this.aboutAction = new global::Gtk.Action ("aboutAction", global::Mono.Unix.Catalog.GetString ("About CatEye..."), null, "gtk-about");
		this.aboutAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("About CatEye...");
		w1.Add (this.aboutAction, null);
		this.StageAction = new global::Gtk.Action ("StageAction", global::Mono.Unix.Catalog.GetString ("Stage"), null, null);
		this.StageAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Stage");
		w1.Add (this.StageAction, null);
		this.ViewAction = new global::Gtk.Action ("ViewAction", global::Mono.Unix.Catalog.GetString ("View"), null, null);
		this.ViewAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("View");
		w1.Add (this.ViewAction, null);
		this.UpdateDuringProcessingAction = new global::Gtk.ToggleAction ("UpdateDuringProcessingAction", global::Mono.Unix.Catalog.GetString ("Update during processing"), null, null);
		this.UpdateDuringProcessingAction.Active = true;
		this.UpdateDuringProcessingAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Update during processing");
		w1.Add (this.UpdateDuringProcessingAction, null);
		this.loadStageAction = new global::Gtk.Action ("loadStageAction", global::Mono.Unix.Catalog.GetString ("Load stage..."), null, "gtk-open");
		this.loadStageAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Load stage...");
		w1.Add (this.loadStageAction, "<Control>o");
		this.saveStageAction = new global::Gtk.Action ("saveStageAction", global::Mono.Unix.Catalog.GetString ("Save stage as..."), null, "gtk-save-as");
		this.saveStageAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Save stage as...");
		w1.Add (this.saveStageAction, "<Control>s");
		this.saveStageAsAction = new global::Gtk.Action ("saveStageAsAction", global::Mono.Unix.Catalog.GetString ("Save stage as..."), null, "gtk-save-as");
		this.saveStageAsAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Save stage as...");
		w1.Add (this.saveStageAsAction, null);
		this.qweAction = new global::Gtk.Action ("qweAction", global::Mono.Unix.Catalog.GetString ("qwe"), null, null);
		this.qweAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("qwe");
		w1.Add (this.qweAction, null);
		this.zoomInAction = new global::Gtk.Action ("zoomInAction", null, null, "gtk-zoom-in");
		w1.Add (this.zoomInAction, null);
		this.zoomOutAction = new global::Gtk.Action ("zoomOutAction", null, null, "gtk-zoom-out");
		w1.Add (this.zoomOutAction, null);
		this.zoom100Action = new global::Gtk.Action ("zoom100Action", null, null, "gtk-zoom-100");
		w1.Add (this.zoom100Action, null);
		this.RenderingQueueAction = new global::Gtk.ToggleAction ("RenderingQueueAction", global::Mono.Unix.Catalog.GetString ("Rendering queue"), null, null);
		this.RenderingQueueAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Rendering queue");
		w1.Add (this.RenderingQueueAction, null);
		this.UIManager.InsertActionGroup (w1, 0);
		this.AddAccelGroup (this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("CatEye");
		this.Icon = global::Gdk.Pixbuf.LoadFromResource ("CatEye.UI.Gtk.res.cateye-small.png");
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox2 = new global::Gtk.VBox ();
		this.vbox2.Name = "vbox2";
		// Container child vbox2.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString ("<ui><menubar name='main_menubar'><menu name='FileAction' action='FileAction'><menuitem name='loadStageAction' action='loadStageAction'/><menuitem name='saveStageAsAction' action='saveStageAsAction'/><separator/><menuitem name='loadRawAction' action='loadRawAction'/><menuitem name='renderToAction' action='renderToAction'/><separator/><menuitem name='quitAction' action='quitAction'/></menu><menu name='ViewAction' action='ViewAction'><menuitem name='UpdateDuringProcessingAction' action='UpdateDuringProcessingAction'/><separator/><menuitem name='RenderingQueueAction' action='RenderingQueueAction'/></menu><menu name='HelpAction' action='HelpAction'><menuitem name='aboutAction' action='aboutAction'/></menu></menubar></ui>");
		this.main_menubar = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/main_menubar")));
		this.main_menubar.Name = "main_menubar";
		this.vbox2.Add (this.main_menubar);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.main_menubar]));
		w2.Position = 0;
		w2.Expand = false;
		w2.Fill = false;
		// Container child vbox2.Gtk.Box+BoxChild
		this.vbox1 = new global::Gtk.VBox ();
		this.vbox1.Name = "vbox1";
		this.vbox1.Spacing = 4;
		// Container child vbox1.Gtk.Box+BoxChild
		this.hpaned1 = new global::Gtk.HPaned ();
		this.hpaned1.CanFocus = true;
		this.hpaned1.Name = "hpaned1";
		this.hpaned1.Position = 237;
		this.hpaned1.BorderWidth = ((uint)(1));
		// Container child hpaned1.Gtk.Paned+PanedChild
		this.left_vbox = new global::Gtk.VBox ();
		this.left_vbox.Name = "left_vbox";
		this.left_vbox.Spacing = 4;
		this.left_vbox.BorderWidth = ((uint)(2));
		// Container child left_vbox.Gtk.Box+BoxChild
		this.stageOperationAdding_hbox = new global::Gtk.HBox ();
		this.stageOperationAdding_hbox.Name = "stageOperationAdding_hbox";
		this.stageOperationAdding_hbox.Spacing = 6;
		// Container child stageOperationAdding_hbox.Gtk.Box+BoxChild
		this.stageOperationToAdd_combobox = global::Gtk.ComboBox.NewText ();
		this.stageOperationToAdd_combobox.Name = "stageOperationToAdd_combobox";
		this.stageOperationAdding_hbox.Add (this.stageOperationToAdd_combobox);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.stageOperationAdding_hbox [this.stageOperationToAdd_combobox]));
		w3.Position = 0;
		// Container child stageOperationAdding_hbox.Gtk.Box+BoxChild
		this.addStageOperation_button = new global::Gtk.Button ();
		this.addStageOperation_button.CanFocus = true;
		this.addStageOperation_button.Name = "addStageOperation_button";
		this.addStageOperation_button.UseUnderline = true;
		// Container child addStageOperation_button.Gtk.Container+ContainerChild
		global::Gtk.Alignment w4 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w5 = new global::Gtk.HBox ();
		w5.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w6 = new global::Gtk.Image ();
		w6.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-add", global::Gtk.IconSize.Menu);
		w5.Add (w6);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w8 = new global::Gtk.Label ();
		w8.LabelProp = global::Mono.Unix.Catalog.GetString ("_Add");
		w8.UseUnderline = true;
		w5.Add (w8);
		w4.Add (w5);
		this.addStageOperation_button.Add (w4);
		this.stageOperationAdding_hbox.Add (this.addStageOperation_button);
		global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.stageOperationAdding_hbox [this.addStageOperation_button]));
		w12.Position = 1;
		w12.Expand = false;
		w12.Fill = false;
		this.left_vbox.Add (this.stageOperationAdding_hbox);
		global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.left_vbox [this.stageOperationAdding_hbox]));
		w13.Position = 0;
		w13.Expand = false;
		w13.Fill = false;
		// Container child left_vbox.Gtk.Box+BoxChild
		this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
		this.GtkScrolledWindow1.HscrollbarPolicy = ((global::Gtk.PolicyType)(2));
		// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
		global::Gtk.Viewport w14 = new global::Gtk.Viewport ();
		w14.ShadowType = ((global::Gtk.ShadowType)(0));
		// Container child GtkViewport2.Gtk.Container+ContainerChild
		this.stage_vbox = new global::Gtk.VBox ();
		this.stage_vbox.Name = "stage_vbox";
		this.stage_vbox.Spacing = 3;
		this.stage_vbox.BorderWidth = ((uint)(6));
		w14.Add (this.stage_vbox);
		this.GtkScrolledWindow1.Add (w14);
		this.left_vbox.Add (this.GtkScrolledWindow1);
		global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.left_vbox [this.GtkScrolledWindow1]));
		w17.Position = 1;
		this.hpaned1.Add (this.left_vbox);
		global::Gtk.Paned.PanedChild w18 = ((global::Gtk.Paned.PanedChild)(this.hpaned1 [this.left_vbox]));
		w18.Resize = false;
		w18.Shrink = false;
		// Container child hpaned1.Gtk.Paned+PanedChild
		this.vbox3 = new global::Gtk.VBox ();
		this.vbox3.Name = "vbox3";
		// Container child vbox3.Gtk.Box+BoxChild
		this.view_widget = new global::CatEye.UI.Gtk.Widgets.FloatPixmapViewWidget ();
		this.view_widget.Events = ((global::Gdk.EventMask)(1022));
		this.view_widget.ExtensionEvents = ((global::Gdk.ExtensionMode)(1));
		this.view_widget.Name = "view_widget";
		this.view_widget.InstantUpdate = false;
		this.vbox3.Add (this.view_widget);
		global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.view_widget]));
		w19.Position = 0;
		// Container child vbox3.Gtk.Box+BoxChild
		this.hbox1 = new global::Gtk.HBox ();
		this.hbox1.Name = "hbox1";
		this.hbox1.Spacing = 6;
		this.hbox1.BorderWidth = ((uint)(2));
		// Container child hbox1.Gtk.Box+BoxChild
		this.status_label = new global::Gtk.Label ();
		this.status_label.Name = "status_label";
		this.status_label.Xalign = 0F;
		this.status_label.LabelProp = global::Mono.Unix.Catalog.GetString ("Parameters can be changed <b>during</b> processing. You are welcome!");
		this.status_label.UseMarkup = true;
		this.status_label.Ellipsize = ((global::Pango.EllipsizeMode)(3));
		this.hbox1.Add (this.status_label);
		global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.status_label]));
		w20.Position = 0;
		// Container child hbox1.Gtk.Box+BoxChild
		this.progressbar = new global::Gtk.ProgressBar ();
		this.progressbar.Name = "progressbar";
		this.hbox1.Add (this.progressbar);
		global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.progressbar]));
		w21.Position = 1;
		w21.Expand = false;
		// Container child hbox1.Gtk.Box+BoxChild
		this.cancel_button = new global::Gtk.Button ();
		this.cancel_button.Sensitive = false;
		this.cancel_button.CanFocus = true;
		this.cancel_button.Name = "cancel_button";
		this.cancel_button.UseUnderline = true;
		// Container child cancel_button.Gtk.Container+ContainerChild
		global::Gtk.Alignment w22 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w23 = new global::Gtk.HBox ();
		w23.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w24 = new global::Gtk.Image ();
		w24.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-cancel", global::Gtk.IconSize.Menu);
		w23.Add (w24);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w26 = new global::Gtk.Label ();
		w26.LabelProp = global::Mono.Unix.Catalog.GetString ("_Cancel");
		w26.UseUnderline = true;
		w23.Add (w26);
		w22.Add (w23);
		this.cancel_button.Add (w22);
		this.hbox1.Add (this.cancel_button);
		global::Gtk.Box.BoxChild w30 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.cancel_button]));
		w30.Position = 2;
		w30.Expand = false;
		w30.Fill = false;
		// Container child hbox1.Gtk.Box+BoxChild
		this.zoomwidget1 = new global::CatEye.UI.Gtk.Widgets.ZoomWidget ();
		this.zoomwidget1.Events = ((global::Gdk.EventMask)(256));
		this.zoomwidget1.Name = "zoomwidget1";
		this.zoomwidget1.Value = 0.25;
		this.hbox1.Add (this.zoomwidget1);
		global::Gtk.Box.BoxChild w31 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.zoomwidget1]));
		w31.Position = 3;
		w31.Expand = false;
		this.vbox3.Add (this.hbox1);
		global::Gtk.Box.BoxChild w32 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox1]));
		w32.Position = 1;
		w32.Expand = false;
		w32.Fill = false;
		this.hpaned1.Add (this.vbox3);
		this.vbox1.Add (this.hpaned1);
		global::Gtk.Box.BoxChild w34 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hpaned1]));
		w34.Position = 0;
		this.vbox2.Add (this.vbox1);
		global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.vbox1]));
		w35.Position = 1;
		this.Add (this.vbox2);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 1021;
		this.DefaultHeight = 668;
		this.progressbar.Hide ();
		this.cancel_button.Hide ();
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.quitAction.Activated += new global::System.EventHandler (this.OnQuitActionActivated);
		this.renderToAction.Activated += new global::System.EventHandler (this.OnRenderToActionActivated);
		this.loadRawAction.Activated += new global::System.EventHandler (this.OnImportFromDCRawActionActivated);
		this.aboutAction.Activated += new global::System.EventHandler (this.OnAboutActionActivated);
		this.ViewAction.Activated += new global::System.EventHandler (this.OnViewActionActivated);
		this.UpdateDuringProcessingAction.Toggled += new global::System.EventHandler (this.OnUpdateDuringProcessingActionToggled);
		this.loadStageAction.Activated += new global::System.EventHandler (this.OnLoadStageActionActivated);
		this.saveStageAsAction.Activated += new global::System.EventHandler (this.OnSaveStageAsActionActivated);
		this.RenderingQueueAction.Toggled += new global::System.EventHandler (this.OnRenderingQueueActionToggled);
		this.addStageOperation_button.Clicked += new global::System.EventHandler (this.OnAddStageOperationButtonClicked);
		this.stage_vbox.ExposeEvent += new global::Gtk.ExposeEventHandler (this.OnStageVboxExposeEvent);
		this.stage_vbox.SizeAllocated += new global::Gtk.SizeAllocatedHandler (this.OnStageVboxSizeAllocated);
		this.cancel_button.Clicked += new global::System.EventHandler (this.OnCancelButtonClicked);
	}
}
