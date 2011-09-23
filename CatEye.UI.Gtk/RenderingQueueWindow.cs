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
		DateTime updateMoment;
		TimeSpan drawingTimeSpan;
		
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
			
			title_Label.ModifyFont(FontHelpers.ScaleFontSize(title_Label, 1.4));
			
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
			
			string icon_res;
			if (System.Environment.OSVersion.Platform == PlatformID.Unix)
				icon_res = "CatEye.UI.Gtk.res.svg-inkscape.cateye-small.svg";
			else
			{
				// Windows, I hope.
				icon_res = "CatEye.UI.Gtk.res.png.cateye-small-16x16.png";
			}
			
			mProcessingStatusIcon = new StatusIcon(Gdk.Pixbuf.LoadFromResource(icon_res));
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
				
					// TODO: "quality" must be specified for jpeg only. 
					// For png it should be "tEXt" fields.
					// http://www.libpng.org/pub/png/spec/1.2/PNG-Chunks.html
					rp.Savev(e.Target.Destination, e.Target.FileType, new string[] { "quality" }, new string[] { "95" });
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
				thumb_image.Clear();
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
				updateMoment = DateTime.Now;
				
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
		
		private void HandleRenderingQueueProgressMessageReport (string source, string destination, double progress, string status, IBitmapCore image)
		{
			Application.Invoke(delegate {
				source_label.Text = source;
				destination_label.Text = destination;
				processing_progressbar.Fraction = progress;
				processing_progressbar.Text = status;
				
				mProcessingStatusIcon.Tooltip = "Processing " + System.IO.Path.GetFileName(source) + ". " + status + " (" + ((int)(progress * 100)).ToString() + "%)";
				
				thumb_image.Visible = (image != null);
				
				if ((DateTime.Now - updateMoment).TotalMilliseconds > drawingTimeSpan.TotalMilliseconds * 5 && image != null && this.Visible)
				{
					updateMoment = DateTime.Now;
					// Drawing
					int size = 300, margins = 30;
					
					thumb_image.SetSizeRequest(size + margins, size + margins);
					using (Gdk.Pixmap pm = new Gdk.Pixmap(thumb_image.GdkWindow, size + margins, size + margins, -1))
					{
						using (Gdk.Pixbuf pb = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 8, image.Width, image.Height))
						{
							using (Gdk.GC gc = new Gdk.GC(thumb_image.GdkWindow))
							{
								((FloatBitmapGtk)image).DrawToPixbuf(pb, null);
								
								Gdk.Pixbuf pb2;
	
								if (pb.Width > pb.Height)
									pb2 = pb.ScaleSimple(size, (int)((double)pb.Height / pb.Width * size), Gdk.InterpType.Bilinear);
								else
									pb2 = pb.ScaleSimple((int)((double)pb.Width / pb.Height * size), size, Gdk.InterpType.Bilinear);
							
								pm.DrawRectangle(gc, true, new Gdk.Rectangle(0, 0, size + margins, size + margins));
								
								pm.DrawPixbuf(gc, pb2, 0, 0, 
								              (size + margins) / 2 - pb2.Width / 2, 
								              (size + margins) / 2 - pb2.Height / 2, 
								              pb2.Width, pb2.Height, Gdk.RgbDither.Max, 0, 0);
								
								pb2.Dispose();
								
								thumb_image.SetFromPixmap(pm, null);
							}
						}
					}
					drawingTimeSpan = DateTime.Now - updateMoment;
				}
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
			cancel_button.Sensitive = false;
			cancelAll_button.Sensitive = false;
			mRenderingQueue.CancelAllItems();
		}

		protected void OnRemoveButtonClicked (object sender, System.EventArgs e)
		{
			ITreeNode[] selnodes = queue_nodeview.NodeSelection.SelectedNodes;
			
			foreach (ITreeNode itr in selnodes)
				mRenderingQueue.Remove(((RenderingTaskTreeNode)itr).Task);
			
			remove_button.Sensitive = false;
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
				remove_button.Sensitive = false;
			}
			else
			{
				RenderingTask seltask = ((RenderingTaskTreeNode)selnodes[0]).Task;
				
				remove_button.Sensitive = true;
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

		}
	}
}

