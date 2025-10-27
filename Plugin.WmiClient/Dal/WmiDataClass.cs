using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading;
using Plugin.WmiClient.Dto;

namespace Plugin.WmiClient.Dal
{
	/// <summary>Represents a WMI class and provides access to its methods, properties, and qualifiers</summary>
	internal class WmiDataClass : WmiData
	{
		private ManagementObject _wmiCache;

		/// <summary>Initializes a new instance of the WmiDataClass</summary>
		/// <param name="path">WMI path containing namespace and class information</param>
		/// <param name="executionTimeout">Timeout for WMI operations</param>
		/// <param name="options">Connection options for WMI</param>
		/// <exception cref="ArgumentNullException">Thrown when path.NamespaceName or path.ClassName is null or empty</exception>
		public WmiDataClass(WmiPathItem path, TimeSpan executionTimeout, ConnectionOptions options)
			: base(path, executionTimeout, options)
		{
			if(String.IsNullOrEmpty(path.NamespaceName))
				throw new ArgumentNullException(nameof(path), "path.NamespaceName");
			if(String.IsNullOrEmpty(path.ClassName))
				throw new ArgumentNullException(nameof(path), "path.ClassName");
		}

		/// <summary>Gets or creates a cached WMI object instance</summary>
		/// <typeparam name="T">Type of ManagementObject to return</typeparam>
		/// <returns>Cached WMI object instance</returns>
		private T GetWmiObject<T>() where T : ManagementObject
		{
			if(this._wmiCache == null)
			{
				ManagementScope scope = this.GetWmiScope(this.Path.NamespaceName);
				ManagementPath path = new ManagementPath(this.Path.ClassName);
				ObjectGetOptions options = new ObjectGetOptions(null, base.ExecutionTimeout, true);

				this._wmiCache = path.IsClass
					? new ManagementClass(scope, path, options)
					: new ManagementObject(scope, path, options);
			}
			return (T)this._wmiCache;
		}

		/// <summary>Get WMI class description (Methods, Properties and Qualifiers)</summary>
		/// <returns>Detailed class information</returns>
		public ClassItemDescription GetClassDetails()
			=> new ClassItemDescription()
			{
				Properties = this.GetProperties().OrderBy(p => p.Name).ToArray(),
				Methods = this.GetMethods().OrderBy(p => p.Name).ToArray(),
				Qualifiers = this.GetQualifiers().OrderBy(p => p.Name).ToArray(),
			};

		/// <summary>Get class description</summary>
		/// <returns>Array of key-value pairs containing class description details</returns>
		public KeyValuePair<String, Object>[] GetClassDescription()
		{
			List<KeyValuePair<String, Object>> result = new List<KeyValuePair<String, Object>>();

			ManagementObject management = this.GetWmiObject<ManagementObject>();
			foreach(QualifierData qualifier in management.Qualifiers)
			{
				KeyValuePair<String, Object> item = new KeyValuePair<String, Object>(qualifier.Name, qualifier.Value);
				if(qualifier.Name == "Description")
					result.Insert(0, item);
				else
					result.Add(item);
			}

			foreach(PropertyData property in management.SystemProperties)
			{
				KeyValuePair<String, Object> item = new KeyValuePair<String, Object>(property.Name + (property.IsArray ? "[]" : String.Empty), property.Value);
				result.Add(item);
			}

			return result.ToArray();
		}

		/// <summary>Gets the description of a WMI property</summary>
		/// <param name="property">Property to get description for</param>
		/// <returns>Array of key-value pairs containing property description details</returns>
		public KeyValuePair<String, Object>[] GetPropertyDescription(PropertyData property)
		{
			List<KeyValuePair<String, Object>> result = new List<KeyValuePair<String, Object>>();

			Object value = property.Value ?? this.GetWmiObject<ManagementObject>().GetPropertyValue(property.Name);
			if(value != null)
				result.Add(new KeyValuePair<String, Object>("Value", value));

			result.Add(new KeyValuePair<String, Object>("Origin", property.Origin));

			foreach(QualifierData qualifier in property.Qualifiers)
			{
				KeyValuePair<String, Object> item = new KeyValuePair<String, Object>(qualifier.Name, qualifier.Value);
				if(item.Key == "Description")
					result.Insert(0, item);
				else
					result.Add(item);
			}

			return result.ToArray();
		}

