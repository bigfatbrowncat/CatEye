// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace CatEye {
    
    
    public partial class StageOperationTitleWidget {
        
        private Gtk.HBox hbox;
        
        private Gtk.Button up_button;
        
        private Gtk.Arrow arrow5;
        
        private Gtk.Button down_button;
        
        private Gtk.Arrow arrow6;
        
        private Gtk.ToggleButton title_togglebutton;
        
        private Gtk.Label title_label;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget CatEye.StageOperationTitleWidget
            Stetic.BinContainer.Attach(this);
            this.Name = "CatEye.StageOperationTitleWidget";
            // Container child CatEye.StageOperationTitleWidget.Gtk.Container+ContainerChild
            this.hbox = new Gtk.HBox();
            this.hbox.Name = "hbox";
            // Container child hbox.Gtk.Box+BoxChild
            this.up_button = new Gtk.Button();
            this.up_button.WidthRequest = 23;
            this.up_button.CanFocus = true;
            this.up_button.Name = "up_button";
            this.up_button.Relief = ((Gtk.ReliefStyle)(2));
            // Container child up_button.Gtk.Container+ContainerChild
            this.arrow5 = new Gtk.Arrow(((Gtk.ArrowType)(0)), ((Gtk.ShadowType)(0)));
            this.arrow5.Name = "arrow5";
            this.up_button.Add(this.arrow5);
            this.up_button.Label = null;
            this.hbox.Add(this.up_button);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.hbox[this.up_button]));
            w2.Position = 0;
            w2.Expand = false;
            w2.Fill = false;
            // Container child hbox.Gtk.Box+BoxChild
            this.down_button = new Gtk.Button();
            this.down_button.WidthRequest = 23;
            this.down_button.CanFocus = true;
            this.down_button.Name = "down_button";
            this.down_button.Relief = ((Gtk.ReliefStyle)(2));
            // Container child down_button.Gtk.Container+ContainerChild
            this.arrow6 = new Gtk.Arrow(((Gtk.ArrowType)(1)), ((Gtk.ShadowType)(0)));
            this.arrow6.Name = "arrow6";
            this.down_button.Add(this.arrow6);
            this.down_button.Label = null;
            this.hbox.Add(this.down_button);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.hbox[this.down_button]));
            w4.Position = 1;
            w4.Expand = false;
            w4.Fill = false;
            // Container child hbox.Gtk.Box+BoxChild
            this.title_togglebutton = new Gtk.ToggleButton();
            this.title_togglebutton.CanFocus = true;
            this.title_togglebutton.Name = "title_togglebutton";
            this.title_togglebutton.Relief = ((Gtk.ReliefStyle)(2));
            this.title_togglebutton.Active = true;
            // Container child title_togglebutton.Gtk.Container+ContainerChild
            this.title_label = new Gtk.Label();
            this.title_label.Name = "title_label";
            this.title_label.LabelProp = Mono.Unix.Catalog.GetString("<b>Title</b>");
            this.title_label.UseMarkup = true;
            this.title_label.SingleLineMode = true;
            this.title_togglebutton.Add(this.title_label);
            this.title_togglebutton.Label = null;
            this.hbox.Add(this.title_togglebutton);
            Gtk.Box.BoxChild w6 = ((Gtk.Box.BoxChild)(this.hbox[this.title_togglebutton]));
            w6.Position = 2;
            this.Add(this.hbox);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Hide();
            this.up_button.Clicked += new System.EventHandler(this.OnUpButtonClicked);
            this.down_button.Clicked += new System.EventHandler(this.OnDownButtonClicked);
            this.title_togglebutton.Clicked += new System.EventHandler(this.OnTitleButtonClicked);
        }
    }
}
