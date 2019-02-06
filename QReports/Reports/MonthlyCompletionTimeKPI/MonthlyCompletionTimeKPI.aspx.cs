using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;

namespace QReports.Reports.MonthlyCompletionTimeKPI
{
    public partial class MonthlyCompletionTimeKPI : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("MonthlyCompletionTimeKPI");
            if (!IsPostBack)
            {
                var startMonth = Request.QueryString["startmonth"];
                var startYear = Request.QueryString["startyear"];

                var endMonth = Request.QueryString["endmonth"];
                var endYear = Request.QueryString["endyear"];

                var assetGroup = Request.QueryString["asstgrp"];
                var woType = Request.QueryString["wotype"];

                var targetResponseRate = Request.QueryString["trgtresponserate"];

                Console.WriteLine("startMonth " + startMonth);
                Console.WriteLine("startYear " + startYear);
                Console.WriteLine("endMonth " + endMonth);
                Console.WriteLine("endYear " + endYear);

                Console.WriteLine("asstGrp " + assetGroup);
                Console.WriteLine("wotype " + woType);
                Console.WriteLine("targetResponseRate " + targetResponseRate);

                rvMonthlyCompletionTime.ProcessingMode = ProcessingMode.Local;
                rvMonthlyCompletionTime.LocalReport.ReportPath = Server.MapPath("MonthlyCompletionTimeKPISummary.rdlc");

                string query = setupChartData(getMonthInt(startMonth), Convert.ToInt32(startYear), getMonthInt(endMonth), Convert.ToInt32(endYear), Convert.ToDecimal(targetResponseRate), assetGroup);

                List<ReportParameter> rParam = new List<ReportParameter>()
                {
                    new ReportParameter("MonthlyCompletionKPIPeriod", startMonth + " " +startYear + " to " + endMonth + " " + endYear, true),
                    new ReportParameter("MonthlyCompletionKPITargetRate", targetResponseRate.ToString(), true),
                    new ReportParameter("MonthlyCompletionKPIAssetGroup", assetGroup.ToString(), true),
                    new ReportParameter("MonthlyCompletionKPIWoType", woType.ToString(), true),
                    new ReportParameter("MonthlyCompletionKPIMinText", getKPILevelString(1), true),
                    new ReportParameter("MonthlyCompletionKPIMiddleText", getKPILevelString(2), true),
                    new ReportParameter("MonthlyCompletionKPIMaxText", getKPILevelString(3), true),
                    new ReportParameter("SQL", query, true)
                };

                rvMonthlyCompletionTime.LocalReport.SetParameters(rParam);
                rvMonthlyCompletionTime.LocalReport.Refresh();
            }
        }

        public string getKPILevelString(int level)
        {
            CustomDS.QREPORTSTableAdapters.QREPORT_COMPLETION_TIME_KPI_REFTableAdapter tableAdapter = new CustomDS.QREPORTSTableAdapters.QREPORT_COMPLETION_TIME_KPI_REFTableAdapter();
            CustomDS.QREPORTS.QREPORT_COMPLETION_TIME_KPI_REFDataTable levelList = tableAdapter.GetData();
            foreach (CustomDS.QREPORTS.QREPORT_COMPLETION_TIME_KPI_REFRow row in levelList)
            {
                switch (level) {
                    case 1:
                        return row.MIN_TITLE.ToString();
                    case 2:
                        return row.MIDDLE_TITLE.ToString();
                    case 3:
                        return row.MAX_TITLE.ToString();
                }
            }
            return String.Empty;
        }

        public int getMonthInt(string monthStr) {
            switch (monthStr) {
                case "January":
                    return 1;
                case "February":
                    return 2;
                case "March":
                    return 3;
                case "April":
                    return 4;
                case "May":
                    return 5;
                case "June":
                    return 6;
                case "July":
                    return 7;
                case "August":
                    return 8;
                case "September":
                    return 9;
                case "October":
                    return 10;
                case "November":
                    return 11;
                case "December":
                    return 12;
            }
            return 0;
        }

        public string getQueryforMonth(int month, int year, decimal expected, string assetGroup)
        {
            string query = @"(SELECT '" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + " " + year + @"' as 'Month', 
                                    ISNULL(CAST(ROUND((COUNT(CASE WHEN (TABLE_A.ElapsedSecs > 0 AND (TABLE_A.Status = 3 OR TABLE_A.Status = 6)) 
		                            THEN 1 ELSE NULL END) * 100.0) / NULLIF(COUNT(*),0),2) as float), 0) as 'Target',
									ISNULL(CAST(ROUND((COUNT(CASE WHEN (TABLE_A.ElapsedSecs < MIN_SECS) 
		                            THEN 1 ELSE NULL END) * 100.0) / NULLIF(COUNT(*),0),2) as float), 0) as 'Min',
	                                ISNULL(CAST(ROUND((COUNT(CASE WHEN (TABLE_A.ElapsedSecs >= MIN_SECS AND TABLE_A.ElapsedSecs <= MIDDLE_SECS) 
		                            THEN 1 ELSE NULL END) * 100.0) / NULLIF(COUNT(*),0),2) as float), 0) as 'Middle',
                                    ISNULL(CAST(ROUND((COUNT(CASE WHEN (TABLE_A.ElapsedSecs > MIDDLE_SECS AND TABLE_A.ElapsedSecs <= MAX_SECS) 
		                            THEN 1 ELSE NULL END) * 100.0) / NULLIF(COUNT(*),0),2) as float), 0)  as 'Max',
	                                ISNULL(CAST(ROUND((COUNT(CASE WHEN (TABLE_A.ElapsedSecs > MAX_SECS) 
		                            THEN 1 ELSE NULL END) * 100.0) / NULLIF(COUNT(*),0),2) as float), 0)  as 'Others'," + expected + @" as 'Expected' FROM 
                            (SELECT T_BT.NUM_BT as 'Work Order Id'
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
                              AND T_FAMILLE_UI.CODE_FAM like '" + (assetGroup.Contains("All") ? "%%" : assetGroup) + "' " 
                              + @" AND MONTH(T_BT.DATE_DEB_PREV) = " + month
                              + @" AND YEAR(T_BT.DATE_DEB_PREV) = " + year
                              + @" GROUP BY T_BT.NUM_BT,T_BT.TITRE_BT,T_BT.CLE_ETAT_BT,T_BT.DATE_DEB_PREV,T_BT.DATE_DERS,T_ETAT_BT.DES_ETAT_BT) TABLE_A 
							  JOIN dbo.QREPORT_COMPLETION_TIME_KPI_REF ON QREPORT_COMPLETION_TIME_KPI_REF.REF_ID = 1)";
            return query;
        }

        public string setupChartData(int monthStart, int yearStart, int monthEnd, int yearEnd, decimal expected, string assetGroup) {
            string Query = "";
            try {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAAConnectionString"].ToString()))
                {
                    Query = getQueryforMonth(monthStart, yearStart, expected, assetGroup) + " UNION ALL " + getQueryforMonth(monthEnd, yearEnd, expected, assetGroup);
                    SqlCommand cmd = new SqlCommand(Query, con);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dt.TableName = "Graph";
                    ReportDataSource rds = new ReportDataSource("MonthlyCompletionTimeKPI", dt);
                    rvMonthlyCompletionTime.LocalReport.DataSources.Clear();
                    rvMonthlyCompletionTime.LocalReport.DataSources.Add(rds);
                }
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(Query);
            }
            return Query;
        }
    }
}