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

namespace Plugin.WmiClient
{
	internal partial class PanelWmiMethod : PanelWmiBase
	{
		private WmiObserver _observer;
		protected override WmiData.WmiFilterType ClassFilterType => WmiData.WmiFilterType.ClassesWithMethods;
		protected override MethodItemDescription MethodName => (MethodItemDescription)tscbMethods.SelectedItem;

		protected override WmiPathItem WmiPath
		{
			get
			{
				WmiPathItem result = base.WmiPath;
				if(lvInstances.SelectedItems.Count == 1)
					result.ClassName = result.ClassName + "." + lvInstances.SelectedItems[0].Tag;
				return result;

			}
		}

		public PanelWmiMethod()
			: base("WMI Methods")
		{
			InitializeComponent();
			tscbMethods.ComboBox.DisplayMember = "DisplayName";
		}

		protected override void OnCreateControl()
		{
			this._observer = new WmiObserver();
			this._observer.OnObjectReady += Instance_OnObjectReady;
			this._observer.OnCompleted += Instance_OnCompleted;

			gvParameters.Plugin = this.Plugin;
			lvResult.Plugin = this.Plugin;
			splitMethod.Panel1Collapsed = true;
			splitMain.Panel2Collapsed = true;

			base.DetachInvokeCodeButton(tsMethods);
			base.OnCreateControl();
		}

		protected override void Window_Closed(Object sender, EventArgs e)
		{
			this._observer?.Dispose();

			base.Window_Closed(sender, e);
		}

		/// <summary></summary>
		/// <param name="ignoreInstance">This argument is used when we need path without instance Ex.: Query methods, Invoke static method</param>
		/// <returns></returns>
		private WmiDataClass CreateWmiDataClass(Boolean ignoreInstance = false)
		{//\\\\MyServer\\root\\MyApp:MyClass.Key='abc'
			WmiPathItem path = ignoreInstance
				? base.WmiPath
				: this.WmiPath;

			return this.Plugin.CreateWmiDataClass(path);
		}

		protected override void Class_FinishedLoading(Object sender, FinishedLoadingEventArgs e)
		{
			lvInstances.Clear();
			gvParameters.Clear();

			tscbMethods.Text = String.Empty;
			tscbMethods.Items.Clear();
			tscbMethods.Enabled = false;

			base.Class_FinishedLoading(sender, e);
		}

		protected override void Class_SelectedIndexChanged(Object sender, EventArgs e)
		{
			if(this._observer.InProgress)
			{//????
				this._observer.Cancel();
				while(this._observer.InProgress)
				{
					Application.DoEvents();
					System.Threading.Thread.Sleep(500);
				}
			}

			tscbMethods.Items.Clear();
			lvInstances.Clear();
			tscbMethods.Enabled = true;
			tscbMethods.Text = String.Empty;
			gvParameters.Clear();

			WmiPathItem path = this.WmiPath;
			using(WmiDataClass data = this.Plugin.CreateWmiDataClass(path))
				data.GetMethodInstances(this._observer);
			
			/*while(bwInstances.IsBusy)
			{
				Application.DoEvents();
				bwInstances.CancelAsync();
				System.Threading.Thread.Sleep(500);
			}
			bwInstances.RunWorkerAsync(path);*/
			this.LoadBase("instances");

			base.Class_SelectedIndexChanged(sender,e);
		}

