using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSupervisor.Controllers
{
    public class ConfigPageController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Setting()
        {
            return View();
        }
    }
}