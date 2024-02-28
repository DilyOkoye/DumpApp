using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DumpApp.Models;

namespace DumpApp.Controllers
{
    public class ErrorController : Controller
    {
        [Helper.SessionExpire]
        public ActionResult Index()
        {
            return View();
        }
    }
}