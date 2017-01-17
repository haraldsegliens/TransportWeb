using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportWeb.Models
{
    public class ViewData_StopRoutes
    {
        public Stop Stop { get; set; }
        public List<ViewData_RouteTimetable> Routes { get; set; }
        public List<ViewData_Timetable> Timetable { get; set; }
    }
}