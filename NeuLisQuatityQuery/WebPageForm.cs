using System;
using System.Windows.Forms;

namespace NeuLisQuatityQuery
{
    public partial class WebPageForm : Form
    {
        private WebBrowser webBrowser;

        public WebPageForm(string url)
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;
            this.Text = "正在加载...";

            // 创建 WebBrowser
            webBrowser = new WebBrowser();
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.AllowWebBrowserDrop = false;
            webBrowser.IsWebBrowserContextMenuEnabled = false;

            // 重要：设置兼容性视图
            webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;

            // 导航到指定URL
            NavigateToUrl(url);

            this.Controls.Add(webBrowser);
        }

        private void NavigateToUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    MessageBox.Show("URL地址不能为空！", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    webBrowser.Navigate("about:blank");
                    return;
                }

                // 验证URL格式
                Uri uriResult;
                bool isValidUrl = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (!isValidUrl)
                {
                    url = "http://" + url;
                    isValidUrl = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                }

                if (isValidUrl)
                {
                    System.Diagnostics.Debug.WriteLine($"导航到: {url}");
                    webBrowser.Navigate(url);
                }
                else
                {
                    MessageBox.Show($"无效的URL格式: {url}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    webBrowser.Navigate("about:blank");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导航失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url == webBrowser.Url)
            {
                string title = webBrowser.Document?.Title ?? "";
                this.Text = string.IsNullOrEmpty(title) ? e.Url.ToString() : $"{title} - {e.Url}";

                // 注入 meta 标签强制使用 Edge 模式
                InjectEdgeModeMetaTag();
            }
        }

        /// <summary>
        /// 注入 meta 标签强制使用 Edge 模式渲染
        /// </summary>
        private void InjectEdgeModeMetaTag()
        {
            try
            {
                if (webBrowser.Document != null && webBrowser.Document.Body != null)
                {
                    // 获取 head 元素
                    HtmlElement head = webBrowser.Document.GetElementsByTagName("head")[0];
                    if (head != null)
                    {
                        // 创建 meta 标签
                        HtmlElement meta = webBrowser.Document.CreateElement("meta");
                        meta.SetAttribute("http-equiv", "X-UA-Compatible");
                        meta.SetAttribute("content", "IE=edge");
                        head.AppendChild(meta);

                        System.Diagnostics.Debug.WriteLine("已注入 Edge 模式 meta 标签");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"注入 meta 标签失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 刷新当前页面
        /// </summary>
        public void RefreshPage()
        {
            if (webBrowser != null)
            {
                webBrowser.Refresh();
            }
        }

        /// <summary>
        /// 获取当前URL
        /// </summary>
        public string CurrentUrl
        {
            get { return webBrowser?.Url?.ToString(); }
        }

        // 支持F5刷新
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                RefreshPage();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}