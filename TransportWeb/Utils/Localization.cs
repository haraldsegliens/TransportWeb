using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportWeb.Utils
{
    public static class Localization
    {
        public static string getText(string code)
        {
            return getEnglishText(code);
        }

        public static string getText(HttpContextBase context,string code)
        {
            if (context.Session["language"] == null)
            {
                if (context.Request.Headers["Accept-Language"] != null && ((string)context.Request.Headers["Accept-Language"]).IndexOf("lv") < ((string)context.Request.Headers["Accept-Language"]).IndexOf("en"))
                {
                    context.Session["language"] = "lv";
                }
                else
                {
                    context.Session["language"] = "en";
                }
            }
            if (((string)context.Session["language"]).ToLower().Contains("lv"))
                return getLatvianText(code);
            else
                return getEnglishText(code);
        }

        private static string getLatvianText(string code)
        {
            switch(code)
            {
                case "conf-not-found":
                    return "konfigurācija nav atrasta";
                case "transport-not-found":
                    return "transporta vārds nav atrasts";
                case "stop-not-found":
                    return "pietura nav atrasta";
                case "invalid-time":
                    return "nepareizs laika formāts, jābūt hh:mm";
                case "conf-loaded":
                    return "konfigurācija ielādēta";
                case "us-created":
                    return "lietotājs izveidots";
                case "us-n-created":
                    return "lietotājs nav izveidots";
                case "us-delet":
                    return "lietotājs izdzēsts";
                case "product-name":
                    return "Transportu Web";
                case "authenticate":
                    return "Authentificēties";
                case "log-out":
                    return "Izrakstīt";
                case "login-demand":
                    return "Ielogojies savā kontā";
                case "username":
                    return "Lietotāja vārds";
                case "password":
                    return "Parole";
                case "auth-failed":
                    return "Autentifikācija nav izdevusies";
                case "unauth-access":
                    return "Neautorizēta pieeja";
                case "error":
                    return "Kļūda";
                case "stops":
                    return "Pieturas";
                case "stop":
                    return "Pietura";
                case "search":
                    return "Meklēt";
                case "route-list":
                    return "Reisu saraksts";
                case "timetable":
                    return "Laiku grafiks";
                case "transports":
                    return "Transporti";
                case "transport":
                    return "Transports";
                case "express":
                    return "Expresis";
                case "stop-list":
                    return "Pieturu saraksts";
                case "favorite-transports":
                    return "Mīļākie transporti";
                default:
                    return code;
            }
        }

        private static string getEnglishText(string code)
        {
            switch (code)
            {
                case "conf-not-found":
                    return "configuration not found";
                case "transport-not-found":
                    return "transport name not found";
                case "stop-not-found":
                    return "stop not found";
                case "invalid-time":
                    return "invalid time format, must be hh:mm";
                case "conf-loaded":
                    return "configuration loaded";
                case "us-created":
                    return "user created";
                case "us-n-created":
                    return "user not created";
                case "us-delet":
                    return "user deleted";
                case "product-name":
                    return "Transport Web";
                case "authenticate":
                    return "Authenticate";
                case "log-out":
                    return "Log out";
                case "login-demand":
                    return "Login to your account";
                case "username":
                    return "Username";
                case "password":
                    return "Password";
                case "auth-failed":
                    return "Authentication failed";
                case "unauth-access":
                    return "Unauthorized access";
                case "error":
                    return "Error";
                case "stops":
                    return "Stops";
                case "stop":
                    return "Stop";
                case "search":
                    return "Search";
                case "route-list":
                    return "Route list";
                case "timetable":
                    return "Timetable";
                case "transports":
                    return "Transports";
                case "transport":
                    return "Transport";
                case "express":
                    return "Express";
                case "stop-list":
                    return "Stop list";
                case "favorite-transports":
                    return "Favorite transports";
                default:
                    return code;
            }
        }
    }
}