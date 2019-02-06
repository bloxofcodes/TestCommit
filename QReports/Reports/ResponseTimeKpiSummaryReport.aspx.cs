using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Data.Entity;
namespace QReports.Reports
{
    public partial class ResponseTimeKpiSummaryReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rvKpiSummary.ProcessingMode = ProcessingMode.Local;
                rvKpiSummary.LocalReport.ReportPath = Server.MapPath("ResponseTimeKpiSummaryReport.rdlc");

                SAAEntities saaentities = new SAAEntities();



                string query = "SELECT table2.[CODE_FAM],";
                query = query + "COUNT(table2.[CODE_FAM]) as SUM_BY_FAMILY ";
                query = query + "FROM[SAA].[dbo].[T_BT] table1 ";
                query = query + "LEFT JOIN(SELECT[NUM_UI]";
                query = query + ",[CLE_FAM],";
                query = query + "b.[CODE_FAM]";
                query = query + ",[DESIGNATION_UI]";
                query = query + "FROM[SAA].[dbo].[T_UI] a ";
                query = query + "INNER JOIN(SELECT[CLE_FAM] as Code,[CODE_FAM] FROM[SAA].[dbo].[T_FAMILLE_UI]) b ON a.[CLE_FAM] = b.[Code] ";
                query = query + "WHERE b.[CODE_FAM] <> '') table2 ON table1.[CLE_UI] = table2.[NUM_UI] ";
                query=query + "GROUP BY table2.[CODE_FAM]";

                //var gx = Request.QueryString["fromdatetime"];

                //var s = from asset in saaentities.T_UI
                //        join assetfamily in saaentities.T_FAMILLE_UI on asset equals assetfamily.
                //var qAssetFamily = from assetfamily in saaentities.T_FAMILLE_UI
                //            select new
                //            {
                //                Code = assetfamily.CLE_FAM,
                //                CODE_FAM = assetfamily.CODE_FAM
                //            };

                //JOIN Structure 
                var qAsset = from asset in saaentities.T_UI
                             join assetfamily in (from af in saaentities.T_FAMILLE_UI select af) on asset.CLE_FAM equals assetfamily.CLE_FAM
                             where assetfamily.CODE_FAM != ""
                             orderby asset.NUM_UI
                             select new
                             {
                                 NUM_BT=asset.NUM_UI,
                                 TITRE_BT = asset.CLE_FAM,
                                 CLE_ETAT_BT = assetfamily.CODE_FAM

                             };
                //var start = DateTime.Parse("09/01/2018 12:00:00 AM");
                //var end = DateTime.Parse("09/30/2018 12:00:00 AM");

                //var f = DateTime.ParseExact("01/09/2018 12:00:00 AM", "dd/MM/yyyy hh:mm:ss tt", null);

                var gx = Request.QueryString["fromdatetime"];
                var gz = Request.QueryString["todatetime"];

                var start = DateTime.ParseExact(gx, "dd/MM/yyyy hh:mm:ss tt", null);
                var end = DateTime.ParseExact(gz, "dd/MM/yyyy hh:mm:ss tt", null);
                //var start = DateTime.Parse(gx);
                //var end = DateTime.Parse(gz);



                var q = from table1 in saaentities.T_BT
                        join table2 in (from asset in saaentities.T_UI
                                        join assetfamily in (from af in saaentities.T_FAMILLE_UI select af) on asset.CLE_FAM equals assetfamily.CLE_FAM
                                        where assetfamily.CODE_FAM != ""
                                        select new
                                        {
                                            NUM_UI = asset.NUM_UI,
                                            CLE_FAM = asset.CLE_FAM,
                                            CODE_FAM = assetfamily.CODE_FAM,
                                            DESIGNATION_UI = asset.DESIGNATION_UI

                                        }) on table1.CLE_UI equals table2.NUM_UI
                        //where table1.DATE_DEB_REEL >= DbFunctions.TruncateTime(DateTime.ParseExact("11/1/2018 12:00 AM", "MM/dd/yyyy HH:mm tt", null)) && table1.DATE_DEB_REEL <= DbFunctions.TruncateTime(DateTime.ParseExact("11/30/2018 11:59 PM", "MM/dd/yyyy HH:mm tt", null))
                        where table1.DATE_DEB_REEL >= start && table1.DATE_DEB_REEL <= end
                        group table1 by table2.CODE_FAM into g
                        select new
                        {
                            NUM_BT = g.Key,
                            TITRE_BT = g.Count()

                        };


