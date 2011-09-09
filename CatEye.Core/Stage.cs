using System;
using System.Xml;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CatEye.Core
{
	public class Stage
	{
		//public static int PreScale = 1;  // TODO: option
		
#region Private data
		private bool mCancelLoadingPending = false;
		private bool mCancelProcessingPending = false;
		private IBitmapCore mSourceImage = null;
		private IBitmapCore mCurrentImage = null;
		private List<StageOperationParameters> mStageQueue;
#endregion
#region Factory functions
		private BitmapCoreFactory mBitmapCoreFactory;
		private StageOperationFactory mStageOperationFactory;
		private StageOperationParametersFactory mStageOperationParametersFactory;
#endregion
#region Events
		public event EventHandler<ReportStageProgressMessageEventArgs> ProgressMessageReport;
		public event EventHandler<StageOperationParametersEventArgs> ItemRemoved;
		public event EventHandler<StageOperationParametersEventArgs> ItemAdded;
		public event EventHandler<StageOperationParametersEventArgs> ItemIndexChanged;
		public event EventHandler<StageOperationParametersEventArgs> ItemChanged;
		public event EventHandler<EventArgs> ImageLoadingCompleted;
		public event EventHandler<EventArgs> ImageLoadingCancelled;
		public event EventHandler<EventArgs> ImageUpdatingCompleted;
#endregion
#region Protected factory methods
		protected StageOperation CallStageOperationFactory(StageOperationParameters parameters)
		{
			return mStageOperationFactory(parameters);
		}
		protected StageOperationParameters CallStageOperationParametersFactory(string id)
		{
			return mStageOperationParametersFactory(id);
		}
#endregion
#region Protected event callers
		protected virtual void OnProgressMessageReport(bool showProgressBar, double progress, string status, bool update)
		{
			if (ProgressMessageReport != null)
			{
				ReportStageProgressMessageEventArgs ea = new ReportStageProgressMessageEventArgs(showProgressBar, 
					progress, status, update);
				ProgressMessageReport(this, ea);
				// Sleeping to wait for UI updating
			}
		}
		
		protected virtual void OnItemAdded(StageOperationParameters item)
		{
			if (ItemAdded != null) 
				ItemAdded(this, new StageOperationParametersEventArgs(item));
		}
		protected virtual void OnItemRemoved(StageOperationParameters item)
		{
			if (ItemRemoved != null) 
				ItemRemoved(this, new StageOperationParametersEventArgs(item));
		}
		protected virtual void OnItemIndexChanged(StageOperationParameters item)
		{
			if (ItemIndexChanged != null)
				ItemIndexChanged(this, new StageOperationParametersEventArgs(item));
		}
		protected virtual void OnItemChanged(StageOperationParameters item)
		{
			if (ItemChanged != null)
				ItemChanged(this, new StageOperationParametersEventArgs(item));
		}
		protected virtual void OnImageUpdatingCompleted()
		{
			if (ImageUpdatingCompleted != null)
				ImageUpdatingCompleted(this, EventArgs.Empty);
		}		
#endregion
#region Public properties
		public ReadOnlyCollection<StageOperationParameters> StageQueue 
		{ 
			get 
			{ 
				return mStageQueue.AsReadOnly(); 
			}
		}
		public virtual IBitmapCore SourceImage
		{
			get { return mSourceImage; }
			protected set 
			{
				mCurrentImage = null;
				mSourceImage = value;
			}
		}
		public virtual IBitmapCore CurrentImage
		{
			get { return mCurrentImage; }
			protected set 
			{
				mCurrentImage = value;
			}
		}
		public StageOperationFactory StageOperationFactory
		{
			get { return mStageOperationFactory; }
			set { mStageOperationFactory = value; }
		}
		public StageOperationParametersFactory StageOperationParametersFactory
		{
			get { return mStageOperationParametersFactory; }
			set { mStageOperationParametersFactory = value; }
		}
		
		public bool CancelProcessingPending
		{
			get { return mCancelProcessingPending; }
			protected set { mCancelProcessingPending = value; }
		}
		
		public bool CancelLoadingPending
		{
			get { return mCancelLoadingPending; }
		}
#endregion

		public virtual void LoadStage(string filename)
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.Load(filename);

			DeserializeFromXML(xdoc.ChildNodes[1]);
		}

		public virtual void LoadStageFromString(string data)
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.InnerXml = data;

			DeserializeFromXML(xdoc.ChildNodes[1]);
		}
		
		public virtual void SaveStage(string filename)
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", null, null));
			xdoc.AppendChild(SerializeToXML(xdoc));
			xdoc.Save(filename);
		}
		
		public virtual string SaveStageToString()
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", null, null));
			xdoc.AppendChild(SerializeToXML(xdoc));
#if DEBUG
			Console.WriteLine("Saved stage code: " + xdoc.InnerXml);
