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
using SAL.Windows;

namespace Plugin.WmiClient
{
	internal partial class PanelWmiEvent : PanelWmiBase
	{
		enum SubscribtionsImageIndex
		{
			Started = 0,
			Stopped = 1,
		}

		protected override WmiData.WmiFilterType ClassFilterType => WmiData.WmiFilterType.Events;

		private WmiDataEvent _wmiEvents;
		private WmiDataEvent WmiEvents
		{
			get
			{
				if(this._wmiEvents == null)
				{
					this._wmiEvents = new WmiDataEvent();
					this._wmiEvents.WmiEventArrived += WmiEvents_WmiEventArrived;
				}
				return this._wmiEvents;
			}
		}

		public PanelWmiEvent()
			: base("WMI Events")
			=> InitializeComponent();

		protected override void OnCreateControl()
		{
			gvParameters.Plugin = this.Plugin;
			lvEvents.Plugin = this.Plugin;

			txtQuery.Visible = false;
			txtQuery.Dock = DockStyle.Fill;
			gvParameters.Visible = true;
			gvParameters.Dock = DockStyle.Fill;
			lvSubscribtions.Visible = true;
			lvSubscribtions.Dock = DockStyle.Fill;
			lvEvents.Visible = false;
			lvEvents.Dock = DockStyle.Fill;
			lvEvents.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
			base.OnCreateControl();
		}

		protected override void FinishedLoading(String readyMessage = null)
		{
			if(readyMessage == null)
				readyMessage = this._wmiEvents == null || this._wmiEvents.Count == 0
					? "Ready"
					: "Listening...";
			base.FinishedLoading(readyMessage);
		}

		protected override void Window_Closed(Object sender, EventArgs e)
			=> this._wmiEvents?.Dispose();

		protected override void Class_SelectedIndexChanged(Object sender, EventArgs e)
		{
			WmiPathItem path = this.WmiPath;
			using(WmiDataClass wmi = this.Plugin.CreateWmiDataClass(path))
			{
				IEnumerable<WmiDataRow> rows = wmi.GetProperties().Select(p => new WmiDataRow(p));
				gvParameters.DataBind(path, rows);
			}

			base.Class_SelectedIndexChanged(sender, e);
		}

		private void tsbnQuery_Click(Object sender, EventArgs e)
		{
			if(tsbnQueryGrid.Checked)//Filling default parameters from the grid
			{
				WqlEventQuery query = this.GetWqlText();
				if(query != null)
					txtQuery.Text = query.QueryString;
			}

			txtQuery.Visible = tsbnQueryText.Checked = sender == tsbnQueryText;
			gvParameters.Visible =  tsbnQueryGrid.Checked = sender == tsbnQueryGrid;
		}

		private void tsbnEvents_Click(Object sender, EventArgs e)
		{
			lvSubscribtions.Visible = tsbnEventsSubscriptions.Checked = sender == tsbnEventsSubscriptions;
			lvEvents.Visible = tsbnEventsEvents.Checked = sender == tsbnEventsEvents;
		}

		private WqlEventQuery GetWqlText()
		{
			WqlEventQuery result;
			if(tsbnQueryGrid.Checked)
			{
				String[] conditions = gvParameters.GetWqlConditions();
				if(conditions==null)
					return null;

				WmiPathItem path = this.WmiPath;
				if(conditions.Length > 0)
				{
					String query = String.Join(" AND ", conditions.ToArray());
					result = new WqlEventQuery(path.ClassName, query);
				} else
					result = new WqlEventQuery(path.ClassName);

				using(WmiDataClass wmi = this.Plugin.CreateWmiDataClass(path))
					if(wmi.IsExtrinsicEvent())
						result.WithinInterval = new TimeSpan(0, 0, this.Plugin.Settings.PoolingInterval);

			} else if(tsbnQueryText.Checked)
				result = new WqlEventQuery(txtQuery.Text.Trim());
			else throw new NotImplementedException();

			return result;
		}

