using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuLis.Models
{
    public class NeulisDictionary
    {
        public Dictionary<string, string> montDic = new Dictionary<string, string> { { "Jan", "01" }, { "Feb", "02" },{ "Mar","03" },{ "Apr","04" }
        ,{ "May","05"},{ "Jun","06"},{ "Jul","07"},{ "Aug","08"},{ "Sep","09"},{ "Oct","10"},{ "Nov","11"},{ "Dec","12"},{ "Qst","ALL"} };
    }
    /// <summary>
    /// 系统字典实体类 - 对应LAS_SYS_DICTIONARY表
    /// </summary>
    public class SysDictionary
    {
        public string Sequences { get; set; }      // String类型
        public string TypeId { get; set; }          // String类型
        public string DicId { get; set; }           // String类型
        public string Shortcut { get; set; }        // String类型
        public string DicName { get; set; }         // String类型
        public decimal ShowOrder { get; set; }      // Decimal类型
        public string Memo1 { get; set; }           // String类型
        public string Memo2 { get; set; }           // String类型
        public string Memo3 { get; set; }           // String类型
        public string IsShow { get; set; }          // String类型（不是bool）
        public string DicClass { get; set; }        // String类型
        public string Memo4 { get; set; }           // String类型
        public string Memo5 { get; set; }           // String类型
        public string IsOpenEdit { get; set; }      // String类型（不是bool）
        public string LspMapping { get; set; }      // String类型
        public string LspMappingName { get; set; }  // String类型
        public string Memo6 { get; set; }           // String类型
        public string Memo7 { get; set; }           // String类型
        public string Memo8 { get; set; }           // String类型
        public string Memo9 { get; set; }           // String类型
        public string Memo10 { get; set; }          // String类型
    }
    /// <summary>
    /// 定义检验类型枚举 用于360报告重新推送 Create by 徐振宇 2026年9月5日11:33:36
    /// </summary>
    public enum TestType
    {
        Microbiology,  // 微生物检验
        Routine        // 常规检验
    }

    /// <summary>
    /// 
    /// </summary>
    public class SampleRegQuery
    {
        public DateTime? TestDate { get; set; }       // testdate
        public string Barcode { get; set; }           // barcode
        public string PushStatus { get; set; }         // push_status（文字）
        public string ReportStatus { get; set; }       // report_status（文字）
        public string MachineId { get; set; }          // machineid
        public string MachineName { get; set; }        // machinename
        public string PatientId { get; set; }          // patientid
        public string PatientSeq { get; set; }         // patientseq
        public string PatientSex { get; set; }         // patientsex
        public string PatientAge { get; set; }         // patientage
        public string HisItemIdList { get; set; }      // hisitemidlist
        public string HisItemNameList { get; set; }    // hisitemnamelist
    }
}
