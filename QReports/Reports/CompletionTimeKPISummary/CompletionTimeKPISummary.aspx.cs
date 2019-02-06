using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QReports.Reports.CompletionTimeKPISummary
{
    public partial class CompletionTimeKPISummary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var assetGroup = Request.QueryString["asstgrp"];
                var woType = Request.QueryString["wotype"];

                var dtStart = Request.QueryString["fromdatetime"];
                var dtEnd = Request.QueryString["todatetime"];

                var filterStart = DateTime.ParseExact(dtStart, "dd/MM/yyyy hh:mm:ss tt", null);
                var filterEnd = DateTime.ParseExact(dtEnd, "dd/MM/yyyy hh:mm:ss tt", null);

                var formatStart = filterStart.ToString("dd-MMM-yyy HH:mm:ss");
                var formatEnd = filterEnd.ToString("dd-MMM-yyy HH:mm:ss");

                rvCompletionTimeKPISummary.ProcessingMode = ProcessingMode.Local;
                rvCompletionTimeKPISummary.LocalReport.ReportPath = Server.MapPath("CompletionTimeKPISummary.rdlc");

                setupChartData(assetGroup, ((DateTime)filterStart).ToString("MM/dd/yyyy hh:mm:ss tt"), ((DateTime)filterEnd).ToString("MM/dd/yyyy hh:mm:ss tt"));

                List<ReportParameter> rParam = new List<ReportParameter>()
                {
                    new ReportParameter("CompletionTimeKPISummaryPeriod"    , formatStart + " To " + formatEnd, true),
                    new ReportParameter("CompletionTimeKPISummaryWOType"    , woType.ToString(), true),
                    new ReportParameter("CompletionTimeKPISummaryAssetGroup", assetGroup.ToString(), true),
                };

                rvCompletionTimeKPISummary.LocalReport.SetParameters(rParam);
                rvCompletionTimeKPISummary.LocalReport.Refresh();
            }
        }

        public string getAssetGroupTitleQuery(string assetGroup)
        {
            string query = @"SELECT LIBELLE_FAM,'' as KPIValue FROM T_FAMILLE_UI WHERE CODE_FAM like '" + assetGroup + "'";
            return query;
        }

        public string getAssetGroupCompletionTimeKPIQuery(string assetGroup, string datestart, string dateend, int kpiLevel)
        {
            string query = @"(SELECT " + getKPIConditionLabel(kpiLevel) + ",(CAST(ISNULL(CAST(ROUND((COUNT(CASE WHEN (" + getKPICondition(kpiLevel) +
                             @" AND (TABLE_A.Status = 6 OR TABLE_A.Status = 3)) THEN 1 ELSE NULL END) * 100.0) / NULLIF(COUNT(*),0),2) as float), 0) as nvarchar)) as KPIValue
                             FROM (SELECT T_BT.NUM_BT as 'Work Order Id'
	                         ,T_BT.TITRE_BT as 'Work Order'
	                         ,T_BT.DATE_DEB_PREV as 'Date Start'
	                         ,T_BT.DATE_DERS as 'Date End'
	                         ,DATEDIFF_BIG(SECOND, T_BT.DATE_DEB_PREV, T_BT.DATE_DERS) as ElapsedSecs
	                         ,CONVERT(varchar, T_BT.DATE_DERS - T_BT.DATE_DEB_PREV, 108) as ElapsedHours
	                         ,T_BT.CLE_ETAT_BT as 'Status'
	                         FROM dbo.T_BT
	                         JOIN dbo.T_UI ON T_BT.CLE_UI = T_UI.NUM_UI
	                         JOIN dbo.T_FAMILLE_UI ON T_UI.CLE_FAM = T_FAMILLE_UI.CLE_FAM
	                         JOIN dbo.T_ETAT_BT ON T_ETAT_BT.CLE_ETAT_BT = T_BT.CLE_ETAT_BT
	                         WHERE T_BT.CLE_TYPE_TRAV = 502 
	                         AND T_FAMILLE_UI.CODE_FAM like '" + assetGroup + @"' 
	                         AND T_BT.DATE_DEB_PREV BETWEEN '" + datestart + @"' AND '" + dateend + @"' 
	                         GROUP BY T_BT.NUM_BT,T_BT.TITRE_BT,T_BT.CLE_ETAT_BT,T_BT.DATE_DEB_PREV,T_ETAT_BT.DES_ETAT_BT,T_BT.DATE_DERS) TABLE_A 
	                         JOIN dbo.QREPORT_COMPLETION_TIME_KPI_REF ON QREPORT_COMPLETION_TIME_KPI_REF.REF_ID = 1)";
            return query;
        }

        public string getKPIConditionLabel(int level)
        {
            switch (level)
            {
                case 1:
                    return "(SELECT CONCAT('Total Corrective Maintenance Workorders responded within ', MIN_TITLE) FROM dbo.QREPORT_COMPLETION_TIME_KPI_REF)";
                case 2:
                    return "(SELECT CONCAT('Total Corrective Maintenance Workorders responded within ', MIN_TITLE ,'-', MIDDLE_TITLE) FROM dbo.QREPORT_COMPLETION_TIME_KPI_REF)";
                case 3:
                    return "(SELECT CONCAT('Total Corrective Maintenance Workorders responded within ', MIDDLE_TITLE ,'-', MAX_TITLE) FROM dbo.QREPORT_COMPLETION_TIME_KPI_REF)";
                case 4:
                    return "(SELECT CONCAT('Total Corrective Maintenance Workorders responded more than ', MAX_TITLE) FROM dbo.QREPORT_COMPLETION_TIME_KPI_REF)";
            }
            return String.Empty;
        }

        public string getKPICondition(int level)
        {
            switch (level)
            {
                case 1:
                    return "TABLE_A.ElapsedSecs < MIN_SECS";
                case 2:
                    return "TABLE_A.ElapsedSecs >= MIN_SECS AND TABLE_A.ElapsedSecs <= MIDDLE_SECS";
                case 3:
                    return "TABLE_A.ElapsedSecs > MIDDLE_SECS AND TABLE_A.ElapsedSecs <= MAX_SECS";
                case 4:
                    return "TABLE_A.ElapsedSecs > MAX_SECS";
            }
            return String.Empty;
        }

        public string assembleAssetGroupDetails(String assetGroup, string dateStart, string dateEnd)
        {
            string query = getAssetGroupTitleQuery(assetGroup) +
                 " UNION ALL " +
                 getAssetGroupCompletionTimeKPIQuery(assetGroup, dateStart, dateEnd, 1) +
                 " UNION ALL " +
                 getAssetGroupCompletionTimeKPIQuery(assetGroup, dateStart, dateEnd, 2) +
                 " UNION ALL " +
                 getAssetGroupCompletionTimeKPIQuery(assetGroup, dateStart, dateEnd, 3) +
                 " UNION ALL " +
                 getAssetGroupCompletionTimeKPIQuery(assetGroup, dateStart, dateEnd, 4);
            return query;
        }

        public void setupChartData(String assetGroup, string dateStart, string dateEnd)
        {
            string masterQuery = string.Empty;
            if (assetGroup.Contains("All"))
            {
                CustomDS.QREPORTSTableAdapters.T_FAMILLE_UITableAdapter tableAdapter = new CustomDS.QREPORTSTableAdapters.T_FAMILLE_UITableAdapter();
                CustomDS.QREPORTS.T_FAMILLE_UIDataTable assetGroupList = tableAdapter.GetData();

                foreach (CustomDS.QREPORTS.T_FAMILLE_UIRow row in assetGroupList)
                {
                    if (!String.IsNullOrEmpty(row.CODE_FAM.ToString()))
                    {
                        if (!String.IsNullOrEmpty(masterQuery))
                        {
                            masterQuery = masterQuery + " UNION ALL ";
                        }
                        masterQuery = masterQuery + assembleAssetGroupDetails(row.CODE_FAM.ToString(), dateStart, dateEnd);
                    }
                }
            }
            else
            {
                masterQuery = assembleAssetGroupDetails(assetGroup, dateStart, dateEnd);
            }

            try
            {
                System.Diagnostics.Debug.WriteLine(masterQuery);
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAAConnectionString"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand(masterQuery, con);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dt.TableName = "Graph";
                    System.Diagnostics.Debug.WriteLine("DATATABLE " + dt.Rows.Count);
                    ReportDataSource rds = new ReportDataSource("CompletionTimeKPISummary", dt);
                    rvCompletionTimeKPISummary.LocalReport.DataSources.Clear();
                    rvCompletionTimeKPISummary.LocalReport.DataSources.Add(rds);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(masterQuery);
            }
        }
    }
}