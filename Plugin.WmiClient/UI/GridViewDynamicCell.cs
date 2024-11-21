using System;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;

namespace Plugin.WmiClient.UI
{
	internal class GridViewDynamicCell
	{
		public enum ControlType
		{
			ComboBox = 1,
			TextBox = 2,
			TextBoxMultiline = 3,
			MonthCalendar = 4,
			Percent = 5,
			ListView = 6,
		}

		private ToolStripDropDown tsdd;
		private Boolean _dropDownClosed;
		private Object _formattedValue;
		private ToolStripControlHost tbh;
		private DataGridView _gridView;

		public GridViewDynamicCell(DataGridView gridView)
		{
			this._gridView = gridView;
			tsdd = new ToolStripDropDown
			{
				Padding = new Padding(0),
				Margin = new Padding(0),
				AutoSize = true
			};
			tsdd.Opened += new EventHandler(tsdd_Opened);
			tsdd.Closing += new ToolStripDropDownClosingEventHandler(tsdd_Closing);
		}

		/// <summary>Получить значение свойства из объекта</summary>
		/// <param name="item">Объект из которого необходимо получить свойство</param>
		/// <param name="propertyName">Наименование свойства значение которого необходимо получить из объекта. Если свойство не задано, то возвращается исходный объект</param>
		/// <returns>Значение свойства</returns>
		private static Object GetPropertyValue(Object item, String propertyName)
			=> propertyName == null
				? item
				: item.GetType().InvokeMember(propertyName, BindingFlags.GetProperty | BindingFlags.ExactBinding, null, item, null);

		public void CreateControl(ControlType ctrl, Object[] values, String displayMember, String valueMember)
		{
			Point cell = this._gridView.CurrentCellAddress;
			Object selectedValue = this._gridView[cell.X, cell.Y].Value;

			switch(ctrl)
			{
			case ControlType.ComboBox:
				ComboBox cb = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = displayMember, ValueMember = valueMember, };
				cb.DropDownClosed += new EventHandler(cb_DropDownClosed);
				cb.Items.AddRange(values);

				if(valueMember != null)
					for(Int32 loop = 0; loop < values.Length; loop++)
					{
						Object item = GridViewDynamicCell.GetPropertyValue(values[loop], valueMember);
						if((item == null && selectedValue == null) || item.Equals(selectedValue))
						{
							cb.SelectedIndex = loop;
							break;
						}
					}

				this.ShowControl(cb);
				break;
			case ControlType.MonthCalendar:
				MonthCalendar cal = new MonthCalendar();
				cal.DateSelected += new DateRangeEventHandler(cal_DateSelected);
				if(selectedValue is DateTime)
					cal.SetDate((DateTime)selectedValue);

				this.ShowControl(cal, cal.Size);
				break;
			case ControlType.TextBoxMultiline:
				TextBox txt = new TextBox() { Multiline = true, Text = selectedValue == null ? String.Empty : selectedValue.ToString(), AcceptsReturn = true, Width = 200, Height = 200, };

				this.ShowControl(txt, new Size(200, 200));
				break;
			case ControlType.Percent:
				NumericUpDown ud = new NumericUpDown() { Minimum = 0, Maximum = 100, };
				Rectangle rect = this._gridView.GetCellDisplayRectangle(cell.X, cell.Y, true);

				this.ShowControl(ud, rect.Size);
				break;
			case ControlType.ListView:
				ListView lv = new ListView() { MultiSelect = false, FullRowSelect = true, GridLines = false, LabelEdit = false, HeaderStyle = ColumnHeaderStyle.None, View = View.Details, };
				lv.Columns.Add(String.Empty);
				lv.MouseDoubleClick += new MouseEventHandler(lv_MouseDoubleClick);

				foreach(Object item in values)
				{
					String displayText = String.Empty;
					if(displayMember.Contains(";"))
					{
						String[] dispProperties = displayMember.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
						foreach(String dispProperty in dispProperties)
							displayText += " " + GridViewDynamicCell.GetPropertyValue(item, dispProperty).ToString();
					} else
						displayText = GridViewDynamicCell.GetPropertyValue(item, displayMember).ToString();

					Object value = GridViewDynamicCell.GetPropertyValue(item, valueMember);
					ListViewItem lvItem = lv.Items.Add(new ListViewItem(displayText) { Tag = value, });

					if(selectedValue != null && selectedValue.Equals(value))
					{
						lv.SelectedIndices.Add(lvItem.Index);
						lv.FocusedItem = lvItem;
					}
				}

				lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				Int32 width = 0;
				Int32 height = lv.Items.Count > 0 ? lv.Items[0].Bounds.Height * lv.Items.Count + 5 : 0;
				foreach(ListViewItem item in lv.Items)
					if(item.Bounds.Width > width)
						width = item.Bounds.Width + 10;

				this.ShowControl(lv, new Size(width, height));
				break;
			default:
				throw new NotImplementedException(String.Format("ControlType: {0}; Values: {1}; DisplayMember: {2}; ValueMember: {3};", ctrl, values, displayMember, valueMember));
			}
		}

