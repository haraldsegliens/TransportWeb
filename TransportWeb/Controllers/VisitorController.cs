﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TransportWeb.Models;

namespace TransportWeb.Controllers
{
    public class VisitorController : Controller
    {
        private TransportWeb_DataModelContainer db = new TransportWeb_DataModelContainer();

        // GET: Visitor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Authenticate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authenticate()
        {
            return View();
        }

        public ActionResult Stops(string searchString)
        {
            
            var stops = from s in db.Stops select s;
            if(searchString != null && searchString.Length != 0)
            {
                stops = stops.Where(s => s.Name.Contains(searchString));
                ViewBag.searchString = searchString;
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
                stopData.ExpressTransport = transports.Where(s => s.Type == "express").ToList();
                viewData.Add(stopData);
            }

            return View(viewData);
        }

        public ActionResult Stop(int stop_id,int? route_id)
        {
            if(!route_id.HasValue)
                route_id = db.Timetables.Where(s => s.S_Id == stop_id).Select(s => s.Route.Id).First();

            var viewData = new ViewData_StopRoutes();
            viewData.Stop = db.Stops.First(s => s.Id == stop_id);
            var routes = db.Timetables.Where(s => s.Stop.Id == stop_id).Select(s => s.Route).Distinct();
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

        public ActionResult Transports()
        {
            var viewData = new List<ViewData_TransportRoute>();
            var transports = from s in db.Transports select s;
            foreach(var transport in transports)
            {
                var transportRoute = new ViewData_TransportRoute();
                transportRoute.Transport = transport;
                transportRoute.FirstRoute = transport.Routes.First();
                viewData.Add(transportRoute);
            }
            return View(viewData);
        }

        public ActionResult Transport(int transport_id, int? route_id)
        {
            if(!route_id.HasValue)
            {
                route_id = db.Transports.First(t => t.Id == transport_id).Routes.First().Id;
            }

            var viewData = new ViewData_TransportRoutes();
            viewData.Transport = db.Transports.First(t => t.Id == transport_id);
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