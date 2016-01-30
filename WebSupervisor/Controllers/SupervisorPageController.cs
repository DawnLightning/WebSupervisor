using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSupervisor.Controllers
{
    public class SupervisorPageController : Controller
    {
        // GET: SupervisorPage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult supervisor()
        {
            return View();
        }
    }
}