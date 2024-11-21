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
using Plugin.WmiClient.UI;

namespace Plugin.WmiClient
{
	internal partial class PanelWqlQuery : PanelWmiBase
	{
		private WmiObserver _observer;
		protected override WmiData.WmiFilterType ClassFilterType => WmiData.WmiFilterType.Classes;

		public PanelWqlQuery()
			: base("WQL Query")
			=> InitializeComponent();

		protected override void OnCreateControl()
		{
			this._observer = new WmiObserver();
			this._observer.OnObjectReady += Query_OnObjectReady;
			this._observer.OnCompleted += Query_OnCompleted;

			gvParameters.Plugin = this.Plugin;
			lvResult.Plugin = this.Plugin;

			txtQuery.Visible = false;
			txtQuery.Dock = DockStyle.Fill;
			gvParameters.Visible = true;
			gvParameters.Dock = DockStyle.Fill;

			base.OnCreateControl();
		}

		protected override void Window_Closed(Object sender, EventArgs e)
		{
			this._observer?.Dispose();

			base.Window_Closed(sender, e);
		}

		private void LoadQuery()
		{
			if(this._observer.InProgress)
			{
				this._observer.Cancel();
				return;
			}

			ManagementQuery query = this.GetWqlQuery();
			if(query == null)
				return;

			this.LoadBase("query");
			lvResult.Clear();

			WmiPathItem path = this.WmiPath;
			using(WmiData wmi = this.Plugin.CreateWmiData())
				wmi.ExecuteQuery2(this._observer, query);
		}

		private void Query_OnCompleted(Object sender, FinishedLoadingEventArgs e)
		{
			this.FinishedLoading(e.Status == ManagementStatus.NoError ? null : e.Status.ToString());
			lvResult.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		private void Query_OnObjectReady(Object sender, ObjectReadyEventArgs e)
			=> lvResult.AddManagementObject(e.NewObject);

		private ManagementQuery GetWqlQuery()
		{
			if(gvParameters.Visible)
			{
				WmiPathItem path = this.WmiPath;
				if(String.IsNullOrEmpty(path.ClassName))
					return null;

				String[] conditions = gvParameters.GetWqlConditions();
				if(conditions == null)
					return null;

				if(conditions.Length > 0)
				{
					String wql = String.Join(" AND ", conditions.ToArray());
					return new WqlEventQuery(path.ClassName, wql);
				} else
					return new WqlEventQuery(path.ClassName);
			} else if(txtQuery.Visible && txtQuery.Text.Trim().Length > 0)
				return new WqlEventQuery(txtQuery.Text);
			else throw new NotImplementedException();
		}

		private void tsbnQuery_Click(Object sender, EventArgs e)
		{
			if(tsbnQueryGrid.Checked)//Filling default parameters from the grid
			{
				ManagementQuery query = this.GetWqlQuery();
				txtQuery.Text = query == null ? null : query.QueryString;
			}

			txtQuery.Visible = tsbnQueryText.Checked = sender == tsbnQueryText;
			gvParameters.Visible = tsbnQueryGrid.Checked = sender == tsbnQueryGrid;
		}

		protected override void Run_Click(Object sender, EventArgs e)
			=> this.LoadQuery();

		protected override WmiFormatDto GetCodeFormat()
		{
			ManagementQuery query = this.GetWqlQuery();
			if(query == null)
				return null;

			String[] properties = new String[gvParameters.Count];
			for(Int32 loop = 0; loop < properties.Length; loop++)
			{
				WmiDataRow row = gvParameters.GetRowItem(loop);
				properties[loop] = row.Name;
			}

			return new WmiFormatDto(Constant.TemplateType.Query)
			{
				Method = null,
				Path = this.WmiPath,
				Properties = properties,
				Arguments = null,
				Query = query.QueryString,
			};
		}

		protected override void Namespace_FinishedLoading(Object sender, FinishedLoadingEventArgs e)
		{
			gvParameters.Clear();
			base.Namespace_FinishedLoading(sender, e);
		}

		protected override void Class_SelectedIndexChanged(Object sender, EventArgs e)
		{
			WmiPathItem path = this.WmiPath;
			using(WmiDataClass wmi = this.Plugin.CreateWmiDataClass(path))
			{
				IEnumerable<WmiDataRow> rows = wmi.GetProperties().Select(p => new WmiDataRow(p));
				gvParameters.DataBind(path, rows);
			}

			WqlEventQuery query = new WqlEventQuery(path.ClassName);
			txtQuery.Text = query.QueryString;
			txtQuery.Focus();

			base.Class_SelectedIndexChanged(sender, e);
		}

		private void bwQuery_DoWork(Object sender, DoWorkEventArgs e)
		{
			WmiQueryRequest request = (WmiQueryRequest)e.Argument;
			List<ListViewItem> listItems = new List<ListViewItem>();
			ColumnHeader[] listColumns = null;

			using(WmiData wmi = this.Plugin.CreateWmiData())
				foreach(ManagementObject item in wmi.ExecuteQuery(request.Path.NamespaceName, request.Query))
				{
					try
					{
						if(item is ManagementClass)
						{
							ManagementClass classItem = item as ManagementClass;
							if(listColumns == null)
								listColumns = new ColumnHeader[]{
									new ColumnHeader() { Text = "__CLASS", },
									new ColumnHeader() { Text = "Qualifiers", },
									new ColumnHeader() { Text = "Text", },
								};

							String[] columns = new String[listColumns.Length];
							columns[0] = classItem["__CLASS"].ToString();
							List<String> qualifiers = new List<String>(classItem.Qualifiers.Count);
							foreach(QualifierData qd in classItem.Qualifiers)
								qualifiers.Add(qd.Name);
							columns[1] = qualifiers.ToArray().ToString();
							columns[2] = classItem.GetText(TextFormat.Mof);

							ListViewItem listItem = new ListViewItem(columns);
							listItems.Add(listItem);

						} else
						{
							if(listColumns == null)
							{
								listColumns = new ColumnHeader[item.Properties.Count];
								Int32 index = 0;
								foreach(PropertyData property in item.Properties)
									listColumns[index++] = new ColumnHeader() { Text = property.Name };
							}

							List<String> columns = new List<String>(listColumns.Length);
							foreach(PropertyData property in item.Properties)
							{//Hope that properties will not change the order inside iteration
								String text = this.Plugin.FormatValue(property);
								columns.Add(text);
							}
							ListViewItem listItem = new ListViewItem(columns.ToArray());
							listItems.Add(listItem);
						}
					} finally
					{
						item.Dispose();
					}
				}

			if(listItems.Count > 0 || (listColumns != null && listColumns.Length > 0))
				e.Result = new QueryListResult(listItems.ToArray(), listColumns);
		}

		private void bwQuery_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
		{
			try
			{
				lvResult.SuspendLayout();
				lvResult.Clear();
				if(e.Error != null)
				{
					this.FinishedLoading(e.Error.Message);
					PluginWindows.Trace.TraceData(TraceEventType.Error, 10, e.Error);
				} else
				{
					this.FinishedLoading();
					if(e.Result != null)//+Cancelled
					{
						QueryListResult result = (QueryListResult)e.Result;
						lvResult.Columns.AddRange(result.Columns);
						lvResult.Items.AddRange(result.Items);
						lvResult.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
					}
				}
			} finally
			{
				tsbnRun.Enabled = true;
				lvResult.ResumeLayout();
			}
		}
	}
}