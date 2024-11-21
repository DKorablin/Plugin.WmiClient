namespace Plugin.WmiClient
{
	partial class PanelWmiBase
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.panelMain = new System.Windows.Forms.Panel();
			this.ssMain = new System.Windows.Forms.StatusStrip();
			this.ssReady = new System.Windows.Forms.ToolStripStatusLabel();
			this.ssTimeSpent = new System.Windows.Forms.ToolStripStatusLabel();
			this.timerWorking = new System.Windows.Forms.Timer(this.components);
			this.tsMain = new System.Windows.Forms.ToolStrip();
			this.tscbNamespace = new Plugin.WmiClient.UI.ToolStripComboBoxNamespace();
			this.tscbClass = new Plugin.WmiClient.UI.ToolStripComboBoxClass();
			this.tsbnRun = new System.Windows.Forms.ToolStripSplitButton();
			this.tsmiRunCode = new System.Windows.Forms.ToolStripMenuItem();
			this.ssMain.SuspendLayout();
			this.tsMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelMain
			// 
			this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMain.Location = new System.Drawing.Point(0, 28);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(406, 116);
			this.panelMain.TabIndex = 3;
			// 
			// ssMain
			// 
			this.ssMain.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ssReady,
            this.ssTimeSpent});
			this.ssMain.Location = new System.Drawing.Point(0, 144);
			this.ssMain.Name = "ssMain";
			this.ssMain.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
			this.ssMain.Size = new System.Drawing.Size(406, 25);
			this.ssMain.TabIndex = 2;
			this.ssMain.Text = "statusStrip1";
			// 
			// ssReady
			// 
			this.ssReady.Name = "ssReady";
			this.ssReady.Size = new System.Drawing.Size(50, 20);
			this.ssReady.Text = "Ready";
			// 
			// ssTimeSpent
			// 
			this.ssTimeSpent.Name = "ssTimeSpent";
			this.ssTimeSpent.Size = new System.Drawing.Size(44, 20);
			this.ssTimeSpent.Text = "00:00";
			// 
			// timerWorking
			// 
			this.timerWorking.Interval = 1000;
			this.timerWorking.Tick += new System.EventHandler(this.timerWorking_Tick);
			// 
			// tsMain
			// 
			this.tsMain.CanOverflow = false;
			this.tsMain.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tscbNamespace,
            this.tscbClass,
            this.tsbnRun});
			this.tsMain.Location = new System.Drawing.Point(0, 0);
			this.tsMain.Name = "tsMain";
			this.tsMain.Size = new System.Drawing.Size(406, 28);
			this.tsMain.TabIndex = 0;
			this.tsMain.Resize += new System.EventHandler(this.tsMain_Resize);
			// 
			// tscbNamespace
			// 
			this.tscbNamespace.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.tscbNamespace.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.tscbNamespace.Name = "tscbNamespace";
			this.tscbNamespace.Size = new System.Drawing.Size(160, 28);
			this.tscbNamespace.Sorted = true;
			this.tscbNamespace.ToolTipText = "All WMI namespaces";
			this.tscbNamespace.StartLoading += new System.EventHandler<System.EventArgs>(this.Namespace_StartLoading);
			this.tscbNamespace.FinishedLoading += new System.EventHandler<Plugin.WmiClient.Events.FinishedLoadingEventArgs>(this.Namespace_FinishedLoading);
			this.tscbNamespace.SelectedIndexChanged += new System.EventHandler(this.Namespace_SelectedIndexChanged);
			// 
			// tscbClass
			// 
			this.tscbClass.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.tscbClass.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.tscbClass.AutoSize = false;
			this.tscbClass.Enabled = false;
			this.tscbClass.Name = "tscbClass";
			this.tscbClass.Size = new System.Drawing.Size(99, 28);
			this.tscbClass.Sorted = true;
			this.tscbClass.ToolTipText = "Classes in namespace (dynamic or static)";
			this.tscbClass.FinishedLoading += new System.EventHandler<Plugin.WmiClient.Events.FinishedLoadingEventArgs>(this.Class_FinishedLoading);
			this.tscbClass.SelectedIndexChanged += new System.EventHandler(this.Class_SelectedIndexChanged);
			// 
			// tsbnRun
			// 
			this.tsbnRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnRun.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiRunCode});
			this.tsbnRun.Image = global::Plugin.WmiClient.Properties.Resources.iconDebug;
			this.tsbnRun.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnRun.Name = "tsbnRun";
			this.tsbnRun.Size = new System.Drawing.Size(39, 25);
			this.tsbnRun.Text = "Run";
			this.tsbnRun.ButtonClick += new System.EventHandler(this.Run_Click);
			// 
			// tsmiRunCode
			// 
			this.tsmiRunCode.Name = "tsmiRunCode";
			this.tsmiRunCode.Size = new System.Drawing.Size(181, 26);
			this.tsmiRunCode.Text = "&Code";
			this.tsmiRunCode.Click += new System.EventHandler(this.RunCode_Click);
			// 
			// PanelWmiBase
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelMain);
			this.Controls.Add(this.tsMain);
			this.Controls.Add(this.ssMain);
			this.Name = "PanelWmiBase";
			this.Size = new System.Drawing.Size(406, 169);
			this.ssMain.ResumeLayout(false);
			this.ssMain.PerformLayout();
			this.tsMain.ResumeLayout(false);
			this.tsMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Plugin.WmiClient.UI.ToolStripComboBoxNamespace tscbNamespace;
		private Plugin.WmiClient.UI.ToolStripComboBoxClass tscbClass;
		private System.Windows.Forms.StatusStrip ssMain;
		private System.Windows.Forms.ToolStripStatusLabel ssReady;
		private System.Windows.Forms.ToolStripStatusLabel ssTimeSpent;
		private System.Windows.Forms.Timer timerWorking;
		protected System.Windows.Forms.Panel panelMain;
		private System.Windows.Forms.ToolStrip tsMain;
		protected System.Windows.Forms.ToolStripSplitButton tsbnRun;
		private System.Windows.Forms.ToolStripMenuItem tsmiRunCode;
	}
}
