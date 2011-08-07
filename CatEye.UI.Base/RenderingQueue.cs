using System;
using System.Collections.Generic;
using System.Threading;
using CatEye.Core;

namespace CatEye.UI.Base
{
	public class RenderingTaskEventArgs : EventArgs
	{
		RenderingTask _Target;
		public RenderingTask Target { get { return _Target; } }
		public RenderingTaskEventArgs(RenderingTask target)
		{
			_Target = target;
		}
	}
	
	public delegate void QueueProgressMessageReporter(string source, string destination, double progress, string status);
	
	public class RenderingTask
	{
		private Stage mStage;
		private string mSource, mDestination, mFileType;
		
		public Stage Stage { get { return mStage; } }
		public string Source { get { return mSource; } }
		public string Destination { get { return mDestination; } }
		public string FileType { get { return mFileType; } }
		
		public RenderingTask(Stage stage, string source, string destination, string fileType)
		{
			mStage = stage; mSource = source; mDestination = destination; mFileType = fileType;
		}
	}
	
	public class RenderingQueue
	{
		private List<RenderingTask> mQueue;
		private RenderingTask mInProgress = null;
		private Thread mWorkingThread = null;
		private volatile bool mCancel = false, mCancelAll = false;
		
		public event EventHandler<RenderingTaskEventArgs> ItemRemoved;
		public event EventHandler<RenderingTaskEventArgs> ItemAdded;
		public event EventHandler<RenderingTaskEventArgs> ItemIndexChanged;
		public event EventHandler<RenderingTaskEventArgs> BeforeItemProcessingStarted;
		public event EventHandler<RenderingTaskEventArgs> AfterItemProcessingFinished;
		public event EventHandler<EventArgs> BeforeProcessingStarted;
		public event EventHandler<EventArgs> AfterProcessingFinished;
		
		public event EventHandler<RenderingTaskEventArgs> ItemRendering;
		
		public event QueueProgressMessageReporter QueueProgressMessageReport; 
		
		public RenderingTask[] Queue
		{
			get 
			{ 
				return mQueue.ToArray(); 
			}
		}
		
		public RenderingQueue ()
		{
			mQueue = new List<RenderingTask>();
		}
		
		protected virtual void OnItemAdded(RenderingTask item)
		{
			if (ItemAdded != null) 
				ItemAdded(this, new RenderingTaskEventArgs(item));
		}
		protected virtual void OnItemRemoved(RenderingTask item)
		{
			if (ItemRemoved != null) 
				ItemRemoved(this, new RenderingTaskEventArgs(item));
		}
		protected virtual void OnItemIndexChanged(RenderingTask item)
		{
			if (ItemIndexChanged != null)
				ItemIndexChanged(this, new RenderingTaskEventArgs(item));
		}
		protected virtual void OnItemRendering(RenderingTask item)
		{
			if (ItemRendering != null) 
				ItemRendering(this, new RenderingTaskEventArgs(item));
		}
		protected virtual void OnBeforeItemProcessingStarted(RenderingTask item)
		{
			if (BeforeItemProcessingStarted != null)
				BeforeItemProcessingStarted(this, new RenderingTaskEventArgs(item));
		}
		protected virtual void OnAfterItemProcessingFinished(RenderingTask item)
		{
			if (AfterItemProcessingFinished != null)
				AfterItemProcessingFinished(this, new RenderingTaskEventArgs(item));
		}
		protected virtual void OnBeforeProcessingStarted()
		{
			if (BeforeProcessingStarted != null)
				BeforeProcessingStarted(this, EventArgs.Empty);
		}
		protected virtual void OnAfterProcessingFinished()
		{
			if (AfterProcessingFinished != null)
				AfterProcessingFinished(this, EventArgs.Empty);
		}

		protected virtual void OnQueueProgressMessageReport(string source, string destination, double progress, string status)
		{
			if (QueueProgressMessageReport != null)
			{
				QueueProgressMessageReport(source, destination, progress, status);
			}
		}
		
