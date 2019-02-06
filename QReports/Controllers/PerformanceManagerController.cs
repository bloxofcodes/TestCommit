using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace QReports.Controllers
{
    public class PerformanceManagerController : Controller
    {
        SAAEntities saaentities = new SAAEntities();
        // GET: PerformanceManager
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult MonthlyResponseTimeKpi()
        {



            //Add Months
            //List<string> months = new List<string>(new string[] { "January", "February", "March",
            //                                                      "April", "May", "June",
            //                                                      "July", "August", "September",
            //                                                      "October", "November", "December"});

            //List<string> assets = new List<string>(new string[] {"Asset Sample 1", "Asset Sample 2", "Asset Sample 3",
            //                                                     "Asset Sample 4", "Asset Sample 5", "Asset Sample 6"});

            //List<string> wotypes = new List<string>(new string[] {"Work Order Type 1", "Work Order Type 2", "Work Order Type 3",
            //                                                     "Work Order Type 4", "Work Order Type 5", "Work Order Type 6"});

            ////Add Years
            //List<string> years = new List<string>();

            //for(int i = 1980; i < 2100; i++)
            //{
            //    years.Add((i).ToString());
            //}

            //ViewBag.Months = months;
            //ViewBag.Years = years;
            //ViewBag.Assets = assets;
            //ViewBag.WoTypes = wotypes;
            //return View();
            List<string> woType = new List<string>();
            List<string> asstGrp = new List<string>();

            //Query Work Order Types
            var queryWoType = (from wotype in saaentities.T_TYPE_TRAV
                               where wotype.DES_TYPE_TRAV != ""
                               select new
                               {
                                   WOTYPE = wotype.DES_TYPE_TRAV
                               }).AsEnumerable()
                        .Select(c => new {
                            WOTYPE = c.WOTYPE
                        }).ToList();

            //Query Asset Groups
            var queryAssGrp = (from assgrp in saaentities.T_FAMILLE_UI
                               where assgrp.CODE_FAM != ""
                               select new
                               {
                                   ASSGRP = assgrp.CODE_FAM
                               }).AsEnumerable()
                               .Select(c => new
                               {
                                   ASSGRP = c.ASSGRP
                               }).ToList();

            foreach (var item in queryAssGrp)
                asstGrp.Add(item.ASSGRP);

            foreach (var wtitem in queryWoType)
                woType.Add(wtitem.WOTYPE);

            ViewBag.wotype = woType;
            ViewBag.assgrp = asstGrp;

            return View();
        }


        public ActionResult MonthlyResponseTimeKpiChart()
        {
            //SELECT tbl1.Months, 
            //tbl1.Years, 
            //IsNull(tbl1.WO_MONTH, 0) as 'WO_MONTH', 
            //IsNull(tbl2.CLOSED_WO_MONTH, 0) as 'WO_CLOSED',
            //IsNull((tbl1.WO_MONTH - tbl2.CLOSED_WO_MONTH), 0) as 'WO_PENDING',			--pending here is any status not 3 or 6 in CLE_ETAT_BT
            //IsNull((tbl2.CLOSED_WO_MONTH * 100 / tbl1.WO_MONTH), 0) as 'TRGT_ACHVD'
            //FROM(
            //  SELECT MONTH(DATE_DEB_PREV) as Months,
            //          YEAR(DATE_DEB_PREV) as Years,
            //          COUNT(CLE_ETAT_BT) as [WO_MONTH]

            //  FROM[SAA].[dbo].[T_BT] t

            //  WHERE CLE_ETAT_BT <> 0

            //  GROUP BY MONTH(DATE_DEB_PREV),
            //          YEAR(DATE_DEB_PREV)) tbl1
            //LEFT JOIN(
            //  SELECT MONTH(DATE_DEB_PREV) as Months,
            //          YEAR(DATE_DEB_PREV) as Years,
            //          COUNT(CLE_ETAT_BT) as CLOSED_WO_MONTH

            //  FROM[SAA].[dbo].[T_BT] t

            //  WHERE CLE_ETAT_BT = 6

            //  GROUP BY MONTH(DATE_DEB_PREV),
            //          YEAR(DATE_DEB_PREV))

            //  tbl2 ON tbl1.Months = tbl2.Months

            //  AND tbl1.Years = tbl2.Years


            Debug.WriteLine(Request.QueryString["startmonth"]);

            Debug.WriteLine(Request.QueryString["startyear"]);

            return View();
        }

        public ActionResult MonthlyCompletionTimeKpi()
        {
            //Add Months
            List<string> months = new List<string>(new string[] { "January", "February", "March",
                                                                  "April", "May", "June",
                                                                  "July", "August", "September",
                                                                  "October", "November", "December"});

            List<string> assets = new List<string>(new string[] {"Asset Sample 1", "Asset Sample 2", "Asset Sample 3",
                                                                 "Asset Sample 4", "Asset Sample 5", "Asset Sample 6"});

            List<string> wotypes = new List<string>(new string[] {"Work Order Type 1", "Work Order Type 2", "Work Order Type 3",
                                                                 "Work Order Type 4", "Work Order Type 5", "Work Order Type 6"});

            //Add Years
            List<string> years = new List<string>();

            for (int i = 1980; i < 2100; i++)
            {
                years.Add((i).ToString());
            }

            ViewBag.Months = months;
            ViewBag.Years = years;
            ViewBag.Assets = assets;
            ViewBag.WoTypes = wotypes;
            return View();
        }

        public ActionResult MonthlyCompletionTimeKpiChart()
        {
            return View();
        }


        public ActionResult ResponseTimeKpiSummary()
        {
            List<string> woType = new List<string>();
            List<string> asstGrp = new List<string>();

            //Query Work Order Types
            var queryWoType = (from wotype in saaentities.T_TYPE_TRAV
                               where wotype.DES_TYPE_TRAV != ""
                               select new
                               {
                                   WOTYPE = wotype.DES_TYPE_TRAV
                               }).AsEnumerable()
                        .Select(c => new {
                            WOTYPE = c.WOTYPE
                        }).ToList();

            //Query Asset Groups
            var queryAssGrp = (from assgrp in saaentities.T_FAMILLE_UI
                               where assgrp.CODE_FAM != ""
                               select new
                               {
                                   ASSGRP = assgrp.CODE_FAM
                               }).AsEnumerable()
                               .Select(c => new
                               {
                                   ASSGRP = c.ASSGRP
                               }).ToList();

            foreach (var item in queryAssGrp)
                asstGrp.Add(item.ASSGRP);

            foreach (var wtitem in queryWoType)
                woType.Add(wtitem.WOTYPE);

            ViewBag.wotype = woType;
            ViewBag.assgrp = asstGrp;

            return View();
        }
        public ActionResult ResponseTimeKpiSummaryChart()
        {
            return View();
        }


        public ActionResult CompletionTimeKpiSummary()
        {

            return View();
        }

        public ActionResult CompletionTimeKpiSummaryChart()
        {
            return View();
        }

        public ActionResult WorkLoadSummary()
        {
            List<string> woType = new List<string>();
            List<string> asstGrp = new List<string>();

            //Query Work Order Types
            var queryWoType = (from wotype in saaentities.T_TYPE_TRAV
                               where wotype.DES_TYPE_TRAV != ""
                               select new
                               {
                                   WOTYPE = wotype.DES_TYPE_TRAV
                               }).AsEnumerable()
                        .Select(c => new {
                            WOTYPE = c.WOTYPE
                        }).ToList();

            //Query Asset Groups
            var queryAssGrp = (from assgrp in saaentities.T_FAMILLE_UI
                               where assgrp.CODE_FAM != ""
                               select new
                               {
                                   ASSGRP = assgrp.CODE_FAM
                               }).AsEnumerable()
                               .Select(c => new
                               {
                                   ASSGRP = c.ASSGRP
                               }).ToList();

            foreach (var item in queryAssGrp)
                asstGrp.Add(item.ASSGRP);

            foreach (var wtitem in queryWoType)
                woType.Add(wtitem.WOTYPE);

            ViewBag.wotype = woType;
            ViewBag.assgrp = asstGrp;

            return View();
        }

        public ActionResult WorkLoadSummaryChart()
        {
            return View();
        }

        public ActionResult WorkOrderCompletionRate()
        {
           // HttpCookie woCmpltRateCookies = new HttpCookie("wocmpltrate");
            List<string> woType = new List<string>();

            //Query Work Order Types
            var queryWoType = (from wotype in saaentities.T_TYPE_TRAV
                               where wotype.DES_TYPE_TRAV != ""
                               select new
                               {
                                   WOTYPE = wotype.DES_TYPE_TRAV
                               }).AsEnumerable()
                        .Select(c => new {
                            WOTYPE = c.WOTYPE
                        }).ToList();

            foreach (var wtitem in queryWoType)
                woType.Add(wtitem.WOTYPE);
            ViewBag.wotype = woType;

            return View();

        }

        public ActionResult WorkOrderCompletionRateChart()
        {
            return View();
        }

        public ActionResult MonthlyBreakDownSummary()
        {
            List<string> woType = new List<string>();
            List<string> asstGrp = new List<string>();

            //Query Work Order Types
            var queryWoType = (from wotype in saaentities.T_TYPE_TRAV
                               where wotype.DES_TYPE_TRAV != ""
                               select new
                               {
                                   WOTYPE = wotype.DES_TYPE_TRAV
                               }).AsEnumerable()
                        .Select(c => new {
                            WOTYPE = c.WOTYPE
                        }).ToList();

            //Query Asset Groups
            var queryAssGrp = (from assgrp in saaentities.T_FAMILLE_UI
                               where assgrp.CODE_FAM != ""
                               select new
                               {
                                   ASSGRP = assgrp.CODE_FAM
                               }).AsEnumerable()
                               .Select(c => new
                               {
                                   ASSGRP = c.ASSGRP
                               }).ToList();

            foreach (var item in queryAssGrp)
                asstGrp.Add(item.ASSGRP);

            foreach (var wtitem in queryWoType)
                woType.Add(wtitem.WOTYPE);

            ViewBag.wotype = woType;
            ViewBag.assgrp = asstGrp;

            return View();
        }

        public ActionResult MonthlyBreakDownSummaryChart()
        {
            return View();
        }

        public ActionResult RepeatedJobPercentage()
        {
            return View();

        }

        public ActionResult RepeatedJobPercentageChart()
        {
            return View();
        }
    }
}