		/// <summary>Gets the description of a WMI qualifier</summary>
		/// <param name="qualifier">Qualifier to get description for</param>
		/// <returns>Collection of key-value pairs containing qualifier properties</returns>
		public IEnumerable<KeyValuePair<String, Object>> GetQualifierDescription(QualifierData qualifier)
		{
			yield return new KeyValuePair<String, Object>("IsAmended", qualifier.IsAmended);
			yield return new KeyValuePair<String, Object>("IsLocal", qualifier.IsLocal);
			yield return new KeyValuePair<String, Object>("IsOverridable", qualifier.IsOverridable);
			yield return new KeyValuePair<String, Object>("PropagatesToInstance", qualifier.PropagatesToInstance);
			yield return new KeyValuePair<String, Object>("PropagatesToSubclass", qualifier.PropagatesToSubclass);
			if(qualifier.Value != null)
				yield return new KeyValuePair<String, Object>("Value", qualifier.Value);
		}

		/// <summary>Get WMI class properties</summary>
		/// <returns>Get all properties of a class</returns>
		public IEnumerable<PropertyData> GetProperties()
		{
			ManagementObject obj = this.GetWmiObject<ManagementObject>();
			foreach(PropertyData property in obj.Properties)
				yield return property;
		}

		/// <summary>Gets a specific property by name</summary>
		/// <param name="propertyName">Name of the property to retrieve</param>
		/// <returns>Property data if found, null otherwise</returns>
		public PropertyData GetProperty(String propertyName)
		{
			foreach(PropertyData property in this.GetWmiObject<ManagementObject>().Properties)
				if(property.Name.Equals(propertyName, StringComparison.Ordinal))
					return property;

			return null;
		}

		/// <summary>Get WMI class methods</summary>
		/// <returns>All methods of the selected class</returns>
		public IEnumerable<MethodData> GetMethods()
		{
			foreach(MethodData method in this.GetWmiObject<ManagementClass>().Methods)
				yield return method;
		}

		/// <summary>Gets a specific method by name</summary>
		/// <param name="methodName">Name of the method to retrieve</param>
		/// <returns>Method data if found, null otherwise</returns>
		private MethodData GetMethod(String methodName)
		{
			foreach(MethodData method in this.GetWmiObject<ManagementClass>().Methods)
				if(method.Name.Equals(methodName, StringComparison.Ordinal))
					return method;

			return null;
		}

		/// <summary>Gets all qualifiers of the WMI class</summary>
		/// <returns>Collection of qualifier data</returns>
		public IEnumerable<QualifierData> GetQualifiers()
		{
			foreach(QualifierData qualifier in this.GetWmiObject<ManagementObject>().Qualifiers)
				yield return qualifier;
		}

		/// <summary>Gets the description of a WMI method</summary>
		/// <param name="method">Method to get description for</param>
		/// <returns>Array of key-value pairs containing method description details</returns>
		public KeyValuePair<String, Object>[] GetMethodDescription(MethodData method)
		{
			List<KeyValuePair<String, Object>> result = new List<KeyValuePair<String, Object>>();

			result.Add(new KeyValuePair<String, Object>("Origin", method.Origin));
			foreach(QualifierData qualifier in method.Qualifiers)
			{
				KeyValuePair<String, Object> item = new KeyValuePair<String, Object>(qualifier.Name, qualifier.Value);
				if(item.Key == "Description")
					result.Insert(0, item);
				else
					result.Add(item);
			}

			if(method.InParameters != null)
			{
				List<String> arguments = new List<String>(method.InParameters.Properties.Count);
				foreach(PropertyData property in method.InParameters.Properties)
					arguments.Add(MethodItemDescription.FormatWmiProperty(property));
				result.Add(new KeyValuePair<String, Object>("InParameters", String.Join(", ", arguments.ToArray())));
			}
			if(method.OutParameters != null)
			{
				List<String> arguments = new List<String>(method.OutParameters.Properties.Count);
				foreach(PropertyData property in method.OutParameters.Properties)
					arguments.Add(MethodItemDescription.FormatWmiProperty(property));
				result.Add(new KeyValuePair<String, Object>("OutParameters", String.Join(", ", arguments.ToArray())));
			}

			return result.ToArray();
		}

		/// <summary>Check if the event corresponds to an extrinsic event</summary>
		/// <returns>True if the event is extrinsic, false otherwise</returns>
		public Boolean IsExtrinsicEvent()
		{
			PropertyData property = this.GetWmiObject<ManagementObject>().SystemProperties["__DERIVATION"];
			if(property.IsArray)
			{
				String[] derivationList = (String[])property.Value ?? new String[] { };
				foreach(String derivationClass in derivationList)
					if(derivationClass.Equals("__ExtrinsicEvent", StringComparison.OrdinalIgnoreCase))
						return true;
			}

			return false;
		}

