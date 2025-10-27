using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using Plugin.WmiClient.Events;

namespace Plugin.WmiClient.Dal
{
	/// <summary>Manages WMI event subscriptions and notifications</summary>
	internal class WmiDataEvent : IDisposable
	{
		/// <summary>List of active WMI event watchers</summary>
		private List<ManagementEventWatcher> _events;

		/// <summary>Gets the count of all registered WQL events</summary>
		/// <returns>Number of registered event watchers</returns>
		public Int32 Count => this._events == null ? 0 : this._events.Count;

		/// <summary>Event raised when a WMI event notification is received</summary>
		public event EventHandler<WmiEventArrivedEventArgs> WmiEventArrived;

		/// <summary>Adds new WQL subscription</summary>
		/// <param name="scope">Connection scope for WMI operations</param>
		/// <param name="query">WQL event query defining the event to monitor</param>
		/// <exception cref="ArgumentNullException">Thrown when scope or query is null</exception>
		/// <exception cref="ArgumentException">Thrown when an event with the same query is already registered</exception>
		/// <exception cref="ManagementException">Thrown when there's an error in WMI operation</exception>
		/// <exception cref="COMException">Thrown when there's an error in COM interaction</exception>
		public void AttachWmiEvent(ManagementScope scope, WqlEventQuery query)
		{
			_ = scope ?? throw new ArgumentNullException(nameof(scope), "Scope is null");
			_ = query ?? throw new ArgumentNullException(nameof(query), "WQL query is empty");

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

		/// <summary>Stops monitoring for a specific WQL event query</summary>
		/// <param name="query">The WQL query string identifying the event subscription</param>
		/// <exception cref="ArgumentNullException">Thrown when the specified query is not found</exception>
		public void StopWatcher(String query)
		{
			ManagementEventWatcher watcher = this.GetWatcher(query)
				?? throw new ArgumentNullException(nameof(query), String.Format("Watcher with query '{0}' not found", query));

			watcher.Stop();
		}

		/// <summary>Starts monitoring for a specific WQL event query</summary>
		/// <param name="query">The WQL query string identifying the event subscription</param>
		/// <exception cref="ArgumentNullException">Thrown when the specified query is not found</exception>
		public void StartWatcher(String query)
		{
			ManagementEventWatcher watcher = this.GetWatcher(query)
				?? throw new ArgumentNullException(nameof(query), String.Format("Watcher with query '{0}' not found", query));

			watcher.Start();
		}

		/// <summary>Remove WQL event by query</summary>
		/// <param name="query">WQL query string used to identify the event subscription</param>
		/// <exception cref="ArgumentNullException">Thrown when the specified query is not found or empty</exception>
		public void DetachWmiEvent(String query)
		{
			ManagementEventWatcher watcher = this.GetWatcher(query)
				?? throw new ArgumentNullException(nameof(query), String.Format("Watcher with query '{0}' not found", query));

			this._events.Remove(watcher);
			watcher.Stop();
			watcher.Dispose();

			PluginWindows.Trace.TraceInformation("WMI event detached: {0}", query);
		}

		/// <summary>Removes all WQL event subscriptions and cleans up resources</summary>
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

		/// <summary>Implements IDisposable and removes all event subscriptions</summary>
		public void Dispose()
			=> this.ClearEvents();

		/// <summary>Gets a WMI event watcher by its query string</summary>
		/// <param name="query">The WQL query string to search for</param>
		/// <returns>The ManagementEventWatcher if found; otherwise null</returns>
		/// <exception cref="ArgumentNullException">Thrown when query is null or empty</exception>
		private ManagementEventWatcher GetWatcher(String query)
		{
			if(String.IsNullOrEmpty(query))
				throw new ArgumentNullException(nameof(query), "WQL query is empty");

			Int32 queryHash = query.ToLowerInvariant().GetHashCode();
			return this._events.FirstOrDefault(p => p.Query.QueryString.ToLowerInvariant().GetHashCode() == queryHash);
		}

		/// <summary>Handler for WMI event notifications</summary>
		/// <param name="sender">The event source (ManagementEventWatcher)</param>
		/// <param name="e">Event arguments containing the WMI event data</param>
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