		protected override void Run_Click(Object sender, EventArgs e)
		{
			WqlEventQuery query = this.GetWqlText();
			if(query == null)
				return;

			try
			{
				this.LoadBase("adding event");
				ManagementScope scope = this.Plugin.CreateWmiScope(this.WmiPath);
				this.WmiEvents.AttachWmiEvent(scope, query);

				String path = scope.Path.Path + "\\" + query.EventClassName;
				ListViewGroup groupItem = lvSubscribtions.Groups.Cast<ListViewGroup>().FirstOrDefault(g => g.Header == path);
				if(groupItem == null)
				{
					groupItem = new ListViewGroup(path);
					lvSubscribtions.Groups.Add(groupItem);
				}

				String[] subItems = new String[lvSubscribtions.Columns.Count];
				subItems[colSubscriptionWql.Index] = query.QueryString;
				subItems[colSubscriptionDate.Index] = DateTime.Now.ToString();
				ListViewItem listEventItem = new ListViewItem(subItems)
				{
					ImageIndex = 0,
					StateImageIndex = 0,
					Group = groupItem,
					Tag = query.QueryString
				};
				lvSubscribtions.Items.Add(listEventItem);
				lvSubscribtions.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
				this.FinishedLoading();
			} catch(Exception exc)
			{
				this.FinishedLoading(exc.Message);
				throw;
			}
		}

		protected override WmiFormatDto GetCodeFormat()
		{
			ManagementQuery query = this.GetWqlText();
			if(query == null)
				return null;

			String[] properties = new String[gvParameters.Count];
			for(Int32 loop = 0; loop < properties.Length; loop++)
			{
				WmiDataRow row = gvParameters.GetRowItem(loop);
				properties[loop] = row.Name;
			}

			return new WmiFormatDto(Constant.TemplateType.Event)
			{
				Method = null,
				Path = this.WmiPath,
				Properties = properties,
				Arguments = null,
				Query = query.QueryString,
			};
		}

		private void WmiEvents_WmiEventArrived(Object sender, WmiEventArrivedEventArgs e)
			=> lvEvents.AddEvent(String.Format("{0}\\{1}", e.Path.Path, e.NewEvent.ClassPath), e.NewEvent.Properties.Cast<PropertyData>().ToArray());

		private void lvSubscribtions_KeyDown(Object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
			case Keys.A | Keys.Control:
				foreach(ListViewItem item in lvSubscribtions.Items)
					item.Selected = true;
				e.Handled = true;
				break;
			case Keys.Delete:
			case Keys.Back:
				this.cmsSubscriptions_ItemClicked(sender, new ToolStripItemClickedEventArgs(tsmiSubscriptionsDelete));
				e.Handled = true;
				break;
			}
		}

		private void cmsSubscriptions_Opening(Object sender, CancelEventArgs e)
		{
			Boolean? isStopped = null;
			foreach(ListViewItem item in lvSubscribtions.SelectedItems)
			{
				if(item.ImageIndex == (Int32)SubscribtionsImageIndex.Started)//Started
				{
					if(isStopped == null)
						isStopped = false;
					else if(isStopped == true)
					{
						isStopped = null;
						break;
					}
				} else if(item.ImageIndex == (Int32)SubscribtionsImageIndex.Stopped)//Stopped
				{
					if(isStopped == null)
						isStopped = true;
					else if(isStopped == false)
					{
						isStopped = null;
						break;
					}
				}
			}

			tsmiSubscriptionsStart.Enabled = isStopped == true;
			tsmiSubscriptionsStop.Enabled = isStopped == false;
		}

		private void cmsSubscriptions_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			if(e.ClickedItem == tsmiSubscriptionsDelete)
				while(lvSubscribtions.SelectedItems.Count > 0)
				{
					ListViewItem item = lvSubscribtions.SelectedItems[0];
					String query = (String)item.Tag;
					this.WmiEvents.DetachWmiEvent(query);
					item.Remove();
				}
			else if(e.ClickedItem == tsmiSubscriptionsStart)
				foreach(ListViewItem item in lvSubscribtions.SelectedItems)
				{
					String query = (String)item.Tag;
					this.WmiEvents.StartWatcher(query);
					item.ImageIndex = (Int32)SubscribtionsImageIndex.Started;
				}
			else if(e.ClickedItem == tsmiSubscriptionsStop)
				foreach(ListViewItem item in lvSubscribtions.SelectedItems)
				{
					String query = (String)item.Tag;
					this.WmiEvents.StopWatcher(query);
					item.ImageIndex = (Int32)SubscribtionsImageIndex.Stopped;
				}
		}
	}
}