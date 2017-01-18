using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;
using TransportWeb.Models;
using System;
using TransportWeb.Utils;
using System.Linq;

namespace TransportWeb.Controllers
{
    public class ConfigurationController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Load([FromBody]Config_Root config)
        {
            if (config == null)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, Localization.getText("conf-not-found"));
            }

            var db = new TransportWeb_DataModelContainer();
            if (config.Overwrite)
            {
                db.Database.ExecuteSqlCommand("DELETE FROM Timetables");
                db.Database.ExecuteSqlCommand("DELETE FROM Route_Segments");
                db.Database.ExecuteSqlCommand("DELETE FROM Routes");
                db.Database.ExecuteSqlCommand("DELETE FROM Stops");
                db.Database.ExecuteSqlCommand("DELETE FROM Transports");
            }

            var transports = new Dictionary<string, Transport>();
            foreach(var transport in config.Transports)
            {
                var newTransport = db.Transports.Add(new Transport());
                newTransport.Number = transport.Number;
                newTransport.Type = transport.Type;
                transports.Add(transport.Type + transport.Number, newTransport);
            }

            var stops = new Dictionary<string, Stop>();
            foreach (var stop in config.Stops)
            {
                var newStops = db.Stops.Add(new Stop());
                newStops.Name = stop;
                stops.Add(stop, newStops);
            }

            foreach (var route in config.Routes)
            {
                if (!transports.ContainsKey(route.TransportName))
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, route.TransportName + " : " + Localization.getText("transport-not-found"));
                }

                var newRoute = db.Routes.Add(new Route());
                newRoute.Name = route.Name;
                newRoute.Transport = transports[route.TransportName];
                int order = 0;
                foreach (var stop in route.Stops)
                {
                    if (!stops.ContainsKey(stop.StopName))
                    {
                        return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, stop.StopName + " : " + Localization.getText("stop-not-found"));
                    }
                    var newSegment = db.Route_Segments.Add(new Route_Segment());
                    newSegment.Order = order;
                    newSegment.Route = newRoute;
                    newSegment.Stop = stops[stop.StopName];

                    foreach (var time in stop.Timetable)
                    {
                        try
                        {
                            var timespan = TimeSpan.ParseExact(time, "hh':'mm", null);
                            var newTimetable = db.Timetables.Add(new Timetable());
                            newTimetable.Route = newRoute;
                            newTimetable.Stop = newSegment.Stop;
                            newTimetable.Time = (int)timespan.TotalSeconds;
                        }
                        catch
                        {
                            return Request.CreateResponse<string>(System.Net.HttpStatusCode.BadRequest, time + Localization.getText("invalid-time"));
                        }
                    }
                    order++;
                }
            }
            db.SaveChanges();
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, Localization.getText("conf-loaded"));
        }
        [HttpPost]
        public HttpResponseMessage CreateUser([FromBody]Config_User model)
        {
            if(UserSystem.Authorize(model.username, model.password, model.access))
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.Created, Localization.getText("us-created"));
            }
            else
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, Localization.getText("us-n-created"));
            }
        }
        [HttpPost]
        public HttpResponseMessage DeleteUser([FromBody]Config_User model)
        {
            UserSystem.Unauthorize(model.username);
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, Localization.getText("us-delet"));
        }

        public HttpResponseMessage GetUsers()
        {
            var db = new TransportWeb_DataModelContainer();
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, db.Users.ToList());
        }
    }
}
