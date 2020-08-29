using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MakrotouchControl2
{
	public class Macro
	{
		public uint ID { get; set; }
		public string Name { get; set; }
		public Bitmap Image { get; set; }
		public string Type { get; set; }
		public string Action { get; set; }
	}
}
