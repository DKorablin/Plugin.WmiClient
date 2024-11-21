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
			this.Control.BeforeExpand += Control_BeforeExpand;
			this.Control.NodeMouseClick += Control_NodeMouseClick;
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

		private void Control_BeforeExpand(Object sender, TreeViewCancelEventArgs e)
			=> throw new NotImplementedException();

		private void Control_NodeMouseClick(Object sender, TreeNodeMouseClickEventArgs e)
			=> throw new NotImplementedException();
	}
}