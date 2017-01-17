using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportWeb.Models
{
    public class Config_Route
    {
        public string Name { get; set; }
        public string TransportName { get; set; }
        public List<Config_RouteStop> Stops { get; set; }
    }
}