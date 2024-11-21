using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;
using Plugin.WmiClient.Dto;

namespace Plugin.WmiClient.UI
{
	internal class ListViewWmiResult : ListView
	{
		private ColumnHeader _colEventName;
		private ColumnHeader _colEventValue;

		[Browsable(false)]
		public PluginWindows Plugin { get; set; }

		public ListViewWmiResult()
		{
			this._colEventName = new ColumnHeader() { Text = "Name", };
			this._colEventValue = new ColumnHeader() { Text = "Value", };
			base.Columns.AddRange(new ColumnHeader[] { this._colEventName, this._colEventValue, });
		}

		public void AddEvent(String groupName, IEnumerable<PropertyData> properties)
		{
			ListViewGroup lvGroup = new ListViewGroup($"{groupName}: {DateTime.Now}");

			base.SuspendLayout();
			try
			{
				List<ListViewItem> itemsToAdd = new List<ListViewItem>();
				foreach(PropertyData property in properties)
				{
					String[] subItems = new String[base.Columns.Count];
					subItems[_colEventName.Index] = MethodItemDescription.FormatWmiProperty(property);
					subItems[_colEventValue.Index] = this.Plugin.FormatValue(property);

					ListViewItem item = new ListViewItem(subItems, lvGroup);
					if(subItems[_colEventValue.Index] == null)
						item.ForeColor = Color.Gray;
					itemsToAdd.Add(item);
				}

				if(itemsToAdd.Count == 0)
				{
					String[] subItems = new String[base.Columns.Count];
					subItems[_colEventName.Index] = "Return";
					subItems[_colEventValue.Index] = null;

					ListViewItem item = new ListViewItem(subItems, lvGroup);
					if(subItems[_colEventValue.Index] == null)
						item.ForeColor = Color.Gray;
					itemsToAdd.Add(item);
				}

				base.Groups.Add(lvGroup);
				base.Items.AddRange(itemsToAdd.ToArray());
				base.AutoResizeColumn(_colEventName.Index, ColumnHeaderAutoResizeStyle.ColumnContent);
				while(base.Items.Count > 500)
				{
					ListViewGroup group = base.Items[0].Group;
					while(base.Items[0].Group == group)
						base.Items[0].Remove();
				}
				base.Items[base.Items.Count - 1].EnsureVisible();//Scroll down to end
			} finally
			{
				base.ResumeLayout();
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch(e.KeyData)
			{
			case Keys.A | Keys.Control:
				foreach(ListViewItem item in base.Items)
					item.Selected = true;
				e.Handled = true;
				break;
			case Keys.Delete:
			case Keys.Back:
				while(base.SelectedItems.Count > 0)
				{
					ListViewItem item = base.SelectedItems[0];
					if(item.Group.Items.Count == 1)
						base.Groups.Remove(item.Group);
					item.Remove();
				}
				e.Handled = true;
				break;
			}

			base.OnKeyDown(e);
		}
	}
}