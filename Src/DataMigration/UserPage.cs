using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataMigration
{
    public partial class UserPage : UserControl
    {
        private string _cfgPageSize = "10,20,30,50,100";
        public delegate void ClickPageButton(int current);
        public event ClickPageButton ClickPageButtonEvent;

        public delegate void ChangedPageSize();
        public event ChangedPageSize ChangedPageSizeEvent;

        public delegate void JumpPage(int jumpPage);
        public event JumpPage JumpPageEvent;

        public int TotalPages { get; set; }

        private int currentPage;
        public int CurrentPage
        {
            get { return this.currentPage; }
            set { this.currentPage = value; }
        }

        private int pageSize;
        public int PageSize
        {
            get { return this.pageSize; }
            set { this.pageSize = value; }
        }

        public ComboBox CboPageSize
        {
            set { this.cboPageSize = value; }
            get { return this.cboPageSize; }
        }

        public Label PageInfo
        {
            set { this.lblPage = value; }
            get { return this.lblPage; }
        }

        public Label TotalRows
        {
            get { return this.lblTotalRows; }
            set { this.lblTotalRows = value; }
        }

        public TextBox JumpPageCtrl
        {
            get { return this.txtJumpPage; }
            set { this.txtJumpPage = value; }
        }
        public UserPage()
        {
            InitializeComponent();
            this.InitCboCtrl();
            this.cboPageSize.TextChanged += cboPageSize_TextChanged;
            this.cboPageSize.KeyPress += cboPageSize_KeyPress;
            this.btnFrist.Tag = "F";
            this.btnPreviou.Tag = "P";
            this.btnNext.Tag = "N";
            this.btnLast.Tag = "L";
            this.btnFrist.Click += btn_Click;
            this.btnPreviou.Click += btn_Click;
            this.btnNext.Click += btn_Click;
            this.btnLast.Click += btn_Click;
            this.cboPageSize.KeyPress += cboPageSize_KeyPress;
            this.txtJumpPage.KeyPress += txtJumpPage_KeyPress;
        }
        void txtJumpPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            //text输入验证
            if (e.KeyChar == 13)
            {
                if (null != this.JumpPageEvent)
                {
                    this.JumpPageEvent(Convert.ToInt32(this.txtJumpPage.Text));
                }
            }
            else
            {
                if (e.KeyChar != 8)
                {
                    int len = this.txtJumpPage.Text.Length;
                    if (len < 1 && e.KeyChar == '0')
                    {
                        e.Handled = true;
                    }
                    else if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                    {
                        e.Handled = true;
                    }
                }
            }
        }
        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (null != this.ClickPageButtonEvent)
            {
                if (null != btn)
                {
                    switch (btn.Tag.ToString())
                    {
                        case "F":
                            this.CurrentPage = 1;
                            break;
                        case "P":
                            this.CurrentPage = this.CurrentPage <= 1 ? 1 : this.CurrentPage - 1;
                            break;
                        case "N":
                            this.CurrentPage = this.CurrentPage + 1;
                            break;
                        case "L":
                            this.CurrentPage = this.TotalPages;
                            break;
                        default:
                            this.CurrentPage = 1;
                            break;
                    }
                    this.ClickPageButtonEvent(this.CurrentPage);
                }
            }
        }
        void cboPageSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        void cboPageSize_TextChanged(object sender, EventArgs e)
        {
            this.PageSize = Convert.ToInt32(this.cboPageSize.Text);
            if (null != ChangedPageSizeEvent)
            {
                this.ChangedPageSizeEvent();
            }
        }
        private void InitCboCtrl()
        {
            this.cboPageSize.ValueMember = "MValue";
            this.cboPageSize.DisplayMember = "MText";
            this.cboPageSize.Text = string.Empty;
            if (!string.IsNullOrEmpty(_cfgPageSize))
            {
                string cfgPageSize = _cfgPageSize.Replace("，", ",");
                if (cfgPageSize.EndsWith(","))
                {
                    cfgPageSize = cfgPageSize.Remove(cfgPageSize.Length - 1);
                }
                string[] strPageSize = cfgPageSize.Split(new char[] { ',' });
                List<string> listPageSize = new List<string>();
                for (int x = 0; x < strPageSize.Length; x++)
                {
                    if (!listPageSize.Contains(strPageSize[x]) && !string.IsNullOrEmpty(strPageSize[x]))
                    {
                        listPageSize.Add(strPageSize[x]);
                    }
                }
                List<KeyAndValueEntity> kve = new List<KeyAndValueEntity>();
                for (int i = 0; i < listPageSize.Count; i++)
                {
                    kve.Add(new KeyAndValueEntity() { MValue = i, MText = listPageSize[i] });
                }
                this.cboPageSize.DataSource = kve;
            }
            else
            {
                this.cboPageSize.DataSource = new List<KeyAndValueEntity>()
                {
                    new KeyAndValueEntity() {MValue = 0,MText = "10"},
                    new KeyAndValueEntity() {MValue = 1,MText = "20"},
                    new KeyAndValueEntity() {MValue = 2,MText = "50"},
                    new KeyAndValueEntity() {MValue = 3,MText = "100"}
                };
            }
            this.cboPageSize.SelectedText = cboPageSize.Items[0] as string;
        }
    }
    internal class KeyAndValueEntity
    {
        public int MValue { get; set; }
        public string MText { get; set; }
    }
}
