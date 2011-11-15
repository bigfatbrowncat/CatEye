using System;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk
{
	public class ReceiptSaveDialog : Dialog
	{
		public enum ReceiptType
		{
			Default, Custom, Class
		}
		
		private bool mUIChangingInProgress = false;
		private Button mCancelButton, mSaveButton;
		private RadioButton mDefault, mCustom, mClass;
		private ComboBoxEntry mNameComboBoxEntry;
		private string mRawFileName;
		
		public ReceiptSaveDialog (Window parent, string rawFileName) : base()
		{
			mRawFileName = rawFileName;
			
			// Laying out
			
			Title = "Save receipt for " + System.IO.Path.GetFileName(mRawFileName);
			SetPosition(WindowPosition.Center);
			SetSizeRequest(450, 180);
			//this.AllowGrow = false;
			this.SkipPagerHint = true;
			this.SkipTaskbarHint = true;
			this.HasSeparator = false;
			Parent = parent;
			mCancelButton = (Button)AddButton("Cancel", ResponseType.Cancel);
			mSaveButton = (Button)AddButton("Save", ResponseType.Accept);
			this.Default = mSaveButton;
			
			VBox radio_buttons_box = new VBox(false, 2);
			radio_buttons_box.BorderWidth = 6;
			mDefault = new RadioButton("Default receipt for this photo (no name)");
			mCustom = new RadioButton(mDefault, "Custom receipt for this photo");
			mClass = new RadioButton(mDefault, "A common receipt for all photos in the same folder");
			radio_buttons_box.PackStart(mDefault);
			radio_buttons_box.PackStart(mCustom);
			radio_buttons_box.PackStart(mClass);
			VBox.Add(radio_buttons_box);
			
			HBox name_box = new HBox(false, 8);
			name_box.BorderWidth = 6;
			Image receipt_icon = new Image();
			receipt_icon.Pixbuf = Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.res.png.cestage-small-24x24.png");

			Label name_label = new Label("Name:");
			
			ListStore name_store = new ListStore(typeof(string));
			mNameComboBoxEntry = new ComboBoxEntry(name_store, 0);
			
			name_box.PackStart(receipt_icon, false, false, 0);
			name_box.PackStart(name_label, false, false, 0);
			name_box.PackStart(mNameComboBoxEntry, true, true, 0);
			VBox.PackStart(name_box, false, false, 0);
			
			// Adding events
			
			Shown += HandleUIChange;
			mDefault.Clicked += HandleUIChange;
			mCustom.Clicked += HandleUIChange;
			mClass.Clicked += HandleUIChange;
			mNameComboBoxEntry.Entry.Changed += HandleUIChange;
			
			this.ShowAll();
		}

		void HandleUIChange (object sender, EventArgs e)
		{
			if (!mUIChangingInProgress)
			{
				mUIChangingInProgress = true;
				try
				{
					if (mDefault.Active)
					{
						mNameComboBoxEntry.Sensitive = false;
						mSaveButton.Sensitive = true;
					}
					else if (mCustom.Active)
					{
						mNameComboBoxEntry.Sensitive = true;
						mSaveButton.Sensitive = (mNameComboBoxEntry.ActiveText != "");
						
						// Updating names list
						ListStore ls = (ListStore)mNameComboBoxEntry.Model;
						ls.Clear();
						string[] customReceipts = CatEye.Core.ReceiptsManager.FindCustomReceiptsForRaw(mRawFileName);
						for (int i = 0; i < customReceipts.Length; i++)
						{
							ls.AppendValues(ReceiptsManager.ExtractReceiptName(customReceipts[i], mRawFileName));
						}
					}
					else if (mClass.Active)
					{
						mNameComboBoxEntry.Sensitive = true;
						mSaveButton.Sensitive = (mNameComboBoxEntry.ActiveText != "");
		
						// Updating names list
						ListStore ls = (ListStore)mNameComboBoxEntry.Model;
						ls.Clear();
						string[] classReceipts = CatEye.Core.ReceiptsManager.FindClassReceiptsForRaw(mRawFileName);
						for (int i = 0; i < classReceipts.Length; i++)
						{
							ls.AppendValues(ReceiptsManager.ExtractReceiptName(classReceipts[i], mRawFileName));
						}
					}
				}
				finally
				{
					mUIChangingInProgress = false;
				}
				
			}
		}

		public ReceiptType SelectedType
		{
			get
			{ 
				if (mDefault.Active) return ReceiptType.Default;
				else if (mCustom.Active) return ReceiptType.Custom;
				return ReceiptType.Class;
			}
			set
			{
				if (value == ReceiptType.Default) mDefault.Active = true;
				else if (value == ReceiptType.Custom) mCustom.Active = true;
				else mClass.Active = true;
			}
		}
		
		public string SelectedName
		{
			get
			{
				return mNameComboBoxEntry.ActiveText;
			}
			set
			{
				mNameComboBoxEntry.Entry.Text = value;
			}
		}
	}
}

