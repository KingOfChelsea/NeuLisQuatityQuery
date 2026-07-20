using NeuLis.Models;
using NeuLis.DataBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraExport;
using DevExpress.Export;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using NeuLis.MedicalIconsLibrary;

namespace NeuLisQuatityQuery
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 
        /// </summary>
        public Form1()
        {
           
            InitializeComponent();
            Image iconImage = IconManager.Instance.GetIcon("Medical", "bingchuang");
            if (iconImage != null)
            {
                // 将Image转换为Icon
                this.Icon = Icon.FromHandle(((Bitmap)iconImage).GetHicon());
            }
        }

        /// <summary>
        /// 点击查询按钮，根据选中的Tab页索引加载对应的质量指标数据
        /// Tab页0：年度标本可接受性标签数据（不合格标本数、标本总数、不合格率等）
        /// Tab页1：周转时间统计数据（检验前/室内周转时间中位数和90位数）
        /// </summary>
        /// <param name="sender">事件源对象</param>
        /// <param name="e">Bar item点击事件参数</param>
        private void btnQuery_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            #region 1.年度标本可接受性标签数据渲染
            if (this.xtraTabControl1.SelectedTabPageIndex == 0)
            {
                string begDate = this.begDate.Text;
                this.labelControl4.Text = this.begDate.Text;
  
                List<Model.QuaShowData> alshowData = new List<Model.QuaShowData>();  // 界面数据
                List<Model.RejectReason> alReason = NeuLis.DataBase.OperDB.GetRejectReason(); // 筛选出 typeid = 'SampleRejectReason'的记录是所有月份、所有原因的不合格数据集合
                //查询每个月的标本数
                List<Model.MonthData> alSampleNum = NeuLis.DataBase.OperDB.GetMonthSapSum(begDate);
                //查询每个月的抗凝标本数
                List<Model.MonthData> alKNSampleNum = NeuLis.DataBase.OperDB.GetMonthKNSapSum(begDate);
                //查询每个月的不合格标本数
                List<Model.MonthData> alRejectNumALL = NeuLis.DataBase.OperDB.GetMonthNum(begDate);
                // 遍历所有拒收原因
                foreach (Model.RejectReason reason in alReason)
                {
                    //每种不合格标本去查询一次对应的数据 // 从总的不合格数据中，筛选出当前拒收原因的数据
                    // alRejectNumALL 是所有月份、所有原因的不合格数据集合
                    // FindAll 找出 reason 字段等于当前原因 memo3 的所有记录
                    List<Model.MonthData> alRejectNum = alRejectNumALL.FindAll(x => x.reason == reason.memo3);

                    // 创建三个统计对象
                    Model.QuaShowData amountRej = new Model.QuaShowData();  // 不合格数量
                    Model.QuaShowData amountSap = new Model.QuaShowData();  // 标本总数
                    Model.QuaShowData rateData = new Model.QuaShowData();   // 不合格率
                    // 累计变量
                    Int64 qstRej = 0;   // 全年不合格总数
                    Int64 qstSap = 0;   // 全年标本总数
                    double qstRate = 0; // 全年不合格率

                    amountRej.TypeID ="1:"+ reason.memo3;
                    amountRej.Typename = reason.memo3;
                    amountRej.Typefx = "≤";
                    amountRej.Typemb = "0.05";
                    amountRej.Qst = "0";

                    Model.MonthData janRej = alRejectNum.Find(x => x.month == "01");
                    if (janRej == null)
                    {
                        amountRej.Jan = "";
                    }
                    else
                    {
                        amountRej.Jan = janRej.monthnum;
                        qstRej+=Convert.ToInt64(janRej.monthnum);
                    }


                    Model.MonthData febRej = alRejectNum.Find(x => x.month == "02");
                    if (febRej == null)
                    {
                        amountRej.Feb = "";
                    }
                    else
                    {
                        amountRej.Feb = febRej.monthnum;
                        qstRej += Convert.ToInt64(febRej.monthnum);
                    }

                    Model.MonthData marRej = alRejectNum.Find(x => x.month == "03");
                    if (marRej == null)
                    {
                        amountRej.Mar = "";
                    }
                    else
                    {
                        amountRej.Mar = marRej.monthnum;
                        qstRej += Convert.ToInt64(marRej.monthnum);
                    }

                    Model.MonthData jarpRej = alRejectNum.Find(x => x.month == "04");
                    if (jarpRej == null)
                    {
                        amountRej.Apr = "";
                    }
                    else
                    {
                        amountRej.Apr = jarpRej.monthnum;
                        qstRej += Convert.ToInt64(jarpRej.monthnum);
                    }

                    Model.MonthData mayRej = alRejectNum.Find(x => x.month == "05");
                    if (mayRej == null)
                    {
                        amountRej.May = "";
                    }
                    else
                    {
                        amountRej.May = mayRej.monthnum;
                        qstRej += Convert.ToInt64(mayRej.monthnum);
                    }

                    Model.MonthData junRej = alRejectNum.Find(x => x.month == "06");
                    if (junRej == null)
                    {
                        amountRej.Jun = "";
                    }
                    else
                    {
                        amountRej.Jun = junRej.monthnum;
                        qstRej += Convert.ToInt64(junRej.monthnum);
                    }

                    Model.MonthData julRej = alRejectNum.Find(x => x.month == "07");
                    if (julRej == null)
                    {
                        amountRej.Jul = "";
                    }
                    else
                    {
                        amountRej.Jul = julRej.monthnum;
                        qstRej += Convert.ToInt64(julRej.monthnum);
                    }

                    Model.MonthData augRej = alRejectNum.Find(x => x.month == "08");
                    if (augRej == null)
                    {
                        amountRej.Aug = "";
                    }
                    else
                    {
                        amountRej.Aug = augRej.monthnum;
                        qstRej += Convert.ToInt64(augRej.monthnum);
                    }

                    Model.MonthData sepRej = alRejectNum.Find(x => x.month == "09");
                    if (sepRej == null)
                    {
                        amountRej.Sep = "";
                    }
                    else
                    {
                        amountRej.Sep = sepRej.monthnum;
                        qstRej += Convert.ToInt64(sepRej.monthnum);
                    }

                    Model.MonthData octRej = alRejectNum.Find(x => x.month == "10");
                    if (octRej == null)
                    {
                        amountRej.Oct = "";
                    }
                    else
                    {
                        amountRej.Oct = octRej.monthnum;
                        qstRej += Convert.ToInt64(octRej.monthnum);
                    }

                    Model.MonthData novRej = alRejectNum.Find(x => x.month == "11");
                    if (novRej == null)
                    {
                        amountRej.Nov = "";
                    }
                    else
                    {
                        amountRej.Nov = novRej.monthnum;
                        qstRej += Convert.ToInt64(novRej.monthnum);
                    }

                    Model.MonthData decRej = alRejectNum.Find(x => x.month == "12");
                    if (decRej == null)
                    {
                        amountRej.Dec = "";
                    }
                    else
                    {
                        amountRej.Dec = decRej.monthnum;
                        qstRej += Convert.ToInt64(decRej.monthnum);
                    }
                    amountRej.Qst = qstRej.ToString();
                    alshowData.Add(amountRej);

                    //标本月总数
                    #region 标本总数
                    amountSap.TypeID ="1:"+ reason.memo3;
                    amountSap.Typename = "标本总数";
                    amountSap.Typefx = "≤";
                    amountSap.Typemb = "0.05";
                    rateData.TypeID = "1:" + reason.memo3;
                    rateData.Typename = amountRej.Typename + "不合格率(%)";
                    rateData.Typefx = "≤";
                    rateData.Typemb = "0.05";


                    Model.MonthData janAmount = new Model.MonthData();
                    if(amountRej.TypeID.IndexOf("抗凝标本")>=0)
                    {
                        janAmount = alKNSampleNum.Find(x => x.month == "01");
                    }
                    else
                    {
                        janAmount = alSampleNum.Find(x => x.month == "01");
                    }
                    if (janAmount == null)
                    {
                        amountSap.Jan = "";
                    }
                    else
                    {
                        amountSap.Jan = janAmount.monthnum;
                        qstSap += Convert.ToInt64(janAmount.monthnum);
                        rateData.Jan = Math.Round((Convert.ToDouble(string.IsNullOrEmpty(amountRej.Jan) ? "0" : amountRej.Jan) / Convert.ToDouble(amountSap.Jan)) * 100, 3).ToString();
                    }


                    Model.MonthData febAmount = new Model.MonthData();
                    if (amountRej.TypeID.IndexOf("抗凝标本") >= 0)
                    {
                        febAmount = alKNSampleNum.Find(x => x.month == "02");
                    }
                    else
                    {
                        febAmount = alSampleNum.Find(x => x.month == "02");
                    }
                    if (febAmount == null)
                    {
                        amountSap.Feb = "";
                    }
                    else
                    {
                        amountSap.Feb = febAmount.monthnum;
                        qstSap += Convert.ToInt64(febAmount.monthnum);
                        rateData.Feb = Math.Round((Convert.ToDouble(string.IsNullOrEmpty(amountRej.Feb) ? "0" : amountRej.Feb) / Convert.ToDouble(amountSap.Feb)) * 100, 3).ToString();
                    }

                    Model.MonthData marAmount = new Model.MonthData();
                    if (amountRej.TypeID.IndexOf("抗凝标本") >= 0)
                    {
                        marAmount = alKNSampleNum.Find(x => x.month == "03");
                    }
                    else
                    {
                        marAmount = alSampleNum.Find(x => x.month == "03");
                    }
                    if (marAmount == null)
                    {
                        amountSap.Mar = "";
                    }
                    else
                    {
                        amountSap.Mar = marAmount.monthnum;
                        qstSap += Convert.ToInt64(marAmount.monthnum);
                        rateData.Mar = Math.Round((Convert.ToDouble(string.IsNullOrEmpty(amountRej.Mar) ? "0" : amountRej.Mar) / Convert.ToDouble(amountSap.Mar)) * 100, 3).ToString();
                    }

                    Model.MonthData jarpAmount = new Model.MonthData();
                    if (amountRej.TypeID.IndexOf("抗凝标本") >= 0)
                    {
                        jarpAmount = alKNSampleNum.Find(x => x.month == "04");
                    }
                    else
                    {
                        jarpAmount = alSampleNum.Find(x => x.month == "04");
                    }
                    if (jarpAmount == null)
                    {
                        amountSap.Apr = "";
                    }
                    else
                    {
                        amountSap.Apr = jarpAmount.monthnum;
                        qstSap += Convert.ToInt64(jarpAmount.monthnum);
                        rateData.Apr = Math.Round((Convert.ToDouble(string.IsNullOrEmpty(amountRej.Apr) ? "0" : amountRej.Apr) / Convert.ToDouble(amountSap.Apr)) * 100, 3).ToString();
                    }

                    Model.MonthData mayAmount = new Model.MonthData();
                    if (amountRej.TypeID.IndexOf("抗凝标本") >= 0)
                    {
                        mayAmount = alKNSampleNum.Find(x => x.month == "05");
                    }
                    else
                    {
                        mayAmount = alSampleNum.Find(x => x.month == "05");
                    }
                    if (mayAmount == null)
                    {
                        amountSap.May = "";
                    }
                    else
                    {
                        amountSap.May = mayAmount.monthnum;
                        qstSap += Convert.ToInt64(mayAmount.monthnum);
                        rateData.May = Math.Round((Convert.ToDouble(string.IsNullOrEmpty(amountRej.May) ? "0" : amountRej.May) / Convert.ToDouble(amountSap.May)) * 100, 3).ToString();
                    }

                    Model.MonthData junAmount = new Model.MonthData();
                    if (amountRej.TypeID.IndexOf("抗凝标本") >= 0)
                    {
                        junAmount = alKNSampleNum.Find(x => x.month == "06");
                    }
                    else
                    {
                        junAmount = alSampleNum.Find(x => x.month == "06");
                    }
                    if (junAmount == null)
                    {
                        amountSap.Jun = "";
                    }
                    else
                    {
                        amountSap.Jun = junAmount.monthnum;
                        qstSap += Convert.ToInt64(junAmount.monthnum);
                        rateData.Jun = Math.Round((Convert.ToDouble(string.IsNullOrEmpty(amountRej.Jun) ? "0" : amountRej.Jun) / Convert.ToDouble(amountSap.Jun)) * 100, 3).ToString();
                    }

                    Model.MonthData julAmount = new Model.MonthData();
                    if (amountRej.TypeID.IndexOf("抗凝标本") >= 0)
                    {
                        julAmount = alKNSampleNum.Find(x => x.month == "07");
                    }
                    else
                    {
                        julAmount = alSampleNum.Find(x => x.month == "07");
                    }
                    if (julAmount == null)
                    {
                        amountSap.Jul = "";
                    }
                    else
                    {
                        amountSap.Jul = julAmount.monthnum;
                        qstSap += Convert.ToInt64(julAmount.monthnum);
                        rateData.Jul = Math.Round((Convert.ToDouble(string.IsNullOrEmpty(amountRej.Jul) ? "0" : amountRej.Jul) / Convert.ToDouble(amountSap.Jul)) * 100, 3).ToString();
                    }

                    Model.MonthData augAmount = new Model.MonthData();
                    if (amountRej.TypeID.IndexOf("抗凝标本") >= 0)
                    {
                        augAmount = alKNSampleNum.Find(x => x.month == "08");
                    }
                    else
                    {
                        augAmount = alSampleNum.Find(x => x.month == "08");
                    }
                    if (augAmount == null)
                    {
                        amountSap.Aug = "";
                    }
                    else
                    {
                        amountSap.Aug = augAmount.monthnum;
                        qstSap += Convert.ToInt64(augAmount.monthnum);
                        rateData.Aug = Math.Round((Convert.ToDouble(string.IsNullOrEmpty(amountRej.Aug) ? "0" : amountRej.Aug) / Convert.ToDouble(amountSap.Aug)) * 100, 3).ToString();
                    }

                    Model.MonthData sepAmount = new Model.MonthData();
                    if (amountRej.TypeID.IndexOf("抗凝标本") >= 0)
                    {
                        sepAmount = alKNSampleNum.Find(x => x.month == "09");
                    }
                    else
                    {
                        sepAmount = alSampleNum.Find(x => x.month == "09");
                    }
                    if (sepAmount == null)
                    {
                        amountSap.Sep = "";
                    }
                    else
                    {
                        amountSap.Sep = sepAmount.monthnum;
                        qstSap += Convert.ToInt64(sepAmount.monthnum);
                        rateData.Sep = Math.Round((Convert.ToDouble(string.IsNullOrEmpty(amountRej.Sep) ? "0" : amountRej.Sep) / Convert.ToDouble(amountSap.Sep)) * 100, 3).ToString();
                    }

                    Model.MonthData octAmount = new Model.MonthData();
                    if (amountRej.TypeID.IndexOf("抗凝标本") >= 0)
                    {
                        octAmount = alKNSampleNum.Find(x => x.month == "10");
                    }
                    else
                    {
                        octAmount = alSampleNum.Find(x => x.month == "10");
                    }
                    if (octAmount == null)
                    {
                        amountSap.Oct = "";
                    }
                    else
                    {
                        amountSap.Oct = octAmount.monthnum;
                        qstSap += Convert.ToInt64(octAmount.monthnum);
                        rateData.Oct = Math.Round((Convert.ToDouble(string.IsNullOrEmpty(amountRej.Oct) ? "0" : amountRej.Oct) / Convert.ToDouble(amountSap.Oct)) * 100, 3).ToString();
                    }

                    Model.MonthData novAmount = new Model.MonthData();
                    if (amountRej.TypeID.IndexOf("抗凝标本") >= 0)
                    {
                        novAmount = alKNSampleNum.Find(x => x.month == "11");
                    }
                    else
                    {
                        novAmount = alSampleNum.Find(x => x.month == "11");
                    }
                    if (novAmount == null)
                    {
                        amountSap.Nov = "";
                    }
                    else
                    {
                        amountSap.Nov = novAmount.monthnum;
                        qstSap += Convert.ToInt64(novAmount.monthnum);
                        rateData.Nov = Math.Round((Convert.ToDouble(string.IsNullOrEmpty(amountRej.Nov) ? "0" : amountRej.Nov) / Convert.ToDouble(amountSap.Nov)) * 100, 3).ToString();
                    }

                    Model.MonthData decAmount = new Model.MonthData();
                    if (amountRej.TypeID.IndexOf("抗凝标本") >= 0)
                    {
                        decAmount = alKNSampleNum.Find(x => x.month == "12");
                    }
                    else
                    {
                        decAmount = alSampleNum.Find(x => x.month == "12");
                    }
                    if (decAmount == null)
                    {
                        amountSap.Dec = "";
                    }
                    else
                    {
                        amountSap.Dec = decAmount.monthnum;
                        qstSap += Convert.ToInt64(decAmount.monthnum);
                        rateData.Dec = Math.Round((Convert.ToDouble(string.IsNullOrEmpty(amountRej.Dec) ? "0" : amountRej.Dec) / Convert.ToDouble(amountSap.Dec)) * 100, 3).ToString();
                    }
                    amountSap.Qst = qstSap.ToString();
                    qstRate = Math.Round((Convert.ToDouble(qstRej) / Convert.ToDouble(qstSap) ) * 100,3);
                    rateData.Qst = qstRate.ToString();
                    alshowData.Add(amountSap);
                    alshowData.Add(rateData);
                    #endregion
                }

                #region 1.1查询错误报告单
                List<Model.QuaShowData> errCount = this.getErrNum(begDate);
                alshowData.AddRange(errCount);
                #endregion 

                #region 1.2统计危急值数量
                List< Model.QuaShowData> alterCount = this.getAlterNum(begDate);
                alshowData.AddRange(alterCount);
                #endregion
                
                #region 1.3血培养污染
                List<Model.QuaShowData> XPYCount = this.getXPYNum(begDate);
                alshowData.AddRange(XPYCount);
                #endregion 

                #region 1.4质控变异系数不合格
                List<Model.QuaShowData> QCCount = this.getQCOverNum(begDate);
                alshowData.AddRange(QCCount);
                #endregion 

                #region 1.5添加标本拒收率、危急值报告时间中位数  Created By 徐振宇  2026年7月14日16:13:28
                List<Model.QuaShowData> totalRejectRate = NeuLis.DataBase.OperDB.GetTotalRejectRate(begDate);
                if (totalRejectRate != null && totalRejectRate.Count > 0)
                {
                    alshowData.AddRange(totalRejectRate);
                }
                #endregion

                #region 1.6 危急值报告时间中位数 Created By 徐振宇 2026年7月15日10:46:16
                List<Model.QuaShowData> totalCrisisReportTimeMedian = NeuLis.DataBase.OperDB.GetCrisisReportTimeMedian(begDate);
                if (totalCrisisReportTimeMedian != null && totalCrisisReportTimeMedian.Count > 0)
                {
                    alshowData.AddRange(totalCrisisReportTimeMedian);
                }
                #endregion

                // 数据渲染必须要放在最后面
                this.gridControl1.DataSource = alshowData;
                this.bandedGridView1.RefreshData();
                this.bandedGridView1.BestFitColumns();
            }
            #endregion

            #region 2.选中tab页第二页渲染的数据 Created By 徐振宇 2026年7月14日18:02:31
            if (this.xtraTabControl1.SelectedTabPageIndex == 1)
            {
                #region
                //检验前：1、按照条码去重统计，2、过滤掉没有采集时间，3、加急标识为急诊
                //室内:1、按照检验单统计，2、接收到审核，3、加急标识为急诊
                #endregion
                string year = this.begData1.Text;
                //清空数据
                this.gridControl2.DataSource = null;
                this.bandedGridView2.RefreshData();
                this.bandedGridView2.BestFitColumns();
                string typeID = "";
                string typeName = "";
                //this.CheckItemType();

                this.labelControl2.Text = this.begData1.Text;

                #region 1. 检验前数据统计
                //不分类查询周转时间
                List<Model.AroundMonthData> aroundData = NeuLis.DataBase.OperDB.GetAroundJYQ(year);  // 获取指定年份所有标本的周转时间中位数（不分患者类型、不分检验类别） 每个月份一个记录，包含12个月的中位数数据
                //分类查询周转时间
                List<Model.AroundMonthData> aroundTypeDate = NeuLis.DataBase.OperDB.GetAroundJYQbyType(year); // 获取按检验类别（如抗凝标本类、生化类等）细分的周转时间数据
                List<Model.QuaShowData> alshowData = new List<Model.QuaShowData>(); // list集合的数据
                Model.QuaShowData showData = new Model.QuaShowData();
                typeID = "1检验前周转时间中位数";
                typeName = "标本采集到实验室接收时间中位数";
                showData = this.getAroundSap(aroundData, "加急", typeID, typeName);
                alshowData.Add(showData);
                List<NeuLis.Models.Model.typeclass> alTypeList = NeuLis.DataBase.OperDB.getTypeList("周转时间类"); //获取时间周转 如;1	2001	抗凝标本类	抗凝标本类
                foreach (NeuLis.Models.Model.typeclass obj in alTypeList)
                {
                    showData = new Model.QuaShowData();
                    List<Model.AroundMonthData> aroundTypeDate1 = aroundTypeDate.FindAll(x => x.classType == obj.typeName);
                    showData = this.getAroundSap(aroundTypeDate1, "加急", typeID, typeName, obj.typeName);
                    alshowData.Add(showData);
                }

                showData = new Model.QuaShowData();
                showData = this.getAroundSap(aroundData, "门诊", typeID, typeName);
                alshowData.Add(showData);
                foreach (NeuLis.Models.Model.typeclass obj in alTypeList)
                {
                    showData = new Model.QuaShowData();
                    List<Model.AroundMonthData> aroundTypeDate1 = aroundTypeDate.FindAll(x => x.classType == obj.typeName);
                    showData = this.getAroundSap(aroundTypeDate1, "门诊", typeID, typeName, obj.typeName);
                    alshowData.Add(showData);
                }

                showData = new Model.QuaShowData();
                showData = this.getAroundSap(aroundData, "住院", typeID, typeName);
                alshowData.Add(showData);
                foreach (NeuLis.Models.Model.typeclass obj in alTypeList)
                {
                    showData = new Model.QuaShowData();
                    List<Model.AroundMonthData> aroundTypeDate1 = aroundTypeDate.FindAll(x => x.classType == obj.typeName);
                    showData = this.getAroundSap(aroundTypeDate1, "住院", typeID, typeName, obj.typeName);
                    alshowData.Add(showData);
                }

                #endregion
                #region 2. 室内中位数统计
                typeID = "2室内周转时间中位数";
                typeName = "实验室接收时间到审核时间中位数";
                //不分类查询周转时间
                aroundData = NeuLis.DataBase.OperDB.GetAroundSN(year);
                //分类查询周转时间
                aroundTypeDate = NeuLis.DataBase.OperDB.GetAroundSNbyType(year);

                showData = new Model.QuaShowData();
                showData = this.getAroundSap(aroundData, "加急", typeID, typeName);
                alshowData.Add(showData);
                alTypeList = NeuLis.DataBase.OperDB.getTypeList("周转时间类");
                foreach (NeuLis.Models.Model.typeclass obj in alTypeList)
                {
                    showData = new Model.QuaShowData();
                    List<Model.AroundMonthData> aroundTypeDate1 = aroundTypeDate.FindAll(x => x.classType == obj.typeName);
                    showData = this.getAroundSap(aroundTypeDate1, "加急", typeID, typeName, obj.typeName);
                    alshowData.Add(showData);
                }

                showData = new Model.QuaShowData();
                showData = this.getAroundSap(aroundData, "门诊", typeID, typeName);
                alshowData.Add(showData);
                foreach (NeuLis.Models.Model.typeclass obj in alTypeList)
                {
                    showData = new Model.QuaShowData();
                    List<Model.AroundMonthData> aroundTypeDate1 = aroundTypeDate.FindAll(x => x.classType == obj.typeName);
                    showData = this.getAroundSap(aroundTypeDate1, "门诊", typeID, typeName, obj.typeName);
                    alshowData.Add(showData);
                }

                showData = new Model.QuaShowData();
                showData = this.getAroundSap(aroundData, "住院", typeID, typeName);
                alshowData.Add(showData);
                foreach (NeuLis.Models.Model.typeclass obj in alTypeList)
                {
                    showData = new Model.QuaShowData();
                    List<Model.AroundMonthData> aroundTypeDate1 = aroundTypeDate.FindAll(x => x.classType == obj.typeName);
                    showData = this.getAroundSap(aroundTypeDate1, "住院", typeID, typeName, obj.typeName);
                    alshowData.Add(showData);
                }
                #endregion
                #region 3.检验前数据统计,90分数
                //不分类查询周转时间
                aroundData = NeuLis.DataBase.OperDB.GetAroundJYQ90(year);
                //分类查询周转时间
                aroundTypeDate = NeuLis.DataBase.OperDB.GetAroundJYQbyType90(year);
                //alshowData = new List<Model.QuaShowData>();
                showData = new Model.QuaShowData();
                typeID = "3检验前周转时间90位数";
                typeName = "标本采集到实验室接收时间90位数";
                showData = this.getAroundSap(aroundData, "加急", typeID, typeName);
                alshowData.Add(showData);
                alTypeList = NeuLis.DataBase.OperDB.getTypeList("周转时间类");
                foreach (NeuLis.Models.Model.typeclass obj in alTypeList)
                {
                    showData = new Model.QuaShowData();
                    List<Model.AroundMonthData> aroundTypeDate1 = aroundTypeDate.FindAll(x => x.classType == obj.typeName);
                    showData = this.getAroundSap(aroundTypeDate1, "加急", typeID, typeName, obj.typeName);
                    alshowData.Add(showData);
                }

                showData = new Model.QuaShowData();
                showData = this.getAroundSap(aroundData, "门诊", typeID, typeName);
                alshowData.Add(showData);
                foreach (NeuLis.Models.Model.typeclass obj in alTypeList)
                {
                    showData = new Model.QuaShowData();
                    List<Model.AroundMonthData> aroundTypeDate1 = aroundTypeDate.FindAll(x => x.classType == obj.typeName);
                    showData = this.getAroundSap(aroundTypeDate1, "门诊", typeID, typeName, obj.typeName);
                    alshowData.Add(showData);
                }

                showData = new Model.QuaShowData();
                showData = this.getAroundSap(aroundData, "住院", typeID, typeName);
                alshowData.Add(showData);
                foreach (NeuLis.Models.Model.typeclass obj in alTypeList)
                {
                    showData = new Model.QuaShowData();
                    List<Model.AroundMonthData> aroundTypeDate1 = aroundTypeDate.FindAll(x => x.classType == obj.typeName);
                    showData = this.getAroundSap(aroundTypeDate1, "住院", typeID, typeName, obj.typeName);
                    alshowData.Add(showData);
                }
                #endregion
                #region 4.室内中位数统计
                typeID = "4室内周转时间90位数";
                typeName = "实验室接收时间到审核时间90位数";
                //不分类查询周转时间
                aroundData = NeuLis.DataBase.OperDB.GetAroundSN90(year);
                //分类查询周转时间
                aroundTypeDate = NeuLis.DataBase.OperDB.GetAroundSNbyType90(year);

                showData = new Model.QuaShowData();
                showData = this.getAroundSap(aroundData, "加急", typeID, typeName);
                alshowData.Add(showData);
                alTypeList = NeuLis.DataBase.OperDB.getTypeList("周转时间类");
                foreach (NeuLis.Models.Model.typeclass obj in alTypeList)
                {
                    showData = new Model.QuaShowData();
                    List<Model.AroundMonthData> aroundTypeDate1 = aroundTypeDate.FindAll(x => x.classType == obj.typeName);
                    showData = this.getAroundSap(aroundTypeDate1, "加急", typeID, typeName, obj.typeName);
                    alshowData.Add(showData);
                }

                showData = new Model.QuaShowData();
                showData = this.getAroundSap(aroundData, "门诊", typeID, typeName);
                alshowData.Add(showData);
                foreach (NeuLis.Models.Model.typeclass obj in alTypeList)
                {
                    showData = new Model.QuaShowData();
                    List<Model.AroundMonthData> aroundTypeDate1 = aroundTypeDate.FindAll(x => x.classType == obj.typeName);
                    showData = this.getAroundSap(aroundTypeDate1, "门诊", typeID, typeName, obj.typeName);
                    alshowData.Add(showData);
                }

                showData = new Model.QuaShowData();
                showData = this.getAroundSap(aroundData, "住院", typeID, typeName);
                alshowData.Add(showData);
                foreach (NeuLis.Models.Model.typeclass obj in alTypeList)
                {
                    showData = new Model.QuaShowData();
                    List<Model.AroundMonthData> aroundTypeDate1 = aroundTypeDate.FindAll(x => x.classType == obj.typeName);
                    showData = this.getAroundSap(aroundTypeDate1, "住院", typeID, typeName, obj.typeName);
                    alshowData.Add(showData);
                }
                #endregion
                #region 5.插入检验总周转时间 Created By 徐振宇 2026年7月14日19:01:07
                List<Model.QuaShowData> totalRejectRate = NeuLis.DataBase.OperDB.GetTATP90(this.begData1.Text);
                if (totalRejectRate != null && totalRejectRate.Count > 0)
                {
                    alshowData.AddRange(totalRejectRate);
                }
                #endregion
                this.gridControl2.DataSource = alshowData;
                this.bandedGridView2.RefreshData();
                this.bandedGridView2.BestFitColumns();
            }
            #endregion
        }
        /// <summary>
        /// 获取危急值数量
        /// </summary>
        /// <param name="aroundData"></param>
        /// <param name="patienttype"></param>
        /// <param name="typeID"></param>
        /// <param name="TypeName"></param>
        /// <param name="typeClass"></param>
        /// <returns></returns>
        private List<Model.QuaShowData> getAlterNum(  string begDate)
        {
           // Model.QuaShowAroundData alShowData = new Model.QuaShowAroundData();
            Model.QuaShowData around = new Model.QuaShowData();
            List<Model.MonthData> alPhoneNum = NeuLis.DataBase.OperDB.GetMonthPhoneAlterSum(begDate);
            List<Model.MonthData> alAlterNum = NeuLis.DataBase.OperDB.GetMonthLifeAlterSum(begDate);
            List<Model.MonthData> alJSAlterNum = NeuLis.DataBase.OperDB.GetMonthJSAlterSum(begDate);
            //通报率
            List<Model.QuaShowData> alShowData = new List<Model.QuaShowData>();
            Model.QuaShowData aroundALL = new Model.QuaShowData();
            Model.QuaShowData aroundRate = new Model.QuaShowData();
            //及时率统计
            Model.QuaShowData aroundJS = new Model.QuaShowData();
            Model.QuaShowData aroundJSALL = new Model.QuaShowData();
            Model.QuaShowData aroundJSRate = new Model.QuaShowData();
            long qstPhone = 0;//通报数
            long qstJss = 0;//及时数
            long qstAll = 0;//危急值总数
            double qstRate = 0;//通报率

            around.TypeID = "3:危急值通报率";
            around.Typename = "已通报的危急值检验项目数" ;
            around.Typefx = "=";
            around.Typemb = "100";
            aroundALL.TypeID = "3:危急值通报率";
            aroundALL.Typename = "分母：同期需要危急值通报的检验项目总数";
            aroundALL.Typefx = "=";
            aroundALL.Typemb = "100";
            aroundRate.TypeID = "3:危急值通报率";
            aroundRate.Typename = "危急值通报率";
            aroundRate.Typefx = "=";
            aroundRate.Typemb = "100";

            aroundJS.TypeID = "4:危急值通报及时率";
            aroundJS.Typename = "危急值通报及时数";
            aroundJS.Typefx = "=";
            aroundJS.Typemb = "100";
            aroundJSALL.TypeID = "4:危急值通报及时率";
            aroundJSALL.Typename = "分母：同期需要危急值通报的检验项目总数";
            aroundJSALL.Typefx = "=";
            aroundJSALL.Typemb = "100";
            aroundJSRate.TypeID = "4:危急值通报及时率";
            aroundJSRate.Typename = "危急值通报及时率";
            aroundJSRate.Typefx = "=";
            aroundJSRate.Typemb = "100";
            #region 危急值通报率
            Model.MonthData janALL = alAlterNum.Find(x => x.month == "01");
            Model.MonthData janPhone = alPhoneNum.Find(x => x.month == "01");
            if (janALL == null)
            {
                aroundALL.Jan = "";
            }
            else
            {
                aroundALL.Jan = janALL.monthnum;
                around.Jan = janPhone == null ? "0" : janPhone.monthnum;
                aroundRate.Jan = Math.Round((Convert.ToDouble(around.Jan) / Convert.ToDouble(aroundALL.Jan)*100),2).ToString();
                qstPhone += Convert.ToInt64( around.Jan);
                qstAll += Convert.ToInt64(aroundALL.Jan);

            }


            Model.MonthData febALL = alAlterNum.Find(x => x.month == "02");
            Model.MonthData febPhone = alPhoneNum.Find(x => x.month == "02");
            
            if (febALL == null)
            {
                around.Feb = "";
            }
            else
            {
                aroundALL.Feb = febALL.monthnum;
                around.Feb = febPhone==null?"0": febPhone.monthnum;
                aroundRate.Feb = Math.Round((Convert.ToDouble(around.Feb) / Convert.ToDouble(aroundALL.Feb) * 100),2).ToString();
                qstPhone += Convert.ToInt64(around.Feb);
                qstAll += Convert.ToInt64(aroundALL.Feb);
            }

            Model.MonthData marALL = alAlterNum.Find(x => x.month == "03");
            Model.MonthData marPhone = alPhoneNum.Find(x => x.month == "03");
            if (marALL == null)
            {
                around.Mar = "";
            }
            else
            {
                aroundALL.Mar = marALL.monthnum;
                around.Mar = febPhone == null ? "0" : febPhone.monthnum;
                aroundRate.Mar = Math.Round((Convert.ToDouble(around.Mar) / Convert.ToDouble(aroundALL.Mar) * 100),2).ToString();
                qstPhone += Convert.ToInt64(around.Mar);
                qstAll += Convert.ToInt64(aroundALL.Mar);
            }

            Model.MonthData jarALL = alAlterNum.Find(x => x.month == "04");
            Model.MonthData jarpPhone = alPhoneNum.Find(x => x.month == "04");
            if (jarALL == null)
            {
                around.Apr = "";
            }
            else
            {
                aroundALL.Apr = jarALL.monthnum;
                around.Apr = jarpPhone == null ? "0" : jarpPhone.monthnum;
                aroundRate.Apr = Math.Round((Convert.ToDouble(around.Apr) / Convert.ToDouble(aroundALL.Apr) * 100),2).ToString();
                qstPhone += Convert.ToInt64(around.Apr);
                qstAll += Convert.ToInt64(aroundALL.Apr);
            }

            Model.MonthData mayALL = alAlterNum.Find(x => x.month == "05");
            Model.MonthData mayPhone = alPhoneNum.Find(x => x.month == "05");
            if (mayALL == null)
            {
                around.May = "";
            }
            else
            {
                aroundALL.May = mayALL.monthnum;
                around.May = mayPhone == null ? "0" : mayPhone.monthnum;
                aroundRate.May = Math.Round((Convert.ToDouble(around.May) / Convert.ToDouble(aroundALL.May) * 100),2).ToString();
                qstPhone += Convert.ToInt64(around.May);
                qstAll += Convert.ToInt64(aroundALL.May);
            }

            Model.MonthData junALL = alAlterNum.Find(x => x.month == "06");
            Model.MonthData junPhone = alPhoneNum.Find(x => x.month == "06");
            if (junALL == null)
            {
                around.Jun = "";
            }
            else
            {
                aroundALL.Jun = junALL.monthnum;
                around.Jun = junPhone == null ? "0" : junPhone.monthnum; 
                aroundRate.Jun = Math.Round((Convert.ToDouble(around.Jun) / Convert.ToDouble(aroundALL.Jun) * 100),2).ToString();
                qstPhone += Convert.ToInt64(around.Jun);
                qstAll += Convert.ToInt64(aroundALL.Jun);
            }

            Model.MonthData julALL = alAlterNum.Find(x => x.month == "07");
            Model.MonthData julPhone = alPhoneNum.Find(x => x.month == "07");
            if (julALL == null)
            {
                around.Jul = "";
            }
            else
            {
                aroundALL.Jul = julALL.monthnum;
                around.Jul = julPhone == null ? "0" : julPhone.monthnum; 
                aroundRate.Jul = Math.Round((Convert.ToDouble(around.Jul) / Convert.ToDouble(aroundALL.Jul) * 100),2).ToString();
                qstPhone += Convert.ToInt64(around.Jul);
                qstAll += Convert.ToInt64(aroundALL.Jul);
            }

            Model.MonthData augALL = alAlterNum.Find(x => x.month == "08");
            Model.MonthData augPhone = alPhoneNum.Find(x => x.month == "08");
            if (augALL == null)
            {
                around.Aug = "";
            }
            else
            {
                aroundALL.Aug = julALL.monthnum;
                around.Aug = augPhone == null ? "0" : augPhone.monthnum;
                aroundRate.Aug = Math.Round((Convert.ToDouble(around.Aug) / Convert.ToDouble(aroundALL.Aug) * 100),2).ToString();
                qstPhone += Convert.ToInt64(around.Aug);
                qstAll += Convert.ToInt64(aroundALL.Aug);
            }

            Model.MonthData sepALL = alAlterNum.Find(x => x.month == "09");
            Model.MonthData sepPhone = alPhoneNum.Find(x => x.month == "09");
            if (sepALL == null)
            {
                around.Sep = "";
            }
            else
            {
                aroundALL.Sep = sepALL.monthnum;
                around.Sep = sepPhone == null ? "0" : sepPhone.monthnum; 
                aroundRate.Sep = Math.Round((Convert.ToDouble(around.Sep) / Convert.ToDouble(aroundALL.Sep) * 100),2).ToString();
                qstPhone += Convert.ToInt64(around.Sep);
                qstAll += Convert.ToInt64(aroundALL.Sep);
            }

            Model.MonthData octALL = alAlterNum.Find(x => x.month == "10");
            Model.MonthData octPhone = alPhoneNum.Find(x => x.month == "10");
            if (octALL == null)
            {
                around.Oct = "";
            }
            else
            {
                aroundALL.Oct = octALL.monthnum;
                around.Oct = octPhone == null ? "0" : octPhone.monthnum;
                aroundRate.Oct = Math.Round((Convert.ToDouble(around.Oct) / Convert.ToDouble(aroundALL.Oct) * 100),2).ToString();
                qstPhone += Convert.ToInt64(around.Oct);
                qstAll += Convert.ToInt64(aroundALL.Oct);
            }

            Model.MonthData novALL = alAlterNum.Find(x => x.month == "11");
            Model.MonthData novPhone = alPhoneNum.Find(x => x.month == "11");
            if (novALL == null)
            {
                around.Nov = "";
            }
            else
            {
                aroundALL.Nov = novALL.monthnum;
                around.Nov = novPhone==null?"0": novPhone.monthnum ;
                aroundRate.Nov = Math.Round( (Convert.ToDouble(around.Nov) / Convert.ToDouble(aroundALL.Nov) * 100),2).ToString();
                qstPhone += Convert.ToInt64(around.Nov);
                qstAll += Convert.ToInt64(aroundALL.Nov);
            }

            Model.MonthData decALL = alAlterNum.Find(x => x.month == "12");
            Model.MonthData decPhone = alPhoneNum.Find(x => x.month == "12");
            if (decALL == null)
            {
                around.Dec = "";
            }
            else
            {
                aroundALL.Dec = decALL.monthnum;
                around.Dec = decPhone == null ? "0" : decPhone.monthnum;
                aroundRate.Dec = Math.Round((Convert.ToDouble(around.Dec) / Convert.ToDouble(aroundALL.Dec) * 100),2).ToString();
                qstPhone += Convert.ToInt64(around.Dec);
                qstAll += Convert.ToInt64(aroundALL.Dec);
            }
            around.Qst = qstPhone.ToString();
            aroundALL.Qst = qstAll.ToString();
            aroundRate.Qst = Math.Round(Convert.ToDouble(qstPhone) / Convert.ToDouble(qstAll) * 100, 2).ToString();
            alShowData.Add(around);
            alShowData.Add(aroundALL);
            alShowData.Add(aroundRate);
            #endregion
            #region 危急值及时率

            Model.MonthData janJS = alJSAlterNum.Find(x => x.month == "01");
            if (janALL == null)
            {
                aroundJSALL.Jan = "";
            }
            else
            {
                aroundJSALL.Jan = janALL.monthnum;
                aroundJS.Jan = janJS == null ? "0" : janJS.monthnum;
                aroundJSRate.Jan = Math.Round((Convert.ToDouble(aroundJS.Jan) / Convert.ToDouble(aroundJSALL.Jan) * 100), 2).ToString();
                qstJss += Convert.ToInt64(aroundJS.Jan);
                
            }


            Model.MonthData febJS = alJSAlterNum.Find(x => x.month == "02");

            if (febALL == null)
            {
                aroundJSALL.Feb = "";
            }
            else
            {
                aroundJSALL.Feb = febALL.monthnum;
                aroundJS.Feb = febJS == null ? "0" : febJS.monthnum;
                aroundJSRate.Feb = Math.Round((Convert.ToDouble(aroundJS.Feb) / Convert.ToDouble(aroundJSALL.Feb) * 100), 2).ToString();
                qstJss += Convert.ToInt64(aroundJS.Feb);
            }

            Model.MonthData marJS = alJSAlterNum.Find(x => x.month == "03");
            if (marALL == null)
            {
                aroundJSALL.Mar = "";
            }
            else
            {
                aroundJSALL.Mar = marALL.monthnum;
                aroundJS.Mar = marJS == null ? "0" : marJS.monthnum;
                aroundJSRate.Mar = Math.Round((Convert.ToDouble(aroundJS.Mar) / Convert.ToDouble(aroundJSALL.Mar) * 100), 2).ToString();
                qstJss += Convert.ToInt64(aroundJS.Mar);
            }

 
            Model.MonthData jarpJS = alJSAlterNum.Find(x => x.month == "04");
            if (jarALL == null)
            {
                aroundJSALL.Apr = "";
            }
            else
            {
                aroundJSALL.Apr = jarALL.monthnum;
                aroundJS.Apr = jarpJS == null ? "0" : jarpJS.monthnum;
                aroundJSRate.Apr = Math.Round((Convert.ToDouble(aroundJS.Apr) / Convert.ToDouble(aroundJSALL.Apr) * 100), 2).ToString();
                qstJss += Convert.ToInt64(aroundJS.Apr);
            }

            Model.MonthData mayJS = alJSAlterNum.Find(x => x.month == "05");
            if (mayALL == null)
            {
                aroundJSALL.May = "";
            }
            else
            {
                aroundJSALL.May = mayALL.monthnum;
                aroundJS.May = mayJS == null ? "0" : mayJS.monthnum;
                aroundJSRate.May = Math.Round((Convert.ToDouble(aroundJS.May) / Convert.ToDouble(aroundJSALL.May) * 100), 2).ToString();
                qstJss += Convert.ToInt64(aroundJS.May);
            }


            Model.MonthData junJS = alJSAlterNum.Find(x => x.month == "06");
            if (junALL == null)
            {
                aroundJSALL.Jun = "";
            }
            else
            {
                aroundJSALL.Jun = junALL.monthnum;
                aroundJS.Jun = junJS == null ? "0" : junJS.monthnum;
                aroundJSRate.Jun = Math.Round((Convert.ToDouble(aroundJS.Jun) / Convert.ToDouble(aroundJSALL.Jun) * 100), 2).ToString();
                qstJss += Convert.ToInt64(aroundJS.Jun);
            }


            Model.MonthData julJS = alJSAlterNum.Find(x => x.month == "07");
            if (julALL == null)
            {
                aroundJSALL.Jul = "";
            }
            else
            {
                aroundJSALL.Jul = julALL.monthnum;
                aroundJS.Jul = julJS == null ? "0" : julJS.monthnum;
                aroundJSRate.Jul = Math.Round((Convert.ToDouble(aroundJS.Jul) / Convert.ToDouble(aroundJSALL.Jul) * 100), 2).ToString();
                qstJss += Convert.ToInt64(aroundJS.Jul);
            }


            Model.MonthData augJS = alJSAlterNum.Find(x => x.month == "08");
            if (augALL == null)
            {
                aroundJSALL.Aug = "";
            }
            else
            {
                aroundJSALL.Aug = julALL.monthnum;
                aroundJS.Aug = augJS == null ? "0" : augJS.monthnum;
                aroundJSRate.Aug = Math.Round((Convert.ToDouble(aroundJS.Aug) / Convert.ToDouble(aroundJSALL.Aug) * 100), 2).ToString();
                qstJss += Convert.ToInt64(aroundJS.Aug);
            }


            Model.MonthData sepJS = alJSAlterNum.Find(x => x.month == "09");
            if (sepALL == null)
            {
                aroundJSALL.Sep = "";
            }
            else
            {
                aroundJSALL.Sep = sepALL.monthnum;
                aroundJS.Sep = sepJS == null ? "0" : sepJS.monthnum;
                aroundJSRate.Sep = Math.Round((Convert.ToDouble(aroundJS.Sep) / Convert.ToDouble(aroundJSALL.Sep) * 100), 2).ToString();
                qstJss += Convert.ToInt64(aroundJS.Sep);
            }


            Model.MonthData octJS = alJSAlterNum.Find(x => x.month == "10");
            if (octALL == null)
            {
                aroundJSALL.Oct = "";
            }
            else
            {
                aroundJSALL.Oct = octALL.monthnum;
                aroundJS.Oct = octJS == null ? "0" : octJS.monthnum;
                aroundJSRate.Oct = Math.Round((Convert.ToDouble(aroundJS.Oct) / Convert.ToDouble(aroundJSALL.Oct) * 100), 2).ToString();
                qstJss += Convert.ToInt64(aroundJS.Oct);
            }


            Model.MonthData novJS = alJSAlterNum.Find(x => x.month == "11");
            if (novALL == null)
            {
                aroundJSALL.Nov = "";
            }
            else
            {
                aroundJSALL.Nov = novALL.monthnum;
                aroundJS.Nov = novJS == null ? "0" : novJS.monthnum;
                aroundJSRate.Nov = Math.Round((Convert.ToDouble(aroundJS.Nov) / Convert.ToDouble(aroundJSALL.Nov) * 100), 2).ToString();
                qstJss += Convert.ToInt64(aroundJS.Nov);
            }


            Model.MonthData decJS = alJSAlterNum.Find(x => x.month == "12");
            if (decALL == null)
            {
                aroundJSALL.Dec = "";
            }
            else
            {
                aroundJSALL.Dec = decALL.monthnum;
                aroundJS.Dec = decJS == null ? "0" : decJS.monthnum;
                aroundJSRate.Dec = Math.Round((Convert.ToDouble(aroundJS.Dec) / Convert.ToDouble(aroundJSALL.Dec) * 100), 2).ToString();
                qstJss += Convert.ToInt64(aroundJS.Dec);
            }
            aroundJS.Qst = qstJss.ToString();
            aroundJSALL.Qst = qstAll.ToString();
            aroundJSRate.Qst = Math.Round(Convert.ToDouble(qstJss) / Convert.ToDouble(qstAll) * 100, 2).ToString();
            alShowData.Add(aroundJS);
            alShowData.Add(aroundJSALL);
            alShowData.Add(aroundJSRate);
            #endregion
            return alShowData;
        }
        /// <summary>
        /// 查询不正确报告单数
        /// </summary>
        /// <param name="begDate"></param>
        /// <returns></returns>
        private List<Model.QuaShowData> getErrNum(string begDate)
        {
            // Model.QuaShowAroundData alShowData = new Model.QuaShowAroundData();
            Model.QuaShowData around = new Model.QuaShowData();
            List<Model.MonthData> alErrNum = NeuLis.DataBase.OperDB.getErrReport(begDate);
            List<Model.MonthData> alReportNum = NeuLis.DataBase.OperDB.GetMonthReportSum(begDate);
            List<Model.QuaShowData> alShowData = new List<Model.QuaShowData>();
            Model.QuaShowData aroundALL = new Model.QuaShowData();
            Model.QuaShowData aroundRate = new Model.QuaShowData();
            long qstErr = 0;//错误数量合计
            long qstAll = 0;//标本总数
            double qstRate = 0;//错误率

            around.TypeID = "2:检验报告不正确";
            around.Typename = "分子：实验室发出的不正确检验报告数";
            around.Typefx = "≤";
            around.Typemb = "0.5";
            aroundALL.TypeID = "2:检验报告不正确";
            aroundALL.Typename = "分母：同期检验报告总数";
            aroundALL.Typefx = "≤";
            aroundALL.Typemb = "0.5";
            aroundRate.TypeID = "2:检验报告不正确";
            aroundRate.Typename = "检验报告不正确率(%)";
            aroundRate.Typefx = "≤";
            aroundRate.Typemb = "0.5";

            Model.MonthData janALL = alReportNum.Find(x => x.month == "01");
            Model.MonthData janErr = alErrNum.Find(x => x.month == "01");
            if (janALL == null)
            {
                aroundALL.Jan = "";
            }
            else
            {
                aroundALL.Jan = janALL.monthnum;
                around.Jan = janErr == null ? "0" : janErr.monthnum;
                aroundRate.Jan = Math.Round((Convert.ToDouble(around.Jan) / Convert.ToDouble(aroundALL.Jan) * 100), 2).ToString();
                qstErr += Convert.ToInt64( around.Jan);
                qstAll += Convert.ToInt64(aroundALL.Jan);
            }


            Model.MonthData febALL = alReportNum.Find(x => x.month == "02");
            Model.MonthData febErr = alErrNum.Find(x => x.month == "02");

            if (febALL == null)
            {
                around.Feb = "";
            }
            else
            {
                aroundALL.Feb = febALL.monthnum;
                around.Feb = febErr == null ? "0" : febErr.monthnum;
                aroundRate.Feb = Math.Round((Convert.ToDouble(around.Feb) / Convert.ToDouble(aroundALL.Feb) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Feb);
                qstAll += Convert.ToInt64(aroundALL.Feb);
            }

            Model.MonthData marALL = alReportNum.Find(x => x.month == "03");
            Model.MonthData marErr = alErrNum.Find(x => x.month == "03");
            if (marALL == null)
            {
                around.Mar = "";
            }
            else
            {
                aroundALL.Mar = marALL.monthnum;
                around.Mar = marErr == null ? "0" : marErr.monthnum;
                aroundRate.Mar = Math.Round((Convert.ToDouble(around.Mar) / Convert.ToDouble(aroundALL.Mar) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Mar);
                qstAll += Convert.ToInt64(aroundALL.Mar);
            }

            Model.MonthData jarALL = alReportNum.Find(x => x.month == "04");
            Model.MonthData jarpErr = alErrNum.Find(x => x.month == "04");
            if (jarALL == null)
            {
                around.Apr = "";
            }
            else
            {
                aroundALL.Apr = jarALL.monthnum;
                around.Apr = jarpErr == null ? "0" : jarpErr.monthnum;
                aroundRate.Apr = Math.Round((Convert.ToDouble(around.Apr) / Convert.ToDouble(aroundALL.Apr) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Apr);
                qstAll += Convert.ToInt64(aroundALL.Apr);
            }

            Model.MonthData mayALL = alReportNum.Find(x => x.month == "05");
            Model.MonthData mayErr = alErrNum.Find(x => x.month == "05");
            if (mayALL == null)
            {
                around.May = "";
            }
            else
            {
                aroundALL.May = mayALL.monthnum;
                around.May = mayErr == null ? "0" : mayErr.monthnum;
                aroundRate.May = Math.Round((Convert.ToDouble(around.May) / Convert.ToDouble(aroundALL.May) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.May);
                qstAll += Convert.ToInt64(aroundALL.May);
            }

            Model.MonthData junALL = alReportNum.Find(x => x.month == "06");
            Model.MonthData junErr = alErrNum.Find(x => x.month == "06");
            if (junALL == null)
            {
                around.Jun = "";
            }
            else
            {
                aroundALL.Jun = junALL.monthnum;
                around.Jun = junErr == null ? "0" : junErr.monthnum;
                aroundRate.Jun = Math.Round((Convert.ToDouble(around.Jun) / Convert.ToDouble(aroundALL.Jun) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Jun);
                qstAll += Convert.ToInt64(aroundALL.Jun);
            }

            Model.MonthData julALL = alReportNum.Find(x => x.month == "07");
            Model.MonthData julErr = alErrNum.Find(x => x.month == "07");
            if (julALL == null)
            {
                around.Jul = "";
            }
            else
            {
                aroundALL.Jul = julALL.monthnum;
                around.Jul = julErr == null ? "0" : julErr.monthnum;
                aroundRate.Jul = Math.Round((Convert.ToDouble(around.Jul) / Convert.ToDouble(aroundALL.Jul) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Jul);
                qstAll += Convert.ToInt64(aroundALL.Jul);
            }

            Model.MonthData augALL = alReportNum.Find(x => x.month == "08");
            Model.MonthData augErr = alErrNum.Find(x => x.month == "08");
            if (augALL == null)
            {
                around.Aug = "";
            }
            else
            {
                aroundALL.Aug = julALL.monthnum;
                around.Aug = augErr == null ? "0" : augErr.monthnum;
                aroundRate.Aug = Math.Round((Convert.ToDouble(around.Aug) / Convert.ToDouble(aroundALL.Aug) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Aug);
                qstAll += Convert.ToInt64(aroundALL.Aug);
            }

            Model.MonthData sepALL = alReportNum.Find(x => x.month == "09");
            Model.MonthData sepErr = alErrNum.Find(x => x.month == "09");
            if (sepALL == null)
            {
                around.Sep = "";
            }
            else
            {
                aroundALL.Sep = sepALL.monthnum;
                around.Sep = sepErr == null ? "0" : sepErr.monthnum;
                aroundRate.Sep = Math.Round((Convert.ToDouble(around.Sep) / Convert.ToDouble(aroundALL.Sep) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Sep);
                qstAll += Convert.ToInt64(aroundALL.Sep);
            }

            Model.MonthData octALL = alReportNum.Find(x => x.month == "10");
            Model.MonthData octErr = alErrNum.Find(x => x.month == "10");
            if (octALL == null)
            {
                around.Oct = "";
            }
            else
            {
                aroundALL.Oct = octALL.monthnum;
                around.Oct = octErr == null ? "0" : octErr.monthnum;
                aroundRate.Oct = Math.Round((Convert.ToDouble(around.Oct) / Convert.ToDouble(aroundALL.Oct) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Oct);
                qstAll += Convert.ToInt64(aroundALL.Oct);
            }

            Model.MonthData novALL = alReportNum.Find(x => x.month == "11");
            Model.MonthData novErr = alErrNum.Find(x => x.month == "11");
            if (novALL == null)
            {
                around.Nov = "";
            }
            else
            {
                aroundALL.Nov = novALL.monthnum;
                around.Nov = novErr == null ? "0" : novErr.monthnum;
                aroundRate.Nov = Math.Round((Convert.ToDouble(around.Nov) / Convert.ToDouble(aroundALL.Nov) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Nov);
                qstAll += Convert.ToInt64(aroundALL.Nov);
            }

            Model.MonthData decALL = alReportNum.Find(x => x.month == "12");
            Model.MonthData decErr = alErrNum.Find(x => x.month == "12");
            if (decALL == null)
            {
                around.Dec = "";
            }
            else
            {
                aroundALL.Dec = decALL.monthnum;
                around.Dec = decErr == null ? "0" : decErr.monthnum;
                aroundRate.Dec = Math.Round((Convert.ToDouble(around.Dec) / Convert.ToDouble(aroundALL.Dec) * 100),2).ToString();
                qstErr += Convert.ToInt64(around.Dec);
                qstAll += Convert.ToInt64(aroundALL.Dec);
            }
            around.Qst = qstErr.ToString();
            aroundALL.Qst = qstAll.ToString();
            aroundRate.Qst = Math.Round(Convert.ToDouble(qstErr) / Convert.ToDouble(qstAll) * 100, 2).ToString();
            alShowData.Add(around);
            alShowData.Add(aroundALL);
            alShowData.Add(aroundRate);
            return alShowData;
        }
        /// <summary>
        /// 血培养污染数据统计
        /// </summary>
        /// <param name="begDate"></param>
        /// <returns></returns>
        private List<Model.QuaShowData> getXPYNum(string begDate)
        {
            // Model.QuaShowAroundData alShowData = new Model.QuaShowAroundData();
            Model.QuaShowData around = new Model.QuaShowData();
            List<Model.MonthData> alErrNum = NeuLis.DataBase.OperDB.GetMonthXPYWRSum(begDate);
            List<Model.MonthData> alReportNum = NeuLis.DataBase.OperDB.GetMonthXPYSum(begDate);
            List<Model.QuaShowData> alShowData = new List<Model.QuaShowData>();
            Model.QuaShowData aroundALL = new Model.QuaShowData();
            Model.QuaShowData aroundRate = new Model.QuaShowData();
            long qstWr = 0;//血培养污染数合计
            long qstALL = 0;//血培养总数
            double qstRate = 0;//血培养污染率

            around.TypeID = "5:血培养污染率";
            around.Typename = "分子：污染的血培养数";
            around.Typefx = "≤";
            around.Typemb = "3";
            aroundALL.TypeID = "5:血培养污染率";
            aroundALL.Typename = "分母：同期血培养总套数";
            aroundALL.Typefx = "≤";
            aroundALL.Typemb = "3";
            aroundRate.TypeID = "5:血培养污染率";
            aroundRate.Typename = "血培养污染率(%)";
            aroundRate.Typefx = "≤";
            aroundRate.Typemb = "3";

            Model.MonthData janALL = alReportNum.Find(x => x.month == "01");
            Model.MonthData janErr = alErrNum.Find(x => x.month == "01");
            if (janALL == null)
            {
                aroundALL.Jan = "";
            }
            else
            {
                aroundALL.Jan = janALL.monthnum;
                around.Jan = janErr == null ? "0" : janErr.monthnum;
                aroundRate.Jan = Math.Round((Convert.ToDouble(around.Jan) / Convert.ToDouble(aroundALL.Jan) * 100), 2).ToString();
                qstWr += Convert.ToInt64(around.Jan);
                qstALL += Convert.ToInt64(aroundALL.Jan);
            }


            Model.MonthData febALL = alReportNum.Find(x => x.month == "02");
            Model.MonthData febErr = alErrNum.Find(x => x.month == "02");

            if (febALL == null)
            {
                around.Feb = "";
            }
            else
            {
                aroundALL.Feb = febALL.monthnum;
                around.Feb = febErr == null ? "0" : febErr.monthnum;
                aroundRate.Feb = Math.Round((Convert.ToDouble(around.Feb) / Convert.ToDouble(aroundALL.Feb) * 100), 2).ToString();
                qstWr += Convert.ToInt64(around.Feb);
                qstALL += Convert.ToInt64(aroundALL.Feb);
            }

            Model.MonthData marALL = alReportNum.Find(x => x.month == "03");
            Model.MonthData marErr = alErrNum.Find(x => x.month == "03");
            if (marALL == null)
            {
                around.Mar = "";
            }
            else
            {
                aroundALL.Mar = marALL.monthnum;
                around.Mar = marErr == null ? "0" : marErr.monthnum;
                aroundRate.Mar = Math.Round((Convert.ToDouble(around.Mar) / Convert.ToDouble(aroundALL.Mar) * 100), 2).ToString();
                qstWr += Convert.ToInt64(around.Mar);
                qstALL += Convert.ToInt64(aroundALL.Mar);
            }

            Model.MonthData jarALL = alReportNum.Find(x => x.month == "04");
            Model.MonthData jarpErr = alErrNum.Find(x => x.month == "04");
            if (jarALL == null)
            {
                around.Apr = "";
            }
            else
            {
                aroundALL.Apr = jarALL.monthnum;
                around.Apr = jarpErr == null ? "0" : jarpErr.monthnum;
                aroundRate.Apr = Math.Round((Convert.ToDouble(around.Apr) / Convert.ToDouble(aroundALL.Apr) * 100), 2).ToString();
                qstWr += Convert.ToInt64(around.Apr);
                qstALL += Convert.ToInt64(aroundALL.Apr);
            }

            Model.MonthData mayALL = alReportNum.Find(x => x.month == "05");
            Model.MonthData mayErr = alErrNum.Find(x => x.month == "05");
            if (mayALL == null)
            {
                around.May = "";
            }
            else
            {
                aroundALL.May = mayALL.monthnum;
                around.May = mayErr == null ? "0" : mayErr.monthnum;
                aroundRate.May = Math.Round((Convert.ToDouble(around.May) / Convert.ToDouble(aroundALL.May) * 100), 2).ToString();
                qstWr += Convert.ToInt64(around.May);
                qstALL += Convert.ToInt64(aroundALL.May);
            }

            Model.MonthData junALL = alReportNum.Find(x => x.month == "06");
            Model.MonthData junErr = alErrNum.Find(x => x.month == "06");
            if (junALL == null)
            {
                around.Jun = "";
            }
            else
            {
                aroundALL.Jun = junALL.monthnum;
                around.Jun = junErr == null ? "0" : junErr.monthnum;
                aroundRate.Jun = Math.Round((Convert.ToDouble(around.Jun) / Convert.ToDouble(aroundALL.Jun) * 100), 2).ToString();
                qstWr += Convert.ToInt64(around.Jun);
                qstALL += Convert.ToInt64(aroundALL.Jun);
            }

            Model.MonthData julALL = alReportNum.Find(x => x.month == "07");
            Model.MonthData julErr = alErrNum.Find(x => x.month == "07");
            if (julALL == null)
            {
                around.Jul = "";
            }
            else
            {
                aroundALL.Jul = julALL.monthnum;
                around.Jul = julErr == null ? "0" : julErr.monthnum;
                aroundRate.Jul = Math.Round((Convert.ToDouble(around.Jul) / Convert.ToDouble(aroundALL.Jul) * 100), 2).ToString();
                qstWr += Convert.ToInt64(around.Jul);
                qstALL += Convert.ToInt64(aroundALL.Jul);
            }

            Model.MonthData augALL = alReportNum.Find(x => x.month == "08");
            Model.MonthData augErr = alErrNum.Find(x => x.month == "08");
            if (augALL == null)
            {
                around.Aug = "";
            }
            else
            {
                aroundALL.Aug = julALL.monthnum;
                around.Aug = augErr == null ? "0" : augErr.monthnum;
                aroundRate.Aug = Math.Round((Convert.ToDouble(around.Aug) / Convert.ToDouble(aroundALL.Aug) * 100), 2).ToString();
                qstWr += Convert.ToInt64(around.Aug);
                qstALL += Convert.ToInt64(aroundALL.Aug);
            }

            Model.MonthData sepALL = alReportNum.Find(x => x.month == "09");
            Model.MonthData sepErr = alErrNum.Find(x => x.month == "09");
            if (sepALL == null)
            {
                around.Sep = "";
            }
            else
            {
                aroundALL.Sep = sepALL.monthnum;
                around.Sep = sepErr == null ? "0" : sepErr.monthnum;
                aroundRate.Sep = Math.Round((Convert.ToDouble(around.Sep) / Convert.ToDouble(aroundALL.Sep) * 100), 2).ToString();
                qstWr += Convert.ToInt64(around.Sep);
                qstALL += Convert.ToInt64(aroundALL.Sep);
            }

            Model.MonthData octALL = alReportNum.Find(x => x.month == "10");
            Model.MonthData octErr = alErrNum.Find(x => x.month == "10");
            if (octALL == null)
            {
                around.Oct = "";
            }
            else
            {
                aroundALL.Oct = octALL.monthnum;
                around.Oct = octErr == null ? "0" : octErr.monthnum;
                aroundRate.Oct = Math.Round((Convert.ToDouble(around.Oct) / Convert.ToDouble(aroundALL.Oct) * 100), 2).ToString();
                qstWr += Convert.ToInt64(around.Oct);
                qstALL += Convert.ToInt64(aroundALL.Oct);
            }

            Model.MonthData novALL = alReportNum.Find(x => x.month == "11");
            Model.MonthData novErr = alErrNum.Find(x => x.month == "11");
            if (novALL == null)
            {
                around.Nov = "";
            }
            else
            {
                aroundALL.Nov = novALL.monthnum;
                around.Nov = novErr == null ? "0" : novErr.monthnum;
                aroundRate.Nov = Math.Round((Convert.ToDouble(around.Nov) / Convert.ToDouble(aroundALL.Nov) * 100), 2).ToString();
                qstWr += Convert.ToInt64(around.Nov);
                qstALL += Convert.ToInt64(aroundALL.Nov);
            }

            Model.MonthData decALL = alReportNum.Find(x => x.month == "12");
            Model.MonthData decErr = alErrNum.Find(x => x.month == "12");
            if (decALL == null)
            {
                around.Dec = "";
            }
            else
            {
                aroundALL.Dec = decALL.monthnum;
                around.Dec = decErr == null ? "0" : decErr.monthnum;
                aroundRate.Dec = Math.Round((Convert.ToDouble(around.Dec) / Convert.ToDouble(aroundALL.Dec) * 100), 2).ToString();
                qstWr += Convert.ToInt64(around.Dec);
                qstALL += Convert.ToInt64(aroundALL.Dec);
            }
            around.Qst = qstWr.ToString();
            aroundALL.Qst = qstALL.ToString();
            aroundRate.Qst = Math.Round(Convert.ToDouble(qstWr) / Convert.ToDouble(qstALL) * 100, 2).ToString();
            alShowData.Add(around);
            alShowData.Add(aroundALL);
            alShowData.Add(aroundRate);
            return alShowData;
        }
        /// <summary>
        /// 质控项目变异系数不合格统计
        /// </summary>
        /// <param name="begDate"></param>
        /// <returns></returns>
        private List<Model.QuaShowData> getQCOverNum(string begDate)
        {
            // Model.QuaShowAroundData alShowData = new Model.QuaShowAroundData();
            Model.QuaShowData around = new Model.QuaShowData();
            List<Model.MonthData> alErrNum = NeuLis.DataBase.OperDB.GetMonthQCOverSum(begDate);
            List<Model.MonthData> alReportNum = NeuLis.DataBase.OperDB.GetMonthQCStanderSum(begDate);
            List<Model.QuaShowData> alShowData = new List<Model.QuaShowData>();
            Model.QuaShowData aroundALL = new Model.QuaShowData();
            Model.QuaShowData aroundRate = new Model.QuaShowData();
            long qstErr = 0;//错误数量合计
            long qstAll = 0;//标本总数
            double qstRate = 0;//错误率

            around.TypeID = "6:室内质控项目变异系数不合格率";
            around.Typename = "分子：室内质控项目变异系数高于要求的检验项目数";
            around.Typefx = "≤";
            around.Typemb = "5";
            aroundALL.TypeID = "6:室内质控项目变异系数不合格率";
            aroundALL.Typename = "分母：对室内质控项目变异系数有要求的检验项目总数";
            aroundALL.Typefx = "≤";
            aroundALL.Typemb = "5";
            aroundRate.TypeID = "6:室内质控项目变异系数不合格率";
            aroundRate.Typename = "室内质控项目变异系数不合格率(%)";
            aroundRate.Typefx = "≤";
            aroundRate.Typemb = "5";

            Model.MonthData janALL = alReportNum.Find(x => x.month == "01");
            Model.MonthData janErr = alErrNum.Find(x => x.month == "01");
            if (janALL == null)
            {
                aroundALL.Jan = "";
            }
            else
            {
                aroundALL.Jan = janALL.monthnum;
                around.Jan = janErr == null ? "0" : janErr.monthnum;
                aroundRate.Jan = Math.Round((Convert.ToDouble(around.Jan) / Convert.ToDouble(aroundALL.Jan) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Jan);
                qstAll += Convert.ToInt64(aroundALL.Jan);
            }


            Model.MonthData febALL = alReportNum.Find(x => x.month == "02");
            Model.MonthData febErr = alErrNum.Find(x => x.month == "02");

            if (febALL == null)
            {
                around.Feb = "";
            }
            else
            {
                aroundALL.Feb = febALL.monthnum;
                around.Feb = febErr == null ? "0" : febErr.monthnum;
                aroundRate.Feb = Math.Round((Convert.ToDouble(around.Feb) / Convert.ToDouble(aroundALL.Feb) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Feb);
                qstAll += Convert.ToInt64(aroundALL.Feb);
            }

            Model.MonthData marALL = alReportNum.Find(x => x.month == "03");
            Model.MonthData marErr = alErrNum.Find(x => x.month == "03");
            if (marALL == null)
            {
                around.Mar = "";
            }
            else
            {
                aroundALL.Mar = marALL.monthnum;
                around.Mar = marErr == null ? "0" : marErr.monthnum;
                aroundRate.Mar = Math.Round((Convert.ToDouble(around.Mar) / Convert.ToDouble(aroundALL.Mar) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Mar);
                qstAll += Convert.ToInt64(aroundALL.Mar);
            }

            Model.MonthData jarALL = alReportNum.Find(x => x.month == "04");
            Model.MonthData jarpErr = alErrNum.Find(x => x.month == "04");
            if (jarALL == null)
            {
                around.Apr = "";
            }
            else
            {
                aroundALL.Apr = jarALL.monthnum;
                around.Apr = jarpErr == null ? "0" : jarpErr.monthnum;
                aroundRate.Apr = Math.Round((Convert.ToDouble(around.Apr) / Convert.ToDouble(aroundALL.Apr) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Apr);
                qstAll += Convert.ToInt64(aroundALL.Apr);
            }

            Model.MonthData mayALL = alReportNum.Find(x => x.month == "05");
            Model.MonthData mayErr = alErrNum.Find(x => x.month == "05");
            if (mayALL == null)
            {
                around.May = "";
            }
            else
            {
                aroundALL.May = mayALL.monthnum;
                around.May = mayErr == null ? "0" : mayErr.monthnum;
                aroundRate.May = Math.Round((Convert.ToDouble(around.May) / Convert.ToDouble(aroundALL.May) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.May);
                qstAll += Convert.ToInt64(aroundALL.May);
            }

            Model.MonthData junALL = alReportNum.Find(x => x.month == "06");
            Model.MonthData junErr = alErrNum.Find(x => x.month == "06");
            if (junALL == null)
            {
                around.Jun = "";
            }
            else
            {
                aroundALL.Jun = junALL.monthnum;
                around.Jun = junErr == null ? "0" : junErr.monthnum;
                aroundRate.Jun = Math.Round((Convert.ToDouble(around.Jun) / Convert.ToDouble(aroundALL.Jun) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Jun);
                qstAll += Convert.ToInt64(aroundALL.Jun);
            }

            Model.MonthData julALL = alReportNum.Find(x => x.month == "07");
            Model.MonthData julErr = alErrNum.Find(x => x.month == "07");
            if (julALL == null)
            {
                around.Jul = "";
            }
            else
            {
                aroundALL.Jul = julALL.monthnum;
                around.Jul = julErr == null ? "0" : julErr.monthnum;
                aroundRate.Jul = Math.Round((Convert.ToDouble(around.Jul) / Convert.ToDouble(aroundALL.Jul) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Jul);
                qstAll += Convert.ToInt64(aroundALL.Jul);
            }

            Model.MonthData augALL = alReportNum.Find(x => x.month == "08");
            Model.MonthData augErr = alErrNum.Find(x => x.month == "08");
            if (augALL == null)
            {
                around.Aug = "";
            }
            else
            {
                aroundALL.Aug = julALL.monthnum;
                around.Aug = augErr == null ? "0" : augErr.monthnum;
                aroundRate.Aug = Math.Round((Convert.ToDouble(around.Aug) / Convert.ToDouble(aroundALL.Aug) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Aug);
                qstAll += Convert.ToInt64(aroundALL.Aug);
            }

            Model.MonthData sepALL = alReportNum.Find(x => x.month == "09");
            Model.MonthData sepErr = alErrNum.Find(x => x.month == "09");
            if (sepALL == null)
            {
                around.Sep = "";
            }
            else
            {
                aroundALL.Sep = sepALL.monthnum;
                around.Sep = sepErr == null ? "0" : sepErr.monthnum;
                aroundRate.Sep = Math.Round((Convert.ToDouble(around.Sep) / Convert.ToDouble(aroundALL.Sep) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Sep);
                qstAll += Convert.ToInt64(aroundALL.Sep);
            }

            Model.MonthData octALL = alReportNum.Find(x => x.month == "10");
            Model.MonthData octErr = alErrNum.Find(x => x.month == "10");
            if (octALL == null)
            {
                around.Oct = "";
            }
            else
            {
                aroundALL.Oct = octALL.monthnum;
                around.Oct = octErr == null ? "0" : octErr.monthnum;
                aroundRate.Oct = Math.Round((Convert.ToDouble(around.Oct) / Convert.ToDouble(aroundALL.Oct) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Oct);
                qstAll += Convert.ToInt64(aroundALL.Oct);
            }

            Model.MonthData novALL = alReportNum.Find(x => x.month == "11");
            Model.MonthData novErr = alErrNum.Find(x => x.month == "11");
            if (novALL == null)
            {
                around.Nov = "";
            }
            else
            {
                aroundALL.Nov = novALL.monthnum;
                around.Nov = novErr == null ? "0" : novErr.monthnum;
                aroundRate.Nov = Math.Round((Convert.ToDouble(around.Nov) / Convert.ToDouble(aroundALL.Nov) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Nov);
                qstAll += Convert.ToInt64(aroundALL.Nov);
            }

            Model.MonthData decALL = alReportNum.Find(x => x.month == "12");
            Model.MonthData decErr = alErrNum.Find(x => x.month == "12");
            if (decALL == null)
            {
                around.Dec = "";
            }
            else
            {
                aroundALL.Dec = decALL.monthnum;
                around.Dec = decErr == null ? "0" : decErr.monthnum;
                aroundRate.Dec = Math.Round((Convert.ToDouble(around.Dec) / Convert.ToDouble(aroundALL.Dec) * 100), 2).ToString();
                qstErr += Convert.ToInt64(around.Dec);
                qstAll += Convert.ToInt64(aroundALL.Dec);
            }
            around.Qst = qstErr.ToString();
            aroundALL.Qst = qstAll.ToString();
            aroundRate.Qst = Math.Round(Convert.ToDouble(qstErr) / Convert.ToDouble(qstAll) * 100, 2).ToString();
            alShowData.Add(around);
            alShowData.Add(aroundALL);
            alShowData.Add(aroundRate);
            return alShowData;
        }
        /// <summary>
        /// 获取周转时间
        /// </summary>
        /// <returns></returns>
        private Model.QuaShowData getAroundSap(List<Model.AroundMonthData> aroundData, string patienttype, string typeID, string TypeName, string typeClass="全部")
        {
            Model.QuaShowAroundData alShowData = new Model.QuaShowAroundData();

            List<Model.AroundMonthData> aroundMonthData = aroundData.FindAll(x => x.patientType == patienttype);
            Model.QuaShowData around = new Model.QuaShowData();

            around.PatientType = patienttype;
            around.typeClass = typeClass;
            around.TypeID = typeID+"(" + patienttype + ")";
            around.Typename = TypeName+"(以分钟为单位)";
            around.Typefx = "≤";
            around.Typemb = "";

            Model.AroundMonthData janRej = aroundMonthData.Find(x => x.month == "01");
            if (janRej == null)
            {
                around.Jan = "";
            }
            else
            {
                around.Jan = janRej.monthnum;
            }


            Model.AroundMonthData febRej = aroundMonthData.Find(x => x.month == "02");
            if (febRej == null)
            {
                around.Feb = "";
            }
            else
            {
                around.Feb = febRej.monthnum;
            }

            Model.AroundMonthData marRej = aroundMonthData.Find(x => x.month == "03");
            if (marRej == null)
            {
                around.Mar = "";
            }
            else
            {
                around.Mar = marRej.monthnum;
            }

            Model.AroundMonthData jarpRej = aroundMonthData.Find(x => x.month == "04");
            if (jarpRej == null)
            {
                around.Apr = "";
            }
            else
            {
                around.Apr = jarpRej.monthnum;
            }

            Model.AroundMonthData mayRej = aroundMonthData.Find(x => x.month == "05");
            if (mayRej == null)
            {
                around.May = "";
            }
            else
            {
                around.May = mayRej.monthnum;
            }

            Model.AroundMonthData junRej = aroundMonthData.Find(x => x.month == "06");
            if (junRej == null)
            {
                around.Jun = "";
            }
            else
            {
                around.Jun = junRej.monthnum;
            }

            Model.AroundMonthData julRej = aroundMonthData.Find(x => x.month == "07");
            if (julRej == null)
            {
                around.Jul = "";
            }
            else
            {
                around.Jul = julRej.monthnum;
            }

            Model.AroundMonthData augRej = aroundMonthData.Find(x => x.month == "08");
            if (augRej == null)
            {
                around.Aug = "";
            }
            else
            {
                around.Aug = augRej.monthnum;
            }

            Model.AroundMonthData sepRej = aroundMonthData.Find(x => x.month == "09");
            if (sepRej == null)
            {
                around.Sep = "";
            }
            else
            {
                around.Sep = sepRej.monthnum;
            }

            Model.AroundMonthData octRej = aroundMonthData.Find(x => x.month == "10");
            if (octRej == null)
            {
                around.Oct = "";
            }
            else
            {
                around.Oct = octRej.monthnum;
            }

            Model.AroundMonthData novRej = aroundMonthData.Find(x => x.month == "11");
            if (novRej == null)
            {
                around.Nov = "";
            }
            else
            {
                around.Nov = novRej.monthnum;
            }

            Model.AroundMonthData decRej = aroundMonthData.Find(x => x.month == "12");
            if (decRej == null)
            {
                around.Dec = "";
            }
            else
            {
                around.Dec = decRej.monthnum;
            }

            //alShowData.Add(around);
            return around;
        }

        private void CheckItemType()
        {
            //List<Model.hisitemtype> alhisitemtype = new List<Model.hisitemtype>();
            //alhisitemtype = NeuLis.DataBase.OperDB.checkItem();
            //if(alhisitemtype.Count>0)
            //{
            //    frmHisItemType frm = new frmHisItemType();
            //    frm.alhisitemtype = alhisitemtype;
            //    frm.ShowDialog();
            //}
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            frmItemConfig frm = new frmItemConfig();
            frm.ShowDialog();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.xtraTabControl1.SelectedTabPageIndex == 0)
            {
                if(this.bandedGridView1.RowCount>0)
                {
                    this.ImportExcel(this.gridControl1);
                }
                
            }
            else if (this.xtraTabControl1.SelectedTabPageIndex == 1)
            {
                if (this.bandedGridView2.RowCount > 0)
                {
                    this.ImportExcel(this.gridControl2);
                }
                    
            }
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

        private void Form1_Load(object sender, EventArgs e)
        {
          //  DateTime januaryOfCurrentYear = new DateTime(DateTime.Now.Year, 1, 1);

            this.begDate.Text= DateTime.Now.ToString();
            // this.endDate.Text = DateTime.Now.ToString("yyyy-MM");
            // LoadTreeList();

        }

        private void btnConfig_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(xtraTabControl1.SelectedTabPageIndex ==1)
            {
                frmItemConfig frm = new frmItemConfig();
                frm.ShowDialog();
            }
        }

        private void btnExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 双击单元格传入数据进入表格展示数据 Added By 徐振宇 2026年7月14日19:41:30
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bandedGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                // 1. 安全检查：表格是否有数据
                if (this.bandedGridView1.RowCount <= 0)
                    return;

                // 2. 获取鼠标点击位置
                GridHitInfo HitInfo = bandedGridView1.CalcHitInfo(e.Location);

                // 3. 校验：必须是双击单元格
                if (!HitInfo.InRowCell || HitInfo.Column == null || e.Button != MouseButtons.Left || e.Clicks != 2)
                    return;

                // 4. 获取行数据，并做空值判断
                Model.QuaShowData showData = this.bandedGridView1.GetFocusedRow() as Model.QuaShowData;
                if (showData == null)
                    return;

                NeuLis.Models.NeulisDictionary dic = new NeuLis.Models.NeulisDictionary();

                // 5. 判断指标类型
                string kind = "不合格";
                string patientType = "";

                // 优先判断更精确的条件
                if (showData.Typename != null && showData.Typename.IndexOf("危急值报告时间中位数") >= 0)
                {
                    kind = "危急值报告时间中位数";
                    patientType = showData.PatientType;
                }
                else if (showData.Typename != null && showData.Typename.IndexOf("危急值") >= 0)
                {
                    kind = "危急值";
                }
                else if (showData.TypeID != null && showData.TypeID.IndexOf("检验报告不正确") >= 0)
                {
                    kind = "检验报告不正确";
                }
                else if (showData.TypeID != null && showData.TypeID.IndexOf("血培养污染") >= 0)
                {
                    kind = "血培养";
                }
                else if (showData.TypeID != null && showData.TypeID.IndexOf("质控项目变异系数") >= 0)
                {
                    kind = "质控";
                }
                else if (showData.TypeID != null && showData.TypeID.IndexOf("总拒收率") >= 0)
                {
                    kind = "总拒收率";
                }

                // 6. 安全获取月份值
                string columnName = HitInfo.Column.FieldName;
                if (!dic.montDic.ContainsKey(columnName))
                    return;

                // 7. 打开明细查询窗体
                frmQueryList frm = new frmQueryList();
                frm.typeid = showData.TypeID;
                frm.typename = showData.Typename;
                frm.month = this.labelControl4.Text + dic.montDic[columnName];
                frm.kind = kind;
                frm.patienttype = patientType;
                frm.WindowState = FormWindowState.Maximized; //最大化
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                // 捕获所有异常，防止程序崩溃
                MessageBox.Show($"操作失败：{ex.Message}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        /// <summary>
        /// 周转时间查询清单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bandedGridView2_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (this.bandedGridView2.RowCount <= 0)
                    return;
                GridHitInfo HitInfo = bandedGridView2.CalcHitInfo(e.Location);//获取鼠标点击的位置
                NeuLis.Models.NeulisDictionary dic = new NeuLis.Models.NeulisDictionary();
                if (HitInfo.InRowCell && HitInfo.Column != null && e.Button == MouseButtons.Left && e.Clicks == 2)
                {
                    Model.QuaShowData showData = (Model.QuaShowData)this.bandedGridView2.GetFocusedRow();

                    string typename = showData.Typename;
                    string month = HitInfo.Column.FieldName;
                    string typeid = showData.TypeID;//指标类型
                    string patienttype = showData.PatientType;//患者类型
                    string typeclass = showData.typeClass;//项目类别，如生化，三大常规等。

                    string kind = "TAT";
                    if (!string.IsNullOrEmpty(typeid) && typeid.IndexOf("TAT_P90") >= 0)
                    {
                        kind = "TAT_P90";
                    }

                    frmQueryList frm = new frmQueryList();
                    frm.typeid = typeid;
                    frm.typename = typename;
                    frm.patienttype = patienttype;
                    frm.typeclass = typeclass;
                    frm.month = this.labelControl2.Text + dic.montDic[month];
                    frm.kind = kind;
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败：{ex.Message}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
           
        }
        /// <summary>
        /// 树形结构加载仪器列表
        /// </summary>
        private void LoadTreeList()
        {
            try
            {
                //构建一个DataTable数据源
                DataTable table = new DataTable();
                table.Columns.Add("parentId");
                table.Columns.Add("Id");
                table.Columns.Add("parentName");
                table.Columns.Add("Name");
                DataRow row = table.NewRow();
                row["parentId"] = "";
                row["Id"] = "*";
                row["Name"] = "所有颜色";
                table.Rows.Add(row);
                row = table.NewRow();
                row["parentId"] = "*";
                row["Id"] = "1";
                row["Name"] = "红色";
                table.Rows.Add(row);
                row = table.NewRow();
                row["parentId"] = "*";
                row["Id"] = "2";
                row["Name"] = "黄色";
                table.Rows.Add(row);
                row = table.NewRow();
                row["parentId"] = "*";
                row["Id"] = "3";
                row["Name"] = "绿色";
                table.Rows.Add(row);
                row = table.NewRow();
                row["parentId"] = "1";
                row["Id"] = "01";
                row["Name"] = "粉红色";
                table.Rows.Add(row);
                row = table.NewRow();
                row["parentId"] = "2";
                row["Id"] = "02";
                row["Name"] = "鹅黄色";
                table.Rows.Add(row);
                treeList1.ParentFieldName = "parentId";
                treeList1.KeyFieldName = "Id";
                treeList1.DataSource = table;
                treeList1.ExpandAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void treeList1_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            try
            {
                SetCheckedChildNodes(e.Node, e.Node.CheckState);
                SetCheckedParentNodes(e.Node, e.Node.CheckState);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 设置子节点的状态
        /// </summary>
        /// <param name="node"></param>
        /// <param name="check"></param>
        private void SetCheckedChildNodes(TreeListNode node, CheckState check)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                node.Nodes[i].CheckState = check;
                SetCheckedChildNodes(node.Nodes[i], check);
            }
        }

        /// <summary>
        /// 设置父节点的状态
        /// </summary>
        /// <param name="node"></param>
        /// <param name="check"></param>
        private void SetCheckedParentNodes(TreeListNode node, CheckState check)
        {
            if (node.ParentNode != null)
            {
                bool b = false;
                CheckState state;
                for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
                {
                    state = (CheckState)node.ParentNode.Nodes[i].CheckState;
                    if (!check.Equals(state))
                    {
                        b = !b;
                        break;
                    }
                }
                node.ParentNode.CheckState = b ? CheckState.Checked : check;
                SetCheckedParentNodes(node.ParentNode, check);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeList1_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
        }
    }
}
