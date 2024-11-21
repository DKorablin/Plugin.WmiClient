using System;
using System.Management;

namespace Plugin.WmiClient.Events
{
	public class ObjectDeliveredEventArgs : EventArgs
	{
		public ManagementBaseObject[] NewObjects { get; private set; }

		public ObjectDeliveredEventArgs(params ManagementBaseObject[] newObjects)
			=> this.NewObjects = newObjects;
	}
}