		/// <summary>This object created as Singleton. It don't have any instances</summary>
		/// <returns>Returns true if object is singleton</returns>
		public Boolean IsSingleton()
		{
			ManagementObject obj = this.GetWmiObject<ManagementObject>();
			foreach(QualifierData qualifier in obj.Qualifiers)
				if(qualifier.Name.Equals("Singleton", StringComparison.OrdinalIgnoreCase))
					return (Boolean)qualifier.Value;
			return false;
		}

		/// <summary>Gets a value of the WMI property</summary>
		/// <param name="name">Name of the property</param>
		/// <returns>Value of the property</returns>
		public Object GetPropertyValue(String name)
		{
			if(this.IsSingleton())
			{
				ManagementObject singleton = this.GetWmiObject<ManagementClass>().GetInstances().Cast<ManagementObject>().Single();
				return singleton.GetPropertyValue(name);
			}else
				return this.GetWmiObject<ManagementObject>().GetPropertyValue(name);
		}

		/// <summary>Sets a value of the WMI property</summary>
		/// <param name="name">Name of the property</param>
		/// <param name="value">Value to set to WMI property</param>
		public void SetPropertyValue(String name,Object value)
			=> this.GetWmiObject<ManagementObject>().SetPropertyValue(name, value);

		/// <summary>Invokes WMI method in specified namespace</summary>
		/// <param name="methodName">Name of the method to invoke</param>
		/// <param name="args">Method arguments</param>
		/// <returns>Result of invoking method</returns>
		public Object InvokeMethod(String methodName, params PropertyData[] args)
		{
			Object[] values = Array.ConvertAll(args, (item) => { return item.Value; });
			return this.GetWmiObject<ManagementObject>().InvokeMethod(methodName, values);
		}

		/// <summary>Invoke WMI method with parameters</summary>
		/// <param name="methodName">Name of the method to invoke</param>
		/// <param name="inParams">Array of input parameters</param>
		/// <returns>Collection of output parameters from the method</returns>
		public IEnumerable<PropertyData> InvokeMethod(String methodName, params WmiDataRow[] inParams)
		{//\\\\MyServer\\root\\MyApp:MyClass.Key='abc'
			//TODO:root\CIMV2\Win32_ClusterShare\GetAccessMask
			ManagementObject obj = this.GetWmiObject<ManagementObject>();

			ManagementBaseObject wmiInParams = obj.GetMethodParameters(methodName);
			foreach(WmiDataRow param in inParams)
				if(param.Value != null)
					wmiInParams[param.Name] = param.Value;

			InvokeMethodOptions options = new InvokeMethodOptions(null, System.TimeSpan.MaxValue);

			using(ManagementBaseObject wmiOutParams = obj.InvokeMethod(methodName, wmiInParams, options))
				foreach(PropertyData outProperty in wmiOutParams.Properties)
					yield return outProperty;
		}

		/// <summary>Get all object instances that are in the selected class</summary>
		/// <returns>An array of object instances of the selected class</returns>
		public IEnumerable<KeyValuePair<String, Object>[]> GetMethodInstances()
		{
			if(this.IsSingleton())
				yield break;

			EnumerationOptions options = new EnumerationOptions() { Timeout = this.ExecutionTimeout, };
			foreach(ManagementObject wmiObject in this.GetWmiObject<ManagementClass>().GetInstances(options))
			{
				List<KeyValuePair<String, Object>> result = new List<KeyValuePair<String, Object>>();
				foreach(PropertyData property in wmiObject.Properties)
					foreach(QualifierData data in property.Qualifiers)
					{
						if(data.Name.Equals("key", StringComparison.Ordinal))
							result.Add(new KeyValuePair<String, Object>(property.Name, wmiObject.GetPropertyValue(property.Name)));
					}
				wmiObject.Dispose();

				if(result.Count > 0)
					yield return result.ToArray();
			}
		}

		/// <summary>Get all object instances that are in the selected class using an observer</summary>
		/// <param name="observer">Observer to receive instance notifications</param>
		public void GetMethodInstances(WmiObserver observer)
		{
			if(this.IsSingleton())
				return;

			EnumerationOptions options = new EnumerationOptions() { Timeout = this.ExecutionTimeout, };
			ManagementClass mClass = this.GetWmiObject<ManagementClass>();
			mClass.GetInstances(observer.GetObserver(), options);
		}

		/// <summary>Disposes of the managed resources used by this class</summary>
		public override void Dispose()
		{
			ManagementObject instance = Interlocked.Exchange(ref this._wmiCache, null);
			if(instance != null)
				instance.Dispose();

			base.Dispose();
		}
	}
}