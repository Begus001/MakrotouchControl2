using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.Json;

namespace MakrotouchControl2
{
	public static class ConfigManager
	{
		public static Dictionary<string, dynamic> Config { get; private set; }
		public static void Init()
		{
			if (!File.Exists("config.json")) { Debug.WriteLine("Config file not found"); return; }

			Config = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(File.ReadAllText("config.json"));
		}
	}
}
