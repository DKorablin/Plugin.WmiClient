using System;
using System.Windows.Forms;

namespace Plugin.WmiClient.UI
{
	internal class NamespacesHost : ToolStripControlHost
	{
		private static readonly TreeNode EmptyNode = new TreeNode(String.Empty);

		internal new TreeView Control => (TreeView)base.Control;

		public NamespacesHost()
			: base(NamespacesHost.CreateControl())
		{
			this.Control.BeforeExpand += (sender, e) => throw new NotImplementedException();
			this.Control.NodeMouseClick += (sender, e) => throw new NotImplementedException();
		}

		private static TreeView CreateControl()
		{
			TreeView result = new TreeView()
			{
				FullRowSelect = true,
				HotTracking = true,
			};

			TreeNode rootNode = new TreeNode(Constant.Wmi.NamespaceRoot);
			rootNode.Nodes.Add(NamespacesHost.EmptyNode);

			result.Nodes.Add(rootNode);

			return result;
		}
	}
}