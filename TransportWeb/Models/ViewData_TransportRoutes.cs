using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportWeb.Models
{
    public class ViewData_TransportRoutes
    {
        public Transport Transport { get; set; }
        public List<ViewData_RouteStops> Routes { get; set; }
        public List<Stop> Stops { get; set; }
    }
}