using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportWeb.Models
{
    public class Config_RouteStop
    {
        public string StopName { get; set; }
        public List<string> Timetable { get; set; }
    }
}