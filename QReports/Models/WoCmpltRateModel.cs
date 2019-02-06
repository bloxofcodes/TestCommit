using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QReports.Models
{
    public class WoCmpltRateModel
    {
        public string MonthYear { get; set; }
        public int IssuedWo { get; set; }
        public int CompletedWo { get; set; }

        public string PercentCmplt { get; set; }


    }
}