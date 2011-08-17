using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using Gtk;
using CatEye.Core;
using CatEye.UI.Base;
using CatEye.UI.Gtk.Widgets;

namespace CatEye.UI.Gtk
{
	class RemotingObject : MarshalByRefObject
	{
		public static RemotingObject rob;
		public static readonly string ChannelPort = "CatEye";

		public RenderingQueueWindow rqwin;
		public RenderingQueue rq;
		
		public static string URI = "RemotingObject";
		public RemotingObject()
		{
			rq = new RenderingQueue();
			rq.ItemRendering += HandleRqItemRendering;
			
			rqwin = new RenderingQueueWindow(rq);
			rqwin.Visible = true;
			
		}
		public void AddToQueue(Stage stage, string src, string dest, string dest_type)
		{
			Stage stg = new Stage(MainClass.StageOperationFactory, 
				MainClass.StageOperationParametersFactoryFromID,
				MainClass.ImageLoader);
			
			for (int i = 0; i < stage.StageQueue.Length; i++)
			{
				stg.Add((StageOperationParameters)stage.StageQueue[i].Clone());
			}
			
			rq.Add(stg, src, dest, dest_type);
		}
		
		void HandleRqItemRendering (object sender, RenderingTaskEventArgs e)
		{
			FloatBitmapGtk renderDest = (FloatBitmapGtk)e.Target.Stage.CurrentImage;
			
			// Drawing to pixbuf and saving to file
			using (Gdk.Pixbuf rp = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 8, renderDest.Width, renderDest.Height))
			{
	
				renderDest.DrawToPixbuf(rp, 
					delegate (double progress) {
					
						// TODO: report progress via RenderingTask (needed to create an event)
						//rpw.SetStatusAndProgress(progress, "Saving image...");
						while (Application.EventsPending()) Application.RunIteration();
						return true;
					}
				);
			
				// TODO Can't be used currently cause of buggy Gtk#
				//rp.Savev(filename, type, new string[] { "quality" }, new string[] { "95" });
		
				rp.Save(e.Target.Destination, e.Target.FileType);
			}
		}
		
		/// <summary>
		/// Assures that the queue service is running.
		/// </summary> 
		public static void AssureQueueServiceIsRunning()
		{
			if (rob != null) return;
			
			System.Threading.Thread thr = new System.Threading.Thread(delegate () {
				// Running queue server
				AppDomain newDomain = AppDomain.CreateDomain("new_domain");
				
				newDomain.ExecuteAssembly(
					System.Reflection.Assembly.GetEntryAssembly().CodeBase, 
					AppDomain.CurrentDomain.Evidence,
					new string[] { "--queue" } 
				);
			});
			thr.Start();
			
			System.Threading.Thread.Sleep(1000);	// TODO This is a dirty hack to assure the assemply is loaded! Change it to something reasonable.
			
			IpcClientChannel icc = new IpcClientChannel();
			ChannelServices.RegisterChannel(icc, false);
			string objectUrl = "ipc://" + ChannelPort + "/" + RemotingObject.URI;
			
			RemotingConfiguration.RegisterWellKnownClientType(
				typeof(RemotingObject), objectUrl);

			try
			{
				rob = new RemotingObject();
				Console.WriteLine("Remoting object is created");
			}
			catch (System.Runtime.Remoting.RemotingException ex)
			{
				Console.WriteLine("Can't connect to the server.");
				
			}

		}
		
		/// <summary>
		/// Runs the queue service or connects to existing one.
		/// </summary>
		/// <returns>
		/// True if new server started. False otherwise.
		/// </returns>
		public static bool RunQueueServiceOrConnectToIt()
		{
			if (rob != null) return false;
				
			// Checking if the service is already started
			bool server_is_running = false;
			
			try
			{
				IpcServerChannel ise = new IpcServerChannel(ChannelPort);
			}
			catch (Exception)
			{
				// I can't create server. 
				// I'll be optimistic here and guess that the server is already started

				server_is_running = true;
			}
			
			if (server_is_running)
			{
				IpcClientChannel icc = new IpcClientChannel();
				ChannelServices.RegisterChannel(icc, false);
				string objectUrl = "ipc://" + ChannelPort + "/" + RemotingObject.URI;
				
				RemotingConfiguration.RegisterWellKnownClientType(
					typeof(RemotingObject), objectUrl);
	
				try
				{
					rob = new RemotingObject();
					Console.WriteLine("Remoting object is created");
				}
				catch (System.Runtime.Remoting.RemotingException ex)
				{
					Console.WriteLine("Can't connect to the server.");
					
				}
			}
			
			else
			{
				Console.WriteLine("No server found. Creating my own one.");
				
				RemotingConfiguration.RegisterWellKnownServiceType(
					typeof(RemotingObject), 
					RemotingObject.URI, 
					WellKnownObjectMode.Singleton);

				try
				{
					rob = new RemotingObject();
					Console.WriteLine("Remoting object is created");
				}
				catch (System.Runtime.Remoting.RemotingException ex)
				{
					Console.WriteLine("Can't connect to the server.");
					
				}
				
			}
			
			return !server_is_running;
		}
		
	}
}


