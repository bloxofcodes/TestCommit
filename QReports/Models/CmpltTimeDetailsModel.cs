using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QReports.Models
{
    public class CmpltTimeDetailsModel
    {
        public int WoNum { get; set; }
        public string WoDesc { get; set; }
        public string WoType { get; set; }
        public string AsstGrp { get; set; }
        public string Location { get; set; }
        public string Asst { get; set; }
        public string WoPriority { get; set; }
        public string RepoBy { get; set; }
        public string Technician { get; set; }
        public string DtStarted { get; set; }
        public string DtClose { get; set; }
        public string Execution { get; set; }
        public string DtDayMonth { get; set; }
    }
}