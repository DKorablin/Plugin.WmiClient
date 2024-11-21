using System;
using System.Management;

namespace Plugin.WmiClient.Dto
{
	internal class ClassItemDescription
	{
		public PropertyData[] Properties { get; set; }

		public MethodData[] Methods { get; set; }

		public QualifierData[] Qualifiers { get; set; }
	}
}