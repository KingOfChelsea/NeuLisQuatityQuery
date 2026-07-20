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
        // 分页相关变量
        private DataTable _allData;
        private int _currentPage = 1;
        private int _pageSize = 50;
        private int _totalPages = 1;
        private int _totalRecords = 0;

        public frmQueryList()
        {
            InitializeComponent();
            InitializePagination();
        }

        public string kind;
        public string typeid;
        public string typename;
        public string month;
        public string patienttype;
        public string typeclass;

        /// <summary>
        /// 初始化分页控件
        /// </summary>
        private void InitializePagination()
        {
            cmbPageSize.SelectedIndex = 1; // 默认50条/页
            UpdatePaginationControls();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLoading"></param>
        private void ShowLoading(bool isLoading)
        {
            if (isLoading)
            {
                this.progressBar1.Visible = true;
                this.progressBar1.Style = ProgressBarStyle.Marquee;
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
        /// 更新加载提示文字 Created By 徐振宇 2026年7月14日20:41:21
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
                DataTable dataSource = GetDataSource(operdb);

                UpdateLoadingText("正在绑定数据...");

                // 保存全部数据用于分页
                _allData = dataSource;

                // 设置分页信息
                _totalRecords = _allData?.Rows.Count ?? 0;
                _totalPages = _totalRecords > 0 ?
                    (int)Math.Ceiling((double)_totalRecords / _pageSize) : 1;
                _currentPage = 1;

                // 数据加载完成后，隐藏进度条，显示表格
                ShowLoading(false);

                // 加载第一页数据
                LoadPageData();
                UpdatePaginationControls();
            }
            catch (Exception ex)
            {
                ShowLoading(false);

                MessageBox.Show($"数据加载失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 加载当前页数据到Grid Created By 徐振宇 2026年7月14日20:41:21
        /// </summary>
        private void LoadPageData()
        {
            if (_allData == null || _allData.Rows.Count == 0)
            {
                gridControl1.DataSource = null;
                return;
            }

            // 创建当前页的DataTable
            DataTable pageData = _allData.Clone();

            int startIndex = (_currentPage - 1) * _pageSize;
            int endIndex = Math.Min(startIndex + _pageSize, _totalRecords);

            for (int i = startIndex; i < endIndex; i++)
            {
                pageData.ImportRow(_allData.Rows[i]);
            }

            gridView1.IndicatorWidth = 70;
            this.gridControl1.DataSource = pageData;
            this.gridView1.RefreshData();
            this.gridView1.BestFitColumns();
        }

        /// <summary>
        /// 更新分页控件状态 Created By 徐振宇 2026年7月14日20:41:21
        /// </summary>
        private void UpdatePaginationControls()
        {
            lblPageInfo.Text = $"第{_currentPage}页/共{_totalPages}页";
            lblPageSize.Text = $"{_pageSize}条/页\n共{_totalRecords}条";
            numericUpDownPage.Maximum = _totalPages;
            numericUpDownPage.Value = _currentPage;

            btnFirst.Enabled = _currentPage > 1;
            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
            btnLast.Enabled = _currentPage < _totalPages;
        }
        /// <summary>
        /// 根据传入的值获取数据源 Add By 徐振宇 2026年7月14日20:41:05
        /// </summary>
        /// <param name="operdb"></param>
        /// <returns></returns>
        private DataTable GetDataSource(NeuLis.DataBase.OperDB operdb)
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
                    return ConvertToDataTable(operdb.getReportList(month)) ;
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
                return ConvertToDataTable(operdb.getRejectSampleList(typename, month));
            }

            // TAT
            if (kind == "TAT")
            {
                string pt = patienttype == "加急" ? "1" : patienttype;
                if (typeclass == "全部")
                    return operdb.GetAroundAllList(month, pt);
                return operdb.GetAroundClassList(month, pt, typeclass);
            }

            // 总拒收率 - 修复类型转换错误
            if (kind == "总拒收率")
            {
                List<NeuLis.Models.Model.sampleReject> rejectList = operdb.GetTotalRejectDetail(month);
                return ConvertToDataTable(rejectList);
            }

            // TAT_P90 2026年7月14日18:43:37
            if (kind == "TAT_P90")
            {
                return operdb.GetTATP90Detail(month);
            }
            //  危急值报告时间 Added By 徐振宇 2026年7月15日11:43:22
            if (kind == "危急值报告时间中位数")
            {
                return operdb.GetCrisisReportTimeDetail(month, patienttype);
            }

            return null;
        }

        private void gridView1_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            // 保留原有逻辑
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                // 显示全局行号（考虑分页偏移）
                int globalRowIndex = (_currentPage - 1) * _pageSize + e.RowHandle + 1;
                e.Info.DisplayText = globalRowIndex.ToString();
            }
        }

        private void btnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // 导出全部数据（不是当前页）
            ExportAllData();
        }

        /// <summary>
        /// 导出全部数据 Add By 徐振宇 2026年7月14日20:43:51
        /// </summary>
        private void ExportAllData()
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
                    // 临时切换到全部数据导出
                    DataTable originalData = (DataTable)gridControl1.DataSource;
                    gridControl1.DataSource = _allData;
                    gridControl1.Refresh();

                    if (fileDialog.FilterIndex == 2)
                        gridControl1.ExportToXls(fileDialog.FileName);
                    else
                    {
                        gridControl1.ExportToXlsx(fileDialog.FileName);
                    }

                    // 恢复当前页数据
                    gridControl1.DataSource = originalData;
                    gridControl1.Refresh();

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

        #region 分页按钮事件

        private void btnFirst_Click(object sender, EventArgs e)
        {
            _currentPage = 1;
            LoadPageData();
            UpdatePaginationControls();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadPageData();
                UpdatePaginationControls();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                LoadPageData();
                UpdatePaginationControls();
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            _currentPage = _totalPages;
            LoadPageData();
            UpdatePaginationControls();
        }

        private void numericUpDownPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                int page = (int)numericUpDownPage.Value;
                if (page >= 1 && page <= _totalPages)
                {
                    _currentPage = page;
                    LoadPageData();
                    UpdatePaginationControls();
                }
            }
        }

        private void cmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            _pageSize = int.Parse(cmbPageSize.SelectedItem.ToString());

            _totalPages = _totalRecords > 0 ?
                (int)Math.Ceiling((double)_totalRecords / _pageSize) : 1;

            _currentPage = 1;
            LoadPageData();
            UpdatePaginationControls();
        }

        /// <summary>
        /// 将泛型List转换为DataTable Created By 徐振宇 2026年7月14日20:44:24
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="list">数据列表</param>
        /// <returns>转换后的DataTable</returns>
        private DataTable ConvertToDataTable<T>(List<T> list)
        {
            DataTable dt = new DataTable();

            if (list == null || list.Count == 0)
                return dt;

            // 获取所有属性
            var properties = typeof(T).GetProperties();

            // 创建列
            foreach (var prop in properties)
            {
                dt.Columns.Add(prop.Name, prop.PropertyType);
            }

            // 添加数据行
            foreach (var item in list)
            {
                DataRow row = dt.NewRow();
                foreach (var prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item, null) ?? DBNull.Value;
                }
                dt.Rows.Add(row);
            }

            return dt;
        }

        #endregion
    }
}