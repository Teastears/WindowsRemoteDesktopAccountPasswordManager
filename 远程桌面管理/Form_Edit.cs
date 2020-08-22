using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 远程桌面管理
{
    public partial class Form_Edit : Form
    {
        public string IP;
        public string ServerName;
        public string UserName;
        public string Password;

        public Form_Edit()
        {
            InitializeComponent();
        }

        public Form_Edit(string IP, string Name, string UserName, string Password)
            : this()
        {
            this.textBox_IP.Text = IP;
            this.textBox_Name.Text = Name;
            this.textBox_UserName.Text = UserName;
            this.textBox_Password.Text = Password;
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            IP = this.textBox_IP.Text;
            ServerName = this.textBox_Name.Text;
            UserName = this.textBox_UserName.Text;
            Password = this.textBox_Password.Text;
            if (string.IsNullOrWhiteSpace(IP))
            {
                MessageBox.Show("IP不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(UserName))
            {
                MessageBox.Show("用户名不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("密码不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(ServerName))
            {
                ServerName = "未命名";
            }
            this.DialogResult = DialogResult.OK;
        }

        private void Form_Edit_Load(object sender, EventArgs e)
        {
            this.textBox_Password.UseSystemPasswordChar = !ConstValue.IsShowPassWord;
        }
    }
}