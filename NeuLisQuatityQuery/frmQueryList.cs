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
using System.Threading;
using System.Threading.Tasks;
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
            // 显示进度条和加载提示
            ShowLoading(true);

            // 使用定时器延迟100ms，确保界面先绘制完成
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 100;
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                timer.Dispose();

                // 再次确认进度条还在显示
                ShowLoading(true);

                // 开始加载数据
                LoadDataSync();
            };
            timer.Start();
        }

        private void ShowLoading(bool isLoading)
        {
            if (isLoading)
            {
                this.progressBar1.Visible = true;
                this.progressBar1.Style = ProgressBarStyle.Marquee; // 使用滚动样式
                this.labelLoading.Visible = true;
                this.labelLoading.Text = "正在加载数据，请稍候...";
                this.gridControl1.Visible = false;
                this.Cursor = Cursors.WaitCursor;
                this.Refresh();
                Application.DoEvents();
            }
            else
            {
                this.progressBar1.Visible = false;
                this.labelLoading.Visible = false;
                this.gridControl1.Visible = true;
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 更新加载提示文字
        /// </summary>
        private void UpdateLoadingText(string text)
        {
            if (this.labelLoading.InvokeRequired)
            {
                this.Invoke(new Action(() => this.labelLoading.Text = text));
            }
            else
            {
                this.labelLoading.Text = text;
            }
        }

        private void LoadDataSync()
        {
            try
            {
                UpdateLoadingText("正在查询数据库...");

                NeuLis.DataBase.OperDB operdb = new NeuLis.DataBase.OperDB();

                // 获取数据源（耗时操作在这里执行）
                object dataSource = GetDataSource(operdb);

                UpdateLoadingText("正在绑定数据...");

                // 数据加载完成后，隐藏进度条，显示表格
                ShowLoading(false);

                // 绑定数据
                if (dataSource != null)
                {
                    gridView1.IndicatorWidth = 70;
                    this.gridControl1.DataSource = dataSource;
                    this.gridView1.RefreshData();
                    this.gridView1.BestFitColumns();
                }
            }
            catch (Exception ex)
            {
                ShowLoading(false);

                MessageBox.Show($"数据加载失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private object GetDataSource(NeuLis.DataBase.OperDB operdb)
        {
            // 危急值
            if (kind == "危急值")
            {
                if (typename == "分母：同期需要危急值通报的检验项目总数")
                    return operdb.getLifeAlterList(month);
                if (typename == "已通报的危急值检验项目数")
                    return operdb.getPhoneAlterList(month);
                if (typename == "危急值通报及时数")
                {
                    this.labelControl1.Visible = true;
                    return operdb.GetMonthJSAlterList(month);
                }
            }

            // 检验报告不正确
            if (kind == "检验报告不正确")
            {
                if (typename == "分母：同期检验报告总数")
                    return operdb.getReportList(month);
                return operdb.getErrReportList(month);
            }

            // 血培养
            if (kind == "血培养")
            {
                if (typename == "分母：同期血培养总套数")
                    return operdb.GetMonthXPYList(month);
                return operdb.GetMonthXPYWRList(month);
            }

            // 质控
            if (kind == "质控")
            {
                if (typename == "分母：对室内质控项目变异系数有要求的检验项目总数")
                    return operdb.GetMonthQCStanderList(month);
                return operdb.GetMonthQCoverList(month);
            }

            // 不合格
            if (kind == "不合格")
            {
                if (typename == "标本总数")
                {
                    if (typeid != null && typeid.IndexOf("抗凝标本") >= 0)
                        return operdb.getKNBarcodeList(month);
                    return operdb.getBarcodeList(month);
                }
                return operdb.getRejectSampleList(typename, month);
            }

            // TAT
            if (kind == "TAT")
            {
                string pt = patienttype == "加急" ? "1" : patienttype;
                if (typeclass == "全部")
                    return operdb.GetAroundAllList(month, pt);
                return operdb.GetAroundClassList(month, pt, typeclass);
            }

            if (kind == "总拒收率")
            {
                // 这里调用你写好的查询明细方法
                return operdb.GetTotalRejectDetail(month);
            }

            return null;
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
