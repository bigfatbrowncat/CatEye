// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------



public partial class MainWindow {
    
    private Gtk.UIManager UIManager;
    
    private Gtk.RadioAction ScaledRangeAction;
    
    private Gtk.Action FileAction;
    
    private Gtk.Action openAction;
    
    private Gtk.Action Action;
    
    private Gtk.Action CloseAction;
    
    private Gtk.Action quitAction;
    
    private Gtk.Action saveAsAction;
    
    private Gtk.Action ImportFromDCRawAction;
    
    private Gtk.Action HelpAction;
    
    private Gtk.Action AboutCatEyeAction;
    
    private Gtk.VBox vbox2;
    
    private Gtk.MenuBar menubar1;
    
    private Gtk.VBox vbox1;
    
    private Gtk.HBox hbox5;
    
    private Gtk.VBox left_vbox;
    
    private Gtk.Label label5;
    
    private Gtk.Table table4;
    
    private Gtk.Label label1;
    
    private Gtk.ComboBox prescale_combobox;
    
    private Gtk.Label label15;
    
    private Gtk.ScrolledWindow GtkScrolledWindow1;
    
    private Gtk.VBox stage_vbox;
    
    private Gtk.HBox hbox9;
    
    private Gtk.Button apply_stage2_button;
    
    private Gtk.VBox vbox3;
    
    private Gtk.ScrolledWindow scrolledwindow3;
    
    private CatEye.DoublePixmapViewWidget ppmviewwidget1;
    
    private Gtk.HBox status_bar_hbox;
    
    private Gtk.ProgressBar progressbar;
    
    private Gtk.Button cancel_button;
    
