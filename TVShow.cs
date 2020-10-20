using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormTV.Functions;

namespace WinFormTV
{
    public partial class TVShow : Form
    {
        int i = 1;
        public static int inTimer = 0;
        public System.Windows.Threading.DispatcherTimer Timer1 = new System.Windows.Threading.DispatcherTimer();
        AutoSizeFormClass asc = new AutoSizeFormClass();
        DataTable dtInfo = new DataTable();
        public TVShow()
        {
            InitializeComponent();
        }
        private void TVShow_Load(object sender, EventArgs e)
        {
            asc.controllInitializeSize(this);
            this.WindowState = FormWindowState.Maximized;
            StartTimer();
        }

        private void TVShow_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }
        private void StartTimer()
        {
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            Timer1.Tick += new EventHandler(DealWithTimer);
            Timer1.Start();
        }
        private void DealWithTimer(object source, EventArgs e)
        {
            try
            {
                initDt();
                KanbanShow();
            }
            catch (Exception ex)
            {

            }
        }

        private void initDt()
        {
            DBOperator DB = new DBOperator();
            string sStnNo = ConfigurationManager.AppSettings["StnNo"].ToString();
            dtInfo.Clear();
            string strSql = string.Format(@"select * from kanbaninfoweb where status in ('0','1') and stn_buf='{0}'", sStnNo);
            DB.ExcuteQureyDataTable(strSql, ref dtInfo);
        }
        #region 查询kanbaninfo
        private void KanbanShow()
        {
            int FlashTime = Convert.ToInt32(ConfigurationManager.AppSettings["Flash"].ToString());
            DBOperator DB = new DBOperator();
            try
            {
                for (int i = 0; i < dtInfo.Rows.Count; i++)
                {
                    l1.Text = (i + 1).ToString();
                    l2.Text = dtInfo.Rows.Count.ToString();
                    txtStnNo.Text = dtInfo.Rows[i]["STN_NO"].ToString();
                    txtLocNo.Text = dtInfo.Rows[i]["LOC"].ToString();
                    txtCmdSno.Text = dtInfo.Rows[i]["CMD_SNO"].ToString();
                    txtItemName.Text = dtInfo.Rows[i]["ITEM_NAME"].ToString();
                    txtDefault.Text = "";
                    txtRqty.Text = (Convert.ToInt32(dtInfo.Rows[i]["PLT_QTY"].ToString()) - Convert.ToInt32(dtInfo.Rows[i]["TRN_QTY"].ToString())).ToString();
                    txtItemSpec.Text = dtInfo.Rows[i]["SPEC_PROP"].ToString();
                    txtMode.Text = dtInfo.Rows[i]["io_type"].ToString();
                    txtSpecProp.Text = dtInfo.Rows[i]["ITEM_SPEC"].ToString();
                    txtDrawNo.Text = dtInfo.Rows[i]["DRAW_NO"].ToString();
                    txtItemNo.Text = dtInfo.Rows[i]["ITEM_NO"].ToString();
                    tableLayoutPanel1.Rows[0].Cells["Column1"].Value = "1";
                    tableLayoutPanel1.Rows[0].Cells["Column2"].Value = dtInfo.Rows[i]["TKT_NO"].ToString();
                    tableLayoutPanel1.Rows[0].Cells["Column3"].Value = dtInfo.Rows[i]["TKT_SEP"].ToString();
                    tableLayoutPanel1.Rows[0].Cells["Column4"].Value = dtInfo.Rows[i]["TKT_QTY"].ToString();
                    tableLayoutPanel1.Rows[0].Cells["Column5"].Value = dtInfo.Rows[i]["TRN_QTY"].ToString();
                    this.Refresh();
                    if (dtInfo.Rows[i]["STATUS"].ToString() == "0")
                    {
                        string strsql = string.Format(@"update kanbaninfoweb set status='1' where STN_NO='{0}'", dtInfo.Rows[i]["STN_NO"].ToString());
                        DB.BeginTran();
                        DB.ExcuteSql(strsql);
                        DB.CommitTran();
                    }
                    Thread.Sleep(FlashTime);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                DB.CloseConn();
            }
        }
        #endregion
    }
}
