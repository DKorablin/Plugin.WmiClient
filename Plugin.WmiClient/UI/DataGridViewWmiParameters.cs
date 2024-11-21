using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using Plugin.WmiClient.Dal;
using Plugin.WmiClient.Dto;

namespace Plugin.WmiClient.UI
{
	internal class DataGridViewWmiParameters : UserControl
	{
		private readonly GridViewDynamicCell _cellPropertiesValue;
		private WmiPathItem _path;
		private System.Windows.Forms.DataGridViewTextBoxColumn colPropertiesType;
		private System.Windows.Forms.DataGridViewTextBoxColumn colPropertiesOrigin;
		private System.Windows.Forms.DataGridViewTextBoxColumn colPropertiesName;
		private System.Windows.Forms.DataGridViewComboBoxColumn colPropertiesSign;
		private System.Windows.Forms.DataGridViewTextBoxColumn colPropertiesValue;
		private System.ComponentModel.IContainer components;
		private DataGridView gvData;
		private System.Windows.Forms.BindingSource bsParameters;
		
		[Browsable(false)]
		public PluginWindows Plugin { get; set; }

		public Int32 Count => gvData.Rows.Count;

		public DataGridViewWmiParameters()
		{
			this.InitializeComponent();
			colPropertiesSign.DisplayMember = "Key";
			colPropertiesSign.ValueMember = "Value";
			colPropertiesSign.DataSource = WmiDataRow.Signs;
			this._cellPropertiesValue = new GridViewDynamicCell(this.gvData);
		}

		public WmiDataRow GetRowItem(Int32 index)
			=> (WmiDataRow)gvData.Rows[index].DataBoundItem;

		public void Reset(Int32 index)
			=> bsParameters.ResetItem(index);

		public void Clear()
		{
			this._path = null;
			bsParameters.DataSource = null;
		}

		public void DataBind(WmiPathItem path, IEnumerable<WmiDataRow> rows)
		{
			this._path = path;
			bsParameters.DataSource = rows.ToList();
			gvData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
		}

		public String[] GetWqlConditions()
		{
			if(!this.gvData.EndEdit())
				return null;

			WmiDataRow[] eventRows = new WmiDataRow[this.gvData.Rows.Count];
			for(Int32 loop = 0; loop < eventRows.Length; loop++)
				eventRows[loop] = (WmiDataRow)this.gvData.Rows[loop].DataBoundItem;

			List<String> conditions = new List<String>(eventRows.Length);
			foreach(WmiDataRow row in eventRows)
			{
				String condition = row.GetFormattedCondition();
				if(condition != null)
					conditions.Add(condition);
			}
			return conditions.ToArray();
		}

		/// <summary>Ошибка ввода данных</summary>
		/// <param name="sender">Таблица</param>
		/// <param name="e">Аргументы ошибки ввода данных</param>
		private void gvParameters_DataError(Object sender, DataGridViewDataErrorEventArgs e)
		{
			Exception exc = e.Exception;
			while(exc.InnerException != null)
				exc = exc.InnerException;
			this.gvData.Rows[e.RowIndex].ErrorText = exc.Message;
		}

		private void gvParameters_CellBeginEdit(Object sender, DataGridViewCellCancelEventArgs e)
		{
			if(e.RowIndex == -1)
				return;

			this.gvData.Rows[e.RowIndex].ErrorText = String.Empty;

			if(e.ColumnIndex == colPropertiesValue.Index)
			{
				e.Cancel = true;

				WmiDataRow row = (WmiDataRow)this.gvData.Rows[e.RowIndex].DataBoundItem;
				switch(row.Type)
				{
				case CimType.Boolean:
					Object[] values1 = new Object[] { true, false, };
					this._cellPropertiesValue.CreateControl(GridViewDynamicCell.ControlType.ComboBox, values1, null, null);
					break;
				case CimType.DateTime:
					this._cellPropertiesValue.CreateControl(GridViewDynamicCell.ControlType.MonthCalendar, null, null, null);
					break;
				case CimType.Object:
					base.Cursor = Cursors.WaitCursor;
					String name = (String)this.gvData.Rows[e.RowIndex].Cells[colPropertiesName.Index].Value;
					String[] data = null;

					try
					{
						switch(name)
						{
						case "TargetClass":
						case "PreviousClass":
							using(WmiData wmi = this.Plugin.CreateWmiData())
								data = wmi.GetClasses(this._path.NamespaceName, WmiData.WmiFilterType.Classes).ToArray();
							break;
						case "TargetNamespace":
							using(WmiData wmi = this.Plugin.CreateWmiData())
								data = wmi.GetNamespacesRecursive().ToArray();
							break;
						case "TargetInstance":
							using(WmiData wmi = this.Plugin.CreateWmiData())
								data = wmi.GetClasses(this._path.NamespaceName, WmiData.WmiFilterType.ClassesWithMethods).ToArray();
							break;
						}
					} finally
					{
						base.Cursor = Cursors.Default;
					}

					if(data == null)
						e.Cancel = false;
					else
						this._cellPropertiesValue.CreateControl(GridViewDynamicCell.ControlType.ComboBox, data, null, null);
					break;
				default://Показываем элемент управления по умолчанию
					e.Cancel = false;
					break;
				}
			}
		}

