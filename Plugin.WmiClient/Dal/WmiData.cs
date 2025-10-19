using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using Plugin.WmiClient.Dto;

namespace Plugin.WmiClient.Dal
{
	internal class WmiData : IDisposable
	{
		public enum WmiFilterType
		{
			None = 0,
			Classes = 1,
			ClassesWithMethods = 2,
			Events = 3,
		}

		private static readonly Dictionary<WmiFilterType, Func<ManagementClass, Boolean>> WmiFilters = new Dictionary<WmiFilterType, Func<ManagementClass, Boolean>>
		{
			{ WmiFilterType.None, null },
			{ WmiFilterType.Classes, WmiData.FilterClasses },
			{ WmiFilterType.ClassesWithMethods, WmiData.FilterClassesWithMethods },
			{ WmiFilterType.Events, WmiData.FilterEvents },
		};

		protected TimeSpan ExecutionTimeout { get; private set; }

		protected ConnectionOptions Options { get; private set; }

		public WmiPathItem Path { get; }

		public WmiData(WmiPathItem path, TimeSpan executionTimeout, ConnectionOptions options)
		{
			this.ExecutionTimeout = executionTimeout;
			this.Path = path;

			//Ignore connection options of it is a local machine or we will get Exception: System.Management.ManagementStatus.LocalCredentials
			if(this.Path.MachineName != null)
				this.Options = options;
		}

		public virtual void Dispose()
		{ }

		/// <summary>Get a list of all available namespaces for WMI</summary>
		/// <param name="namespaceRoot">Root namespace</param>
		/// <returns>Array of namespaces</returns>
		public IEnumerable<String> GetNamespacesRecursive(String namespaceRoot = Constant.Wmi.NamespaceRoot)
		{
			foreach(String namespaceName in this.GetNamespaces(namespaceRoot))
			{
				String fullPath = namespaceRoot + "\\" + namespaceName;
				yield return fullPath;

				foreach(String childNamespace in this.GetNamespacesRecursive(fullPath))
					yield return childNamespace;
			}
		}

		/// <summary>Get a list of all available namespaces for WMI</summary>
		/// <param name="namespaceRoot">Root namespace</param>
		/// <returns>Array of namespaces</returns>
		public IEnumerable<String> GetNamespaces(String namespaceRoot = Constant.Wmi.NamespaceRoot)
		{
			ManagementScope scope = this.GetWmiScope(namespaceRoot);
			/*scope.Connect();
			if(scope.IsConnected == false)
				throw new ArgumentException();*/

			ManagementPath path = new ManagementPath("__namespace");
			using(ManagementClass nsClass = new ManagementClass(scope, path, null))
			{
				ManagementObjectCollection collection = null;
				try
				{
					collection = nsClass.GetInstances();
				} catch(ManagementException exc)
				{
					String fullNamespace = this.GetFullNamespace(namespaceRoot);
					switch(exc.ErrorCode)
					{
					case ManagementStatus.AccessDenied:
						PluginWindows.Trace.TraceEvent(TraceEventType.Warning, (Int32)exc.ErrorCode, "{0}: Path: {1}", exc.Message, fullNamespace);
						yield break;
					case ManagementStatus.LocalCredentials:
						PluginWindows.Trace.TraceEvent(TraceEventType.Warning, (Int32)exc.ErrorCode, "{0}: Path: {1}", exc.Message, fullNamespace);
						yield break;
					default:
						throw;
					}
				}

				foreach(ManagementObject ns in collection)
				{
					String namespaceName = ns["Name"].ToString();
					yield return namespaceName;
				}
			}
		}

		/// <summary>Get an array of classes that are in the selected namespace</summary>
		/// <param name="filterType">Type of filter to apply</param>
		/// <returns>An array of classes that belong to the selected namespace</returns>
		public IEnumerable<String> GetClasses(String namespaceName, WmiFilterType filterType)
			=> this.GetClasses(namespaceName, WmiData.WmiFilters[filterType]);

