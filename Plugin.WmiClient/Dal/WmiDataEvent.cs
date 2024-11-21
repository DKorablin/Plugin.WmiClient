using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using Plugin.WmiClient.Events;

namespace Plugin.WmiClient.Dal
{
	internal class WmiDataEvent : IDisposable
	{
		private List<ManagementEventWatcher> _events;

		/// <summary>Count of all registered WQL events</summary>
		public Int32 Count => this._events == null ? 0 : this._events.Count;

		/// <summary>Пришло событие от подписчика</summary>
		public event EventHandler<WmiEventArrivedEventArgs> WmiEventArrived;

		/// <summary>Adds new WQL subscription</summary>
		/// <param name="scope">Connection scope</param>
		/// <param name="query">WQL event query</param>
		public void AttachWmiEvent(ManagementScope scope, WqlEventQuery query)
		{
			if(scope == null)
				throw new ArgumentNullException(nameof(scope), "Scope is null");
			if(query == null)
				throw new ArgumentNullException(nameof(query), "WQL query is empty");

			if(this._events != null && this.GetWatcher(query.QueryString) != null)
				throw new ArgumentException(nameof(query), $"Event with query '{query.QueryString}' already added");

			ManagementEventWatcher watcher = new ManagementEventWatcher(scope, query);
			try
			{
				watcher.EventArrived += watcher_EventArrived;
				watcher.Start();
			} catch(ManagementException)
			{
				watcher.Dispose();
				throw;
			} catch(COMException)
			{
				watcher.Dispose();
				throw;
			}

			if(this._events == null)
				this._events = new List<ManagementEventWatcher>();

			this._events.Add(watcher);

			PluginWindows.Trace.TraceInformation("WMI event attached: {0}", query.QueryString);
		}

		public void StopWatcher(String query)
		{
			ManagementEventWatcher watcher = this.GetWatcher(query)
				?? throw new ArgumentNullException(nameof(query), String.Format("Watcher with query '{0}' not found", query));

			watcher.Stop();
		}

		public void StartWatcher(String query)
		{
			ManagementEventWatcher watcher = this.GetWatcher(query)
				?? throw new ArgumentNullException(nameof(query), String.Format("Watcher with query '{0}' not found", query));

			watcher.Start();
		}

		/// <summary>Remove WQL event by query</summary>
		/// <param name="query">WQL query key that is used as a key</param>
		public void DetachWmiEvent(String query)
		{
			ManagementEventWatcher watcher = this.GetWatcher(query)
				?? throw new ArgumentNullException(nameof(query), String.Format("Watcher with query '{0}' not found", query));

			this._events.Remove(watcher);
			watcher.Stop();
			watcher.Dispose();

			PluginWindows.Trace.TraceInformation("WMI event detached: {0}", query);
		}

		/// <summary>Removes all WQL events</summary>
		public void ClearEvents()
		{
			List<ManagementEventWatcher> watchers = this._events;
			this._events = null;

			if(watchers != null)
				foreach(ManagementEventWatcher watcher in watchers)
				{
					watcher.Stop();
					watcher.Dispose();
				}
		}

		public void Dispose()
			=> this.ClearEvents();

		private ManagementEventWatcher GetWatcher(String query)
		{
			if(String.IsNullOrEmpty(query))
				throw new ArgumentNullException(nameof(query), "WQL query is empty");

			Int32 queryHash = query.ToLowerInvariant().GetHashCode();
			return this._events.FirstOrDefault(p => p.Query.QueryString.ToLowerInvariant().GetHashCode() == queryHash);
		}

		private void watcher_EventArrived(Object sender, EventArrivedEventArgs e)
		{
			String classPath = e.NewEvent.ClassPath.ToString();
			PluginWindows.Trace.TraceEvent(TraceEventType.Information, 7, "WMI event from class '{0}' arrived", classPath);

			ManagementEventWatcher watcher = (ManagementEventWatcher)sender;
			WmiEventArrivedEventArgs args = new WmiEventArrivedEventArgs(watcher.Scope.Path, watcher.Query, e.NewEvent);
			this.WmiEventArrived?.Invoke(this, args);
		}
	}
}