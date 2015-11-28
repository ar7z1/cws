using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CWS.Controllers
{
    public class CsrfController : Controller
    {
        public ActionResult Level1()
        {
            return View();
        }
    }
}
