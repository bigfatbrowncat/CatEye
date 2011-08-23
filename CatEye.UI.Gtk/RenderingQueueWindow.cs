using System;
using Gtk;
using CatEye.Core;
using CatEye.UI.Base;
using CatEye.UI.Gtk.Widgets;

namespace CatEye.UI.Gtk
{
	public partial class RenderingQueueWindow : Window
	{
		private RenderingQueue mRenderingQueue;
		private NodeStore mQueueNodeStore;
		private bool mIsDestroyed;
		
		private StatusIcon mProcessingStatusIcon;
		
		[TreeNode(ListOnly = true)]
		private class RenderingTaskTreeNode : TreeNode
		{
			private RenderingTask mTask;
			public RenderingTaskTreeNode(RenderingTask task)
			{
				mTask = task;
			}
			
			[TreeNodeValue(Column = 0)]
			public string Source 
			{ 
				get 
				{ 
					return System.IO.Path.GetFileName(mTask.Source); 
				}
			}
			
			[TreeNodeValue(Column = 1)]
			public string Destination 
			{ 
				get 
				{ 
					return System.IO.Path.GetFileName(mTask.Destination); 
				}
			}

			[TreeNodeValue(Column = 2)]
			public string FileType 
			{ 
				get 
				{ 
					return mTask.FileType;
				} 
			}
			
			public RenderingTask Task { get { return mTask; } }
		}
		
		private void RefreshQueueList()
		{
			mQueueNodeStore.Clear();
			for (int i = 0; i < mRenderingQueue.Queue.Length; i++)
			{
				mQueueNodeStore.AddNode(new RenderingTaskTreeNode(mRenderingQueue.Queue[i]));
			}
		}
		
		public RenderingQueueWindow (RenderingQueue renderingQueue) : 
				base(WindowType.Toplevel)
		{
			this.Build ();

			// Adding queue event handlers
			mRenderingQueue = renderingQueue;
			mRenderingQueue.QueueProgressMessageReport += HandleRenderingQueueProgressMessageReport;
			mRenderingQueue.ItemAdded += HandleRenderingQueueItemAdded;
			mRenderingQueue.ItemIndexChanged += HandleRenderingQueueItemIndexChanged;
			mRenderingQueue.ItemRemoved += HandleRenderingQueueItemRemoved;
			mRenderingQueue.BeforeItemProcessingStarted += HandleRenderingQueueBeforeItemProcessingStarted;
			mRenderingQueue.AfterItemProcessingFinished += HandleRenderingQueueAfterItemProcessingFinished;
			mRenderingQueue.ThreadStarted += HandleRenderingQueueThreadStarted;
			mRenderingQueue.ThreadStopped += HandleRenderingQueueThreadStopped;
			mRenderingQueue.QueueEmpty += HandleRenderingQueueQueueEmpty;
			mRenderingQueue.ItemRendering += HandleRenderingQueueItemRendering;
			
			// Creating status icon
			mProcessingStatusIcon = new StatusIcon(Gdk.Pixbuf.LoadFromResource("CatEye.UI.Gtk.res.cateye.png"));
			mProcessingStatusIcon.Visible = false;	// In Windows status icon appears visible by default
			mProcessingStatusIcon.Activate += HandleProcessingStatusIconActivate; 
			
			// Preparing queue list
			mQueueNodeStore = new NodeStore(typeof(RenderingTaskTreeNode));
			queue_nodeview.NodeStore = mQueueNodeStore;

			queue_nodeview.AppendColumn("Source", new CellRendererText(), "text", 0);
			queue_nodeview.AppendColumn("Destination", new CellRendererText(), "text", 1);

			expander1.Expanded = false;
			queue_GtkLabel.Markup = "<b>Queue (" + mRenderingQueue.Queue.Length + " left)</b>";
			
		}

		public bool IsDestroyed { get { return mIsDestroyed; } }
		
#region Handlers called from other thread. 
	// Each handler here should contain Application.Invoke

		void HandleRenderingQueueItemRendering (object sender, RenderingTaskEventArgs e)
		{
			Application.Invoke(delegate {
				FloatBitmapGtk renderDest = (FloatBitmapGtk)e.Target.Stage.CurrentImage;
				
				// Drawing to pixbuf and saving to file
				using (Gdk.Pixbuf rp = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 8, renderDest.Width, renderDest.Height))
				{
		
					renderDest.DrawToPixbuf(rp, 
						delegate (double progress) {
						
							// TODO: report progress via RenderingTask (needed to create an event)
							//rpw.SetStatusAndProgress(progress, "Saving image...");
							return true;
						}
					);
				
					// TODO Can't be used currently cause of buggy Gtk#
					//rp.Savev(filename, type, new string[] { "quality" }, new string[] { "95" });
			
					rp.Save(e.Target.Destination, e.Target.FileType);
				}
			});
			
		}

		void HandleRenderingQueueThreadStopped (object sender, EventArgs e)
		{
			Application.Invoke(delegate {
				Visible = false;
				Destroy();
				mIsDestroyed = true;
			});
		}
		
		
		void HandleRenderingQueueQueueEmpty (object sender, EventArgs e)
		{
			Application.Invoke(delegate {
				mProcessingStatusIcon.Visible = false;
				Visible = false;
			});
		}

