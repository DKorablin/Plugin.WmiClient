using System;
using System.Reflection;

namespace Plugin.WmiClient
{
	internal static class TypeExtender
	{
		public static Type GetMemberType(this MemberInfo member)
		{
			switch(member.MemberType)
			{
			case MemberTypes.Field:
				return ((FieldInfo)member).FieldType;
			case MemberTypes.Property:
				return ((PropertyInfo)member).PropertyType;
			case MemberTypes.Method:
				return ((MethodInfo)member).ReturnType;
			case MemberTypes.TypeInfo:
			case MemberTypes.NestedType:
				return (Type)member;
			default:
				throw new NotImplementedException();
			}
		}

		public static Type GetRealType(this Type type)
		{
			if(type.IsGenericType)
			{
				Type genericType = type.GetGenericTypeDefinition();
				if(genericType == typeof(System.Nullable<>)
					|| genericType == typeof(System.Collections.Generic.IEnumerator<>)
					|| genericType == typeof(System.Collections.Generic.IEnumerable<>)
					/*|| genericType == typeof(System.Collections.Generic.SortedList<,>)*/)
					return type.GetGenericArguments()[0].GetRealType();
			}
			if(type.HasElementType)
				//if(type.BaseType == typeof(Array))//+Для out и ref параметров
				return type.GetElementType().GetRealType();
			return type;
		}
	}
}