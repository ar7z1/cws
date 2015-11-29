using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace CWS.Controllers
{
    [ValidateInput(false)]
    public class XssController : Controller
    {
        public ActionResult Level1(string query)
        {
            Response.AddHeader("X-XSS-Protection", "0");
            return View((object)query);
        }
    }
}
