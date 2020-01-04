using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Office.Interop;
using System.Data;
using System.Windows;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using NPOI.SS.UserModel;

namespace Wpf_Audit
{
    class ExportToExcel
    {
        public static void Export(DataGrid dataGrid, string _fileName)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (dataGrid.Columns[i].Visibility == Visibility.Visible)//只导出可见列
                {
                    dt.Columns.Add(dataGrid.Columns[i].Header.ToString());//构建表头
                }
            }

            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                int columnsIndex = 0;
                DataRow row = dt.NewRow();
                for (int j = 0; j < dataGrid.Columns.Count; j++)
                {
                    if (dataGrid.Columns[j].Visibility == Visibility.Visible)
                    {
                        if (dataGrid.Items[i] != null && (dataGrid.Columns[j].GetCellContent(dataGrid.Items[i]) as TextBlock) != null)//填充可见列数据
                        {
                            row[columnsIndex] = (dataGrid.Columns[j].GetCellContent(dataGrid.Items[i]) as TextBlock).Text.ToString();
                        }
                        else
                        {
                            row[columnsIndex] = "";
                        }
                        columnsIndex++;
                    }
                }
                dt.Rows.Add(row);
            }

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";
            save.Title = "请选择文件导出路径";
            save.FileName = _fileName;

            if (save.ShowDialog() == true)
            {
                string fileName = save.FileName;
                ExportFile(dt, fileName);
            }
        }

        public static void ExportFile(DataTable dt, string excelFilePath = null)
        {
            try
            {
                if (dt == null || dt.Columns.Count == 0)
                    throw new Exception("请检查数据是否为空");

                // load excel, and create a new workbook
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Workbooks.Add();

                // single worksheet
                Microsoft.Office.Interop.Excel._Worksheet workSheet = excelApp.ActiveSheet;

                //标题样式
                

                // column headings
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    workSheet.Cells[1, i + 1] = dt.Columns[i].ColumnName;
                }

                // rows
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    // to do: format datetime values before printing
                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        workSheet.Cells[i + 2, j + 1] = dt.Rows[i][j];
                    }
                }

                // check file path
                if (!string.IsNullOrEmpty(excelFilePath))
                {
                    workSheet.SaveAs(excelFilePath);
                    excelApp.Quit();
                    MessageBox.Show("文件导出成功", "消息提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                { // no file path is given
                    excelApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败！" + ex.Message, "消息提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}