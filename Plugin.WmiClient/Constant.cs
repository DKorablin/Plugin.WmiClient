using System;
using System.Text;
using Plugin.WmiClient.Dto;

namespace Plugin.WmiClient
{
	internal static class Constant
	{
		public static class Wmi
		{
			public const String NamespaceRoot = "root";
			public const String MetaClass = "meta_class";
		}

		public enum TemplateType
		{
			Query,
			Event,
			Method,
		}

		public static class CSCode
		{
			/// <summary>BAT file header for dynamic compilation</summary>
			public const String Compiler = @"
/*
@echo off && cls
set WinDirNet=%WinDir%\Microsoft.NET\Framework
IF EXIST ""%WinDirNet%\v2.0.50727\csc.exe"" set csc=""%WinDirNet%\v2.0.50727\csc.exe""
IF EXIST ""%WinDirNet%\v3.5\csc.exe"" set csc=""%WinDirNet%\v3.5\csc.exe""
IF EXIST ""%WinDirNet%\v4.0.30319\csc.exe"" set csc=""%WinDirNet%\v4.0.30319\csc.exe""
%csc% /nologo /out:""%~0.exe"" %0
""%~0.exe""
del ""%~0.exe""
PAUSE
exit
*/
";

			public const String ClassBody = @"using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;

namespace AlphaOmega.Wmi
{
	public class WmiClient
	{
		{SourceCode}
	}
}

class Program
{
	static void Main(String[] args) { new AlphaOmega.Wmi.WmiClient().Invoke(); }
}";
			public const String WMIProperty = "Console.WriteLine(\"{Property} = \" + item.Properties[\"{Property}\"].Value);";

			public const String WMIQuery = @"
public void Invoke()
{
	foreach(ManagementObject item in Query(""{Namespace}"", ""{Query}""))
	{
		{Properties}
	}
}

private IEnumerable<ManagementObject> Query(String path, String query)
{
	using(ManagementObjectSearcher searcher = new ManagementObjectSearcher(path, query))
	{
		ManagementObjectCollection collection = searcher.Get();

		try
		{
			foreach(ManagementObject item in collection)
				yield return item;
		} finally
		{
			collection.Dispose();
		}
	}
}";

			public const String WMIEvent = @"
public void Invoke()
{
	using(ManagementEventWatcher watcher = Event(""{Namespace}"", ""{Query}""))
	{
		Console.WriteLine(""Listening..."");
		Console.ReadLine();
		watcher.Stop();
	}
}

private void EventArrived(Object sender, EventArrivedEventArgs e)
{
	ManagementBaseObject item = e.NewEvent;
	{Properties}
}

private ManagementEventWatcher Event(String path, String query)
{
	ManagementEventWatcher watcher = new ManagementEventWatcher(path, query);
	try
	{
		watcher.EventArrived += EventArrived;
		watcher.Start();
	} catch(ManagementException)
	{
		watcher.Dispose();
		throw;
	} catch(COMException)
	{
		watcher.Dispose();
		throw;
	}
	return watcher;
}";

