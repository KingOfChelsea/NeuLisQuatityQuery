using DevExpress.XtraGrid.Views.Grid.ViewInfo;
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
    public partial class frmSysConfig : Form
    {
        public frmSysConfig()
        {
            InitializeComponent();
        }

        private void lbtnQuery_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NeuLis.DataBase.QualityOperDB operdb = new NeuLis.DataBase.QualityOperDB();
            List<NeuLis.Models.ModelQuatity.sysConfig> alConfig = new List<NeuLis.Models.ModelQuatity.sysConfig>();
            alConfig = operdb.getsysConfig();
            this.gridControl1.DataSource = alConfig;
            this.gridControl1.Refresh();
            this.gridView1.RefreshData();
        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.gridView1.RowCount <= 0)
                return;
            GridHitInfo HitInfo = gridView1.CalcHitInfo(e.Location);//获取鼠标点击的位置
            NeuLis.Models.NeulisDictionary dic = new NeuLis.Models.NeulisDictionary();
            if (HitInfo.InRowCell && HitInfo.Column != null && e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                NeuLis.Models.ModelQuatity.sysConfig obj = (NeuLis.Models.ModelQuatity.sysConfig)this.gridView1.GetFocusedRow();
                this.txtTypeClass.Text = obj.typeclass;
                this.txtSqlIndex.Text = obj.sqlindex;
                this.rctMemo.Text = obj.memo;
                this.rctSQL.Text = obj.sql;
            }
        }
    }
}
