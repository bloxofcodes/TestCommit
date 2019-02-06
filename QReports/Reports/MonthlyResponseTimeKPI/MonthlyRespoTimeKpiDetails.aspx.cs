using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QReports.Reports.MonthlyResponseTimeKPI
{
    public partial class MonthlyRespoTimeKpiDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie mosBreakDown = new HttpCookie("mosrespotimedetails");
            if (!IsPostBack)
            {

            }
        }
    }
}