			public const String WMIMethod = @"
public void Invoke()
{
	Object result = Method(""{Namespace}"", ""{Method}"",new Object[] { {Arguments} });
}

private Object Method(String path, String methodName, Object[] args)
{
	using(ManagementClass mClass = new ManagementClass(path))
		return mClass.InvokeMethod(methodName, args);
}";
			public static String FormatTemplate(WmiFormatDto dto)
			{
				String template;
				String args = null;
				String props = null;
				switch(dto.Type)
				{
				case TemplateType.Event:
					template = Constant.CSCode.WMIEvent;
					if(dto.Properties != null && dto.Properties.Length > 0)
						props = String.Join(Environment.NewLine, Array.ConvertAll<String, String>(dto.Properties, item => Constant.CSCode.WMIProperty.Replace("{Property}", item)));
					break;
				case TemplateType.Method:
					template = Constant.CSCode.WMIMethod;
					if(dto.Arguments != null && dto.Arguments.Length > 0)
						args = String.Join(",", Array.ConvertAll<Object, String>(dto.Arguments, FormatString));
					break;
				case TemplateType.Query:
					template = Constant.CSCode.WMIQuery;
					if(dto.Properties != null && dto.Properties.Length > 0)
						props = String.Join(Environment.NewLine, Array.ConvertAll<String, String>(dto.Properties, item => Constant.CSCode.WMIProperty.Replace("{Property}", item)));
					break;
				default:
					throw new NotImplementedException();
				}

				String result = template
					.Replace("{Namespace}", FormatString(dto.Path.CreatePath(dto.Method)))
					.Replace("{Class}", FormatString(dto.Path.ClassName))
					.Replace("{Query}", FormatString(dto.Query))
					.Replace("{Method}", dto.Method == null ? "null" : FormatString(dto.Method.Name))
					.Replace("{Arguments}", args ?? String.Empty)
					.Replace("{Properties}", props ?? String.Empty);

				return Constant.CSCode.Compiler
					+ Constant.CSCode.ClassBody
						.Replace("{SourceCode}", result);
			}
			private static String FormatString(Object value)
				=> value is String
					? "\"" + value.ToString().Replace("\\", "\\\\") + "\""
					: value?.ToString() ?? "null";
		}

		public static class PSCode
		{
			public const String WMIEventProperty = "\"{Property} = \",$Event.SourceEventArgs.NewEvent.Properties['{Property}'].Value";
			public const String CIMQuery = "Get-CimInstance  -Namespace {Namespace} -Query {Query} | Select-Object -Property {Properties}";
			public const String WMIQuery = "Get-WmiObject  -Namespace {Namespace} -Query {Query} | Select-Object -Property {Properties}";
			public const String WMIMethod = "Invoke-WmiMethod -Namespace {Namespace} -Path {Class} -Name {Method} -ArgumentList {Arguments}";
			public const String WMIEvent = @"
function Attach-WMIEvent()
{
	$query = {Query}

	$action = 
	{ 
		Write-Host ({Properties})
		Unregister-Event -SourceIdentifier ""Plugin.WmiClient.Event"" -Force
	}

	Register-WMIEvent -query $query -SourceIdentifier ""Plugin.WmiClient.Event"" -action $action
}

Attach-WMIEvent";

			public static String FormatTemplate(WmiFormatDto dto)
			{
				String template;
				String args = null;
				String props = null;
				switch(dto.Type)
				{
				case TemplateType.Event:
					template = Constant.PSCode.WMIEvent;
					if(dto.Properties != null && dto.Properties.Length > 0)
						props = String.Join("," + Environment.NewLine, Array.ConvertAll<String, String>(dto.Properties, item => Constant.PSCode.WMIEventProperty.Replace("{Property}", item)));
					break;
				case TemplateType.Method:
					template = Constant.PSCode.WMIMethod;
					if(dto.Arguments != null && dto.Arguments.Length > 0)
						args = String.Join(",", Array.ConvertAll<Object, String>(dto.Arguments, FormatString));
					if(dto.Properties != null && dto.Properties.Length > 0)
						props = String.Join(",", dto.Properties);
					break;
				case TemplateType.Query:
					template = Constant.PSCode.WMIQuery;
					if(dto.Properties != null && dto.Properties.Length > 0)
						props = String.Join(",", dto.Properties);
					break;
				default:
					throw new NotImplementedException();
				}

				String result = template
					.Replace("{Namespace}", FormatString(dto.Path.NamespaceName))
					.Replace("{Class}", FormatString(dto.Path.ClassName))
					.Replace("{Query}", FormatString(dto.Query))
					.Replace("{Method}", dto.Method == null ? "$null" : FormatString(dto.Method.Name))
					.Replace("{Arguments}", args ?? "$null")
					.Replace("{Properties}", props ?? String.Empty);

				return result;
			}

			private static String FormatString(Object value)
				=> value is String
					? "\"" + value.ToString().Replace("\"", "`\"") + "\""
					: value?.ToString() ?? "$null";
		}
	}
}