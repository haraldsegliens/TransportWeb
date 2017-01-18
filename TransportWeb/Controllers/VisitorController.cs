using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TransportWeb.Models;
using TransportWeb.Utils;

namespace TransportWeb.Controllers
{
    public class VisitorController : Controller
    {
        private TransportWeb_DataModelContainer db = new TransportWeb_DataModelContainer();

        // GET: Visitor
        public ActionResult Index()
        {
            var user = UserSystem.IsAuthenticated(this.HttpContext.Session);
            if (user == null)
                return View(new List<ViewData_TransportRoute>());
            else
            {
                var viewData = new List<ViewData_TransportRoute>();
                var counters = db.User_Transport_CounterSet.Where(x => x.User.Username == user.Username).OrderByDescending(x => x.Count);
                foreach(var counter in counters)
                {
                    var data = getTransportRoute(db.Transports.First(x=>x.Id==counter.T_Id));
                    viewData.Add(data);
                }
                return View(viewData);
            }
                
        }

        [HttpPost]
        public ActionResult Authenticate(string username,string password,string redirectUrl)
        {
            if(UserSystem.Authenticate(this.HttpContext.Session, username, password))
            {
                return Redirect(redirectUrl);
            }
            else
            {
                return RedirectToAction("Error", new { cause = Localization.getText(this.HttpContext, "auth-failed") });
            }
        }

        [HttpPost]
        public ActionResult UnAuthenticate()
        {
            UserSystem.Unauthenticate(this.HttpContext.Session);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ChangeLanguage(string language,string redirectUrl)
        {
            Session["language"] = language;
            return Redirect(redirectUrl);
        }

        public ActionResult Error(string cause)
        {
            ViewBag.error = cause;
            return View();
        }

        public ActionResult Stops(string query)
        {
            var user = UserSystem.IsAuthenticated(this.HttpContext.Session);

            var stops = from s in db.Stops select s;
            if(query != null && query.Length != 0)
            {
                stops = stops.Where(s => s.Name.Contains(query));
                ViewBag.searchString = query;
            }
            else
            {
                ViewBag.searchString = "";
            }
            var viewData = new List<ViewData_StopTransports>();
            foreach (var stop in stops)
            {
                var stopData = new ViewData_StopTransports();
                stopData.Stop = stop;
                var routes = db.Timetables.Where(x=>x.S_Id == stopData.Stop.Id).Select(s => s.Route).Distinct();
                var transports = routes.Select(s => s.Transport);
                stopData.NormalTransport = transports.Where(s => s.Type == "normal").ToList();
                if (user != null && user.Access == "express")
                {
                    stopData.ExpressTransport = transports.Where(s => s.Type == "express").ToList();
                    viewData.Add(stopData);
                }  
                else
                {
                    stopData.ExpressTransport = new List<Models.Transport>();
                    if(stopData.NormalTransport.Count != 0)
                        viewData.Add(stopData);
                }
            }

            return View(viewData);
        }

        public ActionResult Stop(int stop_id,int? route_id)
        {
            var user = UserSystem.IsAuthenticated(this.HttpContext.Session);
            if (!route_id.HasValue)
            {
                if(user == null || user.Access != "express")
                    route_id = db.Timetables.Where(s => s.S_Id == stop_id).Where(s => s.Route.Transport.Type != "express").Select(s => s.Route.Id).First();
                else
                    route_id = db.Timetables.Where(s => s.S_Id == stop_id).Select(s => s.Route.Id).First();
            }
            var viewData = new ViewData_StopRoutes();
            viewData.Stop = db.Stops.First(s => s.Id == stop_id);
            var routes = (user == null || user.Access != "express") ? db.Timetables.Where(s => s.Stop.Id == stop_id).Where(s => s.Route.Transport.Type != "express").Select(s => s.Route).Distinct() :
                db.Timetables.Where(s => s.Stop.Id == stop_id).Select(s => s.Route).Distinct();
            viewData.Routes = new List<ViewData_RouteTimetable>();
            foreach (var r in routes)
            {
                var routeTimetable = new ViewData_RouteTimetable();
                routeTimetable.Stop = viewData.Stop;
                routeTimetable.Route = r;
                routeTimetable.Transport = r.Transport;
                routeTimetable.Selected = (r.Id == route_id);
                if(routeTimetable.Selected)
                {
                    var timetables = db.Timetables.Where(s => s.Route.Id == r.Id && s.Stop.Id == stop_id);
                    viewData.Timetable = new List<ViewData_Timetable>();
                    foreach (var t in timetables)
                    {
                        var timetable = GetVDTimetable(t);
                        viewData.Timetable.Add(timetable);
                    }
                }
                viewData.Routes.Add(routeTimetable);
            }

            
            return View(viewData);
        }

        private ViewData_TransportRoute getTransportRoute(Transport transport)
        {
            var transportRoute = new ViewData_TransportRoute();
            transportRoute.Transport = transport;
            transportRoute.FirstRoute = transport.Routes.First();
            return transportRoute;
        }

        public ActionResult Transports()
        {
            var user = UserSystem.IsAuthenticated(this.HttpContext.Session);
            var viewData = new List<ViewData_TransportRoute>();
            var transports = from s in db.Transports select s;
            if (user == null || user.Access != "express")
                transports = transports.Where(t => t.Type != "express");
            foreach(var transport in transports)
            {
                var transportRoute = getTransportRoute(transport);
                viewData.Add(transportRoute);
            }
            return View(viewData);
        }

        public ActionResult Transport(int transport_id, int? route_id)
        {
            var user = UserSystem.IsAuthenticated(this.HttpContext.Session);
            if (!route_id.HasValue)
            {
                route_id = db.Transports.First(t => t.Id == transport_id).Routes.First().Id;
            }

            var viewData = new ViewData_TransportRoutes();
            viewData.Transport = db.Transports.First(t => t.Id == transport_id);

            if((user == null || user.Access != "express") && viewData.Transport.Type == "express")
            {
                return RedirectToAction("Error", new { cause = Localization.getText(this.HttpContext, "unauth-access") });
            }

            var counter = db.User_Transport_CounterSet.FirstOrDefault(x => x.User.Username == user.Username && x.T_Id == transport_id);
            if (counter == null)
            {
                counter = db.User_Transport_CounterSet.Add(new User_Transport_Counter());
                counter.User = db.Users.First(x => x.Username == user.Username);
                counter.Count = 1;
                counter.T_Id = transport_id;
            }
            else
                counter.Count += 1;
            db.SaveChanges();

            viewData.Routes = new List<ViewData_RouteStops>();
            foreach (var r in viewData.Transport.Routes)
            {
                var routeData = new ViewData_RouteStops();
                routeData.Route = r;
                routeData.Selected = (r.Id == route_id);
                if(routeData.Selected)
                {
                    viewData.Stops = r.Segments.OrderBy(s => s.Order).Select(s => s.Stop).ToList();
                }
                viewData.Routes.Add(routeData);
            }
            return View(viewData);
        }

        DateTime ParseDateTimeFromSeconds(int time_seconds)
        {
            int hours = time_seconds / 3600;
            int minutes = (time_seconds - 3600 * hours) / 60;
            int seconds = time_seconds - 3600 * hours - 60 * minutes;
            return new DateTime(2000, 1, 1, hours, minutes, seconds);
        }

        ViewData_Timetable GetVDTimetable(Timetable t)
        {
            var data = new ViewData_Timetable();
            data.Time = ParseDateTimeFromSeconds(t.Time);
            return data;
        }
    }
}