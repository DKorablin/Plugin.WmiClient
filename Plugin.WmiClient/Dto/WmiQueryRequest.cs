using System;
using System.Management;

namespace Plugin.WmiClient.Dto
{
	internal class WmiQueryRequest
	{
		public WmiPathItem Path { get; set; }
		public ManagementQuery Query { get; set; }
	}
}