using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows;
using System.Threading;

namespace MakrotouchControl2
{
	public static class MacroManager
	{
		private static ListView MainList { get; set; } = null;

		private readonly static List<Macro> macroSource = new List<Macro>();

		public static void Init(ListView mainList)
		{
			MainList = mainList;

			MainList.ItemsSource = macroSource;
		}

		public static void TestPopulate()
		{
			if (MainList == null) { Debug.WriteLine("MainList not assigned"); return; }

			for (uint i = 0; i < 15; i++)
			{
				macroSource.Add(new Macro() { ID = i, Name = "Close Window", Type = "Remote", Action = "Wait 100 ms, Press 'ALT + F4'" });
			}
		}

		public static void Clear()
		{
			MainList.ItemsSource = null;
			MainList.Items.Clear();

			macroSource.Clear();
		}
	}

	class RawMacro
	{
		public uint ID { get; set; }
		public string Name { get; set; }
		public byte[] Image { get; set; }
		public string Type { get; set; }
		public List<Dictionary<string, string>> Action { get; set; } = new List<Dictionary<string, string>>();
	}
}