		/// <summary>Execute query and returns dataset</summary>
		/// <param name="query">SELECT query to execute</param>
		/// <returns>Management objects</returns>
		public IEnumerable<ManagementObject> ExecuteQuery(String namespaceName, ManagementQuery query)
		{
			if(query == null || String.IsNullOrEmpty(query.QueryString))
				throw new ArgumentNullException(nameof(query));

			WqlObjectQuery wqlQuery = new WqlObjectQuery(query.QueryString);
			EnumerationOptions options = new EnumerationOptions() { Timeout = this.ExecutionTimeout, };
			using(ManagementObjectSearcher searcher = new ManagementObjectSearcher(this.GetWmiScope(namespaceName), wqlQuery, options))
			{
				ManagementObjectCollection collection = null;
				try
				{
					collection = searcher.Get();//ManagementException
					Int32 count = collection.Count;//OutOfMemoryException
				} catch(ManagementException)
				{
					throw;
				} catch(OutOfMemoryException)
				{
					PluginWindows.Trace.TraceEvent(TraceEventType.Error, 10, "WMI throws Out of memory Exception. Try to restart Windows Management Instrumentation service");
					throw;
				}

				try
				{
					foreach(ManagementObject item in collection)
						yield return item;
				}
				finally
				{
					collection.Dispose();
				}
			}
		}

		public void ExecuteQuery2(WmiObserver callback, ManagementQuery query)
		{
			if(query == null || String.IsNullOrEmpty(query.QueryString))
				throw new ArgumentNullException(nameof(query));

			WqlObjectQuery wqlQuery = new WqlObjectQuery(query.QueryString);
			EnumerationOptions options = new EnumerationOptions() { Timeout = this.ExecutionTimeout, };
			using(ManagementObjectSearcher searcher = new ManagementObjectSearcher(this.GetWmiScope(this.Path.NamespaceName), wqlQuery, options))
				try
				{
					searcher.Get(callback.GetObserver());//ManagementException
				} catch(ManagementException)
				{
					throw;
				} catch(OutOfMemoryException)
				{
					PluginWindows.Trace.TraceEvent(TraceEventType.Error, 10, "WMI throws Out of memory Exception. Try to restart Windows Management Instrumentation service");
					throw;
				}
		}

		protected ManagementScope GetWmiScope(String namespaceName)
		{
			String fullNamespace = this.GetFullNamespace(namespaceName);
			return new ManagementScope(fullNamespace, this.Options);
		}

		/// <summary>Get an array of classes in the selected namespace and filter them using the incoming method.</summary>
		/// <param name="filter">Filter applied to the array of classes.</param>
		/// <returns>An array of classes in the namespace that match the specified filter.</returns>
		private IEnumerable<String> GetClasses(String namespaceName, Func<ManagementClass, Boolean> filter)
		{
			WqlEventQuery query = new WqlEventQuery(Constant.Wmi.MetaClass);
			foreach(ManagementClass wmiItem in this.ExecuteQuery(namespaceName, query))
			{
				if(filter == null || filter(wmiItem))
					yield return wmiItem["__CLASS"].ToString();
				wmiItem.Dispose();
			}
		}

		/// <summary>Select all instances that are supported classes</summary>
		/// <param name="wmiClass">WMI class to check</param>
		/// <returns>Instance is a supported class</returns>
		private static Boolean FilterClasses(ManagementClass wmiClass)
		{
			foreach(QualifierData qd in wmiClass.Qualifiers)
			{
				switch(qd.Name)
				{
				case "dynamic":
				case "static":
					return true;
				}
			}

			return false;
		}

		/// <summary>Select all instances that contain methods</summary>
		/// <param name="wmiClass">WMI class to check</param>
		/// <returns>The class has methods</returns>
		private static Boolean FilterClassesWithMethods(ManagementClass wmiClass)
			=> FilterClasses(wmiClass) && wmiClass.Methods.Count > 0;

		/// <summary>Select all instances that are events</summary>
		/// <param name="wmiClass">WMI class to check</param>
		/// <returns>The class dispatches events</returns>
		private static Boolean FilterEvents(ManagementClass wmiClass)
			=> wmiClass.Derivation.Contains("__Event");

		public String GetFullNamespace(String namespaceName)
			=> GetFullNamespace(this.Path.MachineName, namespaceName);

		public static String GetFullNamespace(String machineName, String namespaceName)
			=> machineName == null
				? "\\\\.\\" + namespaceName
				: "\\\\" + machineName + "\\" + namespaceName;
	}
}