using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace CatEye.UI.Gtk
{
	class RemoteControlService
	{
		private static string PasswordCode = "\x001\x002LetTheForceBeWithYou\x002\x001";
		
		private string mAddress = "localhost";
		private int mPort = 21985;
		
		private volatile TcpListener mListener;
		private Thread mListeningThread;
		private volatile bool mListeningThreadStopPending = false;
		
		private TcpClient TryConnectClient()
		{
			try
			{
				TcpClient client = new TcpClient();
				client.Connect(mAddress, mPort);
				if (client != null && client.Connected)
				{
#if DEBUG
					Console.WriteLine("[M] Client connected");
#endif
					return client;
				}
				else
				{
					return null;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
		
		private void EncodeAndSendString(NetworkStream stream, string str)
		{
			while (!stream.CanWrite) { Thread.Sleep(10); }
			byte[] text_buf = Encoding.UTF8.GetBytes(str);
			stream.Write(text_buf, 0, text_buf.Length);
			stream.Flush();
		}
		
		private void StartServer()
		{
#if DEBUG
			Console.WriteLine("[M] Starting server");
#endif
			if (mListener != null && mListener.Server.IsBound) 
			{
#if DEBUG
				Console.WriteLine("[M] The server is already running");
#endif
				return;
			}
			
			TcpClient client;
			bool client_opened = false;
			mListener = new TcpListener(mPort);
			
			mListeningThread = new Thread(delegate () {
				mListener.Start();
#if DEBUG
				Console.WriteLine("[S] Server started");
#endif
				try
				{
					while (!mListeningThreadStopPending)
					{
						while (!mListener.Pending()) 
						{
							Thread.Sleep(10);
							if (mListeningThreadStopPending) 
								throw new CatEye.Core.UserCancelException();
						}
						client = mListener.AcceptTcpClient();
						client_opened = true;
#if DEBUG
						Console.WriteLine("[S] Client connected.");
#endif						
						NetworkStream stream = client.GetStream();

						if (stream.CanRead)
						{
							byte[] text_buf = new byte[65536];
							StringBuilder completeText = new StringBuilder();
							int bytesRead = 0;
						
							// Reading string
							stream.ReadTimeout = 1000;	// Wait a second to connect...
#if DEBUG
							Console.WriteLine("[S] Reading the message. Wait a second...");
#endif						
							try
							{
								do
								{
									bytesRead = stream.Read(text_buf, 0, text_buf.Length);
									completeText.Append(Encoding.UTF8.GetString(text_buf, 0, bytesRead));
								}
								while (stream.DataAvailable);
							}
							catch (Exception
#if DEBUG
								e
#endif						
								)
							{
#if DEBUG
								Console.WriteLine("[S] Exception during reading: " + e.Message);
#endif						
							}
						
							string text = completeText.ToString();
							
							// Checking the result
							if (text.Length >= PasswordCode.Length &&
								text.Substring(0, PasswordCode.Length) == PasswordCode)
							{
								// Handling string
								string s = text.Substring(PasswordCode.Length);
								string[] args;
								string command;
								if (s != "")
								{
									args = s.Split('\x000');
									List<string> argslist = new List<string>(args);
									command = argslist[0];
									argslist.RemoveAt(0);
									args = argslist.ToArray();
								}
								else
								{
									command = null;
									args = new string[] {};
								}
								
								OnRemoteCommandReceived(command, args);
							}
							else 
							{
								Console.WriteLine("[S] Unsupported message \"" + text + "\"");
							}
						}
						client.Close();
						client_opened = false;
					}
				}
				catch (SocketException sex)
				{
					// Sorry, it's interrupted, yeah...
#if DEBUG
					Console.WriteLine("[S] Socket exception occured: " + sex.Message + "\n" +
						sex.StackTrace);
#endif	
				}
				catch (CatEye.Core.UserCancelException ucex)
				{
#if DEBUG
					Console.WriteLine("[S] User interrupted the server listening cycle");
#endif	
				}
				finally 
				{
					if (client_opened) client.Close();
				}
#if DEBUG
				Console.WriteLine("[S] Server stopped");
#endif				
			});
			mListeningThread.Start();
#if DEBUG
			Console.WriteLine("[M] Waiting for the server socket to get bound...");
#endif			
			do
			{
				Thread.Sleep(10);
			}
			while (!mListener.Server.IsBound);
#if DEBUG
			Console.WriteLine("[M] The socket is bound");
#endif			
		}
		
		public RemoteControlService()
		{

		}
		
		public RemoteControlService(string address, int port)
		{
			mAddress = address; mPort = port;
		}
		
		/// <summary>
		/// Starts server if it's not started in other process. 
		/// </summary>
		/// <returns>
		/// <c>True</c> if server is actually started.
		/// <c>False</c> if server had been started before.
		/// </returns>
		public bool Start()
		{
			bool result;
			TcpClient client = TryConnectClient();
			if (client == null)
			{
				StartServer();
				client = TryConnectClient();
				result = true;
			}
			else
				result = false;
			
			return result;
		}
		
		public bool SendCommand(string command, string[] arguments)
		{
			TcpClient client = TryConnectClient();
			if (client == null)
			{
				return false;
			}
			else
			{
				InternalSendCommand(client, command, arguments);
				return true;
			}
		}

		private void InternalSendCommand(TcpClient client, string command, string[] args)
		{
			string str;
			List<string> cmd_and_args = new List<string>();
			cmd_and_args.Add(command);
			cmd_and_args.AddRange(args);
			
			if (cmd_and_args.Count > 0)
			{
				StringBuilder sb = new StringBuilder(cmd_and_args[0]);
				for (int i = 1; i < cmd_and_args.Count; i++)
				{
					sb.Append("\x000" + cmd_and_args[i]);
				}
				str = sb.ToString();
			}
			else
				str = "";
#if DEBUG
			Console.WriteLine("[M] Sending command \"" + command + "\"");
#endif
			string str2 = PasswordCode + str;
			EncodeAndSendString(client.GetStream(), str2);
#if DEBUG
			Console.WriteLine("[M] Command \"" + command + "\" has sent");
#endif
		}
		
		public event EventHandler<RemoteCommandEventArgs> RemoteCommandReceived;
		
		protected virtual void OnRemoteCommandReceived(string command, string[] args)
		{
			if (RemoteCommandReceived != null)
				RemoteCommandReceived(this, new RemoteCommandEventArgs(command, args));
		}
		
		public void Stop()
		{
			if (mListener != null)
			{
				mListener.Stop();
			}
			mListeningThreadStopPending = true;
		}
	}
}

