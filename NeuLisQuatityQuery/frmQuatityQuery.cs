using DevExpress.XtraEditors;
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
    public partial class frmQuatityQuery : Form
    {
        public frmQuatityQuery()
        {
            InitializeComponent();
        }
        private void iniGroup()
        {
            //加载小组编码
            NeuLis.DataBase.OperDB operdb = new NeuLis.DataBase.OperDB();
            List<NeuLis.Models.ModelQuatity.lisGroup> dtGroup = operdb.getLisGroup("1");
            this.gridControl2.DataSource = dtGroup;
           // this.groupid.FieldName = "groupid";
            //this.groupname.FieldName = "groupname";
        }

        private void frmQuatityQuery_Load(object sender, EventArgs e)
        {
            this.iniGroup();
            this.dateEdit1.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            this.dateEdit2.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void lbtQuery_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string groupid = "";
            Int32 badSapSum = 0;//不合格标本总数
            string begDate = this.dateEdit1.Text.Replace("-","");
            string endDate = this.dateEdit2.Text.Replace("-", "");
            NeuLis.DataBase.OperDB operdb = new NeuLis.DataBase.OperDB();
            foreach (int a in this.gridView2.GetSelectedRows())
            {
                groupid += "'" + this.gridView2.GetRowCellValue(a, "groupid") + "',";
            }
            if (groupid.Length < 0)
                return;
            groupid = groupid.Substring(0, groupid.Length - 1);
            if (this.xtraTabControl1.SelectedTabPage.Name== "xtraTabPage1")
            {
                List<NeuLis.Models.ModelQuatity.quatitydata> qualityData = new List<NeuLis.Models.ModelQuatity.quatitydata>();
                if (this.gridView2.RowCount > 0)
                {
                   
                    //查询不合格标本数量
                    List<NeuLis.Models.ModelQuatity.badSampleType> listFZ = new List<NeuLis.Models.ModelQuatity.badSampleType>();
                    listFZ = operdb.getListBadSampleList(begDate, endDate, groupid);
                    //查询标本总数
                    List<NeuLis.Models.ModelQuatity.badSampleType> listFM = new List<NeuLis.Models.ModelQuatity.badSampleType>();
                    listFM = operdb.getListTotalSampleList(begDate, endDate, groupid);
                    NeuLis.Models.ModelQuatity.badSampleType objFM = listFM[0] as NeuLis.Models.ModelQuatity.badSampleType;
                    //抗凝标本总数
                    List<NeuLis.Models.ModelQuatity.badSampleType> listKN = new List<NeuLis.Models.ModelQuatity.badSampleType>();
                    listKN = operdb.getListKNTotalSampleList(begDate, endDate, groupid);
                    NeuLis.Models.ModelQuatity.badSampleType objKNFM = listKN[0] as NeuLis.Models.ModelQuatity.badSampleType;
                    NeuLis.Models.ModelQuatity.quatitydata obj;
                    //不合格标本分类统计
                    foreach (NeuLis.Models.ModelQuatity.badSampleType fz in listFZ)
                    {

                        obj = new NeuLis.Models.ModelQuatity.quatitydata();
                        obj.typereason = fz.typereason;
                        if (fz.typereason.IndexOf("凝集") >= 0)
                        {
                            obj.typeClass = "不合格标本";
                            obj.typenum = string.IsNullOrEmpty(fz.typenum) ? "0" : fz.typenum;
                            obj.totalnum = objKNFM.typenum;
                            obj.typerate = (Math.Round(Convert.ToDouble(obj.typenum) / Convert.ToDouble(obj.totalnum) * 100, 2) + "%").ToString();
                            obj.typememo = "计算公式：" + obj.typereason + "/" + objFM.typereason;
                        }
                        else
                        {
                            obj.typeClass = "不合格标本";
                            obj.typenum = string.IsNullOrEmpty(fz.typenum) ? "0" : fz.typenum;
                            obj.totalnum = objFM.typenum;
                            obj.typerate = (Math.Round(Convert.ToDouble(obj.typenum) / Convert.ToDouble(obj.totalnum) * 100, 2) + "%").ToString();
                            obj.typememo = "计算公式：" + obj.typereason + "/" + objFM.typereason;
                        }
                        badSapSum = badSapSum + Convert.ToInt32(fz.typenum);
                        qualityData.Add(obj);
                    }
                    //标本拒收率
                    obj = new NeuLis.Models.ModelQuatity.quatitydata();
                    obj.typeClass = "不合格标本";
                    obj.typereason = "标本拒收率";
                    obj.typenum = badSapSum.ToString();
                    obj.totalnum = objFM.typenum;
                    obj.typerate = (Math.Round(Convert.ToDouble(obj.typenum) / Convert.ToDouble(obj.totalnum) * 100, 2) + "%").ToString();
                    obj.typememo = "计算公式：不合格标本总数/" + objFM.typereason;
                    qualityData.Add(obj);
                    //查询血培养污染标本数量
                    List<NeuLis.Models.ModelQuatity.badSampleType> listxpywr = new List<NeuLis.Models.ModelQuatity.badSampleType>();
                    listxpywr = operdb.getListPolluteSampleList(begDate, endDate, groupid);
                    //查询血培养标本总数
                    List<NeuLis.Models.ModelQuatity.badSampleType> listxpy = new List<NeuLis.Models.ModelQuatity.badSampleType>();
                    listxpy = operdb.getListTotalBloodSampleList(begDate, endDate, groupid);
                    obj = new NeuLis.Models.ModelQuatity.quatitydata();
                    obj.typeClass = "血培养";
                    obj.typereason = "血培养污染率";
                    obj.typenum = string.IsNullOrEmpty(listxpywr[0].typenum) ? "0" : listxpywr[0].typenum;
                    obj.totalnum = listxpy[0].typenum;
                    obj.typerate = (Math.Round(Convert.ToDouble(obj.typenum) / Convert.ToDouble(obj.totalnum) * 100, 2) + "%").ToString();
                    obj.typememo = "计算公式：" + listxpywr[0].typereason + "/" + listxpy[0].typereason;
                    qualityData.Add(obj);

                    //质控项目开展率
                    List<NeuLis.Models.ModelQuatity.badSampleType> listQCT = new List<NeuLis.Models.ModelQuatity.badSampleType>();
                    List<NeuLis.Models.ModelQuatity.badSampleType> listQCN = new List<NeuLis.Models.ModelQuatity.badSampleType>();
                    obj = new NeuLis.Models.ModelQuatity.quatitydata();
                    listQCT = operdb.getListTotalQCItemCount(begDate, endDate, groupid);
                    listQCN = operdb.getListQCItemCount(begDate, endDate, groupid);
                    obj.typeClass = "质控";
                    obj.typereason = "室内质控开展率";
                    obj.typenum = listQCN[0].typenum;
                    obj.totalnum = listQCT[0].typenum;
                    obj.typerate = (Math.Round(Convert.ToDouble(obj.typenum) / Convert.ToDouble(obj.totalnum) * 100, 2) + "%").ToString();
                    obj.typememo = "计算公式：" + listQCN[0].typereason + "/" + listQCT[0].typereason;
                    qualityData.Add(obj);

                    //质控变异系数不合格率
                    List<NeuLis.Models.ModelQuatity.badSampleType> listBadN = new List<NeuLis.Models.ModelQuatity.badSampleType>();
                    listBadN = operdb.getQCBadCount(begDate, endDate, groupid);
                    obj = new NeuLis.Models.ModelQuatity.quatitydata();
                    obj.typeClass = "质控变异系数";
                    obj.typereason = "室内质控项目变异系数不合格率";
                    obj.typenum = listBadN[0].typenum;
                    obj.totalnum = "190";
                    obj.typerate = (Math.Round(Convert.ToDouble(obj.typenum) / Convert.ToDouble(obj.totalnum) * 100, 2) + "%").ToString();
                    obj.typememo = "计算公式：" + listBadN[0].typereason + "/对室内质控项目有要求的项目数(190)";
                    qualityData.Add(obj);

                    //检验报告不正确率
                    List<NeuLis.Models.ModelQuatity.badSampleType> listBadTestForm = new List<NeuLis.Models.ModelQuatity.badSampleType>();
                    List<NeuLis.Models.ModelQuatity.badSampleType> listTestForm = new List<NeuLis.Models.ModelQuatity.badSampleType>();
                    listBadTestForm = operdb.getCancleTestFormNum(begDate, endDate, groupid);
                    listTestForm = operdb.getTestFormNum(begDate, endDate, groupid);
                    obj = new NeuLis.Models.ModelQuatity.quatitydata();
                    obj.typeClass = "检验报告";
                    obj.typereason = "检验报告不正确率";
                    obj.typenum = listBadTestForm[0].typenum;
                    obj.totalnum = listTestForm[0].typenum;
                    obj.typerate = (Math.Round(Convert.ToDouble(obj.typenum) / Convert.ToDouble(obj.totalnum) * 100, 2) + "%").ToString();
                    obj.typememo = "计算公式：" + listBadTestForm[0].typereason + "/" + listTestForm[0].typereason;
                    qualityData.Add(obj);

                    //危急值通报率
                    List<NeuLis.Models.ModelQuatity.badSampleType> listAlterSap = new List<NeuLis.Models.ModelQuatity.badSampleType>();
                    // List<NeuLis.Models.ModelQuatity.badSampleType> listTestForm = new List<NeuLis.Models.ModelQuatity.badSampleType>();
                    listAlterSap = operdb.getLifeAlterNum(begDate, endDate, groupid);

                    obj = new NeuLis.Models.ModelQuatity.quatitydata();
                    obj.typeClass = "危急值";
                    obj.typereason = "危急值通报率";
                    obj.typenum = listAlterSap[0].typenum;
                    obj.totalnum = listAlterSap[0].typenum;
                    obj.typerate = (Math.Round(Convert.ToDouble(obj.typenum) / Convert.ToDouble(obj.totalnum) * 100, 2) + "%").ToString();
                    obj.typememo = "计算公式：" + listAlterSap[0].typereason + "/" + listAlterSap[0].typereason;
                    qualityData.Add(obj);

                    //危急值通报及时率
                    List<NeuLis.Models.ModelQuatity.badSampleType> listAlter = new List<NeuLis.Models.ModelQuatity.badSampleType>();
                    listAlter = operdb.getLifeAlterNotOverTimeNum(begDate, endDate, groupid);

                    obj = new NeuLis.Models.ModelQuatity.quatitydata();
                    obj.typeClass = "危急值";
                    obj.typereason = "危急值通报及时率";
                    obj.typenum = listAlter[0].typenum;
                    obj.totalnum = listAlterSap[0].typenum;
                    obj.typerate = (Math.Round(Convert.ToDouble(obj.typenum) / Convert.ToDouble(obj.totalnum) * 100, 2) + "%").ToString();
                    obj.typememo = "计算公式：" + listAlter[0].typereason + "/" + listAlterSap[0].typereason;
                    qualityData.Add(obj);

                    this.gridControl1.DataSource = qualityData;
                    this.gridView1.BestFitColumns();
                }
            }
            else if(this.xtraTabControl1.SelectedTabPage.Name == "xtraTabPage2")
            {
                NeuLis.DataBase.QualityOperDB quaDB = new NeuLis.DataBase.QualityOperDB();
                List<NeuLis.Models.ModelQuatity.tatItem> alitem = quaDB.getItemTAT(begDate, endDate, groupid);
                this.gridControl3.DataSource = alitem;
                this.gridControl3.Refresh();
                this.bandedGridView1.RefreshData();
            }
            else if (this.xtraTabControl1.SelectedTabPage.Name == "xtraTabPage3")
            {
                NeuLis.DataBase.QualityOperDB quaDB = new NeuLis.DataBase.QualityOperDB();
               // List<NeuLis.Models.ModelQuatity.quatitydata> alNoSampleTime = new List<NeuLis.Models.ModelQuatity.quatitydata>();
                DataTable alNoSampleTime = quaDB.getListNoSampletimeRate(begDate,endDate,groupid);
                this.gridControl4.DataSource = alNoSampleTime;
                this.gridControl4.Refresh();
                this.gridView3.RefreshData();
                gridView3.IndicatorWidth = 70;
            }
        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.gridView1.RowCount <= 0)
                return;
            GridHitInfo HitInfo = gridView1.CalcHitInfo(e.Location);//获取鼠标点击的位置
            NeuLis.Models.NeulisDictionary dic = new NeuLis.Models.NeulisDictionary();
            if (HitInfo.InRowCell && HitInfo.Column != null && e.Button == MouseButtons.Left && e.Clicks == 2)
            {
                NeuLis.Models.ModelQuatity.quatitydata obj = (NeuLis.Models.ModelQuatity.quatitydata)this.gridView1.GetFocusedRow();
                string typeclass = obj.typeClass;//HitInfo.Column.FieldName;
                string typeid = obj.typereason;
                string typename = HitInfo.Column.FieldName;
                string begDate = this.dateEdit1.Text.Replace("-", "");
                string endDate = this.dateEdit2.Text.Replace("-", "");
                string groupid = "";
                foreach (int a in this.gridView2.GetSelectedRows())
                {
                    groupid += "'" + this.gridView2.GetRowCellValue(a, "groupid") + "',";
                }
                if (groupid.Length < 0)
                    return;
                groupid = groupid.Substring(0, groupid.Length - 1);
                frmQualityList frm = new frmQualityList();
                frm.typeclass = typeclass;
                frm.typeid = typeid;
                frm.typename = typename;
                frm.begDate = begDate;
                frm.endDate = endDate;
                frm.groupid = groupid;
                frm.ShowDialog();
            }

        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbtExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void lbtConfig_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmQualityConfig frm = new frmQualityConfig();
            frm.ShowDialog();
        }

        private void lbtRefreash_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.iniGroup();
        }

        private void lbtExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //SaveFileDialog dialog = new SaveFileDialog();
            //dialog.FileName = "质量指标统计.xlsx";
            //if (dialog.ShowDialog() == DialogResult.OK)
            //{
            //    this.gridControl1.ExportToXlsx(dialog.FileName);
            //}
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

        private void bandedGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.bandedGridView1.RowCount <= 0)
                return;
            GridHitInfo HitInfo = bandedGridView1.CalcHitInfo(e.Location);//获取鼠标点击的位置
            NeuLis.Models.NeulisDictionary dic = new NeuLis.Models.NeulisDictionary();
            if (HitInfo.InRowCell && HitInfo.Column != null && e.Button == MouseButtons.Left && e.Clicks == 2)
            {
                NeuLis.Models.ModelQuatity.tatItem obj = (NeuLis.Models.ModelQuatity.tatItem)this.bandedGridView1.GetFocusedRow();
                string typeclass ="TAT";//HitInfo.Column.FieldName;
                string typeid =  obj.typeid;
                string typename = HitInfo.Column.FieldName;
                string begDate = this.dateEdit1.Text.Replace("-", "");
                string endDate = this.dateEdit2.Text.Replace("-", "");
                string groupid = "";
                foreach (int a in this.gridView2.GetSelectedRows())
                {
                    groupid += "'" + this.gridView2.GetRowCellValue(a, "groupid") + "',";
                }
                if (groupid.Length < 0)
                    return;
                groupid = groupid.Substring(0, groupid.Length - 1);
                frmQualityList frm = new frmQualityList();
                frm.typeclass = typeclass;
                frm.typeid = typeid;
                frm.typename = typename;
                frm.begDate = begDate;
                frm.endDate = endDate;
                frm.groupid = groupid;
                frm.ShowDialog();
            }
        }

        private void lbtnSysConfig_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmSysConfig frm = new frmSysConfig();
            frm.ShowDialog();
        }
        /// <summary>
        /// 显示行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView3_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
    }
}
