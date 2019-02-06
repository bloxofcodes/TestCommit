using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using QReports.Models;
using System.Diagnostics;

namespace QReports.Reports.ResponseTimeKPISummary
{
    public partial class ResponseTimeKpiSumm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie respTimeKpiSummCookie = new HttpCookie("resptimekpisumm");
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


                respTimeKpiSummCookie["fromdatetime"] = dtStart;
                respTimeKpiSummCookie["todatetime"] = dtEnd;
                respTimeKpiSummCookie["wotype"] = woType;
                respTimeKpiSummCookie["asstgrp"] = asstGrp;

                respTimeKpiSummCookie.Expires = DateTime.Now.AddYears(1);

                Response.Cookies.Add(respTimeKpiSummCookie);

                rvRespoTimeKpiSumm.ProcessingMode = ProcessingMode.Local;
                rvRespoTimeKpiSumm.LocalReport.ReportPath = Server.MapPath("ResponseTimeKpiSumm.rdlc");

                SAAEntities saaentities = new SAAEntities();


                var respo = (from r in saaentities.QREPORT_COMPLETION_TIME_KPI_REF
                            where r.TYPE == "RESPONSE"
                            select new
                            {
                                MIN_SEC = r.MIN_SECS,
                                MIDDLE_SEC = r.MIDDLE_SECS,
                                MAX_SEC = r.MAX_SECS
                            }).AsEnumerable()
                            .Select(c=>new {
                                MIN = c.MIN_SEC,
                                MIDDLE = c.MIDDLE_SEC,
                                MAX = c.MAX_SEC
                            });

                string minSecs = respo.Select(p => p.MIN).SingleOrDefault().ToString();
                string middleSecs = respo.Select(p => p.MIDDLE).SingleOrDefault().ToString();
                string maxSecs = respo.Select(p => p.MAX).SingleOrDefault().ToString();

                List<ReportParameter> rParam = new List<ReportParameter>()
                {
                    new ReportParameter("ParamMinSecs", string.IsNullOrEmpty(minSecs) ? "0" : minSecs, false),
                    new ReportParameter("ParamMidSecs", string.IsNullOrEmpty(middleSecs) ? "0" : middleSecs, false),
                    new ReportParameter("ParamMaxSecs", string.IsNullOrEmpty(maxSecs) ? "0" : maxSecs, false),
                    new ReportParameter("ParamWoType", woType, true),
                    new ReportParameter("ParamPeriod", formatStart + " to " + formatEnd, true),
                    new ReportParameter("ParamAssetGrp", asstGrp, true),

                };

                rvRespoTimeKpiSumm.LocalReport.SetParameters(rParam);


                var respoQuery = (from wo in saaentities.T_BT
                              join statchange in (from todo in saaentities.T_HISTO_ETATS
                                                  where todo.OLD_ETAT == "To do"
                                                  select new
                                                  {
                                                      CLE_OBJET_HISTO = todo.CLE_OBJET_HISTO,
                                                      OLD_ETAT = todo.OLD_ETAT,
                                                      NEW_ETAT = todo.NEW_ETAT,
                                                      RESPONSE = todo.DATE_HISTO
                                                  }) on wo.NUM_BT equals statchange.CLE_OBJET_HISTO into sc
                              from statchange in sc.DefaultIfEmpty()
                              join asset in saaentities.T_UI on wo.CLE_UI equals asset.NUM_UI into a
                              from asset in a.DefaultIfEmpty()
                              join assetfamily in saaentities.T_FAMILLE_UI on asset.CLE_FAM equals assetfamily.CLE_FAM into af
                              from assetfamily in af.DefaultIfEmpty()
                              join wostat in saaentities.T_ETAT_BT on wo.CLE_ETAT_BT equals wostat.CLE_ETAT_BT into wos
                              from wostat in wos.DefaultIfEmpty()
                              join wotype in saaentities.T_TYPE_TRAV on wo.CLE_TYPE_TRAV equals wotype.NUM_TYPE_TRAV into wot
                              from wotype in wot.DefaultIfEmpty()
                              where wo.DATE_DEB_PREV >= filterStart && wo.DATE_DEB_PREV <= filterEnd
                              select new
                              {
                                  woid = wo.NUM_BT,
                                  wodesc = wo.TITRE_BT,
                                  oldstat = statchange.OLD_ETAT,
                                  newstat = statchange.NEW_ETAT,
                                  wostat = wostat.DES_ETAT_BT,
                                  wotype = wotype.DES_TYPE_TRAV,
                                  assetgrp = assetfamily.CODE_FAM,
                                  planneddt = wo.DATE_DEB_PREV,
                                  responsedt = statchange.RESPONSE,
                                  closedt = wo.DATE_CLOT

                              }).AsEnumerable()
                        .Where(p => p.wostat == "Closed")
                        .Select(c => new {
                            woid = c.woid,
                            wodesc = c.wodesc,
                            oldstat = c.oldstat,
                            newstat = c.newstat,
                            wostat = c.wostat,
                            wotype = c.wotype,
                            assetgrp=c.assetgrp,
                            planneddt = c.planneddt,
                            responsedt = c.responsedt,
                            closedt = c.closedt,
                            elapsec = (((DateTime)(c.responsedt)).Subtract((DateTime)(c.planneddt)).TotalSeconds)

                        });


                if (woType != "All")
                {
                    respoQuery = respoQuery.Where(p => p.wotype == woType);
                }

                if (asstGrp != "All")
                {
                    respoQuery = respoQuery.Where(p => p.assetgrp == asstGrp);
                }



                List<ResponseTimeKpiSummModel> respTimeKpiSumm = new List<ResponseTimeKpiSummModel>();

                foreach (var wo in respoQuery)
                {
                    //int countWorkOrderByMonthYear = (from h in newq where h.MONTHYEAR == month select h).Count();
                    //int countWorkOrderByFam = (from h in newq where h.CODE_FAM == dFam select h).Count();
                    //int countStaff = (from h in newq where h.CODE_FAM == dFam group h by new { h.PRENOM_INTERV } into g select g.Key.PRENOM_INTERV).Count();

                    //int countOutStandingWo = (from oswo in newq where oswo.WOSTAT != "Closed" && oswo.WOSTAT != "Completed" && oswo.CODE_FAM == dFam select oswo).Count();
                    //int countCompltWo = (from oswo in newq where oswo.MONTHYEAR == month && (oswo.WOSTAT == "Closed" || oswo.WOSTAT == "Completed") select oswo).Count();
                    respTimeKpiSumm.Add(new ResponseTimeKpiSummModel()
                    {

                        Description = wo.wodesc,
                        AssetGrp = wo.assetgrp,
                        Percentage = wo.elapsec.ToString()
                    });
                }

                ReportDataSource datasource = new ReportDataSource("DataSet1", respTimeKpiSumm);
                rvRespoTimeKpiSumm.LocalReport.DataSources.Clear();
                rvRespoTimeKpiSumm.Width = (Request.Browser.ScreenPixelsWidth) * 2 - 100;
                rvRespoTimeKpiSumm.Height = (Request.Browser.ScreenPixelsHeight) * 2 - 100;
                //rvWoDetails.Width = (Request.Browser.ScreenPixelsWidth) * 2 - 100;
                //rvWoDetails.Height = (Request.Browser.ScreenPixelsHeight) * 2 - 100;
                rvRespoTimeKpiSumm.LocalReport.DataSources.Add(datasource);


                //int s = queryz.Count();




            }
        }
    }
}