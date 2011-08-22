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
			mRenderingQueue = renderingQueue;
			mRenderingQueue.QueueProgressMessageReport += HandleRenderingQueueProgressMessageReport;
			mRenderingQueue.ItemAdded += HandleRenderingQueueItemAdded;
			mRenderingQueue.ItemIndexChanged += HandleRenderingQueueItemIndexChanged;
			mRenderingQueue.ItemRemoved += HandleRenderingQueueItemRemoved;
			mRenderingQueue.BeforeItemProcessingStarted += HandleRenderingQueueBeforeItemProcessingStarted;
			mRenderingQueue.AfterItemProcessingFinished += HandleRenderingQueueAfterItemProcessingFinished;
			mRenderingQueue.BeforeProcessingStarted += HandleRenderingQueueBeforeProcessingStarted;
			mRenderingQueue.AfterProcessingFinished += HandleRenderingQueueAfterProcessingFinished;
			
			this.Build ();
			
			// Preparing queue list
			mQueueNodeStore = new NodeStore(typeof(RenderingTaskTreeNode));
			queue_nodeview.NodeStore = mQueueNodeStore;

			queue_nodeview.AppendColumn("Source", new CellRendererText(), "text", 0);
			queue_nodeview.AppendColumn("Destination", new CellRendererText(), "text", 1);

			expander1.Expanded = false;
			queue_GtkLabel.Markup = "<b>Queue (" + mRenderingQueue.Queue.Length + " left)</b>";
		}

#region Handlers called from other thread. 
	// Each handler here should contain Application.Invoke

		void HandleRenderingQueueAfterProcessingFinished (object sender, EventArgs e)
		{
			Application.Invoke(delegate {
				Visible = false;
			});
		}

		void HandleRenderingQueueBeforeProcessingStarted (object sender, EventArgs e)
		{
			Application.Invoke(delegate {
				Visible = true;
			});
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
				
			});
		}

		void HandleRenderingQueueItemRemoved (object sender, RenderingTaskEventArgs e)
		{
			Application.Invoke(delegate {
				RefreshQueueList();
				if (!expander1.Expanded)
				{
					queue_GtkLabel.Markup = "<b>Queue (" + mRenderingQueue.Queue.Length + " left)</b>";
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
					queue_GtkLabel.Markup = "<b>Queue (" + mRenderingQueue.Queue.Length + " left)</b>";
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
			});
		}
		
#endregion
		
		protected void OnExpander1Activated (object sender, System.EventArgs e)
		{
			if (expander1.Expanded == false)
			{
				this.AllowGrow = false;
				queue_GtkLabel.Markup = "<b>Queue (" + mRenderingQueue.Queue.Length + " left)</b>";
			}
			else
			{
				this.AllowGrow = true;
				queue_GtkLabel.Markup = "<b>Queue</b>";
			}
		}

		protected void OnCancelButtonClicked (object sender, System.EventArgs e)
		{
			cancel_button.Sensitive = false;
			mRenderingQueue.Cancel();
		}

		protected void OnCancelAllButtonClicked (object sender, System.EventArgs e)
		{
			cancelAll_button.Sensitive = false;
			mRenderingQueue.CancelAll();
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
	}
}

