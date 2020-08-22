using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 远程桌面管理
{
    public partial class Form_Main : Form
    {
        private DataTable data;
        private DataProvider provider;

        public Form_Main()
        {
            InitializeComponent();

            try
            {
                provider = new DataProvider();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.toolStripComboBox_mode.Items.Add("跳过配置自动连接");
            this.toolStripComboBox_mode.Items.Add("自定义配置手动连接");
            this.toolStripComboBox_mode.SelectedIndex = 0;
            LoadData();
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            data = provider.ExecuteQuery("select * from TB_Server ");
            data.DefaultView.Sort = "Name asc";
            this.dataGridView.DataSource = null;
            this.dataGridView.DataSource = data;
        }

        private bool Update(string ID, string Name, string IP, string UserName, string Password)
        {
            return provider.ExecuteNonQuery(string.Format(
                @"update TB_Server set
                    Name='{0}',
                    IP='{1}',
                    UserName='{2}',
                    Password='{3}'
                    where ID={4}",
                 Name, IP, UserName, Password, ID)) == 1;
        }

        private bool Insert(string Name, string IP, string UserName, string Password)
        {
            return provider.ExecuteNonQuery(string.Format(
                @"insert into TB_Server
                    (Name,IP,UserName,Password)
                    values
                    ('{0}','{1}','{2}','{3}')",
                 Name, IP, UserName, Password)) == 1;
        }

        private bool Delete(string ID)
        {
            return provider.ExecuteNonQuery(string.Format(@"delete from TB_Server where ID={0}", ID)) == 1;
        }

        private void btn_Del_Click(object sender, EventArgs e)
        {
            if (this.dataGridView.SelectedRows.Count <= 0)
            {
                MessageBox.Show("请选择要操作的数据行", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            foreach (DataGridViewRow row in this.dataGridView.SelectedRows)
            {
                Delete(row.Cells["ID"].Value.ToString());
            }
            LoadData();
        }

        private void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            provider.Dispose();
        }

        private void dataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView.Columns["IP"].HeaderText = "IP";
            dataGridView.Columns["Name"].HeaderText = "名称";
            dataGridView.Columns["UserName"].HeaderText = "用户名";
            dataGridView.Columns["Password"].Visible = false;
            dataGridView.Columns["ID"].Visible = false;
            dataGridView.Columns["IP"].Width = 150;
            dataGridView.Columns["Name"].Width = 300;
        }

        private void dataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string IP = this.dataGridView.Rows[e.RowIndex].Cells["IP"].Value.ToString();
            string UserName = this.dataGridView.Rows[e.RowIndex].Cells["UserName"].Value.ToString();
            string Passwordstr = this.dataGridView.Rows[e.RowIndex].Cells["Password"].Value.ToString();
            string Base = System.Windows.Forms.Application.StartupPath;
            string Default = Path.Combine(Base, "Default.rdp");
            string str = File.ReadAllText(Default);
            //Password p = new Password();
            //string newpassword = p.Build(Passwordstr);
            string resultstr = str.Replace("###UserName###", UserName).Replace("###IP###", IP);//Environment.NewLine + "password" + Environment.NewLine + "51:b:" + newpassword;
            string result = Path.Combine(Base, "result.rdp");
            File.WriteAllText(result, resultstr);
            try
            {
                {
                    Process rdcProcess = new Process();
                    rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
                    rdcProcess.StartInfo.Arguments = "/generic:TERMSRV/" + IP + " /user:" + UserName + " /password:" + Passwordstr;
                    rdcProcess.Start();
                }
                {
                    Process rdcProcess = new Process();
                    rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe");

                    if (this.toolStripComboBox_mode.SelectedIndex == 0)
                        rdcProcess.StartInfo.Arguments = "\"" + result + "\"";
                    else if (this.toolStripComboBox_mode.SelectedIndex == 1)
                        rdcProcess.StartInfo.Arguments = " /edit \"" + result + "\"";
                    else
                        rdcProcess.StartInfo.Arguments = "\"" + result + "\"";
                    rdcProcess.Start();
                }
                //Thread.Sleep(2 * 1000);
                //{
                //    Process rdcProcess = new Process();
                //    rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
                //    rdcProcess.StartInfo.Arguments = "cmdkey.exe /delete:TERMSRV/" + IP;
                //    rdcProcess.Start();
                //}
            }
            catch (Exception)
            {
                MessageBox.Show("启动失败，可能没有远程文件");
            }
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            Form_Edit form = new Form_Edit();
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (Insert(form.ServerName, form.IP, form.UserName, form.Password))
                {
                    LoadData();
                }
            }
        }

        private void btn_Edit_Click(object sender, EventArgs e)
        {
            if (this.dataGridView.SelectedRows.Count <= 0)
            {
                MessageBox.Show("请选择要操作的数据行", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string ID = this.dataGridView.SelectedRows[0].Cells["ID"].Value.ToString();
            string IP = this.dataGridView.SelectedRows[0].Cells["IP"].Value.ToString();
            string Name = this.dataGridView.SelectedRows[0].Cells["Name"].Value.ToString();
            string UserName = this.dataGridView.SelectedRows[0].Cells["UserName"].Value.ToString();
            string Password = this.dataGridView.SelectedRows[0].Cells["Password"].Value.ToString();
            Form_Edit form = new Form_Edit(IP, Name, UserName, Password);
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (Update(ID, form.ServerName, form.IP, form.UserName, form.Password))
                {
                    LoadData();
                }
            }
        }
    }
}