﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSupervisor.Controllers
{
    public class ScheduleController : Controller
    {
        // GET: Schedule
        public PartialViewResult Schedule()
        {
            return PartialView();
        }
        public PartialViewResult Auto()
        {
            return PartialView();
        }
        public PartialViewResult Reference()
        {
            return PartialView();
        }
    }
}