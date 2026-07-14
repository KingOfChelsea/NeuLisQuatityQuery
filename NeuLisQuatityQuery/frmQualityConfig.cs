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
    public partial class frmQualityConfig : Form
    {
        public frmQualityConfig()
        {
            InitializeComponent();
        }
        // 1. 修改你的数据源类型
        public BindingList<NeuLis.Models.ModelQuatity.lisGroup> gridDataList;
        NeuLis.DataBase.QualityOperDB quaOperdb = new NeuLis.DataBase.QualityOperDB();
        NeuLis.DataBase.OperDB operdb = new NeuLis.DataBase.OperDB();
        NeuLis.Models.ModelQuatity.itemType type = new NeuLis.Models.ModelQuatity.itemType();

        private void iniGroup()
        {
            //加载小组编码
            NeuLis.DataBase.OperDB operdb = new NeuLis.DataBase.OperDB();
            List<NeuLis.Models.ModelQuatity.lisGroup> dtGroup = operdb.getLisGroup("ALL");
            //this.gridControl1.DataSource = dtGroup;
            // 2. 初始化数据源
           // gridDataList = new BindingList<NeuLis.Models.ModelQuatity.lisGroup>(dtGroup);
            this.gridControl1.DataSource = dtGroup;
            

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.iniGroup();
        }

        private void lbtSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridView1.CloseEditor();
            this.gridView1.UpdateCurrentRow();
            
            if(xtraTabControl1.SelectedTabPage.Name == "xtraTabPage1")
            {
               
                for (int i = 0; i < this.gridView1.RowCount; i++)
                {
                    var value = gridView1.GetRowCellValue(i, "isstate");
                    if (value != null)
                    {
                        NeuLis.Models.ModelQuatity.lisGroup obj = (NeuLis.Models.ModelQuatity.lisGroup)this.gridView1.GetRow(i);

                        operdb.updateGroup(obj.groupid, value.ToString());
                    }
                }
                this.iniGroup();
            }
            else if(xtraTabControl1.SelectedTabPage.Name == "xtraTabPage2")
            {
 
                string typeid = this.txtTypeID.Text;
                string typeName = this.txtTypeName.Text;
                string preTime = this.txtPre.Text;
                string afterTime = this.txtAfter.Text;
                string emc = this.txtEmc.Text;
                int  i = quaOperdb.saveType(typeid, typeName, preTime, afterTime, emc);
                this.gridControl2.DataSource = quaOperdb.loadItemType();
                this.gridControl2.Refresh();
                this.gridView2.RefreshData();
            }
            else if(xtraTabControl1.SelectedTabPage.Name == "xtraTabPage3")
            {

            }
            
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            this.gridView1.CloseEditor();
            this.gridView1.RefreshData();
        }

        private void frmQualityConfig_Load(object sender, EventArgs e)
        {
            this.gridView5.ShowFindPanel();
            this.iniGroup();
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage1")
            {
                this.iniGroup();
            }
            else if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage2")
            {
 
                this.gridControl2.DataSource = quaOperdb.loadItemType();
                this.gridControl2.Refresh();
                this.gridView2.RefreshData();
            }
            else if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage3")
            {
                this.gridControl3.DataSource = quaOperdb.loadItemType();
                this.gridControl3.Refresh();
                this.gridView3.RefreshData();
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            //查询未维护项目
            string year = this.dateEdit1.Text;
          //  string classtype = this.comboBoxEdit2.Text;
            List<NeuLis.Models.Model.hisitemtype> alhisitemtype = new List<NeuLis.Models.Model.hisitemtype>();
            alhisitemtype = quaOperdb.checkItem(year);
            this.gridControl5.DataSource = alhisitemtype;
            gridView5.IndicatorWidth = 50;
            this.gridView5.RefreshData();
            this.gridView5.BestFitColumns();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (type == null || string.IsNullOrEmpty(type.typeid))
            {
                MessageBox.Show("请先选择一个分类组合！");
                return;
            }
            if (this.gridView5.RowCount > 0)
            {
                for (int i = this.gridView5.RowCount - 1; i >= 0; i--)
                {
                    if (this.gridView5.IsRowSelected(i))
                    {
                       
                        NeuLis.Models.Model.hisitemtype obj = (NeuLis.Models.Model.hisitemtype)this.gridView5.GetRow(i);
                        int k = quaOperdb.saveTypeItem( type.typeid, type.typename, obj.hisitemid, obj.hisitemname);
                        this.gridView5.DeleteRow(i);

                    }
                }
            }
            List<NeuLis.Models.Model.hisitemtype> alList = quaOperdb.getTypeItemList(type.typeid);
            this.gridControl4.DataSource = alList;
            this.gridView4.RefreshData();
            this.gridView4.BestFitColumns();
        }

        private void gridView3_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            type = (NeuLis.Models.ModelQuatity.itemType)this.gridView3.GetRow(e.RowHandle);
            if (type != null)
            {
                List<NeuLis.Models.Model.hisitemtype> alList = quaOperdb.getTypeItemList(type.typeid);
                this.gridControl4.DataSource = alList;
                this.gridView4.RefreshData();
                this.gridView4.BestFitColumns();
            }
        }

        private void btnDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            for (int i = this.gridView4.RowCount - 1; i >= 0; i--)
            {
                if (this.gridView4.IsRowSelected(i))
                {
                    NeuLis.Models.Model.hisitemtype obj = (NeuLis.Models.Model.hisitemtype)this.gridView4.GetRow(i);
                    quaOperdb.delTypeItem(type.typeid, obj.hisitemid);
                    this.gridView4.DeleteRow(i);
                }
            }
        }

        private void lbtExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void gridView5_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView4_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView3_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
    }
}
