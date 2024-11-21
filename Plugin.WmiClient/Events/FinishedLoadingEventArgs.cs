using System;
using System.Management;

namespace Plugin.WmiClient.Events
{
	public class FinishedLoadingEventArgs : EventArgs
	{
		public Exception Error { get; set; }
		public ManagementStatus Status { get; set; }
	}
}