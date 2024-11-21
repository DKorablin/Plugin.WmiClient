using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AlphaOmega.Windows.Forms;

namespace Plugin.WmiClient.UI
{
	internal class ContextMenuStripCopy2 : ContextMenuStripCopy
	{
		public override void CopyListViewItems(Int32? columnIndex)
		{
			StringBuilder buffer = new StringBuilder();
			ListView lv = base.SourceControl;
			for(Int32 loop = 0; loop < lv.SelectedItems.Count; loop++)
			{
				ListViewItem item = lv.SelectedItems[loop];
				Dictionary<String, String> tagData = item.Tag as Dictionary<String, String>;

				if(columnIndex != null)
				{
					String text = tagData == null ? item.SubItems[columnIndex.Value].Text : tagData[lv.Columns[columnIndex.Value].Text];
					buffer.Append(text);
				} else
					for(Int32 innerLoop = 0; innerLoop < lv.Columns.Count; innerLoop++)
					{
						ColumnHeader column = lv.Columns[innerLoop];
						String text = tagData == null ? item.SubItems[column.Index].Text : tagData[column.Text];
						buffer.Append(text);
						/*if(String.IsNullOrEmpty(column.Text))
							buffer.Append(item.SubItems[column.Index].Text);
						else
							buffer.AppendFormat("{0}={1}", column.Text, item.SubItems[column.Index].Text);*/
						if(innerLoop + 1 < lv.Columns.Count)
							buffer.Append('\t');
					}

				if(loop + 1 < lv.SelectedItems.Count)
					buffer.AppendLine();
			}

			Clipboard.SetText(buffer.ToString());
			//base.CopyListViewItems(columnIndex);
		}
	}
}