		private void gvParameters_CellValueChanged(Object sender, DataGridViewCellEventArgs e)
		{
			if(e.RowIndex == -1)
				return;

			if(e.ColumnIndex == colPropertiesValue.Index)
			{
				WmiDataRow row = (WmiDataRow)this.gvData.Rows[e.RowIndex].DataBoundItem;

				if(row.Type == CimType.Object)
				{
					switch(row.Name)
					{
					case "TargetClass":
					case "PreviousClass":
					case "TargetNamespace":
					case "TargetInstance":
						List<WmiDataRow> originalRows = (List<WmiDataRow>)bsParameters.DataSource;
						String parentClass = (String)row.Value;
						for(Int32 loop = originalRows.Count - 1; loop >= 0; loop--)
						{
							if(originalRows[loop].Name.StartsWith(row.Name + "."))
								originalRows.RemoveAt(loop);
						}

						using(WmiDataClass wmi = this.Plugin.CreateWmiDataClass(this._path, parentClass))
						{
							IEnumerable<WmiDataRow> newRows = wmi.GetProperties().Select(p => new WmiDataRow(p, row.Name));
							originalRows.InsertRange(e.RowIndex + 1, newRows);

							//bsParameters.DataSource = originalRows;
							bsParameters.ResetBindings(true);
						}
						break;
					}
				}
			}
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this.gvData = new System.Windows.Forms.DataGridView();
			this.colPropertiesType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPropertiesOrigin = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPropertiesName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPropertiesSign = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.colPropertiesValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.bsParameters = new System.Windows.Forms.BindingSource(this.components);
			((System.ComponentModel.ISupportInitialize)(this.gvData)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bsParameters)).BeginInit();
			this.SuspendLayout();
			// 
			// _grid
			// 
			this.gvData.AllowUserToAddRows = false;
			this.gvData.AllowUserToDeleteRows = false;
			this.gvData.AllowUserToResizeRows = false;
			this.gvData.AutoGenerateColumns = false;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.gvData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.gvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPropertiesOrigin,
            this.colPropertiesType,
            this.colPropertiesName,
            this.colPropertiesSign,
            this.colPropertiesValue});
			this.gvData.DataSource = this.bsParameters;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.gvData.DefaultCellStyle = dataGridViewCellStyle2;
			this.gvData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gvData.Location = new System.Drawing.Point(0, 0);
			this.gvData.MultiSelect = false;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.gvData.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.gvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.gvData.Size = new System.Drawing.Size(240, 150);
			this.gvData.TabIndex = 0;
			this.gvData.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.gvParameters_CellBeginEdit);
			this.gvData.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvParameters_CellValueChanged);
			this.gvData.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.gvParameters_DataError);
			// 
			// colPropertiesType
			// 
			this.colPropertiesType.DataPropertyName = "TypeName";
			this.colPropertiesType.HeaderText = "Type";
			this.colPropertiesType.Name = "colPropertiesType";
			this.colPropertiesType.ReadOnly = true;
			this.colPropertiesType.Width = 50;
			// 
			// colPropertiesOrigin
			// 
			this.colPropertiesOrigin.DataPropertyName = "Origin";
			this.colPropertiesOrigin.HeaderText = "Origin";
			this.colPropertiesOrigin.Name = "colPropertiesOrigin";
			this.colPropertiesOrigin.ReadOnly = true;
			// 
			// colPropertiesName
			// 
			this.colPropertiesName.DataPropertyName = "Name";
			this.colPropertiesName.HeaderText = "Name";
			this.colPropertiesName.Name = "colPropertiesName";
			this.colPropertiesName.ReadOnly = true;
			// 
			// colPropertiesSign
			// 
			this.colPropertiesSign.DataPropertyName = "Sign";
			this.colPropertiesSign.HeaderText = "Sign";
			this.colPropertiesSign.Name = "colPropertiesSign";
			this.colPropertiesSign.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colPropertiesSign.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.colPropertiesSign.Width = 50;
			// 
			// colPropertiesValue
			// 
			this.colPropertiesValue.DataPropertyName = "Value";
			this.colPropertiesValue.HeaderText = "Value";
			this.colPropertiesValue.Name = "colPropertiesValue";
			// 
			// bsParameters
			// 
			this.bsParameters.AllowNew = false;
			this.bsParameters.DataSource = typeof(Plugin.WmiClient.Dal.WmiDataRow);
			// 
			// DataGridViewWmiParameters
			// 
			this.Controls.Add(this.gvData);
			this.Location = new System.Drawing.Point(194, 0);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "DataGridViewWmiParameters";
			this.Size = new System.Drawing.Size(206, 87);
			((System.ComponentModel.ISupportInitialize)(this.gvData)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bsParameters)).EndInit();
			this.ResumeLayout(false);

		}
	}
}