    protected virtual void Build() {
        Stetic.Gui.Initialize(this);
        // Widget MainWindow
        this.UIManager = new Gtk.UIManager();
        Gtk.ActionGroup w1 = new Gtk.ActionGroup("Default");
        this.ScaledRangeAction = new Gtk.RadioAction("ScaledRangeAction", Mono.Unix.Catalog.GetString("Scaled Range"), null, null, 1);
        this.ScaledRangeAction.Group = new GLib.SList(System.IntPtr.Zero);
        this.ScaledRangeAction.ShortLabel = Mono.Unix.Catalog.GetString("Scaled Range");
        w1.Add(this.ScaledRangeAction, null);
        this.FileAction = new Gtk.Action("FileAction", Mono.Unix.Catalog.GetString("File"), null, null);
        this.FileAction.ShortLabel = Mono.Unix.Catalog.GetString("File");
        w1.Add(this.FileAction, null);
        this.openAction = new Gtk.Action("openAction", Mono.Unix.Catalog.GetString("Open picture..."), null, "gtk-open");
        this.openAction.ShortLabel = Mono.Unix.Catalog.GetString("Open...");
        w1.Add(this.openAction, null);
        this.Action = new Gtk.Action("Action", Mono.Unix.Catalog.GetString("-"), null, null);
        this.Action.ShortLabel = Mono.Unix.Catalog.GetString("-");
        w1.Add(this.Action, null);
        this.CloseAction = new Gtk.Action("CloseAction", Mono.Unix.Catalog.GetString("Close"), null, null);
        this.CloseAction.ShortLabel = Mono.Unix.Catalog.GetString("Close");
        w1.Add(this.CloseAction, null);
        this.quitAction = new Gtk.Action("quitAction", Mono.Unix.Catalog.GetString("Quit"), null, "gtk-quit");
        this.quitAction.ShortLabel = Mono.Unix.Catalog.GetString("Quit");
        w1.Add(this.quitAction, null);
        this.saveAsAction = new Gtk.Action("saveAsAction", Mono.Unix.Catalog.GetString("Save picture as..."), null, "gtk-save-as");
        this.saveAsAction.ShortLabel = Mono.Unix.Catalog.GetString("Save as...");
        w1.Add(this.saveAsAction, null);
        this.ImportFromDCRawAction = new Gtk.Action("ImportFromDCRawAction", Mono.Unix.Catalog.GetString("Import from DCRaw..."), null, null);
        this.ImportFromDCRawAction.ShortLabel = Mono.Unix.Catalog.GetString("Import from DCRaw...");
        w1.Add(this.ImportFromDCRawAction, null);
        this.HelpAction = new Gtk.Action("HelpAction", Mono.Unix.Catalog.GetString("Help"), null, null);
        this.HelpAction.ShortLabel = Mono.Unix.Catalog.GetString("Help");
        w1.Add(this.HelpAction, null);
        this.AboutCatEyeAction = new Gtk.Action("AboutCatEyeAction", Mono.Unix.Catalog.GetString("About CatEye..."), null, null);
        this.AboutCatEyeAction.ShortLabel = Mono.Unix.Catalog.GetString("About CatEye...");
        w1.Add(this.AboutCatEyeAction, null);
        this.UIManager.InsertActionGroup(w1, 0);
        this.AddAccelGroup(this.UIManager.AccelGroup);
        this.Name = "MainWindow";
        this.Title = Mono.Unix.Catalog.GetString("CatEye 0.1");
        this.WindowPosition = ((Gtk.WindowPosition)(4));
        // Container child MainWindow.Gtk.Container+ContainerChild
        this.vbox2 = new Gtk.VBox();
        this.vbox2.Name = "vbox2";
        // Container child vbox2.Gtk.Box+BoxChild
        this.UIManager.AddUiFromString("<ui><menubar name='menubar1'><menu name='FileAction' action='FileAction'><menuitem name='openAction' action='openAction'/><menuitem name='ImportFromDCRawAction' action='ImportFromDCRawAction'/><separator/><menuitem name='saveAsAction' action='saveAsAction'/><separator/><menuitem name='quitAction' action='quitAction'/></menu><menu name='HelpAction' action='HelpAction'><menuitem name='AboutCatEyeAction' action='AboutCatEyeAction'/></menu></menubar></ui>");
        this.menubar1 = ((Gtk.MenuBar)(this.UIManager.GetWidget("/menubar1")));
        this.menubar1.Name = "menubar1";
        this.vbox2.Add(this.menubar1);
        Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.vbox2[this.menubar1]));
        w2.Position = 0;
        w2.Expand = false;
        w2.Fill = false;
        // Container child vbox2.Gtk.Box+BoxChild
        this.vbox1 = new Gtk.VBox();
        this.vbox1.Name = "vbox1";
        this.vbox1.Spacing = 4;
        this.vbox1.BorderWidth = ((uint)(4));
        // Container child vbox1.Gtk.Box+BoxChild
        this.hbox5 = new Gtk.HBox();
        this.hbox5.Name = "hbox5";
        this.hbox5.Spacing = 6;
        // Container child hbox5.Gtk.Box+BoxChild
        this.left_vbox = new Gtk.VBox();
        this.left_vbox.Name = "left_vbox";
        this.left_vbox.Spacing = 10;
        this.left_vbox.BorderWidth = ((uint)(4));
        // Container child left_vbox.Gtk.Box+BoxChild
        this.label5 = new Gtk.Label();
        this.label5.Name = "label5";
        this.label5.Xalign = 0F;
        this.label5.LabelProp = Mono.Unix.Catalog.GetString("<b>Stage 1</b>");
        this.label5.UseMarkup = true;
        this.left_vbox.Add(this.label5);
        Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.left_vbox[this.label5]));
        w3.Position = 0;
        w3.Expand = false;
        w3.Fill = false;
        // Container child left_vbox.Gtk.Box+BoxChild
        this.table4 = new Gtk.Table(((uint)(2)), ((uint)(1)), false);
        this.table4.Name = "table4";
        this.table4.RowSpacing = ((uint)(6));
        this.table4.ColumnSpacing = ((uint)(6));
        // Container child table4.Gtk.Table+TableChild
        this.label1 = new Gtk.Label();
        this.label1.Name = "label1";
        this.label1.Xalign = 0F;
        this.label1.LabelProp = Mono.Unix.Catalog.GetString("Downscale on loading:");
        this.table4.Add(this.label1);
        Gtk.Table.TableChild w4 = ((Gtk.Table.TableChild)(this.table4[this.label1]));
        w4.YOptions = ((Gtk.AttachOptions)(4));
        // Container child table4.Gtk.Table+TableChild
        this.prescale_combobox = Gtk.ComboBox.NewText();
        this.prescale_combobox.Name = "prescale_combobox";
        this.table4.Add(this.prescale_combobox);
        Gtk.Table.TableChild w5 = ((Gtk.Table.TableChild)(this.table4[this.prescale_combobox]));
        w5.TopAttach = ((uint)(1));
        w5.BottomAttach = ((uint)(2));
        w5.XOptions = ((Gtk.AttachOptions)(4));
        w5.YOptions = ((Gtk.AttachOptions)(4));
        this.left_vbox.Add(this.table4);
        Gtk.Box.BoxChild w6 = ((Gtk.Box.BoxChild)(this.left_vbox[this.table4]));
        w6.Position = 1;
        w6.Expand = false;
        w6.Fill = false;
        // Container child left_vbox.Gtk.Box+BoxChild
        this.label15 = new Gtk.Label();
        this.label15.Name = "label15";
        this.label15.Xalign = 0F;
        this.label15.LabelProp = Mono.Unix.Catalog.GetString("<b>Stage 2</b>");
        this.label15.UseMarkup = true;
        this.left_vbox.Add(this.label15);
        Gtk.Box.BoxChild w7 = ((Gtk.Box.BoxChild)(this.left_vbox[this.label15]));
        w7.Position = 2;
        w7.Expand = false;
        w7.Fill = false;
        // Container child left_vbox.Gtk.Box+BoxChild
        this.GtkScrolledWindow1 = new Gtk.ScrolledWindow();
        this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
        this.GtkScrolledWindow1.HscrollbarPolicy = ((Gtk.PolicyType)(2));
        // Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
        Gtk.Viewport w8 = new Gtk.Viewport();
        w8.ShadowType = ((Gtk.ShadowType)(0));
        // Container child GtkViewport2.Gtk.Container+ContainerChild
        this.stage_vbox = new Gtk.VBox();
        this.stage_vbox.Name = "stage_vbox";
        this.stage_vbox.Spacing = 6;
        this.stage_vbox.BorderWidth = ((uint)(3));
        w8.Add(this.stage_vbox);
        this.GtkScrolledWindow1.Add(w8);
        this.left_vbox.Add(this.GtkScrolledWindow1);
        Gtk.Box.BoxChild w11 = ((Gtk.Box.BoxChild)(this.left_vbox[this.GtkScrolledWindow1]));
        w11.Position = 3;
        // Container child left_vbox.Gtk.Box+BoxChild
        this.hbox9 = new Gtk.HBox();
        this.hbox9.Name = "hbox9";
        this.hbox9.Spacing = 6;
        // Container child hbox9.Gtk.Box+BoxChild
        this.apply_stage2_button = new Gtk.Button();
        this.apply_stage2_button.CanFocus = true;
        this.apply_stage2_button.Name = "apply_stage2_button";
        this.apply_stage2_button.UseUnderline = true;
        // Container child apply_stage2_button.Gtk.Container+ContainerChild
        Gtk.Alignment w12 = new Gtk.Alignment(0.5F, 0.5F, 0F, 0F);
        // Container child GtkAlignment.Gtk.Container+ContainerChild
        Gtk.HBox w13 = new Gtk.HBox();
        w13.Spacing = 2;
        // Container child GtkHBox.Gtk.Container+ContainerChild
        Gtk.Image w14 = new Gtk.Image();
        w14.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-refresh", Gtk.IconSize.Menu, 16);
        w13.Add(w14);
        // Container child GtkHBox.Gtk.Container+ContainerChild
        Gtk.Label w16 = new Gtk.Label();
        w16.LabelProp = Mono.Unix.Catalog.GetString("Update view");
        w16.UseUnderline = true;
        w13.Add(w16);
        w12.Add(w13);
        this.apply_stage2_button.Add(w12);
        this.hbox9.Add(this.apply_stage2_button);
        Gtk.Box.BoxChild w20 = ((Gtk.Box.BoxChild)(this.hbox9[this.apply_stage2_button]));
        w20.Position = 0;
        this.left_vbox.Add(this.hbox9);
        Gtk.Box.BoxChild w21 = ((Gtk.Box.BoxChild)(this.left_vbox[this.hbox9]));
        w21.Position = 4;
        w21.Expand = false;
        w21.Fill = false;
        this.hbox5.Add(this.left_vbox);
        Gtk.Box.BoxChild w22 = ((Gtk.Box.BoxChild)(this.hbox5[this.left_vbox]));
        w22.Position = 0;
        w22.Expand = false;
        w22.Fill = false;
        // Container child hbox5.Gtk.Box+BoxChild
        this.vbox3 = new Gtk.VBox();
        this.vbox3.Name = "vbox3";
        this.vbox3.Spacing = 6;
        // Container child vbox3.Gtk.Box+BoxChild
        this.scrolledwindow3 = new Gtk.ScrolledWindow();
        this.scrolledwindow3.CanFocus = true;
        this.scrolledwindow3.Name = "scrolledwindow3";
        this.scrolledwindow3.ShadowType = ((Gtk.ShadowType)(1));
        // Container child scrolledwindow3.Gtk.Container+ContainerChild
        Gtk.Viewport w23 = new Gtk.Viewport();
        w23.ShadowType = ((Gtk.ShadowType)(0));
        // Container child GtkViewport.Gtk.Container+ContainerChild
        this.ppmviewwidget1 = new CatEye.DoublePixmapViewWidget();
        this.ppmviewwidget1.Events = ((Gdk.EventMask)(256));
        this.ppmviewwidget1.Name = "ppmviewwidget1";
        w23.Add(this.ppmviewwidget1);
        this.scrolledwindow3.Add(w23);
        this.vbox3.Add(this.scrolledwindow3);
        Gtk.Box.BoxChild w26 = ((Gtk.Box.BoxChild)(this.vbox3[this.scrolledwindow3]));
        w26.Position = 1;
        // Container child vbox3.Gtk.Box+BoxChild
        this.status_bar_hbox = new Gtk.HBox();
        this.status_bar_hbox.Name = "status_bar_hbox";
        this.status_bar_hbox.Spacing = 6;
        // Container child status_bar_hbox.Gtk.Box+BoxChild
        this.progressbar = new Gtk.ProgressBar();
        this.progressbar.Name = "progressbar";
        this.status_bar_hbox.Add(this.progressbar);
        Gtk.Box.BoxChild w27 = ((Gtk.Box.BoxChild)(this.status_bar_hbox[this.progressbar]));
        w27.Position = 0;
        // Container child status_bar_hbox.Gtk.Box+BoxChild
        this.cancel_button = new Gtk.Button();
        this.cancel_button.Sensitive = false;
        this.cancel_button.CanFocus = true;
        this.cancel_button.Name = "cancel_button";
        this.cancel_button.UseStock = true;
        this.cancel_button.UseUnderline = true;
        this.cancel_button.Label = "gtk-cancel";
        this.status_bar_hbox.Add(this.cancel_button);
        Gtk.Box.BoxChild w28 = ((Gtk.Box.BoxChild)(this.status_bar_hbox[this.cancel_button]));
        w28.Position = 1;
        w28.Expand = false;
        w28.Fill = false;
        this.vbox3.Add(this.status_bar_hbox);
        Gtk.Box.BoxChild w29 = ((Gtk.Box.BoxChild)(this.vbox3[this.status_bar_hbox]));
        w29.Position = 2;
        w29.Expand = false;
        w29.Fill = false;
        this.hbox5.Add(this.vbox3);
        Gtk.Box.BoxChild w30 = ((Gtk.Box.BoxChild)(this.hbox5[this.vbox3]));
        w30.Position = 1;
        this.vbox1.Add(this.hbox5);
        Gtk.Box.BoxChild w31 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbox5]));
        w31.Position = 0;
        this.vbox2.Add(this.vbox1);
        Gtk.Box.BoxChild w32 = ((Gtk.Box.BoxChild)(this.vbox2[this.vbox1]));
        w32.Position = 1;
        this.Add(this.vbox2);
        if ((this.Child != null)) {
            this.Child.ShowAll();
        }
        this.DefaultWidth = 879;
        this.DefaultHeight = 500;
        this.Show();
        this.DeleteEvent += new Gtk.DeleteEventHandler(this.OnDeleteEvent);
        this.openAction.Activated += new System.EventHandler(this.OnOpenActionActivated);
        this.saveAsAction.Activated += new System.EventHandler(this.OnSaveAsActionActivated);
        this.ImportFromDCRawAction.Activated += new System.EventHandler(this.OnImportFromDCRawActionActivated);
        this.apply_stage2_button.Clicked += new System.EventHandler(this.OnApplyStage2ButtonClicked);
        this.cancel_button.Clicked += new System.EventHandler(this.OnCancelButtonClicked);
    }
}
