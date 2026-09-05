using NeuLis.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NeuLis.DataBase
{
    public class QualityOperDB
    {
        /// <summary>
        /// 查询标本清单
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable getBarcodeList(string begDate, string endDate, string groupid)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                           select a.BARCODE as 条码号,
                            a.GROUPID as 小组编码,
                            b.groupname as 小组名称,
                            a.machineid as 仪器编码,
                            a.testdate as 检验日期,
                            a.sampleid as 样本号,
                            a.PATIENTTYPE as 患者类型,
                            a.PATIENTID as 患者编号,
                            a.PATIENTSEQ as 就诊流水号,
                            a.PATIENTNAME as 姓名,
                            a.HISITEMNAMELIST as 检验目的,
                            a.DOCTORNAME as 开单医生,
                            a.DEPTNAME as 申请科室,
                            to_char(a.ORDERTIME,'yyyy-MM-dd HH24:mi:ss') as 医嘱时间,
                            a.NURSENAME as 采样护士,
                            to_char(a.SAMPLETIME,'yyyy-MM-dd HH24:mi:ss') as 采样时间,
                            a.INCEPTORNAME as 接收人,
                            to_char(a.INCEPTTIME,'yyyy-MM-dd HH24:mi:ss') as接收时间,
                            a.ACCEPTERNAME as 核收人,
                            to_char(a.ACCEPTTIME,'yyyy-MM-dd HH24:mi:ss') as 核收时间,
                            a.APPROVERNAME as 审核人,
                            to_char(a.APPROVETIME,'yyyy-MM-dd HH24:mi:ss') as 审核时间
                        from view_las_sap_samplereg a left join las_com_group b on a.GROUPID = b.groupid
                                where a.testdate >= '{begDate}'
                              and a.testdate <= '{endDate}'
                            and a.groupid in ({groupid})";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询凝集标本清单
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable getBarcodeNJList(string begDate, string endDate, string groupid)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                           select a.BARCODE as 条码号,
                            a.GROUPID as 小组编码,
                            b.groupname as 小组名称,
                            a.machineid as 仪器编码,
                            a.testdate as 检验日期,
                            a.sampleid as 样本号,
                            a.PATIENTTYPE as 患者类型,
                            a.PATIENTID as 患者编号,
                            a.PATIENTSEQ as 就诊流水号,
                            a.PATIENTNAME as 姓名,
                            a.HISITEMNAMELIST as 检验目的,
                            a.DOCTORNAME as 开单医生,
                            a.DEPTNAME as 申请科室,
                            to_char(a.ORDERTIME,'yyyy-MM-dd HH24:mi:ss') as 医嘱时间,
                            a.NURSENAME as 采样护士,
                            to_char(a.SAMPLETIME,'yyyy-MM-dd HH24:mi:ss') as 采样时间,
                            a.INCEPTORNAME as 接收人,
                            to_char(a.INCEPTTIME,'yyyy-MM-dd HH24:mi:ss') as接收时间,
                            a.ACCEPTERNAME as 核收人,
                            to_char(a.ACCEPTTIME,'yyyy-MM-dd HH24:mi:ss') as 核收时间,
                            a.APPROVERNAME as 审核人,
                            to_char(a.APPROVETIME,'yyyy-MM-dd HH24:mi:ss') as 审核时间
                        from view_las_sap_samplereg a left join las_com_group b on a.GROUPID = b.groupid
                                where a.testdate >= '{begDate}'
                              and a.testdate <= '{endDate}'
                            and a.groupid in ({groupid})
                          and a.sampletype in ('全血','血浆')";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询不合格标本明细
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <param name="typeid"></param>
        /// <returns></returns>
        public DataTable getBadSampleList(string begDate, string endDate, string groupid, string typeid)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                              select a.barcode as 条码号,
                               a.groupid as 小组编码,
                               b.groupname as 小组名称,
                               a.patientid as 患者编号,
                               a.patientname as 姓名,
                               a.deptname as 申请科室,
                               a.doctorname as 申请医生,
                               a.hisitemnamelist as 检验目的,
                               a.opername as 登记人,
                               to_char(a.opertime, 'yyyy-MM-dd HH24:mi:ss') as 登记时间,
                               a.operip as IP地址,
                               a.reason as 不合格原因,
                               tt.memo3 as 不合格类型
                          from las_sys_dictionary tt , las_sap_samplereject a left join las_com_group b on a.groupid = b.groupid
                         where tt.typeid = 'SampleRejectReason'
                           and tt.dicname = a.reason
                           and a.regdate >= '{begDate}'
                           and a.regdate <= '{endDate}'
                           and a.groupid in ({groupid})
                           and (tt.memo3 ='{typeid}' or '{typeid}'='ALL'  ) and  tt.memo3 is not null";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询血培养污染明细
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <param name="typeid"></param>
        /// <returns></returns>
        public DataTable getXPYWRSampleList(string begDate, string endDate, string groupid)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                             select distinct a.patientseq as 就诊流水号,
                                a.patienttype as 患者类型,
                                a.patientid as 患者编号,
                                a.patientname as 姓名,
                                a.hisitemnamelist as 检验目的,
                                a.doctorname as  申请医生,
                                a.deptname as 申请科室, 
                                a.machineid as 检验仪器,
                                a.testdate as 检验日期,
                                a.sampleid as 样本号,
                                b.germid as 细菌编码,
                                d.chnname as 细菌名称
                  from las_gm_samplereg a, las_gm_resultgerm b, las_gm_gndresult c,las_gm_germ d
                 where a.machineid = b.machineid
                   and a.testdate = b.testdate
                   and a.sampleid = b.sampleid
                   and a.machineid = c.testmachine
                   and a.sampleid = c.sampleid
                   and a.testdate = c.checkdate
                   and b.germid in (select t3.dicid
                                      from las_sys_dictionary t3
                                     where t3.typeid = 'GndPollutionGermDef')
                   and a.testdate >= '{begDate}'
                   and a.testdate <= '{endDate}'
                   and a.groupid in ({groupid})
                   and b.germid = d.germid
                ";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询血培养明细
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <param name="typeid"></param>
        /// <returns></returns>
        public DataTable getXPYSampleList(string begDate, string endDate, string groupid)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                            select tt.barcode as 条码号,
                           tt.patientseq as 就诊流水号,
                           tt.patienttype as 患者类型,
                           tt.patientid as 患者编号,
                           tt.patientname as 姓名,
                           tt.patientage||tt.patientageunit as 年龄,
                           tt.patientsex as 性别,
                           tt.doctorname as 申请医生,
                           tt.deptname as 申请科室,
                           tt.hisitemnamelist as 检验目的, 
                           tt.machineid as 检验仪器,
                           tt.testdate as 检验仪器,
                           tt.sampleid as 样本号
                      from las_gm_samplereg tt
                     where tt.testtype = '血培养'
                       and tt.testdate >= '{begDate}'
                       and tt.testdate <= '{endDate}'
                       and tt.hisitemnamelist like '%需氧%'
                       and groupid in ({groupid})
                                    ";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询质控项目明细
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public DataTable getQCItemList(string begDate, string endDate, string groupid)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                           select distinct a.itemid as 项目编码,
                            a.itemname as 项目名称,
                            listagg(a.machineid, '^') within group(order by a.itemid) as 相关仪器
                            from las_com_machine t, las_com_machineitem a, las_rt_result b
                            where a.machineid = b.machineid
                            and a.itemid = b.itemid
                            and b.testdate >= '{begDate}'
                            and b.testdate <= '{endDate}'
                            and substr(a.state, 4, 1) = '1'
                            and t.machineid = a.machineid
                            and t.groupid in ({groupid})
                            group by a.itemid, a.itemname   ";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询实际开展质控项目明细
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public DataTable getQCRsultItemList(string begDate, string endDate, string groupid)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                          select c.itemid as 项目编码,
                           c.itemname as 项目名称,
                           listagg(a.machineid, '^') within group(order by c.itemid) as 相关仪器
                      from las_com_machine a, las_qc_result b, las_com_machineitem c
                     where a.machineid = b.machineid
                       and a.groupid in ({groupid})
                       and b.qctime >= '{begDate}'
                       and b.qctime <= '{endDate}'
                       and b.machineid = c.machineid
                       and b.itemid = c.itemid
                     group by c.itemid, c.itemname
                     ";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询质控变异系数不合格项目明细
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public DataTable getQCBYItemList(string begMonth, string endMonth, string groupid)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                          select a.machineid as 仪器编码, a.itemid as  项目编码, a.qcmonth as 质控月份, a.qclevel as 质控水平, a.cvavg as 计算CV, b.maxerrlevel1 as 最大运行CV
                      from las_qc_monthdata a, las_qc_itemtea b, las_com_machine t
                     where a.machineid = b.machineid
                       and a.itemid = b.itemid
                       and a.qclevel = '1'
                       and to_number(a.cvavg) > to_number(b.maxerrlevel1)
                       and a.qcmonth >= '{begMonth}'
                       and a.qcmonth <= '{endMonth}'
                       and a.machineid = t.machineid
                       and t.groupid in ({groupid})
                    union all
                    select a.machineid as 仪器编码,a.itemid as  项目编码, a.qcmonth as 质控月份, a.qclevel as 质控水平, a.cvavg as 计算CV, b.maxerrlevel2 as 最大运行CV
                      from las_qc_monthdata a, las_qc_itemtea b, las_com_machine t
                     where a.machineid = b.machineid
                       and a.itemid = b.itemid
                       and a.qclevel = '2'
                       and to_number(a.cvavg) > to_number(b.maxerrlevel2)
                       and a.qcmonth >= '{begMonth}'
                       and a.qcmonth <= '{endMonth}'
                       and a.machineid = t.machineid
                       and t.groupid in ({groupid})
                    union all
                    select a.machineid as 仪器编码,a.itemid as  项目编码, a.qcmonth as 质控月份, a.qclevel as 质控水平, a.cvavg as 计算CV, b.maxerrlevel3 as 最大运行CV
                      from las_qc_monthdata a, las_qc_itemtea b, las_com_machine t
                     where a.machineid = b.machineid
                       and a.itemid = b.itemid
                       and a.qclevel = '3'
                       and to_number(a.cvavg) > to_number(b.maxerrlevel3)
                       and a.qcmonth >= '{begMonth}'
                       and a.qcmonth <= '{endMonth}'
                       and a.machineid = t.machineid
                       and t.groupid in ({groupid})
                    union all
                    select a.machineid as 仪器编码,a.itemid as  项目编码, a.qcmonth as 质控月份, a.qclevel as 质控水平, a.cvavg as 计算CV, b.maxerrlevel4 as 最大运行CV
                      from las_qc_monthdata a, las_qc_itemtea b, las_com_machine t
                     where a.machineid = b.machineid
                       and a.itemid = b.itemid
                       and a.qclevel = '4'
                       and to_number(a.cvavg) > to_number(b.maxerrlevel4)
                       and a.qcmonth >= '{begMonth}'
                       and a.qcmonth <= '{endMonth}'
                       and a.machineid = t.machineid
                       and t.groupid in ({groupid})
                                         ";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询报告单清单
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable getSampleList(string begDate, string endDate, string groupid)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                           select a.BARCODE as 条码号,
                            a.GROUPID as 小组编码,
                            b.groupname as 小组名称,
                            a.machineid as 仪器编码,
                            a.testdate as 检验日期,
                            a.sampleid as 样本号,
                            a.PATIENTTYPE as 患者类型,
                            a.PATIENTID as 患者编号,
                            a.PATIENTSEQ as 就诊流水号,
                            a.PATIENTNAME as 姓名,
                            a.HISITEMNAMELIST as 检验目的,
                            a.DOCTORNAME as 开单医生,
                            a.DEPTNAME as 申请科室,
                            to_char(a.ORDERTIME,'yyyy-MM-dd HH24:mi:ss') as 医嘱时间,
                            a.NURSENAME as 采样护士,
                            to_char(a.SAMPLETIME,'yyyy-MM-dd HH24:mi:ss') as 采样时间,
                            a.INCEPTORNAME as 接收人,
                            to_char(a.INCEPTTIME,'yyyy-MM-dd HH24:mi:ss') as接收时间,
                            a.ACCEPTERNAME as 核收人,
                            to_char(a.ACCEPTTIME,'yyyy-MM-dd HH24:mi:ss') as 核收时间,
                            a.APPROVERNAME as 审核人,
                            to_char(a.APPROVETIME,'yyyy-MM-dd HH24:mi:ss') as 审核时间
                       from view_las_sap_samplereg a left join las_com_group b on a.groupid = b.groupid
                       where a.testdate >= '{begDate}'
                           and a.testdate <='{endDate}'
                           and a.CONFIRMSTATE = '1'
                           and a.PATIENTID is not null
                           and a.GROUPID in ({groupid})";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询不正确报告单清单
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable getCancleSampleList(string begDate, string endDate, string groupid)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                           select (select t.groupname from las_com_group t where t.groupid = a.groupid) as 检验小组,
                           b.BARCODE as 条码号,
                           b.PATIENTTYPE as 患者类型,
                           b.machineid as 检测仪器,
                           b.testdate as 检验日期,
                           b.sampleid as 样本号,
                           b.PATIENTID as 患者编号,
                           b.PATIENTSEQ as 患者流水号,
                           b.PATIENTNAME as 姓名,
                           b.PATIENTSEX as 性别,
                           b.PATIENTAGE as 年龄,
                           b.DOCTORNAME as 申请医生,
                           b.DEPTNAME as 申请科室,
                           b.HISITEMNAMELIST as 检验目的,
                           a.approvername as 审核人,
                           to_char(a.approvetime,'yyyy-MM-dd hh24:mi:ss') as 审核时间,
                           a.cancelername as 取消人,
                           to_char(a.canceltime,'yyyy-MM-dd hh24:mi:ss') as 取消时间,
                           a.reason as 取消原因
                      from LAS_SAP_SAMPLECANCEL a, view_las_sap_samplereg b
                     where a.machineid = b.machineid
                       and a.testdate = b.testdate
                       and a.sampleid = b.sampleid
                          --and b.CONFIRMSTATE='1'
                       and b.testdate >= '{begDate}'
                       and b.testdate <= '{endDate}'
                       and b.groupid in ({groupid})
                       and b.PATIENTID is not null and a.reason not like '%增删项目%'
                    ";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询危急值清单
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable getAlertSampleList(string begDate, string endDate, string groupid)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                          select (select a.groupname from las_com_group a where a.groupid = t.groupid) as 检验小组,
                           t.barcode as 条码号,
                           b.PATIENTTYPE as 患者类型,
                           b.PATIENTID as 患者编号,
                           b.PATIENTNAME as 姓名,
                           b.PATIENTSEX as 性别,
                           b.PATIENTAGE as 年龄,
                           b.DOCTORNAME as 申请医生,
                           b.DEPTNAME as 申请科室,
                           b.HISITEMNAMELIST as 检验目的, 
                           t.itemid as 项目编码,
                           t.itemname as 项目名称,
                           t.reportvalue as 结果,
                           to_char(t.dealdate,'yyyy-MM-dd hh24:mi:ss') as 发现时间,
                           t.sendname as 发送人,
                           to_char(t.sendtime,'yyyy-MM-dd hh24:mi:ss') as 发送时间,
                           round((t.sendtime - t.dealdate)*24*60,1) as 发送间隔,
                           t.answername as 答复人,
                           to_char(t.answertime,'yyyy-MM-dd hh24:mi:ss') as 答复时间,
                           round((t.answertime - t.sendtime)*24*60,1) as 答复间隔,
                           t.answercon as 答复内容 ,
                           t.phonetime as 电话答复时间,
                           t.phoneanswer as 电话答复人,      
                           t.phonerecord as 电话答复内容
                      from las_sap_lifealert t
                      left join view_las_sap_samplereg b
                        on t.machineid = b.machineid
                       and t.testdate = b.testdate
                       and t.sampleid = b.sampleid
                     where t.testdate >= '{begDate}'
                       and t.testdate <= '{endDate}'
                       and t.groupid in ({groupid}) and t.alerttype = '0'    ";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 查询超时发送危急值清单
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable getNoOverAlertSampleList(string begDate, string endDate, string groupid)
        {
            List<Models.Model.barcodeReg> alBarcodeReg = new List<Models.Model.barcodeReg>();
            string strSql = $@"
                          select (select a.groupname from las_com_group a where a.groupid = t.groupid) as 检验小组,
                           t.barcode as 条码号,
                           b.PATIENTTYPE as 患者类型,
                           b.PATIENTID as 患者编号,
                           b.PATIENTNAME as 姓名,
                           b.PATIENTSEX as 性别,
                           b.PATIENTAGE as 年龄,
                           b.DOCTORNAME as 申请医生,
                           b.DEPTNAME as 申请科室,
                           b.HISITEMNAMELIST as 检验目的, 
                           t.itemid as 项目编码,
                           t.itemname as 项目名称,
                           t.reportvalue as 结果,
                           to_char(t.dealdate,'yyyy-MM-dd hh24:mi:ss') as 发现时间,
                           t.sendname as 发送人,
                           to_char(t.sendtime,'yyyy-MM-dd hh24:mi:ss') as 发送时间,
                           to_char( round((t.sendtime - t.dealdate)*24*60,1)) as 发送间隔,
                           to_char(t.phonetime,'yyyy-MM-dd hh24:mi:ss') as 电话时间,
                           round((t.phonetime -t.sendtime)*24*60,1) as 电话时间间隔,
                           t.answername as 答复人,
                           to_char(t.answertime,'yyyy-MM-dd hh24:mi:ss') as 答复时间,
                           round((t.answertime - t.sendtime)*24*60,1) as 答复间隔,
                           t.answercon as 答复内容
                      from las_sap_lifealert t
                      left join view_las_sap_samplereg b
                        on t.machineid = b.machineid
                       and t.testdate = b.testdate
                       and t.sampleid = b.sampleid
                     where t.testdate >= '{begDate}'
                       and t.testdate <= '{endDate}'
                       and t.groupid in ({groupid}) and t.alerttype = '0'
                       and (t.sendtime - t.dealdate)*24*60<=30
                    ";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;
        }
        /// <summary>
        /// 保存项目类别
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public int saveType(string typeID, string typeName, string preTime, string afterTime, string emc)
        {
            int iReturn = -1;
            string strSql = $@"  insert into las_qua_itemtype
                               (typeid, typename, pretime, aftertime, emc)
                             values
                               ('{typeID}', '{typeName}', '{preTime}', '{afterTime}', '{emc}')";
            iReturn = OracleHelp.ExecuteNonQuery(strSql);
            if (iReturn < 0)
            {
                strSql = $@"update las_qua_itemtype set typename='{typeName}',pretime='{preTime}',aftertime='{afterTime}',emc='{emc}' where typeid='{typeID}'";
                iReturn = OracleHelp.ExecuteNonQuery(strSql);
            }
            return iReturn;
        }
        /// <summary>
        /// 保存项目类别
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public int saveTypeItem(string typeID, string typeName, string hisitemid, string hisitemname)
        {
            int iReturn = -1;
            string strSql = $@"   insert into las_qua_item
                           (typeid, typename, hisitemid, hisitemname)
                         values
                           ('{typeID}', '{typeName}', '{hisitemid}', '{hisitemname}')";
            iReturn = OracleHelp.ExecuteNonQuery(strSql);
            return iReturn;
        }
        /// <summary>
        /// 加载项目类别列表
        /// </summary>
        /// <returns></returns>
        public List<Models.ModelQuatity.itemType> loadItemType()
        {
            List<Models.ModelQuatity.itemType> alitem = new List<Models.ModelQuatity.itemType>();
            string strsql = @"select typeid ,typename ,pretime , aftertime , emc  from las_qua_itemtype";
            alitem = OracleHelp.QueryListByEmit<Models.ModelQuatity.itemType>(strsql);
            return alitem;
        }
        /// <summary>
        /// 加载年度检验项目
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<Models.Model.hisitemtype> checkItem(string year)
        {
            List<Models.Model.hisitemtype> alitem = new List<Models.Model.hisitemtype>();
            string strSql = $@"
                            select distinct b.hisitemid, b.hisitemname
                              from view_las_sap_samplereg a, las_sap_sampleitem b
                             where to_char(a.SAMPLETIME, 'yyyy') = '{year}'
                               and a.machineid = b.machineid
                               and a.testdate = b.testdate
                               and a.sampleid = b.sampleid
                             order by b.hisitemid ";
            alitem = OracleHelp.QueryListByEmit<Models.Model.hisitemtype>(strSql);
            return alitem;
        }
        public List<NeuLis.Models.Model.hisitemtype> getTypeItemList(string typeid)
        {
            List<NeuLis.Models.Model.hisitemtype> alitem = new List<Models.Model.hisitemtype>();
            string strSql = $@" select t.hisitemid ,t.hisitemname ,typeid ,typename  from las_qua_item t where t.typeid = '{typeid}'";
            alitem = OracleHelp.QueryListByEmit<NeuLis.Models.Model.hisitemtype>(strSql);
            return alitem;
        }
        public void delTypeItem(string typeid, string hisitemid)
        {
            string strSql = $@"delete from las_qua_item where typeid ='{typeid}' and hisitemid='{hisitemid}'";
            OracleHelp.ExecuteNonQuery(strSql);
        }
        public List<NeuLis.Models.ModelQuatity.tatItem> getItemTAT(string begDate, string endDate, string groupid)
        {
            List<NeuLis.Models.ModelQuatity.tatItem> alitem = new List<Models.ModelQuatity.tatItem>();
            string strSql = $@" 
select t.emc,
       t.typeid,
       t.typename,
       to_char(count(1)) as sapcount,
       to_char(sum(overjyq)) as jyqbhg,
       to_char(round(1 - sum(overjyq) / count(1),4) * 100) as jyqhgl,
       to_char(sum(overjyz)) as jyzbhg,
      to_char(round(1 - sum(overjyz) / count(1),4) * 100) as jyzhgl,
       to_char(round(avg(jyqsj),1)) as jyqpjs,
       to_char(PERCENTILE_CONT(0.5) WITHIN GROUP(ORDER BY jyqsj)) AS jyqzws,
       to_char(PERCENTILE_CONT(0.9) WITHIN GROUP(ORDER BY jyqsj)) AS jyq9fs,
       to_char(round(avg(jyzsj),1)) as jyzpjs,
       to_char(PERCENTILE_CONT(0.5) WITHIN GROUP(ORDER BY jyzsj)) AS jyzzws,
       to_char(PERCENTILE_CONT(0.9) WITHIN GROUP(ORDER BY jyzsj)) AS jyz9fs
  from (select distinct '普通' as emc,
               c.typeid,
               c.typename,
               a.machineid,
               a.testdate,
               a.sampleid,
               a.BARCODE,
               a.SAMPLETIME,
               a.INCEPTTIME,
               a.APPROVETIME,
               round((a.INCEPTTIME - a.SAMPLETIME) * 24 * 60, 1) as jyqsj,
               case
                 when (a.INCEPTTIME - a.SAMPLETIME) * 24 * 60 > b.pretime then
                  1
                 else
                  0
               end as overjyq,
               round((a.APPROVETIME - a.INCEPTTIME) * 24 * 60, 1) as jyzsj,
               case
                 when (a.APPROVETIME - a.INCEPTTIME) * 24 * 60 > b.aftertime then
                  1
                 else
                  0
               end as overjyz
          from view_las_sap_samplereg a, las_qua_item c, las_qua_itemtype b
         where a.HISITEMIDLIST like '%' || c.hisitemid || '%'
           and c.typeid = b.typeid
           and a.PATIENTTYPE in ('门诊', '住院')
           and a.CONFIRMSTATE = '1'
           and a.EMC = '0' and a.testdate>='{begDate}' and a.testdate<='{endDate}'
           and a.groupid in ({groupid})
           and b.emc = '普通') t
 group by t.emc, t.typeid, t.typename
 union all
 
select t.emc,
       t.typeid,
       t.typename,
       to_char(count(1)) as sapcount,
       to_char(sum(overjyq)) as jyqbhg,
       to_char(round(1 - sum(overjyq) / count(1),4) * 100) as jyqhgl,
       to_char(sum(overjyz)) as jyzbhg,
      to_char(round(1 - sum(overjyz) / count(1),4) * 100) as jyzhgl,
       to_char(round(avg(jyqsj),1)) as jyqpjs,
       to_char(PERCENTILE_CONT(0.5) WITHIN GROUP(ORDER BY jyqsj)) AS jyqzws,
       to_char(PERCENTILE_CONT(0.9) WITHIN GROUP(ORDER BY jyqsj)) AS jyq9fs,
       to_char(round(avg(jyzsj),1)) as jyzpjs,
       to_char(PERCENTILE_CONT(0.5) WITHIN GROUP(ORDER BY jyzsj)) AS jyzzws,
       to_char(PERCENTILE_CONT(0.9) WITHIN GROUP(ORDER BY jyzsj)) AS jyz9fs
  from (select distinct '加急' as emc,
               c.typeid,
               c.typename,
               a.machineid,
               a.testdate,
               a.sampleid,
               a.BARCODE,
               a.SAMPLETIME,
               a.INCEPTTIME,
               a.APPROVETIME,
               round((a.INCEPTTIME - a.SAMPLETIME) * 24 * 60, 1) as jyqsj,
               case
                 when (a.INCEPTTIME - a.SAMPLETIME) * 24 * 60 > b.pretime then
                  1
                 else
                  0
               end as overjyq,
               round((a.APPROVETIME - a.INCEPTTIME) * 24 * 60, 1) as jyzsj,
               case
                 when (a.APPROVETIME - a.INCEPTTIME) * 24 * 60 > b.aftertime then
                  1
                 else
                  0
               end as overjyz
          from view_las_sap_samplereg a, las_qua_item c, las_qua_itemtype b
         where a.HISITEMIDLIST like '%' || c.hisitemid || '%'
           and c.typeid = b.typeid
           and a.PATIENTTYPE in ('门诊', '住院')
           and a.CONFIRMSTATE = '1'
           and a.EMC = '1' and a.testdate>='{begDate}' and a.testdate<='{endDate}'
           and a.groupid in ({groupid})
           and b.emc = '加急') t
 group by t.emc, t.typeid, t.typename

";
            alitem = OracleHelp.QueryListByEmit<NeuLis.Models.ModelQuatity.tatItem>(strSql);
            return alitem;
        }
        public DataTable getTATItemList(string begDate, string endDate, string typeid, string groupid, string typename)
        {
            string strsql1 = "";
            if (typename == "sapcount")
            {
                strsql1 = "1=1";
            }
            else if (typename == "jyqbhg")
            {
                strsql1 = "(round((a.INCEPTTIME - a.SAMPLETIME) * 24 * 60, 1)>b.pretime)";
            }
            else if (typename == "jyzbhg")
            {
                strsql1 = "(round((a.APPROVETIME - a.INCEPTTIME) * 24 * 60, 1)>b.aftertime)";
            }
            string strSql = $@"select distinct '普通' as 加急状态,
                               c.typeid as 类别编码,
                               c.typename as 类别名称,
                               b.pretime as 检验前合格时间,
                               b.aftertime as 检验中合格时间,
                               a.machineid as 仪器编码,
                               a.testdate as 检验日期,
                               a.sampleid as 样本号,
                               a.BARCODE as 条码号,
                               a.PATIENTTYPE as 患者类别,
                               a.PATIENTID as 患者编号,
                               a.PATIENTNAME as 姓名,
                               a.PATIENTSEX as 性别,
                               a.PATIENTAGE as 年龄,
                               a.DEPTNAME as 申请科室,
                               a.DOCTORNAME as 申请医生,
                               a.HISITEMNAMELIST as 检验目的,
                               a.NURSENAME as 采集护士,
                               a.INCEPTORNAME as 接收人,
                               a.APPROVERNAME as 审核人,
                               to_char(a.SAMPLETIME,'yyyy-MM-dd hh24:mi:ss') as 采集时间,
                               to_char(a.INCEPTTIME,'yyyy-MM-dd hh24:mi:ss') as 接收时间,
                               to_char(a.APPROVETIME,'yyyy-MM-dd hh24:mi:ss') as 审核时间,
                               round((a.INCEPTTIME - a.SAMPLETIME) * 24 * 60, 1) as 检验前周转时间,
                               round((a.APPROVETIME - a.INCEPTTIME) * 24 * 60, 1) as 检验中周转时间
                      from view_las_sap_samplereg a, las_qua_item c, las_qua_itemtype b
                     where a.HISITEMIDLIST like '%' || c.hisitemid || '%'
                       and c.typeid = b.typeid
                       and a.PATIENTTYPE in ('门诊', '住院')
                       and a.CONFIRMSTATE = '1'
                       and a.EMC = '0'
                       and c.typeid='{typeid}'
                       and a.testdate >= '{begDate}'
                       and a.testdate <= '{endDate}'
                       and a.groupid in ({groupid})
                       and b.emc = '普通'
                       and {strsql1}
                    union all

                    select distinct '加急' as 加急状态,
                          c.typeid as 类别编码,
                           c.typename as 类别名称,
                           b.pretime as 检验前合格时间,
                           b.aftertime as 检验中合格时间,
                           a.machineid as 仪器编码,
                           a.testdate as 检验日期,
                           a.sampleid as 样本号,
                           a.BARCODE as 条码号,
                           a.PATIENTTYPE as 患者类别,
                           a.PATIENTID as 患者编号,
                           a.PATIENTNAME as 姓名,
                           a.PATIENTSEX as 性别,
                           a.PATIENTAGE as 年龄,
                           a.DEPTNAME as 申请科室,
                           a.DOCTORNAME as 申请医生,
                           a.HISITEMNAMELIST as 检验目的,
                           a.NURSENAME as 采集护士,
                           a.INCEPTORNAME as 接收人,
                           a.APPROVERNAME as 审核人,
                           to_char(a.SAMPLETIME,'yyyy-MM-dd hh24:mi:ss') as 采集时间,
                           to_char(a.INCEPTTIME,'yyyy-MM-dd hh24:mi:ss') as 接收时间,
                           to_char(a.APPROVETIME,'yyyy-MM-dd hh24:mi:ss') as 审核时间,
                           round((a.INCEPTTIME - a.SAMPLETIME) * 24 * 60, 1) as 检验前周转时间,
                           round((a.APPROVETIME - a.INCEPTTIME) * 24 * 60, 1) as 检验中周转时间
                      from view_las_sap_samplereg a, las_qua_item c, las_qua_itemtype b
                     where a.HISITEMIDLIST like '%' || c.hisitemid || '%'
                       and c.typeid = b.typeid
                       and a.PATIENTTYPE in ('门诊', '住院')
                       and a.CONFIRMSTATE = '1'
                       and a.EMC = '1'
                        and c.typeid='{typeid}'
                       and a.testdate >= '{begDate}'
                       and a.testdate <= '{endDate}'
                       and a.groupid in ({groupid})
                       and b.emc = '加急'
                        and {strsql1}
                    ";
            DataTable dt = OracleHelp.Query(strSql);
            return dt;

        }
        public List<Models.ModelQuatity.sysConfig> getsysConfig()
        {
            List<Models.ModelQuatity.sysConfig> alSysconfig = new List<Models.ModelQuatity.sysConfig>();
            Models.ModelQuatity.sysConfig model = new Models.ModelQuatity.sysConfig();
            string strSql = "select a.typeclass,a.sqlindex,a.sql,a.memo from las_qua_sysconfig  a";
            DataTable dt = OracleHelp.Query(strSql);
            foreach (DataRow dr in dt.Rows)
            {
                model = new Models.ModelQuatity.sysConfig();
                model.typeclass = dr["typeclass"].ToString();
                model.sqlindex = dr["sqlindex"].ToString();
                model.sql = dr["sql"].ToString();
                model.memo = dr["memo"].ToString();
                alSysconfig.Add(model);
            }
            return alSysconfig;
        }
        /// <summary>
        /// 查询病区未确认采集标本率
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public DataTable getListNoSampletimeRate(string begDate, string endDate, string groupid)
        {
            //List<Models.ModelQuatity.quatitydata> badList = new List<Models.ModelQuatity.quatitydata>();
            string strSql = $@"
                            select PATIENTTYPE as 患者类型,--typememo
                                  WARDID as 病区编码, --typereason
                                   WARDNAME as 病区名称,--typeClass
                                   count(BARCODE) 总标本数,--totalnum
                                    sum(bz) as 未确认采集标本数,--typenum
                                   round((sum(bz) / count(BARCODE)) * 100, 2) as 未确认采集率--typerate
                              from (select distinct a.BARCODE ,a.PATIENTTYPE,
                                                    a.WARDID,
                                                    a.WARDNAME,
                                                    decode(to_char(a.SAMPLETIME, 'yyyyMMdd'), '00010101',1,0) as bz
                                      from view_las_sap_samplereg a
                                     where  a.testdate >= '{begDate}'
                                       and a.testdate <= '{endDate}'
                                       and a.GROUPID in ({groupid})
                                       and a.WARDID is not null)
                             group by PATIENTTYPE,WARDID, WARDNAME 
                              order by PATIENTTYPE,(sum(bz) / count(BARCODE)) ";
            Log.WriteLog("查询未确认采集标本率：" + strSql);
            // badList = OracleHelp.QueryListByEmit<Models.ModelQuatity.quatitydata>(strSql);
            DataTable badList = OracleHelp.Query(strSql);
            return badList;
        }

        /// <summary>
        /// 根据类型ID获取字典列表
        /// </summary>
        /// <param name="typeId">类型ID</param>
        /// <returns>字典列表</returns>
        public List<Models.SysDictionary> GetByTypeId(string typeId)
        {
            // 由于QueryListByEmit只接受1个参数，需要将typeId直接拼接到SQL中

            string sql = $@"
        SELECT a.sequences,
               a.typeid,
               a.dicid,
               a.shortcut,
               a.dicname,
               a.showorder,
               a.memo1,
               a.memo2,
               a.memo3,
               a.isshow,
               a.dicclass,
               a.memo4,
               a.memo5,
               a.isopenedit,
               a.lspmapping,
               a.lspmappingname,
               a.memo6,
               a.memo7,
               a.memo8,
               a.memo9,
               a.memo10
        FROM winlis.LAS_SYS_DICTIONARY a
        WHERE a.TYPEID = '{typeId}' ";


            return OracleHelp.QueryListByEmit<Models.SysDictionary>(sql);

        }

        /// <summary>
        /// 根据条码号和检验类型更新样本登记表 Created by Zane Xu 20260905
        /// </summary>
        /// <param name="barcode">条码号</param>
        /// <param name="testType">检验类型：Microbiology（微生物）/ Routine（常规）</param>
        /// <returns>更新结果信息</returns>
        public string UpdateSampleReg(string barcode, TestType testType)
        {
            // 验证条码号不为空
            if (string.IsNullOrWhiteSpace(barcode))
            {
                return "条码号不能为空";
            }

            // 转义单引号防止SQL注入
            string safeBarcode = barcode;

            // 根据检验类型选择表名
            string tableName;
            string testTypeName;
            switch (testType)
            {
                case TestType.Microbiology:
                    tableName = "las_gm_samplereg";
                    testTypeName = "微生物";
                    break;
                case TestType.Routine:
                    tableName = "las_sap_samplereg";
                    testTypeName = "常规";
                    break;
                default:
                    return $"不支持的检验类型: {testType}";
            }

            // 先查询是否存在该条码
            string checkSql = $@"
        SELECT COUNT(*) 
        FROM {tableName} 
        WHERE barcode = '{safeBarcode}'";

            DataTable dt = OracleHelp.Query(checkSql);
            if (dt == null || dt.Rows.Count == 0 || Convert.ToInt32(dt.Rows[0][0]) == 0)
            {
                return $"条码号 '{barcode}' 在{testTypeName}检验表中不存在";
            }

            // 执行更新
            string updateSql = $@"
        UPDATE {tableName}
        SET lsptestform = SUBSTR(lsptestform, 1, 1) || '0' || SUBSTR(state2, 3)
        WHERE SUBSTR(state2, 10, 1) = '1' 
          AND SUBSTR(lsptestform, 2, 1) != '1'
          AND barcode = '{safeBarcode}'";

            int affectedRows = OracleHelp.ExecuteNonQuery(updateSql);

            if (affectedRows > 0)
            {
                return $"{testTypeName}检验更新成功，共更新 {affectedRows} 条记录";
            }
            else
            {
                return $"{testTypeName}检验没有符合条件的记录需要更新（可能条件不满足）";
            }
        }

        /// <summary>
        /// 根据条件查询样本登记信息（直接拼接SQL）
        /// </summary>
        public DataTable QuerySampleReg(DateTime startDate, DateTime endDate,string barcode, string patientId)
        {
            // 直接拼接SQL
            string strSql = @"
                                    SELECT a.testdate, a.barcode,
                                           CASE SUBSTR(a.lsptestform, 2, 1)
                                               WHEN '0' THEN '未推送'
                                               WHEN '1' THEN '已推送'
                                               WHEN '2' THEN '推送失败'
                                               ELSE SUBSTR(a.lsptestform, 2, 1)
                                           END AS push_status,
                                           CASE SUBSTR(a.state2, 10, 1)
                                               WHEN '0' THEN '未生成PDF'
                                               WHEN '1' THEN '已生成PDF'
                                               ELSE SUBSTR(a.state2, 10, 1)
                                           END AS report_status,
                                           a.machineid, a.machinename, a.patientid, a.patientseq,
                                           a.patientsex, a.patientage, a.hisitemidlist, a.hisitemnamelist
                                      FROM winlis.v_dc_view_las_sap_samplereg a
                                     WHERE a.testdate >= '" + startDate.ToString("yyyyMMdd") + "'" +
                                             "   AND a.testdate <= '" + endDate.ToString("yyyyMMdd") + "'";

            // 条码号（可空）
            if (!string.IsNullOrWhiteSpace(barcode))
            {
                strSql += "   AND a.barcode = '" + barcode.Trim().Replace("'", "''") + "'";
            }

            // 患者号（可空）
            if (!string.IsNullOrWhiteSpace(patientId))
            {
                strSql += "   AND a.PATIENTID = '" + patientId.Trim().Replace("'", "''") + "'";
            }

            strSql += " ORDER BY a.testdate DESC";

            // 直接返回DataTable
            return OracleHelp.Query(strSql);
        }
    }
    }
