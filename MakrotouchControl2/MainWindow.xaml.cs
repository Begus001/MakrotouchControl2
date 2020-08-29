using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
		public MainWindow()
		{
			InitializeComponent();

			ConfigManager.Init();

			Connection mainConnection = new Connection();
			mainConnection.ConnectionStatusChanged += UpdateConnectionStatus;
			mainConnection.Start();

			MacroManager.Init(lvMain);
			MacroManager.TestPopulate();
		}

		private void UpdateConnectionStatus(object sender, ConnectionChangedEventArgs e)
		{
			lbConnected.Content = e.NewConnectionStatus ? "Connected" : "Not connected";
		}

		private void BtExit_Click(object sender, RoutedEventArgs e)
		{
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
