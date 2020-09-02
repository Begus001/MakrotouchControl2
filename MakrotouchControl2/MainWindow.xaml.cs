using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MakrotouchControl2
{
	public partial class MainWindow : Window
	{
		public readonly Connection MainConnection;
		public readonly MacroManager MacroManager;

		public MainWindow()
		{
			InitializeComponent();

			ConfigManager.Init();

			MainConnection = new Connection();
			MainConnection.ConnectionStatusChanged += UpdateConnectionStatus;
			MainConnection.Start();

			MacroManager = new MacroManager(lvMain);
			MacroManager.TestPopulate();
		}

		private void UpdateConnectionStatus(object sender, ConnectionChangedEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				lbConnected.Content = e.NewConnectionStatus ? "Connected" : "Not connected";
				if (e.IP != IPAddress.None)
					lbIP.Content = $"IP: {e.IP}";
				else
					lbIP.Content = "IP:";
			});
		}

		private void BtExit_Click(object sender, RoutedEventArgs e)
		{
			MainConnection.Send("makrotouch disconnect");
			MainConnection.RequestClose = true;
			Environment.Exit(0);
		}

		private void BtMinimize_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void LbTitle_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				DragMove();
		}
	}
}
