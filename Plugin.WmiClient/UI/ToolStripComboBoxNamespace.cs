using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Plugin.WmiClient.Dal;
using Plugin.WmiClient.Events;

namespace Plugin.WmiClient.UI
{
	internal class ToolStripComboBoxNamespace : ToolStripComboBox
	{
		private static Dictionary<String, String[]> NamespaceCache = new Dictionary<String, String[]>();
		private static Object NamespaceCacheLock = new Object();
		private BackgroundWorker bwNamespaceUpdate;
		private Boolean _namespaceChanged = false;

		[Browsable(false)]
		public PluginWindows Plugin { get; set; }

		public event EventHandler<EventArgs> StartLoading;
		public event EventHandler<FinishedLoadingEventArgs> FinishedLoading;
	
		public ToolStripComboBoxNamespace()
			: base("tscbNamespace")
		{
			base.AutoCompleteMode = AutoCompleteMode.Suggest;
			base.AutoCompleteSource = AutoCompleteSource.ListItems;
			base.Sorted = true;

			this.InitializeComponent();
		}

		private void LoadNamespaces()
		{
			if(!bwNamespaceUpdate.IsBusy)
			{
				if(this.Plugin == null)
					throw new InvalidOperationException("this.Plugin is null");

				bwNamespaceUpdate.RunWorkerAsync();

				if(this.StartLoading != null)
					this.StartLoading(this, EventArgs.Empty);
			}
		}

		/// <summary>Set namespace text outside of the control</summary>
		/// <param name="ns">Namespace name</param>
		public void SetNamespaceText(String ns)
		{
			if(String.IsNullOrEmpty(ns))
				return;

			base.Text = ns;
			this.OnSelectedIndexChanged(EventArgs.Empty);
		}

		public void ClearNamespaces()
		{
			base.Items.Clear();
			base.Text = String.Empty;
			base.DropDown += ToolStripComboBoxNamespace_DropDown;//Attach event to load namespaces on next update
		}

		private void InitializeComponent()
		{
			this.bwNamespaceUpdate = new BackgroundWorker();
			// 
			// bwNamespaceUpdate
			// 
			this.bwNamespaceUpdate.DoWork += new DoWorkEventHandler(this.bwNamespaceUpdate_DoWork);
			this.bwNamespaceUpdate.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bwNamespaceUpdate_RunWorkerCompleted);
			// 
			// ToolStripComboBoxNamespace
			// 
			this.DropDown += new EventHandler(this.ToolStripComboBoxNamespace_DropDown);
			//this.DropDownClosed += ToolStripComboBoxNamespace_DropDownClosed;
			this.TextUpdate += new EventHandler(this.ToolStripComboBoxNamespace_TextUpdate);
			this.Enter += new EventHandler(this.ToolStripComboBoxNamespace_Enter);
			this.Leave += new EventHandler(this.ToolStripComboBoxNamespace_Leave);
		}

		private void bwNamespaceUpdate_DoWork(Object sender, DoWorkEventArgs e)
		{
			String machineName = this.Plugin.Settings.MachineName ?? ".";

			if(!NamespaceCache.TryGetValue(machineName, out String[] result))
				lock(NamespaceCacheLock)
					if(!NamespaceCache.TryGetValue(machineName, out result))
					{
						using(WmiData wmi = this.Plugin.CreateWmiData())
						{
							List<String> items = new List<String>(wmi.GetNamespacesRecursive());
							result = items.ToArray();
						}
						NamespaceCache.Add(machineName, result);
					}

			e.Result = result.ToArray();
		}

		private void bwNamespaceUpdate_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
		{
			base.Parent.SuspendLayout();
			try
			{
				if(e.Error != null)
				{
					PluginWindows.Trace.TraceData(TraceEventType.Error, 10, e.Error);
					base.DroppedDown = false;
					base.DropDown += ToolStripComboBoxNamespace_DropDown;//Trying to update namespaces in the next time
				} else
				{
					String[] namespaces = (String[])e.Result;
					if(namespaces != null)//+Cancelled
					{
						base.Items.AddRange(namespaces);
						Utils.SetDropDownListWidth(this, namespaces);
					}
				}
			} finally
			{
				if(this.FinishedLoading != null)
					this.FinishedLoading(this, new FinishedLoadingEventArgs() { Error = e.Error, });
				base.Parent.ResumeLayout(false);
			}
		}

		private void ToolStripComboBoxNamespace_DropDown(Object sender, EventArgs e)
		{
			if(base.Items.Count == 0)
				this.LoadNamespaces();

			//Bypass double AutoComplete mode
			//base.AutoCompleteMode = AutoCompleteMode.None;

			base.DropDown -= ToolStripComboBoxNamespace_DropDown;
		}

		private void ToolStripComboBoxNamespace_DropDownClosed(Object sender, EventArgs e)
			=> base.AutoCompleteMode = AutoCompleteMode.Suggest;

		private void ToolStripComboBoxNamespace_TextUpdate(Object sender, EventArgs e)
		{
			if(base.Items.Count == 0)
				this.LoadNamespaces();
			else
				this._namespaceChanged = true;
		}

		private void ToolStripComboBoxNamespace_Enter(Object sender, EventArgs e)
			=> this._namespaceChanged = false;

		private void ToolStripComboBoxNamespace_Leave(Object sender, EventArgs e)
		{
			if(this._namespaceChanged)//Отдаём событие на смену пространства имён только после покидания элемента управления. Иначе при каждом клике будет евент
				base.OnSelectedIndexChanged(EventArgs.Empty);
		}
	}
}