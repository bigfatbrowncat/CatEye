using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace CatEye.UI.Gtk
{
	class RemoteControlService
	{
		private static string PasswordCode = "\x001\x002LetTheForceBeWithYou\x002\x001";
		private static string PingCode = "\x001\x002IsThereAnybodyThere?\x002\x001";
		
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
		
		private void SendPing(NetworkStream stream)
		{
			string str = PingCode;
			EncodeAndSendString(stream, str);
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
						client = mListener.AcceptTcpClient();
						if (mListeningThreadStopPending) break;
#if DEBUG
						Console.WriteLine("[S] Client connected. Reading the message...");
#endif						
						NetworkStream stream = client.GetStream();

						if (stream.CanRead)
						{
							byte[] text_buf = new byte[65536];
							StringBuilder completeText = new StringBuilder();
							int bytesRead = 0;
						
							// Reading string
							do
							{
								bytesRead = stream.Read(text_buf, 0, text_buf.Length);
								completeText.Append(Encoding.UTF8.GetString(text_buf, 0, bytesRead));
							}
							while (stream.DataAvailable);
						
							string text = completeText.ToString();
							// Checking the result
							if (text == PingCode)
							{
#if DEBUG
								Console.WriteLine("[S] Ping ok");
#endif
							}
							else if (text.Length >= PasswordCode.Length &&
								text.Substring(0, PasswordCode.Length) == PasswordCode)
							{
								// Handling string
								string s = text.Substring(PasswordCode.Length);
								string[] args;
								if (s != "")
									args = s.Split('\x000');
								else
									args = new string[] {};
								
								OnRemoteCommandReceived(args);
							}
							else 
							{
								Console.WriteLine("[S] Unsupported message \"" + text + "\"");
							}
						}
						client.Close();
					}
				}
				catch (SocketException sex)
				{
					// Sorry, it's interrupted, yeah...
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

		public bool Start(string[] args)
		{
			TcpClient client = TryConnectClient();
			if (client == null)
			{
				StartServer();
				client = TryConnectClient();
			}
			if (client != null)
			{
				SendCommand(client, args);
				return true;
			}
			else
			{
#if DEBUG	
				Console.WriteLine("[M] Can't connect client. It's strange");
#endif				
				return false;
			}
		}

		private void SendCommand(TcpClient client, string[] args)
		{
			string str;
			if (args.Length > 0)
			{
				StringBuilder sb = new StringBuilder(args[0]);
				for (int i = 1; i < args.Length; i++)
				{
					sb.Append("\x000" + args[i]);
				}
				str = sb.ToString();
			}
			else
				str = "";
#if DEBUG
			Console.WriteLine("[M] Sending string \"" + str + "\"");
#endif
			string str2 = PasswordCode + str;
			EncodeAndSendString(client.GetStream(), str2);
#if DEBUG
			Console.WriteLine("[M] String \"" + str + "\" sent");
#endif
		}
		
		public event EventHandler<RemoteCommandEventArgs> RemoteCommandReceived;
		
		protected virtual void OnRemoteCommandReceived(string[] args)
		{
			if (RemoteCommandReceived != null)
				RemoteCommandReceived(this, new RemoteCommandEventArgs(args));
		}
		
		public void Stop()
		{
			if (mListener != null)
				mListener.Stop();
			mListeningThreadStopPending = true;
		}
	}
}