		void HandleRenderingQueueThreadStarted (object sender, EventArgs e)
		{
			Application.Invoke(delegate {
				//Visible = true;
			});
		}

		void HandleProcessingStatusIconActivate (object sender, EventArgs e)
		{
			Visible = !Visible;
		}

		void HandleRenderingQueueAfterItemProcessingFinished (object sender, RenderingTaskEventArgs e)
		{
			Application.Invoke(delegate {
				source_label.Text = "";
				destination_label.Text = "";
				processing_progressbar.Fraction = 1;
				processing_progressbar.Text = "Render process is completed";
			});
		}

		void HandleRenderingQueueBeforeItemProcessingStarted (object sender, RenderingTaskEventArgs e)
		{
			Application.Invoke(delegate {
				cancel_button.Sensitive = true;
				cancelAll_button.Sensitive = true;
				source_label.Text = "";
				destination_label.Text = "";
				processing_progressbar.Fraction = 0;
				processing_progressbar.Text = "Starting the render process...";
				mProcessingStatusIcon.Visible = true;
				
			});
		}

		void HandleRenderingQueueItemRemoved (object sender, RenderingTaskEventArgs e)
		{
			Application.Invoke(delegate {
				RefreshQueueList();
				if (!expander1.Expanded)
				{
					queue_GtkLabel.Markup = "<b>Tasks queue (" + mRenderingQueue.Queue.Length + " left)</b>";
				}
			});
		}

		void HandleRenderingQueueItemIndexChanged (object sender, RenderingTaskEventArgs e)
		{
			Application.Invoke(delegate {
				RefreshQueueList();
			});
		}

		void HandleRenderingQueueItemAdded (object sender, RenderingTaskEventArgs e)
		{
			Application.Invoke(delegate {
				RefreshQueueList();
				if (!expander1.Expanded)
				{
					queue_GtkLabel.Markup = "<b>Tasks queue (" + mRenderingQueue.Queue.Length + " left)</b>";
				}
			});
		}

		private void HandleRenderingQueueProgressMessageReport (string source, string destination, double progress, string status)
		{
			Application.Invoke(delegate {
				source_label.Text = source;
				destination_label.Text = destination;
				processing_progressbar.Fraction = progress;
				processing_progressbar.Text = status;
				
				mProcessingStatusIcon.Tooltip = "Processing " + System.IO.Path.GetFileName(source) + ". " + status + " (" + ((int)(progress * 100)).ToString() + "%)";
			});
		}
		
#endregion
		
		protected void OnExpander1Activated (object sender, System.EventArgs e)
		{
			if (expander1.Expanded == false)
			{
				this.AllowGrow = false;
				queue_GtkLabel.Markup = "<b>Tasks queue (" + mRenderingQueue.Queue.Length + " left)</b>";
			}
			else
			{
				this.AllowGrow = true;
				queue_GtkLabel.Markup = "<b>Tasks queue</b>";
			}
		}

		protected void OnCancelButtonClicked (object sender, System.EventArgs e)
		{
			cancel_button.Sensitive = false;
			mRenderingQueue.CancelItem();
		}

		protected void OnCancelAllButtonClicked (object sender, System.EventArgs e)
		{
			cancelAll_button.Sensitive = false;
			mRenderingQueue.CancelAllItems();
		}

		protected void OnRemoveButtonClicked (object sender, System.EventArgs e)
		{
			ITreeNode[] selnodes = queue_nodeview.NodeSelection.SelectedNodes;
			
			foreach (ITreeNode itr in selnodes)
				mRenderingQueue.Remove(((RenderingTaskTreeNode)itr).Task);
		}

		protected void OnDeleteEvent (object o, DeleteEventArgs args)
		{
			this.Hide();
			args.RetVal = true;
		}

		protected void OnQueueNodeviewCursorChanged (object sender, System.EventArgs e)
		{
			ITreeNode[] selnodes = queue_nodeview.NodeSelection.SelectedNodes;
			if (selnodes.Length == 0)
			{
				up_button.Sensitive = false;
				down_button.Sensitive = false;
			}
			else
			{
				RenderingTask seltask = ((RenderingTaskTreeNode)selnodes[0]).Task;
				
				up_button.Sensitive = mRenderingQueue.IndexOf(seltask) > 0;
				down_button.Sensitive = mRenderingQueue.IndexOf(seltask) < mRenderingQueue.Count - 1;
			}
		}

		protected void OnUpButtonClicked (object sender, System.EventArgs e)
		{
			ITreeNode[] selnodes = queue_nodeview.NodeSelection.SelectedNodes;
			if (selnodes.Length > 0)
			{
				RenderingTask seltask = ((RenderingTaskTreeNode)selnodes[0]).Task;
				mRenderingQueue.StepUp(seltask);
			}
		}

		protected void OnDownButtonClicked (object sender, System.EventArgs e)
		{
			ITreeNode[] selnodes = queue_nodeview.NodeSelection.SelectedNodes;
			if (selnodes.Length > 0)
			{
				RenderingTask seltask = ((RenderingTaskTreeNode)selnodes[0]).Task;
				mRenderingQueue.StepDown(seltask);
			}
		}

		protected void OnShown (object sender, System.EventArgs e)
		{
			titleGtkLabel.ModifyFont(Pango.FontDescription.FromString("12"));
		}
	}
}