		public void Add(Stage stage, string source, string destination, string fileType)
		{
			stage.ProgressMessageReport += HandleStageProgressMessageReport;
			RenderingTask rt = new RenderingTask(stage, source, destination, fileType);
			mQueue.Add(rt);
			OnItemAdded(rt);
			Process();
		}
		
		private RenderingTask FindRenderingTaskByStage(Stage stg)
		{
			for (int i = 0; i < mQueue.Count; i++)
			{
				if (mQueue[i].Stage == stg) return mQueue[i];
			}
			return null;
		}
		
		void HandleStageProgressMessageReport (object sender, ReportStageProgressMessageEventArgs e)
		{
			RenderingTask rt = mInProgress;
			if (mCancel == true)
			{
				throw new UserCancelException();
			}
			if (mCancelAll == true)
			{
				throw new UserCancelAllException();
			}
			
			OnQueueProgressMessageReport(rt.Source, rt.Destination, e.Progress, e.Status);
		}
		
		public bool StepDown(RenderingTask item)
		{
			int inx = mQueue.IndexOf(item);
			if (inx < mQueue.Count - 1)
			{
				RenderingTask next = mQueue[inx + 1];
				mQueue[inx + 1] = item;
				mQueue[inx] = next;
				OnItemIndexChanged(item);
				OnItemIndexChanged(next);
				return true;
			}
			else
				return false;
		}

		public bool StepUp(RenderingTask item)
		{
			int inx = mQueue.IndexOf(item);
			if (inx > 0)
			{
				RenderingTask prev = mQueue[inx - 1];
				mQueue[inx - 1] = item;
				mQueue[inx] = prev;
				OnItemIndexChanged(item);
				OnItemIndexChanged(prev);
				return true;
			}
			else
				return false;
		}
		
		public int IndexOf(RenderingTask item)
		{
			return mQueue.IndexOf(item);
		}
		
		public void Remove(RenderingTask item)
		{
			lock (item)
			{
				item.Stage.ProgressMessageReport -= HandleStageProgressMessageReport;
				mQueue.Remove(item);
				OnItemRemoved(item);
			}
		}
		
		private void ProcessingThread()
		{
			OnBeforeProcessingStarted();
			mCancel = false;
			mCancelAll = false;
			while (mQueue.Count > 0 && !mCancelAll)
			{
				try
				{
					RenderingTask cur_task = mQueue[0];
					lock (cur_task)
					{
						if (mQueue.Contains(cur_task))
						{
							mInProgress = cur_task;
							mQueue.Remove(cur_task);
							OnItemRemoved(cur_task);
						}
					}
					
					OnBeforeItemProcessingStarted(mInProgress);
					
					mInProgress.Stage.LoadImage(mInProgress.Source, PPMLoader.PreScale);
					mInProgress.Stage.Process();
					OnItemRendering(mInProgress);
					
					mInProgress.Stage.ProgressMessageReport -= HandleStageProgressMessageReport;
					
					OnAfterItemProcessingFinished(mInProgress);
				}
				catch (UserCancelException exp)
				{
#if DEBUG
					Console.WriteLine("User has sent " + exp.GetType().Name + " exception.");
#endif
					if (mCancel) mCancel = false;
				}
			}
#if DEBUG
			if (mCancelAll)
				Console.WriteLine("Processing cancelled.");
			else
				Console.WriteLine("Processing finished.");
#endif
			OnAfterProcessingFinished();
		}
		
		public bool IsProcessing
		{
			get { return mWorkingThread != null && mWorkingThread.IsAlive; }
		}
		
		protected void Process()
		{
			if (!IsProcessing)
			{
				mWorkingThread = new Thread(ProcessingThread);
				mWorkingThread.Start();
			}
		}
		public void CancelAll()
		{
			for (int i = 0; i < mQueue.Count; i++)
				OnItemRemoved(mQueue[i]);
			mQueue.Clear();

			mCancelAll = true;
		}
		public void Cancel()
		{
			mCancel = true;
		}
		public int Count { get { return mQueue.Count; } }
	}
}

