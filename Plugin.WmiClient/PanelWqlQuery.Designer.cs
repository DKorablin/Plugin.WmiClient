namespace Plugin.WmiClient
{
	partial class PanelWqlQuery
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
			System.Windows.Forms.SplitContainer splitMain;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelWqlQuery));
			System.Windows.Forms.ToolStrip tsQuery;
			this.gvParameters = new Plugin.WmiClient.UI.DataGridViewWmiParameters();
			this.txtQuery = new FastColoredTextBoxNS.FastColoredTextBox();
			this.tsbnQueryGrid = new System.Windows.Forms.ToolStripButton();
			this.tsbnQueryText = new System.Windows.Forms.ToolStripButton();
			this.lvResult = new Plugin.WmiClient.UI.ListViewWmiData();
			this.cmsResult = new AlphaOmega.Windows.Forms.ContextMenuStripCopy();
			this.bwQuery = new System.ComponentModel.BackgroundWorker();
			splitMain = new System.Windows.Forms.SplitContainer();
			tsQuery = new System.Windows.Forms.ToolStrip();
			splitMain.Panel1.SuspendLayout();
			splitMain.Panel2.SuspendLayout();
			splitMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtQuery)).BeginInit();
			tsQuery.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelMain
			// 
			this.panelMain.Size = new System.Drawing.Size(400, 206);
			// 
			// splitMain
			// 
			splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
			splitMain.Location = new System.Drawing.Point(0, 28);
			splitMain.Margin = new System.Windows.Forms.Padding(4);
			splitMain.Name = "splitMain";
			splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitMain.Panel1
			// 
			splitMain.Panel1.Controls.Add(this.gvParameters);
			splitMain.Panel1.Controls.Add(this.txtQuery);
			splitMain.Panel1.Controls.Add(tsQuery);
			// 
			// splitMain.Panel2
			// 
			splitMain.Panel2.Controls.Add(this.lvResult);
			splitMain.Size = new System.Drawing.Size(400, 206);
			splitMain.SplitterDistance = 99;
			splitMain.SplitterWidth = 5;
			splitMain.TabIndex = 1;
			// 
			// gvParameters
			// 
			this.gvParameters.Location = new System.Drawing.Point(29, 4);
			this.gvParameters.Margin = new System.Windows.Forms.Padding(4);
			this.gvParameters.Name = "gvParameters";
			this.gvParameters.Plugin = null;
			this.gvParameters.Size = new System.Drawing.Size(150, 87);
			this.gvParameters.TabIndex = 0;
			// 
			// txtQuery
			// 
			this.txtQuery.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
			this.txtQuery.AutoIndentCharsPatterns = "";
			this.txtQuery.AutoScrollMinSize = new System.Drawing.Size(2, 18);
			this.txtQuery.BackBrush = null;
			this.txtQuery.CharHeight = 18;
			this.txtQuery.CharWidth = 10;
			this.txtQuery.CommentPrefix = "--";
			this.txtQuery.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtQuery.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.txtQuery.IsReplaceMode = false;
			this.txtQuery.Language = FastColoredTextBoxNS.Language.SQL;
			this.txtQuery.LeftBracket = '(';
			this.txtQuery.Location = new System.Drawing.Point(182, 0);
			this.txtQuery.Margin = new System.Windows.Forms.Padding(4);
			this.txtQuery.Name = "txtQuery";
			this.txtQuery.Paddings = new System.Windows.Forms.Padding(0);
			this.txtQuery.RightBracket = ')';
			this.txtQuery.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.txtQuery.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("txtQuery.ServiceColors")));
			this.txtQuery.ShowLineNumbers = false;
			this.txtQuery.Size = new System.Drawing.Size(214, 96);
			this.txtQuery.TabIndex = 0;
			this.txtQuery.Zoom = 100;
			// 
			// tsQuery
			// 
			tsQuery.Dock = System.Windows.Forms.DockStyle.Left;
			tsQuery.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			tsQuery.ImageScalingSize = new System.Drawing.Size(20, 20);
			tsQuery.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbnQueryGrid,
            this.tsbnQueryText});
			tsQuery.Location = new System.Drawing.Point(0, 0);
			tsQuery.Name = "tsQuery";
			tsQuery.Size = new System.Drawing.Size(40, 99);
			tsQuery.TabIndex = 1;
			tsQuery.Text = "Query";
			// 
			// tsbnQueryGrid
			// 
			this.tsbnQueryGrid.Checked = true;
			this.tsbnQueryGrid.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tsbnQueryGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnQueryGrid.Image = global::Plugin.WmiClient.Properties.Resources.Grid;
			this.tsbnQueryGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnQueryGrid.Name = "tsbnQueryGrid";
			this.tsbnQueryGrid.Size = new System.Drawing.Size(37, 24);
			this.tsbnQueryGrid.Text = "Grid";
			this.tsbnQueryGrid.Click += new System.EventHandler(this.tsbnQuery_Click);
			// 
			// tsbnQueryText
			// 
			this.tsbnQueryText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnQueryText.Image = global::Plugin.WmiClient.Properties.Resources.Notepad;
			this.tsbnQueryText.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnQueryText.Name = "tsbnQueryText";
			this.tsbnQueryText.Size = new System.Drawing.Size(37, 24);
			this.tsbnQueryText.Text = "Text";
			this.tsbnQueryText.ToolTipText = "Text";
			this.tsbnQueryText.Click += new System.EventHandler(this.tsbnQuery_Click);
			// 
			// lvResult
			// 
			this.lvResult.AllowColumnReorder = true;
			this.lvResult.ContextMenuStrip = this.cmsResult;
			this.lvResult.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvResult.FullRowSelect = true;
			this.lvResult.GridLines = true;
			this.lvResult.HideSelection = false;
			this.lvResult.Location = new System.Drawing.Point(0, 0);
			this.lvResult.Margin = new System.Windows.Forms.Padding(4);
			this.lvResult.Name = "lvResult";
			this.lvResult.Plugin = null;
			this.lvResult.Size = new System.Drawing.Size(400, 102);
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
			// bwQuery
			// 
			this.bwQuery.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwQuery_DoWork);
			this.bwQuery.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwQuery_RunWorkerCompleted);
			// 
			// PanelWqlQuery
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(splitMain);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "PanelWqlQuery";
			this.Size = new System.Drawing.Size(400, 260);
			this.Controls.SetChildIndex(this.panelMain, 0);
			this.Controls.SetChildIndex(splitMain, 0);
			splitMain.Panel1.ResumeLayout(false);
			splitMain.Panel1.PerformLayout();
			splitMain.Panel2.ResumeLayout(false);
			splitMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtQuery)).EndInit();
			tsQuery.ResumeLayout(false);
			tsQuery.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.ListViewWmiData lvResult;
		private FastColoredTextBoxNS.FastColoredTextBox txtQuery;
		private System.ComponentModel.BackgroundWorker bwQuery;
		private AlphaOmega.Windows.Forms.ContextMenuStripCopy cmsResult;
		private System.Windows.Forms.ToolStripButton tsbnQueryGrid;
		private System.Windows.Forms.ToolStripButton tsbnQueryText;
		private UI.DataGridViewWmiParameters gvParameters;
	}
}