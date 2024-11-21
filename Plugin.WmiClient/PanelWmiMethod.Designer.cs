namespace Plugin.WmiClient
{
	partial class PanelWmiMethod
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
			this.splitMethod = new System.Windows.Forms.SplitContainer();
			this.lvInstances = new AlphaOmega.Windows.Forms.SortableListView();
			this.gvParameters = new Plugin.WmiClient.UI.DataGridViewWmiParameters();
			this.error = new System.Windows.Forms.ErrorProvider(this.components);
			this.bwInstances = new System.ComponentModel.BackgroundWorker();
			this.tsMethods = new System.Windows.Forms.ToolStrip();
			this.tscbMethods = new System.Windows.Forms.ToolStripComboBox();
			this.splitMain = new System.Windows.Forms.SplitContainer();
			this.lvResult = new Plugin.WmiClient.UI.ListViewWmiResult();
			this.cmsResult = new AlphaOmega.Windows.Forms.ContextMenuStripCopy();
			this.splitMethod.Panel1.SuspendLayout();
			this.splitMethod.Panel2.SuspendLayout();
			this.splitMethod.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.error)).BeginInit();
			this.tsMethods.SuspendLayout();
			this.splitMain.Panel1.SuspendLayout();
			this.splitMain.Panel2.SuspendLayout();
			this.splitMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelMain
			// 
			this.panelMain.Size = new System.Drawing.Size(387, 143);
			// 
			// splitMethod
			// 
			this.splitMethod.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitMethod.Location = new System.Drawing.Point(0, 0);
			this.splitMethod.Margin = new System.Windows.Forms.Padding(4);
			this.splitMethod.Name = "splitMethod";
			this.splitMethod.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitMethod.Panel1
			// 
			this.splitMethod.Panel1.Controls.Add(this.lvInstances);
			// 
			// splitMethod.Panel2
			// 
			this.splitMethod.Panel2.Controls.Add(this.gvParameters);
			this.splitMethod.Size = new System.Drawing.Size(387, 55);
			this.splitMethod.SplitterDistance = 25;
			this.splitMethod.TabIndex = 4;
			// 
			// lvInstances
			// 
			this.lvInstances.AllowColumnReorder = true;
			this.lvInstances.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvInstances.FullRowSelect = true;
			this.lvInstances.HideSelection = false;
			this.lvInstances.Location = new System.Drawing.Point(0, 0);
			this.lvInstances.Margin = new System.Windows.Forms.Padding(4);
			this.lvInstances.MultiSelect = false;
			this.lvInstances.Name = "lvInstances";
			this.lvInstances.Size = new System.Drawing.Size(387, 25);
			this.lvInstances.TabIndex = 2;
			this.lvInstances.UseCompatibleStateImageBehavior = false;
			this.lvInstances.View = System.Windows.Forms.View.Details;
			this.lvInstances.SelectedIndexChanged += new System.EventHandler(this.lvInstances_SelectedIndexChanged);
			// 
			// gvParameters
			// 
			this.gvParameters.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gvParameters.Location = new System.Drawing.Point(0, 0);
			this.gvParameters.Margin = new System.Windows.Forms.Padding(4);
			this.gvParameters.Name = "gvParameters";
			this.gvParameters.Plugin = null;
			this.gvParameters.Size = new System.Drawing.Size(387, 26);
			this.gvParameters.TabIndex = 3;
			// 
			// error
			// 
			this.error.ContainerControl = this;
			// 
			// bwInstances
			// 
			this.bwInstances.WorkerReportsProgress = true;
			this.bwInstances.WorkerSupportsCancellation = true;
			this.bwInstances.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwInstances_DoWork);
			this.bwInstances.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwInstances_ProgressChanged);
			this.bwInstances.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwInstances_RunWorkerCompleted);
			// 
			// tsMethods
			// 
			this.tsMethods.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.tsMethods.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tscbMethods});
			this.tsMethods.Location = new System.Drawing.Point(0, 28);
			this.tsMethods.Name = "tsMethods";
			this.tsMethods.Size = new System.Drawing.Size(387, 28);
			this.tsMethods.TabIndex = 1;
			this.tsMethods.Resize += new System.EventHandler(this.tsMethods_Resize);
			// 
			// tscbMethods
			// 
			this.tscbMethods.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.tscbMethods.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.tscbMethods.AutoSize = false;
			this.tscbMethods.Enabled = false;
			this.tscbMethods.Name = "tscbMethods";
			this.tscbMethods.Size = new System.Drawing.Size(121, 28);
			this.tscbMethods.ToolTipText = "WMI Methods and Properties";
			this.tscbMethods.DropDown += new System.EventHandler(this.tscbMethods_DropDown);
			this.tscbMethods.SelectedIndexChanged += new System.EventHandler(this.tscbMethods_SelectedIndexChanged);
			// 
			// splitMain
			// 
			this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitMain.Location = new System.Drawing.Point(0, 56);
			this.splitMain.Name = "splitMain";
			this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitMain.MouseDoubleClick += splitMain_MouseDoubleClick;
			// 
			// splitMain.Panel1
			// 
			this.splitMain.Panel1.Controls.Add(this.splitMethod);
			// 
			// splitMain.Panel2
			// 
			this.splitMain.Panel2.Controls.Add(this.lvResult);
			this.splitMain.Size = new System.Drawing.Size(387, 115);
			this.splitMain.SplitterDistance = 55;
			this.splitMain.TabIndex = 5;
			// 
			// lvResult
			// 
			this.lvResult.AllowColumnReorder = true;
			this.lvResult.ContextMenuStrip = this.cmsResult;
			this.lvResult.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvResult.FullRowSelect = true;
			this.lvResult.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvResult.HideSelection = false;
			this.lvResult.Location = new System.Drawing.Point(0, 0);
			this.lvResult.Name = "lvResult";
			this.lvResult.Plugin = null;
			this.lvResult.Size = new System.Drawing.Size(387, 56);
			this.lvResult.TabIndex = 0;
			this.lvResult.UseCompatibleStateImageBehavior = false;
			this.lvResult.View = System.Windows.Forms.View.Details;
			// 
			// cmsResult
			// 
			this.cmsResult.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmsResult.Name = "cmsResult";
			this.cmsResult.Size = new System.Drawing.Size(113, 28);
			// 
			// PanelWmiMethod
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitMain);
			this.Controls.Add(this.tsMethods);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "PanelWmiMethod";
			this.Size = new System.Drawing.Size(387, 196);
			this.Controls.SetChildIndex(this.panelMain, 0);
			this.Controls.SetChildIndex(this.tsMethods, 0);
			this.Controls.SetChildIndex(this.splitMain, 0);
			this.splitMethod.Panel1.ResumeLayout(false);
			this.splitMethod.Panel2.ResumeLayout(false);
			this.splitMethod.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.error)).EndInit();
			this.tsMethods.ResumeLayout(false);
			this.tsMethods.PerformLayout();
			this.splitMain.Panel1.ResumeLayout(false);
			this.splitMain.Panel2.ResumeLayout(false);
			this.splitMain.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.DataGridViewWmiParameters gvParameters;
		private System.Windows.Forms.ErrorProvider error;
		private System.ComponentModel.BackgroundWorker bwInstances;
		private AlphaOmega.Windows.Forms.SortableListView lvInstances;
		private System.Windows.Forms.ToolStrip tsMethods;
		private System.Windows.Forms.ToolStripComboBox tscbMethods;
		private System.Windows.Forms.SplitContainer splitMethod;
		private System.Windows.Forms.SplitContainer splitMain;
		private UI.ListViewWmiResult lvResult;
		private AlphaOmega.Windows.Forms.ContextMenuStripCopy cmsResult;
	}
}