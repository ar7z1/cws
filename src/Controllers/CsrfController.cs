using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CWS.Controllers
{
    public class CsrfController : Controller
    {
        private const string AuthCookieName = "AuthCookie";
        private const string LevelCompletedCookieName = "LevelCompleted";
        public const string LevelCompletedKey = "5D916DDD-AA6D-4E0C-BED8-896F7C936FE4";

        [HttpGet]
        public ActionResult Level1()
        {
            var cookie = new HttpCookie(AuthCookieName, Guid.NewGuid().ToString()) {
                HttpOnly = true,
                Path = Request.Url.AbsolutePath,
                Expires = DateTime.UtcNow.AddMonths(1)
            };
            Response.SetCookie(cookie);
            return View();
        }

        [HttpPost]
        [ActionName("Level1")]
        public ActionResult Level1Post()
        {
            if (!CheckAuth()) {
                return View("Level1AuthFailed");
            }

            if (Request.Form.Get("hacker") != "true") {
                return View("Level1WrongParameter");
            }

            if (!IsCsrf()) {
                return View("Level1NotCsrf");
            }

            var cookie = new HttpCookie(LevelCompletedCookieName, LevelCompletedKey) {
                HttpOnly = true,
                Path = Url.Action("Level2"),
                Expires = DateTime.UtcNow.AddMonths(1)
            };
            Response.SetCookie(cookie);
            return View("Level1Success");
        }

        private bool CheckAuth()
        {
            var authCookie = Request.Cookies.Get(AuthCookieName);
            if (authCookie != null && authCookie.Value != null)
                return true;
            return false;
        }

        private bool IsCsrf()
        {
            return Request.UrlReferrer == null || Request.UrlReferrer != Request.Url;
        }

        [HttpGet]
        public ActionResult Level2()
        {
            var previousLevelCompletedCookie = Request.Cookies.Get(LevelCompletedCookieName);
            if (previousLevelCompletedCookie == null || previousLevelCompletedCookie.Value != LevelCompletedKey) {
                return View("BackToLevel1");
            }

            var cookie = new HttpCookie(AuthCookieName, Guid.NewGuid().ToString()) {
                HttpOnly = true,
                Path = Request.Url.AbsolutePath,
                Expires = DateTime.UtcNow.AddMonths(1)
            };
            Response.SetCookie(cookie);

            return View();
        }
    }
}
