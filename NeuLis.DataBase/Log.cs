using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace NeuLis.DataBase
{
    /// <summary>
    /// 日志记录类 - 提供统一的日志记录功能
    /// </summary>
    /// <remarks>
    /// 原始作者: 元宝
    /// 编辑者: 徐振宇
    /// 创建日期: 2026-09-05
    /// 最后修改日期: 2026-09-06
    /// 版本: 2.0
    /// 
    /// 修改历史:
    /// 2026-09-06 徐振宇 - 优化日志格式，增加线程安全机制，添加自动备份和清理功能，增加IP记录功能
    /// 2026-09-05 元宝 - 初始版本创建
    /// </remarks>
    public class Log
    {
        #region 枚举定义

        /// <summary>
        /// 日志级别枚举
        /// </summary>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        public enum LogLevel
        {
            /// <summary>调试信息 - 最详细，用于开发和调试阶段</summary>
            DEBUG = 0,
            /// <summary>普通信息 - 记录系统正常运行状态</summary>
            INFO = 1,
            /// <summary>警告信息 - 可能的问题但不影响运行</summary>
            WARN = 2,
            /// <summary>错误信息 - 发生了错误但系统可以继续运行</summary>
            ERROR = 3,
            /// <summary>致命错误 - 严重错误可能导致系统崩溃</summary>
            FATAL = 4
        }

        #endregion

        #region 私有字段

        /// <summary>
        /// 最小日志级别，低于此级别的日志将被忽略
        /// </summary>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        private static LogLevel _minLogLevel = LogLevel.DEBUG;

        /// <summary>
        /// 单个日志文件的最大大小（字节），默认10MB
        /// </summary>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        private static long _maxFileSize = 10 * 1024 * 1024;

        /// <summary>
        /// 日志文件的保留天数，超过此天数的日志将被自动清理
        /// </summary>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        private static int _retentionDays = 30;

        /// <summary>
        /// 线程同步锁对象，保证多线程环境下的写入安全
        /// </summary>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        private static readonly object _lockObj = new object();

        #endregion

        #region 公共属性

        /// <summary>
        /// 获取或设置最小日志级别
        /// 低于此级别的日志将不会被记录
        /// 默认值: DEBUG
        /// </summary>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        public static LogLevel MinLogLevel
        {
            get => _minLogLevel;
            set => _minLogLevel = value;
        }

        /// <summary>
        /// 获取或设置单个日志文件的最大大小（字节）
        /// 超过此大小会自动备份并创建新文件
        /// 默认值: 10MB (10 * 1024 * 1024)
        /// </summary>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        public static long MaxFileSize
        {
            get => _maxFileSize;
            set => _maxFileSize = value;
        }

        /// <summary>
        /// 获取或设置日志文件的保留天数
        /// 超过此天数的日志文件会被自动删除
        /// 默认值: 30天
        /// </summary>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        public static int RetentionDays
        {
            get => _retentionDays;
            set => _retentionDays = value;
        }

        #endregion

        #region 公开方法 - 写日志（无IP参数，兼容旧版本）

        /// <summary>
        /// 写入日志信息（兼容旧版本调用方式）
        /// 默认使用 INFO 级别，不记录IP
        /// </summary>
        /// <param name="msg">要记录的日志消息内容</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// 修改说明: 增加日志级别过滤和线程安全机制
        /// </remarks>
        public static void WriteLog(string msg)
        {
            WriteLog(LogLevel.INFO, msg, "");
        }

        #endregion

        #region 公开方法 - 写日志（带IP参数）

        /// <summary>
        /// 写入日志信息（带IP地址记录）
        /// 默认使用 INFO 级别
        /// </summary>
        /// <param name="msg">要记录的日志消息内容</param>
        /// <param name="ip">客户端IP地址</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// 修改说明: 增加IP地址记录功能
        /// </remarks>
        public static void WriteLog(string msg, string ip)
        {
            WriteLog(LogLevel.INFO, msg, ip);
        }

        /// <summary>
        /// 写入指定级别的日志信息（带IP地址记录）
        /// 如果指定的级别低于 MinLogLevel 设置，则不会记录
        /// </summary>
        /// <param name="level">日志级别（DEBUG/INFO/WARN/ERROR/FATAL）</param>
        /// <param name="msg">要记录的日志消息内容</param>
        /// <param name="ip">客户端IP地址</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// 修改说明: 
        /// 1. 增加日志级别过滤功能
        /// 2. 增加线程同步锁机制
        /// 3. 增加文件大小自动备份功能
        /// 4. 增加过期日志自动清理功能
        /// 5. 增加日志系统异常处理机制
        /// 6. 增加IP地址记录功能
        /// </remarks>
        public static void WriteLog(LogLevel level, string msg, string ip)
        {
            // 判断日志级别是否满足记录条件
            if (level < _minLogLevel) return;

            // 使用锁保证多线程环境下的写入顺序和安全
            lock (_lockObj)
            {
                try
                {
                    // 构建日志文件名：按小时生成，如 "2026090614.txt"
                    string logFileName = DateTime.Now.ToString("yyyyMMddHH") + ".txt";

                    // 日志文件存储路径：程序运行目录下的 sqlLog 文件夹
                    string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sqlLog");

                    // 完整的日志文件路径
                    string fullPath = Path.Combine(logPath, logFileName);

                    // 如果日志目录不存在，则创建
                    if (!Directory.Exists(logPath))
                    {
                        Directory.CreateDirectory(logPath);
                    }

                    // 检查当前日志文件是否超过大小限制，超过则自动备份
                    CheckAndBackupFile(fullPath);

                    // 追加写入日志内容
                    using (StreamWriter writer = File.AppendText(fullPath))
                    {
                        ALog(level, msg, ip, writer);
                        writer.Close();
                    }

                    // 清理过期的历史日志文件
                    CleanupOldLogs(logPath);
                }
                catch (Exception ex)
                {
                    // 日志系统自身出现异常时的应急处理
                    HandleLogSystemError(ex);
                }
            }
        }

        #endregion

        #region 公开方法 - 便捷日志记录（不带IP）

        /// <summary>
        /// 记录调试级别日志
        /// 用于开发和调试阶段的详细信息输出
        /// </summary>
        /// <param name="msg">调试信息内容</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        public static void Debug(string msg) => WriteLog(LogLevel.DEBUG, msg, "");

        /// <summary>
        /// 记录信息级别日志
        /// 用于记录系统正常运行的状态信息
        /// </summary>
        /// <param name="msg">信息内容</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        public static void Info(string msg) => WriteLog(LogLevel.INFO, msg, "");

        /// <summary>
        /// 记录警告级别日志
        /// 用于记录可能出现问题但不影响系统正常运行的情况
        /// </summary>
        /// <param name="msg">警告信息内容</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        public static void Warn(string msg) => WriteLog(LogLevel.WARN, msg, "");

        /// <summary>
        /// 记录错误级别日志
        /// 用于记录发生了错误但系统仍可继续运行的情况
        /// </summary>
        /// <param name="msg">错误信息内容</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        public static void Error(string msg) => WriteLog(LogLevel.ERROR, msg, "");

        /// <summary>
        /// 记录致命级别日志
        /// 用于记录可能导致系统崩溃的严重错误
        /// </summary>
        /// <param name="msg">致命错误信息内容</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        public static void Fatal(string msg) => WriteLog(LogLevel.FATAL, msg, "");

        #endregion

        #region 公开方法 - 便捷日志记录（带IP）

        /// <summary>
        /// 记录调试级别日志（带IP地址）
        /// 用于开发和调试阶段的详细信息输出
        /// </summary>
        /// <param name="msg">调试信息内容</param>
        /// <param name="ip">客户端IP地址</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        public static void Debug(string msg, string ip) => WriteLog(LogLevel.DEBUG, msg, ip);

        /// <summary>
        /// 记录信息级别日志（带IP地址）
        /// 用于记录系统正常运行的状态信息
        /// </summary>
        /// <param name="msg">信息内容</param>
        /// <param name="ip">客户端IP地址</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        public static void Info(string msg, string ip) => WriteLog(LogLevel.INFO, msg, ip);

        /// <summary>
        /// 记录警告级别日志（带IP地址）
        /// 用于记录可能出现问题但不影响系统正常运行的情况
        /// </summary>
        /// <param name="msg">警告信息内容</param>
        /// <param name="ip">客户端IP地址</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        public static void Warn(string msg, string ip) => WriteLog(LogLevel.WARN, msg, ip);

        /// <summary>
        /// 记录错误级别日志（带IP地址）
        /// 用于记录发生了错误但系统仍可继续运行的情况
        /// </summary>
        /// <param name="msg">错误信息内容</param>
        /// <param name="ip">客户端IP地址</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        public static void Error(string msg, string ip) => WriteLog(LogLevel.ERROR, msg, ip);

        /// <summary>
        /// 记录致命级别日志（带IP地址）
        /// 用于记录可能导致系统崩溃的严重错误
        /// </summary>
        /// <param name="msg">致命错误信息内容</param>
        /// <param name="ip">客户端IP地址</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// </remarks>
        public static void Fatal(string msg, string ip) => WriteLog(LogLevel.FATAL, msg, ip);

        #endregion

        #region 公开方法 - 异常记录

        /// <summary>
        /// 记录异常信息的详细日志
        /// 包含异常类型、消息、堆栈跟踪和内部异常等信息
        /// </summary>
        /// <param name="ex">要记录的异常对象</param>
        /// <param name="additionalInfo">附加说明信息（可选）</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// 修改说明: 增加多层内部异常的递归记录功能
        /// </remarks>
        public static void WriteException(Exception ex, string additionalInfo = "")
        {
            WriteException(ex, additionalInfo, "");
        }

        /// <summary>
        /// 记录异常信息的详细日志（带IP地址）
        /// 包含异常类型、消息、堆栈跟踪和内部异常等信息
        /// </summary>
        /// <param name="ex">要记录的异常对象</param>
        /// <param name="additionalInfo">附加说明信息（可选）</param>
        /// <param name="ip">客户端IP地址（可选）</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// 修改说明: 增加IP地址记录功能
        /// </remarks>
        public static void WriteException(Exception ex, string additionalInfo = "", string ip = "")
        {
            // 参数验证
            if (ex == null) return;

            // 构建异常信息的完整描述
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"【异常类型】: {ex.GetType().FullName}");
            sb.AppendLine($"【异常消息】: {ex.Message}");
            sb.AppendLine($"【堆栈跟踪】: {ex.StackTrace}");

            // 如果有IP地址则追加
            if (!string.IsNullOrWhiteSpace(ip))
            {
                sb.AppendLine($"【客户端IP】: {ip}");
            }

            // 如果有附加信息则追加
            if (!string.IsNullOrWhiteSpace(additionalInfo))
            {
                sb.AppendLine($"【附加信息】: {additionalInfo}");
            }

            // 递归记录内部异常信息
            Exception innerEx = ex.InnerException;
            int innerLevel = 1;
            while (innerEx != null)
            {
                sb.AppendLine($"--- 内部异常(第{innerLevel}层) ---");
                sb.AppendLine($"【异常类型】: {innerEx.GetType().FullName}");
                sb.AppendLine($"【异常消息】: {innerEx.Message}");
                sb.AppendLine($"【堆栈跟踪】: {innerEx.StackTrace}");

                innerEx = innerEx.InnerException;
                innerLevel++;
            }

            // 以ERROR级别记录异常信息
            WriteLog(LogLevel.ERROR, sb.ToString(), ip);
        }

        #endregion

        #region 私有方法 - 核心日志写入

        /// <summary>
        /// 核心日志写入方法（无IP参数，兼容旧版本）
        /// 按照固定格式将日志内容写入文本写入器
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="logMessage">日志消息内容</param>
        /// <param name="writer">文本写入器实例</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// 修改说明: 优化日志输出格式，增加线程编号显示
        /// </remarks>
        private static void ALog(LogLevel level, string logMessage, TextWriter writer)
        {
            ALog(level, logMessage, "", writer);
        }

        /// <summary>
        /// 核心日志写入方法（带IP参数）
        /// 按照固定格式将日志内容写入文本写入器
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="logMessage">日志消息内容</param>
        /// <param name="ip">客户端IP地址</param>
        /// <param name="writer">文本写入器实例</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// 修改说明: 增加IP地址记录功能
        /// </remarks>
        private static void ALog(LogLevel level, string logMessage, string ip, TextWriter writer)
        {
            // 写入日志分隔线和格式化内容
            writer.Write("\r\n");
            writer.WriteLine("========================================================================");
            writer.WriteLine($"  【日志级别】: {level,-5}");
            writer.WriteLine($"  【记录时间】: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            writer.WriteLine($"  【线程编号】: {Thread.CurrentThread.ManagedThreadId}");
            
            // 如果有IP地址则记录
            if (!string.IsNullOrWhiteSpace(ip))
            {
                writer.WriteLine($"  【客户端IP】: {ip}");
            }
            
            writer.WriteLine($"  【日志内容】: {logMessage}");
            writer.WriteLine("========================================================================");
            writer.Flush();
        }

        #endregion

        #region 私有方法 - 文件管理

        /// <summary>
        /// 检查日志文件大小，超过限制时自动备份
        /// 备份文件命名格式：原文件名_备份时间戳.bak
        /// </summary>
        /// <param name="filePath">要检查的日志文件完整路径</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// 修改说明: 增加文件大小检测和自动备份功能
        /// </remarks>
        private static void CheckAndBackupFile(string filePath)
        {
            // 检查文件是否存在且超过大小限制
            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length >= _maxFileSize)
                {
                    // 生成备份文件名：在原文件名后加上备份时间戳
                    string backupFileName = Path.GetFileNameWithoutExtension(filePath) +
                                           "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bak";
                    string backupPath = Path.Combine(Path.GetDirectoryName(filePath), backupFileName);

                    // 执行文件移动（备份）
                    File.Move(filePath, backupPath);
                }
            }
        }

        /// <summary>
        /// 清理超过保留期限的历史日志文件
        /// 定期执行以释放磁盘空间
        /// </summary>
        /// <param name="logPath">日志文件所在目录路径</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// 修改说明: 增加自动清理过期日志功能
        /// </remarks>
        private static void CleanupOldLogs(string logPath)
        {
            try
            {
                // 检查目录是否存在
                if (!Directory.Exists(logPath)) return;

                // 计算保留期限的截止日期
                DateTime cutoffDate = DateTime.Now.AddDays(-_retentionDays);

                // 查找所有超过保留期限的 .txt 日志文件
                var oldFiles = Directory.GetFiles(logPath, "*.txt")
                                       .Select(f => new FileInfo(f))
                                       .Where(f => f.CreationTime < cutoffDate);

                // 逐个删除过期文件
                foreach (var file in oldFiles)
                {
                    try
                    {
                        File.Delete(file.FullName);
                    }
                    catch
                    {
                        // 单个文件删除失败不影响其他文件的清理
                    }
                }
            }
            catch
            {
                // 清理过程中的异常不影响主流程
            }
        }

        #endregion

        #region 私有方法 - 错误处理

        /// <summary>
        /// 处理日志系统自身的异常
        /// 当日志写入发生错误时，尝试写入应急日志文件
        /// </summary>
        /// <param name="ex">日志系统发生的异常</param>
        /// <remarks>
        /// 编辑者: 徐振宇
        /// 修改日期: 2026-09-06
        /// 修改说明: 增加日志系统异常的自恢复机制
        /// </remarks>
        private static void HandleLogSystemError(Exception ex)
        {
            try
            {
                // 尝试将错误写入应急日志文件
                string errorLogPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "LogSystemError.txt"
                );

                string errorMsg = string.Format(
                    "[{0}] 日志系统异常:\r\n异常类型: {1}\r\n异常消息: {2}\r\n堆栈跟踪: {3}\r\n",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    ex.GetType().Name,
                    ex.Message,
                    ex.StackTrace
                );

                File.AppendAllText(errorLogPath, errorMsg);
            }
            catch
            {
                // 如果连应急日志都无法写入，则忽略此异常
                // 避免引发级联异常导致系统崩溃
            }
        }

        #endregion
    }
}