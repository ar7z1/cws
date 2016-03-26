using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CWS.Controllers
{
    [ValidateInput(false)]
    public class XssController : Controller
    {
        LinksRepository linksRepository;

        public XssController()
        {
            linksRepository = new LinksRepository();
        }

        [HttpGet]
        public ActionResult Level1(string query)
        {
            Response.AddHeader("X-XSS-Protection", "0");
            return View((object) query);
        }

        [HttpGet]
        public ActionResult Level2()
        {
            var links = linksRepository.Get(Session.SessionID);
            return View(links);
        }

        [HttpPost]
        public ActionResult Level2(string link)
        {
            Uri uri;
            if (Uri.TryCreate(link, UriKind.Absolute, out uri))
            {
                linksRepository.Add(Session.SessionID, uri);
            }
            else
            {
                ViewBag.UriParseErrorLink = link;
            }

            var links = linksRepository.Get(Session.SessionID);
            return View(links);
        }

        [HttpGet]
        public ActionResult Level3()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Level4()
        {
            return View();
        }
    }
}