using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TransportWeb.Models
{
    public class Config_User
    {
        public string username { get; set; }
        public string password { get; set; }
        public string access { get; set; }
    }
}