using System;
using System.Xml;
using System.Collections.Generic;

namespace CatEye.Core
{
	public delegate IBitmapCore ImageLoader(PPMLoader ppl, ProgressReporter callback);
	public class StageOperationParametersEventArgs : EventArgs
	{
		StageOperationParameters _Target;
		public StageOperationParameters Target { get { return _Target; } }
		public StageOperationParametersEventArgs(StageOperationParameters target)
		{
			_Target = target;
		}
	}
	public delegate void ProgressMessageReporter(bool showProgressBar, double progress, string status, bool update);
	
	public class Stage
	{
		private bool mCancelLoadingPending = false;
		private ImageLoader mImageLoader;
		
		protected StageOperationFactory _StageOperationFactory;
		protected StageOperationParametersFactoryFromID _StageOperationParametersFactoryFromID;
		protected List<StageOperationParameters> _StageQueue;
		
		public StageOperationParameters[] StageQueue { get { return _StageQueue.ToArray(); } }
		
		public event ProgressMessageReporter ProgressMessageReport;
		public event EventHandler<StageOperationParametersEventArgs> ItemRemoved;
		public event EventHandler<StageOperationParametersEventArgs> ItemAdded;
		public event EventHandler<StageOperationParametersEventArgs> ItemIndexChanged;
		public event EventHandler<StageOperationParametersEventArgs> ItemChanged;
		
		void HandleItemChanged(object sender, EventArgs e)
		{
			OnItemChanged((StageOperationParameters)sender);			
		}

		public Stage(StageOperationFactory stageOperationFactory, 
			StageOperationParametersFactoryFromID stageOperationParametersFactoryFromID,
			ImageLoader imageLoader)
		{
			_StageQueue = new List<StageOperationParameters>();
			_StageOperationFactory = stageOperationFactory;
			_StageOperationParametersFactoryFromID = stageOperationParametersFactoryFromID;
			mImageLoader = imageLoader;
		}
		
		protected virtual void OnProgressMessageReport(bool showProgressBar, double progress, string status, bool update)
		{
			if (ProgressMessageReport != null)
				ProgressMessageReport(showProgressBar, progress, status, update);
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
		
		public void ApplyAllOperations(IBitmapCore hdp)
		{
			for (int j = 0; j < _StageQueue.Count; j++)
			{
				if (_StageQueue[j].Active)
				{
					StageOperation so = _StageOperationFactory(_StageQueue[j]);
					so.OnDo(hdp);
				}
			}
		}
		
		public double CalculateAllEfforts(IBitmapCore hdp)
		{
			double res = 0;
			for (int j = 0; j < _StageQueue.Count; j++)
			{
				if (_StageQueue[j].Active)
				{
					StageOperation so = _StageOperationFactory(_StageQueue[j]);
					res += so.CalculateEfforts(hdp);
				}
			}
			return res;
		}

		public void Add(StageOperationParameters newItem)
		{
			_StageQueue.Add(newItem);
			newItem.Changed += HandleItemChanged;
			OnItemAdded(newItem);
		}

		public void Remove(StageOperationParameters item)
		{
			_StageQueue.Remove(item);
			item.Changed -= HandleItemChanged;
			OnItemRemoved(item);
		}

		public void StepDown(StageOperationParameters item)
		{
			int index = _StageQueue.IndexOf(item);
			if (index < _StageQueue.Count - 1)
			{
				_StageQueue.Remove(item);
				_StageQueue.Insert(index + 1, item);
				OnItemIndexChanged(item);
			}
		}

		public void StepUp(StageOperationParameters item)
		{
			int index = _StageQueue.IndexOf(item);
			if (index > 0)
			{
				_StageQueue.Remove(item);
				_StageQueue.Insert(index - 1, item);
				OnItemIndexChanged(item);
			}
		}
		
		public void Clear()
		{
			while (_StageQueue.Count > 0)
				Remove(_StageQueue[0]);
		}
		
		public XmlNode SerializeToXML(XmlDocument xdoc)
		{
			XmlNode xn = xdoc.CreateElement("Stage");
			for (int i = 0; i < _StageQueue.Count; i++)
			{
				XmlNode ch = _StageQueue[i].SerializeToXML(xdoc);
				xn.AppendChild(ch);
			}
			return xn;
		}

		/*
		protected Type FindTypeWithStageOperationIDEqualTo(Type[] types, string id)
		{
			for (int i = 0; i < types.Length; i++)
			{
				object[] attrs = types[i].GetCustomAttributes(typeof(StageOperationIDAttribute), true);
				if (attrs.Length == 0)
				{
					continue;
				}
				if (((StageOperationIDAttribute)attrs[0]).ID == id)
					return types[i];
			}
			return null;
		}
		*/
		
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
					
					StageOperationParameters sop = _StageOperationParametersFactoryFromID(ch.Attributes["ID"].Value);
					 
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
						
						return LoadRaw(ms, downscale_by);
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
				
				return mImageLoader(ppl, 
					delegate (double progress) {
						OnProgressMessageReport(true, 0.666 + progress / 3, "3 of 3: Converting to editable format...", false);
						return !mCancelLoadingPending;
					}
				);
				
			}
		}
		
	}
}
