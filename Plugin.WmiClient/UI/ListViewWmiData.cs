using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management;
using System.Windows.Forms;
using AlphaOmega.Windows.Forms;

namespace Plugin.WmiClient.UI
{
	internal class ListViewWmiData : SortableListView
	{
		[Browsable(false)]
		public PluginWindows Plugin { get; set; }

		public ListViewWmiData()
		{
			base.FullRowSelect = true;
			base.GridLines = true;
			base.HideSelection = false;
			base.DoubleBuffered = true;
			base.View = View.Details;
			base.ContextMenuStrip = new ContextMenuStripCopy2();
		}

		public void AddManagementObject(ManagementBaseObject item)
		{
			try
			{
				if(item is ManagementClass)
				{
					ManagementClass classItem = item as ManagementClass;
					if(base.Columns.Count == 0)
						base.Columns.AddRange(new ColumnHeader[]{
									new ColumnHeader() { Text = "__CLASS", },
									new ColumnHeader() { Text = "Qualifiers", },
									new ColumnHeader() { Text = "Text", },
								});

					String[] columns = new String[base.Columns.Count];
					columns[0] = classItem["__CLASS"].ToString();
					List<String> qualifiers = new List<String>(classItem.Qualifiers.Count);
					foreach(QualifierData qd in classItem.Qualifiers)
						qualifiers.Add(qd.Name);
					columns[1] = qualifiers.ToArray().ToString();
					columns[2] = classItem.GetText(TextFormat.Mof);

					ListViewItem listItem = new ListViewItem(columns);
					base.Items.Add(listItem);

				} else
				{
					if(base.Columns.Count == 0)
					{
						ColumnHeader[] listColumns = new ColumnHeader[item.Properties.Count];
						Int32 index = 0;
						foreach(PropertyData property in item.Properties)
							listColumns[index++] = new ColumnHeader() { Text = property.Name, Name = property.Name, };
						base.Columns.AddRange(listColumns);
					}

					Dictionary<String, String> tagData = new Dictionary<String, String>();
					String[] columns = new String[base.Columns.Count];
					foreach(PropertyData property in item.Properties)
					{//Hope that properties will not change the order inside iteration
						String text = this.Plugin.FormatValue(property);
						tagData.Add(property.Name, text);
						columns[base.Columns[property.Name].Index] = text;
					}
					ListViewItem listItem = new ListViewItem(columns) { Tag = tagData };
					base.Items.Add(listItem);
				}
			} finally
			{
				item.Dispose();
			}
		}
	}
}