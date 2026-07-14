using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using NeuLis.Models;
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
    public partial class frmQueryList : Form
    {
        public frmQueryList()
        {
           
           InitializeComponent();
        }
        public string kind;
        public string typeid;
        public string typename;
        public string month;
        public string patienttype;
        public string typeclass;

        private void frmQueryList_Load(object sender, EventArgs e)
        {
            NeuLis.DataBase.OperDB operdb = new NeuLis.DataBase.OperDB();
            if(kind =="危急值")
            {
                if (typename == "分母：同期需要危急值通报的检验项目总数")
                {
                    //查询危急值总数
                    DataTable alAlterReg = operdb.getLifeAlterList(month);
                    this.gridControl1.DataSource = alAlterReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                }
                else if(typename == "已通报的危急值检验项目数")
                {
                    //查询已打电话数量
                    DataTable alSamReg = operdb.getPhoneAlterList(month);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                }
                else if(typename == "危急值通报及时数")
                {
                    this.labelControl1.Visible = true;
                    DataTable dt = operdb.GetMonthJSAlterList(month);
                    this.gridControl1.DataSource = dt;
                    this.gridView1.RefreshData();
                    this.gridView1.BestFitColumns();
                    gridView1.IndicatorWidth = 70;
                }

            }
            else if(kind== "检验报告不正确")
            {
                if (typename == "分母：同期检验报告总数")
                {
                    //查询报告单清单
                    List<Model.barcodeReg> alSamReg = operdb.getReportList(month);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                }
                else
                {
                    //查询不正确检验单清单
                    DataTable alSamReg = operdb.getErrReportList( month);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                }
            }
            else if (kind == "血培养")
            {
                if (typename == "分母：同期血培养总套数")
                {
                    //查询报告单清单
                    DataTable alSamReg = operdb.GetMonthXPYList(month);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                }
                else
                {
                    //查询不正确检验单清单
                    DataTable alSamReg = operdb.GetMonthXPYWRList(month);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                }
            }
            else if (kind == "质控")
            {
                if (typename == "分母：对室内质控项目变异系数有要求的检验项目总数")
                {
                    //查询报告单清单
                    DataTable alSamReg = operdb.GetMonthQCStanderList(month);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                }
                else
                {
                    //查询不正确检验单清单
                    DataTable alSamReg = operdb.GetMonthQCoverList(month);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                }
            }
            else if(kind =="不合格")
            {
                if(typename =="标本总数")
                {
                    DataTable alSamReg = new DataTable();
                    //查询标本清单
                    if (typeid.IndexOf("抗凝标本")>=0)
                    {
                        alSamReg = operdb.getKNBarcodeList(month);
                    }
                    else
                    {
                        alSamReg = operdb.getBarcodeList(month);
                    }
                    
                    
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                }
                else
                {
                    //查询不合格标本清单
                    List<Model.sampleReject> alSamReg = operdb.getRejectSampleList(typename, month);
                    this.gridControl1.DataSource = alSamReg;
                    this.gridView1.RefreshData();
                    gridView1.IndicatorWidth = 70;
                    this.gridView1.BestFitColumns();
                }
               
            }
            else if(kind =="TAT")
            {
                if(patienttype=="加急")
                {
                    patienttype = "1";
                }
                if (typeclass == "全部")
                {
                    DataTable dt = operdb.GetAroundAllList(month,patienttype);
                    this.gridControl1.DataSource = dt;
                    this.gridView1.RefreshData();
                    this.gridView1.BestFitColumns();
                    gridView1.IndicatorWidth = 70;
                }
                else
                {
                    DataTable dt = operdb.GetAroundClassList(month, patienttype, typeclass);
                    this.gridControl1.DataSource = dt;
                    this.gridView1.RefreshData();
                    this.gridView1.BestFitColumns();
                    gridView1.IndicatorWidth = 70;
                }
            }
            
        }

        private void gridView1_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
         
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if(e.Info.IsRowIndicator && e.RowHandle>=0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void btnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ImportExcel(this.gridControl1);
        }
        /// <summary>
        /// 导出Excel支持 xlsx 和xls
        /// </summary>
        public void ImportExcel(GridControl gridControl1, string pathName = "")
        {
            try
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Title = "导出Excel";
                fileDialog.Filter = "(Excel)*.xlsx|*.xlsx|(Excel)*.xls|*.xls";
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

        private void btnExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
    }
}
