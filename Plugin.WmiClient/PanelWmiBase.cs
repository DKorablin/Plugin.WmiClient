using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Plugin.WmiClient.Dal;
using Plugin.WmiClient.Dto;
using Plugin.WmiClient.Events;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.WmiClient
{
	internal partial class PanelWmiBase : UserControl, IPluginSettings<PanelWmiBaseSettings>
	{
		private readonly String _caption = "WMI Base";
		private PanelWmiBaseSettings _settings;
		private Stopwatch _workTimer = new Stopwatch();

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected PluginWindows Plugin => (PluginWindows)this.Window.Plugin;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected IWindow Window => (IWindow)base.Parent;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		Object IPluginSettings.Settings => this.Settings;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PanelWmiBaseSettings Settings
			=> this._settings ?? (this._settings = new PanelWmiBaseSettings());

		protected virtual MethodItemDescription MethodName => null;
		protected virtual WmiData.WmiFilterType ClassFilterType => WmiData.WmiFilterType.None;
		protected virtual WmiPathItem WmiPath
			=> new WmiPathItem(this.Plugin.Settings.MachineName, tscbNamespace.Text, tscbClass.Text);

		public PanelWmiBase()
		{
			InitializeComponent();
			ssTimeSpent.Text = String.Empty;
			tsbnRun.Enabled = false;
		}

		public PanelWmiBase(String caption)
			: this()
			=> this._caption = caption;

		protected virtual WmiFormatDto GetCodeFormat()
			=> throw new NotImplementedException();

		protected override void OnCreateControl()
		{
			if(!this.DesignMode)
			{
				tscbNamespace.Plugin = this.Plugin;
				tscbClass.Plugin = this.Plugin;
				this.Window.Closed += Window_Closed;
				this.Plugin.Settings.PropertyChanged += Settings_PropertyChanged;
				tscbNamespace.SetNamespaceText(this.Settings.Namespace);
				this.SetCaption();
				this.tsMain_Resize(this, EventArgs.Empty);
			}

			base.OnCreateControl();
		}

		protected void DetachInvokeCodeButton(ToolStrip target)
		{
			tsMain.Items.Remove(tsbnRun);
			target.Items.Add(tsbnRun);
		}

		protected virtual void Window_Closed(Object sender, EventArgs e)
			=> this.Plugin.Settings.PropertyChanged -= Settings_PropertyChanged;

		protected virtual void Settings_PropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
			case nameof(PluginSettings.MachineName):
				tsbnRun.Enabled = false;
				tscbNamespace.ClearNamespaces();
				tscbClass.ClearClasses();
				this.SetCaption();
				break;
			}
		}

		private void tsMain_Resize(Object sender, EventArgs e)
		{
			Int32 controlsWidth = 5;
			foreach(ToolStripItem control in tsMain.Items)
				if(control.Visible && control != tscbClass)
					controlsWidth += control.Width + control.Margin.Horizontal;
			if(tsMain.OverflowButton.Visible)
				controlsWidth += tsMain.OverflowButton.Width + tsMain.OverflowButton.Margin.Horizontal;

			tscbClass.Width = tsMain.Width
				- (tsMain.GripRectangle.Width
				+ tsMain.Padding.Horizontal
				+ tsMain.GripMargin.Horizontal
				+ controlsWidth);

			tsMain.CanOverflow = tscbClass.Width <= 100;

			if(tsbnRun.Visible)
				tsbnRun.Alignment = ToolStripItemAlignment.Right;
		}

		private void Namespace_StartLoading(Object sender, EventArgs e)
			=> this.LoadBase("namespaces");

		protected virtual void Namespace_FinishedLoading(Object sender, FinishedLoadingEventArgs e)
		{
			if(e.Error == null)
				this.FinishedLoading();
			else
			{
				this.FinishedLoading(e.Error.Message);
				PluginWindows.Trace.TraceData(TraceEventType.Error, 10, e.Error);
			}
		}

		private void Namespace_SelectedIndexChanged(Object sender, EventArgs e)
		{
			tsbnRun.Enabled = false;
			this.Settings.Namespace = tscbNamespace.Text;
			if(tscbClass.LoadClasses(tscbNamespace.Text, this.ClassFilterType))
			{
				this.SetCaption();
				this.LoadBase("classes");
				tscbNamespace.Enabled = false;
			}
		}

		protected virtual void Class_FinishedLoading(Object sender, FinishedLoadingEventArgs e)
		{
			tscbNamespace.Enabled = true;
			if(tscbClass.Enabled)
				tscbClass.Focus();
			else//Set input focus to Namespaces if no classes was loaded
				tscbNamespace.Focus();

			if(e.Error == null)
				this.FinishedLoading();
			else
			{
				this.FinishedLoading(e.Error.Message);
				PluginWindows.Trace.TraceData(TraceEventType.Error, 10, e.Error);
			}
		}

		protected virtual void Class_SelectedIndexChanged(Object sender, EventArgs e)
		{
			tsbnRun.Enabled = true;
			this.SetCaption();
		}

		protected virtual void Run_Click(Object sender, EventArgs e)
			=> throw new NotSupportedException();

		private void RunCode_Click(Object sender, EventArgs e)
		{
			WmiFormatDto dto = this.GetCodeFormat();
			if(dto == null)
				return;

			String defaultExtension;
			switch(this.Plugin.Settings.TemplateType)
			{
			case PluginSettings.TemplateCode.CS:
				defaultExtension = "bat";
				break;
			case PluginSettings.TemplateCode.PS:
			default:
				defaultExtension = "ps1";
				break;
			}

			using(SaveFileDialog dlg = new SaveFileDialog() { OverwritePrompt = true, AddExtension = true, DefaultExt = defaultExtension, Filter = "PowerShell script (*.ps1)|*.ps1|C# batch (*.bat)|*.bat", })
				if(dlg.ShowDialog() == DialogResult.OK)
				{
					this.Plugin.Settings.TemplateType = (PluginSettings.TemplateCode)dlg.FilterIndex;
					switch(this.Plugin.Settings.TemplateType)
					{
					case PluginSettings.TemplateCode.CS://.bat
						String code1 = Constant.CSCode.FormatTemplate(dto);
						File.WriteAllText(dlg.FileName, code1);
						break;
					case PluginSettings.TemplateCode.PS://.ps1
						String code2 = Constant.PSCode.FormatTemplate(dto);
						File.WriteAllText(dlg.FileName, code2);
						break;
					default:
						throw new NotImplementedException();
					}
				}
		}

		private void SetCaption()
		{
			WmiPathItem path = this.WmiPath;
			String strPath = path.CreatePath(this.MethodName);
			this.Window.Caption = strPath == null
				? this._caption
				: this._caption + " - " + strPath;
		}

		protected virtual void LoadBase(String target)
		{
			base.Cursor = Cursors.WaitCursor;
			ssReady.Text = "Loading " + target + "...";
			this._workTimer.Reset();
			ssTimeSpent.Text = this._workTimer.Elapsed.ToString();
			this._workTimer.Start();
			timerWorking.Start();
		}

		protected virtual void FinishedLoading(String readyMessage = null)
		{
			base.Cursor = Cursors.Default;
			this._workTimer.Stop();
			timerWorking.Stop();
			ssTimeSpent.Text = this._workTimer.Elapsed.ToString();
			ssReady.Text = readyMessage ?? "Ready";
		}

		private void timerWorking_Tick(Object sender, EventArgs e)
			=> ssTimeSpent.Text = _workTimer.Elapsed.ToString();
	}
}