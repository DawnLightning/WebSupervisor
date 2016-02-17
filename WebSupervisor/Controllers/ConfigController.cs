using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSupervisor.Controllers
{
    public class ConfigController : Controller
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