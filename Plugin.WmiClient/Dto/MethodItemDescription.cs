using System;
using System.Management;

namespace Plugin.WmiClient.Dto
{
	internal class MethodItemDescription
	{
		public String DisplayName { get; }

		public String Name
		{
			get
			{
				if(this.Method != null)
					return this.Method.Name;
				else if(this.Property != null)
					return this.Property.Name;
				else if(this.Qualifier != null)
					return this.Qualifier.Name;
				else
					throw new NotImplementedException();
			}
		}

		public PropertyData Property { get; private set; }
		
		public MethodData Method { get; private set; }

		public Boolean IsStatic { get; private set; }

		public QualifierData Qualifier { get; private set; }

		public MethodItemDescription(MethodData method)
		{
			_ = method ?? throw new ArgumentNullException(nameof(method));

			this.Method = method;
			this.IsStatic = MethodItemDescription.CheckStatic(method.Qualifiers);

			this.DisplayName = MethodItemDescription.FormatWmiMethod(method);
		}

		public MethodItemDescription(PropertyData property)
		{
			_ = property ?? throw new ArgumentNullException(nameof(property));

			this.Property = property;
			this.DisplayName = MethodItemDescription.FormatWmiProperty(property);
		}

		public MethodItemDescription(QualifierData qualifier)
		{
			_ = qualifier ?? throw new ArgumentNullException(nameof(qualifier));

			this.Qualifier = qualifier;
			this.DisplayName = qualifier.Name;
		}

		internal static String FormatWmiMethod(MethodData method)
			=> FormatWmiMethod(method, CheckStatic(method.Qualifiers));

		private static String FormatWmiMethod(MethodData method, Boolean isStatic)
		{
			String[] args = method.InParameters == null
				? new String[] { }
				: new String[method.InParameters.Properties.Count];

			if(args.Length > 0)
			{
				Int32 loop = 0;
				foreach(PropertyData property in method.InParameters.Properties)
					args[loop++] = MethodItemDescription.FormatWmiProperty(property);
			}

			return method.Name + "(" + String.Join(", ", args) + ")" + (isStatic ? " [static]" : String.Empty);
		}

		internal static String FormatWmiProperty(PropertyData property)//TODO: Is WMI have static properties???
			=> property.Type
				+ (property.IsArray ? "[]" : String.Empty)
				+ " "
				+ property.Name;

		private static Boolean CheckStatic(QualifierDataCollection qualifiers)
		{
			foreach(QualifierData item in qualifiers)
				if(String.Equals(item.Name, "static", StringComparison.OrdinalIgnoreCase))
					return true;
			return false;
		}
	}
}