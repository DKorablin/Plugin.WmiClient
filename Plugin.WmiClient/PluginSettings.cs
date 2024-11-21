using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management;

namespace Plugin.WmiClient
{
	public class PluginSettings : INotifyPropertyChanged
	{
		/// <summary>Generate script code</summary>
		public enum TemplateCode
		{
			/// <summary>PowerShell</summary>
			PS = 1,
			/// <summary>.NET Framework C#</summary>
			CS = 2,
		}

		private TimeSpan _connectionTimeout = ManagementOptions.InfiniteTimeout;
		private TimeSpan _executionTimeout = ManagementOptions.InfiniteTimeout;
		private String _machineName;
		private AuthenticationLevel _authentication = AuthenticationLevel.Packet;
		private Boolean _enablePrivileges = true;
		private ImpersonationLevel _impersonation = ImpersonationLevel.Impersonate;
		private String _locale;
		private String _authority;
		private String _userName;
		private String _password;
		private Boolean? _showAsHexValue;
		private SByte _maxArrayDisplay = 10;
		private Int32 _poolingInterval = 10;
		private TemplateCode _templateType = TemplateCode.PS;

		[Category("General")]
		[DefaultValue(typeof(TemplateCode),"PS")]
		public TemplateCode TemplateType
		{
			get => this._templateType;
			set => this.SetField(ref this._templateType, value, nameof(this.TemplateType));
		}
		
		[Category("Connection")]
		[Description("Max connection interval for WMI connection (Default value: ManagementOptions.InfiniteTimeout)")]
		public TimeSpan ConnectionTimeout
		{
			get => this._connectionTimeout;
			set => this.SetField(ref this._connectionTimeout, value, nameof(this.ConnectionTimeout));
		}

		[Category("Connection")]
		[Description("Remote machine name")]
		public String MachineName
		{
			get => this._machineName;
			set
			{
				if(value != null && value.Trim().Length == 0)
					value = null;

				this.SetField(ref this._machineName, value, nameof(this.MachineName));
			}
		}

		[Category("Connection")]
		[Description("Gets or sets the COM authentication level to be used for operations in this connection")]
		[DefaultValue(AuthenticationLevel.Packet)]
		public AuthenticationLevel Authentication
		{
			get => this._authentication;
			set => this.SetField(ref this._authentication, value, nameof(this.Authentication));
		}

		[Category("Connection")]
		[Description("Gets or sets a value indicating whether user privileges need to be enabled for the connection operation. This property should only be used when the operation performed requires a certain user privilege to be enabled (for example, a machine restart).")]
		[DefaultValue(true)]
		public Boolean EnablePrivileges
		{
			get => this._enablePrivileges;
			set => this.SetField(ref this._enablePrivileges, value, nameof(this.EnablePrivileges));
		}

		[Category("Connection")]
		[Description("Gets or sets the COM impersonation level to be used for operations in this connection.")]
		[DefaultValue(ImpersonationLevel.Impersonate)]
		public ImpersonationLevel Impersonation
		{
			get => this._impersonation;
			set => this.SetField(ref this._impersonation, value, nameof(this.Impersonation));
		}

		[Category("Connection")]
		[Description("Gets or sets the locale to be used for the connection operation.")]
		public String Locale
		{
			get => this._locale;
			set => this.SetField(ref this._locale, value, nameof(this.Locale));
		}

		[Category("Connection")]
		[Description("Gets or sets the authority to be used to authenticate the specified user (Specify ntlmdomain:{DomainName} for domain user)")]
		public String Authority
		{
			get => this._authority;
			set => this.SetField(ref this._authority, value, nameof(this.Authority));
		}

		[Category("Connection")]
		[Description("Remote credentials userName (Specify userName without domain for domain user)")]
		[DefaultValue(null)]
		public String UserName
		{
			get => this._userName;
			set => this.SetField(ref this._userName, value, nameof(this.UserName));
		}

		[Category("Connection")]
		[Description("Remote credentials password")]
		[PasswordPropertyText(true)]
		[DefaultValue(null)]
		public String Password
		{
			get => this._password;
			set => this.SetField(ref this._password, value, nameof(this.Password));
		}

		[Category("Query")]
		[Description("Max connection interval for WMI query (Default value: ManagementOptions.InfiniteTimeout)")]
		public TimeSpan ExecutionTimeout
		{
			get => this._executionTimeout;
			set => this.SetField(ref this._executionTimeout, value, nameof(this.ExecutionTimeout));
		}

		[Category("Query")]
		[DefaultValue(false)]
		[Description("Show integer value in hexadecimal format")]
		public Boolean ShowAsHexValue
		{
			get => this._showAsHexValue.GetValueOrDefault();
			set => this.SetField(ref this._showAsHexValue, value, nameof(this.ShowAsHexValue));
		}

		[Category("Query")]
		[DefaultValue(typeof(SByte), "10")]
		[Description("Maximum items in array to display")]
		public SByte MaxArrayDisplay
		{
			get => this._maxArrayDisplay;
			set => this.SetField(ref this._maxArrayDisplay, value, nameof(this.MaxArrayDisplay));
		}

		[DefaultValue(typeof(Int32), "10")]
		[Category("Event")]
		[DisplayName("Pooling interval (sec)")]
		[Description("A polling interval is the interval that Windows Management Instrumentation (WMI) uses to poll the data provider responsible for the class for intrinsic events, of which the event queried is a member. This interval is the maximum amount of time that can pass before notification of an event must be delivered. A consumer uses a polling interval in a WITHIN clause when the consumer requires notification of changes to a class, and an event provider is unavailable. The consumer registers for an intrinsic event and includes the polling interval.")]
		public Int32 PoolingInterval
		{
			get => this._poolingInterval;
			set => this.SetField(ref this._poolingInterval, value, nameof(this.PoolingInterval));
		}

		internal ConnectionOptions GetConnectionOptions()
		{
			/*
			 * Authentication: Packet
			 * Authority: ntlmdomain:{DomainName}
			 * EnablePriviledge: True
			 * Impersonation: Impersonate
			 * UserName: {UserName} without domain
			 */

			return new ConnectionOptions()
			{
				Authentication = this.Authentication,
				Authority = this.Authority,
				EnablePrivileges = this.EnablePrivileges,
				Impersonation = this.Impersonation,
				Locale = this.Locale,
				Password = this.Password,
				Username = this.UserName,
				Timeout = this.ConnectionTimeout,
			};
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private Boolean SetField<T>(ref T field, T value, String propertyName)
		{
			if(EqualityComparer<T>.Default.Equals(field, value))
				return false;

			field = value;
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
		#endregion INotifyPropertyChanged
	}
}