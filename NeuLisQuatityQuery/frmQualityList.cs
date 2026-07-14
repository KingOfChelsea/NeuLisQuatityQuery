using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeuLisQuatityQuery
{
    public partial class frmQualityList : Form
    {
        public frmQualityList()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 类别
        /// </summary>
        public string typeclass { get; set; }
        /// <summary>
        /// 统计类型
        /// </summary>
        public string typeid { get; set; }
        /// <summary>
        /// 统计分子还是分母
        /// </summary>
        public string typename { get; set; }
        public string begDate { get; set; }
        public string endDate { get; set; }
        public string groupid { get; set; }
        // 存储重复值（你要标色的列名）
        private HashSet<object> _duplicateValues = new HashSet<object>();


        private void frmQualityList_Load(object sender, EventArgs e)
        {
            QueryList();
        }
        private void QueryList()
        {
            NeuLis.DataBase.QualityOperDB operdb = new NeuLis.DataBase.QualityOperDB();
            if (typeclass=="不合格标本")
            {
                if (typename == "totalnum")
                {
                    DataTable alSamReg = new DataTable();
                    //查询标本清单
                    if (typeid.IndexOf("标本凝集") >= 0)
                    {
                        alSamReg = operdb.getBarcodeNJList(begDate,endDate,groupid);
                    }
                    else
                    {
                        alSamReg = operdb.getBarcodeList(begDate, endDate, groupid);
                    }
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                    MarkDuplicateCells("条码号");
                }
                else
                {
                    if(typeid.IndexOf("标本拒收率") >= 0)
                    {
                        //查询不合格标本清单
                        DataTable alSamReg = new DataTable();
                        alSamReg = operdb.getBadSampleList(begDate, endDate, groupid, "ALL");
                        this.gridControl1.DataSource = alSamReg;
                        this.gridView1.RefreshData();
                        gridView1.IndicatorWidth = 70;
                        this.gridView1.BestFitColumns();
                    }
                    else
                    {
                        //查询不合格标本清单
                        DataTable alSamReg = new DataTable();
                        alSamReg = operdb.getBadSampleList(begDate, endDate, groupid, typeid);
                        this.gridControl1.DataSource = alSamReg;
                        this.gridView1.RefreshData();
                        gridView1.IndicatorWidth = 70;
                        this.gridView1.BestFitColumns();
                    }
                   
                    MarkDuplicateCells("条码号");
                }
            }
            else if(typeclass=="血培养")
            {
                if (typename == "totalnum")
                {
                    //查询血培养总数
                    DataTable alSamReg = new DataTable();
                    alSamReg = operdb.getXPYSampleList(begDate, endDate, groupid);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                    MarkDuplicateCells("就诊流水号");
                }
                else
                {
                    //查询血培养污染数
                    DataTable alSamReg = new DataTable();
                    alSamReg = operdb.getXPYWRSampleList(begDate, endDate, groupid);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                    MarkDuplicateCells("就诊流水号");
                }
            }
            else if (typeclass == "质控")
            {
                if(typename == "totalnum")
                {
                    //查询应开展质控项目
                    DataTable alSamReg = new DataTable();
                    alSamReg = operdb.getQCItemList(begDate, endDate, groupid);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                    MarkDuplicateCells("项目编码");
                }
                else
                {
                    //查询实际开展质控
                    DataTable alSamReg = new DataTable();
                    alSamReg = operdb.getQCRsultItemList(begDate, endDate, groupid);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                    MarkDuplicateCells("项目编码");
                }
            }
            else if (typeclass == "质控变异系数")
            {
                if (typename == "totalnum")
                {
                }
                else
                {
                    //查询实际开展质控
                    DataTable alSamReg = new DataTable();
                    string begMonth = begDate.Substring(0, 6);
                    string endMonth = endDate.Substring(0, 6);
                    alSamReg = operdb.getQCBYItemList(begMonth, endMonth, groupid);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                    MarkDuplicateCells("项目编码");
                }
            }
            else if (typeclass == "检验报告")
            {
                if (typename == "totalnum")
                {
                    //查询血培养总数
                    DataTable alSamReg = new DataTable();
                    alSamReg = operdb.getSampleList(begDate, endDate, groupid);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                    MarkDuplicateCells("条码号");
                }
                else
                {
                    //查询血培养污染数
                    DataTable alSamReg = new DataTable();
                    alSamReg = operdb.getCancleSampleList(begDate, endDate, groupid);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                    MarkDuplicateCells("条码号");
                }
            }
            else if (typeclass == "危急值")
            {
                if (typename == "totalnum" || typeid=="危急值通报率")
                {
                    //查询血培养总数
                    DataTable alSamReg = new DataTable();
                    alSamReg = operdb.getAlertSampleList(begDate, endDate, groupid);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                    MarkDuplicateCells("条码号");
                }
                else
                {
                    //查询血培养污染数
                    DataTable alSamReg = new DataTable();
                    alSamReg = operdb.getNoOverAlertSampleList(begDate, endDate, groupid);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                    MarkDuplicateCells("条码号");
                }
            }
            else if(typeclass =="TAT")
            {
                DataTable dt =  operdb.getTATItemList(begDate,endDate,typeid,groupid, typename);
                this.gridControl1.DataSource = dt;
                this.gridControl1.Refresh();
                this.gridView1.RefreshData();
                this.gridView1.BestFitColumns();
            }
        }
        /// <summary>
        /// 显示行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Title = "导出Excel";
                fileDialog.Filter = "(Excel)*.xlsx|*.xlsx|(Excel)*.xls|*.xls";
                string pathName = "";
                if (pathName != "")
                    fileDialog.FileName = pathName;
                DialogResult dialogResult = fileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    if (fileDialog.FilterIndex == 2)
                        gridControl1.ExportToXls(fileDialog.FileName);
                    else
                    {
                        gridControl1.ExportToXlsx(fileDialog.FileName);
                    }
                    if (XtraMessageBox.Show("导出成功,是否打开", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(fileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {

                XtraMessageBox.Show("导出失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
           
        }
        /// <summary>
        /// 标记某一列的重复值
        /// </summary>
        /// <param name="fieldName">列字段名</param>
        private void MarkDuplicateCells(string fieldName)
        {
            _duplicateValues.Clear();
            Dictionary<object, int> valueCount = new Dictionary<object, int>();

            // 统计每个值出现次数
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                if (gridView1.IsGroupRow(i)) continue;

                object val = gridView1.GetRowCellValue(i, fieldName);
                if (val == null || val == DBNull.Value) continue;

                if (!valueCount.ContainsKey(val))
                    valueCount[val] = 0;

                valueCount[val]++;
            }

            // 把出现 ≥2 次的值加入重复集合
            foreach (var item in valueCount)
            {
                if (item.Value >= 2)
                    _duplicateValues.Add(item.Key);
            }

            gridView1.RefreshData();
        }

        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            // 只对【你要标色的那一列】生效（改成你的列名）
            if (e.Column.FieldName == "条码号")
            {
                object cellValue = e.CellValue;
                if (cellValue != null && cellValue != DBNull.Value)
                {
                    if (_duplicateValues.Contains(cellValue))
                    {
                        // 重复单元格颜色
                        e.Appearance.BackColor = Color.Red;
                        e.Appearance.Options.UseBackColor = true;
                    }
                }
            }
        }
    }
}