                var qz = from iw in (from table1 in saaentities.T_BT
                                     join table2 in (from asset in saaentities.T_UI
                                                     join assetfamily in (from af in saaentities.T_FAMILLE_UI select af) on asset.CLE_FAM equals assetfamily.CLE_FAM
                                                     where assetfamily.CODE_FAM != ""
                                                     select new
                                                     {
                                                         NUM_UI = asset.NUM_UI,
                                                         CLE_FAM = asset.CLE_FAM,
                                                         CODE_FAM = assetfamily.CODE_FAM,
                                                         DESIGNATION_UI = asset.DESIGNATION_UI

                                                     }) on table1.CLE_UI equals table2.NUM_UI
                                     //where table1.DATE_DEB_REEL >= DbFunctions.TruncateTime(DateTime.ParseExact("11/1/2018 12:00 AM", "MM/dd/yyyy HH:mm tt", null)) && table1.DATE_DEB_REEL <= DbFunctions.TruncateTime(DateTime.ParseExact("11/30/2018 11:59 PM", "MM/dd/yyyy HH:mm tt", null))
                                     where table1.DATE_DEB_REEL >= start && table1.DATE_DEB_REEL <= end
                                     group table1 by table2.CODE_FAM into g
                                     select new
                                     {
                                         NUM_BT = g.Key,
                                         TITRE_BT1 = g.Count()

                                     })
                         join ow in (from table1 in saaentities.T_BT
                                     join table2 in (from asset in saaentities.T_UI
                                                     join assetfamily in (from af in saaentities.T_FAMILLE_UI select af) on asset.CLE_FAM equals assetfamily.CLE_FAM
                                                     where assetfamily.CODE_FAM != ""
                                                     select new
                                                     {
                                                         NUM_UI = asset.NUM_UI,
                                                         CLE_FAM = asset.CLE_FAM,
                                                         CODE_FAM = assetfamily.CODE_FAM,
                                                         DESIGNATION_UI = asset.DESIGNATION_UI

                                                     }) on table1.CLE_UI equals table2.NUM_UI
                                     //where table1.DATE_DEB_REEL >= DbFunctions.TruncateTime(DateTime.ParseExact("11/1/2018 12:00 AM", "MM/dd/yyyy HH:mm tt", null)) && table1.DATE_DEB_REEL <= DbFunctions.TruncateTime(DateTime.ParseExact("11/30/2018 11:59 PM", "MM/dd/yyyy HH:mm tt", null))
                                     where table1.DATE_DEB_REEL >= start && table1.DATE_DEB_REEL <= end && table1.CLE_ETAT_BT != 3
                                     group table1 by table2.CODE_FAM into g
                                     select new
                                     {
                                         NUM_BT = g.Key,
                                         TITRE_BT2 = g.Count()

                                     })
                        on iw.NUM_BT equals ow.NUM_BT
                         select new {
                             NUM_BT=iw.NUM_BT,
                             IW = iw.TITRE_BT1,
                             OW = ow.TITRE_BT2,
                             CMPL = iw.TITRE_BT1 - ow.TITRE_BT2
                         };


                 //DateTime.ParseExact("11/30/2018 11:59 PM", "MM/dd/yyyy HH:mm tt", null);

                 //Convert.ToDateTime("11/30/2018 11:59 PM")
                 int x = qAsset.Count();
                //var testquery = saaentities.T_BT
                //    .SqlQuery(query).ToList();


                //var woList = saaentities.T_BT.ToList();

                ReportDataSource datasource = new ReportDataSource("KpiSummary", qz);
                rvKpiSummary.LocalReport.DataSources.Clear();
                rvKpiSummary.LocalReport.DataSources.Add(datasource);
            }
        }
    }
}