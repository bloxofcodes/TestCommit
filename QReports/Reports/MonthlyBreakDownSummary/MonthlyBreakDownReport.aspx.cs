using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using QReports.Models;


namespace QReports.Reports.MonthlyBreakDownSummary
{
    public partial class MonthlyBreakDownReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie mosBreakDown = new HttpCookie("mosbreakdown");
            if (!IsPostBack)
            {

                var dtStart = Request.QueryString["fromdatetime"];
                var dtEnd = Request.QueryString["todatetime"];
                var asstGrp = Request.QueryString["asstgrp"];
                var woType = Request.QueryString["wotype"];


                var filterStart = DateTime.ParseExact(dtStart, "dd/MM/yyyy hh:mm:ss tt", null);
                var filterEnd = DateTime.ParseExact(dtEnd, "dd/MM/yyyy hh:mm:ss tt", null);

                //var dtStart = "01/09/2018 12:00:00 AM";
                //var dtEnd = "30/12/2018 11:59:59 PM";
                //var filterStart = DateTime.ParseExact(dtStart, "dd/MM/yyyy hh:mm:ss tt", null);
                //var filterEnd = DateTime.ParseExact(dtEnd, "dd/MM/yyyy hh:mm:ss tt", null);

                var formatStart = filterStart.ToString("dd-MMM-yyy HH:mm:ss");
                var formatEnd = filterEnd.ToString("dd-MMM-yyy HH:mm:ss");

                rvMonthBdown.ProcessingMode = ProcessingMode.Local;
                rvMonthBdown.LocalReport.ReportPath = Server.MapPath("MonthlyBreakDown.rdlc");
                SAAEntities saaentities = new SAAEntities();

                mosBreakDown["fromdatetime"] = dtStart;
                mosBreakDown["todatetime"] = dtEnd;
                mosBreakDown["wotype"] = woType;
                mosBreakDown["asstgrp"] = asstGrp;

                mosBreakDown.Expires = DateTime.Now.AddYears(1);

                Response.Cookies.Add(mosBreakDown);

                var newq = (from workorders in saaentities.T_BT
                            join resources in (from ligne_bt in saaentities.T_LIGNE_BT
                                               where ligne_bt.TYPE_LIGNE_BT == "I"
                                               select new
                                               {
                                                   CLE_BT = ligne_bt.CLE_BT,
                                                   CLE_ELEMENT = ligne_bt.CLE_ELEMENT
                                               })
                            on workorders.NUM_BT equals resources.CLE_BT into r
                            from resources in r.DefaultIfEmpty()
                            join tech in saaentities.T_INTERV on resources.CLE_ELEMENT equals tech.NUM_INTERV into t
                            from tech in t.DefaultIfEmpty()
                            join asset in saaentities.T_UI on workorders.CLE_UI equals asset.NUM_UI into a
                            from asset in a.DefaultIfEmpty()
                            join assetfamily in saaentities.T_FAMILLE_UI on asset.CLE_FAM equals assetfamily.CLE_FAM into af
                            from assetfamily in af.DefaultIfEmpty()
                            join wostat in saaentities.T_ETAT_BT on workorders.CLE_ETAT_BT equals wostat.CLE_ETAT_BT into wos
                            from wostat in wos.DefaultIfEmpty()
                            join wotype in saaentities.T_TYPE_TRAV on workorders.CLE_TYPE_TRAV equals wotype.NUM_TYPE_TRAV into wot
                            from wotype in wot.DefaultIfEmpty()
                            where workorders.DATE_DEB_PREV >= filterStart && workorders.DATE_DEB_PREV <= filterEnd
                            orderby workorders.NUM_BT ascending
                            group workorders by new { workorders.NUM_BT, workorders.TITRE_BT, assetfamily.CODE_FAM, tech.PRENOM_INTERV, wostat.DES_ETAT_BT, wotype.DES_TYPE_TRAV, workorders.DATE_DEB_PREV } into g
                            select new
                            {

                                TITRE_BT = g.Key.TITRE_BT,
                                CODE_FAM = g.Key.CODE_FAM,
                                PRENOM_INTERV = g.Key.PRENOM_INTERV,
                                WOSTAT = g.Key.DES_ETAT_BT,
                                WOTYPE = g.Key.DES_TYPE_TRAV,
                                YEARDATE = g.Key.DATE_DEB_PREV,
                                COUNT = g.Count()
                            }).AsEnumerable()
                               .Select(c => new
                               {
                                   DESC = c.TITRE_BT,
                                   ASSFAM = c.CODE_FAM,
                                   ASST = c.PRENOM_INTERV,
                                   WOSTAT = c.WOSTAT,
                                   WOTYPE = c.WOTYPE,
                                   MONTHYEAR = ((DateTime)c.YEARDATE.Value).ToString("MMM") + " " + ((DateTime)c.YEARDATE.Value).Year,
                               });

                if (asstGrp != "All")
                {
                    newq = newq.Where(p => p.ASSFAM == asstGrp);
                }

                if (woType != "All")
                {
                    newq = newq.Where(p => p.WOTYPE == woType);
                }

                var getMonthYear = newq.Select(c => c.MONTHYEAR).Distinct();
                List<WoCmpltRateModel> workCmpltRate = new List<WoCmpltRateModel>();
                List<WorkLoadSummaryModel> woLoadSummModel = new List<WorkLoadSummaryModel>();
                foreach (var month in getMonthYear)
                {
                    int countWorkOrderByMonthYear = (from h in newq where h.MONTHYEAR == month select h).Count();
                    //int countWorkOrderByFam = (from h in newq where h.CODE_FAM == dFam select h).Count();
                    //int countStaff = (from h in newq where h.CODE_FAM == dFam group h by new { h.PRENOM_INTERV } into g select g.Key.PRENOM_INTERV).Count();

                    //int countOutStandingWo = (from oswo in newq where oswo.WOSTAT != "Closed" && oswo.WOSTAT != "Completed" && oswo.CODE_FAM == dFam select oswo).Count();
                    int countCompltWo = (from oswo in newq where oswo.MONTHYEAR == month && (oswo.WOSTAT == "Closed" || oswo.WOSTAT == "Completed") select oswo).Count();
                    workCmpltRate.Add(new WoCmpltRateModel()
                    {
                        MonthYear = month,
                        IssuedWo = countWorkOrderByMonthYear,
                        CompletedWo = countCompltWo,
                        PercentCmplt = string.Format("{0:0.00}", (decimal)countCompltWo / countWorkOrderByMonthYear * 100)

                    });
                }

                var getAsstGrp = newq.Select(c => c.ASSFAM).Distinct();
                getAsstGrp.Reverse();
                foreach (var ag in getAsstGrp)
                {
                    int countWorkOrderByFam = (from h in newq where h.ASSFAM == ag select h).Count();
                    int countOutStandingWo = (from oswo in newq where oswo.WOSTAT != "Closed" && oswo.WOSTAT != "Completed" && oswo.ASSFAM == ag select oswo).Count();
                    int countCompltWo = (from oswo in newq where oswo.ASSFAM == ag && (oswo.WOSTAT == "Closed" || oswo.WOSTAT == "Completed") select oswo).Count();
                    woLoadSummModel.Add(new WorkLoadSummaryModel()
                    {
                        AssetGroup = ag,
                        IssuedWorkOrders = countWorkOrderByFam,
                        OutStandingWorkOrders = countOutStandingWo,
                        CompletedWorkOrders = countCompltWo,

                    });
                }

                ReportDataSource datasource = new ReportDataSource("DataSet1", workCmpltRate);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", woLoadSummModel);

                rvMonthBdown.LocalReport.DataSources.Clear();
                rvMonthBdown.LocalReport.DataSources.Add(datasource);
                rvMonthBdown.LocalReport.DataSources.Add(datasource2);

                List<ReportParameter> rParam = new List<ReportParameter>()
                {
                    new ReportParameter("ParamMosBreakDownPeriod", formatStart + " to " + formatEnd, true),
                    new ReportParameter("ParamMosBreakDownWoType", Request.QueryString["wotype"], true),
                    new ReportParameter("ParamMosBreakDownAsstGrp", Request.QueryString["asstgrp"], true)


                };

                rvMonthBdown.LocalReport.SetParameters(rParam);
            }

            
        }
    }
}