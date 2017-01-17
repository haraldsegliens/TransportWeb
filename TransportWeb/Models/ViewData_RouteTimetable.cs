using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportWeb.Models
{
    public class ViewData_RouteTimetable
    {
        public Stop Stop { get; set; }
        public Route Route { get; set; }
        public Transport Transport { get; set; }
        public bool Selected { get; set; }
    }
}