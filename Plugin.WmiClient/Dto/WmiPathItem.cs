using System;

namespace Plugin.WmiClient.Dto
{
	internal class WmiPathItem
	{
		private String _className;

		public String MachineName { get; private set; }

		public String NamespaceName { get; private set; }

		public String ClassName
		{
			get => this._className;
			set => this._className = String.IsNullOrEmpty(value) ? null : value;
		}

		public WmiPathItem(String machineName, String namespaceName, String className)
		{
			this.MachineName = machineName;
			if(!String.IsNullOrEmpty(namespaceName))
				this.NamespaceName = namespaceName;
			this.ClassName = className;
		}

		public String CreatePath(MethodItemDescription member = null)
		{
			String captionPart = null;
			if(member != null && (member.Method != null || member.Property != null))
				captionPart = this.NamespaceName + "\\" + this.ClassName + "\\" + member.Name;
			else if(!String.IsNullOrEmpty(this.ClassName))
				captionPart = this.NamespaceName + "\\" + this.ClassName;
			else if(!String.IsNullOrEmpty(this.NamespaceName))
				captionPart = this.NamespaceName;

			String result = captionPart == null
				? null
				: "\\\\" + (this.MachineName ?? ".") + "\\" + captionPart;

			return result;
		}
	}
}