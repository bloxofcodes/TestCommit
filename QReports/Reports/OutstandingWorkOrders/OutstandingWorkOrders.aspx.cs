using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using QReports.Models;


namespace QReports.Reports.OutstandingWorkOrders
{
    public partial class OutstandingWorkOrders : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie outWoCookies = new HttpCookie("outworks");
            if (!IsPostBack)
            {
                //form in WorkOrderDetails - WebForm
                var dtStart = Request.QueryString["fromdatetime"];
                var dtEnd = Request.QueryString["todatetime"];

                var asstGrp = Request.QueryString["asstgrp"];
                var woType = Request.QueryString["wotype"];
                var locs = Request.QueryString["location"];
                var asstsItems = Request.QueryString["asset"];

                var techRep = Request.QueryString["tech"];
                var problem = Request.QueryString["problem"];

                //dtStart = "01/09/2018 12:00:00 AM";
                //dtEnd = "30/09/2018 11:59:59 PM";


                var filterStart = DateTime.ParseExact(dtStart, "dd/MM/yyyy hh:mm:ss tt", null);
                var filterEnd = DateTime.ParseExact(dtEnd, "dd/MM/yyyy hh:mm:ss tt", null);

                var formatStart = filterStart.ToString("dd-MMM-yyy HH:mm:ss");
                var formatEnd = filterEnd.ToString("dd-MMM-yyy HH:mm:ss");

                rvOutWorkOrders.ProcessingMode = ProcessingMode.Local;
                rvOutWorkOrders.LocalReport.ReportPath = Server.MapPath("OutstandingWorkOrders.rdlc");
                SAAEntities saaentities = new SAAEntities();

                outWoCookies["fromdatetime"] = dtStart;
                outWoCookies["todatetime"] = dtEnd;
                outWoCookies["wotype"] = woType;
                outWoCookies["asstgrp"] = asstGrp;
                outWoCookies["location"] = locs;
                outWoCookies["asset"] = asstsItems;
                outWoCookies["tech"] = techRep;
                outWoCookies["problem"] = problem;
                outWoCookies.Expires = DateTime.Now.AddYears(1);
                Response.Cookies.Add(outWoCookies);

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
                            join assettype in saaentities.T_TYPE_UI on asset.CLE_TUI equals assettype.CLE_TYPE_UI into at
                            from assettype in at.DefaultIfEmpty()
                            join locations in saaentities.T_LIEU on asset.CLE_LIEU equals locations.CLE_LIEU into loc
                            from locations in loc.DefaultIfEmpty()
                            join assetfamily in saaentities.T_FAMILLE_UI on asset.CLE_FAM equals assetfamily.CLE_FAM into af
                            from assetfamily in af.DefaultIfEmpty()
                            join wostat in saaentities.T_ETAT_BT on workorders.CLE_ETAT_BT equals wostat.CLE_ETAT_BT into wos
                            from wostat in wos.DefaultIfEmpty()
                            join wotype in saaentities.T_TYPE_TRAV on workorders.CLE_TYPE_TRAV equals wotype.NUM_TYPE_TRAV into wot
                            from wotype in wot.DefaultIfEmpty()
                            join reportby in saaentities.T_DEMANDEUR on workorders.CLE_DEMANDEUR equals reportby.CLE_DEMANDEUR into rb
                            from reportby in rb.DefaultIfEmpty()
                            where workorders.DATE_DEB_PREV >= filterStart && workorders.DATE_DEB_PREV <= filterEnd
                            orderby workorders.NUM_BT ascending
                            group workorders by new
                            {
                                workorders.NUM_BT,
                                workorders.TITRE_BT,
                                assetfamily.CODE_FAM,
                                tech.PRENOM_INTERV,
                                wostat.DES_ETAT_BT,
                                wotype.DES_TYPE_TRAV,
                                workorders.DATE_DEB_PREV,
                                asset.DESIGNATION_UI,
                                assettype.LIBELLE_TUI,
                                locations.LIBELLE_LIEU,
                                reportby.NOM_DEMANDEUR
                            } into g
                            select new
                            {
                                NUM_BT = g.Key.NUM_BT,
                                TITRE_BT = g.Key.TITRE_BT,
                                DESIGNATION_UI = g.Key.DESIGNATION_UI,
                                LIBELLE_TUI = g.Key.LIBELLE_TUI,
                                LIBELLE_LIEU = g.Key.LIBELLE_LIEU,
                                CODE_FAM = g.Key.CODE_FAM,
                                PRENOM_INTERV = g.Key.PRENOM_INTERV,
                                WOSTAT = g.Key.DES_ETAT_BT,
                                WOTYPE = g.Key.DES_TYPE_TRAV,
                                YEARDATE = g.Key.DATE_DEB_PREV,
                                REPORTBY = g.Key.NOM_DEMANDEUR,
                                COUNT = g.Count()
                            }).AsEnumerable()
                            .Where(c=>c.WOSTAT != "Closed" && c.WOSTAT != "Completed")
                           .Select(c => new
                           {
                               WoNumber = c.NUM_BT,
                               WoDescription = c.TITRE_BT,
                               WoType = c.WOTYPE,
                               AssetGrp = c.CODE_FAM,
                               Asset = c.DESIGNATION_UI,
                               Location = c.LIBELLE_LIEU,
                               AssetType = c.LIBELLE_TUI,
                               Technician = c.PRENOM_INTERV,
                               WoStat = (c.WOSTAT == null ? "NULL" : c.WOSTAT),
                               ReportBy = c.REPORTBY,
                               DateATime = ((DateTime)c.YEARDATE.Value).ToString("dd") + "-" +
                                             ((DateTime)c.YEARDATE.Value).ToString("MMM") + "-" +
                                             ((DateTime)c.YEARDATE.Value).ToString("yy") + " " +
                                             ((DateTime)c.YEARDATE.Value).ToString("hh") + ":" +
                                             ((DateTime)c.YEARDATE.Value).ToString("mm") + ":" +
                                             ((DateTime)c.YEARDATE.Value).ToString("ss") + " " +
                                             ((DateTime)c.YEARDATE.Value).ToString("tt")
                           });



