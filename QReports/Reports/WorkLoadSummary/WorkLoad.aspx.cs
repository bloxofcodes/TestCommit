using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using Microsoft.Reporting.WebForms;
using QReports.Models;

namespace QReports.Reports.WorkLoadSummary
{
    public partial class WorkLoad : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie woLoadSummCookies = new HttpCookie("woloadsumm");
            if (!IsPostBack)
            {
                
                var dtStart = Request.QueryString["fromdatetime"];
                var dtEnd = Request.QueryString["todatetime"];
                var asstGrp = Request.QueryString["asstgrp"];
                var woType = Request.QueryString["wotype"];


                var filterStart = DateTime.ParseExact(dtStart, "dd/MM/yyyy hh:mm:ss tt", null);
                var filterEnd = DateTime.ParseExact(dtEnd, "dd/MM/yyyy hh:mm:ss tt", null);

                var formatStart = filterStart.ToString("dd-MMM-yyy HH:mm:ss");
                var formatEnd = filterEnd.ToString("dd-MMM-yyy HH:mm:ss");
                
                //Get a work orders lists
                rvWorkLoad.ProcessingMode = ProcessingMode.Local;
                rvWorkLoad.LocalReport.ReportPath = Server.MapPath("WorkLoadSumm.rdlc");
                SAAEntities saaentities = new SAAEntities();

                woLoadSummCookies["fromdatetime"] = dtStart;
                woLoadSummCookies["todatetime"] = dtEnd;
                woLoadSummCookies["asstgrp"] = asstGrp;
                woLoadSummCookies["wotype"] = woType;
                woLoadSummCookies.Expires = DateTime.Now.AddYears(1);
                Response.Cookies.Add(woLoadSummCookies);

                var newq = from workorders in saaentities.T_BT
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
                           group workorders by new { workorders.NUM_BT, workorders.TITRE_BT, assetfamily.CODE_FAM, tech.PRENOM_INTERV, wostat.DES_ETAT_BT, wotype.DES_TYPE_TRAV } into g
                           select new
                           {
                               TITRE_BT = g.Key.TITRE_BT,
                               CODE_FAM = g.Key.CODE_FAM,
                               PRENOM_INTERV = g.Key.PRENOM_INTERV,
                               WOSTAT = g.Key.DES_ETAT_BT,
                               WOTYPE = g.Key.DES_TYPE_TRAV,
                               COUNT = g.Count()
                           };


                if(asstGrp != "All")
                {
                    newq = newq.Where(p => p.CODE_FAM == asstGrp);
                }

                if(woType != "All")
                {
                    newq = newq.Where(p => p.WOTYPE == woType);
                }



                var distinctCodeFam = (from r in newq select r.CODE_FAM).Distinct();
                List<WorkLoadSummaryModel> workLoadSumm = new List<WorkLoadSummaryModel>();
                foreach (var dFam in distinctCodeFam)
                {
                    int countWorkOrderByFam = (from h in newq where h.CODE_FAM == dFam select h).Count();
                    int countStaff = (from h in newq where h.CODE_FAM == dFam group h by new { h.PRENOM_INTERV } into g select g.Key.PRENOM_INTERV).Count();

                    int countOutStandingWo = (from oswo in newq where oswo.WOSTAT != "Closed" && oswo.WOSTAT != "Completed" && oswo.CODE_FAM == dFam select oswo).Count();
                    int countCompltWo = (from oswo in newq where oswo.CODE_FAM == dFam && (oswo.WOSTAT == "Closed" || oswo.WOSTAT == "Completed") select oswo).Count();
                    workLoadSumm.Add(new WorkLoadSummaryModel()
                    {
                        AssetGroup = dFam,
                        StaffCount = countStaff,
                        IssuedWorkOrders = countWorkOrderByFam,
                        OutStandingWorkOrders = countOutStandingWo,
                        CompletedWorkOrders = countCompltWo,
                        AverageWoPerStaff = string.Format("{0:0.00}", (decimal)countWorkOrderByFam / countStaff)
                    });
                }

                ReportDataSource datasource = new ReportDataSource("DataSet1", workLoadSumm);
                rvWorkLoad.LocalReport.DataSources.Clear();
                rvWorkLoad.LocalReport.DataSources.Add(datasource);

                //ReportParameter rp = new ReportParameter("WorkLoadSummaryPeriod", formatStart + " - " + formatEnd, true);

                List<ReportParameter> rParam = new List<ReportParameter>()
                {
                    new ReportParameter("WorkLoadSummaryPeriod", formatStart + " to " + formatEnd, true),
                    new ReportParameter("WorkLoadSummaryWOType", Request.QueryString["wotype"], true),
                    new ReportParameter("WorkLoadSummaryAsstGrp", Request.QueryString["asstgrp"], true),
                };

                //ReportParameter rp2 = new ReportParameter("DateTo", this.TextBox2.Text, false);
                rvWorkLoad.LocalReport.SetParameters(rParam);





            }
        }

       


        
    }
}