		private void CloseControl()
			=> this.tsdd.Close(ToolStripDropDownCloseReason.CloseCalled);

		public void ShowControl(Control ctrl, Size? size = null)
		{
			Point cell = this._gridView.CurrentCellAddress;

			tbh = new ToolStripControlHost(ctrl)
			{
				Padding = Padding.Empty
			};

			Rectangle rect = this._gridView.GetCellDisplayRectangle(cell.X, cell.Y, true);
			if(size == null)
			{
				ctrl.Size = rect.Size;
				tbh.Size = rect.Size;
				tbh.AutoSize = true;
			} else
			{
				tbh.Margin = Padding.Empty;
				tbh.Size = size.Value;
				tbh.AutoSize = false;
			}

			this._dropDownClosed = false;
			this._formattedValue = this._gridView[cell.X, cell.Y].Value;

			tsdd.Items.Clear();
			tsdd.Items.Add(tbh);
			tsdd.Show(this._gridView, rect.Location);

			while(!this._dropDownClosed)
				Application.DoEvents();

			this._gridView.BeginInvoke((MethodInvoker)delegate()
			{
				this._gridView.EndEdit();
				if(this._formattedValue != null)
					this._gridView[cell.X, cell.Y].Value = this._formattedValue;
			});
		}

		private void lv_MouseDoubleClick(Object sender, MouseEventArgs e)
		{
			ListView lv = (ListView)sender;
			if(lv.SelectedItems.Count == 1)
				this._formattedValue = lv.SelectedItems[0].Tag;
			this.CloseControl();
		}

		private void cal_DateSelected(Object sender, DateRangeEventArgs e)
		{
			this._formattedValue = ((MonthCalendar)sender).SelectionStart;
			this.CloseControl();
		}

		private void cb_DropDownClosed(Object sender, EventArgs e)
			=> this.CloseControl();

		private void tsdd_Opened(Object sender, EventArgs e)
		{
			tbh.Focus();
			ComboBox cb = tbh.Control as ComboBox;
			if(cb != null)
				cb.DroppedDown = true;
		}

		private void tsdd_Closing(Object sender, ToolStripDropDownClosingEventArgs e)
		{
			if(tbh.Control is TextBox)
				this._formattedValue = tsdd.Items[0].Text;
			else if(tbh.Control is NumericUpDown)
				this._formattedValue = (Byte)((NumericUpDown)tbh.Control).Value;
			else if(tbh.Control is ComboBox)
			{
				ComboBox cb = (ComboBox)tbh.Control;
				Object item = cb.SelectedItem;
				if(item != null)
					this._formattedValue = cb.ValueMember.Length == 0
						? item
						: cb.SelectedItem.GetType().InvokeMember(cb.ValueMember, BindingFlags.GetProperty | BindingFlags.ExactBinding, null, item, null);
			}
			this._dropDownClosed = true;
		}
	}
}