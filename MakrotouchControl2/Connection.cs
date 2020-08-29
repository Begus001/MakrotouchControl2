using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace MakrotouchControl2
{
	public class Connection
	{
		public bool Connected { get; private set; }
		public string URL { get; private set; }
		public ushort Port { get; private set; }

		public Connection()
		{
			Port = UInt16.Parse(ConfigManager.Config["port"].ToString());
		}

		public event EventHandler<ConnectionChangedEventArgs> ConnectionStatusChanged;

		protected virtual void OnConnectionStatusChanged()
		{
			ConnectionStatusChanged?.Invoke(this, new ConnectionChangedEventArgs { NewConnectionStatus = Connected, OldConnectionStatus = !Connected });
		}

		public void Start()
		{
			Thread connectionLooper = new Thread(ConnectionLoop);
			connectionLooper.Start();
		}

		private void ConnectionLoop()
		{

		}

		private void Listen()
		{

		}
	}

	public class ConnectionChangedEventArgs : EventArgs
	{
		public bool NewConnectionStatus { get; set; }
		public bool OldConnectionStatus { get; set; }
	}
}
