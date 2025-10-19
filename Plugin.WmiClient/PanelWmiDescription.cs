using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using Plugin.WmiClient.Dal;
using Plugin.WmiClient.Dto;
using Plugin.WmiClient.Events;
using SAL.Windows;

namespace Plugin.WmiClient
{
	internal partial class PanelWmiDescription : PanelWmiBase
	{
		enum DefinitionImage
		{
			Loading = 0,
			Namespace = 1,
			Class = 2,
			Method = 3,
			Property = 4,
			Qualifier = 5,
		}

		private readonly ListViewGroup lvgDefMethod = new ListViewGroup("Methods");
		private readonly ListViewGroup lvgDefProperty = new ListViewGroup("Properties");
		private readonly ListViewGroup lvgDefQualifier = new ListViewGroup("Qualifiers");

		public PanelWmiDescription()
			: base("WMI Description")
		{
			this.InitializeComponent();
			lvDefinitions.Groups.AddRange(new ListViewGroup[] { lvgDefMethod, lvgDefProperty, lvgDefQualifier, });
		}

		protected override void OnCreateControl()
		{
			tvDefinitions.Dock = DockStyle.Fill;
			tvDefinitions.Visible = true;
			lvDefinitions.Dock = DockStyle.Fill;
			lvDefinitions.Visible = false;
			base.tsbnRun.Visible = false;
			this.EnsureRootNode();

			base.OnCreateControl();
		}

		private void EnsureRootNode()
		{
			TreeNode foundNode = null;
			foreach(TreeNode node in tvDefinitions.Nodes)
				if(((WmiPathItem)node.Tag).MachineName == this.Plugin.Settings.MachineName)
				{
					foundNode = node;
					break;
				}
			if(foundNode == null)
			{
				String rootText = this.Plugin.Settings.MachineName == null
					? Constant.Wmi.NamespaceRoot
					: "\\\\" + this.Plugin.Settings.MachineName + "\\" + Constant.Wmi.NamespaceRoot;
				foundNode = new TreeNode(rootText)
				{
					ImageIndex = (Int32)DefinitionImage.Namespace,
					SelectedImageIndex = (Int32)DefinitionImage.Namespace,
					Tag = new WmiPathItem(this.Plugin.Settings.MachineName, Constant.Wmi.NamespaceRoot, null),
				};
				foundNode.Nodes.Add(CreateLoadingNode());
				tvDefinitions.Nodes.Add(foundNode);
			}
			tvDefinitions.SelectedNode = foundNode;
		}

		private void FillDescription(WmiPathItem path, Object item)
		{
			KeyValuePair<String, Object>[] data;
			using(WmiDataClass wmi = new WmiDataClass(path, this.Plugin.Settings.ExecutionTimeout, this.Plugin.Settings.GetConnectionOptions()))
				if(item == null)
					data = wmi.GetClassDescription();
				else if(item is MethodData mItem)
					data = wmi.GetMethodDescription(mItem);
				else if(item is PropertyData pItem)
					data = wmi.GetPropertyDescription(pItem);
				else if(item is QualifierData qItem)
					data = wmi.GetQualifierDescription(qItem).ToArray();
				else throw new NotImplementedException();

			txtDescription.Text = data == null
				? null
				: String.Join(Environment.NewLine, data.Select(p => p.Key + "=" + this.Plugin.FormatValue(p.Value)).ToArray());
		}

		protected override WmiFormatDto GetCodeFormat()
			=> throw new NotSupportedException();

		protected override void Settings_PropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
			case nameof(PluginSettings.MachineName):
				lvDefinitions.Items.Clear();
				txtDescription.Text = String.Empty;

