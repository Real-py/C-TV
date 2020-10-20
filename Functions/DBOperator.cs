using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace WinFormTV
{
    public class DBOperator
    {
        private string strConnectingStr = null;
        public SqlTransaction dbTran = null;
        private SqlConnection conn = null;

        public DBOperator()
        {
            try
            {
                
                strConnectingStr = ConfigurationManager.AppSettings["DBConnectingStr"].ToString();
                conn = new SqlConnection(strConnectingStr);
                conn.Open();
            }
            catch (Exception ex)
            {
                Log.WriteLog(LogFile.SQL, ex.Message);
            }

        }

        public bool BeginTran()
        {
            try
            {
                dbTran = conn.BeginTransaction();
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog(LogFile.SQL, ex.TargetSite.ToString());
                return false;
            }

        }

        public bool CommitTran()
        {
            try
            {
                if (dbTran != null) dbTran.Commit();
                else throw new Exception();
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog(LogFile.SQL, ex.Message + ex.TargetSite.ToString());
                return false;
            }
        }

        public bool RollBackTran()
        {
            try
            {
                if (dbTran != null) dbTran.Rollback();
                else throw new Exception();
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog(LogFile.SQL, ex.Message + ex.TargetSite.ToString());
                return false;
            }

        }

        public bool ExcuteSql(string strSql, bool isTrans = true)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                if (isTrans == true)
                {
                    cmd.Transaction = dbTran;
                }
                cmd.CommandText = strSql;
                if (cmd.ExecuteNonQuery() <= 0) throw new Exception(strSql + "-Excute fail!");
                return true;

            }
            catch (Exception ex)
            {
                Log.WriteLog(LogFile.SQL, ex.Message + ex.TargetSite.ToString());
                return false;
            }
        }

        public bool ExcuteQureyDataTable(string strSql, ref DataTable dt)
        {
            try
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(strSql, conn);
                sqlDataAdapter.Fill(dt);
                if (dt.Rows.Count <= 0) throw new Exception();
                return true;
            }
            catch (Exception ex)
            {
                //Log.WriteLog(LogFile.SQL, ex.Message + ex.TargetSite.ToString());
                return false;
            }
        }

        public object ExcuteQureyValue(string strSql)
        {
            object objReslut = null;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = strSql;
                objReslut = cmd.ExecuteScalar();
                return objReslut;
            }
            catch (Exception ex)
            {
                Log.WriteLog(LogFile.SQL, strSql + ex.TargetSite.ToString());
                return objReslut;
            }

        }

        public void CloseConn()
        {
            try
            {
                conn.Dispose();
            }
            catch (Exception ex)
            {
                Log.WriteLog(LogFile.SQL, ex.Message + ex.TargetSite.ToString());
            }
        }

    }
}