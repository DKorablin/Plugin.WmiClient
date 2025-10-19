using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Plugin.WmiClient.Dal;
using Plugin.WmiClient.Events;

namespace Plugin.WmiClient.UI
{
	internal class ToolStripComboBoxClass : ToolStripComboBox
	{
		private sealed class WmiClassRequest
		{
			public String NamespaceName { get; set; }
			public WmiData.WmiFilterType FilterType { get; set; }
		}

		private System.ComponentModel.BackgroundWorker bwClassUpdate;
		private Boolean _selectedIndexChangedRequired = false;
		private Boolean _widthCalculated = false;

		public event EventHandler<FinishedLoadingEventArgs> FinishedLoading;

		[Browsable(false)]
		public PluginWindows Plugin { get; set; }

		public ToolStripComboBoxClass()
			: base("tscbClass")
		{
			base.AutoCompleteMode = AutoCompleteMode.Suggest;
			base.AutoCompleteSource = AutoCompleteSource.ListItems;

			this.InitializeComponent();
		}

		public Boolean LoadClasses(String namespaceName, WmiData.WmiFilterType filterType)
		{
			base.Text = String.Empty;
			if(String.IsNullOrEmpty(namespaceName))
			{
				this.ClearClasses();
				return false;
			}

			Boolean result = !bwClassUpdate.IsBusy;
			if(result)
			{
				if(this.Plugin == null)
					throw new InvalidOperationException("this.Plugin is null");

				bwClassUpdate.RunWorkerAsync(new WmiClassRequest() { NamespaceName = namespaceName, FilterType = filterType, });
			}

			return result;
		}

		public void ClearClasses()
		{
			base.Items.Clear();
			base.Enabled = false;
			base.Text = String.Empty;
		}

		private void InitializeComponent()
		{
			this.bwClassUpdate = new BackgroundWorker();
			// 
			// bwClassUpdate
			// 
			this.bwClassUpdate.DoWork += new DoWorkEventHandler(this.bwClassUpdate_DoWork);
			this.bwClassUpdate.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bwClassUpdate_RunWorkerCompleted);
		}

		private void bwClassUpdate_DoWork(Object sender, DoWorkEventArgs e)
		{
			WmiClassRequest request = (WmiClassRequest)e.Argument;
			using(WmiData wmi = this.Plugin.CreateWmiData())
			{
				IEnumerable<String> classes = wmi.GetClasses(request.NamespaceName, request.FilterType);
				e.Result = classes.ToArray();
			}
		}

		private void bwClassUpdate_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
		{
			base.Parent.SuspendLayout();
			try
			{
				if(e.Error != null)
				{//TODO: Pass "Access Denied" exceptions to UI
					PluginWindows.Trace.TraceData(TraceEventType.Error, 10, e.Error);
					base.DroppedDown = false;
				} else if(e.Result != null)//+Cancelled
				{
					String[] items = (String[])e.Result;
					base.Items.Clear();
					if(items.Length == 0)
					{
						base.Enabled = false;
						base.Text = String.Empty;
					} else
					{
						base.Enabled = true;
						base.Items.AddRange(items);

						String selectedClass = base.Text;
						if(items.Contains(selectedClass))
							base.OnSelectedIndexChanged(EventArgs.Empty);
						else
							base.Text = String.Empty;
					}
				}
			} finally
			{
				this._widthCalculated = false;
				this.FinishedLoading?.Invoke(this, new FinishedLoadingEventArgs() { Error = e.Error, });
				base.Parent.ResumeLayout(false);
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
			case Keys.Return:
				if(this.DroppedDown
					&& this._oldSelectedIndex != null && this._oldSelectedIndex != this.SelectedIndex)
				{
					this.DroppedDown = false;
					e.Handled = true;
				}
				break;
			}
			base.OnKeyDown(e);
		}

		private Int32? _oldSelectedIndex;
		protected override void OnDropDown(EventArgs e)
		{
			base.OnDropDown(e);

			if(!this._widthCalculated)
			{
				Utils.SetDropDownListWidth(this);
				this._widthCalculated = true;
			}
			this._oldSelectedIndex = this.SelectedIndex;

			//Bypass double AutoComplete mode when DDL is opened and user inputting text
			base.AutoCompleteMode = AutoCompleteMode.None;
		}

		protected override void OnDropDownClosed(EventArgs e)
		{
			base.OnDropDownClosed(e);
			base.AutoCompleteMode = AutoCompleteMode.Suggest;

			if(this._oldSelectedIndex != null && this._oldSelectedIndex != this.SelectedIndex)
			{//Without this hack I can't turn off AutoCompleteMode on DropDown opened
				this.OnSelectedIndexChanged(EventArgs.Empty);
			}
		}

		protected override void OnTextUpdate(EventArgs e)
		{
			this._selectedIndexChangedRequired = true;

			base.OnTextUpdate(e);
		}

		protected override void OnEnter(EventArgs e)
		{
			this._selectedIndexChangedRequired = false;

			base.OnEnter(e);
		}

		protected override void OnLeave(EventArgs e)
		{
			if(this._selectedIndexChangedRequired)//We dispatch the namespace change event only after the control is exited. Otherwise, the event will be triggered on every click.
				this.OnSelectedIndexChanged(EventArgs.Empty);

			base.OnLeave(e);
		}

		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			if(this.DroppedDown)
				return;//Ignore event if DDL is opened. Waiting for closing event

			this._oldSelectedIndex = null;
			this._selectedIndexChangedRequired = false;//Without this, after Enter key pressed and leaving control triggers OnLeave event
			base.OnSelectedIndexChanged(e);
		}
	}
}