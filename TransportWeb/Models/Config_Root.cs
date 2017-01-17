using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportWeb.Models
{
    public class Config_Root
    {
        public bool Overwrite { get; set; }
        public List<Config_Transport> Transports { get; set; }
        public List<string> Stops { get; set; }
        public List<Config_Route> Routes { get; set; }
    }
}