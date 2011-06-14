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
		private string mSource, mDestination;
		
		public Stage Stage { get { return mStage; } }
		public string Source { get { return mSource; } }
		public string Destination { get { return mDestination; } }
		
		public RenderingTask(Stage stage, string source, string destination)
		{
			mStage = stage; mSource = source; mDestination = destination;
		}
	}
	
	public class RenderingQueue
	{
		private List<RenderingTask> mQueue;
		private RenderingTask mInProgress = null;
		
		private Thread mWorkingThread = null;
		
		public event EventHandler<RenderingTaskEventArgs> ItemRemoved;
		public event EventHandler<RenderingTaskEventArgs> ItemAdded;
		public event EventHandler<RenderingTaskEventArgs> ItemIndexChanged;
		public event QueueProgressMessageReporter QueueProgressMessageReport; 

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
		protected virtual void OnQueueProgressMessageReport(string source, string destination, double progress, string status)
		{
			if (QueueProgressMessageReport != null)
				QueueProgressMessageReport(source, destination, progress, status);
		}
		
		public void Add(Stage stage, string source, string destination)
		{
			stage.ProgressMessageReport += HandleStageProgressMessageReport;
			RenderingTask rt = new RenderingTask(stage, source, destination);
			mQueue.Add(rt);
			OnItemAdded(rt);
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
			RenderingTask rt = FindRenderingTaskByStage((Stage)sender);
			
			OnQueueProgressMessageReport(rt.Source, rt.Destination, e.Progress, e.Status);
		}

		public void Remove(RenderingTask item)
		{
			item.Stage.ProgressMessageReport -= HandleStageProgressMessageReport;
			mQueue.Remove(item);
			OnItemRemoved(item);
		}
		
		private void ProcessingThread()
		{
			while (mQueue.Count > 0)
			{
				mInProgress = mQueue[0];
				mQueue.RemoveAt(0);
				
				mInProgress.Stage.LoadImage(mInProgress.Source, 2);
				mInProgress.Stage.Process();
//				mInProgress.Stage.
				
				mInProgress.Stage.ProgressMessageReport -= HandleStageProgressMessageReport;
				OnItemRemoved(mInProgress);
			}
		
		}
		
		public void Process()
		{
			if (mWorkingThread == null || !mWorkingThread.IsAlive)
			{
				mWorkingThread = new Thread(ProcessingThread);
				mWorkingThread.Start();
			}
			
		}
	}
}

