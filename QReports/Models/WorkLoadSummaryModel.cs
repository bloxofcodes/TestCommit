using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QReports.Models
{
    public class WorkLoadSummaryModel
    {
        public string AssetGroup { get; set; }
        public int StaffCount { get; set; }
        public int IssuedWorkOrders { get; set; }
        public int OutStandingWorkOrders { get; set; }
        public int CompletedWorkOrders { get; set; }
        public string AverageWoPerStaff { get; set; }
    }
}