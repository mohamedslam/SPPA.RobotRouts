#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft     							}
{	ALL RIGHTS RESERVED												}
{																	}
{	The entire contents of this file is protected by U.S. and		}
{	International Copyright Laws. Unauthorized reproduction,		}
{	reverse-engineering, and distribution of all or any portion of	}
{	the code contained in this file is strictly prohibited and may	}
{	result in severe civil and criminal penalties and will be		}
{	prosecuted to the maximum extent possible under the law.		}
{																	}
{	RESTRICTIONS													}
{																	}
{	THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES			}
{	ARE CONFIDENTIAL AND PROPRIETARY								}
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Reflection;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Base;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Controls;
using System.Runtime.InteropServices;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dialogs
{
    /// <summary>
    /// This class provide forms rendering.
    /// </summary>
    public class StiWinDialogsProvider : StiDialogsProvider
    {
        #region Methods.DllImport
        [DllImport("User32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        #endregion

        #region Fields
        private Hashtable controlToReport;
        private Hashtable reportToControl;
        private Hashtable managerToGridControl;
        private StiForm formControl;
        private ToolTip toolTip;
        private bool lockReportControlUpdate;
        #endregion

        #region Properties
        protected override StiGuiMode GuiMode => StiGuiMode.Gdi;

        public Form Form { get; private set; }
        
        public override StiReport Report { get; set; }
        #endregion

        #region Handlers
        private void Manager_PositionChanged(object sender, EventArgs e)
        {
            var control = managerToGridControl[sender] as StiGridControl;
            if (control != null)
                control.InvokePositionChanged(e);

            InvokeEventFired(sender, e);
        }

        private void Control_Click(object sender, EventArgs e)
        {
            if (controlToReport == null) return;

            var control = controlToReport[sender] as StiReportControl;
            var formControl = controlToReport[sender] as StiForm;
            if (control != null)
            {
                #region Set Focus to Button
                Control selectedControl = null;
                var winControl = sender as Control;
                var form = winControl.FindForm();
                if (form != null)
                {
                    selectedControl = form.ActiveControl;
                    form.ActiveControl = winControl;
                }
                #endregion

                control.InvokeClick(control, e);
                if (sender is Button)
                    InvokeButtonClick(sender, e);

                //Set Focus to Original Control
                if (form != null && form.Visible)
                    form.ActiveControl = selectedControl;
            }

            formControl?.InvokeClick(control, e);
        }

        private void Control_DoubleClick(object sender, EventArgs e)
        {
            if (controlToReport == null) return;

            var control = controlToReport[sender] as StiReportControl;
            control.InvokeDoubleClick(control, e);

            var form = controlToReport[sender] as StiForm;
            form?.InvokeDoubleClick(control, e);
        }

        private void Control_Enter(object sender, EventArgs e)
        {
            if (controlToReport == null) return;

            var control = controlToReport[sender] as StiReportControl;
            control?.InvokeEnter(e);
        }

        private void Control_Leave(object sender, EventArgs e)
        {
            lockReportControlUpdate = true;
            FormToReportControl(formControl);
            lockReportControlUpdate = false;

            if (controlToReport == null) return;

            var control = controlToReport[sender] as StiReportControl;
            control?.InvokeLeave(e);
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (controlToReport == null) return;

            var control = controlToReport[sender] as StiReportControl;
            control?.InvokeMouseDown(e);
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            if (controlToReport == null) return;

            var control = controlToReport[sender] as StiReportControl;
            control?.InvokeMouseUp(e);
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (controlToReport == null) return;

            var control = controlToReport[sender] as StiReportControl;
            control?.InvokeMouseMove(e);
        }

        private void Form_Closed(object sender, EventArgs e)
        {
            if (controlToReport == null) return;

            var formControl = controlToReport[sender] as StiForm;
            formControl?.InvokeClosedForm(e);
        }

        private void Form_Closing(object sender, CancelEventArgs e)
        {
            if (controlToReport == null) return;

            var formControl = controlToReport[sender] as StiForm;
            formControl?.InvokeClosingForm(e);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            if (controlToReport == null) return;

            var formControl = controlToReport[sender] as StiForm;
            formControl?.InvokeLoadForm(e);
        }

        private void Control_CheckedChanged(object sender, EventArgs e)
        {
            if (controlToReport == null) return;

            lockReportControlUpdate = true;
            FormToReportControl(formControl);
            lockReportControlUpdate = false;

            var control = controlToReport[sender] as StiCheckBoxControl;
            control?.InvokeCheckedChanged(e);

            var radioButton = controlToReport[sender] as StiRadioButtonControl;
            radioButton?.InvokeCheckedChanged(e);

            InvokeEventFired(sender, e);
        }

        private void Control_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (controlToReport == null) return;

            lockReportControlUpdate = true;
            FormToReportControl(formControl);
            lockReportControlUpdate = false;

            var control = controlToReport[sender] as StiListBoxControl;
            control?.InvokeSelectedIndexChanged(e);

            var checkedListBox = controlToReport[sender] as StiCheckedListBoxControl;
            checkedListBox?.InvokeSelectedIndexChanged(e);

            var listViewControl = controlToReport[sender] as StiListViewControl;
            listViewControl?.InvokeSelectedIndexChanged(e);

            var comboBox = controlToReport[sender] as StiComboBoxControl;
            comboBox?.InvokeSelectedIndexChanged(e);

            InvokeEventFired(sender, e);
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (controlToReport == null) return;

            lockReportControlUpdate = true;
            FormToReportControl(formControl);
            lockReportControlUpdate = false;

            var control = controlToReport[sender] as StiTreeViewControl;
            control?.InvokeAfterSelect(e);

            InvokeEventFired(sender, e);
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            if (controlToReport == null) return;

            lockReportControlUpdate = true;
            FormToReportControl(formControl);
            lockReportControlUpdate = false;

            var control = controlToReport[sender] as StiDateTimePickerControl;
            control?.InvokeValueChanged(e);

            var numeric = controlToReport[sender] as StiNumericUpDownControl;
            numeric?.InvokeValueChanged(e);

            InvokeEventFired(sender, e);
        }
        #endregion

        #region Methods.ConvertReportControlToControl
        protected virtual void ConvertReportControlToControl(Control control, StiReportControl reportControl)
        {
            if (reportControl.TagValueBinding.Length > 0)
            {
                reportControl.TagValue = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, reportControl.TagValueBinding);
            }

            control.Dock = StiComponent.ConvertDockStyle(reportControl.DockStyle);
            control.Location = StiScale.I(reportControl.Location);
            control.Size = StiScale.I(reportControl.Size);

            if (control.BackColor != reportControl.BackColor)
                control.BackColor = reportControl.BackColor;

            control.Font = reportControl.Font;
            control.RightToLeft = reportControl.RightToLeft;
            control.Tag = reportControl.TagValue;

            if (control.ForeColor != reportControl.ForeColor)
                control.ForeColor = reportControl.ForeColor;

            control.Enabled = reportControl.Enabled;
            control.Visible = reportControl.Visible;

            reportControl.InvokeEvents();
            toolTip.SetToolTip(control, reportControl.ToolTipValue as string);

            control.Click += Control_Click;
            control.DoubleClick += Control_DoubleClick;
            control.Enter += Control_Enter;
            control.Leave += Control_Leave;
            control.MouseDown += Control_MouseDown;
            control.MouseUp += Control_MouseUp;
            control.MouseMove += Control_MouseMove;
        }

        protected virtual Control ConvertReportControlToPanel(StiPanelControl panelControl)
        {
            var panel = new Controls.StiDialogPanel();

            panelControl.Control = panel;
            ConvertReportControlToControl(panel, panelControl);
            panel.BorderStyle = panelControl.BorderStyle;

            return panel;
        }

        protected virtual Control ConvertReportControlToTextBox(StiTextBoxControl textBoxControl)
        {
            var textBox = new TextBox();
            textBoxControl.Control = textBox;
            ConvertReportControlToControl(textBox, textBoxControl);

            #region DataBinding
            if (textBoxControl.TextBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(Report.Dictionary, textBoxControl.TextBinding);
                if (value != null)
                    textBoxControl.Text = value.ToString();
            }
            #endregion

            textBox.MaxLength = textBoxControl.MaxLength;
            textBox.PasswordChar = textBoxControl.PasswordChar;
            textBox.Text = textBoxControl.Text;
            textBox.Multiline = textBoxControl.Multiline;
            textBox.WordWrap = textBoxControl.WordWrap;
            textBox.AcceptsReturn = textBoxControl.AcceptsReturn;
            textBox.AcceptsTab = textBoxControl.AcceptsTab;

            return textBox;
        }

        protected virtual Control ConvertReportControlToRichTextBox(StiRichTextBoxControl textBoxControl)
        {
            var textBox = new StiRichTextBox(textBoxControl.BackColor == Color.Transparent);
            textBoxControl.Control = textBox;
            ConvertReportControlToControl(textBox, textBoxControl);

            textBox.Rtf = textBoxControl.RtfText;

            return textBox;
        }

        protected virtual Control ConvertReportControlToButton(StiButtonControl buttonControl)
        {
            var button = new Button
            {
                TextAlign = buttonControl.TextAlign,
                ImageAlign = buttonControl.ImageAlign,
                Image = buttonControl.Image
            };

            buttonControl.Control = button;
            ConvertReportControlToControl(button, buttonControl);
            button.DialogResult = buttonControl.DialogResult;
            button.Text = buttonControl.Text;

            if (buttonControl.Default)
                Form.AcceptButton = button;

            if (buttonControl.Cancel)
                Form.CancelButton = button;

            return button;
        }

        protected virtual Control ConvertReportControlToCheckBox(StiCheckBoxControl checkBoxControl)
        {
            var checkBox = new CheckBox();
            checkBoxControl.Control = checkBox;
            ConvertReportControlToControl(checkBox, checkBoxControl);

            #region DataBinding
            if (checkBoxControl.CheckedBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(Report.Dictionary, checkBoxControl.CheckedBinding);
                if (value is bool)
                    checkBoxControl.Checked = (bool)value;
            }

            if (checkBoxControl.TextBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(Report.Dictionary, checkBoxControl.TextBinding);
                if (value != null)
                    checkBoxControl.Text = value.ToString();
            }
            #endregion

            checkBox.Checked = checkBoxControl.Checked;
            checkBox.Text = checkBoxControl.Text;

            checkBox.CheckedChanged += Control_CheckedChanged;

            return checkBox;
        }

        protected virtual Control ConvertReportControlToRadioButton(StiRadioButtonControl radioButtonControl)
        {
            var radioButton = new RadioButton();
            radioButtonControl.Control = radioButton;
            ConvertReportControlToControl(radioButton, radioButtonControl);

            #region DataBinding
            if (radioButtonControl.CheckedBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(Report.Dictionary, radioButtonControl.CheckedBinding);
                if (value is bool)
                    radioButtonControl.Checked = (bool)value;
            }

            if (radioButtonControl.TextBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(Report.Dictionary, radioButtonControl.TextBinding);
                if (value != null)
                    radioButtonControl.Text = value.ToString();
            }
            #endregion

            radioButton.Checked = radioButtonControl.Checked;
            radioButton.Text = radioButtonControl.Text;

            radioButton.CheckedChanged += Control_CheckedChanged;

            return radioButton;
        }

        protected virtual Control ConvertReportControlToLabel(StiLabelControl labelControl)
        {
            var label = new Label();
            labelControl.Control = label;
            ConvertReportControlToControl(label, labelControl);

            #region DataBinding
            if (labelControl.TextBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(Report.Dictionary, labelControl.TextBinding);
                if (value != null)
                    labelControl.Text = value.ToString();
            }
            #endregion

            label.TextAlign = labelControl.TextAlign;
            label.Text = labelControl.Text;

            return label;
        }

        protected virtual Control ConvertReportControlToGrid(StiGridControl gridControl)
        {
            var grid = new DataGridView();
#if !BLAZOR
#if !NETCOREAPP
            gridControl.Control = grid;
#endif
            ConvertReportControlToControl(grid, gridControl);

            grid.ColumnHeadersVisible = gridControl.ColumnHeadersVisible;
            grid.RowHeadersVisible = gridControl.RowHeadersVisible;
#if !NETCOREAPP
            //grid.GridLine = gridControl.GridLineStyle;
#endif
            grid.ColumnHeadersDefaultCellStyle.Font = gridControl.HeaderFont;
            grid.AutoResizeColumns();

            grid.ColumnHeadersHeight = Stimulsoft.Base.StiScale.YYI(20);
            grid.RowTemplate.Height = gridControl.PreferredRowHeight;

            if (gridControl.DataSource != null)
            {
                var dataSource = gridControl.DataSource;

                #region Create Table
                //var tableStyle = new DataGridTableStyle();
                if (gridControl.Columns.Count == 0)
                {
                    grid.RowsDefaultCellStyle.BackColor = gridControl.BackgroundColor;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = gridControl.AlternatingBackColor;

                    //grid.GridLineColor = gridControl.GridLineColor;
                    grid.ForeColor = gridControl.ForeColor;
                    grid.BackColor = gridControl.BackColor;
                    //grid.HeaderBackColor = gridControl.HeaderBackColor;
                    //grid.HeaderForeColor = gridControl.HeaderForeColor;
                    //grid.SelectionBackColor = gridControl.SelectionBackColor;
                    //grid.SelectionForeColor = gridControl.SelectionForeColor;
                }

                var dataSet = new DataSet();
                var table = new DataTable(dataSource.Name);
                dataSet.Tables.Add(table);

                #region Create columns from gridControl.Columns
                if (gridControl.Columns.Count > 0)
                {
                    foreach (StiGridColumn column in gridControl.Columns)
                    {
                        if (!column.Visible || column.DataTextField.Length <= 0) continue;

                        var dtColumn = StiDataColumn.GetDataColumnFromColumnName(
                            dataSource.Dictionary, column.DataTextField);

                        if (dtColumn == null) continue;

                        var columnInRow = StiDataColumn.GetColumnNameFromDataColumn(dataSource.Dictionary,
                            column.DataTextField);

                        table.Columns.Add(new DataColumn(columnInRow, dtColumn.Type)
                        {
                            Caption = dtColumn.Alias,
                            AllowDBNull = true
                        });

                        var columnStyle = new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = columnInRow,
                            HeaderText = column.HeaderText.Length == 0 ? dtColumn.Alias : column.HeaderText
                        };
                        columnStyle.DefaultCellStyle.Alignment = ToDataGridViewContentAlignment(column.Alignment);

                        if (column.Width != 0)
                            columnStyle.Width = column.Width;

                        grid.Columns.Add(columnStyle);
                    }
                }
                #endregion

                #region Auto Refresh Columns
                else
                {
                    foreach (StiDataColumn column in dataSource.Columns)
                    {
                        table.Columns.Add(new DataColumn(column.Name, column.Type)
                        {
                            Caption = column.Alias,
                            AllowDBNull = true
                        });
                    }
                }
                #endregion
                #endregion

                #region Fill data
                while (!dataSource.IsEof)
                {
                    var row = table.NewRow();

                    if (gridControl.Columns.Count > 0)
                    {
                        foreach (StiGridColumn column in gridControl.Columns)
                        {
                            if (!column.Visible || column.DataTextField.Length <= 0) continue;

                            var columnInRow = StiDataColumn.GetColumnNameFromDataColumn(dataSource.Dictionary,
                                column.DataTextField);

                            row[columnInRow] = StiDataColumn.GetDataFromDataColumn(
                                dataSource.Dictionary, column.DataTextField);
                        }
                    }
                    else
                    {
                        foreach (StiDataColumn column in dataSource.Columns)
                        {
                            row[column.Name] = dataSource[column.Name];
                        }
                    }

                    table.Rows.Add(row);

                    dataSource.Next();
                }
                dataSource.First();
                #endregion

                var view = new DataView(table);
                view.RowFilter = gridControl.Filter;

                grid.DataSource = view;
                grid.ReadOnly = true;
                //grid.AllowNavigation = false;
                //grid.CaptionVisible = false;
                grid.RowHeadersVisible = false;

                #region Set colors
                if (gridControl.Columns.Count > 0)
                {
                    grid.BackgroundColor = gridControl.BackgroundColor;

                    grid.RowsDefaultCellStyle.BackColor = gridControl.BackgroundColor;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = gridControl.AlternatingBackColor;

                    grid.ForeColor = gridControl.ForeColor;
                    grid.BackColor = gridControl.BackColor;

                    //tableStyle.AlternatingBackColor = gridControl.AlternatingBackColor;
                    //tableStyle.GridLineColor = gridControl.GridLineColor;
                    //tableStyle.ForeColor = gridControl.ForeColor;
                    //tableStyle.BackColor = gridControl.BackColor;
                    //tableStyle.HeaderBackColor = gridControl.HeaderBackColor;
                    //tableStyle.HeaderForeColor = gridControl.HeaderForeColor;
                    //tableStyle.SelectionBackColor = gridControl.SelectionBackColor;
                    //tableStyle.SelectionForeColor = gridControl.SelectionForeColor;

                    //tableStyle.MappingName = dataSource.Name;
                    //grid.TableStyles.Add(tableStyle);
                }
                #endregion

                var manager = Form.BindingContext[grid.DataSource, grid.DataMember];
                managerToGridControl[manager] = gridControl;
                manager.PositionChanged += Manager_PositionChanged;

            }
#endif
            return grid;
        }

        private DataGridViewContentAlignment ToDataGridViewContentAlignment(HorizontalAlignment alignment)
        {
            switch (alignment)
            {
                case HorizontalAlignment.Center:
                    return DataGridViewContentAlignment.MiddleCenter;
                case HorizontalAlignment.Right:
                    return DataGridViewContentAlignment.MiddleRight;
                case HorizontalAlignment.Left:
                    return DataGridViewContentAlignment.MiddleLeft;
            }

            return DataGridViewContentAlignment.MiddleCenter;
        }

        protected virtual Control ConvertReportControlToPictureBox(StiPictureBoxControl pictureBoxControl)
        {
            var pictureBox = new PictureBox();

            pictureBoxControl.Control = pictureBox;
            ConvertReportControlToControl(pictureBox, pictureBoxControl);
            pictureBox.SizeMode = pictureBoxControl.SizeMode;
            pictureBox.BorderStyle = pictureBoxControl.BorderStyle;
            pictureBox.Image = pictureBoxControl.Image;

            return pictureBox;
        }

        protected virtual Control ConvertReportControlToGroupBox(StiGroupBoxControl groupBoxControl)
        {
            var groupBox = new GroupBox();
            groupBoxControl.Control = groupBox;
            ConvertReportControlToControl(groupBox, groupBoxControl);

            #region DataBinding
            if (groupBoxControl.TextBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(Report.Dictionary, groupBoxControl.TextBinding);
                if (value != null)
                    groupBoxControl.Text = value.ToString();
            }
            #endregion

            groupBox.Text = groupBoxControl.Text;

            return groupBox;
        }

        protected virtual Control ConvertReportControlToListBox(StiListBoxControl listBoxControl)
        {
            var listBox = new ListBox();
            listBoxControl.Control = listBox;
            ConvertReportControlToControl(listBox, listBoxControl);

            #region ItemsBinding
            if (listBoxControl.ItemsBinding.Length > 0)
            {
                var values = StiDataColumn.GetDatasFromDataColumn(Report.Dictionary, listBoxControl.ItemsBinding);
                if (listBoxControl.Sorted)
                    Array.Sort(values);

                listBoxControl.Items.ClearCore();
                listBoxControl.Items.AddRangeCore(values);

            }
            listBox.Items.AddRange(listBoxControl.Items.ToArray());

            if (listBox.Items.Count > 0)
                listBox.SelectedItem = listBoxControl.SelectedItem = listBox.Items[0];
            #endregion

            #region SelectedItemBinding
            if (listBoxControl.SelectedItemBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, listBoxControl.SelectedItemBinding);

                listBox.SelectedItem = listBoxControl.SelectedItem = value;
            }
            #endregion

            #region SelectedValueBinding
            if (listBoxControl.SelectedValueBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, listBoxControl.SelectedValueBinding);
                listBox.SelectedValue = listBoxControl.SelectedValue = value;
            }
            #endregion

            listBox.SelectionMode = listBoxControl.SelectionMode;
            listBox.Sorted = listBoxControl.Sorted;
            listBox.ItemHeight = StiScale.I(listBoxControl.ItemHeight);
            listBox.HorizontalScrollbar = true;

            listBox.SelectedIndexChanged += Control_SelectedIndexChanged;

            return listBox;
        }

        protected virtual Control ConvertReportControlToTreeView(StiTreeViewControl treeViewControl)
        {
            var treeView = new TreeView();
            treeView.Scrollable = true;
            treeViewControl.Control = treeView;
            ConvertReportControlToControl(treeView, treeViewControl);

            treeView.AfterSelect += TreeView_AfterSelect;

            return treeView;
        }

        protected virtual Control ConvertReportControlToListView(StiListViewControl listViewControl)
        {
            var listView = new ListView();
            listView.Scrollable = true;
            listViewControl.Control = listView;
            ConvertReportControlToControl(listView, listViewControl);

            listView.SelectedIndexChanged += Control_SelectedIndexChanged;

            return listView;
        }

        protected virtual Control ConvertReportControlToCheckedListBox(StiCheckedListBoxControl checkedListBoxControl)
        {
            var checkedListBox = new CheckedListBox();
            checkedListBoxControl.Control = checkedListBox;
            ConvertReportControlToControl(checkedListBox, checkedListBoxControl);

            checkedListBox.SelectionMode = checkedListBoxControl.SelectionMode;
            checkedListBox.Sorted = checkedListBoxControl.Sorted;
            checkedListBox.ItemHeight = StiScale.I(checkedListBoxControl.ItemHeight);
            checkedListBox.CheckOnClick = checkedListBoxControl.CheckOnClick;
            checkedListBox.HorizontalScrollbar = true;

            #region ItemsBinding
            if (checkedListBoxControl.ItemsBinding.Length > 0)
            {
                var values = StiDataColumn.GetDatasFromDataColumn(Report.Dictionary, checkedListBoxControl.ItemsBinding);
                if (checkedListBoxControl.Sorted)
                    Array.Sort(values);

                checkedListBoxControl.Items.ClearCore();
                checkedListBoxControl.Items.AddRangeCore(values);

            }
            checkedListBox.Items.AddRange(checkedListBoxControl.Items.ToArray());

            if (checkedListBox.Items.Count > 0)
                checkedListBox.SelectedItem = checkedListBoxControl.SelectedItem = checkedListBox.Items[0];
            #endregion

            #region SelectedItemBinding
            if (checkedListBoxControl.SelectedItemBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, checkedListBoxControl.SelectedItemBinding);

                checkedListBox.SelectedItem = checkedListBoxControl.SelectedItem = value;
            }
            #endregion

            #region SelectedValueBinding
            if (checkedListBoxControl.SelectedValueBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, checkedListBoxControl.SelectedValueBinding);
                checkedListBox.SelectedValue = checkedListBoxControl.SelectedValue = value;
            }
            #endregion

            checkedListBox.SelectedIndexChanged += Control_SelectedIndexChanged;

            return checkedListBox;
        }

        protected virtual Control ConvertReportControlToComboBox(StiComboBoxControl comboBoxControl)
        {
            var comboBox = new ComboBox();
            comboBoxControl.Control = comboBox;
            ConvertReportControlToControl(comboBox, comboBoxControl);

            #region ItemsBinding
            if (comboBoxControl.ItemsBinding.Length > 0)
            {
                var values = StiDataColumn.GetDatasFromDataColumn(Report.Dictionary, comboBoxControl.ItemsBinding);
                if (comboBoxControl.Sorted)
                    Array.Sort(values);

                comboBoxControl.Items.ClearCore();
                comboBoxControl.Items.AddRangeCore(values);

            }
            comboBox.Items.AddRange(comboBoxControl.Items.ToArray());

            if (comboBox.Items.Count > 0)
                comboBox.SelectedItem = comboBoxControl.SelectedItem = comboBox.Items[0];
            #endregion

            #region TextBinding
            if (comboBoxControl.TextBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(Report.Dictionary, comboBoxControl.TextBinding);
                if (value != null)
                    comboBoxControl.Text = value.ToString();
            }
            #endregion

            #region SelectedItemBinding
            if (comboBoxControl.SelectedItemBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, comboBoxControl.SelectedItemBinding);

                comboBox.SelectedItem = comboBoxControl.SelectedItem = value;
            }
            #endregion

            #region SelectedValueBinding
            if (comboBoxControl.SelectedValueBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, comboBoxControl.SelectedValueBinding);
                comboBox.SelectedValue = comboBoxControl.SelectedValue = value;
            }
            #endregion

            comboBox.DropDownStyle = comboBoxControl.DropDownStyle;
            comboBox.MaxLength = comboBoxControl.MaxLength;
            comboBox.Sorted = comboBoxControl.Sorted;
            comboBox.ItemHeight = StiScale.I(comboBoxControl.ItemHeight);
            comboBox.MaxDropDownItems = comboBoxControl.MaxDropDownItems;
            comboBox.DropDownWidth = StiScale.I(comboBoxControl.DropDownWidth);

            comboBox.SelectedIndexChanged += Control_SelectedIndexChanged;

            return comboBox;
        }

        protected virtual Control ConvertReportControlToLookUpBox(StiLookUpBoxControl lookUpBoxControl)
        {
            var lookUpBox = new StiDialogLookUpBox();
            lookUpBoxControl.Control = lookUpBox;
            ConvertReportControlToControl(lookUpBox, lookUpBoxControl);

            #region DataBinding
            if (lookUpBoxControl.TextBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(Report.Dictionary, lookUpBoxControl.TextBinding);
                if (value != null)
                    lookUpBoxControl.Text = value.ToString();
            }

            lookUpBox.Items.Clear();
            if (lookUpBoxControl.ItemsBinding.Length > 0)
            {
                lookUpBox.Items.AddRange(StiDataColumn.GetDatasFromDataColumn(Report.Dictionary, lookUpBoxControl.ItemsBinding));

                if (lookUpBox.Items.Count > 0)
                    lookUpBox.SelectedItem = lookUpBoxControl.SelectedItem = lookUpBox.Items[0];
            }
            else lookUpBox.Items.AddRange(lookUpBoxControl.Items.ToArray());

            lookUpBox.Keys.Clear();
            if (lookUpBoxControl.KeysBinding.Length > 0)
            {
                lookUpBox.Keys.AddRange(StiDataColumn.GetDatasFromDataColumn(
                    Report.Dictionary, lookUpBoxControl.KeysBinding));

            }
            else lookUpBox.Keys.AddRange(lookUpBoxControl.Keys.ToArray());

            if (lookUpBoxControl.SelectedItemBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, lookUpBoxControl.SelectedItemBinding);

                lookUpBox.SelectedItem = lookUpBoxControl.SelectedItem = value;
            }

            if (lookUpBoxControl.SelectedKeyBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, lookUpBoxControl.SelectedKeyBinding);

                lookUpBox.SelectedKey = lookUpBoxControl.SelectedKey = value;
            }

            if (lookUpBoxControl.SelectedValueBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, lookUpBoxControl.SelectedValueBinding);
                lookUpBox.SelectedValue = lookUpBoxControl.SelectedValue = value;
            }
            #endregion

            lookUpBox.DropDownStyle = lookUpBoxControl.DropDownStyle;
            lookUpBox.MaxLength = lookUpBoxControl.MaxLength;
            lookUpBox.Sorted = lookUpBoxControl.Sorted;
            lookUpBox.ItemHeight = StiScale.I(lookUpBoxControl.ItemHeight);
            lookUpBox.MaxDropDownItems = lookUpBoxControl.MaxDropDownItems;
            lookUpBox.DropDownWidth = StiScale.I(lookUpBoxControl.DropDownWidth);
            lookUpBox.Flat = false;

            lookUpBox.SelectedIndexChanged += Control_SelectedIndexChanged;

            return lookUpBox;
        }

        protected virtual Control ConvertReportControlToNumericUpDown(StiNumericUpDownControl numericUpDownControl)
        {
            var numericUpDown = new NumericUpDown();
            numericUpDownControl.Control = numericUpDown;
            ConvertReportControlToControl(numericUpDown, numericUpDownControl);

            #region DataBinding
            if (numericUpDownControl.ValueBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, numericUpDownControl.ValueBinding);

                if (value is int)
                    numericUpDownControl.Value = (int)value;
            }

            if (numericUpDownControl.MaximumBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, numericUpDownControl.MaximumBinding);

                if (value is int)
                    numericUpDownControl.Maximum = (int)value;
            }

            if (numericUpDownControl.MinimumBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, numericUpDownControl.MinimumBinding);

                if (value is int)
                    numericUpDownControl.Minimum = (int)value;
            }
            #endregion

            numericUpDown.Minimum = numericUpDownControl.Minimum;
            numericUpDown.Maximum = numericUpDownControl.Maximum;
            numericUpDown.Increment = numericUpDownControl.Increment;
            numericUpDown.Value = numericUpDownControl.Value;

            numericUpDown.ValueChanged += OnValueChanged;

            return numericUpDown;
        }

        protected virtual Control ConvertReportControlToDateTimePicker(StiDateTimePickerControl dateTimePickerControl)
        {
            var dateTimePicker = new DateTimePicker();
            dateTimePickerControl.Control = dateTimePicker;
            ConvertReportControlToControl(dateTimePicker, dateTimePickerControl);

            #region DataBinding
            if (dateTimePickerControl.TagValueBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, dateTimePickerControl.TagValueBinding);

                if (value is DateTime)
                    dateTimePickerControl.Value = (DateTime)value;
            }

            if (dateTimePickerControl.MaxDateBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, dateTimePickerControl.MaxDateBinding);

                if (value is DateTime)
                    dateTimePickerControl.MaxDate = (DateTime)value;
            }

            if (dateTimePickerControl.MinDateBinding.Length > 0)
            {
                var value = StiDataColumn.GetDataFromDataColumn(
                    Report.Dictionary, dateTimePickerControl.MinDateBinding);

                if (value is DateTime)
                    dateTimePickerControl.MinDate = (DateTime)value;
            }
            #endregion

            if (dateTimePickerControl.Today)
                dateTimePickerControl.Value = DateTime.Now;

            dateTimePicker.CalendarFont = dateTimePickerControl.Font;
            dateTimePicker.CalendarForeColor = dateTimePickerControl.ForeColor;
            dateTimePicker.CustomFormat = dateTimePickerControl.CustomFormat;
            dateTimePicker.DropDownAlign = dateTimePickerControl.DropDownAlign;
            dateTimePicker.ShowUpDown = dateTimePickerControl.ShowUpDown;
            dateTimePicker.MaxDate = dateTimePickerControl.MaxDate;
            dateTimePicker.MinDate = dateTimePickerControl.MinDate;
            dateTimePicker.Value = dateTimePickerControl.Value;
            dateTimePicker.Format = dateTimePickerControl.Format;

            dateTimePicker.ValueChanged += OnValueChanged;

            return dateTimePicker;
        }

        protected virtual Control ConvertReportControlToCustomControl(StiCustomControl customControl)
        {
            var control = customControl.Control as Control;
            control.Location = StiScale.I(customControl.Location);
            control.Size = StiScale.I(customControl.Size);

            control.Click += Control_Click;
            control.DoubleClick += Control_DoubleClick;
            control.Enter += Control_Enter;
            control.Leave += Control_Leave;
            control.MouseDown += Control_MouseDown;
            control.MouseUp += Control_MouseUp;
            control.MouseMove += Control_MouseMove;

            return control;
        }

        protected virtual void ConvertDialogsToControls(Control parentControl, StiReportControl control, int tabIndex)
        {
            Control createdControl = null;

            if (control is StiPanelControl)
                createdControl = ConvertReportControlToPanel(control as StiPanelControl);

            if (control is StiTextBoxControl)
                createdControl = ConvertReportControlToTextBox(control as StiTextBoxControl);

            if (control is StiRichTextBoxControl)
                createdControl = ConvertReportControlToRichTextBox(control as StiRichTextBoxControl);

            if (control is StiButtonControl)
                createdControl = ConvertReportControlToButton(control as StiButtonControl);

            if (control is StiCheckBoxControl)
                createdControl = ConvertReportControlToCheckBox(control as StiCheckBoxControl);

            if (control is StiRadioButtonControl)
                createdControl = ConvertReportControlToRadioButton(control as StiRadioButtonControl);

            if (control is StiLabelControl)
                createdControl = ConvertReportControlToLabel(control as StiLabelControl);

            if (control is StiGridControl)
                createdControl = ConvertReportControlToGrid(control as StiGridControl);

            if (control is StiPictureBoxControl)
                createdControl = ConvertReportControlToPictureBox(control as StiPictureBoxControl);

            if (control is StiGroupBoxControl)
                createdControl = ConvertReportControlToGroupBox(control as StiGroupBoxControl);

            if (control is StiListBoxControl)
                createdControl = ConvertReportControlToListBox(control as StiListBoxControl);

            if (control is StiTreeViewControl)
                createdControl = ConvertReportControlToTreeView(control as StiTreeViewControl);

            if (control is StiListViewControl)
                createdControl = ConvertReportControlToListView(control as StiListViewControl);

            if (control is StiCheckedListBoxControl)
                createdControl = ConvertReportControlToCheckedListBox(control as StiCheckedListBoxControl);

            if (control is StiLookUpBoxControl)
                createdControl = ConvertReportControlToLookUpBox(control as StiLookUpBoxControl);

            else if (control is StiComboBoxControl)
                createdControl = ConvertReportControlToComboBox(control as StiComboBoxControl);

            if (control is StiNumericUpDownControl)
                createdControl = ConvertReportControlToNumericUpDown(control as StiNumericUpDownControl);

            if (control is StiDateTimePickerControl)
                createdControl = ConvertReportControlToDateTimePicker(control as StiDateTimePickerControl);

            if (control is StiCustomControl)
            {
                if (((StiCustomControl) control).Control != null)
                    createdControl = ConvertReportControlToCustomControl(control as StiCustomControl);
                else
                    return;
            }

            createdControl.TabIndex = tabIndex;
            parentControl.Controls.Add(createdControl);

            reportToControl[control] = createdControl;
            controlToReport[createdControl] = control;

            if (control.IsReportContainer)
            {
                for (var index = control.Components.Count - 1; index >= 0; index--)
                {
                    var component = control.Components[index] as StiReportControl;
                    ConvertDialogsToControls(createdControl, component, index);
                }
            }
        }

        protected virtual void ConvertReportControlToForm(StiForm formControl)
        {
            Form = new Form();

            var activeForm = Form.ActiveForm;
            if (activeForm != null)
            {
                try
                {
                    uint processid1 = 0;
                    uint processid2 = 0;
                    var threadid1 = GetWindowThreadProcessId(Form.Handle, out processid1);
                    var threadid2 = GetWindowThreadProcessId(activeForm.Handle, out processid2);

                    if (threadid1 == threadid2)
                        Form.Owner = activeForm;
                }
                catch
                {
                }
            }


            formControl.Control = Form;
            Form.Handle.ToString();

            Form.FormBorderStyle = FormBorderStyle.FixedDialog;
            Form.MaximizeBox = false;
            Form.MinimizeBox = false;
            Form.ShowInTaskbar = false;

            Form.Location = StiScale.I(formControl.Location);
            Form.Size = new Size(StiScale.I(formControl.Size.Width), StiScale.I(formControl.Size.Height - 7) + SystemInformation.CaptionHeight);
            Form.WindowState = formControl.WindowState;
            Form.StartPosition = formControl.StartPosition;
            Form.BackColor = formControl.BackColor;
            Form.Font = formControl.Font;
            Form.Text = formControl.Text;
            Form.RightToLeft = formControl.RightToLeft;
            Form.DialogResult = formControl.DialogResult;

            Form.Closed += Form_Closed;
            Form.Closing += Form_Closing;
            Form.Click += Control_Click;
            Form.DoubleClick += Control_DoubleClick;

            reportToControl[formControl] = Form;
            controlToReport[Form] = formControl;

            for (var index = formControl.Components.Count - 1; index >= 0; index--)
            {
                var component = formControl.Components[index] as StiReportControl;
                if (component != null)
                    ConvertDialogsToControls(Form, component, index);
            }
        }
        #endregion

        #region Methods.ConvertControlToReportControl
        protected virtual void ConvertControlToReportControl(StiReportControl reportControl, Control control)
        {
            reportControl.DockStyle = StiComponent.ConvertDock(control.Dock);
            reportControl.Location = StiScale.I(control.Location);
            reportControl.Size = StiScale.I(control.Size);
            reportControl.BackColor = control.BackColor;
            reportControl.ForeColor = control.ForeColor;
            reportControl.Font = control.Font;
            reportControl.RightToLeft = control.RightToLeft;
            reportControl.Enabled = control.Enabled;
            reportControl.TagValue = control.Tag;
        }

        protected virtual void ConvertPanelToReportControl(StiPanelControl panelControl, Control panel)
        {
            ConvertControlToReportControl(panelControl, panel);
            panelControl.BorderStyle = ((Controls.StiDialogPanel)panel).BorderStyle;
        }

        protected virtual void ConvertTextBoxToReportControl(StiTextBoxControl textBoxControl, Control textBox)
        {
            ConvertControlToReportControl(textBoxControl, textBox);
            textBoxControl.MaxLength = ((TextBox)textBox).MaxLength;
            textBoxControl.PasswordChar = ((TextBox)textBox).PasswordChar;
            textBoxControl.Text = ((TextBox)textBox).Text;
            textBoxControl.Multiline = ((TextBox)textBox).Multiline;
            textBoxControl.WordWrap = ((TextBox)textBox).WordWrap;
            textBoxControl.AcceptsReturn = ((TextBox)textBox).AcceptsReturn;
            textBoxControl.AcceptsTab = ((TextBox)textBox).AcceptsTab;
        }

        protected virtual void ConvertRichTextBoxToReportControl(StiRichTextBoxControl textBoxControl, Control textBox)
        {
            ConvertControlToReportControl(textBoxControl, textBox);
            textBoxControl.RtfText = ((RichTextBox)textBox).Rtf;
        }

        protected virtual void ConvertButtonToReportControl(StiButtonControl buttonControl, Control button)
        {
            ConvertControlToReportControl(buttonControl, button);
            buttonControl.DialogResult = ((Button)button).DialogResult;
            buttonControl.Text = button.Text;
        }

        protected virtual void ConvertCheckBoxToReportControl(StiCheckBoxControl checkBoxControl, Control checkBox)
        {
            ConvertControlToReportControl(checkBoxControl, checkBox);
            checkBoxControl.Checked = ((CheckBox)checkBox).Checked;
            checkBoxControl.Text = checkBox.Text;
        }

        protected virtual void ConvertRadioButtonToReportControl(StiRadioButtonControl radioButtonControl, Control radioButton)
        {
            ConvertControlToReportControl(radioButtonControl, radioButton);
            radioButtonControl.Checked = ((RadioButton)radioButton).Checked;
            radioButtonControl.Text = radioButton.Text;
        }

        protected virtual void ConvertLabelToReportControl(StiLabelControl labelControl, Control label)
        {
            ConvertControlToReportControl(labelControl, label);
            labelControl.TextAlign = ((Label)label).TextAlign;
            labelControl.Text = label.Text;
        }

        protected virtual void ConvertGridToReportControl(StiGridControl gridControl, Control grid)
        {
#if !BLAZOR
            ConvertControlToReportControl(gridControl, grid);

            var gd = grid as DataGridView;

            gridControl.ColumnHeadersVisible = gd.ColumnHeadersVisible;
            gridControl.RowHeadersVisible = gd.RowHeadersVisible;
//#if !NETCOREAPP
            //gridControl.GridLineStyle = gd.GridLineStyle;
//#endif
            gridControl.HeaderFont = gd.ColumnHeadersDefaultCellStyle.Font;
            //gridControl.PreferredColumnWidth = gd.PreferredColumnWidth;
            gridControl.PreferredRowHeight = gd.RowTemplate.Height;
            gridControl.RowHeaderWidth = gd.ColumnHeadersHeight;

            gridControl.AlternatingBackColor = gd.AlternatingRowsDefaultCellStyle.BackColor;
            gridControl.BackgroundColor = gd.BackgroundColor;
            //gridControl.GridLineColor = gd.GridLineColor;
            gridControl.ForeColor = gd.ForeColor;
            gridControl.BackColor = gd.BackColor;
            //gridControl.HeaderBackColor = gd.HeaderBackColor;
            //gridControl.HeaderForeColor = gd.HeaderForeColor;
            //gridControl.SelectionBackColor = gd.SelectionBackColor;
            //gridControl.SelectionForeColor = gd.SelectionForeColor;
#endif
        }

        protected virtual void ConvertPictureBoxToReportControl(StiPictureBoxControl pictureBoxControl, Control pictureBox)
        {
            ConvertControlToReportControl(pictureBoxControl, pictureBox);
            pictureBoxControl.SizeMode = ((PictureBox)pictureBox).SizeMode;
            pictureBoxControl.BorderStyle = ((PictureBox)pictureBox).BorderStyle;
            pictureBoxControl.Image = ((PictureBox)pictureBox).Image;
        }

        protected virtual void ConvertGroupBoxToReportControl(StiGroupBoxControl groupBoxControl, Control groupBox)
        {
            ConvertControlToReportControl(groupBoxControl, groupBox);
            groupBoxControl.Text = groupBox.Text;
        }

        protected virtual void ConvertListBoxToReportControl(StiListBoxControl listBoxControl, ListBox listBox)
        {
            ConvertControlToReportControl(listBoxControl, listBox);
            listBoxControl.SelectionMode = listBox.SelectionMode;
            listBoxControl.Sorted = listBox.Sorted;
            listBoxControl.SelectedIndex = listBox.SelectedIndex;
            listBoxControl.SelectedItem = listBox.SelectedItem;
            listBoxControl.SelectedValue = listBox.SelectedValue;
            listBoxControl.ItemHeight = (int)(listBox.ItemHeight / StiScale.Factor);

            listBoxControl.Items.ClearCore();
            foreach (var obj in listBox.Items) listBoxControl.Items.AddCore(obj);
        }

        protected virtual void ConvertTreeViewToReportControl(StiTreeViewControl treeViewControl, TreeView treeView)
        {
            ConvertControlToReportControl(treeViewControl, treeView);
        }

        protected virtual void ConvertListViewToReportControl(StiListViewControl listViewControl, ListView listView)
        {
            ConvertControlToReportControl(listViewControl, listView);
        }

        protected virtual void ConvertCheckedListBoxToReportControl(StiCheckedListBoxControl checkedListBoxControl, CheckedListBox checkedListBox)
        {
            ConvertControlToReportControl(checkedListBoxControl, checkedListBox);
            checkedListBoxControl.SelectionMode = checkedListBox.SelectionMode;
            checkedListBoxControl.Sorted = checkedListBox.Sorted;
            checkedListBoxControl.SelectedIndex = checkedListBox.SelectedIndex;
            checkedListBoxControl.SelectedItem = checkedListBox.SelectedItem;
            checkedListBoxControl.SelectedValue = checkedListBox.SelectedValue;
            checkedListBoxControl.ItemHeight = (int)(checkedListBox.ItemHeight / StiScale.Factor);
            checkedListBoxControl.CheckOnClick = checkedListBox.CheckOnClick;

            #region Checked Items
            var items = new object[checkedListBox.CheckedItems.Count];
            var index = 0;
            foreach (var obj in checkedListBox.CheckedItems)
            {
                items[index++] = obj;
            }
            checkedListBoxControl.CheckedItems = items;
            #endregion

            checkedListBoxControl.Items.ClearCore();
            foreach (var obj in checkedListBox.Items)
                checkedListBoxControl.Items.AddCore(obj);
        }

        protected virtual void ConvertComboBoxToReportControl(StiComboBoxControl comboBoxControl, ComboBox comboBox)
        {
            ConvertControlToReportControl(comboBoxControl, comboBox);
            comboBoxControl.DropDownStyle = comboBox.DropDownStyle;
            comboBoxControl.MaxLength = comboBox.MaxLength;
            comboBoxControl.Sorted = comboBox.Sorted;
            comboBoxControl.SelectedIndex = comboBox.SelectedIndex;
            comboBoxControl.SelectedItem = comboBox.SelectedItem;
            comboBoxControl.SelectedValue = comboBox.SelectedValue;
            comboBoxControl.ItemHeight = (int)(comboBox.ItemHeight / StiScale.Factor);
            comboBoxControl.MaxDropDownItems = comboBox.MaxDropDownItems;
            comboBoxControl.DropDownWidth = (int)(comboBox.DropDownWidth / StiScale.Factor);
            comboBoxControl.Text = comboBox.Text;

            comboBoxControl.Items.ClearCore();
            foreach (var obj in comboBox.Items)
                comboBoxControl.Items.AddCore(obj);
        }

        protected virtual void ConvertLookUpBoxToReportControl(StiLookUpBoxControl lookUpBoxControl, StiDialogLookUpBox lookUpBox)
        {
            ConvertControlToReportControl(lookUpBoxControl, lookUpBox);
            lookUpBoxControl.DropDownStyle = lookUpBox.DropDownStyle;
            lookUpBoxControl.MaxLength = lookUpBox.MaxLength;
            lookUpBoxControl.Sorted = lookUpBox.Sorted;
            lookUpBoxControl.SelectedIndex = lookUpBox.SelectedIndex;
            lookUpBoxControl.SelectedItem = lookUpBox.SelectedItem;
            lookUpBoxControl.SelectedValue = lookUpBox.SelectedValue;
            lookUpBoxControl.SelectedKey = lookUpBox.SelectedKey;
            lookUpBoxControl.ItemHeight = (int)(lookUpBox.ItemHeight / StiScale.Factor);
            lookUpBoxControl.MaxDropDownItems = lookUpBox.MaxDropDownItems;
            lookUpBoxControl.DropDownWidth = (int)(lookUpBox.DropDownWidth / StiScale.Factor);
            lookUpBoxControl.Text = lookUpBox.Text;

            lookUpBoxControl.Items.Clear();
            foreach (var obj in lookUpBox.Items)
                lookUpBoxControl.Items.Add(obj);

            lookUpBoxControl.Keys.Clear();
            foreach (var obj in lookUpBox.Keys)
                lookUpBoxControl.Keys.Add(obj);
        }

        protected virtual void ConvertNumericUpDownToReportControl(StiNumericUpDownControl numericUpDownControl, NumericUpDown numericUpDown)
        {
            ConvertControlToReportControl(numericUpDownControl, numericUpDown);
            numericUpDownControl.Minimum = (int)numericUpDown.Minimum;
            numericUpDownControl.Maximum = (int)numericUpDown.Maximum;
            numericUpDownControl.Increment = (int)numericUpDown.Increment;
            numericUpDownControl.Value = (int)numericUpDown.Value;
        }

        protected virtual void ConvertDateTimePickerToReportControl(StiDateTimePickerControl dateTimePickerControl, DateTimePicker dateTimePicker)
        {
            ConvertControlToReportControl(dateTimePickerControl, dateTimePicker);
            dateTimePickerControl.CustomFormat = dateTimePicker.CustomFormat;
            dateTimePickerControl.DropDownAlign = dateTimePicker.DropDownAlign;
            dateTimePickerControl.ShowUpDown = dateTimePicker.ShowUpDown;
            dateTimePickerControl.MaxDate = dateTimePicker.MaxDate;
            dateTimePickerControl.MinDate = dateTimePicker.MinDate;
            dateTimePickerControl.Value = dateTimePicker.Value;
            dateTimePickerControl.Format = dateTimePicker.Format;
        }

        protected virtual void ConvertControlsToDialogs(Control parentControl, StiReportControl control)
        {
            var createdControl = reportToControl[control] as Control;
            if (createdControl == null) return;

            if (control is StiPanelControl)
                ConvertPanelToReportControl(control as StiPanelControl, createdControl);

            if (control is StiTextBoxControl)
                ConvertTextBoxToReportControl(control as StiTextBoxControl, createdControl);

            if (control is StiRichTextBoxControl)
                ConvertRichTextBoxToReportControl(control as StiRichTextBoxControl, createdControl);

            if (control is StiButtonControl)
                ConvertButtonToReportControl(control as StiButtonControl, createdControl);

            if (control is StiCheckBoxControl)
                ConvertCheckBoxToReportControl(control as StiCheckBoxControl, createdControl);

            if (control is StiRadioButtonControl)
                ConvertRadioButtonToReportControl(control as StiRadioButtonControl, createdControl);

            if (control is StiLabelControl)
                ConvertLabelToReportControl(control as StiLabelControl, createdControl);

            if (control is StiGridControl)
                ConvertGridToReportControl(control as StiGridControl, createdControl);

            if (control is StiPictureBoxControl)
                ConvertPictureBoxToReportControl(control as StiPictureBoxControl, createdControl);

            if (control is StiGroupBoxControl)
                ConvertGroupBoxToReportControl(control as StiGroupBoxControl, createdControl);

            if (control is StiListBoxControl)
                ConvertListBoxToReportControl(control as StiListBoxControl, (ListBox)createdControl);

            if (control is StiTreeViewControl)
                ConvertTreeViewToReportControl(control as StiTreeViewControl, (TreeView)createdControl);

            if (control is StiListViewControl)
                ConvertListViewToReportControl(control as StiListViewControl, (ListView)createdControl);

            if (control is StiCheckedListBoxControl)
                ConvertCheckedListBoxToReportControl(control as StiCheckedListBoxControl, (CheckedListBox)createdControl);

            if (control is StiLookUpBoxControl)
                ConvertLookUpBoxToReportControl(control as StiLookUpBoxControl, (StiDialogLookUpBox)createdControl);

            else if (control is StiComboBoxControl)
                ConvertComboBoxToReportControl(control as StiComboBoxControl, (ComboBox)createdControl);

            if (control is StiNumericUpDownControl)
                ConvertNumericUpDownToReportControl(control as StiNumericUpDownControl, (NumericUpDown)createdControl);

            if (control is StiDateTimePickerControl)
                ConvertDateTimePickerToReportControl(control as StiDateTimePickerControl, (DateTimePicker)createdControl);


            if (!control.IsReportContainer) return;

            foreach (StiReportControl component in control.Components)
            {
                ConvertControlsToDialogs(createdControl, component);
            }
        }

        protected virtual void FormToReportControl(StiForm formControl)
        {
            if (formControl == null) return;

            formControl.Location = new Point((int)(Form.Location.X / StiScale.Factor), (int)(Form.Location.Y / StiScale.Factor));
            formControl.Size = new Size((int)(Form.Size.Width / StiScale.Factor), (int)(Form.Size.Height / StiScale.Factor));
            formControl.WindowState = Form.WindowState;
            formControl.StartPosition = Form.StartPosition;
            formControl.BackColor = Form.BackColor;
            formControl.Font = Form.Font;
            formControl.Text = Form.Text;
            formControl.RightToLeft = Form.RightToLeft;
            formControl.DialogResult = Form.DialogResult;

            foreach (StiComponent component in formControl.Components)
            {
                var control = component as StiReportControl;
                if (control != null)
                    ConvertControlsToDialogs(Form, control);
            }
        }
        #endregion

        #region Methods
        private void OnFormClose(object sender, EventArgs e)
        {
            Form.Close();
        }

        private void OnReportControlUpdate(object sender, StiReportControlUpdateEventArgs e)
        {
            if (lockReportControlUpdate) return;

            var propertyName = e.PropertyName;
            var control = reportToControl[sender] as Control;
            var reportControl = sender as StiComponent;

            PropertyInfo reportInfo;

            //Special checking for RightToLeft property because StiForm contain two RightToLeft property
            if (propertyName == "RightToLeft")
                reportInfo = reportControl.GetType().GetProperty(propertyName, typeof(RightToLeft));

            else if (propertyName == "BorderStyle")
                reportInfo = reportControl.GetType().GetProperty(propertyName, typeof(BorderStyle));

            else
                reportInfo = reportControl.GetType().GetProperty(propertyName);

            PropertyInfo controlInfo = null;

            try
            {
                if (propertyName == "BorderStyle")
                    controlInfo = control.GetType().GetProperty(propertyName, typeof(BorderStyle));

                else if (propertyName == "Value" && reportControl is StiDateTimePickerControl)
                    controlInfo = control.GetType().GetProperty(propertyName, typeof(DateTime));

                else if (propertyName == "Value")
                    controlInfo = control.GetType().GetProperty(propertyName, typeof(decimal));

                else if (propertyName == "Filter")
                {
                    var grid = control as DataGridView;
                    if (grid != null)
                    {
                        var value3 = reportInfo.GetValue(reportControl, null);

                        var view = grid.DataSource as DataView;
                        if (view != null)
                            view.RowFilter = value3 as string;
                    }
                }
                else
                {
                    var properties = control.GetType().GetProperties();

                    foreach (var propertyInfo in properties)
                    {
                        if (propertyName == propertyInfo.Name && (propertyInfo.PropertyType == reportInfo.PropertyType || propertyName == "Items" || propertyName == "Keys"))
                        {
                            controlInfo = propertyInfo;
                            break;
                        }
                    }
                }
            }
            catch
            {
            }

            if (reportInfo == null) return;
            if (controlInfo == null) return;

            var value = reportInfo.GetValue(reportControl, null);
            if (value is StiArrayList && propertyName == "Items" && control is ListBox)
            {
                var listBox = control as ListBox;
                listBox.Items.Clear();

                if (reportControl is StiCheckedListBoxControl)
                    listBox.Items.AddRange(((StiCheckedListBoxControl)reportControl).Items.ToArray());
                else
                    listBox.Items.AddRange(((StiListBoxControl)reportControl).Items.ToArray());

            }
            else if (value is StiArrayList && propertyName == "Items" && control is ComboBox)
            {
                var comboBox = control as ComboBox;
                comboBox.Items.Clear();
                comboBox.Items.AddRange(((StiComboBoxControl)reportControl).Items.ToArray());
            }
            else if (value is StiArrayList && propertyName == "Keys" && control is StiDialogLookUpBox)
            {
                var lookUpBox = control as StiDialogLookUpBox;
                lookUpBox.Keys.Clear();
                lookUpBox.Keys.AddRange(((StiLookUpBoxControl)reportControl).Keys.ToArray());
            }
            else if (value is StiArrayList && propertyName == "Items" && control is CheckedListBox)
            {
                var checkedListBox = control as CheckedListBox;
                checkedListBox.Items.Clear();
                checkedListBox.Items.AddRange(((StiCheckedListBoxControl)reportControl).Items.ToArray());
            }
            else if (propertyName == "Filter")
            {
                var grid = control as DataGridView;
                if (grid != null)
                {
                    var view = grid.DataSource as DataView;
                    if (view != null)
                        view.RowFilter = value as string;
                }
            }
            else
            {
                try
                {
                    if (value is IConvertible)
                    {
                        value = Convert.ChangeType(value, controlInfo.PropertyType);
                        controlInfo.SetValue(control, value, null);
                    }
                    else if (value is Color)
                    {
                        controlInfo.SetValue(control, value, null);
                    }
                }
                catch
                {
                }
            }
        }

        public override void PrepareForm()
        {
            controlToReport = new Hashtable();
            reportToControl = new Hashtable();
            managerToGridControl = new Hashtable();
            toolTip = new ToolTip();
        }

        public override void DisposeForm()
        {
            if (controlToReport != null)
                controlToReport.Clear();

            if (reportToControl != null)
                reportToControl.Clear();

            if (managerToGridControl != null)
                managerToGridControl.Clear();

            controlToReport = null;
            reportToControl = null;
            managerToGridControl = null;

            if (Form != null)
                Form.Dispose();

            Form = null;

            if (toolTip != null)
                toolTip.Dispose();

            toolTip = null;

            formControl = null;
        }

        public override void LoadForm(IStiForm formControl)
        {
            this.formControl = formControl as StiForm;

            ConvertReportControlToForm(formControl as StiForm);
            this.formControl.ReportControlUpdate += OnReportControlUpdate;
            this.formControl.FormClose += OnFormClose;
            this.formControl.InvokeLoadForm(EventArgs.Empty);
        }

        public override void CloseForm()
        {
            if (formControl == null || Form == null) return;

            formControl.ReportControlUpdate -= OnReportControlUpdate;
            formControl.FormClose -= OnFormClose;

            FormToReportControl(formControl);
        }

        /// <summary>
        /// Render form.
        /// </summary>
        public override bool RenderForm(IStiForm formControl)
        {
            try
            {
                this.formControl = formControl as StiForm;

                PrepareForm();

                if (formControl.Visible)
                {
                    if (Report.Progress != null && Report.Progress.IsVisible)
                        Report.Progress.Hide();

                    try
                    {
                        LoadForm(formControl);

                        if (formControl.Visible)
                        {
                            var result = Form.ShowDialog();
                            CloseForm();

                            if (result == DialogResult.Cancel)
                                return false;
                        }
                    }
                    catch (Exception ee)
                    {
                        StiExceptionProvider.Show(ee);
                        return false;
                    }
                }
            }
            finally
            {
                CloseForm();
            }

            return true;
        }

        /// <summary>
        /// Render all forms in report.
        /// </summary>
        public override bool Render(StiReport report, StiFormStartMode startMode)
        {
            try
            {
                this.Report = report;
                foreach (StiPage page in report.Pages)
                {
                    var formControl = page as StiForm;

                    if (formControl != null && formControl.StartMode == startMode && !this.RenderForm(formControl))
                        return false;
                }
                return true;
            }
            finally
            {
                this.Report = null;
            }
        }

        public override IStiForm CreateForm(StiReport report)
        {
            return new StiForm(report);
        }

        public override IStiTextBoxControl CreateTextBoxControl()
        {
            return new StiTextBoxControl();
        }

        public override IStiLabelControl CreateLabelControl()
        {
            return new StiLabelControl();
        }

        public override IStiCheckBoxControl CreateCheckBoxControl()
        {
            return new StiCheckBoxControl();
        }

        public override IStiPictureBoxControl CreatePictureBoxControl()
        {
            return new StiPictureBoxControl();
        }
        #endregion

        public StiWinDialogsProvider(StiReport report)
        {
            this.Report = report;
        }

        public StiWinDialogsProvider() : this(null)
        {
        }
    }
}
