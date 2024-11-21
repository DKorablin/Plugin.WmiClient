namespace Plugin.WmiClient
{
	partial class PanelWmiDescription
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
			System.Windows.Forms.ImageList ilDefinitions;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelWmiDescription));
			this.splitMain = new System.Windows.Forms.SplitContainer();
			this.tvDefinitions = new System.Windows.Forms.TreeView();
			this.lvDefinitions = new System.Windows.Forms.ListView();
			this.colDefName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.cmsDefinitions = new AlphaOmega.Windows.Forms.ContextMenuStripCopy();
			this.txtDescription = new System.Windows.Forms.TextBox();
			this.timerLVIndexHack = new System.Windows.Forms.Timer(this.components);
			ilDefinitions = new System.Windows.Forms.ImageList(this.components);
			this.splitMain.Panel1.SuspendLayout();
			this.splitMain.Panel2.SuspendLayout();
			this.splitMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelMain
			// 
			this.panelMain.Size = new System.Drawing.Size(387, 160);
			// 
			// ilDefinitions
			// 
			ilDefinitions.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilDefinitions.ImageStream")));
			ilDefinitions.TransparentColor = System.Drawing.Color.Magenta;
			ilDefinitions.Images.SetKeyName(0, "Hourglass.png");
			ilDefinitions.Images.SetKeyName(1, "Namespace.bmp");
			ilDefinitions.Images.SetKeyName(2, "Class.Public.bmp");
			ilDefinitions.Images.SetKeyName(3, "Method.Public.bmp");
			ilDefinitions.Images.SetKeyName(4, "Property.Public.bmp");
			ilDefinitions.Images.SetKeyName(5, "Field.Public.bmp");
			// 
			// splitMain
			// 
			this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitMain.Location = new System.Drawing.Point(0, 28);
			this.splitMain.Margin = new System.Windows.Forms.Padding(4);
			this.splitMain.Name = "splitMain";
			this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitMain.Panel1
			// 
			this.splitMain.Panel1.Controls.Add(this.tvDefinitions);
			this.splitMain.Panel1.Controls.Add(this.lvDefinitions);
			// 
			// splitMain.Panel2
			// 
			this.splitMain.Panel2.Controls.Add(this.txtDescription);
			this.splitMain.Size = new System.Drawing.Size(387, 132);
			this.splitMain.SplitterDistance = 65;
			this.splitMain.SplitterWidth = 5;
			this.splitMain.TabIndex = 2;
			// 
			// tvDefinitions
			// 
			this.tvDefinitions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tvDefinitions.ImageIndex = 0;
			this.tvDefinitions.ImageList = ilDefinitions;
			this.tvDefinitions.Location = new System.Drawing.Point(4, 4);
			this.tvDefinitions.Name = "tvDefinitions";
			this.tvDefinitions.SelectedImageIndex = 0;
			this.tvDefinitions.Size = new System.Drawing.Size(194, 58);
			this.tvDefinitions.TabIndex = 1;
			this.tvDefinitions.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tvDefinitions_AfterExpand);
			this.tvDefinitions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvDefinitions_AfterSelect);
			// 
			// lvDefinitions
			// 
			this.lvDefinitions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lvDefinitions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDefName});
			this.lvDefinitions.ContextMenuStrip = this.cmsDefinitions;
			this.lvDefinitions.FullRowSelect = true;
			this.lvDefinitions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvDefinitions.HideSelection = false;
			this.lvDefinitions.Location = new System.Drawing.Point(205, 0);
			this.lvDefinitions.Margin = new System.Windows.Forms.Padding(4);
			this.lvDefinitions.MultiSelect = false;
			this.lvDefinitions.Name = "lvDefinitions";
			this.lvDefinitions.Size = new System.Drawing.Size(182, 65);
			this.lvDefinitions.TabIndex = 0;
			this.lvDefinitions.UseCompatibleStateImageBehavior = false;
			this.lvDefinitions.View = System.Windows.Forms.View.Details;
			this.lvDefinitions.SelectedIndexChanged += new System.EventHandler(this.lvDefinitions_SelectedIndexChanged);
			// 
			// colDefName
			// 
			this.colDefName.Text = "Name";
			// 
			// cmsDefinitions
			// 
			this.cmsDefinitions.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmsDefinitions.Name = "cmsDefinitions";
			this.cmsDefinitions.Size = new System.Drawing.Size(113, 28);
			// 
			// txtDescription
			// 
			this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtDescription.Location = new System.Drawing.Point(0, 0);
			this.txtDescription.Margin = new System.Windows.Forms.Padding(4);
			this.txtDescription.Multiline = true;
			this.txtDescription.Name = "txtDescription";
			this.txtDescription.ReadOnly = true;
			this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtDescription.Size = new System.Drawing.Size(387, 62);
			this.txtDescription.TabIndex = 0;
			// 
			// timerLVIndexHack
			// 
			this.timerLVIndexHack.Interval = 10;
			this.timerLVIndexHack.Tick += new System.EventHandler(this.timerLVIndexHack_Tick);
			// 
			// PanelWmiDescription
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitMain);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "PanelWmiDescription";
			this.Size = new System.Drawing.Size(387, 185);
			this.Controls.SetChildIndex(this.panelMain, 0);
			this.Controls.SetChildIndex(this.splitMain, 0);
			this.splitMain.Panel1.ResumeLayout(false);
			this.splitMain.Panel2.ResumeLayout(false);
			this.splitMain.Panel2.PerformLayout();
			this.splitMain.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitMain;
		private System.Windows.Forms.ListView lvDefinitions;
		private System.Windows.Forms.TextBox txtDescription;
		private System.Windows.Forms.ColumnHeader colDefName;
		private System.Windows.Forms.Timer timerLVIndexHack;
		private AlphaOmega.Windows.Forms.ContextMenuStripCopy cmsDefinitions;
		private System.Windows.Forms.TreeView tvDefinitions;

	}
}
