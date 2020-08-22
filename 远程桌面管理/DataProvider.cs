using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;

namespace 远程桌面管理
{
    public class DataProvider : IDisposable
    {
        private SqlCeConnection conn;

        public DataProvider()
        {
            string Base = System.Windows.Forms.Application.StartupPath;
            string DatabaseFile = Path.Combine(Base, "Database.sdf");
            conn = new SqlCeConnection("Data Source=" + DatabaseFile + ";Persist Security Info=False;Password=0;");
            conn.Open();
        }

        public int ExecuteNonQuery(string CommadStr)
        {
            return ExecuteNonQuery(CommadStr, new Dictionary<string, object>());
        }

        public int ExecuteNonQuery(string CommadStr, Dictionary<string, object> Params)
        {
            using (SqlCeCommand com = new SqlCeCommand(CommadStr, conn))
            {
                if (Params.Count > 0)
                {
                    foreach (var item in Params)
                    {
                        com.Parameters.AddWithValue(item.Key, item.Value);
                    }
                }
                try
                {
                    return com.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public DataTable ExecuteQuery(string CommadStr)
        {
            return ExecuteQuery(CommadStr, new Dictionary<string, object>());
        }

        public DataTable ExecuteQuery(string CommadStr, Dictionary<string, object> Params)
        {
            DataTable dt = new DataTable();
            using (SqlCeCommand com = new SqlCeCommand(CommadStr, conn))
            {
                if (Params.Count > 0)
                {
                    foreach (var item in Params)
                    {
                        com.Parameters.AddWithValue(item.Key, item.Value);
                    }
                }
                SqlCeDataAdapter da;
                try
                {
                    da = new SqlCeDataAdapter(com);
                    var ds = new DataSet();
                    da.Fill(ds);
                    dt = ds.Tables[0];
                    return dt;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public object ExecuteScalar(string CommadStr)
        {
            return ExecuteScalar(CommadStr, new Dictionary<string, object>());
        }

        public object ExecuteScalar(string CommadStr, Dictionary<string, object> Params)
        {
            var table = this.ExecuteQuery(CommadStr, Params);
            if (table.Rows.Count != 1)
            {
                return null;
            }
            return table.Rows[0][0];
        }

        public void Dispose()
        {
            conn.Close();
            conn.Dispose();
        }
    }
}