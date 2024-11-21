using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Reflection;
using System.Text;
using Plugin.WmiClient.Dal;
using Plugin.WmiClient.Dto;
using Plugin.WmiClient.Properties;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.WmiClient
{
	public class PluginWindows : IPlugin, IPluginSettings<PluginSettings>
	{
		#region Fields
		private static TraceSource _trace;
		private readonly IHostWindows _hostWindows;
		private PluginSettings _settings;
		private Dictionary<String, DockState> _documentTypes;

		private IMenuItem _menuWinApi;
		private IMenuItem _menuWmi;
		private IMenuItem _wmiQueryMenu;
		private IMenuItem _wmiEventMenu;
		private IMenuItem _wmiMethodMenu;
		private IMenuItem _wmiDescriptionMenu;
		#endregion Fields

		#region Properties
		internal static TraceSource Trace => _trace ?? (_trace = PluginWindows.CreateTraceSource<PluginWindows>());

		/// <summary>Настройки для взаимодействия из хоста</summary>
		Object IPluginSettings.Settings => this.Settings;

		/// <summary>Настройки для взаимодействия из плагина</summary>
		public PluginSettings Settings
		{
			get
			{
				if(this._settings == null)
				{
					this._settings = new PluginSettings();
					this._hostWindows.Plugins.Settings(this).LoadAssemblyParameters(this._settings);
				}
				return this._settings;
			}
		}

		private Dictionary<String, DockState> DocumentTypes
		{
			get
			{
				if(this._documentTypes == null)
					this._documentTypes = new Dictionary<String, DockState>()
					{
						{ typeof(PanelWqlQuery).ToString(), DockState.DockLeftAutoHide },
						{ typeof(PanelWmiEvent).ToString(), DockState.DockRightAutoHide },
						{ typeof(PanelWmiMethod).ToString(), DockState.DockRightAutoHide },
						{ typeof(PanelWmiDescription).ToString(), DockState.DockRightAutoHide },
					};
				return this._documentTypes;
			}
		}
		#endregion Properties

		public PluginWindows(IHostWindows hostWindows)
			=> this._hostWindows = hostWindows ?? throw new ArgumentNullException(nameof(hostWindows));

		public IWindow GetPluginControl(String typeName, Object args)
		{
			return this.CreateWindow(typeName, false, args);
		}

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			IMenuItem menuTools = this._hostWindows.MainMenu.FindMenuItem("Tools");
			if(menuTools == null)
			{
				PluginWindows.Trace.TraceEvent(TraceEventType.Error, 10, "Menu item 'Tools' not found");
				return false;
			}

			this._menuWinApi = menuTools.FindMenuItem("WinAPI");
			if(this._menuWinApi == null)
			{
				this._menuWinApi = menuTools.Create("WinAPI");
				this._menuWinApi.Name = "Tools.WinAPI";
				menuTools.Items.Add(this._menuWinApi);
			}

			this._menuWmi = this._menuWinApi.Create("WMI");
			this._menuWinApi.Items.Add(this._menuWmi);

			this._wmiQueryMenu = this._menuWmi.Create("WQL &Query");
			this._wmiQueryMenu.Name = "Tools.Test.Wmi.WmiQuery";
			this._wmiQueryMenu.Click += (sender, e) => { this.CreateWindow(typeof(PanelWqlQuery).ToString(), false); };
			this._menuWmi.Items.Add(this._wmiQueryMenu);

			this._wmiMethodMenu = this._menuWmi.Create("WMI &Method");
			this._wmiMethodMenu.Name = "tools.Test.Wmi.WmiMethod";
			this._wmiMethodMenu.Click += (sender, e) => { this.CreateWindow(typeof(PanelWmiMethod).ToString(), true); };
			this._menuWmi.Items.Add(this._wmiMethodMenu);

			this._wmiEventMenu = this._menuWmi.Create("WMI &Event");
			this._wmiEventMenu.Name = "Tools.Test.Wmi.WmiEvent";
			this._wmiEventMenu.Click += (sender, e) => { this.CreateWindow(typeof(PanelWmiEvent).ToString(), true); };
			this._menuWmi.Items.Add(this._wmiEventMenu);

			this._wmiDescriptionMenu = this._menuWmi.Create("De&scription");
			this._wmiDescriptionMenu.Name = "Tools.Test.Wmi.Description";
			this._wmiDescriptionMenu.Click += (sender, e) => { this.CreateWindow(typeof(PanelWmiDescription).ToString(), true); };
			this._menuWmi.Items.Add(this._wmiDescriptionMenu);

			return true;
		}

		Boolean IPlugin.OnDisconnection(DisconnectMode mode)
		{
			if(this._wmiQueryMenu != null)
				this._hostWindows.MainMenu.Items.Remove(this._wmiQueryMenu);

			if(this._wmiEventMenu != null)
				this._hostWindows.MainMenu.Items.Remove(this._wmiEventMenu);

			if(this._wmiMethodMenu != null)
				this._hostWindows.MainMenu.Items.Remove(this._wmiMethodMenu);

			if(this._wmiDescriptionMenu != null)
				this._hostWindows.MainMenu.Items.Remove(this._wmiDescriptionMenu);

			if(this._menuWmi != null && this._menuWmi.Items.Count == 0)
				this._hostWindows.MainMenu.Items.Remove(this._menuWmi);

			if(this._menuWinApi != null && this._menuWinApi.Items.Count == 0)
				this._hostWindows.MainMenu.Items.Remove(this._menuWinApi);
			return true;
		}

		private IWindow CreateWindow(String typeName, Boolean searchForOpened, Object args = null)
			=> this.DocumentTypes.TryGetValue(typeName, out DockState state)
				? this._hostWindows.Windows.CreateWindow(this, typeName, searchForOpened, state, args)
				: null;

		private static TraceSource CreateTraceSource<T>(String name = null) where T : IPlugin
		{
			TraceSource result = new TraceSource(typeof(T).Assembly.GetName().Name + name);
			result.Switch.Level = SourceLevels.All;
			result.Listeners.Remove("Default");
			result.Listeners.AddRange(System.Diagnostics.Trace.Listeners);
			return result;
		}

		internal WmiDataClass CreateWmiDataClass(WmiPathItem path)
			=> this.CreateWmiDataClass(path, path.ClassName);

		internal WmiDataClass CreateWmiDataClass(WmiPathItem path, String className)
		{
			path.ClassName = className;
			return new WmiDataClass(path, this.Settings.ExecutionTimeout, this.Settings.GetConnectionOptions());
		}

		internal WmiData CreateWmiData()
		{
			WmiPathItem path = new WmiPathItem(this.Settings.MachineName, null, null);
			return new WmiData(path, this.Settings.ExecutionTimeout, this.Settings.GetConnectionOptions());
		}

		internal ManagementScope CreateWmiScope(WmiPathItem path)
		{
			ConnectionOptions options = this.Settings.GetConnectionOptions();
			if(path.MachineName == null)
				options = null;

			ManagementScope scope = new ManagementScope(WmiData.GetFullNamespace(path.MachineName, path.NamespaceName), options);
			return scope;
		}

		internal String FormatValue(PropertyData property)
		{
			if(property == null || property.Value == null)
				return null;

			if(property.Type == CimType.DateTime && property.Value is String)
				return ManagementDateTimeConverter.ToDateTime((String)property.Value).ToString();
			if(property.Type == CimType.UInt64 && property.IsArray == false && property.Name == "TIME_CREATED" && property.Origin == "__Event")
				return DateTime.FromFileTime((Int64)(UInt64)property.Value).ToString();

			return this.FormatValue(property.Value);
		}

		internal String FormatValue(Object value)
			=> value == null ? null : this.FormatValue(value.GetType(), value);

		internal String FormatValue(MemberInfo info, Object value)
		{
			if(value == null)
				return null;

			Type type = info.GetMemberType();

			if(type.IsEnum)
				return value.ToString();
			else if(type == typeof(Char))
			{
				switch((Char)value)
				{
				case '\'':
					return "\\\'";
				case '\"':
					return "\\\"";
				case '\0':
					return "\\0";
				case '\a':
					return "\\a";
				case '\b':
					return "\\b";
				case '\f':
					return "\\b";
				case '\t':
					return "\\t";
				case '\n':
					return "\\n";
				case '\r':
					return "\\r";
				case '\v':
					return "\\v";
				default:
					return value.ToString();
				}
			} else if(value is IFormattable)
			{
				type = type.GetRealType();//INullable<Enum>
				if(type.IsEnum)
					return value.ToString();

				switch(Convert.GetTypeCode(value))
				{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					if(this.Settings.ShowAsHexValue)
						return "0x" + ((IFormattable)value).ToString("X", CultureInfo.CurrentCulture);
					else
						return ((IFormattable)value).ToString("n0", CultureInfo.CurrentCulture);
				default:
					return value.ToString();
				}
			} else
			{
				Type elementType = type.HasElementType ? type.GetElementType() : null;
				if(elementType != null && type.BaseType == typeof(Array))
					//&& elementType.IsPrimitive)//System.String[]
				{
					Int32 index = 0;
					Array arr = (Array)value;
					StringBuilder values = new StringBuilder($"{elementType}[{this.FormatValue(arr.Length)}]");
					if(this.Settings.MaxArrayDisplay > 0)
					{
						values.Append(" { ");
						foreach(Object item in arr)
						{
							if(index++ > this.Settings.MaxArrayDisplay)
							{
								values.Append("...");
								break;
							}

							values.Append((this.FormatValue(item) ?? Resources.NullString) + ", ");
						}
						values.Append("}");
					}
					return values.ToString();
				} else
					return value.ToString();
			}
		}
	}
}