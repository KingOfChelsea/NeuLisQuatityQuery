using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeuLis.DataBase
{
    public class OperDB
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<Models.Model.RejectReason> GetRejectReason()
        {
            List<Models.Model.RejectReason> alReason = new List<Models.Model.RejectReason>();
            string strSql = @"select a.dicname as reason, nvl( a.memo3,a.dicname) as memo3, a.showorder
                              from las_sys_dictionary a
                             where a.typeid = 'SampleRejectReason'
                             and a.dicname is not null
                             and a.isshow ='1'
                             order by a.showorder";
            alReason = OracleHelp.QueryListByReflect<Models.Model.RejectReason>(strSql);
            return alReason;
        }
        /// <summary>
        /// 不合格样本
        /// </summary>
        /// <param name="begDate"></param>
        /// <returns></returns>
       public static List<Models.Model.MonthData> GetMonthNum(string begDate)//,string reason
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"select substr(t2.regdate, 5, 2) as month,t2.reason, to_char(count(t2.barcode)) as monthnum
                              from las_sap_samplereject t2
                             where  substr(t2.regdate, 1, 4) >= '{begDate}'

                             group by substr(t2.regdate, 5, 2),t2.reason ";
            //t2.reason = '{reason}'
             
            month = OracleHelp.QueryListByEmit<Models.Model.MonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 根据类别和时间查询不合格标本列表
        /// </summary>
        /// <param name="typeid"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<Models.Model.sampleReject> getRejectSampleList(string typeid,string month)
        {
            List<Models.Model.sampleReject> alSapRej = new List<Models.Model.sampleReject>();
            string strSql = $@"select a.regdate,a.barcode,a.patientid,a.patientname,a.sampletype,a.hisitemnamelist,a.reason,a.opername
                            from las_sap_samplereject a
                            where substr(a.regdate,1,6)='{month}'
                            and a.reason='{typeid}'";
            alSapRej = OracleHelp.QueryListByEmit<Models.Model.sampleReject>(strSql);
            return alSapRej;

        }
        /// <summary>
        /// 查询月标本总数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<Models.Model.MonthData> GetMonthSapSum(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"select to_char(t.APPROVETIME, 'mm') as month,
                               to_char(count(distinct t.barcode)) as monthnum
                          from view_las_sap_samplereg t
                         where to_char(t.APPROVETIME, 'yyyy') >= '{begDate}'
                         group by to_char(t.APPROVETIME, 'mm') ";

            month = OracleHelp.QueryListByEmit<Models.Model.MonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 查询月抗凝标本总数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<Models.Model.MonthData> GetMonthKNSapSum(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"select to_char(t.APPROVETIME, 'mm') as month,
                               to_char(count(distinct t.barcode)) as monthnum
                          from view_las_sap_samplereg t,
                               las_sap_sampleitem     a,
                               las_stat_itemconfig    b
                         where to_char(t.APPROVETIME, 'yyyy') >= '{begDate}'
                           and t.machineid = a.machineid
                           and t.sampleid = a.sampleid
                           and t.testdate = a.testdate
                           and a.hisitemid = b.hisitemid
                           and b.classtype = '抗凝标本类'
                         group by to_char(t.APPROVETIME, 'mm') ";

            month = OracleHelp.QueryListByEmit<Models.Model.MonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 查询标本清单
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable getBarcodeList(string month)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                           select a.barcode as 条码号,
                               to_char(a.sampletime, 'yyyy-MM-dd hh24:mi:ss') as 采集时间,
                               a.patienttype as 患者类型,
                               a.patientid as 患者编号,
                               a.patientname as 姓名,
                               a.sampletype as 样本类型,
                               a.hisitemnamelist as 检验目的
                          from view_las_sap_samplereg a
                         where to_char(a.APPROVETIME, 'yyyyMM') = '{month}'";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询抗凝标本清单
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable getKNBarcodeList(string month)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                           select a.barcode as 条码号,
                            to_char(a.sampletime, 'yyyy-MM-dd hh24:mi:ss') as 采集时间,
                            a.patienttype as 患者类型,
                            a.patientid as 患者编号,
                            a.patientname as 姓名,
                            a.sampletype as 样本类型,
                            a.HISITEMNAMELIST as 检验目的
                        from view_las_sap_samplereg a,
                            las_stat_itemconfig    b,
                            las_sap_sampleitem     c
                        where to_char(a.APPROVETIME, 'yyyyMM') = '{month}'
                        and a.machineid = c.machineid
                        and a.testdate = c.testdate
                        and a.sampleid = c.sampleid
                        and b.classtype = '抗凝标本类'
                        and c.hisitemid = b.hisitemid";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询不正确报告单数量
        /// </summary>
        /// <param name="begDate"></param>
        /// <returns></returns>
        public static List<Models.Model.MonthData> getErrReport(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"
                               select  to_char(a.applytime,'MM') as month,to_char(count( a.barcode)) as monthnum
                              from las_sap_samplecancel a,view_las_sap_samplereg b
                             where to_char(a.applytime, 'yyyy') = '{begDate}'
                             and to_char( a.approvetime,'yyyyMMdd') <>'00010101'
                              and a.approvetime is not null
                               and (a.applytime - a.approvetime) * 24 * 60 > 10
                               and a.machineid = b.machineid
                               and a.testdate = b.testdate
                               and a.sampleid = b.sampleid
                               and b.CONFIRMSTATE='1'
                               group by to_char(a.applytime,'MM')";
            month = OracleHelp.QueryListByEmit<Models.Model.MonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 查询月报告总数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<Models.Model.MonthData> GetMonthReportSum(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"select to_char(t.approvetime, 'mm') as month, to_char( count(1)) as monthnum
                              from view_las_sap_samplereg t
                             where to_char(t.approvetime, 'yyyy') >= '{begDate}'
         
                             group by to_char(t.approvetime, 'mm') ";

            month = OracleHelp.QueryListByEmit<Models.Model.MonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 查询报告单清单
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<Models.Model.barcodeReg> getReportList(string month)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                            select a.barcode,
                                   to_char(a.approvetime,'yyyy-MM-dd hh24:mi:ss') as sampletime,
                                   a.patienttype,
                                   a.patientid,
                                   a.patientname,
                                   a.sampletype,
                                   hisitemnamelist
                              from view_las_sap_samplereg a
                              where to_char(a.approvetime,'yyyyMM')='{month}' ";
            alBarcodeReg = OracleHelp.QueryListByEmit<Models.Model.barcodeReg>(strSql);
            return alBarcodeReg;
        }
        /// <summary>
        /// 查询不正确报告单清单
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable getErrReportList(string month)
        {
            List<Models.Model.errSampleReg> alBarcodeReg = new List<Models.Model.errSampleReg>();
            string strSql = $@"                           
                       select a.barcode as 条码号,
                       a.machineid as 检测仪器,
                       a.testdate as 检验日期,
                       a.sampleid as 样本号,
                       b.PATIENTTYPE as 患者类型,
                       b.PATIENTID as 患者编号,
                       b.PATIENTNAME as 姓名,
                       b.DEPTNAME as 申请科室,
                       b.HISITEMNAMELIST as 检验目的,
                       to_char(a.approvetime, 'yyyy-MM-dd hh24:mi:ss') as 上传审核时间,
                       a.approvername as 上次审核人,
                       to_char(a.applytime, 'yyyy-MM-dd hh24:mi:ss') as 取消审核时间,
                       a.applyname as 取消审核人,
                       to_char(b.APPROVETIME, 'yyyy-MM-dd hh24:mi:ss') as 最终审核时间,
                       b.APPROVERNAME as 终审人
                  from las_sap_samplecancel a, view_las_sap_samplereg b
                 where to_char(a.applytime, 'yyyymm') = '{month}'
                   and to_char(a.approvetime, 'yyyyMMdd') <> '00010101'
                   and a.approvetime is not null
                   and (a.applytime - a.approvetime) * 24 * 60 > 10
                   and a.machineid = b.machineid
                   and a.testdate = b.testdate
                   and a.sampleid = b.sampleid
                   and b.CONFIRMSTATE = '1'
                 order by a.barcode";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 已经打电话的危急值数量
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<Models.Model.MonthData> GetMonthPhoneAlterSum(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"select to_char(t.dealdate, 'MM') as month, to_char(count(1)) as monthnum
                              from las_sap_lifealert t, view_las_sap_samplereg b
                             where to_char(t.dealdate, 'yyyy') >= '{begDate}'
                               and t.machineid = b.machineid
                               and t.testdate= b.testdate
                               and t.sampleid = b.sampleid
                               and b.CONFIRMSTATE='1'
                               and b.EXECDEPTID  ='60100'    
                               and phoneanswer is not null
                             group by to_char(t.dealdate, 'MM')";

            month = OracleHelp.QueryListByEmit<Models.Model.MonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 查询危急值清单列表
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable getPhoneAlterList(string month)
        {
            List<Models.Model.lifeAlter> alAlters = new List<Models.Model.lifeAlter>();
            string strSql = $@"select t.barcode as 条码号,
                           b.PATIENTTYPE as 患者类型,
                           b.PATIENTID as 患者编号,
                           b.PATIENTNAME as 姓名,
                           b.DEPTNAME as 申请科室,
                           b.HISITEMNAMELIST as 检验目的,
                           to_char(dealdate, 'yyyy-MM-dd hh24:mi:ss') as 发生时间,
                           itemid as 项目编码,
                           itemname as 项目名称,
                           reportvalue as 结果,
                           rangeinfo as 参考范围,
                           unit as 单温,
                           rangelimit as 极限范围,
                           t.machineid as 检测仪器,
                           t.testdate as 检验日期,
                           t.sampleid as 样本哈,
                           to_char(t.sendtime, 'yyyy-MM-dd hh24:mi:ss') as 发送时间,
                           sendname as 发送人,
                           to_char(phonetime, 'yyyy-MM-dd hh24:mi:ss') as 电话时间,
                           phoneanswer 接电话人
                          from las_sap_lifealert t, view_las_sap_samplereg b
                         where to_char(t.dealdate, 'yyyyMM') = '{month}'
                         and t.machineid = b.machineid
                         and t.testdate = b.testdate
                         and t.sampleid = b.sampleid
                         and b.CONFIRMSTATE ='1'
                         and b.EXECDEPTID='60100'
                         and phoneanswer is not null";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询危急值总数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<Models.Model.MonthData> GetMonthLifeAlterSum(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"select to_char(t.dealdate, 'MM') as month, to_char(count(1)) as monthnum
                          from las_sap_lifealert t,view_las_sap_samplereg b
                         where to_char(t.dealdate, 'yyyy') >= '{begDate}'
                        and t.machineid = b.machineid
                        and t.testdate = b.testdate
                        and t.sampleid = b.sampleid
                        and b.CONFIRMSTATE='1'
                        and b.EXECDEPTID = '60100'
                         group by to_char(t.dealdate, 'MM') ";

            month = OracleHelp.QueryListByEmit<Models.Model.MonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 查询危急值清单列表
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable getLifeAlterList(string month)
        {
            //DataTable dt = new DataTable();
            string strSql = $@"select a.barcode as 条码号,
                               b.machineid as 仪器编码,
                               b.testdate as 检测日期,
                               b.sampleid as 样本号,
                               b.PATIENTTYPE as 患者类型,
                               b.PATIENTID as 患者编号,
                               b.PATIENTNAME as 姓名,
                               b.DEPTNAME as 申请科室,
                               b.HISITEMNAMELIST as 检验目的,
                               a.itemid as 项目编码,
                               a.itemname as 项目名称,
                               a.reportvalue as 结果,
                               a.unit as 单位,
                               a.rangeinfo as 参考范围,
                               a.rangelimit as 极限范围,
                               b.APPROVETIME as 审核时间,
                               a.phonetime as 电话通报时间
                          from las_sap_lifealert a, view_las_sap_samplereg b
                         where a.machineid = b.machineid
                           and a.testdate = b.testdate
                           and a.sampleid = b.sampleid
                           and b.CONFIRMSTATE = '1'
                           and EXECDEPTID = '60100'
                            and to_char(a.dealdate, 'yyyyMM') = '{month}'";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 危急值及时率
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<Models.Model.MonthData> GetMonthJSAlterSum(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"select  to_char(b.APPROVETIME,'MM') as month,to_char(count(1)) as monthnum
                              from las_sap_lifealert a, view_las_sap_samplereg b
                             where a.machineid = b.machineid
                               and a.testdate = b.testdate
                               and a.sampleid = b.sampleid
                               and b.CONFIRMSTATE='1'
                               and to_char(b.APPROVETIME,'yyyy')='{begDate}'
                              and ABS(b.APPROVETIME-a.phonetime)*24*60 <=5
                              and  EXECDEPTID = '60100'
                            group by  to_char(b.APPROVETIME,'MM') ";

            month = OracleHelp.QueryListByEmit<Models.Model.MonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 血培养污染数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<Models.Model.MonthData> GetMonthXPYWRSum(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"select substr(t.testdate,5,2) as month,to_char(nvl(sum(t.pollute),0)) as monthnum
                            from LAS_GM_GNDRESULTGERM t,las_gm_samplereg a
                            where substr(t.testdate,1,4)='{begDate}'
                            and t.machineid = a.machineid
                            and t.testdate = a.testdate
                            and t.sampleid = a.sampleid
                            and a.testtype='血培养'
                            and a.hisitemidlist like '%F000030199%'
                            group by substr(t.testdate,5,2)
                             ";

            month = OracleHelp.QueryListByEmit<Models.Model.MonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 血培养数量
        /// </summary>
        /// <param name="begDate"></param>
        /// <returns></returns>
        public static List<Models.Model.MonthData> GetMonthXPYSum(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"select substr(a.testdate,5,2) as month,to_char(nvl(count(a.barcode),0)) as monthnum
                                from las_gm_samplereg a
                                where substr(a.testdate,1,4)='{begDate}'
                                and a.testtype='血培养'
                                and a.hisitemidlist like '%F000030199%'
                                group by substr(a.testdate,5,2)  ";

            month = OracleHelp.QueryListByEmit<Models.Model.MonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 血培养数量
        /// </summary>
        /// <param name="begDate"></param>
        /// <returns></returns>
        public  DataTable GetMonthXPYList(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"select a.barcode as 条码号,
                               a.patienttype as 患者类型,
                               a.patientid as 患者编号,
                               a.patientname as 姓名,
                               a.deptname as 申请科室,
                               a.hisitemnamelist as 检验目的,
                               a.sampletype as 样本类型,
                               to_char(a.sampletime,'yyyy-MM-dd hh24:mi:ss') as 采集时间,
                               to_char(a.incepttime,'yyyy-MM-dd hh24:mi:ss') as 接收时间,
                               to_char(a.approvetime,'yyyy-MM-dd hh24:mi:ss') as 审核时间
                          from las_gm_samplereg a
                         where substr(a.testdate, 1, 6) = '{begDate}'
                           and a.testtype = '血培养'  and a.hisitemidlist like '%F000030199%'
                         ";

            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 血培养污染数量
        /// </summary>
        /// <param name="begDate"></param>
        /// <returns></returns>
        public DataTable GetMonthXPYWRList(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"select a.barcode         as 条码号,
                               a.patienttype     as 患者类型,
                               a.patientid       as 患者编号,
                               a.patientname     as 姓名,
                               a.deptname        as 申请科室,
                               a.hisitemnamelist as 检验目的,
                               a.sampletype      as 样本类型,
                                to_char(a.sampletime,'yyyy-MM-dd hh24:mi:ss') as 采集时间,
                               to_char(a.incepttime,'yyyy-MM-dd hh24:mi:ss') as 接收时间,
                               to_char(a.approvetime,'yyyy-MM-dd hh24:mi:ss') as 审核时间
                          from las_gm_samplereg a, LAS_GM_GNDRESULTGERM b
                         where substr(a.testdate, 1, 6) = '{begDate}'
                           and a.machineid = b.machineid
                           and a.testdate = b.testdate
                           and a.sampleid = b.sampleid
                           and b.pollute = '1'
                           and a.testtype = '血培养' ";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 质控变异系数不合格数量
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<Models.Model.MonthData> GetMonthQCOverSum(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"
                            select substr(t2.qcmonth,5,2) as month,to_char(count(1)) as monthnum
                              from las_qc_monthdata t2,
                                   (
        
                                    select a.machineid,
                                            a.itemid,
                                            '1' as qclevel,
                                            greatest(to_number(nvl(a.tealevel1, 0)),
                                                     to_number(nvl(a.maxerrlevel1, 0)),
                                                     to_number(nvl(a.targetcv, 0))) as maxalue
                                      from LAS_QC_ITEMTEA a
                                    union all
                                    select a.machineid,
                                           a.itemid,
                                           '2' as qclevel,
                                           greatest(to_number(nvl(a.tealevel2, 0)),
                                                    to_number(nvl(a.maxerrlevel2, 0)),
                                                    to_number(nvl(a.targetcv2, 0))) as maxalue
                                      from LAS_QC_ITEMTEA a
                                    union all
                                    select a.machineid,
                                           a.itemid,
                                           '3' as qclevel,
                                           greatest(to_number(nvl(a.tealevel3, 0)),
                                                    to_number(nvl(a.maxerrlevel3, 0)),
                                                    to_number(nvl(a.targetcv3, 0))) as maxalue
                                      from LAS_QC_ITEMTEA a) t1
                             where t1.maxalue > 0
                               and t2.machineid = t1.machineid
                               and t2.itemid = t1.itemid
                               and t2.qclevel = t1.qclevel
                               and t2.cvavg > t1.maxalue
                               and substr(t2.qcmonth,1,4)='{begDate}'
                               and REGEXP_LIKE(t2.cv, '\d')
                             group by t2.qcmonth ";

            month = OracleHelp.QueryListByEmit<Models.Model.MonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 质控项目变异系数不合格清单
        /// </summary>
        /// <param name="begDate"></param>
        /// <returns></returns>
        public DataTable GetMonthQCoverList(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"
                        select t2.machineid as 检验仪器,
                               t2.qcmonth   as 质控月份,
                                t3.qcid as 批号,
                               t2.itemid    as 项目编码,
                               t2.qclevel   as 质控水平,
                               t2.cvavg        as 当月实测CV,
                              t1.targetcv as 目标CV,
                              t1.tealevel as TEa,
                              t1.maxerrlevel as CV最大允许误差,
                               t1.maxalue   as 最大允许系数
                          from las_qc_monthdata t2,las_qc_info t3,
                               (
        
                                select a.machineid,
                                        a.itemid,
                                        '1' as qclevel,
                                        a.targetcv,
                                        a.tealevel1 as tealevel,
                                        a.maxerrlevel1 as maxerrlevel,
                                        greatest(to_number(nvl(a.tealevel1, 0)),
                                                 to_number(nvl(a.maxerrlevel1, 0)),
                                                 to_number(nvl(a.targetcv, 0))) as maxalue
                                  from LAS_QC_ITEMTEA a
                                union all
                                select a.machineid,
                                       a.itemid,
                                       '2' as qclevel, 
                                        a.targetcv2 as targetcv,
                                       a.tealevel2 as tealevel,
                                       a.maxerrlevel2 as maxerrlevel,
                                       greatest(to_number(nvl(a.tealevel2, 0)),
                                                to_number(nvl(a.maxerrlevel2, 0)),
                                                to_number(nvl(a.targetcv2, 0))) as maxalue
                                  from LAS_QC_ITEMTEA a
                                union all
                                select a.machineid,
                                       a.itemid,
                                       '3' as qclevel, 
                                a.targetcv3 as targetcv,
                               a.tealevel3 as tealevel,
                               a.maxerrlevel3 as maxerrlevel,
                                       greatest(to_number(nvl(a.tealevel3, 0)),
                                                to_number(nvl(a.maxerrlevel3, 0)),
                                                to_number(nvl(a.targetcv3, 0))) as maxalue
                                  from LAS_QC_ITEMTEA a) t1
                         where t1.maxalue > 0
                           and t2.machineid = t1.machineid
                           and t2.itemid = t1.itemid
                           and t2.qclevel = t1.qclevel
                           and t2.cvavg > t1.maxalue
                           and t2.machineid = t3.machineid
                           
                           and t2.batch = t3.batch
                           and t2.qcmonth = '{begDate}'
                           and REGEXP_LIKE(t2.cv, '\d')";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 维护了质控变异系统的项目
        /// </summary>
        /// <param name="begDate"></param>
        /// <returns></returns>
        public static List<Models.Model.MonthData> GetMonthQCStanderSum(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"
                            select substr(t2.qcmonth,5,2) as month,to_char(count(1)) as monthnum
                              from las_qc_monthdata t2,
                                   (
        
                                    select a.machineid,
                                            a.itemid,
                                            '1' as qclevel,
                                            greatest(to_number(nvl(a.tealevel1, 0)),
                                                     to_number(nvl(a.maxerrlevel1, 0)),
                                                     to_number(nvl(a.targetcv, 0))) as maxalue
                                      from LAS_QC_ITEMTEA a
                                    union all
                                    select a.machineid,
                                           a.itemid,
                                           '2' as qclevel,
                                           greatest(to_number(nvl(a.tealevel2, 0)),
                                                    to_number(nvl(a.maxerrlevel2, 0)),
                                                    to_number(nvl(a.targetcv2, 0))) as maxalue
                                      from LAS_QC_ITEMTEA a
                                    union all
                                    select a.machineid,
                                           a.itemid,
                                           '3' as qclevel,
                                           greatest(to_number(nvl(a.tealevel3, 0)),
                                                    to_number(nvl(a.maxerrlevel3, 0)),
                                                    to_number(nvl(a.targetcv3, 0))) as maxalue
                                      from LAS_QC_ITEMTEA a) t1
                             where t1.maxalue > 0
                               and t2.machineid = t1.machineid
                               and t2.itemid = t1.itemid
                               and t2.qclevel = t1.qclevel
                               and substr(t2.qcmonth,1,4)='{begDate}'
                               and REGEXP_LIKE(t2.cv, '\d')
                             group by t2.qcmonth ";

            month = OracleHelp.QueryListByEmit<Models.Model.MonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 维护了质控项目变异系数项目清单
        /// </summary>
        /// <param name="begDate"></param>
        /// <returns></returns>
        public DataTable GetMonthQCStanderList(string begDate)
        {
            List<Models.Model.MonthData> month = new List<Models.Model.MonthData>();
            string strSql = $@"
                        select t2.machineid as 检验仪器,
                               t2.qcmonth   as 质控月份,
                                t3.qcid as 批号,
                               t2.itemid    as 项目编码,
                               t2.qclevel   as 质控水平,
                               t2.cvavg        as 当月实测CV,
                              t1.targetcv as 目标CV,
                              t1.tealevel as TEa,
                              t1.maxerrlevel as CV最大允许误差,
                               t1.maxalue   as 最大允许系数
                          from las_qc_monthdata t2,las_qc_info t3,
                               (
        
                                select a.machineid,
                                        a.itemid,
                                        '1' as qclevel,
                                        a.targetcv,
                                        a.tealevel1 as tealevel,
                                        a.maxerrlevel1 as maxerrlevel,
                                        greatest(to_number(nvl(a.tealevel1, 0)),
                                                 to_number(nvl(a.maxerrlevel1, 0)),
                                                 to_number(nvl(a.targetcv, 0))) as maxalue
                                  from LAS_QC_ITEMTEA a
                                union all
                                select a.machineid,
                                       a.itemid,
                                       '2' as qclevel, 
                                        a.targetcv2 as targetcv,
                                       a.tealevel2 as tealevel,
                                       a.maxerrlevel2 as maxerrlevel,
                                       greatest(to_number(nvl(a.tealevel2, 0)),
                                                to_number(nvl(a.maxerrlevel2, 0)),
                                                to_number(nvl(a.targetcv2, 0))) as maxalue
                                  from LAS_QC_ITEMTEA a
                                union all
                                select a.machineid,
                                       a.itemid,
                                       '3' as qclevel, 
                                a.targetcv3 as targetcv,
                               a.tealevel3 as tealevel,
                               a.maxerrlevel3 as maxerrlevel,
                                       greatest(to_number(nvl(a.tealevel3, 0)),
                                                to_number(nvl(a.maxerrlevel3, 0)),
                                                to_number(nvl(a.targetcv3, 0))) as maxalue
                                  from LAS_QC_ITEMTEA a) t1
                         where t1.maxalue > 0
                           and t2.machineid = t1.machineid
                           and t2.itemid = t1.itemid
                           and t2.qclevel = t1.qclevel
                           and t2.machineid = t3.machineid
                           and t2.batch = t3.batch
                           and t2.qcmonth = '{begDate}'
                           and REGEXP_LIKE(t2.cv, '\d')";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询特危急值及时清单
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public DataTable GetMonthJSAlterList(string month)
        {
            string strSql = $@"
                       select a.barcode as 条码号,
                       b.machineid as 仪器编码,
                       b.testdate as 检测日期,
                       b.sampleid as 样本号,
                       b.PATIENTTYPE as 患者类型,
                       b.PATIENTID as 患者编号,
                       b.PATIENTNAME as 姓名,
                       b.DEPTNAME as 申请科室,
                       b.HISITEMNAMELIST as 检验目的,
                       a.itemid as 项目编码,
                       a.itemname as 项目名称,
                       a.reportvalue as 结果,
                       a.unit as 单位,
                       a.rangeinfo as 参考范围,
                       a.rangelimit as 极限范围,
                       to_char( b.APPROVETIME,'yyyy-MM-dd hh24:mi:ss') as 审核时间,
                       to_char(a.phonetime,'yyyy-MM-dd hh24:mi:ss') as 电话通报时间
                  from las_sap_lifealert a, view_las_sap_samplereg b
                 where a.machineid = b.machineid
                   and a.testdate = b.testdate
                   and a.sampleid = b.sampleid
                   and b.CONFIRMSTATE = '1'
                   and to_char(b.INCEPTTIME, 'yyyyMM') = '{month}'
                   and ABS(b.APPROVETIME - a.phonetime) * 24 * 60 > 5";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 检验前中位数
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static List<Models.Model.AroundMonthData> GetAroundJYQ(string year)
        {
            List<Models.Model.AroundMonthData> month = new List<Models.Model.AroundMonthData>();
            string strSql = $@"select to_char(T.INCEPTTIME, 'MM') as month, T.PATIENTTYPE ,to_char( median(arountime)) as monthnum
                              from (
        
                                    select distinct a.BARCODE,
                                                     decode(a.EMC, '1', '加急', a.PATIENTTYPE) as PATIENTTYPE,
                                                     a.SAMPLETIME,
                                                     a.INCEPTTIME,
                                                     round（(a.INCEPTTIME - a.SAMPLETIME) * 24 * 60,
                                                     0） as arountime
                                      from view_las_sap_samplereg a
                                     where to_char(a.INCEPTTIME, 'yyyy') = '{year}' 
                                      and confirmstate='1'
                                      and to_char(a.SAMPLETIME,'yyyy')>'1990'
                                       ) T
                             group by to_char(T.INCEPTTIME, 'MM'), T.PATIENTTYPE";
            month = OracleHelp.QueryListByEmit<Models.Model.AroundMonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 检验前中位数，分检验类别
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static List<Models.Model.AroundMonthData> GetAroundJYQbyType(string year)
        {
            List<Models.Model.AroundMonthData> month = new List<Models.Model.AroundMonthData>();
            string strSql = $@"select to_char(T.INCEPTTIME, 'MM') as month,
                                T.PATIENTTYPE,
                                T.typename as classType,
                                to_char(median(arountime)) as monthnum
                            from (
                                select c.typename,
                                        a.BARCODE,
                                        decode(a.EMC, '1', '加急', a.PATIENTTYPE) as PATIENTTYPE,
                                        a.SAMPLETIME,
                                        a.INCEPTTIME,
                                        round((a.INCEPTTIME - a.SAMPLETIME) * 24 * 60, 0) as arountime
                                    from view_las_sap_samplereg a,
                                        las_sap_sampleitem     b,
                                        las_stat_itemconfig    c
                                    where a.machineid = b.machineid
                                    and a.testdate = b.testdate
                                    and a.sampleid = b.sampleid
                                    and b.hisitemid = c.hisitemid
                                    and to_char(a.INCEPTTIME, 'yyyy') = '{year}' and a.confirmstate='1'
                                   and to_char(a.SAMPLETIME,'yyyy')>'1990') T
                            group by to_char(T.INCEPTTIME, 'MM'), T.PATIENTTYPE, T.typename";
            month = OracleHelp.QueryListByEmit<Models.Model.AroundMonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 室内中位数
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static List<Models.Model.AroundMonthData> GetAroundSN(string year)
        {
            List<Models.Model.AroundMonthData> month = new List<Models.Model.AroundMonthData>();
            string strSql = $@"select to_char(T.INCEPTTIME, 'MM') as month, T.PATIENTTYPE ,to_char( median(arountime)) as monthnum
                              from (
        
                                    select distinct a.BARCODE,
                                                     decode(a.EMC, '1', '加急', a.PATIENTTYPE) as PATIENTTYPE,
                                                     a.INCEPTTIME,
                                                     a.APPROVETIME,
                                                     round（(a.APPROVETIME - a.INCEPTTIME) * 24 * 60,
                                                     0） as arountime
                                      from view_las_sap_samplereg a
                                     where to_char(a.INCEPTTIME, 'yyyy') = '{year}' and confirmstate='1' and to_char(a.SAMPLETIME,'yyyy')>'1990') T
                             group by to_char(T.INCEPTTIME, 'MM'), T.PATIENTTYPE";
            month = OracleHelp.QueryListByEmit<Models.Model.AroundMonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 室内中位数，分检验类别
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static List<Models.Model.AroundMonthData> GetAroundSNbyType(string year)
        {
            List<Models.Model.AroundMonthData> month = new List<Models.Model.AroundMonthData>();
            string strSql = $@"select to_char(T.INCEPTTIME, 'MM') as month,
                                T.PATIENTTYPE,
                                T.typename as classType,
                                to_char(median(arountime)) as monthnum
                            from (
                                select c.typename,
                                        a.BARCODE,
                                        decode(a.EMC, '1', '加急', a.PATIENTTYPE) as PATIENTTYPE,
                                        a.INCEPTTIME,
                                        a.APPROVETIME,
                                        round((a.APPROVETIME - a.INCEPTTIME) * 24 * 60, 0) as arountime
                                    from view_las_sap_samplereg a,
                                        las_sap_sampleitem     b,
                                        las_stat_itemconfig    c
                                    where a.machineid = b.machineid
                                    and a.testdate = b.testdate
                                    and a.sampleid = b.sampleid
                                    and b.hisitemid = c.hisitemid
                                    and to_char(a.INCEPTTIME, 'yyyy') = '{year}' and a.confirmstate='1' and to_char(a.SAMPLETIME,'yyyy')>'1990' ) T
                            group by to_char(T.INCEPTTIME, 'MM'), T.PATIENTTYPE, T.typename";
            month = OracleHelp.QueryListByEmit<Models.Model.AroundMonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 检验前90位数
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static List<Models.Model.AroundMonthData> GetAroundJYQ90(string year)
        {
            List<Models.Model.AroundMonthData> month = new List<Models.Model.AroundMonthData>();
            string strSql = $@"select to_char(T.INCEPTTIME, 'MM') as month, T.PATIENTTYPE ,to_char( percentile_cont(0.9) within group(order by arountime)) as monthnum
                              from (
        
                                    select distinct a.BARCODE,
                                                     decode(a.EMC, '1', '加急', a.PATIENTTYPE) as PATIENTTYPE,
                                                     a.SAMPLETIME,
                                                     a.INCEPTTIME,
                                                     round（(a.INCEPTTIME - a.SAMPLETIME) * 24 * 60,
                                                     0） as arountime
                                      from view_las_sap_samplereg a
                                     where to_char(a.INCEPTTIME, 'yyyy') = '{year}' and confirmstate='1' and to_char(a.SAMPLETIME,'yyyy')>'1990') T
                             group by to_char(T.INCEPTTIME, 'MM'), T.PATIENTTYPE";
            month = OracleHelp.QueryListByEmit<Models.Model.AroundMonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 检验前90分数，分检验类别
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static List<Models.Model.AroundMonthData> GetAroundJYQbyType90(string year)
        {
            List<Models.Model.AroundMonthData> month = new List<Models.Model.AroundMonthData>();
            string strSql = $@"select to_char(T.INCEPTTIME, 'MM') as month,
                                T.PATIENTTYPE,
                                T.typename as classType,
                                to_char(percentile_cont(0.9) within group(order by arountime)) as monthnum
                            from (
                                select c.typename,
                                        a.BARCODE,
                                        decode(a.EMC, '1', '加急', a.PATIENTTYPE) as PATIENTTYPE,
                                        a.SAMPLETIME,
                                        a.INCEPTTIME,
                                        round((a.INCEPTTIME - a.SAMPLETIME) * 24 * 60, 0) as arountime
                                    from view_las_sap_samplereg a,
                                        las_sap_sampleitem     b,
                                        las_stat_itemconfig    c
                                    where a.machineid = b.machineid
                                    and a.testdate = b.testdate
                                    and a.sampleid = b.sampleid
                                    and b.hisitemid = c.hisitemid
                                    and to_char(a.INCEPTTIME, 'yyyy') = '{year}' and a.confirmstate='1' and to_char(a.SAMPLETIME,'yyyy')>'1990') T
                            group by to_char(T.INCEPTTIME, 'MM'), T.PATIENTTYPE, T.typename";
            month = OracleHelp.QueryListByEmit<Models.Model.AroundMonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 室内90分数
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static List<Models.Model.AroundMonthData> GetAroundSN90(string year)
        {
            List<Models.Model.AroundMonthData> month = new List<Models.Model.AroundMonthData>();
            string strSql = $@"select to_char(T.INCEPTTIME, 'MM') as month, T.PATIENTTYPE ,to_char(percentile_cont(0.9) within group(order by arountime)) as monthnum
                              from (
        
                                    select distinct a.BARCODE,
                                                     decode(a.EMC, '1', '加急', a.PATIENTTYPE) as PATIENTTYPE,
                                                     a.INCEPTTIME,
                                                     a.APPROVETIME,
                                                     round（(a.APPROVETIME - a.INCEPTTIME) * 24 * 60,
                                                     0） as arountime
                                      from view_las_sap_samplereg a
                                     where to_char(a.INCEPTTIME, 'yyyy') = '{year}' and confirmstate='1' and to_char(a.SAMPLETIME,'yyyy')>'1990') T
                             group by to_char(T.INCEPTTIME, 'MM'), T.PATIENTTYPE";
            month = OracleHelp.QueryListByEmit<Models.Model.AroundMonthData>(strSql);
            return month;
        }
        /// <summary>
        /// 室内90分数，分检验类别
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static List<Models.Model.AroundMonthData> GetAroundSNbyType90(string year)
        {
            List<Models.Model.AroundMonthData> month = new List<Models.Model.AroundMonthData>();
            string strSql = $@"select to_char(T.INCEPTTIME, 'MM') as month,
                                T.PATIENTTYPE,
                                T.typename as classType,
                                to_char(percentile_cont(0.9) within group(order by arountime)) as monthnum
                            from (
                                select c.typename,
                                        a.BARCODE,
                                        decode(a.EMC, '1', '加急', a.PATIENTTYPE) as PATIENTTYPE,
                                        a.INCEPTTIME,
                                        a.APPROVETIME,
                                        round((a.APPROVETIME - a.INCEPTTIME) * 24 * 60, 0) as arountime
                                    from view_las_sap_samplereg a,
                                        las_sap_sampleitem     b,
                                        las_stat_itemconfig    c
                                    where a.machineid = b.machineid
                                    and a.testdate = b.testdate
                                    and a.sampleid = b.sampleid
                                    and b.hisitemid = c.hisitemid
                                    and to_char(a.INCEPTTIME, 'yyyy') = '{year}' and a.confirmstate='1' and to_char(a.SAMPLETIME,'yyyy')>'1990') T
                            group by to_char(T.INCEPTTIME, 'MM'), T.PATIENTTYPE, T.typename";
            month = OracleHelp.QueryListByEmit<Models.Model.AroundMonthData>(strSql);
            return month;
        }
        
        /// <summary>
        /// 查询全部类型的周转时间清单
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public   DataTable GetAroundAllList(string year,string patienttype)
        {
            string strSql = $@"
                            select distinct a.BARCODE as 条码号,
                                            decode(a.EMC, '1', '加急', a.PATIENTTYPE) as 患者类型,
                                            a.PATIENTID as 患者编号,
                                            a.PATIENTNAME as 姓名,
                                            a.DEPTNAME as 申请科室,
                                            a.HISITEMNAMELIST as 检验目的,
                                            a.SAMPLETYPE as 样本类型,
                                            to_char(a.SAMPLETIME,'yyyy-MM-dd hh24:mi:ss') as 采集时间,
                                            to_char(a.INCEPTTIME,'yyyy-MM-dd hh24:mi:ss') as 接收时间,
                                            to_char(a.APPROVETIME,'yyyy-MM-dd hh24:mi:ss') as 审核时间,
                                            round（(a.INCEPTTIME - a.SAMPLETIME) * 24 * 60, 0） as 检验前周转时间,
                                            round（(a.APPROVETIME - a.INCEPTTIME) * 24 * 60, 0） as 室内周转时间
                              from view_las_sap_samplereg a
                             where to_char(a.INCEPTTIME, 'yyyyMM') = '{year}' and CONFIRMSTATE='1' and to_char(a.SAMPLETIME,'yyyy')>'1990'
                              and (a.PATIENTTYPE='{patienttype}' or a.EMC ='{patienttype}')
                             order by a.BARCODE";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询特定类型的周转时间清单
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public DataTable GetAroundClassList(string month, string patienttype,string typename)
        {
            string strSql = $@"
                           select c.typename as 类别,
                           a.BARCODE as 条码号,
                           decode(a.EMC, '1', '加急', a.PATIENTTYPE) as 患者类型,
                           a.PATIENTID as  患者编号,
                           a.PATIENTNAME as 姓名,
                           a.DEPTNAME as 申请科室,
                           a.HISITEMNAMELIST as 检验目的,
                           b.hisitemname as 类别项目,
                           a.SAMPLETYPE as 样本类型,
                           to_char(a.SAMPLETIME,'yyyy-MM-dd hh24:mi:ss') as 采集时间,
                           to_char(a.INCEPTTIME,'yyyy-MM-dd hh24:mi:ss') as 接收时间,
                           to_char(a.APPROVETIME,'yyyy-MM-dd hh24:mi:ss') as 审核时间,
                           round（(a.INCEPTTIME - a.SAMPLETIME) * 24 * 60,  0） as 检验前周转时间,
                           round（(a.APPROVETIME - a.INCEPTTIME) * 24 * 60,  0） as 室内周转时间
                      from view_las_sap_samplereg a,
                           las_sap_sampleitem     b,
                           las_stat_itemconfig    c
                     where a.machineid = b.machineid
                       and a.testdate = b.testdate
                       and a.sampleid = b.sampleid
                       and b.hisitemid = c.hisitemid
                       and to_char(a.INCEPTTIME, 'yyyyMM') = '{month}'
                       and c.typename = '{typename}'
                       and (a.PATIENTTYPE = '{patienttype}' or a.EMC = '{patienttype}')
                       and a.confirmstate = '1'
                       and c.classtype='周转时间类'
                       and to_char(a.SAMPLETIME,'yyyy')>'1990'
                       order by a.BARCODE";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        public static List<Models.Model.hisitemtype> checkItem(string year,string classtype)
        {
            List<Models.Model.hisitemtype> alitem = new List<Models.Model.hisitemtype>();
            string strSql = @"
                            select distinct b.hisitemid, b.hisitemname
                              from view_las_sap_samplereg a, las_sap_sampleitem b
                             where to_char(a.SAMPLETIME, 'yyyy') = '2025'
                               and a.machineid = b.machineid
                               and a.testdate = b.testdate
                               and a.sampleid = b.sampleid
                               and b.hisitemid not in (
                                                       select c.hisitemid
                                                         from las_stat_itemconfig c
                                                         where c.classtype='{classtype}'
                                                        )
                             order by b.hisitemid
                            ";
            alitem = OracleHelp.QueryListByEmit<Models.Model.hisitemtype>(strSql);
            return alitem;
        }
        public int InsertItemType(string typeid,string typename,string classtype)
        {
            int i = -1;
            string strSql = $@"insert into las_stat_itemtype
                              (typeid, typename,classtype)
                            values
                              ('{typeid}', '{typename}','{classtype}') ";
            try
            {
                 i = OracleHelp.ExecuteNonQuery(strSql);
            }
            catch
            {
                i = -1;
            }
            
            return i;
        }
        public int UpdateItemType(string typeid, string typename)
        {
            string strSql = $@"update las_stat_itemtype set typename =  '{typename}' where typeid='{typeid}'";
            int i = OracleHelp.ExecuteNonQuery(strSql);
            return i;
        }
        /// <summary>
        /// 获取类别列表
        /// </summary>
        /// <returns></returns>
        public static List<Models.Model.typeclass> getTypeList(string classtype)
        {
            List<Models.Model.typeclass> typeList = new List<Models.Model.typeclass>();
            string strSql = $@"select typeid,typename from las_stat_itemtype where classtype='{classtype}'";
            typeList = OracleHelp.QueryListByEmit<Models.Model.typeclass>(strSql);
            return typeList;
        }
        /// <summary>
        /// 根据检验类别查询对应的项目列表
        /// </summary>
        /// <param name="typeid"></param>
        /// <returns></returns>
        public static List<Models.Model.hisitemtype> hisItemList(string typeid)
        {
            List<Models.Model.hisitemtype> alhisitemlist = new List<Models.Model.hisitemtype>();
            string strSql = $@"select t.hisitemid,t.hisitemname,t.typeid,t.typename from las_stat_itemconfig t where t.typeid='{typeid}'";
            alhisitemlist = OracleHelp.QueryListByEmit<Models.Model.hisitemtype>(strSql);
            return alhisitemlist;
        }
        public static int insertItemType(string hisitemid,string hisitemname,string typeid,string typename,string classtype)
        {
            int i = -1;
            string strSql = $@"insert into las_stat_itemconfig
                          (hisitemid, hisitemname, typeid, typename,classtype)
                        values
                          ('{hisitemid}', '{hisitemname}', '{typeid}', '{typename}','{classtype}')";
            try
            {
                i = OracleHelp.ExecuteNonQuery(strSql);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return i;
        }
        public List<Models.ModelQuatity.lisGroup> getLisGroup(string isstate)
        {
            List<Models.ModelQuatity.lisGroup> listGroup = new List<Models.ModelQuatity.lisGroup>();
            string strSql = $@"select groupid,groupname, isstate as isstate 
                            from las_com_group t where (isstate='{isstate}' or 'ALL'='{isstate}') order by groupid";
            listGroup = OracleHelp.QueryListByEmit<Models.ModelQuatity.lisGroup>(strSql);
            return listGroup;
        }
        public DataTable getDtGroup()
        {
            List<Models.ModelQuatity.lisGroup> listGroup = new List<Models.ModelQuatity.lisGroup>();
            string strSql = @"select groupid,groupname, isstate as isstate from las_com_group t order by groupid";
            //listGroup = OracleHelp.QueryListByEmit<Models.ModelQuatity.lisGroup>(strSql);
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        //----------------------------质量指标统计------------------------------------------------
        /// <summary>
        /// 查询不合格类别及数据
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public List<Models.ModelQuatity.badSampleType> getListBadSampleList(string begDate,string endDate,string groupid)
        {
            List<Models.ModelQuatity.badSampleType> badList = new List<Models.ModelQuatity.badSampleType>();
            string strSql = $@"select tt.memo3 AS typereason, to_char(nvl(count(1),0)) AS typenum
                              from las_sys_dictionary tt, las_sap_samplereject a
                             where tt.typeid = 'SampleRejectReason'
                               and tt.dicname = a.reason
                               and a.regdate >= '{begDate}'
                               and a.regdate <= '{endDate}'
                               and a.groupid in ({groupid}) and tt.memo3 is not null
                             group by tt.memo3";
            badList = OracleHelp.QueryListByEmit<Models.ModelQuatity.badSampleType>(strSql);
            return badList;
        }
        /// <summary>
        /// 查询样本总数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public List<Models.ModelQuatity.badSampleType> getListTotalSampleList(string begDate, string endDate, string groupid)
        {
            List<Models.ModelQuatity.badSampleType> totalList = new List<Models.ModelQuatity.badSampleType>();
            string strSql = $@"select '样本总数' as typereason, to_char( count(distinct barcode)) as typenum
                                from view_las_sap_samplereg a
                                where a.testdate >= '{begDate}'
                              and a.testdate <= '{endDate}'
                            and a.groupid in ({groupid}) ";
            totalList = OracleHelp.QueryListByEmit<Models.ModelQuatity.badSampleType>(strSql);
            return totalList;
        }
        /// <summary>
        /// 查询抗凝标本总数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public List<Models.ModelQuatity.badSampleType> getListKNTotalSampleList(string begDate, string endDate, string groupid)
        {
            List<Models.ModelQuatity.badSampleType> totalList = new List<Models.ModelQuatity.badSampleType>();
            string strSql = $@"select '样本总数' as typereason, to_char( count(distinct barcode)) as typenum
                                from view_las_sap_samplereg a
                                where a.testdate >= '{begDate}'
                              and a.testdate <= '{endDate}'
                            and a.groupid in ({groupid})
                            and a.sampletype in ('全血','血浆')";
            totalList = OracleHelp.QueryListByEmit<Models.ModelQuatity.badSampleType>(strSql);
            return totalList;
        }
        /// <summary>
        /// 查询血培养污染数据
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public List<Models.ModelQuatity.badSampleType> getListPolluteSampleList(string begDate, string endDate, string groupid)
        {
            List<Models.ModelQuatity.badSampleType> badList = new List<Models.ModelQuatity.badSampleType>();
            //string strSql = $@"select '血培养污染数' as typereason,
            //               to_char(nvl(sum(t.pollute), 0)) as typenum
            //           from las_gm_gndresultgerm t,las_gm_samplereg a
            //         where t.testdate >= '{begDate}'
            //         and t.testdate <= '{endDate}'
            //         and t.machineid= a.machineid
            //         and t.testdate = a.testdate
            //         and t.sampleid = a.sampleid
            //         and a.groupid in ({groupid})";
            string strSql = $@"
                        select '血培养污染数' as typereason, to_char(count(1)) as typenum
                          from (select distinct a.patientseq, b.germid
                                  from las_gm_samplereg a, las_gm_resultgerm b, las_gm_gndresult c
                                 where a.machineid = b.machineid
                                   and a.testdate = b.testdate
                                   and a.sampleid = b.sampleid
                                   and a.machineid = c.testmachine
                                   and a.sampleid = c.sampleid
                                   and a.testdate = c.checkdate
                                   and b.germid in
                                       (select t3.dicid
                                          from las_sys_dictionary t3
                                         where t3.typeid = 'GndPollutionGermDef')
                                   and a.testdate >= '{begDate}'
                                   and a.testdate <= '{endDate}'
                                   and a.groupid in ({groupid}))
                        ";
            badList = OracleHelp.QueryListByEmit<Models.ModelQuatity.badSampleType>(strSql);
            return badList;
        }
        /// <summary>
        /// 查询血培养总数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public List<Models.ModelQuatity.badSampleType> getListTotalBloodSampleList(string begDate, string endDate,string groupid)
        {
            List<Models.ModelQuatity.badSampleType> totalList = new List<Models.ModelQuatity.badSampleType>();
            string strSql = $@"select '血培养总数' as typereason, to_char(nvl(count(distinct barcode),0)) as typenum
                          from las_gm_samplereg tt
                         where tt.testtype = '血培养'
                           and tt.testdate >= '{begDate}'
                           and tt.testdate<='{endDate}'
                           and tt.hisitemnamelist like '%需氧%'
                           and groupid in ({groupid})";
            totalList = OracleHelp.QueryListByEmit<Models.ModelQuatity.badSampleType>(strSql);
            return totalList;
        }
        /// <summary>
        /// 查询同期应开展质控项目总数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public List<Models.ModelQuatity.badSampleType> getListTotalQCItemCount(string begDate, string endDate, string groupid)
        {
            List<Models.ModelQuatity.badSampleType> totalList = new List<Models.ModelQuatity.badSampleType>();
            string strSql = $@"select '同期应开展质控检验项目总数' as typereason ,to_char(nvl(count(distinct a.itemid),0))  as typenum
                          from las_com_machine t, las_com_machineitem a, las_rt_result b
                         where a.machineid = b.machineid
                           and a.itemid = b.itemid
                           and b.testdate >= '{begDate}'
                           and b.testdate <= '{endDate}'
                           and substr(a.state,4,1)='1'
                           and t.machineid = a.machineid
                           and t.groupid in ({groupid})  ";
            totalList = OracleHelp.QueryListByEmit<Models.ModelQuatity.badSampleType>(strSql);
            return totalList;
        }
        /// <summary>
        /// 查询同期已开展质控项目总数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public List<Models.ModelQuatity.badSampleType> getListQCItemCount(string begDate, string endDate, string groupid)
        {
            List<Models.ModelQuatity.badSampleType> totalList = new List<Models.ModelQuatity.badSampleType>();
            string strSql = $@"  select '已开展室内质控的检验项目数' as typereason,to_char(nvl(count(distinct b.itemid),0)) as typenum
                            from las_com_machine a, las_qc_result b
                           where a.machineid = b.machineid
                             and a.groupid in ({groupid})
                             and b.qctime >= '{begDate}'  and  b.qctime <= '{endDate}'";
            totalList = OracleHelp.QueryListByEmit<Models.ModelQuatity.badSampleType>(strSql);
            return totalList;
        }
        /// <summary>
        /// 查询同期已开展质控项目CV不合格数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public List<Models.ModelQuatity.badSampleType> getQCBadCount(string begDate, string endDate, string groupid)
        {
            List<Models.ModelQuatity.badSampleType> totalList = new List<Models.ModelQuatity.badSampleType>();
            string begMonth = begDate.Substring(0, 6);
            string endMonth = endDate.Substring(0, 6);
            string strSql = $@"  
                        select '室内质控不合格数' as typereason,to_char(nvl( count(distinct itemid),0)) as typenum
                        from 
                        (
                        select a.itemid,a.qcmonth,a.qclevel,a.cvavg,b.maxerrlevel1 as maxlevel
                        from las_qc_monthdata a,las_qc_itemtea b,las_com_machine t
                        where a.machineid = b.machineid
                        and a.itemid = b.itemid
                        and a.qclevel = '1'
                        and to_number(a.cvavg) > to_number(b.maxerrlevel1)
                        and a.qcmonth>='{begMonth}'
                        and a.qcmonth<='{endMonth}'
                        and a.machineid = t.machineid
                        and t.groupid in ({groupid})
                        union all
                        select a.itemid,a.qcmonth,a.qclevel,a.cvavg,b.maxerrlevel2 as maxlevel
                        from las_qc_monthdata a,las_qc_itemtea b,las_com_machine t
                        where a.machineid = b.machineid
                        and a.itemid = b.itemid
                        and a.qclevel = '2'
                        and to_number(a.cvavg) > to_number(b.maxerrlevel2)
                        and a.qcmonth>='{begMonth}'
                        and a.qcmonth<='{endMonth}'
                        and a.machineid = t.machineid
                        and t.groupid in ({groupid})
                        union all
                        select a.itemid,a.qcmonth,a.qclevel,a.cvavg,b.maxerrlevel3 as maxlevel
                        from las_qc_monthdata a,las_qc_itemtea b,las_com_machine t
                        where a.machineid = b.machineid
                        and a.itemid = b.itemid
                        and a.qclevel = '3'
                        and to_number(a.cvavg) > to_number(b.maxerrlevel3)
                        and a.qcmonth>='{begMonth}'
                        and a.qcmonth<='{endMonth}'
                        and a.machineid = t.machineid
                        and t.groupid in ({groupid})
                        union all
                        select a.itemid,a.qcmonth,a.qclevel,a.cvavg,b.maxerrlevel4 as maxlevel
                        from las_qc_monthdata a,las_qc_itemtea b,las_com_machine t
                        where a.machineid = b.machineid
                        and a.itemid = b.itemid
                        and a.qclevel = '4'
                        and to_number(a.cvavg) > to_number(b.maxerrlevel4)
                        and a.qcmonth>='{begMonth}'
                        and a.qcmonth<='{endMonth}'
                        and a.machineid = t.machineid
                        and t.groupid in ({groupid})   )";
            totalList = OracleHelp.QueryListByEmit<Models.ModelQuatity.badSampleType>(strSql);
            return totalList;
        }

        /// <summary>
        /// 取消审核报告单数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public List<Models.ModelQuatity.badSampleType> getCancleTestFormNum(string begDate, string endDate, string groupid)
        {
            List<Models.ModelQuatity.badSampleType> totalList = new List<Models.ModelQuatity.badSampleType>();
            string strSql = $@"  
                        select '取消审核的报告单数' as typereason, to_char(nvl(count(1),0)) as typenum
                          from LAS_SAP_SAMPLECANCEL a, view_las_sap_samplereg b
                         where a.machineid = b.machineid
                           and a.testdate = b.testdate
                           and a.sampleid = b.sampleid
                           --and b.CONFIRMSTATE='1'
                           and b.testdate >= '{begDate}'
                           and b.testdate <= '{endDate}'
                           and b.groupid in ({groupid})
                           and b.PATIENTID is not null and a.reason not like '%增删项目%'";
            totalList = OracleHelp.QueryListByEmit<Models.ModelQuatity.badSampleType>(strSql);
            return totalList;
        }
        /// <summary>
        /// 报告单总数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public List<Models.ModelQuatity.badSampleType> getTestFormNum(string begDate, string endDate, string groupid)
        {
            List<Models.ModelQuatity.badSampleType> totalList = new List<Models.ModelQuatity.badSampleType>();
            string strSql = $@"  
                       select '检验报告单总数' as typereason, to_char(nvl(count(1),0)) as typenum
                          from view_las_sap_samplereg t
                         where t.testdate >= '{begDate}'
                           and t.testdate <='{endDate}'
                           and t.CONFIRMSTATE = '1'
                           and t.PATIENTID is not null
                           and t.GROUPID in ({groupid})";
            totalList = OracleHelp.QueryListByEmit<Models.ModelQuatity.badSampleType>(strSql);
            return totalList;
        }
        /// <summary>
        /// 通报危急值数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public List<Models.ModelQuatity.badSampleType> getLifeAlterNum(string begDate, string endDate, string groupid)
        {
            List<Models.ModelQuatity.badSampleType> totalList = new List<Models.ModelQuatity.badSampleType>();
            string strSql = $@"  
                       select '通报危机值数' as typereason, to_char(count(1)) as typenum
                          from las_sap_lifealert t
                         where t.testdate >= '{begDate}'
                           and t.testdate <='{endDate}'
                           and t.groupid in ({groupid}) and t.alerttype = '0' ";
            
            totalList = OracleHelp.QueryListByEmit<Models.ModelQuatity.badSampleType>(strSql);
            return totalList;
        }
        /// <summary>
        /// 通报危急值数
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public List<Models.ModelQuatity.badSampleType> getLifeAlterNotOverTimeNum(string begDate, string endDate, string groupid)
        {
            List<Models.ModelQuatity.badSampleType> totalList = new List<Models.ModelQuatity.badSampleType>();
            string strSql = $@"  
                       select '发送时间-发现时间不超过30分钟' as typereason, to_char(count(1)) as typenum
                          from las_sap_lifealert t
                         where t.testdate >= '{begDate}'
                           and t.testdate <='{endDate}'
                           and t.groupid in ({groupid}) and t.alerttype = '0'
                          and (t.sendtime - t.dealdate)*24*60<=30";
            totalList = OracleHelp.QueryListByEmit<Models.ModelQuatity.badSampleType>(strSql);
            return totalList;
        }
        public int updateGroup(string groupid,string isstate)
        {
            string strSql = $@"update las_com_group set isstate='{isstate}' where groupid ='{groupid}'";
            return OracleHelp.ExecuteNonQuery(strSql);
        }
        /// <summary>
        /// 查询标本总拒收率（按月横向排列） Creatd By 徐振宇 2026年7月13日17:47:32
        /// </summary>
        public static List<Models.Model.QuaShowData> GetTotalRejectRate(string begDate)
        {
            List<Models.Model.QuaShowData> result = new List<Models.Model.QuaShowData>();

            string strSql = $@"
        WITH SAMPLE_DATA AS (
            SELECT 
                TO_CHAR(APPROVETIME, 'mm') AS MONTH,
                COUNT(DISTINCT barcode) AS TOTAL_SAMPLE
            FROM view_las_sap_samplereg
            WHERE TO_CHAR(APPROVETIME, 'yyyy') >= '{begDate}'
            GROUP BY TO_CHAR(APPROVETIME, 'mm')
        ),
        REJECT_DATA AS (
            SELECT 
                SUBSTR(t2.regdate, 5, 2) AS MONTH,
                COUNT(DISTINCT t2.barcode) AS TOTAL_REJECT
            FROM las_sap_samplereject t2
            WHERE SUBSTR(t2.regdate, 1, 4) >= '{begDate}'
              AND t2.reason IN (
                  SELECT nvl(a.memo3, a.dicname)
                  FROM las_sys_dictionary a
                  WHERE a.typeid = 'SampleRejectReason'
                    AND a.dicname IS NOT NULL
                    AND a.isshow = '1'
              )
            GROUP BY SUBSTR(t2.regdate, 5, 2)
        )
        SELECT 
            '5:总拒收率' AS TYPEID,
            '标本总拒收率(%)' AS TYPENAME,
            '≤' AS TYPEFX,
            '0.05' AS TYPEMB,
            MAX(CASE WHEN S.MONTH = '01' THEN ROUND(NVL(R.TOTAL_REJECT, 0) / NULLIF(S.TOTAL_SAMPLE, 0) * 100, 2) END) AS JAN,
            MAX(CASE WHEN S.MONTH = '02' THEN ROUND(NVL(R.TOTAL_REJECT, 0) / NULLIF(S.TOTAL_SAMPLE, 0) * 100, 2) END) AS FEB,
            MAX(CASE WHEN S.MONTH = '03' THEN ROUND(NVL(R.TOTAL_REJECT, 0) / NULLIF(S.TOTAL_SAMPLE, 0) * 100, 2) END) AS MAR,
            MAX(CASE WHEN S.MONTH = '04' THEN ROUND(NVL(R.TOTAL_REJECT, 0) / NULLIF(S.TOTAL_SAMPLE, 0) * 100, 2) END) AS APR,
            MAX(CASE WHEN S.MONTH = '05' THEN ROUND(NVL(R.TOTAL_REJECT, 0) / NULLIF(S.TOTAL_SAMPLE, 0) * 100, 2) END) AS MAY,
            MAX(CASE WHEN S.MONTH = '06' THEN ROUND(NVL(R.TOTAL_REJECT, 0) / NULLIF(S.TOTAL_SAMPLE, 0) * 100, 2) END) AS JUN,
            MAX(CASE WHEN S.MONTH = '07' THEN ROUND(NVL(R.TOTAL_REJECT, 0) / NULLIF(S.TOTAL_SAMPLE, 0) * 100, 2) END) AS JUL,
            MAX(CASE WHEN S.MONTH = '08' THEN ROUND(NVL(R.TOTAL_REJECT, 0) / NULLIF(S.TOTAL_SAMPLE, 0) * 100, 2) END) AS AUG,
            MAX(CASE WHEN S.MONTH = '09' THEN ROUND(NVL(R.TOTAL_REJECT, 0) / NULLIF(S.TOTAL_SAMPLE, 0) * 100, 2) END) AS SEP,
            MAX(CASE WHEN S.MONTH = '10' THEN ROUND(NVL(R.TOTAL_REJECT, 0) / NULLIF(S.TOTAL_SAMPLE, 0) * 100, 2) END) AS OCT,
            MAX(CASE WHEN S.MONTH = '11' THEN ROUND(NVL(R.TOTAL_REJECT, 0) / NULLIF(S.TOTAL_SAMPLE, 0) * 100, 2) END) AS NOV,
            MAX(CASE WHEN S.MONTH = '12' THEN ROUND(NVL(R.TOTAL_REJECT, 0) / NULLIF(S.TOTAL_SAMPLE, 0) * 100, 2) END) AS DEC,
            ROUND(SUM(NVL(R.TOTAL_REJECT, 0)) / NULLIF(SUM(S.TOTAL_SAMPLE), 0) * 100, 2) AS QST
        FROM SAMPLE_DATA S
        LEFT JOIN REJECT_DATA R ON S.MONTH = R.MONTH
    ";

            // 改用 QueryListByReflect
            result = OracleHelp.QueryListByReflect<Models.Model.QuaShowData>(strSql);
            return result;
        }

        /// <summary>
        /// 查询标本总拒收率的明细数据
        /// </summary>
        /// <param name="month">传入年月，比如202601、202602、202603</param>
        /// <returns>样本明细</returns>
        public List<Models.Model.sampleReject> GetTotalRejectDetail(string month)
        {
            List<Models.Model.sampleReject> alSapRej = new List<Models.Model.sampleReject>();
            string strSql = $@"
                            select a.regdate, a.barcode, a.patientid, a.patientname, a.sampletype, a.hisitemnamelist, a.reason, a.opername
                            from las_sap_samplereject a
                            where substr(a.regdate, 1, 6) = '{month}'
                              and a.reason in (
                                  select nvl(b.memo3, b.dicname)
                                  from las_sys_dictionary b
                                  where b.typeid = 'SampleRejectReason'
                                    and b.dicname is not null
                                    and b.isshow = '1'
                              )
                            order by a.regdate desc
                        ";
            alSapRej = OracleHelp.QueryListByEmit<Models.Model.sampleReject>(strSql);
            return alSapRej;
        }
    }
}
