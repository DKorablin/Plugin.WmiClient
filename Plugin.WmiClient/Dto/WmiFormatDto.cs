using System;

namespace Plugin.WmiClient.Dto
{
	internal class WmiFormatDto
	{
		public Constant.TemplateType Type { get; private set; }
		public WmiPathItem Path { get; set; }
		public String Query { get; set; }
		public MethodItemDescription Method { get; set; }
		public String[] Properties { get; set; }
		public Object[] Arguments { get; set; }

		public WmiFormatDto(Constant.TemplateType type)
			=> this.Type = type;
	}
}