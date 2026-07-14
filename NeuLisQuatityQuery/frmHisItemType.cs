using NeuLis.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace NeuLisQuatityQuery
{
    public partial class frmHisItemType : Form
    {
        public frmHisItemType()
        {
            InitializeComponent();
        }
        public List<Model.hisitemtype> alhisitemtype  = new List<Model.hisitemtype>();

        private void gridControl1_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt = ConvertListToDataTable<Model.hisitemtype>(alhisitemtype);
            this.gridControl1.DataSource =  alhisitemtype;
            this.itemtype.FieldName = "itemttype";
            this.gridView1.RefreshData();
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(this.gridView1.Columns.Count>0)
            {
                //int[] rownumber = this.gridView1.GetSelectedRows();//获取选中行号；
                //DataRow row = this.gridView1.GetDataRow(rownumber[0]);//根据行号获取相应行的数据；
                for(int i=0;i<this.gridView1.RowCount;i++)
                {
                    if(this.gridView1.IsRowSelected(i))
                    {
                        Model.hisitemtype row = this.gridView1.GetRow(i) as Model.hisitemtype;
                        row.typename = this.comboBoxEdit1.Text;
                        //this.gridView1.SetRowCellValue(i, "itemtype", this.comboBoxEdit1.Text);
                        this.gridView1.UpdateCurrentRow();
                    }
                }
                this.gridControl1.RefreshDataSource();
                this.gridView1.RefreshData();
                //this.gridView1.RefreshData();

            }
        }
        public DataTable ConvertListToDataTable<T>(List<T> list)
        {
            DataTable dataTable = new DataTable();

            if (list != null && list.Count != 0)
            {
                var propertyInfoArray = typeof(T).GetProperties();

                foreach (var propertyInfo in propertyInfoArray)
                {
                    dataTable.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
                }

                list.ForEach(item =>
                {
                    var values = propertyInfoArray.Select(p => p.GetValue(item,null));
                    dataTable.Rows.Add(values.ToArray());
                });
            }

            return dataTable;
        }

    }
}
