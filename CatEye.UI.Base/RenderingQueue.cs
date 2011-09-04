using System;
using System.Collections.Generic;
using System.Threading;
using CatEye.Core;

namespace CatEye.UI.Base
{
	public class RenderingTask
	{
		private Stage mStage;
		private int mPrescale;
		private string mSource, mDestination, mFileType;
		
		public Stage Stage { get { return mStage; } }
		public string Source { get { return mSource; } }
		public string Destination { get { return mDestination; } }
		public string FileType { get { return mFileType; } }
		public int Prescale { get { return mPrescale; } } 
		
		public RenderingTask(Stage stage, string source, int prescale, string destination, string fileType)
		{
			mStage = stage; 
			mPrescale = prescale;
			mSource = source; 
			mDestination = destination; 
			mFileType = fileType;
		}
	}
	
	public class RenderingQueue
	{
		private List<RenderingTask> mQueue;
		private RenderingTask mInProgress = null;
		private Thread mWorkingThread = null;
		private volatile bool mCancelItem = false, mCancelAllItems = false;
		
		public event EventHandler<RenderingTaskEventArgs> ItemRemoved;
		public event EventHandler<RenderingTaskEventArgs> ItemAdded;
		public event EventHandler<RenderingTaskEventArgs> ItemIndexChanged;
		public event EventHandler<RenderingTaskEventArgs> BeforeItemProcessingStarted;
		public event EventHandler<RenderingTaskEventArgs> AfterItemProcessingFinished;
		public event EventHandler<EventArgs> ThreadStarted;
		public event EventHandler<EventArgs> QueueEmpty;
		public event EventHandler<EventArgs> ThreadStopped;
		
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
		protected virtual void OnThreadStarted()
		{
			if (ThreadStarted != null)
				ThreadStarted(this, EventArgs.Empty);
		}
		protected virtual void OnThreadStopped()
		{
			if (ThreadStopped != null)
				ThreadStopped(this, EventArgs.Empty);
		}
		protected virtual void OnQueueEmpty()
		{
			if (QueueEmpty != null)
				QueueEmpty(this, EventArgs.Empty);
		}

		protected virtual void OnQueueProgressMessageReport(string source, string destination, double progress, string status)
		{
			if (QueueProgressMessageReport != null)
			{
				QueueProgressMessageReport(source, destination, progress, status);
			}
		}
		
		public void Add(Stage stage, string source, int prescale, string destination, string fileType)
		{
			lock (mQueue)
			{
				stage.ProgressMessageReport += HandleStageProgressMessageReport;
				RenderingTask rt = new RenderingTask(stage, source, prescale, destination, fileType);
				mQueue.Add(rt);
				OnItemAdded(rt);
			}
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
			if (mCancelItem == true)
			{
				throw new UserCancelException();
			}
			if (mCancelAllItems == true)
			{
				throw new UserCancelException();
			}
			
			OnQueueProgressMessageReport(rt.Source, rt.Destination, e.Progress, e.Status);
		}
		
		public bool StepDown(RenderingTask item)
		{
			lock (item)
			{
				int inx = mQueue.IndexOf(item);
				if (inx >= 0 && inx < mQueue.Count - 1)
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
		}

		public bool StepUp(RenderingTask item)
		{
			lock (item)
			{
				int inx = mQueue.IndexOf(item);
				if (inx > 0 && inx < mQueue.Count)
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
		}
		
		public int IndexOf(RenderingTask item)
		{
			return mQueue.IndexOf(item);
		}
		
		public void Remove(RenderingTask item)
		{
			lock (item)
			{
				if (mQueue.Contains(item))
				{
					item.Stage.ProgressMessageReport -= HandleStageProgressMessageReport;
					mQueue.Remove(item);
					OnItemRemoved(item);
				}
			}
		}
		
		private volatile bool mThreadStopped = false;
		
		public void StopThread()
		{
			mThreadStopped = true;
		}
		
		private void ProcessingThread()
		{
			mCancelItem = false;
			mCancelAllItems = false;
			
			OnThreadStarted();
			while (!mThreadStopped)
			{
				if (mQueue.Count > 0)
				{
					try
					{
						mIsProcessingItem = true;
						RenderingTask cur_task = mQueue[0];
						lock (cur_task)
						{
							if (mQueue.Contains(cur_task))
							{
								lock (mQueue)
								{
									mInProgress = cur_task;
									mQueue.Remove(cur_task);
									OnItemRemoved(cur_task);
								}
							}
						}
						
						OnBeforeItemProcessingStarted(mInProgress);
						
						if (!mInProgress.Stage.LoadImage(mInProgress.Source, mInProgress.Prescale))
							throw new UserCancelException();

						mInProgress.Stage.Process();
						OnQueueProgressMessageReport(mInProgress.Source, mInProgress.Destination, 1, "Saving image to file...");
						OnItemRendering(mInProgress);
						OnQueueProgressMessageReport(mInProgress.Source, mInProgress.Destination, 1, "Image saved");

						mInProgress.Stage.ProgressMessageReport -= HandleStageProgressMessageReport;
						
						OnAfterItemProcessingFinished(mInProgress);
					}
					catch (UserCancelException exp)
					{
#if DEBUG
						Console.WriteLine("User has sent " + exp.GetType().Name + " exception.");
#endif
					}
					finally 
					{
						mIsProcessingItem = false;
						mCancelItem = false;
						mCancelAllItems = false;
						if (mQueue.Count == 0)
							OnQueueEmpty();
					}
				}
				else
				{
					Thread.Sleep(30);
				}
			}
			
			OnThreadStopped();
			mWorkingThread = null;
		}
		
		private volatile bool mIsProcessingItem = false;
		
		public bool IsProcessingItem
		{
			get 
			{ 
				return mIsProcessingItem;
			}
		}
		
		public void StartThread()
		{
			if (mWorkingThread == null)
			{
				mWorkingThread = new Thread(ProcessingThread);
				mWorkingThread.Start();
			}
		}
		public void CancelAllItems()
		{
			for (int i = 0; i < mQueue.Count; i++)
				OnItemRemoved(mQueue[i]);
			mQueue.Clear();

			mCancelAllItems = true;
		}
		public void CancelItem()
		{
			mCancelItem = true;
		}
		public int Count { get { return mQueue.Count; } }
	}
}

