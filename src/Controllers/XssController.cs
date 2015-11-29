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
        private static readonly List<Uri> links = new List<Uri>();

        [HttpGet]
        public ActionResult Level1(string query)
        {
            Response.AddHeader("X-XSS-Protection", "0");
            return View((object)query);
        }

        [HttpGet]
        public ActionResult Level2()
        {
            return View(links);
        }

        [HttpPost]
        public ActionResult Level2(string link)
        {
            Uri uri;
            if (Uri.TryCreate(link, UriKind.Absolute, out uri)) {
                links.Add(uri);
            } else {
                ViewBag.UriParseErrorLink = link;
            }
            return View(links);
        }
    }
}