#endif
			return xdoc.InnerXml;
		}
		
		public virtual void CancelProcessing()
		{
			mCancelProcessingPending = true;
		}
		
		public void CancelAll()
		{
			CancelLoading();
			CancelProcessing();
		}

		public int IndexOf(StageOperationParameters so)
		{
			return mStageQueue.IndexOf(so);
		}
		
		public void Process()
		{
			DoProcess();
		}
		
		protected virtual void DoProcess()
		{
			if (mSourceImage != null)
			{
				try
				{
					mCancelProcessingPending = false;

					mCurrentImage = (IBitmapCore)mSourceImage.Clone();
					
					// Making the list of stage operations to apply
					List<StageOperation> operationsToApply = new List<StageOperation>();
					List<double> efforts = new List<double>();
					double full_efforts = 0;

					for (int i = 0; i < mStageQueue.Count; i++)
					{
						// Don't add inactives
						if (mStageQueue[i].Active == false) continue;
						
						StageOperation newOperation = CallStageOperationFactory(mStageQueue[i]);
						operationsToApply.Add(newOperation);
						efforts.Add(newOperation.CalculateEfforts(mCurrentImage));
						full_efforts += efforts[efforts.Count - 1];
						
						newOperation.ReportProgress += delegate(object sender, ReportStageOperationProgressEventArgs e) {
							double cur_eff = 0;
							int j = 0;
							while (operationsToApply[j] != (StageOperation)sender)
							{
								cur_eff += efforts[j];
								j++;
							}
							cur_eff += e.Progress * efforts[j];
							string desc = StageOperationDescriptionAttribute.GetSOName(sender.GetType());
							
							OnProgressMessageReport(true,
								cur_eff / full_efforts, 
								"" + (j + 1) + " of " + efforts.Count +  ": " + desc + "...", true);
							
							if (mCancelProcessingPending)
								e.Cancel = true;
						};

					}
					
					// Executing
					for (int k = 0; k < operationsToApply.Count; k++)
					{
						operationsToApply[k].OnDo(mCurrentImage);
					}
					OnImageUpdatingCompleted();
				}
				catch (UserCancelException ex)
				{
					throw ex;
				}
			}
		}

		public virtual bool LoadImage(string filename, int downscale_by)
		{
			CancelProcessing();
			
			IBitmapCore ibc = ProcessRawFromDCRaw(filename, downscale_by);
			if (ibc != null)
			{
				if (ImageLoadingCompleted != null) ImageLoadingCompleted(this, EventArgs.Empty);
				SourceImage = ibc;
				return true;
			}
			else
			{
				if (ImageLoadingCancelled != null) ImageLoadingCancelled(this, EventArgs.Empty);
				SourceImage = ibc;
				return false;
			}
		}
		
		void HandleItemChanged(object sender, EventArgs e)
		{
			OnItemChanged((StageOperationParameters)sender);			
		}

		public Stage(StageOperationFactory stageOperationFactory, 
			StageOperationParametersFactory stageOperationParametersFactory,
			BitmapCoreFactory bitmapCoreFactory)
		{
			mStageQueue = new List<StageOperationParameters>();
			mStageOperationFactory = stageOperationFactory;
			mStageOperationParametersFactory = stageOperationParametersFactory;
			mBitmapCoreFactory = bitmapCoreFactory;
		}
		
		public Stage(BitmapCoreFactory bitmapCoreFactory)
		{
			mBitmapCoreFactory = bitmapCoreFactory;
		}
		
		public Stage()
		{
		}
		
		public void Add(StageOperationParameters newItem)
		{
			mStageQueue.Add(newItem);
			newItem.Changed += HandleItemChanged;
			OnItemAdded(newItem);
		}

		public void Remove(StageOperationParameters item)
		{
			mStageQueue.Remove(item);
			item.Changed -= HandleItemChanged;
			OnItemRemoved(item);
		}

		public void StepDown(StageOperationParameters item)
		{
			int index = mStageQueue.IndexOf(item);
			if (index < mStageQueue.Count - 1)
			{
				mStageQueue.Remove(item);
				mStageQueue.Insert(index + 1, item);
				OnItemIndexChanged(item);
			}
		}

		public void StepUp(StageOperationParameters item)
		{
			int index = mStageQueue.IndexOf(item);
			if (index > 0)
			{
				mStageQueue.Remove(item);
				mStageQueue.Insert(index - 1, item);
				OnItemIndexChanged(item);
			}
		}
		
		public void Clear()
		{
			while (mStageQueue.Count > 0)
				Remove(mStageQueue[0]);
		}
		
		public XmlNode SerializeToXML(XmlDocument xdoc)
		{
			XmlNode xn = xdoc.CreateElement("Stage");
			for (int i = 0; i < mStageQueue.Count; i++)
			{
				XmlNode ch = mStageQueue[i].SerializeToXML(xdoc);
				xn.AppendChild(ch);
			}
			return xn;
		}

		protected virtual void OnItemDeserialized(StageOperationParameters sop)
		{
		}
		
		public void DeserializeFromXML(XmlNode xn)
		{
			if (xn.Name != "Stage")
				throw new IncorrectNodeException("Node isn't a Stage node");
			
			List<StageOperationParameters> sops = new List<StageOperationParameters>();
			
			for (int i = 0; i < xn.ChildNodes.Count; i++)
			{
				XmlNode ch = xn.ChildNodes[i];
				if (ch.Name == "StageOperationParameters")
				{
					if (ch.Attributes["ID"] == null)
						throw new IncorrectNodeException("StageOperationParameters node doesn't contain ID");
					
					StageOperationParameters sop = CallStageOperationParametersFactory(ch.Attributes["ID"].Value);
					 
					// Deserializing stage operation parameters
					sop.DeserializeFromXML(ch);
					sops.Add(sop);
					
					OnItemDeserialized(sop);
				}
			}
			
			Clear();
			
			for (int i = 0; i < sops.Count; i++)
			{
				Add(sops[i]);
			}
			
		}

		public void CancelLoading()
		{
			mCancelLoadingPending = true;
		}
				
		/// <summary>
		/// Launching dcraw to process the raw file, loads the result into a memory stream
		/// </summary>
		/// <returns>
		/// A stream to read the decoded PPM data from. 
		/// Should be closed by user.
		/// </returns>
		protected IBitmapCore ProcessRawFromDCRaw(string filename, int downscale_by)
		{
#if DEBUG
			GC.Collect();
			Console.WriteLine("Starting raw processing. I'm using " + System.GC.GetTotalMemory(true) / 1024 + " Kbytes of memory now");
#endif
			mCancelLoadingPending = false;
			OnProgressMessageReport(false, 0, "Waiting for dcraw...", false);
			
			System.Diagnostics.Process prc = DCRawConnection.CreateDCRawProcess("-4 -c \"" + filename.Replace("\"", "\\\"") + "\"");
			try
			{
				if (prc.Start())
				{
					int cnt = 0;
					int readed = 0;
					int readed_all = 0;
					System.IO.MemoryStream ms = new System.IO.MemoryStream();
					try
					{
						byte[] buf = new byte[1024 * 4];
						do
						{
							cnt++;
							if (cnt % 100 == 0)
							{
								OnProgressMessageReport(false, 0, "Reading data: " + (readed_all / (1024 * 1024)) + " M", false);
								if (mCancelLoadingPending)
								{
#if DEBUG
									Console.WriteLine("Data processing cancelled. ProcessRawFromDCRaw is returning null.");
#endif								
									return null;
								}
							}
							readed = prc.StandardOutput.BaseStream.Read(buf, 0, buf.Length);
							ms.Write(buf, 0, readed);
							readed_all += readed;
						}
						while (readed > 0);
			
						OnProgressMessageReport(false, 0, "Data reading complete.", false);
						ms.Seek(0, System.IO.SeekOrigin.Begin);
						
#if DEBUG
						Console.WriteLine("Loading raw from " + filename);
#endif								
						return LoadRaw(ms, downscale_by);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Exception occured during loading process: " + ex.Message);
						
						//Console.WriteLine("DCRaw error output: " + prc.StandardError.ReadToEnd());
						return null;
					}
					finally
					{
#if DEBUG
						Console.WriteLine("Closing and disposing the memory stream");
#endif
						ms.Close();
						ms.Dispose();
					}
				}
				else
				{
#if DEBUG
					Console.WriteLine("The DCRaw process isn't started.");
#endif
					return null;
				}
			}
			finally
			{
				prc.StandardOutput.BaseStream.Dispose();
				prc.StandardOutput.Dispose();
				prc.WaitForExit(-1);	// R.I.P.
				prc.Close();
#if DEBUG
				GC.Collect();
				Console.WriteLine("Closing and disposing the dcraw process. I'm using " + System.GC.GetTotalMemory(true) / 1024 + " Kbytes of memory now");
#endif
			}
		}
		
		public IBitmapCore LoadRaw(System.IO.MemoryStream stream, int downscale_by)
		{
			mCancelLoadingPending = false;
			PPMLoader ppl = PPMLoader.FromStream(stream, delegate (double progress) {
				OnProgressMessageReport(true, progress / 3, "1 of 3: Parsing image...", false);
				return !mCancelLoadingPending;
			});
	
			if (ppl == null)
			{
				return null;
			}
			else
			{
				if (downscale_by != 1)		
				{
					bool dsres = ppl.Downscale(downscale_by, delegate (double progress) {
						OnProgressMessageReport(true, 0.333 + progress / 3, "2 of 3: Downscaling...", false);
						return !mCancelLoadingPending;
					});
					
					if (dsres == false)
					{
						ppl = null;
						return null;
					}
				}
				
				return mBitmapCoreFactory(ppl, 
					delegate (double progress) {
						OnProgressMessageReport(true, 0.666 + progress / 3, "3 of 3: Converting to editable format...", false);
						return !mCancelLoadingPending;
					}
				);
				
			}
		}
		
	}
}
