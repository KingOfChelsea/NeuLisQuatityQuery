using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuLis.Models
{
    public class Model
    {
        public class RejectReason
        {
            public string reason { get; set; }
            public string memo3 { get; set; }
            public string showorder { get; set; }
        }
        public class MonthData
        {
            public string month { get; set; } //  月份 "01"-"12"
            public string reason { get; set; } // 数量（字符串类型）
            public string monthnum { get; set; }  // 拒收原因（仅在GetMonthNum中有）
        }
        public class MonthRejData
        {
            public string reason { get; set; }
            public string month { get; set; }
            public string monthnum { get; set; }
        }
        public class QuaShowData
        {
            public string PatientType { get; set; }
            public string typeClass { get; set; }
            public string TypeID { get; set; }
            public string Typename { get; set; }
            public string Typefx { get; set; }
            public string Typemb { get; set; }
            public string Jan { get; set; }
            public string Feb { get; set; }
            public string Mar { get; set; }
            public string Apr { get; set; }
            public string May { get; set; }
            public string Jun { get; set; }
            public string Jul { get; set; }
            public string Aug { get; set; }
            public string Sep { get; set; }
            public string Oct { get; set; }
            public string Nov { get; set; }
            public string Dec { get; set; }
            public string Qst { get; set; }
        }
        public class AroundMonthData
        {
            public string month { get; set; }
            public string patientType { get; set; }
            public string classType { get; set; }
            public string monthnum { get; set; }
        }
        public class QuaShowAroundData
        {
            public string TypeID1 { get; set; }
            public string Typename1 { get; set; }
            public string Typefx1 { get; set; }
            public string Typemb1 { get; set; }
            public string Jan1 { get; set; }
            public string Feb1 { get; set; }
            public string Mar1 { get; set; }
            public string Apr1 { get; set; }
            public string May1 { get; set; }
            public string Jun1 { get; set; }
            public string Jul1 { get; set; }
            public string Aug1 { get; set; }
            public string Sep1 { get; set; }
            public string Oct1 { get; set; }
            public string Nov1 { get; set; }
            public string Dec1 { get; set; }
            public string Qst1 { get; set; }
        }

        public class hisitemtype
        {
            public string hisitemid { get; set; }
            public string hisitemname { get; set; }
            public string typeid{ get; set; }
            public string typename { get; set; }

        }
        public class typeclass
        {
            public string typeID { get; set; }
            public string typeName { get; set; }
            public string patientid { get; set; }
            public string patientname { get; set; }
            public string sampletype { get; set; }
            public string hisitemnamelist { get; set; }
            public string reason { get; set; }
            public string opername { get; set; }

        }
        /// <summary>
        /// 不合格标本列表
        /// </summary>
        public class sampleReject
        {
            public string barcode { get; set; }
            public string regdate { get; set; }
            public string patientid { get; set; }
            public string patientname { get; set; }
            public string sampletype { get; set; }
            public string hisitemnamelist { get; set; }
            public string reason { get; set; }
            public string opername { get; set; }
        }
        /// <summary>
        /// 标本信息
        /// </summary>
        public class barcodeReg
        {
            public string barcode { get; set; }
            public string sampletime { get; set; }
            public string patienttype { get; set; }
            public string patientid { get; set; }
            public string patientname { get; set; }
            public string sampletype { get; set; }
            public string hisitemnamelist { get; set; }
        }
        public class lifeAlter
        {
            public string barcode { get; set; }
            public string dealdate { get; set; }
            public string itemid { get; set; }
            public string itemname { get; set; }
            public string reportvalue { get; set; }
            public string rangeinfo { get; set; }
            public string unit { get; set; }
            public string rangelimit { get; set; }
            public string machineid { get; set; }
            public string testdate { get; set; }
            public string sampleid { get; set; }
            public string sendtime { get; set; }
            public string sendname { get; set; }
            public string phonetime { get; set; }
            public string phoneanswer { get; set; }
        }

        /// <summary>
        /// 错误报告单清单
        /// </summary>
        public class errSampleReg
        {
            public string barcode { get; set; }
            public string machineid { get; set; }
            public string testdate { get; set; }
            public string sampleid { get; set; }
            public string lastapprovetime { get; set; }
            public string applytime { get; set; }
            public string APPROVETIME { get; set; }
        }
    }
}
