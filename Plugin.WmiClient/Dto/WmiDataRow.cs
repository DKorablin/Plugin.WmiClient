using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;

namespace Plugin.WmiClient.Dal
{
	/// <summary>Represents a WMI data row for event subscription and parameter handling</summary>
	internal class WmiDataRow
	{
		/// <summary>Operation sign types for WQL queries</summary>
		public enum QuerySignType
		{
			/// <summary>Equals operator (=)</summary>
			Equal,
			/// <summary>Not equals operator (&lt;&gt;)</summary>
			NotEqual,
			/// <summary>Greater than operator (&gt;)</summary>
			More,
			/// <summary>Greater than or equal operator (&gt;=)</summary>
			MoreEqual,
			/// <summary>Less than operator (&lt;)</summary>
			Less,
			/// <summary>Less than or equal operator (&lt;=)</summary>
			LessEqual,
			/// <summary>LIKE operator for pattern matching</summary>
			Like,
			/// <summary>ISA operator for type checking</summary>
			ISA,
		}

		private Object _value;

		/// <summary>Maps equality signs to their string representation (Used in the events table)</summary>
		public static KeyValuePair<String, QuerySignType>[] Signs = new KeyValuePair<String, QuerySignType>[]
		{
			new KeyValuePair<String,QuerySignType>("=", QuerySignType.Equal),
			new KeyValuePair<String,QuerySignType>("<>",QuerySignType.NotEqual),
			new KeyValuePair<String,QuerySignType>(">",QuerySignType.More),
			new KeyValuePair<String,QuerySignType>(">=",QuerySignType.MoreEqual),
			new KeyValuePair<String,QuerySignType>("<",QuerySignType.Less),
			new KeyValuePair<String,QuerySignType>("<=",QuerySignType.LessEqual),
			new KeyValuePair<String,QuerySignType>("LIKE",QuerySignType.Like),
			new KeyValuePair<String,QuerySignType>("ISA",QuerySignType.ISA),
		};

		/// <summary>Gets or sets the WMI object type (CIM type)</summary>
		public CimType Type { get; set; }

		/// <summary>Gets the WMI object type name with array notation if applicable</summary>
		public String TypeName
			=> this.IsArray
				? this.Type.ToString() + "[]"
				: this.Type.ToString();

		/// <summary>Gets or sets the origin (parent) of the property</summary>
		public String Origin { get; set; }

		/// <summary>Gets or sets the filter name</summary>
		public String Name { get; set; }

		/// <summary>Gets or sets the operation sign for the query</summary>
		public QuerySignType Sign { get; set; }

		/// <summary>Gets or sets whether the value is an array</summary>
		public Boolean IsArray { get; set; }

		/// <summary>Gets or sets the user-defined value with type validation</summary>
		/// <exception cref="FormatException">Thrown when the value cannot be converted to the required type</exception>
		public Object Value
		{
			get => this._value;
			set
			{//TODO: Add array check
				switch(this.Type)
				{
				case CimType.Boolean:
					value = ParseToType<Boolean>(value);
					break;
				case CimType.Char16:
					value = ParseToType<Char>(value);
					break;
				case CimType.DateTime:
					value = ParseToType<DateTime>(value);
					break;
				case CimType.Real32:
				case CimType.Real64:
					break;
				case CimType.SInt16:
					value = ParseToType<Int16>(value);
					break;
				case CimType.SInt32:
					value = ParseToType<Int32>(value);
					break;
				case CimType.SInt64:
					value = ParseToType<Int64>(value);
					break;
				case CimType.SInt8:
				case CimType.UInt8://TODO: Verify
					value = ParseToType<Byte>(value);
					break;
				case CimType.UInt16:
					value = ParseToType<UInt16>(value);
					break;
				case CimType.UInt32:
					value = ParseToType<UInt32>(value);
					break;
				case CimType.UInt64:
					value = ParseToType<UInt64>(value);
					break;
				}

				this._value = value;
			}
		}

		/// <summary>Parses a value to a specified type with proper error handling</summary>
		/// <typeparam name="T">Target type to parse to</typeparam>
		/// <param name="value">Value to parse</param>
		/// <returns>Parsed value of type T</returns>
		/// <exception cref="FormatException">Thrown when the value cannot be parsed to the target type</exception>
		private static Object ParseToType<T>(Object value)
		{
			if(value == null)
				return null;

			if(value is T r)
				return r;

			if(typeof(T) == typeof(DateTime) && value is String s)
				return ManagementDateTimeConverter.ToDateTime(s);

			try
			{
				return (T)typeof(T).InvokeMember("Parse", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new Object[] { value.ToString(), });
			} catch(TargetInvocationException exc)
			{
				if(exc.InnerException is FormatException)
					throw new FormatException($"Value '{value}' could't be parsed to {typeof(T)}");
				else
					throw;
			}
		}

		/// <summary>Initializes a new instance of WmiDataRow from PropertyData</summary>
		/// <param name="item">PropertyData instance to initialize from</param>
		public WmiDataRow(PropertyData item)
		{
			this.Type = item.Type;
			this.Origin = item.Origin;
			this.Name = item.Name;
			this.IsArray = item.IsArray;
			this._value = item.Value;
			switch(this.Type)
			{
			case CimType.Object:
			case CimType.Reference:
				this.Sign = QuerySignType.ISA;
				break;
			default:
				this.Sign = QuerySignType.Equal;
				break;
			}
		}

		/// <summary>Initializes a new instance of WmiDataRow from PropertyData with a prefix</summary>
		/// <param name="item">PropertyData instance to initialize from</param>
		/// <param name="prefix">Prefix to add to the property name</param>
		public WmiDataRow(PropertyData item, String prefix)
			: this(item)
			=> this.Name = prefix + "." + this.Name;

		/// <summary>Gets the WQL operator sign as string</summary>
		/// <returns>String representation of the current query sign</returns>
		public String GetWqlSign()
			=> WmiDataRow.Signs.First(p => p.Value == this.Sign).Key;

		/// <summary>Formats the condition for WQL query</summary>
		/// <returns>Formatted WQL condition string or null if value is not set</returns>
		public String GetFormattedCondition()
		{
			if(this.Value == null)
				return null;

			String value;
			if(this.Sign == QuerySignType.ISA || this.Sign == QuerySignType.Like)
				value = '\'' + this.Value.ToString().Replace("\\", "\\\\") + '\'';
			else
				switch(this.Type)
				{
				case CimType.Boolean:
					value = ((Boolean)this.Value) == true ? "TRUE" : "FALSE";
					break;
				case CimType.String:
					value = '\'' + this.Value.ToString().Replace("\\", "\\\\") + '\'';
					break;
				case CimType.DateTime:
					value = '\'' + ManagementDateTimeConverter.ToDmtfDateTime((DateTime)this.Value) + '\'';
					break;
				case CimType.Object:
				default:
					value = this.Value.ToString();
					break;
				}

			return String.Join(" ", new String[] { this.Name, this.GetWqlSign(), value, });
		}
	}
}