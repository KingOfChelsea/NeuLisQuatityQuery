using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace NeuLisQuatityQuery.Utils
{
    public  class GridHelper
    {
        #region 字段
        private GridControl _gridControl;
        private GridView _view;
        private ContextMenuStrip _contextMenu;
        private bool _useEmbeddedNavigator;
        private bool _useContextMenu;
        private bool _useFindPanel;
        private bool _useAutoFilterRow;
        private bool _useGroupPanel;
        private bool _useFooter;
        private bool _useRowIndicator;
        private bool _useAlternatingRowColors;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gridControl">要配置的GridControl控件</param>
        /// <param name="useEmbeddedNavigator">是否启用底部导航器</param>
        /// <param name="useContextMenu">是否启用右键菜单</param>
        /// <param name="useFindPanel">是否启用搜索面板</param>
        /// <param name="useAutoFilterRow">是否启用自动筛选行</param>
        /// <param name="useGroupPanel">是否启用分组面板</param>
        /// <param name="useFooter">是否启用统计栏</param>
        /// <param name="useRowIndicator">是否显示行号</param>
        /// <param name="useAlternatingRowColors">是否启用交替行颜色</param>
        public GridHelper(
            GridControl gridControl,
            bool useEmbeddedNavigator = true,
            bool useContextMenu = true,
            bool useFindPanel = true,
            bool useAutoFilterRow = true,
            bool useGroupPanel = true,
            bool useFooter = true,
            bool useRowIndicator = true,
            bool useAlternatingRowColors = true)
        {
            _gridControl = gridControl;
            _view = gridControl.MainView as GridView;
            if (_view == null)
                throw new ArgumentException("GridControl 的 MainView 必须是 GridView");

            _useEmbeddedNavigator = useEmbeddedNavigator;
            _useContextMenu = useContextMenu;
            _useFindPanel = useFindPanel;
            _useAutoFilterRow = useAutoFilterRow;
            _useGroupPanel = useGroupPanel;
            _useFooter = useFooter;
            _useRowIndicator = useRowIndicator;
            _useAlternatingRowColors = useAlternatingRowColors;
        }
        #endregion

        #region 公共方法

        /// <summary>
        /// 设置列标题（字典方式批量设置）
        /// </summary>
        /// <param name="columnCaptions">字段名与标题的映射字典</param>
        public void SetColumnCaptions(Dictionary<string, string> columnCaptions)
        {
            foreach (var kvp in columnCaptions)
            {
                // 检查列是否存在
                if (_view.Columns[kvp.Key] != null)
                {
                    _view.Columns[kvp.Key].Caption = kvp.Value;
                }
                else
                {
                    // 列不存在时输出调试信息
                    System.Diagnostics.Debug.WriteLine($"列 '{kvp.Key}' 不存在，跳过设置标题");
                }
            }
        }

        /// <summary>
        /// 设置列标题（单个设置）
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="caption">标题</param>
        public void SetColumnCaption(string fieldName, string caption)
        {
            if (_view.Columns[fieldName] != null)
                _view.Columns[fieldName].Caption = caption;
        }

        /// <summary>
        /// 设置列宽
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="width">宽度</param>
        public void SetColumnWidth(string fieldName, int width)
        {
            if (_view.Columns[fieldName] != null)
                _view.Columns[fieldName].Width = width;
        }

        /// <summary>
        /// 设置列可见性
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="visible">是否可见</param>
        public void SetColumnVisible(string fieldName, bool visible)
        {
            if (_view.Columns[fieldName] != null)
                _view.Columns[fieldName].Visible = visible;
        }

        /// <summary>
        /// 添加统计项
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="summaryType">统计类型（Count, Sum, Average, Max, Min等）</param>
        /// <param name="displayFormat">显示格式，如 "共 {0} 条"</param>
        public void AddSummary(string fieldName, DevExpress.Data.SummaryItemType summaryType, string displayFormat)
        {
            if (_view.Columns[fieldName] == null) return;

            // 先清除该列已有的统计项，避免重复
            _view.Columns[fieldName].Summary.Clear();
            _view.Columns[fieldName].Summary.Add(summaryType, fieldName, displayFormat);
        }

        /// <summary>
        /// 添加多个统计项
        /// </summary>
        /// <param name="summaries">统计项配置列表</param>
        public void AddSummaries(List<SummaryConfig> summaries)
        {
            foreach (var summary in summaries)
            {
                AddSummary(summary.FieldName, summary.SummaryType, summary.DisplayFormat);
            }
        }

        /// <summary>
        /// 设置行单元格样式事件
        /// </summary>
        /// <param name="handler">行单元格样式事件处理器</param>
        public void SetRowCellStyleHandler(RowCellStyleEventHandler handler)
        {
            _view.RowCellStyle -= handler;
            _view.RowCellStyle += handler;
        }

        /// <summary>
        /// 自动调整所有列宽
        /// </summary>
        public void BestFitColumns()
        {
            _view.BestFitColumns();
        }

        /// <summary>
        /// 应用所有配置（在设置完所有属性后调用）
        /// </summary>
        public void Apply()
        {
            // 搜索面板
            if (_useFindPanel)
            {
                _view.OptionsFind.AlwaysVisible = true;
                _view.OptionsFind.ClearFindOnClose = true;
                _view.OptionsFind.FindDelay = 500;
                _view.OptionsFind.ShowClearButton = true;
                _view.OptionsFind.ShowCloseButton = true;
                _view.OptionsFind.ShowFindButton = true;
            }

            // 自动筛选行
            _view.OptionsView.ShowAutoFilterRow = _useAutoFilterRow;

            // 分组面板
            _view.OptionsView.ShowGroupPanel = _useGroupPanel;
            if (_useGroupPanel)
                _view.OptionsBehavior.AutoExpandAllGroups = true;

            // 统计栏
            _view.OptionsView.ShowFooter = _useFooter;

            // 行号
            _view.OptionsView.ShowIndicator = _useRowIndicator;

            // 交替行颜色
            if (_useAlternatingRowColors)
            {
                _view.OptionsView.EnableAppearanceEvenRow = true;
                _view.OptionsView.EnableAppearanceOddRow = true;
                _view.Appearance.EvenRow.BackColor = Color.FromArgb(240, 240, 240);
                _view.Appearance.OddRow.BackColor = Color.White;
            }

            // 底部导航器
            if (_useEmbeddedNavigator)
            {
                _gridControl.UseEmbeddedNavigator = true;
                _gridControl.EmbeddedNavigator.TextStringFormat = "第 {0} 条 / 共 {1} 条";

                // 隐藏编辑按钮
                _gridControl.EmbeddedNavigator.Buttons.Append.Visible = false;
                _gridControl.EmbeddedNavigator.Buttons.Edit.Visible = false;
                _gridControl.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
                _gridControl.EmbeddedNavigator.Buttons.Remove.Visible = false;
                _gridControl.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            }

            // 右键菜单
            if (_useContextMenu)
            {
                CreateContextMenu();
            }
        }

        /// <summary>
        /// 添加自定义导航按钮（图标按钮）
        /// </summary>
        /// <param name="imageIndex">图标索引</param>
        /// <param name="hint">提示文字</param>
        /// <param name="tag">标识</param>
        public void AddCustomNavigatorButton(int imageIndex, string hint, string tag)
        {
            if (!_useEmbeddedNavigator) return;

            var button = _gridControl.EmbeddedNavigator.Buttons.CustomButtons.Add();
            button.ImageIndex = imageIndex;
            button.Hint = hint;
            button.Tag = tag;
        }

        /// <summary>
        /// 订阅导航器按钮点击事件
        /// </summary>
        /// <param name="handler">事件处理器</param>
        public void SetNavigatorButtonClickHandler(NavigatorButtonClickEventHandler handler)
        {
            if (!_useEmbeddedNavigator) return;

            _gridControl.EmbeddedNavigator.ButtonClick -= handler;
            _gridControl.EmbeddedNavigator.ButtonClick += handler;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 创建右键菜单
        /// </summary>
        private void CreateContextMenu()
        {
            _contextMenu = new ContextMenuStrip();

            // 刷新
            ToolStripMenuItem miRefresh = new ToolStripMenuItem("刷新");
            miRefresh.Click += (s, e) => OnRefreshClick();

            // 导出Excel
            ToolStripMenuItem miExport = new ToolStripMenuItem("导出Excel");
            miExport.Click += (s, e) => OnExportClick();

            // 打印预览
            ToolStripMenuItem miPrint = new ToolStripMenuItem("打印预览");
            miPrint.Click += (s, e) => OnPrintClick();

            // 复制选中行
            ToolStripMenuItem miCopy = new ToolStripMenuItem("复制选中行");
            miCopy.Click += (s, e) => OnCopyClick();

            // 全选
            ToolStripMenuItem miSelectAll = new ToolStripMenuItem("全选");
            miSelectAll.Click += (s, e) => OnSelectAllClick();

            _contextMenu.Items.AddRange(new ToolStripItem[] {
                miRefresh, miExport, miPrint,
                new ToolStripSeparator(),
                miCopy, miSelectAll
            });

            _gridControl.ContextMenuStrip = _contextMenu;
        }

        #endregion

        #region 事件触发（可重写）

        /// <summary>
        /// 刷新点击事件（可重写）
        /// </summary>
        protected virtual void OnRefreshClick()
        {
            // 子类重写或通过事件订阅
            RefreshClicked?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 导出点击事件（可重写）
        /// </summary>
        protected virtual void OnExportClick()
        {
            ExportClicked?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 打印点击事件（可重写）
        /// </summary>
        protected virtual void OnPrintClick()
        {
            PrintClicked?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 复制点击事件（可重写）
        /// </summary>
        protected virtual void OnCopyClick()
        {
            _view.CopyToClipboard();
        }

        /// <summary>
        /// 全选点击事件（可重写）
        /// </summary>
        protected virtual void OnSelectAllClick()
        {
            _view.SelectAll();
        }

        #endregion

        #region 事件

        /// <summary>
        /// 刷新按钮点击事件
        /// </summary>
        public event EventHandler RefreshClicked;

        /// <summary>
        /// 导出按钮点击事件
        /// </summary>
        public event EventHandler ExportClicked;

        /// <summary>
        /// 打印按钮点击事件
        /// </summary>
        public event EventHandler PrintClicked;

        #endregion
    }

    /// <summary>
    /// 统计项配置类
    /// </summary>
    public class SummaryConfig
    {
        public string FieldName { get; set; }
        public DevExpress.Data.SummaryItemType SummaryType { get; set; }
        public string DisplayFormat { get; set; }

        public SummaryConfig(string fieldName, DevExpress.Data.SummaryItemType summaryType, string displayFormat)
        {
            FieldName = fieldName;
            SummaryType = summaryType;
            DisplayFormat = displayFormat;
        }
    }
}
