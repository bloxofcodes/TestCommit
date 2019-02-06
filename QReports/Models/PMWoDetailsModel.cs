using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QReports.Models
{
    public class PMWoDetailsModel
    {
        public int WoNumber { get; set; }
        public string WoDescription { get; set; }
        public string WoType { get; set; }
        public string AssetGrp { get; set; }
        public string Location { get; set; }
        public string Asset { get; set; }

        public string AssetType { get; set; }
        public string Technician { get; set; }
        public string WoStat { get; set; }
        public string ReportBy { get; set; }
        public string DateATime { get; set; }

        public string TrgtStrtDate { get; set; }
        public string CmpltStrtDate { get; set; }
        public string Comments { get; set; }

    }
}