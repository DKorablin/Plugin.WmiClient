using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Plugin.WmiClient
{
	internal static class Utils
	{
		public static void SetDropDownListWidth(ToolStripComboBox cb)
			=> Utils.SetDropDownListWidth(cb, cb.Items.Cast<Object>().Select(p => p.ToString()));

		public static void SetDropDownListWidth(ToolStripComboBox cb, IEnumerable<String> items)
		{
			Int32 result = cb.Width;
			foreach(String item in items)
			{
				Int32 itemWidth = TextRenderer.MeasureText(item, cb.Font).Width;
				if(itemWidth > result)
					result = itemWidth;
			}

			Boolean isDroppedDown = cb.DroppedDown;
			cb.DropDownWidth = result;
			cb.DroppedDown = isDroppedDown;
		}
	}
}