				TreeNode foundNode = null;
				foreach(TreeNode node in tvDefinitions.Nodes)
					if(((WmiPathItem)node.Tag).MachineName == this.Plugin.Settings.MachineName)
					{
						foundNode = node;
						break;
					}
				if(foundNode == null)
				{
					foundNode = new TreeNode(WmiData.GetFullNamespace(this.Plugin.Settings.MachineName, Constant.Wmi.NamespaceRoot))
					{
						ImageIndex = (Int32)DefinitionImage.Namespace,
						SelectedImageIndex = (Int32)DefinitionImage.Namespace,
						Tag = new WmiPathItem(this.Plugin.Settings.MachineName, Constant.Wmi.NamespaceRoot, null),
					};
					foundNode.Nodes.Add(CreateLoadingNode());
					tvDefinitions.Nodes.Add(foundNode);
				}
				tvDefinitions.SelectedNode = foundNode;
				break;
			}
			base.Settings_PropertyChanged(sender, e);
		}

		private void tvDefinitions_AfterExpand(Object sender, TreeViewEventArgs e)
		{
			TreeNode childNode = e.Node.Nodes.Count == 1 ? e.Node.Nodes[0] : null;
			if(childNode == null || childNode.ImageIndex != (Int32)DefinitionImage.Loading)
				return;

			List<TreeNode> childNodes = new List<TreeNode>();
			try
			{
				base.Cursor = Cursors.WaitCursor;
				WmiPathItem item = (WmiPathItem)e.Node.Tag;
				if(item.ClassName == null)
				{//Namespace
					using(WmiData wmi = new WmiData(item, this.Plugin.Settings.ExecutionTimeout, this.Plugin.Settings.GetConnectionOptions()))
					{
						foreach(String namespaceName in wmi.GetNamespaces(item.NamespaceName).OrderBy(p=>p))
							childNodes.Add(CreateDefinitionNode(namespaceName, item.MachineName, item.NamespaceName + "\\" + namespaceName, null));

						foreach(String className in wmi.GetClasses(item.NamespaceName, WmiData.WmiFilterType.None).OrderBy(p => p))
							childNodes.Add(CreateDefinitionNode(className, item.MachineName, item.NamespaceName, className));
					}
				}

				if(item.ClassName != null)
				{
					ClassItemDescription description;
					using(WmiDataClass wmi = new WmiDataClass(item, this.Plugin.Settings.ExecutionTimeout, this.Plugin.Settings.GetConnectionOptions()))
						description = wmi.GetClassDetails();

					childNodes.AddRange(description.Methods.Select(p => new TreeNode(MethodItemDescription.FormatWmiMethod(p)) { Tag = p, ImageIndex = (Int32)DefinitionImage.Method, SelectedImageIndex = (Int32)DefinitionImage.Method, }));
					childNodes.AddRange(description.Properties.Select(p => new TreeNode(MethodItemDescription.FormatWmiProperty(p)) { Tag = p, ImageIndex = (Int32)DefinitionImage.Property, SelectedImageIndex = (Int32)DefinitionImage.Property, }));
					childNodes.AddRange(description.Qualifiers.Select(p => new TreeNode(p.Name) { Tag = p, ImageIndex = (Int32)DefinitionImage.Qualifier, SelectedImageIndex = (Int32)DefinitionImage.Qualifier, }));
				}
			} catch(ManagementException exc)
			{
				childNode.Text = exc.Message;
				throw;
			} finally
			{
				base.Cursor = Cursors.Default;
			}

			e.Node.Nodes.Clear();
			e.Node.Nodes.AddRange(childNodes.ToArray());
		}

		private void tvDefinitions_AfterSelect(Object sender, TreeViewEventArgs e)
		{
			WmiPathItem path = null;
			Object item = e.Node.Tag;
			switch((DefinitionImage)e.Node.ImageIndex)
			{
			case DefinitionImage.Loading:
			case DefinitionImage.Namespace:
				return;
			case DefinitionImage.Class:
				path = (WmiPathItem)item;
				item = null;
				break;
			case DefinitionImage.Method:
			case  DefinitionImage.Property:
			case DefinitionImage.Qualifier:
				path = (WmiPathItem)e.Node.Parent.Tag;
				break;
			default:
				throw new NotImplementedException();
			}

			if(path != null)
				this.FillDescription(path, item);
		}

		private TreeNode CreateLoadingNode()
			=> new TreeNode("Loading...") { ImageIndex = (Int32)DefinitionImage.Loading, SelectedImageIndex = (Int32)DefinitionImage.Loading, };

		private TreeNode CreateDefinitionNode(String text, String machineName, String namespaceName, String className)
		{
			TreeNode result = new TreeNode(text) { Tag = new WmiPathItem(machineName,namespaceName,className) };
			result.ImageIndex = result.SelectedImageIndex = className == null
				? (Int32)DefinitionImage.Namespace
				: (Int32)DefinitionImage.Class;

			result.Nodes.Add(CreateLoadingNode());
			return result;
		}

		protected override void Class_FinishedLoading(Object sender, FinishedLoadingEventArgs e)
		{
			if(!String.IsNullOrEmpty(base.WmiPath.ClassName))
				lvDefinitions.Items.Clear();

			base.Class_FinishedLoading(sender, e);
		}

		protected override void Class_SelectedIndexChanged(Object sender, EventArgs e)
		{
			tvDefinitions.Visible = false;
			lvDefinitions.Items.Clear();
			lvDefinitions.Visible = true;
			ClassItemDescription description;

			using(WmiDataClass wmi = this.Plugin.CreateWmiDataClass(this.WmiPath))
				description = wmi.GetClassDetails();

			IEnumerable<ListViewItem> lvMethods = description.Methods.Select(p => new ListViewItem(MethodItemDescription.FormatWmiMethod(p), lvgDefMethod) { Tag = p });
			IEnumerable<ListViewItem> lvProperties = description.Properties.Select(p => new ListViewItem(MethodItemDescription.FormatWmiProperty(p), lvgDefProperty) { Tag = p });
			IEnumerable<ListViewItem> lvQualifiers = description.Qualifiers.Select(p => new ListViewItem(p.Name, lvgDefQualifier) { Tag = p });

			lvDefinitions.Items.AddRange(lvMethods.Union(lvProperties).Union(lvQualifiers).ToArray());
			lvDefinitions.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
			
			base.Class_SelectedIndexChanged(sender, e);
		}

		private void timerLVIndexHack_Tick(Object sender, EventArgs e)
		{
			timerLVIndexHack.Stop();
			this.lvDefinitions_SelectedIndexChanged(sender, e);
		}

		private void lvDefinitions_SelectedIndexChanged(Object sender, EventArgs e)
		{
			if(sender == lvDefinitions)
			{
				if(!timerLVIndexHack.Enabled)
					timerLVIndexHack.Start();
				return;
			}

			ListViewItem item = lvDefinitions.SelectedItems.Count == 0 ? null : lvDefinitions.SelectedItems[0];
			WmiPathItem path = this.WmiPath;
			this.FillDescription(path, item == null ? null : item.Tag);
		}
	}
}