                if (woType != "All")
                {
                    newq = newq.Where(p => p.WoType == woType);
                }

                if (asstGrp != "All")
                {
                    newq = newq.Where(p => p.AssetGrp == asstGrp);
                }


                if (locs != "All")
                {
                    newq = newq.Where(p => p.Location == locs);
                }

                if (asstsItems != "All")
                {
                    newq = newq.Where(p => p.Asset == asstsItems);
                }

                if (techRep != "All")
                {
                    newq = newq.Where(p => p.Technician == techRep);
                }

                if (problem != "")
                {
                    newq = newq.Where(p => p.WoDescription.Contains(problem.ToLower()) || 
                                            p.WoDescription.Contains(problem.ToUpper()) || 
                                            p.WoDescription.Contains(UppercaseFirst(problem)) ||
                                            p.WoDescription.Contains(problem));
                }


                List<WoDetailsModel> workDetails = new List<WoDetailsModel>();
                foreach (var wo in newq)
                {
                    //int countWorkOrderByMonthYear = (from h in newq where h.MONTHYEAR == month select h).Count();
                    //int countWorkOrderByFam = (from h in newq where h.CODE_FAM == dFam select h).Count();
                    //int countStaff = (from h in newq where h.CODE_FAM == dFam group h by new { h.PRENOM_INTERV } into g select g.Key.PRENOM_INTERV).Count();

                    //int countOutStandingWo = (from oswo in newq where oswo.WOSTAT != "Closed" && oswo.WOSTAT != "Completed" && oswo.CODE_FAM == dFam select oswo).Count();
                    //int countCompltWo = (from oswo in newq where oswo.MONTHYEAR == month && (oswo.WOSTAT == "Closed" || oswo.WOSTAT == "Completed") select oswo).Count();
                    workDetails.Add(new WoDetailsModel()
                    {

                        WoNumber = wo.WoNumber,
                        WoDescription = wo.WoDescription,
                        WoType = wo.WoType,
                        AssetGrp = wo.AssetGrp,
                        Asset = wo.Asset,
                        Location = wo.Location,
                        AssetType = wo.AssetType,
                        Technician = wo.Technician,
                        WoStat = wo.WoStat,
                        ReportBy = wo.ReportBy,
                        DateATime = wo.DateATime

                    });
                }



                //string height = HttpContext.Current.Request.Params["clientScreenHeight"];
                //string width = HttpContext.Current.Request.Params["clientScreenWidth"];

                ReportDataSource datasource = new ReportDataSource("DataSet1", workDetails);
                rvOutWorkOrders.LocalReport.DataSources.Clear();
                rvOutWorkOrders.Width = (Request.Browser.ScreenPixelsWidth) * 2 - 100;
                rvOutWorkOrders.Height = (Request.Browser.ScreenPixelsHeight) * 2 - 100;
                //rvOutWorkOrders.Width = (Request.Browser.ScreenPixelsWidth) * 2 - 100;
                //rvOutWorkOrders.Height = (Request.Browser.ScreenPixelsHeight) * 2 - 100;
                rvOutWorkOrders.LocalReport.DataSources.Add(datasource);


                List<ReportParameter> rParam = new List<ReportParameter>()
                {
                    new ReportParameter("ParamOutWoPeriod", formatStart + " to " + formatEnd, true),
                    new ReportParameter("ParamOutWoWoType", Request.QueryString["wotype"], true),

                    new ReportParameter("ParamOutWoAsstGrp", Request.QueryString["asstgrp"], true),
                    new ReportParameter("ParamOutWoAsst", Request.QueryString["asset"], true),
                    new ReportParameter("ParamOutWoLocs", Request.QueryString["location"], true),
                    new ReportParameter("ParamOutWoTech", Request.QueryString["tech"], true),
                    new ReportParameter("ParamOutWoProb", (string.IsNullOrEmpty(Request.QueryString["problem"])) ? "All" : Request.QueryString["problem"], true)

                };

                rvOutWorkOrders.LocalReport.SetParameters(rParam);


            }
        }

        private string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}