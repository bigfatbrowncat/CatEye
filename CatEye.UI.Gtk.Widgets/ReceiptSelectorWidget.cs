using System;
using Gtk;
using CatEye.Core;

namespace CatEye.UI.Gtk.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ReceiptSelectorWidget : Bin
	{
		private string mRawFileName;
		private string mSelectedReceiptFileName;
		public string SelectedReceiptFileName
		{
			get { return mSelectedReceiptFileName; }
		}
		
		public string RawFileName
		{
			get 
			{
				return mRawFileName;
			}
			set
			{
				if (mRawFileName != value)
				{
					mRawFileName = value;
					mSelectedReceiptFileName = null;
					
					if (mRawFileName != null)
					{
						this.receipt_combobox.RowSeparatorFunc = delegate (TreeModel model, TreeIter iter)
						{
							return ((string)model.GetValue(iter, 0)) == "-";
						};
						// Acquring receipts
						string[] receiptFiles = ReceiptsManager.FindReceiptsForRaw(mRawFileName);
						
						// ... and listing them
						ListStore ls = new ListStore(typeof(string), typeof(string));
						receipt_combobox.Model = ls;
						
						if (receiptFiles.Length > 0)
						{
							ReceiptsManager.ReceiptType lastRecType = ReceiptsManager.DetermineReceiptType(receiptFiles[0], mRawFileName);
							for (int i = 0; i < receiptFiles.Length; i++)
							{
								if (lastRecType != ReceiptsManager.DetermineReceiptType(receiptFiles[i], mRawFileName))
								{
									lastRecType = ReceiptsManager.DetermineReceiptType(receiptFiles[i], mRawFileName);
									ls.AppendValues("-", "-");
								}
								string rnm = System.IO.Path.GetFileNameWithoutExtension(receiptFiles[i]);
								if (lastRecType == ReceiptsManager.ReceiptType.Default) rnm = "Default";
								if (lastRecType == ReceiptsManager.ReceiptType.Custom)
								{
									rnm = rnm.Substring(rnm.IndexOf("--") + 2);
								}
								
								ls.AppendValues(rnm, receiptFiles[i]);
							}
							TreeIter itf;
							ls.GetIterFirst(out itf);
							receipt_combobox.SetActiveIter(itf);
						}
					}
				}
			}
		}
		
		public ReceiptSelectorWidget ()
		{
			this.Build ();
		}

		protected void OnReceiptComboboxChanged (object sender, System.EventArgs e)
		{
			TreeIter itf;
			receipt_combobox.GetActiveIter(out itf);
			ListStore ls = (ListStore)receipt_combobox.Model;
			mSelectedReceiptFileName = (string)ls.GetValue(itf, 1);
		}
	}
}

