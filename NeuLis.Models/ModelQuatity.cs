using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuLis.Models
{
    public class ModelQuatity
    {
        public class lisGroup
        {
            public string groupid { get; set; }
            public string groupname { get; set; }
            public string isstate { get; set; }
        }
        public class badSampleType
        {
            /// <summary>
            /// 统计类别
            /// </summary>
            public string typereason { get; set; }
            /// <summary>
            /// 统计数据
            /// </summary>
            public string typenum { get; set; }
        }
        public class quatitydata
        {
            /// <summary>
            /// 类别
            /// </summary>
            public string typeClass { get; set; }
            /// <summary>
            /// 统计类别
            /// </summary>
            public string typereason { get; set; }
            /// <summary>
            /// 分子数据
            /// </summary>
            public string typenum { get; set; }
            /// <summary>
            /// 分母数据
            /// </summary>
            public string totalnum { get; set; }
            /// <summary>
            /// 计算比率
            /// </summary>
            public string typerate { get; set; }
            /// <summary>
            /// 计算公式
            /// </summary>
            public string typememo { get; set; }
        }
        public class itemType
        {
            public string typeid { get; set; }
            public string typename { get; set; }
            public string pretime { get; set; }
            public string aftertime { get; set; }
            public string emc { get; set; }
        }

        public class tatItem
        {
            public string emc { get; set; }
            public string typeid { get; set; }
            public string typename { get; set; }
            public string sapcount { get; set; }
            public string jyqbhg { get; set; }
            public string jyqhgl { get; set; }
            public string jyzbhg { get; set; }
            public string jyzhgl { get; set; }
            public string jyqpjs { get; set; }
            public string jyqzws { get; set; }
            public string jyq9fs { get; set; }
            public string jyzpjs { get; set; }
            public string jyzzws { get; set; }
            public string jyz9fs { get; set; }
        }
        public class sysConfig
        {
            public string typeclass { get; set; }
            public string sqlindex { get; set; }
            public string sql { get; set; }
            public string memo { get; set; }
        }


    }
}
