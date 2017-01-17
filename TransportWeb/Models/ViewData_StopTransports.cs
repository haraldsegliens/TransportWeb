using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportWeb.Models
{
    public class ViewData_StopTransports
    {
        public Stop Stop { get; set; }
        public List<Transport> NormalTransport { get; set; }
        public List<Transport> ExpressTransport { get; set; }
    }
}