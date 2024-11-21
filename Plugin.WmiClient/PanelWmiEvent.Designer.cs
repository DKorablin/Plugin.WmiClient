namespace Plugin.WmiClient
{
	partial class PanelWmiEvent
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
			System.Windows.Forms.ToolStrip tsQuery;
			System.Windows.Forms.ToolStrip tsEvents;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelWmiEvent));
			System.Windows.Forms.ToolStripSeparator tsmiSubscriptionsSeparator1;
			this.tsbnQueryGrid = new System.Windows.Forms.ToolStripButton();
			this.tsbnQueryText = new System.Windows.Forms.ToolStripButton();
			this.tsbnEventsSubscriptions = new System.Windows.Forms.ToolStripButton();
			this.tsbnEventsEvents = new System.Windows.Forms.ToolStripButton();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.txtQuery = new FastColoredTextBoxNS.FastColoredTextBox();
			this.gvParameters = new Plugin.WmiClient.UI.DataGridViewWmiParameters();
			this.lvEvents = new Plugin.WmiClient.UI.ListViewWmiResult();
			this.cmsEvents = new AlphaOmega.Windows.Forms.ContextMenuStripCopy();
			this.lvSubscribtions = new System.Windows.Forms.ListView();
			this.colSubscriptionDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colSubscriptionWql = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.cmsSubscriptions = new AlphaOmega.Windows.Forms.ContextMenuStripCopy();
			this.tsmiSubscriptionsStart = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiSubscriptionsStop = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiSubscriptionsDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.ilSubscribtions = new System.Windows.Forms.ImageList(this.components);
			this.error = new System.Windows.Forms.ErrorProvider(this.components);
			tsQuery = new System.Windows.Forms.ToolStrip();
			tsEvents = new System.Windows.Forms.ToolStrip();
			tsmiSubscriptionsSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			tsQuery.SuspendLayout();
			tsEvents.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtQuery)).BeginInit();
			this.cmsSubscriptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.error)).BeginInit();
			this.SuspendLayout();
			// 
			// panelMain
			// 
			this.panelMain.Size = new System.Drawing.Size(400, 206);
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
			tsQuery.Size = new System.Drawing.Size(40, 90);
			tsQuery.TabIndex = 2;
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
			this.tsbnQueryText.Click += new System.EventHandler(this.tsbnQuery_Click);
			// 
			// tsEvents
			// 
			tsEvents.Dock = System.Windows.Forms.DockStyle.Left;
			tsEvents.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			tsEvents.ImageScalingSize = new System.Drawing.Size(20, 20);
			tsEvents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbnEventsSubscriptions,
            this.tsbnEventsEvents});
			tsEvents.Location = new System.Drawing.Point(0, 0);
			tsEvents.Name = "tsEvents";
			tsEvents.Size = new System.Drawing.Size(30, 111);
			tsEvents.TabIndex = 2;
			// 
			// tsbnEventsSubscriptions
			// 
			this.tsbnEventsSubscriptions.Checked = true;
			this.tsbnEventsSubscriptions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tsbnEventsSubscriptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnEventsSubscriptions.Image = ((System.Drawing.Image)(resources.GetObject("tsbnEventsSubscriptions.Image")));
			this.tsbnEventsSubscriptions.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnEventsSubscriptions.Name = "tsbnEventsSubscriptions";
			this.tsbnEventsSubscriptions.Size = new System.Drawing.Size(27, 24);
			this.tsbnEventsSubscriptions.Text = "Subscriptions";
			this.tsbnEventsSubscriptions.Click += new System.EventHandler(this.tsbnEvents_Click);
			// 
			// tsbnEventsEvents
			// 
			this.tsbnEventsEvents.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnEventsEvents.Image = global::Plugin.WmiClient.Properties.Resources.Event_Public;
			this.tsbnEventsEvents.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnEventsEvents.Name = "tsbnEventsEvents";
			this.tsbnEventsEvents.Size = new System.Drawing.Size(27, 24);
			this.tsbnEventsEvents.Text = "Events";
			this.tsbnEventsEvents.Click += new System.EventHandler(this.tsbnEvents_Click);
			// 
			// tsmiSubscriptionsSeparator1
			// 
			tsmiSubscriptionsSeparator1.Name = "tsmiSubscriptionsSeparator1";
			tsmiSubscriptionsSeparator1.Size = new System.Drawing.Size(119, 6);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 28);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.txtQuery);
			this.splitContainer1.Panel1.Controls.Add(this.gvParameters);
			this.splitContainer1.Panel1.Controls.Add(tsQuery);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.lvEvents);
			this.splitContainer1.Panel2.Controls.Add(this.lvSubscribtions);
			this.splitContainer1.Panel2.Controls.Add(tsEvents);
			this.splitContainer1.Size = new System.Drawing.Size(400, 206);
			this.splitContainer1.SplitterDistance = 90;
			this.splitContainer1.SplitterWidth = 5;
			this.splitContainer1.TabIndex = 2;
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
			this.txtQuery.Font = new System.Drawing.Font("Courier New", 9.75F);
			this.txtQuery.IsReplaceMode = false;
			this.txtQuery.Language = FastColoredTextBoxNS.Language.SQL;
			this.txtQuery.LeftBracket = '(';
			this.txtQuery.Location = new System.Drawing.Point(28, 3);
			this.txtQuery.Name = "txtQuery";
			this.txtQuery.Paddings = new System.Windows.Forms.Padding(0);
			this.txtQuery.RightBracket = ')';
			this.txtQuery.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.txtQuery.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("txtQuery.ServiceColors")));
			this.txtQuery.ShowLineNumbers = false;
			this.txtQuery.Size = new System.Drawing.Size(159, 85);
			this.txtQuery.TabIndex = 1;
			this.txtQuery.Visible = false;
			this.txtQuery.Zoom = 100;
			// 
			// gvParameters
			// 
			this.gvParameters.Location = new System.Drawing.Point(194, 3);
			this.gvParameters.Margin = new System.Windows.Forms.Padding(4);
			this.gvParameters.Name = "gvParameters";
			this.gvParameters.Plugin = null;
			this.gvParameters.Size = new System.Drawing.Size(206, 84);
			this.gvParameters.TabIndex = 0;
			// 
			// lvEvents
			// 
			this.lvEvents.AllowColumnReorder = true;
			this.lvEvents.ContextMenuStrip = this.cmsEvents;
			this.lvEvents.FullRowSelect = true;
			this.lvEvents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvEvents.HideSelection = false;
			this.lvEvents.Location = new System.Drawing.Point(29, -2);
			this.lvEvents.Name = "lvEvents";
			this.lvEvents.Plugin = null;
			this.lvEvents.Size = new System.Drawing.Size(158, 111);
			this.lvEvents.TabIndex = 3;
			this.lvEvents.UseCompatibleStateImageBehavior = false;
			this.lvEvents.View = System.Windows.Forms.View.Details;
			// 
			// cmsEvents
			// 
			this.cmsEvents.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmsEvents.Name = "cmsEvents";
			this.cmsEvents.Size = new System.Drawing.Size(113, 28);
			// 
			// lvSubscribtions
			// 
			this.lvSubscribtions.AllowColumnReorder = true;
			this.lvSubscribtions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSubscriptionDate,
            this.colSubscriptionWql});
			this.lvSubscribtions.ContextMenuStrip = this.cmsSubscriptions;
			this.lvSubscribtions.FullRowSelect = true;
			this.lvSubscribtions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvSubscribtions.HideSelection = false;
			this.lvSubscribtions.Location = new System.Drawing.Point(194, 0);
			this.lvSubscribtions.Margin = new System.Windows.Forms.Padding(4);
			this.lvSubscribtions.Name = "lvSubscribtions";
			this.lvSubscribtions.Size = new System.Drawing.Size(206, 109);
			this.lvSubscribtions.SmallImageList = this.ilSubscribtions;
			this.lvSubscribtions.TabIndex = 0;
			this.lvSubscribtions.UseCompatibleStateImageBehavior = false;
			this.lvSubscribtions.View = System.Windows.Forms.View.Details;
			this.lvSubscribtions.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvSubscribtions_KeyDown);
			// 
			// colSubscriptionDate
			// 
			this.colSubscriptionDate.Text = "Date";
			// 
			// colSubscriptionWql
			// 
			this.colSubscriptionWql.Text = "WQL";
			// 
			// cmsSubscriptions
			// 
			this.cmsSubscriptions.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmsSubscriptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSubscriptionsStart,
            this.tsmiSubscriptionsStop,
            tsmiSubscriptionsSeparator1,
            this.tsmiSubscriptionsDelete});
			this.cmsSubscriptions.Name = "cmsSubscriptions";
			this.cmsSubscriptions.Size = new System.Drawing.Size(123, 106);
			this.cmsSubscriptions.Opening += new System.ComponentModel.CancelEventHandler(this.cmsSubscriptions_Opening);
			this.cmsSubscriptions.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsSubscriptions_ItemClicked);
			// 
			// tsmiSubscriptionsStart
			// 
			this.tsmiSubscriptionsStart.Name = "tsmiSubscriptionsStart";
			this.tsmiSubscriptionsStart.Size = new System.Drawing.Size(122, 24);
			this.tsmiSubscriptionsStart.Text = "Start";
			// 
			// tsmiSubscriptionsStop
			// 
			this.tsmiSubscriptionsStop.Name = "tsmiSubscriptionsStop";
			this.tsmiSubscriptionsStop.Size = new System.Drawing.Size(122, 24);
			this.tsmiSubscriptionsStop.Text = "Stop";
			// 
			// tsmiSubscriptionsDelete
			// 
			this.tsmiSubscriptionsDelete.Name = "tsmiSubscriptionsDelete";
			this.tsmiSubscriptionsDelete.Size = new System.Drawing.Size(122, 24);
			this.tsmiSubscriptionsDelete.Text = "Delete";
			// 
			// ilSubscribtions
			// 
			this.ilSubscribtions.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilSubscribtions.ImageStream")));
			this.ilSubscribtions.TransparentColor = System.Drawing.Color.Magenta;
			this.ilSubscribtions.Images.SetKeyName(0, "Event.Public.bmp");
			this.ilSubscribtions.Images.SetKeyName(1, "iconPause.bmp");
			// 
			// error
			// 
			this.error.ContainerControl = this;
			// 
			// PanelWmiEvent
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "PanelWmiEvent";
			this.Size = new System.Drawing.Size(400, 260);
			this.Controls.SetChildIndex(this.panelMain, 0);
			this.Controls.SetChildIndex(this.splitContainer1, 0);
			tsQuery.ResumeLayout(false);
			tsQuery.PerformLayout();
			tsEvents.ResumeLayout(false);
			tsEvents.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtQuery)).EndInit();
			this.cmsSubscriptions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.error)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListView lvSubscribtions;
		private Plugin.WmiClient.UI.DataGridViewWmiParameters gvParameters;
		private FastColoredTextBoxNS.FastColoredTextBox txtQuery;
		private System.Windows.Forms.ColumnHeader colSubscriptionDate;
		private System.Windows.Forms.ImageList ilSubscribtions;
		private System.Windows.Forms.ColumnHeader colSubscriptionWql;
		private System.Windows.Forms.ErrorProvider error;
		private System.Windows.Forms.ToolStripButton tsbnQueryGrid;
		private System.Windows.Forms.ToolStripButton tsbnQueryText;
		private System.Windows.Forms.ToolStripButton tsbnEventsSubscriptions;
		private System.Windows.Forms.ToolStripButton tsbnEventsEvents;
		private UI.ListViewWmiResult lvEvents;
		private AlphaOmega.Windows.Forms.ContextMenuStripCopy cmsSubscriptions;
		private System.Windows.Forms.ToolStripMenuItem tsmiSubscriptionsDelete;
		private AlphaOmega.Windows.Forms.ContextMenuStripCopy cmsEvents;
		private System.Windows.Forms.ToolStripMenuItem tsmiSubscriptionsStart;
		private System.Windows.Forms.ToolStripMenuItem tsmiSubscriptionsStop;
	}
}
