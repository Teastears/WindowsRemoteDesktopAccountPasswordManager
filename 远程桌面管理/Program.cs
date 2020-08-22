using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace 远程桌面管理
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0)
            {
                ConstValue.IsShowPassWord = (args[0].ToString() == "showpassword");
            }
            Application.Run(new Form_Main());
        }
    }
}