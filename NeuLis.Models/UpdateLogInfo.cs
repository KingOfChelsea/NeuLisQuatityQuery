using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuLis.Models
{
    /// <summary>
    /// 更新操作日志实体类
    /// </summary>
    public class UpdateLogInfo
    {
        /// <summary>
        /// 条码号
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 检验类型（微生物检验/常规检验）
        /// </summary>
        public string TestType { get; set; }

        /// <summary>
        /// 操作类型（如：更新360PDF）
        /// </summary>
        public string OperateType { get; set; }

        /// <summary>
        /// 操作结果信息
        /// </summary>
        public string OperateResult { get; set; }

        /// <summary>
        /// 状态（SUCCESS/FAILED）
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