		private void Instance_OnCompleted(Object sender, FinishedLoadingEventArgs e)
		{
			this.FinishedLoading(e.Status == ManagementStatus.NoError ? null : e.Status.ToString());
			lvInstances.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		private void Instance_OnObjectReady(Object sender, ObjectReadyEventArgs e)
		{
			ManagementBaseObject wmiObject = e.NewObject;

			//Collecting key properties
			List<KeyValuePair<String, Object>> instanceRow = new List<KeyValuePair<String, Object>>();
			foreach(PropertyData property in wmiObject.Properties)
				foreach(QualifierData data in property.Qualifiers)
				{
					if(data.Name.Equals("key", StringComparison.Ordinal))
						instanceRow.Add(new KeyValuePair<String, Object>(property.Name, wmiObject.GetPropertyValue(property.Name)));
				}

			//Creating list item
			ListViewItem listItem = new ListViewItem(new String[instanceRow.Count]);
			foreach(KeyValuePair<String, Object> row in instanceRow)
			{
				ColumnHeader header = null;
				foreach(ColumnHeader col in lvInstances.Columns)
					if(col.Text == row.Key)
					{
						header = col;
						break;
					}
				if(header == null)
					lvInstances.Columns.Add(header = new ColumnHeader() { Text = row.Key, });

				listItem.SubItems[header.Index].Text = row.Value == null ? "null" : row.Value.ToString();
			}

			//Joining WMI key
			listItem.Tag = String.Join(",", Array.ConvertAll(instanceRow.ToArray(), delegate(KeyValuePair<String, Object> item) { return item.Key + "='" + item.Value + "'"; }));
			lvInstances.Items.Add(listItem);

			if(lvInstances.Items.Count == 1)//At the first element expanding the list
				splitMethod.Panel1Collapsed = false;
		}

		private void lvInstances_SelectedIndexChanged(Object sender, EventArgs e)
		{
			MethodItemDescription method = this.MethodName;
			if(method != null && method.Property != null)
			{
				if(lvInstances.SelectedItems.Count == 1 && gvParameters.Count == 1)
				{
					WmiDataRow row = gvParameters.GetRowItem(0);
					Object oldValue = row.Value;
					using(WmiDataClass data = this.CreateWmiDataClass())
						row.Value = data.GetPropertyValue(method.Property.Name);
					if(!Object.Equals(oldValue, row.Value))
						gvParameters.Reset(0);
				}
			}
		}

		private void tsMethods_Resize(Object sender, EventArgs e)
		{
			Int32 controlsWidth = 10;
			foreach(ToolStripItem ctrl in tsMethods.Items)
				if(ctrl != tscbMethods)
					controlsWidth += ctrl.Width + ctrl.Padding.Left + ctrl.Padding.Right + ctrl.Margin.Left + ctrl.Margin.Right;

			tscbMethods.Width = tsMethods.Width
				- (tsMethods.GripRectangle.Width
				+ tsMethods.Padding.Left
				+ tsMethods.Padding.Right
				+ tsMethods.GripMargin.Left
				+ tsMethods.GripMargin.Right
				+ controlsWidth);
		}

		private void tscbMethods_SelectedIndexChanged(Object sender, EventArgs e)
		{
			MethodItemDescription description = this.MethodName;
			if(lvInstances.Items.Count > 0)//Toggle instance panel only if method is static and class has instances
				splitMethod.Panel1Collapsed = description.IsStatic;

			if(description.Method != null)
			{
				if(description.Method.InParameters == null || description.Method.InParameters.Properties.Count == 0)
				{
					if(!splitMethod.Panel1Collapsed)
						splitMethod.Panel2Collapsed = true;//Otherwise both panels will be collapsed and will be big white space
					gvParameters.Clear();
				} else
				{
					IEnumerable<PropertyData> arguments = description.Method.InParameters.Properties.Cast<PropertyData>();
					gvParameters.DataBind(this.WmiPath, arguments.Select(p => new WmiDataRow(p)));
					splitMethod.Panel2Collapsed = false;
				}
			} else if(description.Property != null)
			{
				WmiDataRow row = new WmiDataRow(description.Property);
				if(lvInstances.Items.Count == 0 || lvInstances.SelectedItems.Count == 1)
					using(WmiDataClass wmi = this.CreateWmiDataClass())
						row.Value = wmi.GetPropertyValue(description.Property.Name);

				gvParameters.DataBind(this.WmiPath, new WmiDataRow[] { row, });
				splitMethod.Panel2Collapsed = false;
			}
		}

		private void tscbMethods_DropDown(Object sender, EventArgs e)
		{
			if(tscbMethods.Items.Count == 0)
			{
				using(WmiDataClass wmi = this.CreateWmiDataClass(true))
				{
					List<MethodItemDescription> items = new List<MethodItemDescription>();
					foreach(MethodData method in wmi.GetMethods())
						items.Add(new MethodItemDescription(method));

					foreach(PropertyData property in wmi.GetProperties())
						items.Add(new MethodItemDescription(property));

					tscbMethods.Items.AddRange(items.ToArray());
				}
			}
		}

		private void bwInstances_DoWork(Object sender, DoWorkEventArgs e)
		{
			const Int32 MaxItems = 10;
			WmiPathItem path = (WmiPathItem)e.Argument;

			Int32 instancesCount = 0;

			List<KeyValuePair<String, Object>[]> instances = new List<KeyValuePair<String, Object>[]>(MaxItems);
			using(WmiDataClass wmi = this.Plugin.CreateWmiDataClass(path))
				foreach(KeyValuePair<String, Object>[] instance in wmi.GetMethodInstances())
				{
					instancesCount++;
					instances.Add(instance);
					if(bwInstances.CancellationPending)
						break;
					else if(instances.Count == MaxItems)
					{
						bwInstances.ReportProgress(MaxItems, instances.ToArray());
						instances.Clear();
					}
				}

			if(!bwInstances.CancellationPending && instances.Count > 0)
				bwInstances.ReportProgress(MaxItems, instances.ToArray());
		}

		private void bwInstances_ProgressChanged(Object sender, ProgressChangedEventArgs e)
		{
			KeyValuePair<String, Object>[][] instances = (KeyValuePair<String, Object>[][])e.UserState;
			foreach(KeyValuePair<String, Object>[] instanceRow in instances)
			{
				ListViewItem listItem = new ListViewItem(new String[instanceRow.Length]);
				foreach(KeyValuePair<String, Object> row in instanceRow)
				{
					ColumnHeader header = null;
					foreach(ColumnHeader col in lvInstances.Columns)
						if(col.Text == row.Key)
						{
							header = col;
							break;
						}
					if(header == null)
						lvInstances.Columns.Add(header = new ColumnHeader() { Text = row.Key, });

					listItem.SubItems[header.Index].Text = row.Value == null ? "null" : row.Value.ToString();
				}

				listItem.Tag = String.Join(",", Array.ConvertAll(instanceRow, item => item.Key + "='" + item.Value + "'"));
				lvInstances.Items.Add(listItem);
			}
		}

		private void bwInstances_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
		{
			if(e.Error != null)
			{
				PluginWindows.Trace.TraceData(TraceEventType.Error, 10, e.Error);
				this.FinishedLoading(e.Error.Message);
			} else
			{
				if(lvInstances.Items.Count > 0)
				{
					splitMethod.Panel1Collapsed = false;
					lvInstances.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
				} else
					splitMethod.Panel1Collapsed = true;
				this.FinishedLoading();
			}

			//tsbnInvoke.Enabled = lvInstances.Items.Count > 0;
		}

		private void splitMain_MouseDoubleClick(Object sender, MouseEventArgs e)
		{
			if(splitMain.SplitterRectangle.Contains(e.Location))
				splitMain.Panel2Collapsed = true;
		}

		protected override void Run_Click(Object sender, EventArgs e)
		{
			MethodItemDescription method = this.MethodName;
			if(method == null)
				return;

			if(this._observer.InProgress)
			{//Cancel loading of the list of WMI class instances
				this._observer.Cancel();
				while(this._observer.InProgress)
				{
					Application.DoEvents();
					System.Threading.Thread.Sleep(1000);
				}
			}

			WmiDataRow[] inParameters = new WmiDataRow[gvParameters.Count];
			for(Int32 loop = 0; loop < inParameters.Length; loop++)
				inParameters[loop] = gvParameters.GetRowItem(loop);

			try
			{
				this.LoadBase("method");
				using(WmiDataClass data = this.CreateWmiDataClass(method.IsStatic))//Static methods invoked without instance
					if(method.Method != null)
					{
						PropertyData[] result = data.InvokeMethod(method.Method.Name, inParameters).ToArray();
						lvResult.AddEvent(this.WmiPath.CreatePath(method), result);
						splitMain.Panel2Collapsed = false;
					} else if(method.Property != null)
						data.SetPropertyValue(method.Property.Name, inParameters[0].Value);
					else throw new NotImplementedException();
				this.FinishedLoading();
			} catch(Exception exc)
			{
				this.FinishedLoading(exc.Message);
				throw;
			}
		}

		protected override WmiFormatDto GetCodeFormat()
		{
			MethodItemDescription method = this.MethodName;
			if(method == null)
				return null;

			Object[] arguments = new Object[gvParameters.Count];
			for(Int32 loop = 0; loop < arguments.Length; loop++)
				arguments[loop] = gvParameters.GetRowItem(loop).Value;

			String[] properties = null;
			if(method.Method != null && method.Method.OutParameters.Properties != null)
			{
				properties = new String[method.Method.OutParameters.Properties.Count];
				Int32 loop = 0;
				foreach(PropertyData prop in method.Method.OutParameters.Properties)
					properties[loop++] = prop.Name;
			}

			return new WmiFormatDto(Constant.TemplateType.Method)
			{
				Method = method,
				Path = method.IsStatic ? base.WmiPath : this.WmiPath,
				Arguments = arguments,
				Properties = properties,
				Query = null,
			};
		}
	}
}