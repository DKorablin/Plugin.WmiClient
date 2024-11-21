using System.Windows.Forms;

namespace Plugin.WmiClient.Dto
{
	internal class QueryListResult
	{
		public ListViewItem[] Items { get; set; }
		public ColumnHeader[] Columns { get; set; }

		public QueryListResult(ListViewItem[] items, ColumnHeader[] columns)
		{
			this.Items = items;
			this.Columns = columns;
		}
	}
}