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
    public partial class frmItemConfig : Form
    {
        public frmItemConfig()
        {
            InitializeComponent();
        }
        private NeuLis.Models.Model.typeclass type = new NeuLis.Models.Model.typeclass();
        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(this.xtraTabControl1.SelectedTabPageIndex==0)
            {

            }
            else if(this.xtraTabControl1.SelectedTabPageIndex==1)
            {
                string typeID = this.txtTypeID.Text;
                string typeName = this.txtTypeName.Text;
                string classtype = this.comboBoxEdit1.Text;
                NeuLis.DataBase.OperDB operdb = new NeuLis.DataBase.OperDB();
                int i  = operdb.InsertItemType(typeID,typeName, classtype);
                if(i<0)
                {
                    if(MessageBox.Show($"{typeName}【{typeID}】数据已经存在，是否更新数据？","提示",MessageBoxButtons.YesNo)==DialogResult.Yes)
                    {
                        operdb.UpdateItemType(typeID, typeName);
                    }
                }
                this.gridControl4.DataSource = null;
                List<NeuLis.Models.Model.typeclass> alTypeList =  NeuLis.DataBase.OperDB.getTypeList(classtype);
                this.gridControl4.DataSource = alTypeList;
                this.gridView4.RefreshData();
                this.gridView4.BestFitColumns();
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {

            if (this.xtraTabControl1.SelectedTabPageIndex == 0)
            {
                string classtype = this.comboBoxEdit2.Text;
                List<NeuLis.Models.Model.typeclass> alTypeList = NeuLis.DataBase.OperDB.getTypeList(classtype);
                this.gridControl1.DataSource = alTypeList;
                this.gridView1.RefreshData();
                this.gridView1.BestFitColumns();
            }
            else if (this.xtraTabControl1.SelectedTabPageIndex == 1)
            {
                string classtype = this.comboBoxEdit1.Text;
                List<NeuLis.Models.Model.typeclass> alTypeList = NeuLis.DataBase.OperDB.getTypeList(classtype);
                this.gridControl4.DataSource = alTypeList;
                this.gridView4.RefreshData();
                this.gridView4.BestFitColumns();
            }
        }

        private void frmItemConfig_Load(object sender, EventArgs e)
        {
            //List<NeuLis.Models.Model.typeclass> alTypeList = NeuLis.DataBase.OperDB.getTypeList();
            //this.gridControl1.DataSource = alTypeList;
            //this.gridView1.RefreshData();
            //this.gridView1.BestFitColumns();

            ////加载未维护对照项目
            //List< NeuLis.Models.Model.hisitemtype> alhisitemtype = new List<NeuLis.Models.Model.hisitemtype>();
            //alhisitemtype = NeuLis.DataBase.OperDB.checkItem();
            //this.gridControl3.DataSource = alhisitemtype;
            //this.gridView3.RefreshData();
            //this.gridView3.BestFitColumns();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnQuer_Click(object sender, EventArgs e)
        {
            //查询未维护项目
            string year = this.dateEdit1.Text;
            string  classtype = this.comboBoxEdit2.Text;
            List<NeuLis.Models.Model.hisitemtype> alhisitemtype = new List<NeuLis.Models.Model.hisitemtype>();
            alhisitemtype = NeuLis.DataBase.OperDB.checkItem(year, classtype);
            this.gridControl3.DataSource = alhisitemtype;
            this.gridView3.RefreshData();
            this.gridView3.BestFitColumns();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(type==null || string.IsNullOrEmpty(type.typeID))
            {
                MessageBox.Show("请先选择一个分类组合！");
                return;
            }
            if(this.gridView3.RowCount>0)
            {
                for(int i= this.gridView3.RowCount -1; i>=0;i--)
                {
                    if (this.gridView3.IsRowSelected(i))
                    {
                        string classtype = this.comboBoxEdit2.Text;
                        NeuLis.Models.Model.hisitemtype obj = (NeuLis.Models.Model.hisitemtype)this.gridView3.GetRow(i);
                        int k = NeuLis.DataBase.OperDB.insertItemType(obj.hisitemid, obj.hisitemname, type.typeID, type.typeName, classtype);
                        this.gridView3.DeleteRow(i);

                    }
                }
            }
            List<NeuLis.Models.Model.hisitemtype> alList = NeuLis.DataBase.OperDB.hisItemList(type.typeID);
            this.gridControl2.DataSource = alList;
            this.gridView2.RefreshData();
            this.gridView2.BestFitColumns();
            //// 获取双击命中的数据
            //var hisItemView = (sender as DevExpress.XtraGrid.Views.Grid.GridView);
            //var hitInfo = (hisItemView.CalcHitInfo((e as MouseEventArgs).Location));
            //// 判断是否命中数据
            //if (!hitInfo.InRow)
            //{
            //    // 未命中数据行
            //    return;
            //}
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            type = (NeuLis.Models.Model.typeclass)this.gridView1.GetRow(e.RowHandle);
            if(type !=null)
            {
                List<NeuLis.Models.Model.hisitemtype> alList = NeuLis.DataBase.OperDB.hisItemList(type.typeID);
                this.gridControl2.DataSource = alList;
                this.gridView2.RefreshData();
                this.gridView2.BestFitColumns();
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void comboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string classtype = this.comboBoxEdit2.Text;
            List<NeuLis.Models.Model.typeclass> alTypeList = NeuLis.DataBase.OperDB.getTypeList(classtype);
            this.gridControl1.DataSource = alTypeList;
            this.gridView1.RefreshData();
            this.gridView1.BestFitColumns();

            //加载未维护对照项目
            string year = this.dateEdit1.Text;
            List<NeuLis.Models.Model.hisitemtype> alhisitemtype = new List<NeuLis.Models.Model.hisitemtype>();
            alhisitemtype = NeuLis.DataBase.OperDB.checkItem(year, classtype);
            this.gridControl3.DataSource = alhisitemtype;
            this.gridView3.RefreshData();
            this.gridView3.BestFitColumns();
        }

        private void comboBoxEdit1_SelectedValueChanged(object sender, EventArgs e)
        {
            string classtype = this.comboBoxEdit1.Text;
            List<NeuLis.Models.Model.typeclass> alTypeList = NeuLis.DataBase.OperDB.getTypeList(classtype);
            this.gridControl4.DataSource = alTypeList;
            this.gridView4.RefreshData();
            this.gridView4.BestFitColumns();
        }
    }
}
