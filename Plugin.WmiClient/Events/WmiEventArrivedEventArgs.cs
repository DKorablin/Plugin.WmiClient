using System;
using System.Management;

namespace Plugin.WmiClient.Events
{
	internal class WmiEventArrivedEventArgs : EventArgs
	{
		public ManagementPath Path { get; private set; }
		public EventQuery Query { get; private set; }
		public ManagementBaseObject NewEvent { get; private set; }

		public WmiEventArrivedEventArgs(ManagementPath path, EventQuery query, ManagementBaseObject newEvent)
		{
			this.Path = path;
			this.Query = query;
			this.NewEvent = newEvent;
		}
	}
}