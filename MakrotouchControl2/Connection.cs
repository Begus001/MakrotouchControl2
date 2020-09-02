using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Windows;
using System.Diagnostics.Eventing.Reader;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MakrotouchControl2
{
	public class Connection
	{
		public bool Connected { get; private set; } = false;
		public IPAddress IP { get; private set; } = IPAddress.None;
		public ushort Port { get; private set; }

		public bool RequestClose { get; set; } = false;

		private readonly Timer TimeoutTimer = new Timer();

		public Connection()
		{
			Port = UInt16.Parse(ConfigManager.Config["port"].ToString());
			TimeoutTimer.Interval = UInt32.Parse(ConfigManager.Config["timeout"].ToString());
			TimeoutTimer.Elapsed += Reset;
		}

		public event EventHandler<ConnectionChangedEventArgs> ConnectionStatusChanged;

		protected virtual void OnConnectionStatusChanged()
		{
			ConnectionStatusChanged?.Invoke(this, new ConnectionChangedEventArgs { NewConnectionStatus = Connected, OldConnectionStatus = !Connected, IP = this.IP });
		}

		public void Start()
		{
			Thread connectionLooper = new Thread(ConnectionLoop);
			connectionLooper.Start();
		}

		private void ConnectionLoop()
		{
			while (!RequestClose)
			{
				Broadcast();
				KeepAlive();
			}
		}

		private void Broadcast()
		{
			UdpClient socket = new UdpClient(new IPEndPoint(IPAddress.Any, Port));
			socket.Client.Blocking = false;

			byte[] broadcastMessage = Encoding.UTF8.GetBytes("makrotouch connect");
			IPEndPoint messageSender = new IPEndPoint(0, 0);
			IPAddress hostAddress = GetHostIP();

			while (!Connected && !RequestClose)
			{
				Thread.Sleep(250);

				socket.Send(broadcastMessage, broadcastMessage.Length, "255.255.255.255", Port);
				string message = Encoding.UTF8.GetString(socket.Receive(ref messageSender));

				if (message != "makrotouch connect" || messageSender.Address.ToString() == hostAddress.ToString()) continue;

				Connected = true;
				IP = messageSender.Address;
				OnConnectionStatusChanged();

				TimeoutTimer.Start();
			}

			socket.Close();
		}

		private IPAddress GetHostIP()
		{
			IPAddress address;
			using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
			{
				socket.Connect("8.8.8.8", 65530);
				IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
				address = endPoint.Address;
			}

			return address;
		}

		private void KeepAlive()
		{
			UdpClient socket = new UdpClient(new IPEndPoint(IPAddress.Any, Port));
			socket.Client.Blocking = true;
			byte[] keepAliveMessage = Encoding.UTF8.GetBytes("makrotouch ping");
			IPEndPoint messageSender = new IPEndPoint(0, 0);

			while (Connected && !RequestClose)
			{
				Thread.Sleep(100);

				socket.Send(keepAliveMessage, keepAliveMessage.Length, new IPEndPoint(IP, Port));
				string message = Encoding.UTF8.GetString(socket.Receive(ref messageSender));
				string[] makrotouchCommand = message.Split(' ');

				if (!message.Contains("makrotouch") || makrotouchCommand.Length <= 1) continue;

				HandleCommand(makrotouchCommand);
			}

			socket.Close();
		}

		private void HandleCommand(string[] command)
		{
			switch (command[1])
			{
				case "pong":
					TimeoutTimer.Stop();
					TimeoutTimer.Start();
					break;

				case "disconnect":
					Debug.WriteLine("Closing connection");
					Reset();
					break;

				case "exec":
					// TODO: Execute macros
					break;

				default:
					Debug.WriteLine("Received invalid command");
					break;
			}
		}

		public void Send(string message)
		{
			if (!Connected) return;

			using(UdpClient socket = new UdpClient())
			{
				socket.Send(Encoding.UTF8.GetBytes(message), message.Length, new IPEndPoint(IP, Port));
			}
		}

		private void Reset()
		{
			Reset(null, null);
		}

		private void Reset(object sender, ElapsedEventArgs e)
		{
			Connected = false;
			IP = IPAddress.None;
			OnConnectionStatusChanged();
		}
	}

	public class ConnectionChangedEventArgs : EventArgs
	{
		public bool NewConnectionStatus { get; set; }
		public bool OldConnectionStatus { get; set; }
		public IPAddress IP { get; set; } = IPAddress.None;
	}
}
