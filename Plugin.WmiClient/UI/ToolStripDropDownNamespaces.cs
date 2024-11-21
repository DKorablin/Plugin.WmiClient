using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Plugin.WmiClient.UI
{
	public class ToolStripDropDownNamespaces : ToolStripDropDownButton
	{
		private readonly NamespacesHost _host;

		public EventHandler<EventArgs> SelectedIndexChanged;

		public ToolStripDropDownNamespaces()
			: base("tsbnNamespace")
		{
			this._host = new NamespacesHost();
			base.DropDownItems.Add(this._host);
			base.DropDownOpening += ToolStripDropDownNamespaces_DropDownOpening;
		}

		protected void OnSelectedIndexChanged()
			=> this.SelectedIndexChanged?.Invoke(this, EventArgs.Empty);

		private void ToolStripDropDownNamespaces_DropDownOpening(Object sender, EventArgs e)
			=> throw new NotImplementedException();
	}
}