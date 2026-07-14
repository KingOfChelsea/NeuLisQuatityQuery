using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeuLisQuatityQuery
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void 查询1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void 标本质量指标统计ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmQuatityQuery frm = new frmQuatityQuery();
            frm.MdiParent = this;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }

        private void 年度TAT周转统计ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.MdiParent = this;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // 找到 MdiClient 控件
            MdiClient mdiClient = this.Controls.OfType<MdiClient>().FirstOrDefault();
            if (mdiClient != null)
            {
                // 直接从资源或文件加载背景图
                mdiClient.BackgroundImage = Properties.Resources.backgroud; ;
                mdiClient.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }
    }
}
