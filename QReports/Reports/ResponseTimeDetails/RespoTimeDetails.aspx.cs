using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QReports.Reports.ResponseTimeDetails
{
    public partial class RespoTimeDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie respoTimeDetailsCookies = new HttpCookie("respotimedetails");
            if (!IsPostBack)
            {
                //form in WorkOrderDetails - WebForm
                var dtStart = Request.QueryString["fromdatetime"];
                var dtEnd = Request.QueryString["todatetime"];
                var asstGrp = Request.QueryString["asstgrp"];
                var woType = Request.QueryString["wotype"];
                //var woStat = Request.QueryString["wostat"];
                var locs = Request.QueryString["location"];
                //var asstsItems = Request.QueryString["asset"];

                //var asstType = Request.QueryString["assttyp"];
                var techRep = Request.QueryString["tech"];
                //var repoBy = Request.QueryString["reportby"];

                var problem = Request.QueryString["problem"];

                var priority = Request.QueryString["priority"];

                //Response.Cookies["fromdatetime"].Value = dtStart;
                //Response.Cookies["todatetime"].Value = dtEnd;
                //Response.Cookies["asstgrp"].Value = asstGrp;
                //Response.Cookies["wotype"].Value = woType;

                respoTimeDetailsCookies["fromdatetime"] = dtStart;
                respoTimeDetailsCookies["todatetime"] = dtEnd;
                respoTimeDetailsCookies["asstgrp"] = asstGrp;
                respoTimeDetailsCookies["wotype"] = woType;

                //cmpltTimeDetailsCookies["wostat"] = woStat;
                respoTimeDetailsCookies["location"] = locs;
                //cmpltTimeDetailsCookies["asset"] = asstsItems;

                //cmpltTimeDetailsCookies["assttyp"] = asstType;
                respoTimeDetailsCookies["tech"] = techRep;
                //cmpltTimeDetailsCookies["reportby"] = repoBy;

                respoTimeDetailsCookies["problem"] = problem;

                respoTimeDetailsCookies["priority"] = priority;
                //woDetailsCookies["wotype"] = woType;
                //dtStart = "01/09/2018 12:00:00 AM";
                //dtEnd = "30/09/2018 11:59:59 PM";
                //var asstgrp = Request.Cookies("wodetails")("asstgrp");
                //if (string.IsNullOrEmpty())
                //{
                //    woDetailsCookies["asstgrp"] = asstGrp;
                //}

                //Debug.WriteLine(woDetailsCookies["asstgrp"]);
                respoTimeDetailsCookies.Expires = DateTime.Now.AddYears(1);
                Response.Cookies.Add(respoTimeDetailsCookies);
            }
        }
    }
}