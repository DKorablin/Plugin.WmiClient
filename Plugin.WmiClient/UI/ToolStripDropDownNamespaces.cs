using System;
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
			base.DropDownOpening += (sender, e) => throw new NotImplementedException();
		}

		protected void OnSelectedIndexChanged()
			=> this.SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
	}
}