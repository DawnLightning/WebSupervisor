using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSupervisor.Controllers
{
    public class HomeController : Controller
    {
        // GET: HomePage
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult Confirm()
        {

            return PartialView();
        }
        public PartialViewResult Set()
        {
            return PartialView();
        }
        public PartialViewResult Teacher()
        {
            return PartialView();
        }
    }
}