using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Analytics;

namespace SitecoreOwinFederator.Controllers
{
    public class HelloWorldController : Controller
    {
        // GET: HelloWorld
        public ActionResult Index()
        {
            var ctx = Tracker.Current.Session;            
            return View();
        }
    }
}