using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;

namespace Plugin.WmiClient.Dal
{
	/// <summary>Данные для подписывание на WMI событие</summary>
	internal class WmiDataRow
	{
		/// <summary>Знак операции</summary>
		public enum QuerySignType
		{

			Equal,
			NotEqual,
			More,
			MoreEqual,
			Less,
			LessEqual,
			Like,
			ISA,
		}

		private Object _value;

		/// <summary>Маппинг знаков равенства на строковое представление (Используется в таблице с событиями)</summary>
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

		/// <summary>Тип объекта со стороны WMI</summary>
		public CimType Type { get; set; }

		/// <summary>Тип объекта со стороны WMI + features</summary>
		public String TypeName
			=> this.IsArray
				? this.Type.ToString() + "[]"
				: this.Type.ToString();

		/// <summary>Родитель свойства</summary>
		public String Origin { get; set; }

		/// <summary>Наименование фильтра</summary>
		public String Name { get; set; }

		/// <summary>Знак операции</summary>
		public QuerySignType Sign { get; set; }

		/// <summary>Массив значений</summary>
		public Boolean IsArray { get; set; }

		/// <summary>Пользовательское значение</summary>
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
				case CimType.UInt8://TODO: Проверить
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

		public WmiDataRow(PropertyData item,String prefix)
			:this(item)
			=> this.Name = prefix + "." + this.Name;

		public String GetWqlSign()
			=> WmiDataRow.Signs.First(p => p.Value == this.Sign).Key;

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