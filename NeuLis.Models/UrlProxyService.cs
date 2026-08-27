using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace NeuLis.Models
{
    /// <summary>
    /// 完整HTTP代理转发服务 - 支持所有HTTP方法和请求头
    /// </summary>
    public class UrlProxyService : IDisposable
    {
        private HttpListener listener;
        private Thread listenThread;
        private readonly string targetBaseUrl;
        private readonly int localPort;
        private bool isRunning;

        public string LocalUrl => $"http://127.0.0.1:{localPort}/";

        public UrlProxyService(string targetUrl, int port = 56789)
        {
            // 提取基础URL（去掉查询参数）
            Uri uri = new Uri(targetUrl);
            this.targetBaseUrl = $"{uri.Scheme}://{uri.Host}:{uri.Port}";
            this.localPort = port;
        }

        public void Start()
        {
            if (isRunning) return;

            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add($"http://127.0.0.1:{localPort}/");
                listener.Start();
                isRunning = true;

                listenThread = new Thread(ListenLoop)
                {
                    IsBackground = true,
                    Name = "FullProxyThread"
                };
                listenThread.Start();
            }
            catch (Exception ex)
            {
                throw new Exception($"启动代理失败：{ex.Message}");
            }
        }

        private void ListenLoop()
        {
            while (isRunning && listener != null && listener.IsListening)
            {
                try
                {
                    var context = listener.GetContext();
                    ThreadPool.QueueUserWorkItem(state => ProcessRequest(context));
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            try
            {
                // 构建目标URL（保留原始查询参数和路径）
                string targetUrl = BuildTargetUrl(request);

                // 创建对目标服务器的请求
                HttpWebRequest targetRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
                targetRequest.Method = request.HttpMethod;
                targetRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";
                targetRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                targetRequest.KeepAlive = true;
                targetRequest.Timeout = 120000; // 2分钟超时
                targetRequest.ReadWriteTimeout = 120000;
                targetRequest.AllowAutoRedirect = true;
                targetRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                // 复制请求头
                CopyRequestHeaders(request, targetRequest);

                // 处理请求体（POST/PUT等）
                if (request.HasEntityBody)
                {
                    using (var bodyStream = request.InputStream)
                    using (var memoryStream = new MemoryStream())
                    {
                        bodyStream.CopyTo(memoryStream);
                        byte[] bodyBytes = memoryStream.ToArray();

                        if (bodyBytes.Length > 0)
                        {
                            targetRequest.ContentLength = bodyBytes.Length;
                            using (var targetStream = targetRequest.GetRequestStream())
                            {
                                targetStream.Write(bodyBytes, 0, bodyBytes.Length);
                            }
                        }
                    }
                }

                // 获取目标服务器响应
                try
                {
                    using (HttpWebResponse targetResponse = (HttpWebResponse)targetRequest.GetResponse())
                    {
                        // 复制响应状态
                        response.StatusCode = (int)targetResponse.StatusCode;
                        response.StatusDescription = targetResponse.StatusDescription;

                        // 复制响应头
                        response.ContentType = targetResponse.ContentType;

                        // 复制内容长度（如果有）
                        if (targetResponse.ContentLength > 0)
                        {
                            response.ContentLength64 = targetResponse.ContentLength;
                        }

                        // 转发响应体
                        using (var responseStream = targetResponse.GetResponseStream())
                        {
                            byte[] buffer = new byte[65536]; // 64KB缓冲区
                            int bytesRead;
                            while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                response.OutputStream.Write(buffer, 0, bytesRead);
                                response.OutputStream.Flush();
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    // 处理HTTP错误响应（如404, 500等）
                    if (ex.Response != null)
                    {
                        using (HttpWebResponse errorResponse = (HttpWebResponse)ex.Response)
                        {
                            response.StatusCode = (int)errorResponse.StatusCode;
                            response.ContentType = errorResponse.ContentType;

                            using (var errorStream = errorResponse.GetResponseStream())
                            {
                                errorStream.CopyTo(response.OutputStream);
                            }
                        }
                    }
                    else
                    {
                        throw;
                    }
                }

                response.Close();
            }
            catch (Exception ex)
            {
                SendErrorPage(response, ex.Message);
            }
        }

        private string BuildTargetUrl(HttpListenerRequest request)
        {
            // 获取原始请求的路径和查询字符串
            string path = request.Url.AbsolutePath;
            string query = request.Url.Query;

            // 如果是根路径，添加完整的报表URL
            if (path == "/" || string.IsNullOrEmpty(path))
            {
                // 这里使用你的完整报表URL
                return "http://10.161.211.95:8087/report/Report-EntryAction.do?reportId=REPORT-CBC2FB77E9100001D841365AE3D0A800";
            }

            // 否则拼接路径
            return $"{targetBaseUrl}{path}{query}";
        }

        private void CopyRequestHeaders(HttpListenerRequest source, HttpWebRequest target)
        {
            foreach (string key in source.Headers.AllKeys)
            {
                try
                {
                    switch (key.ToLower())
                    {
                        case "host":
                        case "connection":
                        case "content-length":
                        case "transfer-encoding":
                        case "keep-alive":
                            // 这些头由系统自动处理，跳过
                            break;
                        case "referer":
                            target.Referer = source.Headers[key];
                            break;
                        case "user-agent":
                            // 使用我们自己的User-Agent
                            break;
                        case "content-type":
                            target.ContentType = source.Headers[key];
                            break;
                        case "accept":
                            target.Accept = source.Headers[key];
                            break;
                        default:
                            target.Headers[key] = source.Headers[key];
                            break;
                    }
                }
                catch
                {
                    // 忽略无法设置的头部
                }
            }
        }

        private void SendErrorPage(HttpListenerResponse response, string errorMessage)
        {
            try
            {
                string html = $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>报表加载中...</title>
    <style>
        body {{ font-family: 'Microsoft YaHei', sans-serif; padding: 40px; text-align: center; background: #f5f5f5; }}
        .container {{ max-width: 600px; margin: 100px auto; background: white; padding: 40px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        h2 {{ color: #e74c3c; }}
        p {{ color: #666; line-height: 1.6; }}
        .loading {{ font-size: 18px; color: #3498db; }}
        .spinner {{ border: 4px solid #f3f3f3; border-top: 4px solid #3498db; border-radius: 50%; width: 40px; height: 40px; animation: spin 1s linear infinite; margin: 20px auto; }}
        @keyframes spin {{ 0% {{ transform: rotate(0deg); }} 100% {{ transform: rotate(360deg); }} }}
    </style>
    <script>
        // 自动重试
        setTimeout(function() {{ location.reload(); }}, 3000);
    </script>
</head>
<body>
    <div class='container'>
        <div class='spinner'></div>
        <h2>报表加载中...</h2>
        <p class='loading'>正在连接到报表服务器，请稍候...</p>
        <p style='color:#999;font-size:12px;'>如果长时间无响应，请点击下方按钮重试</p>
        <button onclick='location.reload()' style='padding:10px 30px;background:#3498db;color:white;border:none;border-radius:4px;cursor:pointer;margin-top:10px;'>重新加载</button>
        <p style='color:#ccc;font-size:11px;margin-top:20px;'>{errorMessage}</p>
    </div>
</body>
</html>";

                byte[] buffer = Encoding.UTF8.GetBytes(html);
                response.ContentType = "text/html; charset=utf-8";
                response.ContentLength64 = buffer.Length;
                response.StatusCode = 200;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.Close();
            }
            catch
            {
                // 忽略发送错误页时的异常
            }
        }

        public void Stop()
        {
            isRunning = false;
            try { listener?.Stop(); } catch { }
            try { listener?.Close(); } catch { }
        }

        public void Dispose()
        {
            Stop();
            try { listener?.Abort(); } catch { }
        }
    }
}